using SlugBase.DataTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager;
using UnityEngine;

namespace Scientist;

public class ScientistTools
{
    public static bool splitScreenCoopEnabled;

    public static bool Probability(float chance)
    {
        int digit10 = (int)Mathf.Pow(10, GetNumberOfDecimalPlaces(chance));
        return (UnityEngine.Random.Range(1, digit10 + 1) / (float)digit10) <= chance;
    }

    public static bool InRange(float value, float min, float max, bool inclusiveLeft = true, bool inclusiveRight = true) => (inclusiveLeft? value >= min : value > min) && (inclusiveRight? value <= max : value < max);

    public static float GetFractionalPart(float num)
    {
        int digit10 = (int)Mathf.Pow(10, GetNumberOfDecimalPlaces(num));
        return (float)((float)(num * digit10 % digit10) / digit10);
    }

    public static int GetNumberOfDecimalPlaces<T>(T decimalV) // where T : float, double, decimal
    {
        string[] temp = decimalV.ToString().Split('.');
        if (temp.Length == 2 && temp[1].Length > 0)
        {
            int index = temp[1].Length - 1;
            while (temp[1][index] == '0' && index-- > 0) ;
            return index + 1;
        }
        return 0;
    }

    public static int PlayerIndex(Player player) => player.playerState.playerNumber;// player.abstractCreature.ID.number;

    public static Color ColorFromHex(string hex)
    {
        try
        {
            if (hex.StartsWith("#"))
            {
                hex = hex.Substring(1);
            }
            if (hex.Length != 6 && hex.Length != 8)
            {
                return Color.white;
            }
            byte br = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte bg = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte bb = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            float r = br / 255f;
            float g = bg / 255f;
            float b = bb / 255f;
            if (hex.Length == 6)
            {
                return new Color(r, g, b);
            }
            byte cc = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
            float a = cc / 255f;
            return new Color(r, g, b, a);
        }
        catch (Exception ex)
        {
            ScientistLogger.Warning("Could not parse color from hex: " + ex.Message + " " + hex + "ScienistTools.ColorFromHex");
            return Color.white;
        }
    }

    public static Color ColorReverse(Color color) => new Color(1f - color.r, 1f - color.g, 1f - color.b, color.a);

    public static Color ColorReverse(Color color, float a) => new Color(1f - color.r, 1f - color.g, 1f - color.b, a);

    public static bool ColorIsLight(Color color) => (0.299 * color.r + 0.587 * color.g + 0.114 * color.b) > 0.5;

    public static Color ColorContrast(Color color) => ColorIsLight(color) ? Color.black : Color.white;

    public static Color ColorContrast(Color color, float a) => ColorIsLight(color) ? new Color(0, 0, 0, a) : new Color(1f, 1f, 1f, a);

    public static Color ColorChangeAlpha(Color color, float a) => new Color(color.r, color.g, color.b, a);

    public static Vector2 AngleToVector2(float angle) => new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

    public static Vector2 RandomAngleVector2(params float[][] range)
    {
        if (range.Length == 0) { return AngleToVector2(UnityEngine.Random.Range(0f, 360f)); }
        int rangeIndex = UnityEngine.Random.Range(0, range.Length);
        return AngleToVector2(UnityEngine.Random.Range(range[rangeIndex][0] % 360f, range[rangeIndex][1] % 360f));
    }

    public static string ExtenumTypeString(AbstractPhysicalObject apo) => apo.type == AbstractPhysicalObject.AbstractObjectType.Creature ? ExtenumTypeString(apo.realizedObject as Creature) : apo.type.value;

    public static string ExtenumTypeString(PhysicalObject po) => ExtenumTypeString(po.abstractPhysicalObject);

    public static string ExtenumTypeString(Creature c) => ExtenumTypeString(c.abstractCreature);

    public static string ExtenumTypeString(AbstractCreature ac) => ac.type.value;

    public static string FeaturesTypeString(AbstractPhysicalObject apo) => apo.type == AbstractPhysicalObject.AbstractObjectType.Creature ? FeaturesTypeString(apo.realizedObject as Creature) : $"{apo.ID}_{apo.type.value}";

    public static string FeaturesTypeString(PhysicalObject po) => FeaturesTypeString(po.abstractPhysicalObject);

