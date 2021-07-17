using UnityEngine;

using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Linq;

/// <summary>
/// Need optimaze
/// </summary>
public class Player : MonoBehaviour
{
	[SerializeField] private Data data;

	#region Properties
	private static Player instance;
	public static Player Instance
	{
		get
		{
			if(instance == null)
			{
				instance = FindObjectOfType<Player>();
			}
			return instance;
		}
	}


	[SerializeField] private PlayerStatus status;
	public PlayerStatus Status => status;

	[SerializeField] private PlayerInventory inventory;
	public PlayerInventory Inventory => inventory;


	[SerializeField] private Inspector inspector;
	public Inspector Inspector => inspector;

	[SerializeField] private Build build;
	public Build Build => build;


	[SerializeField] private PlayerController controller;
	public PlayerController Controller => controller;

	[SerializeField] private PlayerCamera camera;
	public PlayerCamera Camera => camera;

	[SerializeField] private PlayerUI ui;
	public PlayerUI UI => ui;
	#endregion

	[Space]
	[SerializeField] private bool isLockCursor = true;

	private bool isMoveLocked = false;
	private bool isLookLocked = false;
	private bool isBrainPaussed = false;

	
	private void Setup()
    {
		Controller.Setup(this);

		UI.Setup(this);
		Inspector.Setup(this);
		Build.Setup(this);

		CheckCursor();
	}

	private void Update()
	{
		Controller.UpdateGravity();

        if (isMoveLocked == false)
        {
            Controller.UpdateMobileMovement();
            Controller.UpdateMovement();
        }
        if (isLookLocked == false)
        {
            Controller.UpdateMobileLook();
        }
    }

	public void ChangePosition(Vector3 position, Quaternion rotation)
    {
		transform.position = position;
		transform.rotation = rotation;
	}
	public void ChangePosition(Stay3 stay)
	{
		ChangePosition(stay.position, stay.rotation);
	}

	#region Lock
	public void Lock()
    {
		isMoveLocked = true;
		isLookLocked = true;
        camera.LockVision();
        UI.controlUI.LockControl();
    }
	public void UnLock()
    {
		isMoveLocked = false;
		isLookLocked = false;
        camera.UnLockVision();
        UI.controlUI.UnLockControl();
    }

	public void LockMovement()
    {
		isMoveLocked = true;
		isLookLocked = true;
		UI.controlUI.LockControl();
	}
	public void UnLockMovement()
    {
		isMoveLocked = false;
		isLookLocked = false;
		UI.controlUI.UnLockControl();
	}
    #endregion

	private void CheckCursor()
	{
		if(isLockCursor)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
	}


	public void SetData(Data data)
	{
		if (data == null)
		{
			Status.SetData(this.data.statusData).Init(this);

			Inventory.SetData(this.data.inventoryData).Init();
		}
		else
		{
			ChangePosition(data.stay);

			Status.SetData(data.statusData).Init(this);

			Inventory.SetData(data.inventoryData).Init();
		}

		Setup();
	}
	public Data GetData()
	{
		Data data = new Data()
		{
			stay = new Stay3()
			{
				position = transform.localPosition,
				rotation = transform.localRotation,
			},

			statusData = Status.GetData(),
			inventoryData = Inventory.GetData(),
		};

		return data;
	}

	[System.Serializable]
	public class Data 
	{
		public Stay3 stay;

		public PlayerStatus.Data statusData;
		public Inventory.Data inventoryData;
	}
}
[System.Serializable]
public class Inspector
{
	[SerializeField] private Transform modelPlace;
	[SerializeField] private Camera camera;

	[SerializeField] private Vector2 rotationSpeedMobile = new Vector2(10, 10);
	[SerializeField] private Vector2 rotationSpeedPC = new Vector2(0.1f, 0.1f);

	private Coroutine reviewCoroutine = null;
	public bool IsReviewProccess => reviewCoroutine != null;
	private WaitForSeconds waiter = new WaitForSeconds(0.05f);

	private Player owner;

	private ItemObject itemObject;

	private UnityAction onItemTake;
	private UnityAction onItemAction;
	private UnityAction onItemLeave;

	public void Setup(Player player)
    {
		this.owner = player;

		//ui
		player.UI.windowsUI.itemInspectorWindow.Setup(ItemTake, ItemAction, ItemLeave);
		opportunities = player.Status.opportunities;
	}

