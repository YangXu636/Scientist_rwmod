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

#nullable enable
    public static Dictionary<string, ColorfulSprite?> colorfulCreatures = new Dictionary<string, ColorfulSprite?>();

    public static Dictionary<string, AnesthesiaCreature> anesthesiaCreatures = new Dictionary<string, AnesthesiaCreature>();
}

public class ColorfulSprite
{
    public int counterTotal = 560;
    public int counter;
    public bool enabled = false;
    public LightSource? lightSource;
    public Creature c;

    public ColorfulSprite(Creature c, bool enabled = false, LightSource? ls = null)
    {
        this.counter = 0;
        this.c = c;
        this.enabled = enabled;
        this.lightSource = ls;
    }

    public void AddCounter(int amount = 1)
    {
        this.counter %= this.counterTotal;
        this.counter += amount;
    }

    public void SetEnabled(bool enabled)
    {
        this.enabled = enabled;
        this.SetLightSource();
    }

    public void SetLightSource()
    {
        if (this.enabled)
        {
            if (this.lightSource == null)
            {
                this.lightSource = new LightSource(c.mainBodyChunk.pos, false, Color.red, c);
                this.lightSource.requireUpKeep = true;
                this.lightSource.setRad = new float?(300f);
                this.lightSource.setAlpha = new float?(1f);
                c.room.AddObject(this.lightSource);
            }
            this.lightSource.stayAlive = true;
            this.lightSource.setPos = new Vector2?(c.mainBodyChunk.pos);
            this.lightSource.color = Color.HSVToRGB(this.counter / 560f, 0.7f, 0.7f);
            if (this.lightSource.slatedForDeletetion)
            {
                this.lightSource = null;
            }
        }
        else 
        {
            this.lightSource = null;
        }
        if (this.lightSource != null && this.lightSource.slatedForDeletetion)
        {
            this.lightSource = null;
        }
    }

    public override string ToString()
    {
        return $"{this.c}<>{this.counterTotal}<>{this.counter}<>{this.enabled}<>{this.lightSource}";
    }
}

public class AnesthesiaCreature
{
    public int counterTotal = 3200;
    public int counter;
    public Func<int, int, bool> enabled;

    public AnesthesiaCreature(Func<int, int, bool> enabled, int counterTotal = 3200)
    {
        this.counter = 0;
        this.enabled = enabled;
        this.counterTotal = counterTotal;
    }

    public void AddCounter(int amount = 1)
    {
        this.counter = Mathf.Min(this.counter + amount, this.counterTotal);
    }

    public bool IsEnabled()
    {
        return this.enabled(this.counter, this.counterTotal);
    }

    public override string ToString()
    {
        return $"{this.counterTotal}<>{this.counter}<>{this.enabled}";
    }
}