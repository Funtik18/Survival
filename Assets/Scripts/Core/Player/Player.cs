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

	[SerializeField] private Build build;
	public Build Build => build;

	[SerializeField] private PlayerController controller;
	public PlayerController Controller => controller;

	[SerializeField] private PlayerCamera camera;
	public PlayerCamera Camera => camera;

	[SerializeField] private PlayerUI ui;
	public PlayerUI UI => ui;
	#endregion

	[SerializeField] private Data data;

	public ItemInspector itemInspector;//make it

	[Space]
	[SerializeField] private bool isLockCursor = true;

	private bool isMoveLocked = false;
	private bool isLookLocked = false;
	private bool isBrainPaussed = false;

	private Coroutine brainCoroutine;
	public bool IsBrainProccess => brainCoroutine != null;


	
	private void Setup()
    {
		Controller.Init(this);

		UI.Setup(this);
		Build.Init(this);

		CheckCursor();

		StartBrain();
	}
	
	

	private void StartBrain()
    {
        if (!IsBrainProccess)
        {
			brainCoroutine = StartCoroutine(Brain());
		}
    }
	private IEnumerator Brain()
    {
        while (true)
        {
			while (isBrainPaussed)
			{
				yield return null;
			}

			Controller.UpdateGravity();

			if (GeneralSettings.IsPlatformMobile)
			{
				Debug.LogError("Mobile");
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
			else if (GeneralSettings.IsPlatformPC)
			{
				Debug.LogError("PC");
				if (isMoveLocked == false)
				{
					Controller.UpdatePCMovement();
					Controller.UpdateMovement();
				}
				if (isLookLocked == false)
				{
					Controller.UpdatePCLook();
				}
			}

			yield return null;
		}

		StopBrain();
	}
	private void PauseBrain()
    {
		isBrainPaussed = true;

		Controller.Enable(false);
	}
	private void ResumeBrain()
    {
		isBrainPaussed = false;

		Controller.Enable(true);
	}
	private void StopBrain()
    {
        if (IsBrainProccess)
        {
			StopCoroutine(brainCoroutine);
			brainCoroutine = null;
        }
    }

	public void ChangePosition(Vector3 position, Quaternion rotation)
    {
		PauseBrain();
		transform.position = position;
		transform.rotation = rotation;
		ResumeBrain();
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
public struct Stay3
{
	public bool isEnable;

	public Vector3 position;
	public Quaternion rotation;
	public Vector3 scale;
}