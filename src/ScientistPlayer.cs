using Expedition;
using items;
using MoreSlugcats;
using RWCustom;
using Scientist;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scientist;

public static class ScientistPlayer
{
    public static int offlineTime = 0;

    //pf = painlessfruit
    public static int[] pfEatTimesInACycle;
    public static int[] pfTime;
    public static bool[] pfAfterActiveDie;

    public static Dictionary<string, ColorfulSprite> colorfulCreatures = new Dictionary<string, ColorfulSprite>();
}

public class ColorfulSprite
{
    public int counterTotal = 560;
    public int counter;
    public bool enabled = false;

    public ColorfulSprite(bool enabled = false)
    {
        this.counter = 0;
        this.enabled = enabled;
    }

    public void AddCounter(int amount = 1)
    {
        this.counter %= this.counterTotal;
        this.counter += amount;
    }

    public void SetEnabled(bool enabled)
    {
        this.enabled = enabled;
    }

    public override string ToString()
    {
        return $"{this.counterTotal}<>{this.counter}<>{this.enabled}";
    }
}