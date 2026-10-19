using Expedition;
using Scientist.Items;
using MoreSlugcats;
using RWCustom;
using Scientist;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;
using System.Reflection;
using Scientist.Debug;

namespace Scientist.Data;

public static class PlayerVariables
{
    public static int offlineTime = 0;

    //pf = painlessfruit
    public static Dictionary<string, int> pfEatTimesInACycle = new();
    public static Dictionary<string, int> pfTime = new();
    public static Dictionary<string, int> pfDieInActive = new();

    //craftingTime
    public static Dictionary<string, int> craftingTime = new();

#nullable enable
    public static Dictionary<string, ColorfulSprite?> colorfulCreatures = new Dictionary<string, ColorfulSprite?>();
#nullable disable

    public static Dictionary<string, AnesthesiaCreature> anesthesiaCreatures = new Dictionary<string, AnesthesiaCreature>();
}

public static class Miscellaneous
{
    public static Dictionary<string, Handle.BeamExpansion> beInRoom = new();
    public static int beamAllCounter = 0;
    public static Hashtable beamID = new Hashtable();
    public static int GetNewBeamID() { return ++beamAllCounter; }
}

public static class GlobalVariables
{
    public static bool SEnableOldPf = false;
    public static bool SEnableTfKeepShaking = false;
    public static bool SEnableOpenPanelPauseGame = true;
    public static KeyCode SOpenScientistPanelKey = KeyCode.E;
    public static bool SUnlockAllContent = false;
    public static bool SEnableDebug = false;

    public static bool isPanelOpen = false;
    public static bool isPanelChanged = false;

    public static List<AbstractPhysicalObject.AbstractObjectType> apoModTypeList = new();
    public static List<AbstractPhysicalObject.AbstractObjectType> apoAllTypeList = new();
    public static List<AbstractPhysicalObject.AbstractObjectType> apoEnableTypeList = new();

    public static bool BlockBeamObject(object obj) { return false; }

    public static CacheData<KeyValuePair<int, float>, float> CD_SbS_GetECGsOffset = new(100000);
}

public static class DebugVariables
{
    //public static bool enable = true;

    public static bool changed = false;

    public static bool showBodyChunks = false;
    //public static ConditionalWeakTable<BodyChunk, FSprite> showBodyChunkSprites = new ConditionalWeakTable<BodyChunk, FSprite>();
    public static Dictionary<string, ShowBodychunk> ShowbodychunksList = new();
    public static bool[,] ShowbodychunksGrid = new bool[100, 100].SetAll(false);
    public static string ShowbodychunksRoomName = "";

    //public static bool addPhysicsobjectInRoom = false;
    //public static string addPhysicsobjectInRoomType = "";
    //public static FSprite addPhysicsobjectInRoomSprite = new FSprite("Futile_White", true);

    public static bool playerNoDie = false;
    public static bool logWhenPlayerDie = false;
    public static bool playerNoStun = false;
    public static bool logWhenPlayerStun = false;

    public static bool NofriendInjuries = false;
}

public struct SmoothTransform
{
    public const int frameRate = 40;            //
    public SmoothChange tp = new();  //timePercent
    public Func<float, float> func = new Func<float, float>(x => x);

    public SmoothTransform()
    {

    }
}

public struct SmoothChange
{
    public float value;
    public float destination;
    public float delta;
    public bool ing = false;

    public SmoothChange(float value = 0f, float destination = 0, float delta = 0.01f)
    {
        this.value = value;
        this.destination = destination;
        this.delta = delta;
    }

    public void Update()
    {
        if (!this.ing) return;
        int sign = Math.Sign(this.destination - this.value);
        this.value += sign * this.delta;
        if (this.value * sign > this.destination * sign)
        {
            this.value = this.destination;
            this.ing = false;
        }
    }

    public void SetDelta(float delta)
    {
        this.delta = delta;
    }

    public void GotoF(float destination)
    {
        this.destination = destination;
        this.ing = true;
    }

    public float gvFolat { get { return this.value; } }
    public int gvInt { get { return (int)this.value; } }
    public double gvDouble { get { return (double)this.value; } }
}

public class ColorfulSprite
{
    public float secondTotal = -1f;
    public float second = 0f;
    public int counterTotal = 560;
    public int counter;
    public bool enabled = false;
#nullable enable
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
#nullable disable

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
                    this.lightSource = new((c != null ? c.mainBodyChunk.pos : po.firstChunk.pos), false, Color.red, uad)
                    {
                        requireUpKeep = true,
                        setRad = new float?(300f),
                        setAlpha = new float?(1f)
                    };
                    c?.room.AddObject(this.lightSource);
                    po?.room.AddObject(this.lightSource);
                }
                this.lightSource.stayAlive = true;
                this.lightSource.setPos = new Vector2?(c != null ? c.mainBodyChunk.pos : po.firstChunk.pos);
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


public struct CacheData<T, K>
{
    public int _maxSize;
    public Dictionary<T, K> _cache;

    public CacheData(int maxSize)
    {
        _maxSize = maxSize;
        _cache = new Dictionary<T, K>();
    }

    public readonly void ClearCache()
    {
        lock (_cache)
        {
            _cache.Clear();
        }
    }

    public void AddToCache(T key, K value)
    {
        lock (_cache)
        {
            if (_cache.Count < _maxSize)
            {
                _cache.Add(key, value);
            }
        }
    }

    public bool TryGetFromCache(T key, out K value)
    {
        lock (_cache)
        {
            return _cache.TryGetValue(key, out value);
        }
    }
}