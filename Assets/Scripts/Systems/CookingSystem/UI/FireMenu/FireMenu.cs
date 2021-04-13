using UnityEngine;

public abstract class FireMenu : MonoBehaviour
{
    [SerializeField] protected Pointer pointerBack;

    public void OpenMenu()
    {
        gameObject.SetActive(true);
    }
    public void CloseMenu()
    {
        gameObject.SetActive(false);
    }
}