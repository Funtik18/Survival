using UnityEngine;

public static class ExtensionRandom
{
    public static int RandomNumBtw(this Vector2Int minMax)
    {
        return Random.Range(minMax.x, minMax.y);
    }
    public static float RandomNumBtw(this Vector2 minMax)
    {
        return Random.Range(minMax.x, minMax.y);
    }


    public static Vector3 RandomPointInAnnulus(Vector3 origin, float minRadius, float maxRadius)
    {
        Vector3 point = GetRandomPointInCircleXZNoZero();

        float distance = Random.Range(minRadius, maxRadius);

        Vector3 result = origin + point * distance;

        return result;
    }

    public static Vector3 GetRandomPointInCircleXZ() => new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
    public static Vector3 GetRandomPointInCircleXZNoZero()
    {
        Vector3 result = GetRandomPointInCircleXZ();

        if (result.x == 0 && result.y == 0)
            return GetRandomPointInCircleXZNoZero();

        return result;
    }


    public static Vector3 RandomPointInBox(Vector3 center, Vector3 size)
    {
        return center + new Vector3(
           (Random.value - 0.5f) * size.x,
           (Random.value - 0.5f) * size.y,
           (Random.value - 0.5f) * size.z
        );
    }
}