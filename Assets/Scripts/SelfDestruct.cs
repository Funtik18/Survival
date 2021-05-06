using System.Collections;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    public float timeLeft = 1f;

    private WaitForSeconds seconds;

    private void Awake()
    {
        seconds = new WaitForSeconds(timeLeft);
    }

    public void StartDestruct()
    {
        StopAllCoroutines();
        StartCoroutine(Destruct());
    }
    private IEnumerator Destruct()
    {
        yield return seconds;
        ObjectPool.Instance.ReturnGameObject(gameObject);
    }
}