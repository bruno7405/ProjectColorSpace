#if UNITY_EDITOR
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditor.Splines;
using UnityEngine;
using UnityEngine.Splines;

/// <summary>
/// A spline tool that shows a combined Move + Rotate handle on the selected knot.
/// Requires the Splines package (com.unity.splines 2.x).
/// Put this file in any folder named "Editor" (e.g. Assets/Editor/).
///
/// Usage: select a GameObject with a SplineContainer, switch the tool context to "Spline",
/// click a knot to select it, then pick this tool from the custom tool button in the
/// Tools overlay ("Spline Knot Move + Rotate").
///
/// If several knots are selected, the handle sits on the active (last selected) knot.
/// Moving applies to every selected knot; rotating turns each selected knot in place.
/// </summary>
[EditorTool("Spline Knot Move + Rotate", typeof(ISplineContainer), typeof(SplineToolContext))]
public class SplineKnotTransformTool : EditorTool
{
    private static readonly List<SplineInfo> splineInfos = new List<SplineInfo>();
    private static readonly List<SelectableKnot> selectedKnots = new List<SelectableKnot>();
    private static readonly List<Object> undoTargets = new List<Object>();

    private GUIContent icon;

    public override GUIContent toolbarIcon
    {
        get
        {
            if (icon == null)
            {
                Texture image = EditorGUIUtility.IconContent("TransformTool").image;
                if (image == null) image = EditorGUIUtility.IconContent("MoveTool").image;
                icon = new GUIContent(image, "Spline Knot Move + Rotate");
            }
            return icon;
        }
    }

    public override void OnToolGUI(EditorWindow window)
    {
        if (!(window is SceneView)) return;

        // Collect every spline in the targeted containers.
        splineInfos.Clear();
        foreach (Object target in targets)
        {
            if (!(target is ISplineContainer container)) continue;

            IReadOnlyList<Spline> splines = container.Splines;
            for (int i = 0; i < splines.Count; i++)
                splineInfos.Add(new SplineInfo(container, i));
        }

        // Selected knots (tangents are ignored by this tool).
        selectedKnots.Clear();
        SplineSelection.GetElements(splineInfos, selectedKnots);
        if (selectedKnots.Count == 0) return;

        SelectableKnot activeKnot = selectedKnots[selectedKnots.Count - 1];
        if (SplineSelection.GetActiveElement(splineInfos) is SelectableKnot active)
            activeKnot = active;
        if (!activeKnot.IsValid()) return;

        Vector3 position = activeKnot.Position;
        Quaternion knotRotation = ToQuaternion(activeKnot.Rotation);

        // Respect the Local/Global toggle in the Tools overlay.
        Quaternion handleRotation = Tools.pivotRotation == PivotRotation.Local
            ? knotRotation
            : Quaternion.identity;

        Vector3 newPosition = position;
        Quaternion newHandleRotation = handleRotation;

        EditorGUI.BeginChangeCheck();
        Handles.TransformHandle(ref newPosition, ref newHandleRotation);
        if (!EditorGUI.EndChangeCheck()) return;

        Vector3 moveDelta = newPosition - position;
        Quaternion rotationDelta = newHandleRotation * Quaternion.Inverse(handleRotation);
        bool moved = moveDelta.sqrMagnitude > 1e-12f;
        bool rotated = Quaternion.Angle(newHandleRotation, handleRotation) > 1e-4f;
        if (!moved && !rotated) return;

        // One undo entry per affected container.
        undoTargets.Clear();
        foreach (SelectableKnot knot in selectedKnots)
        {
            Object owner = knot.SplineInfo.Object;
            if (owner != null && !undoTargets.Contains(owner))
                undoTargets.Add(owner);
        }
        Undo.RecordObjects(undoTargets.ToArray(), "Transform Spline Knot");

        foreach (SelectableKnot selected in selectedKnots)
        {
            if (!selected.IsValid()) continue;

            SelectableKnot knot = selected; // local copy so the property setters can be used
            if (moved)
                knot.Position = (Vector3)knot.Position + moveDelta;
            if (rotated)
                knot.Rotation = ToMathQuaternion(rotationDelta * ToQuaternion(knot.Rotation));
        }
    }

    private static Quaternion ToQuaternion(quaternion q)
    {
        return new Quaternion(q.value.x, q.value.y, q.value.z, q.value.w);
    }

    private static quaternion ToMathQuaternion(Quaternion q)
    {
        return new quaternion(q.x, q.y, q.z, q.w);
    }
}
#endif
