using UnityEngine;
using UnityEngine.Events;

public class ItemObjectWeapon : ItemObject
{
    public UnityAction onRequiredReload;

    [SerializeField] private bool autoReload = true;
    [SerializeField] private int ammoReloadRate = 1;
    [SerializeField] private float ammoReloadDelay = 2;
    public int AmmoReloadRate => ammoReloadRate;
    public float AmmoReloadDelay => ammoReloadDelay;

    [SerializeField] private GameObject impactSnow;
    [SerializeField] private GameObject impactAnimal;
    [Header("Internal References")]
    [SerializeField] private Transform weaponMuzzle;
    [SerializeField] private Transform ejectionPort;

    [Header("Shoot Parameters")]
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private GameObject muzzleFlashPrefab;
    [SerializeField] private GameObject shellCasing;
    [Min(100f)]
    [SerializeField] private float rangeShot = 1000f;
    [SerializeField] private float delayBetweenShots = 0.5f;
    [SerializeField] private float bulletSpreadAngle = 0f;
    [SerializeField] private int bulletsPerShot = 1;
    [SerializeField] private bool offhandShooting = false;
    [Space]
    [SerializeField] private float recoilForce = 1;

    [Header("Crosshair")]
    [SerializeField] private Sprite crosshair;

    [Header("Audio & Visual")]
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

    private bool isReadyToShoot = false;

    public bool IsReadyToShoot => isReadyToShoot;

    public bool IsEmpty => CurrentСlipCapacity == 0;
    public bool IsFull => CurrentСlipCapacity == magazineCapacity;


    private bool isAiming = false;

    private void Awake()
    {
        magazineCapacity = Data.MaxMagaizneCapacity;
    }

    public void ActionItem()
    {
        Item item = GeneralAvailability.PlayerInventory.FindItemByData(Data);

        GeneralAvailability.Player.Status.opportunities.EquipItem(item);
    }

    public void Enable(bool trigger)
    {
        animator.enabled = trigger;
    }

    public void Shoot()
    {
        if (!IsEmpty)
        {
            if (isAiming || offhandShooting)
            {
                MuzzleFlash();

                CurrentСlipCapacity -= bulletsPerShot;

                ShootHit();

                isReadyToShoot = false;
            }
        }
        else
        {
            if (autoReload)
            {
                onRequiredReload?.Invoke();
            }
        }
    }
    public void Aim()
    {
        isAiming = true;
        animator.SetBool("Aim", true);
    }
    public void DeAim()
    {
        animator.SetBool("Aim", false);
        isAiming = false;
    }


    private void ShootHit()
    {
        RaycastHit hit;
        if(Physics.Raycast(weaponMuzzle.position, weaponMuzzle.forward, out hit, rangeShot))
        {
            LayerMask hitMask = hit.collider.gameObject.layer;

            if(hitMask == LayerMask.NameToLayer("Ground"))
            {
                ImpactEffect(hit, impactSnow);
            }
            else if(hitMask == LayerMask.NameToLayer("Animal"))
            {
                HitZone zoneHit = hit.collider.gameObject.GetComponent<HitZone>();
                if (zoneHit != null)
                    zoneHit.Hit();

                ImpactEffect(hit, impactAnimal);
            }
        }
    }
    private void MuzzleFlash()
    {
        GameObject muzzleFlash = ObjectPool.GetObject(muzzleFlashPrefab, false);
        muzzleFlash.transform.position = weaponMuzzle.position;
        muzzleFlash.transform.forward = weaponMuzzle.forward;

        muzzleFlash.SetActive(true);
        muzzleFlash.GetComponent<SelfDestruct>().StartDestruct();
    }
    private void ImpactEffect(RaycastHit hit, GameObject impact)
    {
        GameObject impactEffect = ObjectPool.GetObject(impact, false);
        impactEffect.transform.position = hit.point;
        impactEffect.transform.rotation = Quaternion.LookRotation(hit.normal);

        impactEffect.SetActive(true);
        impactEffect.GetComponent<SelfDestruct>().StartDestruct();
    }


    private void OnDrawGizmos()
    {
        if (weaponMuzzle)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(weaponMuzzle.position, weaponMuzzle.position + weaponMuzzle.forward * 1000);
        }
    }
    protected override void OnDisable()
    {
        base.OnDisable();

        onRequiredReload = null;
    }
}