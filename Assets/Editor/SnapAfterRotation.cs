#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Snaps objects back onto the grid after you rotate them.
/// Rotating around the pivot changes where an object's bounding box sits (e.g. a 1x2 block
/// rotated 90 degrees shifts its corner by half a unit), which can push it off the grid.
/// When a rotation finishes, any object that was on the grid beforehand is moved back onto it.
/// Objects that were already off the grid are left alone.
///
/// Uses the same grid logic as AutoSnapNewObjects.cs (keep both files in your Editor folder),
/// including the "Auto Snap By Bounds" setting and Edit > Snap Settings increments.
/// Toggle via Tools > Snap After Rotation.
/// </summary>
[InitializeOnLoad]
public static class SnapAfterRotation
{
    private const string EnabledKey = "SnapAfterRotation.Enabled";
    private const string MenuPath = "Tools/Snap After Rotation";
    private const float RotationEpsilonDegrees = 0.01f;
    private const float OnGridTolerance = 1e-3f;

    private class State
    {
        public Quaternion rotation;
        public bool wasOnGrid;   // on the grid before the current rotation started
        public bool pending;     // rotated, waiting for the drag to finish
    }

    private static readonly Dictionary<int, State> states = new Dictionary<int, State>();
    private static int pendingUndoGroup = -1;

    private static bool Enabled
    {
        get => EditorPrefs.GetBool(EnabledKey, true);
        set => EditorPrefs.SetBool(EnabledKey, value);
    }

    static SnapAfterRotation()
    {
        EditorApplication.update += OnUpdate;
        Selection.selectionChanged += Reset;
    }

    private static void Reset()
    {
        states.Clear();
        pendingUndoGroup = -1;
    }

    [MenuItem(MenuPath)]
    private static void ToggleEnabled()
    {
        Enabled = !Enabled;
        Reset();
    }

    [MenuItem(MenuPath, true)]
    private static bool ToggleEnabledValidate()
    {
        Menu.SetChecked(MenuPath, Enabled);
        return true;
    }

    private static bool IsOnGrid(Transform t)
    {
        Vector3 o = AutoSnapNewObjects.GetSnapOffset(t);
        return Mathf.Abs(o.x) < OnGridTolerance &&
               Mathf.Abs(o.y) < OnGridTolerance &&
               Mathf.Abs(o.z) < OnGridTolerance;
    }

    private static void OnUpdate()
    {
        if (!Enabled) return;

        Transform[] selected = Selection.transforms;
        bool anyPending = false;

        foreach (Transform t in selected)
        {
            int id = t.GetInstanceID();

            if (!states.TryGetValue(id, out State s))
            {
                states[id] = new State { rotation = t.rotation, wasOnGrid = IsOnGrid(t) };
                continue;
            }

            if (Quaternion.Angle(s.rotation, t.rotation) > RotationEpsilonDegrees)
            {
                // Rotation changed this frame. Keep the "was on grid" value from before it started.
                if (!s.pending)
                {
                    s.pending = true;
                    if (pendingUndoGroup < 0) pendingUndoGroup = Undo.GetCurrentGroup();
                }
                s.rotation = t.rotation;
            }
            else if (!s.pending)
            {
                s.wasOnGrid = IsOnGrid(t);
            }

            anyPending |= s.pending;
        }

        // Wait until the rotate handle / field drag is released so we don't fight the user.
        if (!anyPending || GUIUtility.hotControl != 0) return;

        bool moved = false;
        foreach (Transform t in selected)
        {
            if (!states.TryGetValue(t.GetInstanceID(), out State s) || !s.pending) continue;
            s.pending = false;

            if (!s.wasOnGrid) continue;

            Vector3 offset = AutoSnapNewObjects.GetSnapOffset(t);
            if (offset.sqrMagnitude < 1e-12f) continue;

            Undo.RecordObject(t, "Snap After Rotation");
            t.position += offset;
            moved = true;
        }

        // Fold the snap into the same undo step as the rotation.
        if (moved && pendingUndoGroup >= 0)
            Undo.CollapseUndoOperations(pendingUndoGroup);

        pendingUndoGroup = -1;
    }
}
#endif
