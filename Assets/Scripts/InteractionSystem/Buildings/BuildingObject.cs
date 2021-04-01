using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingObject : WorldObject
{
    [SerializeField] private LayerMask ignoreLayers;

    [SerializeField] private List<MeshRenderer> renderers = new List<MeshRenderer>();

    private List<Collider> collidersIntersects = new List<Collider>();

    private List<Material> materialsBasic = new List<Material>();

    public bool IsIntersects => collidersIntersects.Count > 0;

    private void Awake()
    {
        for (int i = 0; i < renderers.Count; i++)
        {
            materialsBasic.Add(renderers[i].material);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!collidersIntersects.Contains(other))
        {
            if(((1 << other.gameObject.layer) & ignoreLayers) == 0)
            {
                collidersIntersects.Add(other);
            }

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (collidersIntersects.Contains(other))
        {
            collidersIntersects.Remove(other);
        }
    }

    public void SetMaterial(Material material)
    {
        for (int i = 0; i < renderers.Count; i++)
        {
            renderers[i].sharedMaterial = material;
        }
    }
    public void SetMaterialOnBasic()
    {
        for (int i = 0; i < renderers.Count; i++)
        {
            renderers[i].sharedMaterial = materialsBasic[i];
        }
    }


    public override void StartObserve()
    {
        base.StartObserve();
        Button.SetIconOnInteraction();
        Button.OpenButton();
        //GeneralAvailability.TargetPoint.SetToolTipText(scriptableData.information.name).ShowToolTip();
    }
}