	/// <summary>
	/// Осматриваем предмет из мира
	/// </summary>
	/// <param name="itemObject"></param>
	public void SetItem(ItemObject itemObject)
	{
		this.itemObject = itemObject;
		itemObject.ColliderEnable(false);

		onItemTake = FromWorldTake;
		onItemAction = Action;
		onItemLeave = ToWorldLeave;

		Inspect.Setup(true, true);
		Inspect.Start(owner, camera, itemObject.transform, modelPlace, itemObject.CurrentData.scriptableData.orientation, GeneralSettings.IsPlatformPC ? rotationSpeedPC : rotationSpeedMobile);
	}
	/// <summary>
	/// Осматриваем предметы из контейнера
	/// </summary>
	/// <param name="from"></param>
	public void ItemsReview(Inventory from)
	{
		onItemTake = FromInventoryTake;
		onItemAction = Action;
		onItemLeave = ToInventoryLeave;

		currentInventory = from;

		Inspect.Setup(false, true);

		StartReview();
    }

	#region Review
	private PlayerOpportunities opportunities;

	private List<Item> cashItemsOnDelete = new List<Item>();
	private Item currentInventoryItem;
	private Inventory currentInventory;

	private void StartReview()
	{
		if (!IsReviewProccess)
		{
			reviewCoroutine = owner.StartCoroutine(Review());
		}
	}
	//Работает на соплях
	private IEnumerator Review()
	{
		List<Item> items = currentInventory.CurrentItemsCopy;

        foreach (var item in items)
        {
			currentInventoryItem = item;

			InstantieateModel(currentInventoryItem);//создание модели

			itemObject.InteractSub();//генерирует дату и открывает ui

			while (opportunities.IsUseProccess)
			{
				yield return null;
			}

			Inspect.Start(owner, camera, itemObject.transform, modelPlace, currentInventoryItem.itemData.scriptableData.orientation, GeneralSettings.IsPlatformPC ? rotationSpeedPC : rotationSpeedMobile);

			yield return waiter;

			while (Inspect.IsInspectProccess)
			{
				yield return null;
			}
		}

		//чистка
		currentInventoryItem = null;
		currentInventory = null;

		StopReview();
	}
	private void StopReview()
	{
		if (IsReviewProccess)
		{
			owner.StopCoroutine(reviewCoroutine);
			reviewCoroutine = null;
		}
	}
	#endregion


	private void FromWorldTake()
    {
		AddItem();

		Inspect.Break();

		ObjectPool.ReturnGameObject(itemObject.gameObject);
	}
	private void ToWorldLeave()
    {
		Inspect.Stop();

		itemObject.ColliderEnable(true);
	}

	private void FromInventoryTake()
    {
		AddItem();
		RemoveInventoryItem();

		Inspect.Break();

		ObjectPool.ReturnGameObject(itemObject.gameObject);
	}
	private void ToInventoryLeave()
    {
		Inspect.Break();

		ObjectPool.ReturnGameObject(itemObject.gameObject);
	}

	private void Action()
    {
		Inspect.Break();

		if (itemObject.CurrentData.IsWeapon)
        {
			AddItem();
			RemoveInventoryItem();

			ObjectPool.ReturnGameObject(itemObject.gameObject);

			Item item = GeneralAvailability.PlayerInventory.FindItemByData(itemObject.CurrentData);
			GeneralAvailability.Player.Status.opportunities.EquipItem(item);
		}
		else if (itemObject.CurrentData.IsConsumable)
        {
			AddItem();
			RemoveInventoryItem();

			ObjectPool.ReturnGameObject(itemObject.gameObject);
			
			Item item = GeneralAvailability.PlayerInventory.FindItemByData(itemObject.CurrentData);
			GeneralAvailability.Player.Status.opportunities.UseItem(item);
		}

		itemObject.ItemAction();
    }


	private void InstantieateModel(Item item)
	{
		GameObject obj = ObjectPool.GetObject(item.itemData.scriptableData.model.gameObject, false);
		obj.SetActive(true);

		itemObject = obj.GetComponent<ItemObject>();
	}

	private void AddItem()
	{
		GeneralAvailability.PlayerInventory.AddItem(itemObject.CurrentData);
	}
	private void RemoveInventoryItem()
    {
		currentInventory?.RemoveItem(currentInventoryItem);
	}

	private void ItemTake()
	{
		onItemTake?.Invoke();
	}
	private void ItemAction()
	{
		onItemAction?.Invoke();
	}
	private void ItemLeave()
	{
		onItemLeave?.Invoke();
	}

	public static class Inspect 
	{
		private static Coroutine inspectCorutine = null;
		public static bool IsInspectProccess => inspectCorutine != null;
		private static bool IsInspection = false;

		private static bool isAnimateStart = true; 
		private static bool isAnimateEnd = true;

		private static MonoBehaviour owner;

		private static Camera camera;

		private static Transform obj;
		private static Transform objPlace;
		private static Transform objParent;

		private static Vector3 newWorldPosition;
		private static Quaternion newWorldRotation;

