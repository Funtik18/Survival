using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomHolder : CustomUI
{
    public PointerHold pointer;

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
