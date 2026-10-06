#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
/// Replaces the Scale tool's handles with face "extrusion" handles.
/// Dragging a face moves it outward/inward in fixed increments (world units)
/// while the opposite face stays put.
/// Put this file in any folder named "Editor" (e.g. Assets/Editor/).
/// Select the Scale tool (R) to use it. Toggle via Tools > Extrude Scale Handles.
/// </summary>
[InitializeOnLoad]
public static class ExtrudeScaleTool
{
    private const string EnabledKey = "ExtrudeScale.Enabled";
    private const string StepKey = "ExtrudeScale.Step";
    private const string MenuPath = "Tools/Extrude Scale Handles";
    private const float MinSize = 0.001f;

    private static bool weHidTools;

    private static bool Enabled
    {
        get => EditorPrefs.GetBool(EnabledKey, true);
        set => EditorPrefs.SetBool(EnabledKey, value);
    }

    // Increment in world units. 0 = free (no snapping).
    private static float Step
    {
        get => EditorPrefs.GetFloat(StepKey, 0.5f);
        set => EditorPrefs.SetFloat(StepKey, Mathf.Max(0f, value));
    }

    static ExtrudeScaleTool()
    {
        SceneView.duringSceneGui += OnSceneGUI;
        Tools.hidden = false;
    }

    [MenuItem(MenuPath)]
    private static void ToggleEnabled()
    {
        Enabled = !Enabled;
        if (!Enabled) ReleaseTools();
        SceneView.RepaintAll();
    }

    [MenuItem(MenuPath, true)]
    private static bool ToggleEnabledValidate()
    {
        Menu.SetChecked(MenuPath, Enabled);
        return true;
    }

    private static void ReleaseTools()
    {
        if (!weHidTools) return;
        Tools.hidden = false;
        weHidTools = false;
    }

    private static void OnSceneGUI(SceneView sceneView)
    {
        Transform active = Selection.activeTransform;
        MeshFilter mf = active != null ? active.GetComponent<MeshFilter>() : null;
        bool useTool = Enabled && Tools.current == Tool.Scale && mf != null && mf.sharedMesh != null;

        if (!useTool)
        {
            ReleaseTools();
            return;
        }

        // Hide the default scale gizmo while ours is active.
        Tools.hidden = true;
        weHidTools = true;

        DrawStepPanel();

        Bounds b = mf.sharedMesh.bounds;
        Vector3 centerWorld = active.TransformPoint(b.center);
        Color oldColor = Handles.color;

        for (int axis = 0; axis < 3; axis++)
        {
            for (int s = 0; s < 2; s++)
            {
                bool positiveFace = s == 0;

                Vector3 localFace = b.center;
                localFace[axis] = positiveFace ? b.max[axis] : b.min[axis];
                Vector3 facePos = active.TransformPoint(localFace);

                // Outward direction in world space (robust to negative scale).
                Vector3 dir = facePos - centerWorld;
                if (dir.sqrMagnitude < 1e-10f) continue;
                dir.Normalize();

                Handles.color = AxisColor(axis);
                float size = HandleUtility.GetHandleSize(facePos) * 0.1f;

                EditorGUI.BeginChangeCheck();
                Vector3 result = Handles.Slider(facePos, dir, size, Handles.CubeHandleCap, Step);
                if (EditorGUI.EndChangeCheck())
                {
                    float delta = Vector3.Dot(result - facePos, dir);
                    ExtrudeSelection(axis, positiveFace, delta);
                }
            }
        }

        Handles.color = oldColor;
    }

    private static void DrawStepPanel()
    {
        Handles.BeginGUI();
        GUILayout.BeginArea(new Rect(10, 30, 180, 38), EditorStyles.helpBox);
        float oldLabelWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 105;
        EditorGUI.BeginChangeCheck();
        float step = EditorGUILayout.FloatField(new GUIContent("Extrude Step", "World units per increment. 0 = free."), Step);
        if (EditorGUI.EndChangeCheck()) Step = step;
        EditorGUIUtility.labelWidth = oldLabelWidth;
        GUILayout.EndArea();
        Handles.EndGUI();
    }

    private static Color AxisColor(int axis)
    {
        switch (axis)
        {
            case 0: return Handles.xAxisColor;
            case 1: return Handles.yAxisColor;
            default: return Handles.zAxisColor;
        }
    }

    /// <summary>
    /// Moves one face of every selected mesh object by 'delta' world units
    /// (positive = outward), keeping the opposite face fixed.
    /// </summary>
    private static void ExtrudeSelection(int axis, bool positiveFace, float delta)
    {
        if (Mathf.Abs(delta) < 1e-6f) return;

        foreach (Transform t in Selection.transforms)
        {
            MeshFilter mf = t.GetComponent<MeshFilter>();
            if (mf == null || mf.sharedMesh == null) continue;

            Bounds b = mf.sharedMesh.bounds;
            float worldSize = b.size[axis] * Mathf.Abs(t.lossyScale[axis]);
            if (worldSize < 1e-5f) continue;

            float newSize = worldSize + delta;
            if (newSize < MinSize) continue;

            // Anchor = center of the face opposite the one being moved.
            Vector3 anchorLocal = b.center;
            anchorLocal[axis] = positiveFace ? b.min[axis] : b.max[axis];

            Undo.RecordObject(t, "Extrude Scale");

            Vector3 anchorBefore = t.TransformPoint(anchorLocal);

            Vector3 scale = t.localScale;
            scale[axis] *= newSize / worldSize;
            t.localScale = scale;

            t.position += anchorBefore - t.TransformPoint(anchorLocal);
        }
    }
}
#endif