		private static Vector3 oldWorldPosition;
		private static Quaternion oldWorldRotation;

		private static float rotationSpeedX;
		private static float rotationSpeedY;

		public static void Setup(bool animateStart = true, bool animateEnd = true)
        {
			isAnimateStart = animateStart;
			isAnimateEnd = animateEnd;
		}
		public static void Start(MonoBehaviour owner, Camera camera, Transform obj, Transform objPlace, Stay3 newOrientation, Vector2 rotationSpeed)
        {
			if (!IsInspectProccess)
			{
				Inspect.owner = owner;
				Inspect.obj = obj;

				Inspect.camera = camera;

				Inspect.objPlace = objPlace;
				Inspect.newWorldPosition = newOrientation.position;
				Inspect.newWorldRotation = newOrientation.rotation;

				objParent = obj.parent;
				oldWorldPosition = obj.position;
				oldWorldRotation = obj.rotation;

				rotationSpeedX = rotationSpeed.x;
				rotationSpeedY = rotationSpeed.y;

				inspectCorutine = owner.StartCoroutine(Inspector());
			}
		}

		public static void Break()
        {
			StopInspect();
			//IsInspectObject = false;
		}
		public static void Stop()
        {
			IsInspection = false;
		}

		private static IEnumerator Inspector()
		{
			obj.SetParent(objPlace);
            if (isAnimateStart)
            {
				yield return LerpItem(obj, newWorldPosition, newWorldRotation, 0.25f, true);//lerp item from world to local
			}
            else
            {
				obj.localPosition = newWorldPosition; 
				obj.localRotation = newWorldRotation;
			}

			yield return InspectTransform();

			obj.SetParent(objParent);
            if (isAnimateEnd)
            {
				yield return LerpItem(obj, oldWorldPosition, oldWorldRotation, 0.3f);//lerp item back to world
            }
            else
            {
				obj.position = oldWorldPosition;
				obj.rotation = oldWorldRotation;
			}

			StopInspect();
		}
		private static void StopInspect()
		{
			if (IsInspectProccess)
			{
				owner.StopCoroutine(inspectCorutine);
				inspectCorutine = null;
			}
		}

		private static IEnumerator LerpItem(Transform trans, Vector3 endPosition, Quaternion endRotation, float duration = 0.2f, bool isLocal = false)
		{
			Vector3 startPosition;
			Quaternion startRotation;

			if (isLocal)
			{
				startPosition = trans.localPosition;
				startRotation = trans.localRotation;

				float time = 0;

				while (time < duration)
				{
					float normalStep = time / duration;

					trans.localPosition = Vector3.Lerp(startPosition, endPosition, normalStep);
					trans.localRotation = Quaternion.Lerp(startRotation, endRotation, normalStep);

					time += Time.deltaTime;

					yield return null;
				}
				trans.localPosition = endPosition;
				trans.localRotation = endRotation;
			}
			else
			{
				startPosition = trans.position;
				startRotation = trans.rotation;

				float time = 0;

				while (time < duration)
				{
					float normalStep = time / duration;

					trans.position = Vector3.Lerp(startPosition, endPosition, normalStep);
					trans.rotation = Quaternion.Lerp(startRotation, endRotation, normalStep);

					time += Time.deltaTime;

					yield return null;
				}
				trans.position = endPosition;
				trans.rotation = endRotation;
			}
		}

		private static IEnumerator InspectTransform()
		{
			IsInspection = true;
			while (IsInspection)
			{
				if (Input.touchCount > 0)
				{
					float rotX = Input.touches[0].deltaPosition.x * rotationSpeedX;
					float rotY = Input.touches[0].deltaPosition.y * rotationSpeedY;

					RotateItem(rotX, rotY);
				}
				if (Input.GetMouseButton(0))
				{
					float rotX = Input.GetAxis("Mouse X") * rotationSpeedX;
					float rotY = Input.GetAxis("Mouse Y") * rotationSpeedY;

					RotateItem(rotX, rotY);
				}
				yield return null;
			}
		}

		/// <summary>
		/// Детальный осмотр предмета с поворотами по X и Y.
		/// </summary>
		/// <returns></returns>
		private static void RotateItem(float rotX, float rotY)
		{
			Vector3 right = Vector3.Cross(camera.transform.up, obj.position - camera.transform.position);
			Vector3 up = Vector3.Cross(obj.position - camera.transform.position, right);
			obj.rotation = Quaternion.AngleAxis(-rotX, up) * obj.rotation;
			obj.rotation = Quaternion.AngleAxis(rotY, right) * obj.rotation;
		}
	}
}




[System.Serializable]
public struct Stay3
{
	public bool isEnable;

	public Vector3 position;
	public Quaternion rotation;
	public Vector3 scale;
}