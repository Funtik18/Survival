using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FieldOfView))]
public class FieldOfViewEditor : Editor
{
	void OnSceneGUI()
	{
		FieldOfView fow = (FieldOfView)target;
		Handles.color = Color.white;
			
		Handles.DrawWireArc(fow.Transform.position, Vector3.up, Vector3.forward, 360f, fow.viewRadius);

		Vector3 viewAngleA = fow.DirFromAngle(-fow.viewAngle / 2, false);
		Vector3 viewAngleB = fow.DirFromAngle(fow.viewAngle / 2, false);

		Handles.DrawLine(fow.Transform.position, fow.Transform.position + viewAngleA * fow.viewRadius);
		Handles.DrawLine(fow.Transform.position, fow.Transform.position + viewAngleB * fow.viewRadius);


		Handles.color = Color.red;

		for (int i = 0; i < fow.visibleTargets.Count; i++)
		{
			Handles.DrawLine(fow.Transform.position, fow.visibleTargets[i].position);
		}
	}
}