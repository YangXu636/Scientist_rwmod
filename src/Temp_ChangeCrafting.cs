using System;
using System.Collections;
using System.Collections.Generic;
using MoreSlugcats;
using RWCustom;
using UnityEngine;

namespace Scientist;

public class Temp_ChangeCrafting
{
    public static Dictionary<AbstractPhysicalObject.AbstractObjectType, int> objectsLibrary;
    public static Dictionary<CreatureTemplate.Type, int> critsLibrary;
    public static Temp_ChangeCrafting.CraftDat[,] craftingGrid_ObjectsOnly;
    public static Temp_ChangeCrafting.CraftDat[,] craftingGrid_CritterObjects;
    public static Temp_ChangeCrafting.CraftDat[,] craftingGrid_CrittersOnly;
    public static bool showDebug;

    public static Dictionary<string, AbstractPhysicalObject.AbstractObjectType> s2o;
    public static Dictionary<string, CreatureTemplate.Type> s2c;

    public static Hashtable s2t = new Hashtable();
    public static Hashtable t2s = new Hashtable();

    public struct CraftDat
    {
        public AbstractPhysicalObject.AbstractObjectType type;
        public CreatureTemplate.Type crit;
        public bool enabled;
        public CraftDat(AbstractPhysicalObject.AbstractObjectType typeResult, CreatureTemplate.Type critResult)
        {
            this.enabled = true;
            this.type = AbstractPhysicalObject.AbstractObjectType.Creature;
            this.crit = CreatureTemplate.Type.StandardGroundCreature;
            if (critResult != null)
            {
                this.crit = critResult;
                return;
            }
            if (typeResult != null)
            {
                this.type = typeResult;
                return;
            }
            this.enabled = false;
        }
    }

