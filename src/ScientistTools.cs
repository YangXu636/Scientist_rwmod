using SlugBase.DataTypes;
using System;
using System.Collections.Generic;
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

    public static bool DictionaryKeyHasValue<K, V>(Dictionary<K, V> dictionary, K key, V value)
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

    public static int PlayerIndex(Player player) => player.abstractCreature.ID.number;

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

    public static Vector2 Vector2VerticalNormalized(Vector2 vector)
    {
        return new Vector2(vector.y, -vector.x).normalized;
    }

    public static int[] ArrayAdd(int[] array, int item)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] += item;
        }
        return array;
    }

    public static string ExtenumTypeString(AbstractPhysicalObject apo) => apo.type == AbstractPhysicalObject.AbstractObjectType.Creature ? ExtenumTypeString(apo.realizedObject as Creature) : apo.type.value;

    public static string ExtenumTypeString(PhysicalObject po) => ExtenumTypeString(po.abstractPhysicalObject);

    public static string ExtenumTypeString(Creature c) => ExtenumTypeString(c.abstractCreature);

    public static string ExtenumTypeString(AbstractCreature ac) => ac.type.value;

    public static string FeaturesTypeString(AbstractPhysicalObject apo) => apo.type == AbstractPhysicalObject.AbstractObjectType.Creature ? FeaturesTypeString(apo.realizedObject as Creature) : apo.ToString();

    public static string FeaturesTypeString(PhysicalObject po) => FeaturesTypeString(po.abstractPhysicalObject);

    public static string FeaturesTypeString(Creature c) => FeaturesTypeString(c.abstractCreature);

    public static string FeaturesTypeString(AbstractCreature ac) => ac.ToString();

    public static T[] Memset<T>(T[] array, T value)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = value;
        }
        return array;
    }
}
