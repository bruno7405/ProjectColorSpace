#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// Automatically snaps objects to the world grid when you drag them from the
/// Project window (prefabs, models, etc.) into the Scene view or Hierarchy.
/// Works in regular scenes and in Prefab Mode.
/// Uses the increments from Edit > Snap Settings (Move X/Y/Z).
/// Put this file in any folder named "Editor" (e.g. Assets/Editor/).
/// Toggle via Tools > Auto Snap New Objects.
/// Requires Unity 2021.2 or newer.
/// </summary>
[InitializeOnLoad]
public static class AutoSnapNewObjects
{
    private const string EnabledKey = "AutoSnapNewObjects.Enabled";
    private const string MenuPath = "Tools/Auto Snap New Objects";
    private const string BoundsKey = "AutoSnapNewObjects.SnapByBounds";
    private const string BoundsMenuPath = "Tools/Auto Snap By Bounds (not Pivot)";

    // Object-creation events can land a moment after the drag state clears,
    // so a creation counts as "from a drag" if a drag was seen very recently.
    private const double DragGraceSeconds = 1.0;

    // instanceId -> undo group at creation time
    private static readonly Dictionary<int, int> pending = new Dictionary<int, int>();
    private static double lastAssetDragTime = double.NegativeInfinity;
    private static bool assetDragActive;

    private static bool Enabled
    {
        get => EditorPrefs.GetBool(EnabledKey, true);
        set => EditorPrefs.SetBool(EnabledKey, value);
    }

    static AutoSnapNewObjects()
    {
        ObjectChangeEvents.changesPublished += OnChangesPublished;
        EditorApplication.update += OnUpdate;
    }

    [MenuItem(MenuPath)]
    private static void ToggleEnabled() => Enabled = !Enabled;

    [MenuItem(MenuPath, true)]
    private static bool ToggleEnabledValidate()
    {
        Menu.SetChecked(MenuPath, Enabled);
        return true;
    }

    // When true, the object's bounding box (its min corner) is snapped to the grid instead
    // of its pivot. This keeps faces on the grid even when the pivot sits at a half step
    // (e.g. a 0.5-wide object centered on a 0.5 grid line has its faces off-grid by half a step).
    private static bool SnapByBounds
    {
        get => EditorPrefs.GetBool(BoundsKey, true);
        set => EditorPrefs.SetBool(BoundsKey, value);
    }

    [MenuItem(BoundsMenuPath)]
    private static void ToggleSnapByBounds() => SnapByBounds = !SnapByBounds;

    [MenuItem(BoundsMenuPath, true)]
    private static bool ToggleSnapByBoundsValidate()
    {
        Menu.SetChecked(BoundsMenuPath, SnapByBounds);
        return true;
    }

    // Fixes objects that are already half a step off the grid.
    [MenuItem("Tools/Snap Selection To Grid")]
    private static void SnapSelectionToGrid()
    {
        Transform[] selected = Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.Editable);
        if (selected.Length == 0) return;

        Undo.RecordObjects(selected, "Snap Selection To Grid");
        foreach (Transform t in selected)
            t.position += GetSnapOffset(t);
    }

    private static void OnChangesPublished(ref ObjectChangeEventStream stream)
    {
        if (!Enabled) return;

        bool fromAssetDrag = assetDragActive ||
                             EditorApplication.timeSinceStartup - lastAssetDragTime < DragGraceSeconds;
        if (!fromAssetDrag) return;

        for (int i = 0; i < stream.length; i++)
        {
            if (stream.GetEventType(i) != ObjectChangeKind.CreateGameObjectHierarchy) continue;

            stream.GetCreateGameObjectHierarchyEvent(i, out CreateGameObjectHierarchyEventArgs e);
            pending[e.instanceId] = Undo.GetCurrentGroup();
        }
    }

    private static void OnUpdate()
    {
        UpdateDragState();

        // Wait until the drag is finished so we don't fight the drag preview.
        if (pending.Count == 0 || assetDragActive) return;

        foreach (KeyValuePair<int, int> kv in pending)
            SnapObject(kv.Key, kv.Value);

        pending.Clear();
    }

    private static void UpdateDragState()
    {
        bool active = false;
        Object[] refs = DragAndDrop.objectReferences;
        if (refs != null)
        {
            foreach (Object o in refs)
            {
                // Persistent = an asset from the Project window (not a scene object).
                if (o != null && EditorUtility.IsPersistent(o)) { active = true; break; }
            }
        }

        if (active) lastAssetDragTime = EditorApplication.timeSinceStartup;
        assetDragActive = active;
    }

    private static void SnapObject(int instanceId, int undoGroup)
    {
        GameObject go = EditorUtility.InstanceIDToObject(instanceId) as GameObject;
        if (go == null || EditorUtility.IsPersistent(go) || !go.scene.IsValid()) return;

        if (go.transform is RectTransform) return;

        PrefabStage stage = PrefabStageUtility.GetCurrentPrefabStage();
        bool inCurrentStage = stage != null && go.scene == stage.scene;

        // Never move the prefab's own root.
        if (inCurrentStage && go == stage.prefabContentsRoot) return;

        // Snap top-level objects, plus (in Prefab Mode) objects dropped directly under the
        // prefab root, since that's where Unity parents them by default. Objects dropped
        // onto any other parent keep their offset relative to it.
        bool topLevel = go.transform.parent == null;
        bool directChildOfPrefabRoot = inCurrentStage && go.transform.parent == stage.prefabContentsRoot.transform;
        if (!topLevel && !directChildOfPrefabRoot) return;

        Vector3 offset = GetSnapOffset(go.transform);
        if (offset.sqrMagnitude < 1e-12f) return;

        Undo.RecordObject(go.transform, "Snap New Object to Grid");
        go.transform.position += offset;

        // Fold the snap into the same undo step as the object's creation.
        Undo.CollapseUndoOperations(undoGroup);
    }

    // World-space movement needed to put the object on the grid.
    internal static Vector3 GetSnapOffset(Transform t)
    {
        Vector3 step = EditorSnapSettings.move;

        // Reference point: the bounds' min corner (faces on grid) or, as a fallback, the pivot.
        Vector3 reference = t.position;
        if (SnapByBounds && TryGetWorldBounds(t, out Bounds b))
            reference = b.min;

        return new Vector3(
            SnapAxis(reference.x, step.x) - reference.x,
            SnapAxis(reference.y, step.y) - reference.y,
            SnapAxis(reference.z, step.z) - reference.z);
    }

    private static bool TryGetWorldBounds(Transform t, out Bounds bounds)
    {
        bounds = default;
        bool found = false;

        foreach (Renderer r in t.GetComponentsInChildren<Renderer>())
        {
            if (!(r is MeshRenderer || r is SkinnedMeshRenderer || r is SpriteRenderer)) continue;

            if (!found) { bounds = r.bounds; found = true; }
            else bounds.Encapsulate(r.bounds);
        }

        return found;
    }

    private static float SnapAxis(float value, float step)
    {
        return step > 1e-6f ? Mathf.Round(value / step) * step : value;
    }
}
#endif
