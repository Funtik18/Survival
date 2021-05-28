using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

public class PIRadialMenu : MonoBehaviour
{
    public UnityAction onOpened;
    public UnityAction onClosed;

    private List<Transform> transforms = new List<Transform>();

    public void OpenMenu()
    {
        onOpened?.Invoke();

        gameObject.SetActive(true);
    }
    public void CloseMenu()
    {
        gameObject.SetActive(false);

        onClosed?.Invoke();
    }

    public void UpdateMenu()
    {
        GetAll();

        for (int i = 0; i < transforms.Count; i++)
        {
            float theta = ((2 * Mathf.PI) / transforms.Count) * i;

            float xPos = Mathf.Sin(theta);
            float yPos = Mathf.Cos(theta);

            float distanceBtwOptions = 100;
            if (transforms.Count == 1) distanceBtwOptions = 0;
            else if (transforms.Count > 3 && transforms.Count <= 5) distanceBtwOptions = 150;
            else if (transforms.Count > 5 && transforms.Count <= 8) distanceBtwOptions = 200;
            else if (transforms.Count > 8 && transforms.Count <= 10) distanceBtwOptions = 250;
            transforms[i].localPosition = new Vector3(xPos, yPos, 0) * distanceBtwOptions;
        }
    }
    private void Dispose()
    {
        transforms.Clear();
    }
    private void GetAll()
    {
        Dispose();
        for (int i = 0; i < transform.childCount; i++)
        {
            transforms.Add(transform.GetChild(i));
        }
    }
}