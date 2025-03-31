using Expedition;
using Scientist.Items;
using MoreSlugcats;
using RWCustom;
using Scientist;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scientist.Data;

public static class Player
{
    public static int offlineTime = 0;

    //pf = painlessfruit
    //public static int[] pfEatTimesInACycle;
    //public static int[] pfTime;
    //public static bool[] pfAfterActiveDie;
    public static Dictionary<string, int> pfEatTimesInACycle = new();
    public static Dictionary<string, int> pfTime = new();
    public static Dictionary<string, int> pfDieInActive = new();

    //craftingTime
    public static Dictionary<string, int> craftingTime = new();

#nullable enable
    public static Dictionary<string, ColorfulSprite?> colorfulCreatures = new Dictionary<string, ColorfulSprite?>();

    public static Dictionary<string, AnesthesiaCreature> anesthesiaCreatures = new Dictionary<string, AnesthesiaCreature>();
}

public static class GolbalVariables
{
    public static bool SEnableOldPf = false;
    public static bool SEnableTfKeepShaking = false;
    public static bool SEnableOpenPanelPauseGame = true;
    public static KeyCode SOpenScientistPanelKey = KeyCode.E;

    public static bool isPanelOpen = false;
    public static bool isPanelChanged = false;
}

public class ColorfulSprite
{
    public float secondTotal = -1f;
    public float second = 0f;
    public int counterTotal = 560;
    public int counter;
    public bool enabled = false;
    public LightSource? lightSource;
    public Creature? c;
    public PhysicalObject? po;
    public UpdatableAndDeletable uad;
    public bool getOriginalColors;
    public Color[] originalColors;

    public ColorfulSprite(Creature c, int counter = 0, bool enabled = false, float secondTotal = -1f, LightSource? ls = null)
    {
        this.c = c;
        this.po = null;
        this.uad = c;
        this.counter = counter;
        this.enabled = enabled;
        this.secondTotal = secondTotal;
        this.second = 0f;
        this.lightSource = ls;
        this.getOriginalColors = false;
        this.originalColors = new Color[0];
    }

    public ColorfulSprite(PhysicalObject po, int counter = 0, bool enabled = false, float secondTotal = -1f, LightSource? ls = null)
    {
        this.c = null;
        this.po = po;
        this.uad = po;
        this.counter = counter;
        this.enabled = enabled;
        this.secondTotal = secondTotal;
        this.second = 0f;
        this.lightSource = ls;
        this.getOriginalColors = false;
        this.originalColors = new Color[0];
    }

    public void AddCounter(int amount = 1, float second = 1/40.000f)
    {
        this.counter %= this.counterTotal;
        this.counter += amount;
        this.second += second;
        if (this.enabled && Scientist.ScientistTools.InRange(this.second, this.secondTotal, this.secondTotal + 0.1f))
        {
            this.SetEnabled(false);
        }
    }

    public void SetEnabled(bool enabled)
    {
        this.enabled = enabled;
        this.getOriginalColors = !enabled || (enabled && this.originalColors.Length == 0);
        this.SetLightSource();
    }

    public void SetLightSource()
    {
        if (this.enabled)
        {
            if (this.uad != null && (this.c != null || this.po != null) )
            {
                if (this.lightSource == null)
                {
#pragma warning disable CS8602 // 解引用可能出现空引用。
                    this.lightSource = new((c != null ? c.mainBodyChunk.pos : po.firstChunk.pos), false, Color.red, uad)
                    {
#pragma warning restore CS8602 // 解引用可能出现空引用。
                        requireUpKeep = true,
                        setRad = new float?(300f),
                        setAlpha = new float?(1f)
                    };
                    c?.room.AddObject(this.lightSource);
                    po?.room.AddObject(this.lightSource);
                }
                this.lightSource.stayAlive = true;
#pragma warning disable CS8602 // 解引用可能出现空引用。
                this.lightSource.setPos = new Vector2?(c != null ? c.mainBodyChunk.pos : po.firstChunk.pos);
#pragma warning restore CS8602 // 解引用可能出现空引用。
                this.lightSource.color = Color.HSVToRGB(this.counter / 560f, 0.7f, 0.5f);
                if (this.lightSource.slatedForDeletetion)
                {
                    this.lightSource = null;
                }
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

    public void SetOriginalColors(RoomCamera.SpriteLeaser sLeaser)
    {
        if (this.getOriginalColors && this.originalColors.Length == 0)
        {
            this.originalColors = new Color[sLeaser.sprites.Length];
            for (int i = 0; i < sLeaser.sprites.Length; i++)
            {
                this.originalColors[i] = sLeaser.sprites[i].color;
            }
            this.getOriginalColors = false;
        }
        else if (this.getOriginalColors && this.originalColors.Length != 0)
        {
            for (int i = 0; i < sLeaser.sprites.Length; i++)
            {
                sLeaser.sprites[i].color = this.originalColors[i];
            }
            this.getOriginalColors = false;
        }
    }

    public override string ToString()
    {
        return $"{this.c}<>{this.po}<>{this.counterTotal}<>{this.counter}<>{this.enabled}<>{this.lightSource}";
    }
}

public class AnesthesiaCreature
{
    public int counterTotal = 1600;
    public int counter;
    public Func<int, int, bool> enabled;

    public AnesthesiaCreature(Func<int, int, bool> enabled, int counterTotal = 1600)
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