using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class NPC : MonoBehaviour
{
    [SerializeField] protected NPCSearch search;

    private Coroutine brainCoroutine = null;
    public bool IsBrainProccess => brainCoroutine != null;

    private bool isAlive = true;

    protected virtual void Awake()
    {
        StartBrain();
    }

    private void StartBrain()
    {
        if (!IsBrainProccess)
        {
            brainCoroutine = StartCoroutine(Brain());
        }
    }
    private IEnumerator Brain()
    {
        while (isAlive)
        {
            yield return search.Proccess();
        }
        //death
        StopBrain();
    }
    [Button]
    private void StopBrain()
    {
        if (IsBrainProccess)
        {
            StopCoroutine(brainCoroutine);
            brainCoroutine = null;

            search.Stop();
        }
    }
}