    static Temp_ChangeCrafting()
    {     //打表x1
        int num = 0;
        Temp_ChangeCrafting.objectsLibrary = new Dictionary<AbstractPhysicalObject.AbstractObjectType, int>();
        s2t.Add("Null", null);
        Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Rock] = num;
        num++;
        Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlareBomb] = num;
        num++;
        Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.VultureMask] = num;
        num++;
        Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.PuffBall] = num;
        num++;
        Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DangleFruit] = num;
        num++;
        Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SLOracleSwarmer] = num;
        num++;
        Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SSOracleSwarmer] = num;
        num++;
        Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DataPearl] = num;
        num++;
        Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.WaterNut] = num;
        num++;
        Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.JellyFish] = num;
        num++;
        Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Lantern] = num;
        num++;
        Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.KarmaFlower] = num;
        num++;
        Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Mushroom] = num;
        num++;
        Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant] = num;
        num++;
        Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SlimeMold] = num;
        num++;
        Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlyLure] = num;
        num++;
        Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb] = num;
        num++;
        Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant] = num;
        num++;
        Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg] = num;
        num++;
        Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg] = num;
        num++;
        Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass] = num;
        num++;
        Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass] = num;
        num++;
        Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb] = num;
        num++;
        Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg] = num;
        num++;
        Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed] = num;
        num++;
        Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck] = num;
        num++;
        Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck] = num;
        num++;
        Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed] = num;
        num++;
        Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach] = num;
        num++;
        //新增物品
        Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Spear] = num;
        num++;

        int num2 = 0;
        Temp_ChangeCrafting.critsLibrary = new Dictionary<CreatureTemplate.Type, int>();
        Temp_ChangeCrafting.critsLibrary[CreatureTemplate.Type.VultureGrub] = num2;
        num2++;
        Temp_ChangeCrafting.critsLibrary[CreatureTemplate.Type.SmallCentipede] = num2;
        num2++;
        Temp_ChangeCrafting.critsLibrary[CreatureTemplate.Type.SmallNeedleWorm] = num2;
        num2++;
        Temp_ChangeCrafting.critsLibrary[CreatureTemplate.Type.Hazer] = num2;
        num2++;
        Temp_ChangeCrafting.critsLibrary[CreatureTemplate.Type.Fly] = num2;
        num2++;
        //新增生物

        s2t.Add("Rock", AbstractPhysicalObject.AbstractObjectType.Rock);
        s2t.Add("FlareBomb", AbstractPhysicalObject.AbstractObjectType.FlareBomb);
        s2t.Add("VultureMask", AbstractPhysicalObject.AbstractObjectType.VultureMask);
        s2t.Add("PuffBall", AbstractPhysicalObject.AbstractObjectType.PuffBall);
        s2t.Add("DangleFruit", AbstractPhysicalObject.AbstractObjectType.DangleFruit);
        s2t.Add("SLOracleSwarmer", AbstractPhysicalObject.AbstractObjectType.SLOracleSwarmer);
        s2t.Add("SSOracleSwarmer", AbstractPhysicalObject.AbstractObjectType.SSOracleSwarmer);
        s2t.Add("DataPearl", AbstractPhysicalObject.AbstractObjectType.DataPearl);
        s2t.Add("WaterNut", AbstractPhysicalObject.AbstractObjectType.WaterNut);
        s2t.Add("JellyFish", AbstractPhysicalObject.AbstractObjectType.JellyFish);
        s2t.Add("Lantern", AbstractPhysicalObject.AbstractObjectType.Lantern);
        s2t.Add("KarmaFlower", AbstractPhysicalObject.AbstractObjectType.KarmaFlower);
        s2t.Add("Mushroom", AbstractPhysicalObject.AbstractObjectType.Mushroom);
        s2t.Add("FirecrackerPlant", AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant);
        s2t.Add("SlimeMold", AbstractPhysicalObject.AbstractObjectType.SlimeMold);
        s2t.Add("FlyLure", AbstractPhysicalObject.AbstractObjectType.FlyLure);
        s2t.Add("ScavengerBomb", AbstractPhysicalObject.AbstractObjectType.ScavengerBomb);
        s2t.Add("SporePlant", AbstractPhysicalObject.AbstractObjectType.SporePlant);
        s2t.Add("EggBugEgg", AbstractPhysicalObject.AbstractObjectType.EggBugEgg);
        s2t.Add("NeedleEgg", AbstractPhysicalObject.AbstractObjectType.NeedleEgg);
        s2t.Add("BubbleGrass", AbstractPhysicalObject.AbstractObjectType.BubbleGrass);
        s2t.Add("OverseerCarcass", AbstractPhysicalObject.AbstractObjectType.OverseerCarcass);
        s2t.Add("SingularityBomb", MoreSlugcatsEnums.AbstractObjectType.SingularityBomb);
        s2t.Add("FireEgg", MoreSlugcatsEnums.AbstractObjectType.FireEgg);
        s2t.Add("Seed", MoreSlugcatsEnums.AbstractObjectType.Seed);
        s2t.Add("GooieDuck", MoreSlugcatsEnums.AbstractObjectType.GooieDuck);
        s2t.Add("LillyPuck", MoreSlugcatsEnums.AbstractObjectType.LillyPuck);
        s2t.Add("GlowWeed", MoreSlugcatsEnums.AbstractObjectType.GlowWeed);
        s2t.Add("DandelionPeach", MoreSlugcatsEnums.AbstractObjectType.DandelionPeach);
        s2t.Add("Spear", AbstractPhysicalObject.AbstractObjectType.Spear);
        s2t.Add("VultureGrub", CreatureTemplate.Type.VultureGrub);
        s2t.Add("SmallCentipede", CreatureTemplate.Type.SmallCentipede);
        s2t.Add("SmallNeedleWorm", CreatureTemplate.Type.SmallNeedleWorm);
        s2t.Add("Hazer", CreatureTemplate.Type.Hazer);
        s2t.Add("Fly", CreatureTemplate.Type.Fly);
        foreach (DictionaryEntry de in s2t) { t2s[de.Value] = de.Key; }

        Temp_ChangeCrafting.craftingGrid_ObjectsOnly = new Temp_ChangeCrafting.CraftDat[num, num];
        Temp_ChangeCrafting.craftingGrid_CritterObjects = new Temp_ChangeCrafting.CraftDat[num2, num];
        Temp_ChangeCrafting.craftingGrid_CrittersOnly = new Temp_ChangeCrafting.CraftDat[num2, num2];
        Temp_ChangeCrafting.InitCraftingLibrary();
    }

    public static void SetLibraryData(AbstractPhysicalObject.AbstractObjectType objectA, AbstractPhysicalObject.AbstractObjectType objectB, AbstractPhysicalObject.AbstractObjectType resultType, CreatureTemplate.Type resultCritter)
    {
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[objectA], Temp_ChangeCrafting.objectsLibrary[objectB], 0, resultType, resultCritter);
    }

    public static void SetLibraryData(CreatureTemplate.Type critterA, AbstractPhysicalObject.AbstractObjectType objectB, AbstractPhysicalObject.AbstractObjectType resultType, CreatureTemplate.Type resultCritter)
    {
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[critterA], Temp_ChangeCrafting.objectsLibrary[objectB], 1, resultType, resultCritter);
    }

    public static void SetLibraryData(CreatureTemplate.Type critterA, CreatureTemplate.Type critterB, AbstractPhysicalObject.AbstractObjectType resultType, CreatureTemplate.Type resultCritter)
    {
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[critterA], Temp_ChangeCrafting.critsLibrary[critterB], 2, resultType, resultCritter);
    }

    public static void SetLibraryData(int x, int y, int tableSelect, AbstractPhysicalObject.AbstractObjectType resultType, CreatureTemplate.Type resultCritter)
    {
        if (Temp_ChangeCrafting.showDebug)
        {
            /*Custom.Log(new string[]
            {
                    string.Format("CRAFTTABLE: T {0} X {1} Y {2} = {3} - {4}", new object[]
                    {
                        tableSelect,
                        x,
                        y,
                        resultType,
                        resultCritter
                    })
            });*/
        }
        if (tableSelect == 0)
        {
            Temp_ChangeCrafting.craftingGrid_ObjectsOnly[x, y] = new Temp_ChangeCrafting.CraftDat(resultType, resultCritter);
            Temp_ChangeCrafting.craftingGrid_ObjectsOnly[y, x] = new Temp_ChangeCrafting.CraftDat(resultType, resultCritter);
            return;
        }
        if (tableSelect == 1)
        {
            Temp_ChangeCrafting.craftingGrid_CritterObjects[x, y] = new Temp_ChangeCrafting.CraftDat(resultType, resultCritter);
            return;
        }
        if (tableSelect == 2)
        {
            Temp_ChangeCrafting.craftingGrid_CrittersOnly[x, y] = new Temp_ChangeCrafting.CraftDat(resultType, resultCritter);
            Temp_ChangeCrafting.craftingGrid_CrittersOnly[y, x] = new Temp_ChangeCrafting.CraftDat(resultType, resultCritter);
        }
    }

    public static Temp_ChangeCrafting.CraftDat GetFilteredLibraryData(Creature.Grasp graspA, Creature.Grasp graspB)
    {
        AbstractPhysicalObject.AbstractObjectType abstractObjectType = graspA.grabbed.abstractPhysicalObject.type;
        AbstractPhysicalObject.AbstractObjectType abstractObjectType2 = graspB.grabbed.abstractPhysicalObject.type;
        if (abstractObjectType == AbstractPhysicalObject.AbstractObjectType.WaterNut && graspA.grabbed is WaterNut) //泡水果 -> 石头
        {
            abstractObjectType = AbstractPhysicalObject.AbstractObjectType.Rock;
        }
        if (abstractObjectType2 == AbstractPhysicalObject.AbstractObjectType.WaterNut && graspB.grabbed is WaterNut)
        {
            abstractObjectType2 = AbstractPhysicalObject.AbstractObjectType.Rock;
        }
        if (abstractObjectType == AbstractPhysicalObject.AbstractObjectType.PebblesPearl) // 五块卵石的珍珠 -> 珍珠
        {
            abstractObjectType = AbstractPhysicalObject.AbstractObjectType.DataPearl;
        }
        if (abstractObjectType2 == AbstractPhysicalObject.AbstractObjectType.PebblesPearl)
        {
            abstractObjectType2 = AbstractPhysicalObject.AbstractObjectType.DataPearl;
        }
        if (abstractObjectType == AbstractPhysicalObject.AbstractObjectType.SLOracleSwarmer) // 神经元？
        {
            abstractObjectType = AbstractPhysicalObject.AbstractObjectType.SSOracleSwarmer;
        }
        if (abstractObjectType2 == AbstractPhysicalObject.AbstractObjectType.SLOracleSwarmer)
        {
            abstractObjectType2 = AbstractPhysicalObject.AbstractObjectType.SSOracleSwarmer;
        }
        if (graspA.grabbed is Creature && graspB.grabbed is Creature)
        {
            return Temp_ChangeCrafting.GetLibraryData((graspA.grabbed as Creature).abstractCreature.creatureTemplate.type, (graspB.grabbed as Creature).abstractCreature.creatureTemplate.type);
        }
        if (graspA.grabbed is Creature)
        {
            return Temp_ChangeCrafting.GetLibraryData((graspA.grabbed as Creature).abstractCreature.creatureTemplate.type, abstractObjectType2);
        }
        if (graspB.grabbed is Creature)
        {
            return Temp_ChangeCrafting.GetLibraryData((graspB.grabbed as Creature).abstractCreature.creatureTemplate.type, abstractObjectType);
        }
        return Temp_ChangeCrafting.GetLibraryData(abstractObjectType, abstractObjectType2);
    }

    public static Hashtable GetItems(string a, string b)
    {
        Hashtable ans = new Hashtable();
        if (a == null || b == null || !s2t.ContainsKey(a) || !s2t.ContainsKey(b))
        {
            Console.WriteLine($"Could not find object \"{(a ?? "null")}\" or \"{(b ?? "null")}\"");
            ans["tableSelect"] = -1;
            ans["i1"] = null;
            ans["i2"] = null;
            return ans;
        }
        bool tmpa = (s2t[a] is AbstractPhysicalObject.AbstractObjectType || s2t[a] is MoreSlugcatsEnums.AbstractObjectType);
        bool tmpb = (s2t[b] is AbstractPhysicalObject.AbstractObjectType || s2t[b] is MoreSlugcatsEnums.AbstractObjectType);
        ans["tableSelect"] = (tmpa ? 0 : 1) + (tmpb ? 0 : 1);
        ans["i1"] = (tmpa && !tmpb) ? s2t[b] : s2t[a];
        ans["i2"] = (tmpa && !tmpb) ? s2t[a] : s2t[b];
        Console.WriteLine($"ans : tableSelect = {ans["tableSelect"]}  i1 = {ans["i1"] ?? null}  i2 = {ans["i2"] ?? null}");
        return ans;
    }


    public static void InitCraftingLibrary()        //打表x2
    {
        int tableSelect = 0;
        AbstractPhysicalObject.AbstractObjectType key = AbstractPhysicalObject.AbstractObjectType.Rock;
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Rock], tableSelect, null, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlareBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.Lantern, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.VultureMask], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.PuffBall], tableSelect, AbstractPhysicalObject.AbstractObjectType.SporePlant, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DangleFruit], tableSelect, AbstractPhysicalObject.AbstractObjectType.FlareBomb, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SSOracleSwarmer], tableSelect, AbstractPhysicalObject.AbstractObjectType.OverseerCarcass, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DataPearl], tableSelect, null, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.WaterNut], tableSelect, AbstractPhysicalObject.AbstractObjectType.JellyFish, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.JellyFish], tableSelect, MoreSlugcatsEnums.AbstractObjectType.LillyPuck, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Lantern], tableSelect, AbstractPhysicalObject.AbstractObjectType.FlareBomb, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.KarmaFlower], tableSelect, AbstractPhysicalObject.AbstractObjectType.ScavengerBomb, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Mushroom], tableSelect, AbstractPhysicalObject.AbstractObjectType.PuffBall, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.ScavengerBomb, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SlimeMold], tableSelect, AbstractPhysicalObject.AbstractObjectType.Lantern, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlyLure], tableSelect, AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.PuffBall, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.NeedleEgg, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, null, CreatureTemplate.Type.Fly);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.ScavengerBomb, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, null, CreatureTemplate.Type.Fly);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.SporePlant, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.Lantern, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, AbstractPhysicalObject.AbstractObjectType.Lantern, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.PuffBall, null);
        key = AbstractPhysicalObject.AbstractObjectType.FlareBomb;
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlareBomb], tableSelect, null, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.VultureMask], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.PuffBall], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DangleFruit], tableSelect, AbstractPhysicalObject.AbstractObjectType.SlimeMold, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SSOracleSwarmer], tableSelect, AbstractPhysicalObject.AbstractObjectType.OverseerCarcass, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DataPearl], tableSelect, null, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.WaterNut], tableSelect, AbstractPhysicalObject.AbstractObjectType.JellyFish, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.JellyFish], tableSelect, MoreSlugcatsEnums.AbstractObjectType.LillyPuck, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Lantern], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GlowWeed, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.KarmaFlower], tableSelect, AbstractPhysicalObject.AbstractObjectType.Lantern, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Mushroom], tableSelect, AbstractPhysicalObject.AbstractObjectType.PuffBall, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.Lantern, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SlimeMold], tableSelect, AbstractPhysicalObject.AbstractObjectType.Lantern, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlyLure], tableSelect, AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.Lantern, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.SporePlant, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, null, CreatureTemplate.Type.VultureGrub);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.ScavengerBomb, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, null, CreatureTemplate.Type.VultureGrub);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.Lantern, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.Lantern, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, AbstractPhysicalObject.AbstractObjectType.Lantern, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.SporePlant, null);
        key = AbstractPhysicalObject.AbstractObjectType.VultureMask;
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.VultureMask], tableSelect, null, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.PuffBall], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DangleFruit], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SSOracleSwarmer], tableSelect, AbstractPhysicalObject.AbstractObjectType.OverseerCarcass, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DataPearl], tableSelect, null, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.WaterNut], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.JellyFish], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Lantern], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.KarmaFlower], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Mushroom], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SlimeMold], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlyLure], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, null, CreatureTemplate.Type.VultureGrub);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.ScavengerBomb, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, null, CreatureTemplate.Type.VultureGrub);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        key = AbstractPhysicalObject.AbstractObjectType.PuffBall;
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.PuffBall], tableSelect, null, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DangleFruit], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SSOracleSwarmer], tableSelect, AbstractPhysicalObject.AbstractObjectType.OverseerCarcass, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DataPearl], tableSelect, null, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.WaterNut], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GooieDuck, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.JellyFish], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GooieDuck, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Lantern], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.KarmaFlower], tableSelect, AbstractPhysicalObject.AbstractObjectType.NeedleEgg, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Mushroom], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GooieDuck, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.ScavengerBomb, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SlimeMold], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlyLure], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GooieDuck, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GooieDuck, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GooieDuck, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, null, CreatureTemplate.Type.SmallCentipede);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.ScavengerBomb, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, null, CreatureTemplate.Type.SmallCentipede);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GooieDuck, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GooieDuck, null);
        key = AbstractPhysicalObject.AbstractObjectType.DangleFruit;
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DangleFruit], tableSelect, null, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SSOracleSwarmer], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DataPearl], tableSelect, null, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.WaterNut], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.JellyFish], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Lantern], tableSelect, AbstractPhysicalObject.AbstractObjectType.FlareBomb, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.KarmaFlower], tableSelect, MoreSlugcatsEnums.AbstractObjectType.Seed, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Mushroom], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GooieDuck, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.SporePlant, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SlimeMold], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlyLure], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GooieDuck, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, null, CreatureTemplate.Type.Fly);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, MoreSlugcatsEnums.AbstractObjectType.LillyPuck, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        key = AbstractPhysicalObject.AbstractObjectType.SSOracleSwarmer;
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SSOracleSwarmer], tableSelect, null, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DataPearl], tableSelect, AbstractPhysicalObject.AbstractObjectType.OverseerCarcass, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.WaterNut], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.JellyFish], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Lantern], tableSelect, AbstractPhysicalObject.AbstractObjectType.OverseerCarcass, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.KarmaFlower], tableSelect, AbstractPhysicalObject.AbstractObjectType.OverseerCarcass, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Mushroom], tableSelect, AbstractPhysicalObject.AbstractObjectType.OverseerCarcass, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.OverseerCarcass, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SlimeMold], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlyLure], tableSelect, AbstractPhysicalObject.AbstractObjectType.OverseerCarcass, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.OverseerCarcass, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.OverseerCarcass, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.OverseerCarcass, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, AbstractPhysicalObject.AbstractObjectType.OverseerCarcass, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.OverseerCarcass, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        key = AbstractPhysicalObject.AbstractObjectType.DataPearl;
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DataPearl], tableSelect, null, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.WaterNut], tableSelect, null, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.JellyFish], tableSelect, null, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Lantern], tableSelect, null, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.KarmaFlower], tableSelect, null, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Mushroom], tableSelect, null, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant], tableSelect, null, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SlimeMold], tableSelect, null, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlyLure], tableSelect, null, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb], tableSelect, null, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, null, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, null, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, null, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, null, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, null, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, null, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, null, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, null, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, null, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, null, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, null, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, null, null);
        key = AbstractPhysicalObject.AbstractObjectType.WaterNut;
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.WaterNut], tableSelect, null, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.JellyFish], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Lantern], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GlowWeed, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.KarmaFlower], tableSelect, MoreSlugcatsEnums.AbstractObjectType.Seed, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Mushroom], tableSelect, AbstractPhysicalObject.AbstractObjectType.NeedleEgg, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.BubbleGrass, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SlimeMold], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlyLure], tableSelect, AbstractPhysicalObject.AbstractObjectType.BubbleGrass, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb], tableSelect, null, CreatureTemplate.Type.Snail);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, null, CreatureTemplate.Type.Hazer);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, null, CreatureTemplate.Type.Hazer);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, MoreSlugcatsEnums.AbstractObjectType.LillyPuck, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        key = AbstractPhysicalObject.AbstractObjectType.JellyFish;
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.JellyFish], tableSelect, null, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Lantern], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GlowWeed, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.KarmaFlower], tableSelect, AbstractPhysicalObject.AbstractObjectType.NeedleEgg, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Mushroom], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GooieDuck, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.FlyLure, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SlimeMold], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlyLure], tableSelect, AbstractPhysicalObject.AbstractObjectType.BubbleGrass, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb], tableSelect, null, CreatureTemplate.Type.Snail);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, null, CreatureTemplate.Type.Hazer);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, null, CreatureTemplate.Type.Snail);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, MoreSlugcatsEnums.AbstractObjectType.LillyPuck, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        key = AbstractPhysicalObject.AbstractObjectType.Lantern;
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Lantern], tableSelect, null, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.KarmaFlower], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Mushroom], tableSelect, AbstractPhysicalObject.AbstractObjectType.PuffBall, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.ScavengerBomb, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SlimeMold], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlyLure], tableSelect, AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.NeedleEgg, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, null, CreatureTemplate.Type.VultureGrub);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GlowWeed, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.ScavengerBomb, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, null, CreatureTemplate.Type.VultureGrub);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, MoreSlugcatsEnums.AbstractObjectType.LillyPuck, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GlowWeed, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, MoreSlugcatsEnums.AbstractObjectType.LillyPuck, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GlowWeed, null);
        key = AbstractPhysicalObject.AbstractObjectType.KarmaFlower;
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.KarmaFlower], tableSelect, null, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Mushroom], tableSelect, AbstractPhysicalObject.AbstractObjectType.NeedleEgg, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.ScavengerBomb, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SlimeMold], tableSelect, MoreSlugcatsEnums.AbstractObjectType.Seed, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlyLure], tableSelect, MoreSlugcatsEnums.AbstractObjectType.Seed, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.ScavengerBomb, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.NeedleEgg, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.OverseerCarcass, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, MoreSlugcatsEnums.AbstractObjectType.Seed, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.SSOracleSwarmer, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, MoreSlugcatsEnums.AbstractObjectType.SingularityBomb, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, AbstractPhysicalObject.AbstractObjectType.OverseerCarcass, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, MoreSlugcatsEnums.AbstractObjectType.Seed, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, MoreSlugcatsEnums.AbstractObjectType.Seed, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, MoreSlugcatsEnums.AbstractObjectType.Seed, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, MoreSlugcatsEnums.AbstractObjectType.Seed, null);
        key = AbstractPhysicalObject.AbstractObjectType.Mushroom;
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Mushroom], tableSelect, null, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant], tableSelect, null, CreatureTemplate.Type.SmallCentipede);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SlimeMold], tableSelect, null, CreatureTemplate.Type.SmallNeedleWorm);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlyLure], tableSelect, null, CreatureTemplate.Type.Fly);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.SporePlant, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GooieDuck, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GooieDuck, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, null, CreatureTemplate.Type.SmallCentipede);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, null, CreatureTemplate.Type.Hazer);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, null, CreatureTemplate.Type.SmallCentipede);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.ScavengerBomb, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, null, CreatureTemplate.Type.SmallCentipede);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.PuffBall, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GooieDuck, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GooieDuck, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GooieDuck, null);
        key = AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant;
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant], tableSelect, null, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SlimeMold], tableSelect, AbstractPhysicalObject.AbstractObjectType.Lantern, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlyLure], tableSelect, AbstractPhysicalObject.AbstractObjectType.SporePlant, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.Lantern, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.ScavengerBomb, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.SporePlant, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, null, CreatureTemplate.Type.SmallCentipede);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, null, CreatureTemplate.Type.Snail);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.ScavengerBomb, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, null, CreatureTemplate.Type.SmallCentipede);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.SporePlant, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.SporePlant, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, AbstractPhysicalObject.AbstractObjectType.SporePlant, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.SporePlant, null);
        key = AbstractPhysicalObject.AbstractObjectType.SlimeMold;
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SlimeMold], tableSelect, null, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlyLure], tableSelect, AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.Lantern, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, null, CreatureTemplate.Type.SmallCentipede);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        key = AbstractPhysicalObject.AbstractObjectType.FlyLure;
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlyLure], tableSelect, null, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.PuffBall, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GooieDuck, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, null, CreatureTemplate.Type.Fly);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.ScavengerBomb, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, null, CreatureTemplate.Type.Fly);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, null, CreatureTemplate.Type.Hazer);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, null, CreatureTemplate.Type.SmallCentipede);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, null, CreatureTemplate.Type.Fly);
        key = AbstractPhysicalObject.AbstractObjectType.ScavengerBomb;
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb], tableSelect, null, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.PuffBall, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, null, CreatureTemplate.Type.SmallCentipede);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, AbstractPhysicalObject.AbstractObjectType.PuffBall, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, MoreSlugcatsEnums.AbstractObjectType.SingularityBomb, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, null, CreatureTemplate.Type.SmallCentipede);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.PuffBall, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.SporePlant, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, AbstractPhysicalObject.AbstractObjectType.Lantern, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.PuffBall, null);
        key = AbstractPhysicalObject.AbstractObjectType.SporePlant;
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, null, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.PuffBall, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, null, CreatureTemplate.Type.TubeWorm);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, AbstractPhysicalObject.AbstractObjectType.PuffBall, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.ScavengerBomb, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, null, CreatureTemplate.Type.TubeWorm);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.PuffBall, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.JellyFish, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, AbstractPhysicalObject.AbstractObjectType.JellyFish, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        key = AbstractPhysicalObject.AbstractObjectType.EggBugEgg;
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, null, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, null, CreatureTemplate.Type.SmallNeedleWorm);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        key = AbstractPhysicalObject.AbstractObjectType.NeedleEgg;
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, null, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, null, CreatureTemplate.Type.Hazer);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, null, CreatureTemplate.Type.VultureGrub);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, null, CreatureTemplate.Type.VultureGrub);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, null, CreatureTemplate.Type.SmallNeedleWorm);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, null, CreatureTemplate.Type.TubeWorm);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, null, CreatureTemplate.Type.Hazer);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, null, CreatureTemplate.Type.Hazer);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, null, CreatureTemplate.Type.SmallNeedleWorm);
        key = AbstractPhysicalObject.AbstractObjectType.BubbleGrass;
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, null, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.ScavengerBomb, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, null, CreatureTemplate.Type.Hazer);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.PuffBall, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.JellyFish, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, MoreSlugcatsEnums.AbstractObjectType.LillyPuck, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GlowWeed, null);
        key = AbstractPhysicalObject.AbstractObjectType.OverseerCarcass;
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, null, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        key = MoreSlugcatsEnums.AbstractObjectType.SingularityBomb;
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, null, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.KarmaFlower, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        key = MoreSlugcatsEnums.AbstractObjectType.FireEgg;
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, null, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        key = MoreSlugcatsEnums.AbstractObjectType.Seed;
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, null, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        key = MoreSlugcatsEnums.AbstractObjectType.GooieDuck;
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, null, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        key = MoreSlugcatsEnums.AbstractObjectType.LillyPuck;
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, null, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        key = MoreSlugcatsEnums.AbstractObjectType.GlowWeed;
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, null, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        key = MoreSlugcatsEnums.AbstractObjectType.DandelionPeach;
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.objectsLibrary[key], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, null, null);
        tableSelect = 1;
        CreatureTemplate.Type key2 = CreatureTemplate.Type.Fly;
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Rock], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlareBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.VultureMask], tableSelect, MoreSlugcatsEnums.AbstractObjectType.Seed, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.PuffBall], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DangleFruit], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SSOracleSwarmer], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DataPearl], tableSelect, null, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.WaterNut], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.JellyFish], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Lantern], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.KarmaFlower], tableSelect, AbstractPhysicalObject.AbstractObjectType.FlyLure, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Mushroom], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GooieDuck, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.SporePlant, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SlimeMold], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlyLure], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.FlyLure, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, MoreSlugcatsEnums.AbstractObjectType.Seed, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, AbstractPhysicalObject.AbstractObjectType.FlyLure, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        key2 = CreatureTemplate.Type.SmallCentipede;
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Rock], tableSelect, AbstractPhysicalObject.AbstractObjectType.SporePlant, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlareBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.SporePlant, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.VultureMask], tableSelect, MoreSlugcatsEnums.AbstractObjectType.Seed, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.PuffBall], tableSelect, AbstractPhysicalObject.AbstractObjectType.SporePlant, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DangleFruit], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SSOracleSwarmer], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DataPearl], tableSelect, null, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.WaterNut], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.JellyFish], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Lantern], tableSelect, AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.KarmaFlower], tableSelect, AbstractPhysicalObject.AbstractObjectType.PuffBall, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Mushroom], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GooieDuck, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.SporePlant, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SlimeMold], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlyLure], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, null, CreatureTemplate.Type.TubeWorm);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, MoreSlugcatsEnums.AbstractObjectType.Seed, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, AbstractPhysicalObject.AbstractObjectType.JellyFish, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        key2 = CreatureTemplate.Type.VultureGrub;
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Rock], tableSelect, AbstractPhysicalObject.AbstractObjectType.Lantern, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlareBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.Lantern, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.VultureMask], tableSelect, MoreSlugcatsEnums.AbstractObjectType.Seed, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.PuffBall], tableSelect, AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DangleFruit], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SSOracleSwarmer], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DataPearl], tableSelect, null, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.WaterNut], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.JellyFish], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Lantern], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.KarmaFlower], tableSelect, AbstractPhysicalObject.AbstractObjectType.VultureMask, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Mushroom], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GooieDuck, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.SporePlant, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SlimeMold], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlyLure], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.FlareBomb, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, MoreSlugcatsEnums.AbstractObjectType.Seed, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GlowWeed, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        key2 = CreatureTemplate.Type.SmallNeedleWorm;
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Rock], tableSelect, AbstractPhysicalObject.AbstractObjectType.PuffBall, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlareBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.PuffBall, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.VultureMask], tableSelect, MoreSlugcatsEnums.AbstractObjectType.Seed, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.PuffBall], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DangleFruit], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SSOracleSwarmer], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DataPearl], tableSelect, null, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.WaterNut], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.JellyFish], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Lantern], tableSelect, AbstractPhysicalObject.AbstractObjectType.NeedleEgg, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.KarmaFlower], tableSelect, AbstractPhysicalObject.AbstractObjectType.NeedleEgg, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Mushroom], tableSelect, AbstractPhysicalObject.AbstractObjectType.PuffBall, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.SporePlant, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SlimeMold], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlyLure], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.PuffBall, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.PuffBall, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, MoreSlugcatsEnums.AbstractObjectType.Seed, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        key2 = CreatureTemplate.Type.Hazer;
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Rock], tableSelect, MoreSlugcatsEnums.AbstractObjectType.LillyPuck, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlareBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GlowWeed, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.VultureMask], tableSelect, MoreSlugcatsEnums.AbstractObjectType.Seed, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.PuffBall], tableSelect, AbstractPhysicalObject.AbstractObjectType.BubbleGrass, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DangleFruit], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SSOracleSwarmer], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DataPearl], tableSelect, null, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.WaterNut], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.JellyFish], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Lantern], tableSelect, null, CreatureTemplate.Type.Snail);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.KarmaFlower], tableSelect, AbstractPhysicalObject.AbstractObjectType.BubbleGrass, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Mushroom], tableSelect, AbstractPhysicalObject.AbstractObjectType.BubbleGrass, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.JellyFish, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SlimeMold], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlyLure], tableSelect, AbstractPhysicalObject.AbstractObjectType.BubbleGrass, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.FlareBomb, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.JellyFish, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, MoreSlugcatsEnums.AbstractObjectType.Seed, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GlowWeed, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        /*Custom.Log(new string[]
        {
                "--Setup Creature + Creature table"
        });*/
        tableSelect = 2;
        key2 = CreatureTemplate.Type.Fly;
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.critsLibrary[CreatureTemplate.Type.Fly], tableSelect, null, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.critsLibrary[CreatureTemplate.Type.VultureGrub], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.critsLibrary[CreatureTemplate.Type.SmallCentipede], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.critsLibrary[CreatureTemplate.Type.SmallNeedleWorm], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.critsLibrary[CreatureTemplate.Type.Hazer], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        key2 = CreatureTemplate.Type.VultureGrub;
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.critsLibrary[CreatureTemplate.Type.VultureGrub], tableSelect, null, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.critsLibrary[CreatureTemplate.Type.SmallCentipede], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.critsLibrary[CreatureTemplate.Type.SmallNeedleWorm], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.critsLibrary[CreatureTemplate.Type.Hazer], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        key2 = CreatureTemplate.Type.SmallCentipede;
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.critsLibrary[CreatureTemplate.Type.SmallCentipede], tableSelect, null, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.critsLibrary[CreatureTemplate.Type.SmallNeedleWorm], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.critsLibrary[CreatureTemplate.Type.Hazer], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        key2 = CreatureTemplate.Type.SmallNeedleWorm;
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.critsLibrary[CreatureTemplate.Type.SmallNeedleWorm], tableSelect, null, null);
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.critsLibrary[CreatureTemplate.Type.Hazer], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        key2 = CreatureTemplate.Type.Hazer;
        Temp_ChangeCrafting.SetLibraryData(Temp_ChangeCrafting.critsLibrary[key2], Temp_ChangeCrafting.critsLibrary[CreatureTemplate.Type.Hazer], tableSelect, null, null);
    }

    public static AbstractPhysicalObject CraftingResults(PhysicalObject crafter, Creature.Grasp graspA, Creature.Grasp graspB)
    {
        AbstractPhysicalObject.AbstractObjectType abstractObjectType = Temp_ChangeCrafting.CraftingResults_ObjectData(graspA, graspB, true);
        /*Custom.Log(new string[]
        {
                "CRAFTING INPUT",
                graspA.grabbed.abstractPhysicalObject.type.ToString(),
                "+",
                graspB.grabbed.abstractPhysicalObject.type.ToString()
        });*/
        if (abstractObjectType == null)
        {
            return new AbstractPhysicalObject(crafter.room.world, AbstractPhysicalObject.AbstractObjectType.Rock, null, crafter.abstractPhysicalObject.pos, crafter.room.game.GetNewID());
        }
        /*Custom.Log(new string[]
        {
                string.Format("CRAFTING RESULTS {0}", abstractObjectType)
        });*/
        if (abstractObjectType == AbstractPhysicalObject.AbstractObjectType.Creature)
        {
            CreatureTemplate.Type type = Temp_ChangeCrafting.CraftingResults_CreatureData(graspA, graspB);
            return new AbstractCreature(crafter.room.world, StaticWorld.GetCreatureTemplate(type), null, crafter.abstractPhysicalObject.pos, crafter.room.game.GetNewID());
        }
        if (abstractObjectType == AbstractPhysicalObject.AbstractObjectType.DataPearl) //珍珠
        {
            return new DataPearl.AbstractDataPearl(crafter.room.world, AbstractPhysicalObject.AbstractObjectType.DataPearl, null, crafter.abstractPhysicalObject.pos, crafter.room.game.GetNewID(), -1, -1, null, DataPearl.AbstractDataPearl.DataPearlType.Misc);
        }
        if (abstractObjectType == AbstractPhysicalObject.AbstractObjectType.OverseerCarcass) //监察者眼球
        {
            return new OverseerCarcass.AbstractOverseerCarcass(crafter.room.world, null, crafter.abstractPhysicalObject.pos, crafter.room.game.GetNewID(), Color.white, 0);
        }
        if (abstractObjectType == AbstractPhysicalObject.AbstractObjectType.SporePlant) //蜂巢
        {
            return new SporePlant.AbstractSporePlant(crafter.room.world, null, crafter.abstractPhysicalObject.pos, crafter.room.game.GetNewID(), -1, -1, null, false, true)
            {
                isFresh = false,
                isConsumed = true
            };
        }
        if (abstractObjectType == AbstractPhysicalObject.AbstractObjectType.VultureMask) //秃鹫面具
        {
            return new VultureMask.AbstractVultureMask(crafter.room.world, null, crafter.abstractPhysicalObject.pos, crafter.room.game.GetNewID(), UnityEngine.Random.Range(0, 4000), false);
        }
        if (abstractObjectType == AbstractPhysicalObject.AbstractObjectType.WaterNut) //泡水果
        {
            return new WaterNut.AbstractWaterNut(crafter.room.world, null, crafter.abstractPhysicalObject.pos, crafter.room.game.GetNewID(), -1, -1, null, true)
            {
                isFresh = false,
                isConsumed = true
            };
        }
        if (abstractObjectType == AbstractPhysicalObject.AbstractObjectType.BubbleGrass) //气泡草
        {
            return new BubbleGrass.AbstractBubbleGrass(crafter.room.world, null, crafter.abstractPhysicalObject.pos, crafter.room.game.GetNewID(), 1f, -1, -1, null)
            {
                isFresh = false,
                isConsumed = true
            };
        }
        if (abstractObjectType == MoreSlugcatsEnums.AbstractObjectType.LillyPuck) //百合花灯
        {
            return new LillyPuck.AbstractLillyPuck(crafter.room.world, null, crafter.abstractPhysicalObject.pos, crafter.room.game.GetNewID(), 3, -1, -1, null)
            {
                isFresh = false,
                isConsumed = true
            };
        }
        if (abstractObjectType == MoreSlugcatsEnums.AbstractObjectType.FireEgg) //火虫卵
        {
            return new FireEgg.AbstractBugEgg(crafter.room.world, null, crafter.abstractPhysicalObject.pos, crafter.room.game.GetNewID(), UnityEngine.Random.value);
        }
        if (AbstractConsumable.IsTypeConsumable(abstractObjectType)) //消耗品
        {
            return new AbstractConsumable(crafter.room.world, abstractObjectType, null, crafter.abstractPhysicalObject.pos, crafter.room.game.GetNewID(), -1, -1, null)
            {
                isFresh = false,
                isConsumed = true
            };
        }
        return new AbstractPhysicalObject(crafter.room.world, abstractObjectType, null, crafter.abstractPhysicalObject.pos, crafter.room.game.GetNewID());
    }

    public static CreatureTemplate.Type CraftingResults_CreatureData(Creature.Grasp graspA, Creature.Grasp graspB)
    {
        if (graspA.grabbed is Creature && graspB.grabbed is Creature)
        {
            return Temp_ChangeCrafting.GetLibraryData((graspA.grabbed as Creature).abstractCreature.creatureTemplate.type, (graspB.grabbed as Creature).abstractCreature.creatureTemplate.type).crit;
        }
        if (graspA.grabbed is Creature)
        {
            return Temp_ChangeCrafting.GetLibraryData((graspA.grabbed as Creature).abstractCreature.creatureTemplate.type, graspB.grabbed.abstractPhysicalObject.type).crit;
        }
        if (graspB.grabbed is Creature)
        {
            return Temp_ChangeCrafting.GetLibraryData((graspB.grabbed as Creature).abstractCreature.creatureTemplate.type, graspA.grabbed.abstractPhysicalObject.type).crit;
        }
        return Temp_ChangeCrafting.GetLibraryData(graspA.grabbed.abstractPhysicalObject.type, graspB.grabbed.abstractPhysicalObject.type).crit;
    }

    public static Temp_ChangeCrafting.CraftDat GetLibraryData(AbstractPhysicalObject.AbstractObjectType objectA, AbstractPhysicalObject.AbstractObjectType objectB)
    {
        if (Temp_ChangeCrafting.objectsLibrary.ContainsKey(objectA) && Temp_ChangeCrafting.objectsLibrary.ContainsKey(objectB))
        {
            int num = Temp_ChangeCrafting.objectsLibrary[objectA];
            int num2 = Temp_ChangeCrafting.objectsLibrary[objectB];
            return Temp_ChangeCrafting.craftingGrid_ObjectsOnly[num, num2];
        }
        return new Temp_ChangeCrafting.CraftDat(null, null);
    }

    public static Temp_ChangeCrafting.CraftDat GetLibraryData(CreatureTemplate.Type critterA, AbstractPhysicalObject.AbstractObjectType objectB)
    {
        if (Temp_ChangeCrafting.critsLibrary.ContainsKey(critterA) && Temp_ChangeCrafting.objectsLibrary.ContainsKey(objectB))
        {
            int num = Temp_ChangeCrafting.critsLibrary[critterA];
            int num2 = Temp_ChangeCrafting.objectsLibrary[objectB];
            return Temp_ChangeCrafting.craftingGrid_CritterObjects[num, num2];
        }
        return new Temp_ChangeCrafting.CraftDat(null, null);
    }

    public static Temp_ChangeCrafting.CraftDat GetLibraryData(CreatureTemplate.Type critterA, CreatureTemplate.Type critterB)
    {
        if (Temp_ChangeCrafting.critsLibrary.ContainsKey(critterA) && Temp_ChangeCrafting.critsLibrary.ContainsKey(critterB))
        {
            int num = Temp_ChangeCrafting.critsLibrary[critterA];
            int num2 = Temp_ChangeCrafting.critsLibrary[critterB];
            return Temp_ChangeCrafting.craftingGrid_CrittersOnly[num, num2];
        }
        return new Temp_ChangeCrafting.CraftDat(null, null);
    }

    public static AbstractPhysicalObject.AbstractObjectType CraftingResults_ObjectData(Creature.Grasp graspA, Creature.Grasp graspB, bool canMakeMeals)
    {
        if (graspA == null || graspB == null)
        {
            return null;
        }
        if (graspA.grabbed is IPlayerEdible && !(graspA.grabbed as IPlayerEdible).Edible)
        {
            return null;
        }
        if (graspB.grabbed is IPlayerEdible && !(graspB.grabbed as IPlayerEdible).Edible)
        {
            return null;
        }
        AbstractPhysicalObject.AbstractObjectType result = null;
        Temp_ChangeCrafting.CraftDat filteredLibraryData = Temp_ChangeCrafting.GetFilteredLibraryData(graspA, graspB);
        if (filteredLibraryData.enabled)
        {
            result = filteredLibraryData.type;
        }
        return result;
    }


    public static AbstractPhysicalObject RandomStomachItem(PhysicalObject caller)
    {
        float value = UnityEngine.Random.value;
        AbstractPhysicalObject abstractPhysicalObject;
        if (value <= 0.32894737f)
        {
            abstractPhysicalObject = new AbstractConsumable(caller.room.world, AbstractPhysicalObject.AbstractObjectType.FlyLure, null, caller.room.GetWorldCoordinate(caller.firstChunk.pos), caller.room.game.GetNewID(), -1, -1, null);
        }
        else if (value <= 0.4276316f)
        {
            abstractPhysicalObject = new AbstractConsumable(caller.room.world, AbstractPhysicalObject.AbstractObjectType.Mushroom, null, caller.room.GetWorldCoordinate(caller.firstChunk.pos), caller.room.game.GetNewID(), -1, -1, null);
        }
        else if (value <= 0.5065789f)
        {
            abstractPhysicalObject = new AbstractConsumable(caller.room.world, AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant, null, caller.room.GetWorldCoordinate(caller.firstChunk.pos), caller.room.game.GetNewID(), -1, -1, null);
        }
        else if (value <= 0.6118421f)
        {
            abstractPhysicalObject = new WaterNut.AbstractWaterNut(caller.room.world, null, caller.room.GetWorldCoordinate(caller.firstChunk.pos), caller.room.game.GetNewID(), -1, -1, null, false);
        }
        else if (value <= 0.6644737f)
        {
            abstractPhysicalObject = new AbstractCreature(caller.room.world, StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.VultureGrub), null, caller.room.GetWorldCoordinate(caller.firstChunk.pos), caller.room.game.GetNewID());
        }
        else if (value <= 0.7302632f)
        {
            abstractPhysicalObject = new AbstractCreature(caller.room.world, StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.Snail), null, caller.room.GetWorldCoordinate(caller.firstChunk.pos), caller.room.game.GetNewID());
        }
        else if (value <= 0.79605263f)
        {
            abstractPhysicalObject = new AbstractCreature(caller.room.world, StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.Hazer), null, caller.room.GetWorldCoordinate(caller.firstChunk.pos), caller.room.game.GetNewID());
        }
        else if (value <= 0.82894737f)
        {
            abstractPhysicalObject = new AbstractConsumable(caller.room.world, AbstractPhysicalObject.AbstractObjectType.PuffBall, null, caller.room.GetWorldCoordinate(caller.firstChunk.pos), caller.room.game.GetNewID(), -1, -1, null);
        }
        else if (value <= 0.8486842f)
        {
            abstractPhysicalObject = new AbstractPhysicalObject(caller.room.world, AbstractPhysicalObject.AbstractObjectType.ScavengerBomb, null, caller.room.GetWorldCoordinate(caller.firstChunk.pos), caller.room.game.GetNewID());
        }
        else if (value <= 0.9144737f)
        {
            abstractPhysicalObject = new BubbleGrass.AbstractBubbleGrass(caller.room.world, null, caller.room.GetWorldCoordinate(caller.firstChunk.pos), caller.room.game.GetNewID(), 1f, -1, -1, null);
        }
        else if (value <= 0.93421054f)
        {
            abstractPhysicalObject = new SporePlant.AbstractSporePlant(caller.room.world, null, caller.room.GetWorldCoordinate(caller.firstChunk.pos), caller.room.game.GetNewID(), -1, -1, null, false, (double)UnityEngine.Random.value < 0.5);
        }
        else if (value <= 0.46710527f)
        {
            Color color;
            color = new Color(1f, 0.8f, 0.3f);
            int ownerIterator = 1;
            if (UnityEngine.Random.value <= 0.35f)
            {
                color = new Color(0.44705883f, 0.9019608f, 0.76862746f);
                ownerIterator = 0;
            }
            else if (UnityEngine.Random.value <= 0.05f)
            {
                color = new Color(0f, 1f, 0f);
                ownerIterator = 2;
            }
            abstractPhysicalObject = new OverseerCarcass.AbstractOverseerCarcass(caller.room.world, null, caller.abstractPhysicalObject.pos, caller.room.game.GetNewID(), color, ownerIterator);
        }
        else if (value <= 0.4736842f)
        {
            abstractPhysicalObject = new AbstractConsumable(caller.room.world, AbstractPhysicalObject.AbstractObjectType.KarmaFlower, null, caller.room.GetWorldCoordinate(caller.firstChunk.pos), caller.room.game.GetNewID(), -1, -1, null);
        }
        else if (value <= 0.9934211f)
        {
            abstractPhysicalObject = new AbstractPhysicalObject(caller.room.world, AbstractPhysicalObject.AbstractObjectType.Lantern, null, caller.room.GetWorldCoordinate(caller.firstChunk.pos), caller.room.game.GetNewID());
        }
        else if (value <= 0.79605263f)
        {
            abstractPhysicalObject = new AbstractCreature(caller.room.world, StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.TubeWorm), null, caller.room.GetWorldCoordinate(caller.firstChunk.pos), caller.room.game.GetNewID());
        }
        else if (value <= 0.8f)
        {
            abstractPhysicalObject = new VultureMask.AbstractVultureMask(caller.room.world, null, caller.room.GetWorldCoordinate(caller.firstChunk.pos), caller.room.game.GetNewID(), caller.abstractPhysicalObject.ID.RandomSeed, (double)UnityEngine.Random.value <= 0.05);
        }
        else
        {
            abstractPhysicalObject = new DataPearl.AbstractDataPearl(caller.room.world, AbstractPhysicalObject.AbstractObjectType.DataPearl, null, caller.room.GetWorldCoordinate(caller.firstChunk.pos), caller.room.game.GetNewID(), -1, -1, null, DataPearl.AbstractDataPearl.DataPearlType.Misc);
        }
        if (AbstractConsumable.IsTypeConsumable(abstractPhysicalObject.type))
        {
            (abstractPhysicalObject as AbstractConsumable).isFresh = false;
            (abstractPhysicalObject as AbstractConsumable).isConsumed = true;
        }
        return abstractPhysicalObject;
    }
}
