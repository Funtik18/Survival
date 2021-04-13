using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WindowFireMenu : WindowUI
{
    public UnityAction onBack;

    [SerializeField] private FireMenuPrimary menuPrimary;

    private void Awake()
    {
        menuPrimary.onBack += Back;
    }

    public void Setup(Inventory inventory, CamfireBuilding camfireBuilding)
    {
        menuPrimary.Setup(inventory);

        ShowWindow();
    }

    private void Back()
    {
        onBack?.Invoke();
    }
}