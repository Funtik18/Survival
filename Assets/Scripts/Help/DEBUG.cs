using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Sirenix.OdinInspector;

public class DEBUG : MonoBehaviour
{
    [SerializeField] private Image background;
    [SerializeField] private TMPro.TextMeshProUGUI txtFPS;
	[SerializeField] private TMPro.TextMeshProUGUI txtTIME;

    private void Update()
    {
        txtFPS.text = "FPS : " + (int)(1f / Time.unscaledDeltaTime);
        txtTIME.text = "TIME : " + GeneralTime.Instance.ToString();
    }
}
