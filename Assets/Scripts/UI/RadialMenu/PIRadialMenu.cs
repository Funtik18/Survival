using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PIRadialMenu : MonoBehaviour
{
    [Range(1, 10)]
    [SerializeField] private int optionCount = 1;

    [Min(0)]
    [SerializeField] private float distanceBtwOptions = 100f;


    [SerializeField] private PIRadialOption optionPrefab;
    [SerializeField] private CanvasGroup canvasGroup;

    [SerializeField] private List<PIRadialOption> options = new List<PIRadialOption>();

    public void Setup(RadialOptionData[] optionsData)
    {
        int count = 0;
        for (int i = 0; i < optionsData.Length; i++)
        {
            if (!optionsData[i].IsNull)
            {
                count++;
            }
        }

        optionCount = count;
        UpdateMenu();

        for (int i = 0; i < options.Count; i++)
        {
            options[i].onChoosen += optionsData[i].EventInvoke;
            options[i].SetIcon(optionsData[i].optionIcon);
        }
    }


    private void GetAllOptions()
    {
        foreach (Transform child in transform)
        {
            options.Add(child.GetComponent<PIRadialOption>());
        }
    }
    [Button]
    private void DisposeOptions()
    {
        options.Clear();

        while (transform.childCount != 0)
        {
            DestroyImmediate(transform.GetChild(transform.childCount - 1).gameObject);
        }
    }

    [Button]
    private void UpdateMenu()
    {
        DisposeOptions();

        for (int i = 0; i < optionCount; i++)
        {
#if UNITY_EDITOR
            PrefabUtility.InstantiatePrefab(optionPrefab, transform);
#else
                    Instantiate(optionPrefab, transform);
#endif
        }


        GetAllOptions();

        for (int i = 0; i < options.Count; i++)
        {
            PIRadialOption option = options[i];

            float theta = ((2 * Mathf.PI) / options.Count) * i;

            float xPos = Mathf.Sin(theta);
            float yPos = Mathf.Cos(theta);

            if(options.Count > 1)
            {
                distanceBtwOptions = 100;
                if (options.Count > 3) distanceBtwOptions = 150;
                if (options.Count > 5) distanceBtwOptions = 200;
                if (options.Count > 8) distanceBtwOptions = 250;
                option.transform.localPosition = new Vector3(xPos, yPos, 0) * distanceBtwOptions;
            }
        }

    }

    [Button]
    public void OpenMenu()
    {
        canvasGroup.IsEnabled(true);
    }
    [Button]
    public void CloseMenu()
    {
        canvasGroup.IsEnabled(false);
    }
}
