using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector.Editor;

[CustomEditor(typeof(Generator), true)]
public class GeneratorEditor : OdinEditor
{
    private void OnSceneGUI()
    {
        Generator generator = (Generator)target;

        Handles.color = Color.green;

        Handles.DrawWireArc(generator.transform.position, Vector3.up, Vector3.forward, 360f, generator.zoneRadius);

        Handles.color = Color.red;
        Handles.DrawLine(generator.transform.position, generator.transform.position + (Vector3.forward * generator.prefabRadius));
    }
}