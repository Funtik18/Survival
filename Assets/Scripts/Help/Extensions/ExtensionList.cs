using System;
using System.Collections.Generic;

public static class ExtensionList
{
    private static Random rnd = new Random();

    public static T GetRandomItem<T>(this IList<T> list) => list[rnd.Next(0, list.Count)];

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rnd.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}