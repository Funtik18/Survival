using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuilding : MonoBehaviour
{
	[SerializeField] private PlayerCamera playerCamera;
	[Space]
	[SerializeField] private GameObject prefab;
	private Mesh Mesh => prefab.GetComponent<MeshFilter>().sharedMesh;

	[SerializeField] private Material acceptMaterial;
	[SerializeField] private Material rejectMaterial;

	[SerializeField] private LayerMask groundLayers;
	[Space]
	[SerializeField] private float rayDistance = 2.5f;
	[SerializeField] private float rayUpDistance = 1f;
	[Range(-90, 90)]
	[SerializeField] private float anglePlacement = 60f;

	private Coroutine buildCoroutine = null;
	public bool IsBuildingProccess => buildCoroutine != null;
	private WaitForSeconds buildingSeconds = new WaitForSeconds(0.05f);


	private bool isCanBuild = false;

	private Vector3 SpherePoint => playerCamera.Transform.position + (playerCamera.Transform.forward * rayDistance);

	#region Cash
	private RaycastHit lastHit;

	private Vector3 lastPosition;
	private Quaternion lastRotation;
	private float lastAngle;
    #endregion

    private void Update()
	{
		if (Input.GetKeyDown(KeyCode.F))
		{
			StartBuild();
		}
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (IsBuildingProccess && isCanBuild)
            {
				StopBuild();
				Transform obj = Instantiate(prefab).transform;
				obj.position = lastPosition;
				obj.rotation = lastRotation;
			}
        }
	}


	#region Building
	private void StartBuild()
	{
		if (!IsBuildingProccess)
		{
			buildCoroutine = StartCoroutine(Build());
        }
        else
        {
			StopBuild();
		}
	}
	private IEnumerator Build()
	{
		while (true)
		{
			RaycastHit hit;

			Ray ray = new Ray(playerCamera.Transform.position, playerCamera.Transform.forward);
			//первый чек проверяет на максимум до rayDistance
			if (CheckCast(ray, out hit, rayDistance) == false)
            {
				ray = new Ray(SpherePoint, Vector3.down);
				//второй чек проверяет луч от rayDistance конечной точки вниз Vector.down до rayDistance * 2f
				CheckCast(ray, out hit, rayDistance * 2f);
			}
			yield return null;
		}

		StopBuild();
	}
	private void StopBuild()
	{
		if (IsBuildingProccess)
		{
			StopCoroutine(buildCoroutine);
			buildCoroutine = null;
		}
	}
	#endregion


	private void DrawMesh(bool trigger)
    {
		isCanBuild = trigger;

		Graphics.DrawMesh(Mesh, lastPosition, lastRotation, isCanBuild ? acceptMaterial : rejectMaterial, 0);
	}


	private bool CheckCast(Ray ray, out RaycastHit hit, float distance)
	{
		if (Physics.Raycast(ray, out hit, distance, groundLayers))
		{
			lastHit = hit;
			lastPosition = hit.point;
			lastRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
			lastAngle = 90 - (Vector3.Angle(Vector3.up, hit.normal));

			if (CheckPlacementAngle(lastAngle))
				DrawMesh(true);
            else
				DrawMesh(false);

			return true;
		}
        return false;
	}

	private bool CheckPlacementAngle(float angle)
	{
		if (angle >= anglePlacement)
			return true;
		return false;
	}

    private void OnDrawGizmos()
    {
        if (IsBuildingProccess)
        {
			Gizmos.color = Color.red;
			
			//sphere point and ray down
			Gizmos.DrawSphere(SpherePoint, 0.1f);
			Gizmos.DrawLine(SpherePoint, SpherePoint + (Vector3.down * rayDistance * 2));

			//ray hit
			Gizmos.DrawLine(lastHit.point, lastHit.point + (lastHit.normal * 1.1f));
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(lastHit.point, lastHit.point + (lastHit.barycentricCoordinate * 1f));
			Gizmos.color = Color.green;
			Gizmos.DrawSphere(lastHit.point, 0.04f);

			//ray up
			Gizmos.color = Color.green;
			Gizmos.DrawLine(lastHit.point, lastHit.point + (Vector3.up * rayUpDistance));
		}
	}
}
