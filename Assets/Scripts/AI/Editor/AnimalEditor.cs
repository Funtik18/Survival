using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector.Editor;

[CustomEditor(typeof(Animal), true)]
public class AnimalEditor : OdinEditor
{
    void OnSceneGUI()
    {
        Animal ai = (Animal)target;

        Handles.color = Color.green;

        Handles.DrawWireArc(ai.Transform.position, Vector3.up, Vector3.forward, 360f, ai.wanderInnerRadius);
        Handles.DrawWireArc(ai.Transform.position, Vector3.up, Vector3.forward, 360f, ai.wanderOuterRadius);

        Handles.color = Color.red;

        Handles.DrawWireArc(ai.Transform.position, Vector3.up, Vector3.forward, 360f, ai.awayInnerRadius);
        Handles.DrawWireArc(ai.Transform.position, Vector3.up, Vector3.forward, 360f, ai.awayOuterRadius);
    }
}
