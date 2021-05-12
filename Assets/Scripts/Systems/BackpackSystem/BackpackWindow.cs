using UnityEngine;

public class BackpackWindow : MonoBehaviour
{
    public string windowName;

    protected PlayerInventory inventory;

    public virtual void Setup(PlayerInventory inventory)
    {
        this.inventory = inventory;
    }
    public virtual void UpdateWindow()
    {

    }
    public virtual void OpenWindow()
    {
        gameObject.SetActive(true);
    }
    public virtual void CloseWindow()
    {
        gameObject.SetActive(false);
    }
}