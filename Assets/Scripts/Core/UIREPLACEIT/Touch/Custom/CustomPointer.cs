using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomPointer : CustomUI
{
    public Pointer pointer;

    public bool IsEnable => pointer.IsEnable;

    public override void OpenButton()
    {
        pointer.IsEnable = true;
        base.OpenButton();
    }
    public override void CloseButton()
    {
        pointer.IsEnable = false;
        base.CloseButton();
    }
}
