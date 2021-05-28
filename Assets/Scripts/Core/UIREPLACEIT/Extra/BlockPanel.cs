using Sirenix.OdinInspector;

using UnityEngine;

public class BlockPanel : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    public bool IsEnable => panel.activeSelf;

    public void Enable(bool trigger)
    {
        panel.SetActive(trigger);
    }

    [Button]
    public void Enable()
    {
        Enable(true);
    }
    [Button]
    public void Disable()
    {
        Enable(false);
    }
}
