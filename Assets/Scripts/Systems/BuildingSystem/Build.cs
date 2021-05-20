using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;
using UnityEngine.Events;

[System.Serializable]
public class Build
{
	public UnityAction<BuildingObject> onStartBuild;
	public UnityAction onEndBuild;

	[SerializeField] private Material acceptMaterial;
	[SerializeField] private Material rejectMaterial;

	[SerializeField] private LayerMask groundLayers;
	[Space]
	[SerializeField] private float rayDistance = 2.5f;
	[SerializeField] private float rayUpDistance = 1f;
	[Range(-90, 90)]
	[SerializeField] private float anglePlacement = 60f;

	public bool isCanBuild { get; set; }

	private Player player;
	private PlayerInventory inventory;
	private PlayerCamera playerCamera;

	private BuildingObject currentBuilding;
	private Transform currentTransform => currentBuilding.transform;

	private Coroutine buildCoroutine = null;
	public bool IsBuildingProccess => buildCoroutine != null;
	private WaitForSeconds buildingSeconds = new WaitForSeconds(0.05f);

	private Vector3 SpherePoint => playerCamera.Transform.position + (playerCamera.Transform.forward * rayDistance);

	private ObjectPool pool;

	#region Cash
	private List<Collider> collidersIntersects;

	private RaycastHit lastHit;

	private Vector3 lastPosition;
	private Quaternion lastRotation;
	private float lastAngle;
    #endregion

    public void Init(Player player)
    {
		this.player = player;
		this.inventory = player.Inventory;
		this.playerCamera = player.Camera;
		collidersIntersects = playerCamera.collidersIntersects;

		pool = ObjectPool.Instance;
	}

	public bool IsCanBuild(BuildingSD sd)
	{
		if (sd != null)
		{
			if (sd.isFromInventory)
			{
				if (sd.isFromInventoryComplex)
				{
					if (sd.isTypes)
					{
						return inventory.ContainsType(sd.categories);
					}
					else
					{
						for (int i = 0; i < sd.items.Count; i++)
						{
							if (inventory.IsContainsBlueprintItem(sd.items[i]) == false)
							{
								return false;
							}
						}
						return true;
					}
				}
				else
				{
					return inventory.IsContainsBlueprintItem(sd.item);
				}
			}
		}

		return true;
	}


	public void BuildBuilding(BuildingObject building)
	{
		if (building == null) return;

		playerCamera.LockVision();

		currentBuilding = pool.GetObject(building.gameObject).GetComponent<BuildingObject>();

		onStartBuild?.Invoke(currentBuilding);

		GeneralAvailability.PlayerUI.CloseRadialMenu();

		StartBuild();
	}
	public void PlacementBuilding()
    {
		if (IsBuildingProccess && isCanBuild)
		{
			StopBuild();

			currentTransform.position = lastPosition;
			currentTransform.rotation = lastRotation;

			currentBuilding.Place();

			Exchange();
		}
	}
	private void Dispose()
    {
		if (currentBuilding != null)
        {
			GameObject.Destroy(currentBuilding.gameObject);
        }
    }

	#region Building
	private void StartBuild()
	{
		if (!IsBuildingProccess)
		{
			buildCoroutine = player.StartCoroutine(Building());
        }
        else
        {
			StopBuild();
		}
	}
	private IEnumerator Building()
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
			player.StopCoroutine(buildCoroutine);
			buildCoroutine = null;

			playerCamera.UnLockVision();

			GeneralAvailability.PlayerUI.OpenRadialMenu();

			onEndBuild?.Invoke();
		}
	}

	public void BreakBuild()
    {
		StopBuild();

		Dispose();
	}
	#endregion

	private void Draw(bool trigger)
    {
		isCanBuild = trigger;

		currentTransform.position = lastPosition;
		currentTransform.rotation = lastRotation;

		currentBuilding.IsPlacement = false;

		currentBuilding.SetMaterial(isCanBuild ? acceptMaterial : rejectMaterial);
	}

	private bool CheckCast(Ray ray, out RaycastHit hit, float distance)
	{
		if (Physics.Raycast(ray, out hit, distance, groundLayers))
		{
			lastHit = hit;
			lastPosition = hit.point;
			lastRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
			lastAngle = 90 - (Vector3.Angle(Vector3.up, hit.normal));

			if (CheckPlacementAngle(lastAngle) && !currentBuilding.IsIntersects)
				Draw(true);
            else
				Draw(false);

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


	private void Exchange()
    {
		BuildingSD sd = currentBuilding.Data;

        if (sd.IsItem)
        {
			if (sd.exchangeAfterBuild)
			{
				Item item = inventory.GetBySD(sd.item.item);
				currentBuilding.StoredItem = item;
				inventory.RemoveItem(sd.item);
			}
		}
		else if (sd.IsListItems)
        {
			if (sd.exchangeAfterBuild)
			{
                for (int i = 0; i < sd.items.Count; i++)
                {
					inventory.RemoveItem(sd.items[i]);
				}
			}
		}
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