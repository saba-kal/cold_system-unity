
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Utils
{
    public static T GetValueOrNull<T>(this T[] list, int index) where T : class
    {
        if (index < 0 || index >= list.Length)
        {
            return null;
        }

        return list[index];
    }

    public static T GetValueOrNull<T>(this List<T> list, int index) where T : class
    {
        if (index < 0 || index >= list.Count)
        {
            return null;
        }

        return list[index];
    }

    public static List<T> ChooseRandomElements<T>(this List<T> list, int count)
    {
        if (count > list.Count)
        {
            throw new ArgumentException($"Cannot choose {count} random items from a list that is only {list.Count} in size");
        }

        if (count == 0)
        {
            return null;
        }

        if (count == list.Count)
        {
            return list;
        }

        var randomElements = new List<T>();
        var copyList = list.ToList(); // Create a copy of the source list to avoid modifying the original list

        for (var i = 0; i < count; i++)
        {
            var randomIndex = UnityEngine.Random.Range(0, copyList.Count);
            randomElements.Add(copyList[randomIndex]);
            copyList.RemoveAt(randomIndex); // Remove the chosen element from the copy
        }

        return randomElements;
    }

    //Credit: https://gist.github.com/johnsoncodehk/2ecb0136304d4badbb92bd0c1dbd8bae
    public static float ClampAngle(float angle, float min, float max)
    {
        float start = (min + max) * 0.5f - 180;
        float floor = Mathf.FloorToInt((angle - start) / 360) * 360;
        return Mathf.Clamp(angle, min + floor, max + floor);
    }
}
