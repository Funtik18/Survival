using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalSelector : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI main;
    [SerializeField] private TMPro.TextMeshProUGUI textHelper;

    [SerializeField] private Animator animator;

    [SerializeField] private bool isInverse = false;

    public void UpdateUI(string text)
    {
        textHelper.text = main.text;
        main.text = text;
    }

    public void AnimatorDirection(bool forward)
    {
        animator.Play(null);
        animator.StopPlayback();

        if (forward)
        {
            if (isInverse)
            {
                animator.Play("Previous");
            }
            else
            {
                animator.Play("Forward");
            }
        }
        else
        {
            if (isInverse)
            {
                animator.Play("Forward");
            }
            else
            {
                animator.Play("Previous");
            }
        }
    }
}