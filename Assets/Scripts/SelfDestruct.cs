using System.Collections;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    public float timeLeft = 1f;

    public void StartDestruct()
    {
        StopAllCoroutines();
        StartCoroutine(Destruct());
    }
    private IEnumerator Destruct()
    {
        yield return new WaitForSeconds(timeLeft);
        ObjectPool.Instance.ReturnGameObject(gameObject);
    }
}