    public static string FeaturesTypeString(Creature c) => FeaturesTypeString(c.abstractCreature);

    public static string FeaturesTypeString(AbstractCreature ac) =>$"{ac.ID}_{ac.type.value}";
}

public static class ArrayExtensions
{

    public static int[] AddAll(this int[] array, int item)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] += item;
        }
        return array;
    }

    public static T[] SetAll<T>(this T[] array, T value)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = value;
        }
        return array;
    }

    public static bool AllEqualValue<T>(this T[] array, T value)
    {
        for (int i = 0; i < array.Length; i++)
            if (!array[i].Equals(value))
                return false;
        return true;
    }

    public static T FindDifferent<T>(this T[] array)
    {
        Dictionary<T, int> dict = new Dictionary<T, int>();
        for (int i = 0; i < array.Length; i++)
        {
            if (!dict.ContainsKey(array[i]))
                dict.Add(array[i], 0);
            dict[array[i]]++;
        }
        return dict.Where(x => x.Value == 1).ToArray().First().Key;
    }

    public static T[,] SetAll<T>(this T[,] array, T value, int layerIStart = -1, int layerIEnd = -1, int layerJStart = -1, int layerJEnd = -1)
    {
        for (int i = layerIStart <= -1 ? 0 : (layerIStart > array.GetLength(0) - 1 ? array.GetLength(0) - 1 : layerIStart); i <= (layerIEnd <= -1 ? array.GetLength(0) - 1 : (layerIEnd > array.GetLength(0) - 1 ? array.GetLength(0) - 1 : layerIEnd) ); i++)
        {
            for (int j = layerJStart <= -1 ? 0 : (layerJStart > array.GetLength(1) - 1 ? array.GetLength(1) - 1 : layerJStart); j <= (layerJEnd <= -1 ? array.GetLength(1) - 1 : (layerJEnd > array.GetLength(1) - 1 ? array.GetLength(1) - 1 : layerJEnd) ); j++)
            {
                array[i, j] = value;
            }
        }
        return array;
    }

    public static T[][] SetAll<T>(this T[][] array, T value)
    {
        for (int i = 0; i < array.Length; i++)
        {
            for (int j = 0; j < array[i].Length; j++)
            {
                array[i][j] = value;
            }
        }
        return array;
    }
}

public static class DictionaryExtensions
{
    public static bool KeyIsValue<K, V>(this Dictionary<K, V> dictionary, K key, V value)
    {
        if (dictionary == null) { return false; }
        try
        {
            return (dictionary.ContainsKey(key) && dictionary[key].Equals(value));
        }
        catch (Exception)
        {
            return false;
        }
    }

    public static Dictionary<T, int> AddAll<T>(this Dictionary<T, int> dict, int item)
    {
        List<T> keys = new(dict.Keys);
        for (int i = 0; i < keys.Count; i++)
        {
            dict[keys[i]] += item;
        }
        return dict;
    }
}

public static class ListExtensions
{
    public static bool Empty<T>(this List<T> list)
    {
        return list == null || list.Count == 0;
    }

    public static List<T> SetAll<T>(this List<T> list, T item)
    {
        for (int i = 0; i < list.Count; i++)
        {
            list[i] = item;
        }
        return list;
    }

    /// <summary>
    /// 查询列表中是否有除此元素外的其他元素
    /// </summary>
    /// <typeparam name="T">列表元素类型</typeparam>
    /// <param name="list">列表</param>
    /// <param name="item">元素</param>
    /// <returns>列表中是否有除此元素外的其他元素</returns>
    public static bool HasItemBesides<T>(this List<T> list, T item)
    {
        if (list == null || list.Count == 0) { return false; }
        if (list.Count == 1) { return !list[0].Equals(item); }
        return true;
    }

    public static List<T> AddSafe<T>(this List<T> list, T item)
    {
        list ??= new List<T>();
        if (!list.Contains(item)) { list.Add(item); }
        return list;
    }
}

public static class Vector2Extensions
{
    public static Vector2 VerticalNormalized(this Vector2 vector)
    {
        return new Vector2(vector.y, -vector.x).normalized;
    }
}

public static class QueueExtensions
{
    public static bool Empty(this Queue queue)
    {
        return queue == null || (queue != null && queue.Count == 0);
    }
}