using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowBuild : WindowUI
{
    [SerializeField] private CustomPointer buttonAccept;
    [SerializeField] private CustomPointer buttonReject;

    private Build build;

    public void Setup(Build build)
    {
        this.build = build;

        this.build.onStartBuild += StartBuild;
        this.build.onEndBuild += EndBuild;

        buttonAccept.pointer.AddPressListener(build.PlacementBuilding);
        buttonReject.pointer.AddPressListener(build.BreakBuild);
    }
    private void StartBuild(BuildingObject building) 
    {
        ShowWindow();
    }
    private void EndBuild()
    {
        HideWindow();
    }
}