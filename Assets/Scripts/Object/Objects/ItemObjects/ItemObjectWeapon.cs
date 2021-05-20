using UnityEngine;
using UnityEngine.Events;

public class ItemObjectWeapon : ItemObject
{
    [SerializeField] private Animator animator;

    public UnityAction onCapacityСlipChanged;

    private int magazineCapacity;
    public int MagazineCapacity => magazineCapacity;

    public int CurrentСlipCapacity
    {
        get => Data.CurrentMagazineCapacity;
        set
        {
            Data.CurrentMagazineCapacity = value;
            onCapacityСlipChanged?.Invoke();
        }
    }

    public bool IsEmpty => CurrentСlipCapacity == 0;
    public bool IsFull => CurrentСlipCapacity == magazineCapacity;

    private void Awake()
    {
        magazineCapacity = Data.MaxMagaizneCapacity;
    }

    public void ActionItem()
    {
        Item item = GeneralAvailability.PlayerInventory.FindItemByData(Data);

        GeneralAvailability.Player.Status.opportunities.EquipItem(item);

        animator.enabled = true;
    }

    public void Shoot()
    {
        if (!IsEmpty)
        {
            CurrentСlipCapacity--;
        }
        else
        {
            Debug.LogError("No Shoot");
        }
    }
    public void Aim()
    {
        animator.SetBool("Aim", true);
    }
    public void DeAim()
    {
        animator.SetBool("Aim", false);
    }
}