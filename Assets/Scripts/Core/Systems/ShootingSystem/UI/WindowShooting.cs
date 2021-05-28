using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WindowShooting : WindowUI
{
    public UnityAction onAim;
    public UnityAction onDeAim;
    public UnityAction onShoot;
    public UnityAction onReload;
    public UnityAction onTakeAway;

    [SerializeField] private Image crosshair;
    [Space]
    [SerializeField] private Pointer pointerAim;
    [SerializeField] private Pointer pointerShoot;
    [SerializeField] private Pointer pointerReload;
    [SerializeField] private Pointer pointerTakeAway;
    [Space]
    [SerializeField] private TMPro.TextMeshProUGUI textAllPatrons;
    [SerializeField] private Transform parentBulletIndicators;
    [SerializeField] private IndicatorUI bulletIndicatorPrefab;

    private List<IndicatorUI> indicators = new List<IndicatorUI>();

    private bool isAim = false;
    private ItemObjectWeapon weapon;

    private void Awake()
    {
        pointerAim.AddPressListener(AimDeAim);
        pointerShoot.AddPressListener(Shoot);
        pointerReload.AddPressListener(Reload);
        pointerTakeAway.AddPressListener(TakeAway);
    }

    public WindowShooting Setup(ItemObjectWeapon weapon, UnityAction aim = null, UnityAction deaim = null, UnityAction shoot = null, UnityAction reload = null, UnityAction takeaway = null)
    {
        DeAim();

        this.weapon = weapon;
        weapon.onCapacityСlipChanged += UpdateIndicatorsClipCapacity;

        UpdateUI();

        onAim = aim;
        onDeAim = deaim;
        onShoot = shoot;
        onReload = reload;
        onTakeAway = takeaway;
        return this;
    }

    private void UpdateUI()
    {
        UpdateIndicators();
    }


    private void UpdateIndicators()
    {
        indicators.Clear();
        for (int i = parentBulletIndicators.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(parentBulletIndicators.GetChild(i).gameObject);
        }

        for (int i = 0; i < weapon.MagazineCapacity; i++)
        {
            IndicatorUI indicator = Instantiate(bulletIndicatorPrefab, parentBulletIndicators);

            indicators.Add(indicator);
        }

        UpdateIndicatorsClipCapacity();
    }
    private void UpdateIndicatorsClipCapacity()
    {
        int count = weapon.CurrentСlipCapacity;
        for (int i = 0; i < indicators.Count; i++)
        {
            indicators[i].IsOn = i < count;
        }
    }



    public void DeAim()
    {
        pointerAim.GetComponent<Image>().color = Color.white;

        isAim = false;

        crosshair.enabled = !isAim;
    }
    public void Aim()
    {
        pointerAim.GetComponent<Image>().color = Color.grey;

        isAim = true;

        crosshair.enabled = !isAim;
    }

    private void AimDeAim()
    {
        if (isAim)
            onDeAim?.Invoke();
        else
            onAim?.Invoke();
    }

    private void Shoot()
    {
        onShoot?.Invoke();
    }
    private void Reload()
    {
        onReload?.Invoke();
    }
    private void TakeAway()
    {
        onTakeAway?.Invoke();
    }
}