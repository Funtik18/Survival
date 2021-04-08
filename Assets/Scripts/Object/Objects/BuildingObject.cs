using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

/// <summary>
/// Строение для создания и переноса
/// </summary>
public class BuildingObject : WorldObject<BuildingSD> 
{
    [PropertyOrder(-1)]
    [SerializeField] protected bool isPlacement = false;
    [SerializeField] protected LayerMask ignoreLayers;
    [SerializeField] private List<MeshRenderer> renderers = new List<MeshRenderer>();

    public bool IsIntersects => collidersIntersects.Count > 0;
    public virtual bool IsCanBeBuild => true;

    public bool IsPlacement { get => isPlacement; set => isPlacement = value; }

    private List<Collider> collidersIntersects = new List<Collider>();
    private List<Material> materialsBasic = new List<Material>();

    protected virtual void Awake()
    {
        for (int i = 0; i < renderers.Count; i++)
        {
            materialsBasic.Add(renderers[i].material);
        }
    }

    public virtual void Place()
    {
        SetMaterial();

        IsPlacement = true;

        if (data.isImmediatelyCalculation)
        {

        }
    }

    public void SetMaterial(Material material)
    {
        for (int i = 0; i < renderers.Count; i++)
        {
            renderers[i].sharedMaterial = material;
        }
    }
    public void SetMaterial()
    {
        for (int i = 0; i < renderers.Count; i++)
        {
            renderers[i].sharedMaterial = materialsBasic[i];
        }
    }

    public override void StartObserve()
    {
        if (IsPlacement)
        {
            base.StartObserve();
            InteractionButton.SetIconOnInteraction();
            InteractionButton.OpenButton();
        }
    }
    public override void EndObserve()
    {
        base.EndObserve();
        InteractionButton.CloseButton();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (IsPlacement) return;

        if (!collidersIntersects.Contains(other))
        {
            if (((1 << other.gameObject.layer) & ignoreLayers) == 0)
            {
                collidersIntersects.Add(other);
            }

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (IsPlacement) return;

        if (collidersIntersects.Contains(other))
        {
            collidersIntersects.Remove(other);
        }
    }
}