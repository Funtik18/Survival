using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector.Editor;

[CustomEditor(typeof(AI), true)]
public class AIEditor : OdinEditor
{
    void OnSceneGUI()
    {
        AI ai = (AI)target;

        Handles.color = Color.green;

        Handles.DrawWireArc(ai.Transform.position, Vector3.up, Vector3.forward, 360f, ai.wanderInnerRadius);
        Handles.DrawWireArc(ai.Transform.position, Vector3.up, Vector3.forward, 360f, ai.wanderOuterRadius);

        Handles.color = Color.red;

        Handles.DrawWireArc(ai.Transform.position, Vector3.up, Vector3.forward, 360f, ai.awayInnerRadius);
        Handles.DrawWireArc(ai.Transform.position, Vector3.up, Vector3.forward, 360f, ai.awayOuterRadius);
    }
}
