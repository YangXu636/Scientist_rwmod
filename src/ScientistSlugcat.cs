using System.Collections;
using System;
using System.Collections.Generic;
using MoreSlugcats;
using RWCustom;
using UnityEngine;
using System.Reflection;

namespace Scientist;

public class ScientistSlugcat
{
    public static Dictionary<AbstractPhysicalObject.AbstractObjectType, int> objectsLibrary;
    public static Dictionary<CreatureTemplate.Type, int> critsLibrary;
    public static ScientistSlugcat.CraftDat[,] craftingGrid_ObjectsOnly;
    public static ScientistSlugcat.CraftDat[,] craftingGrid_CritterObjects;
    public static ScientistSlugcat.CraftDat[,] craftingGrid_CrittersOnly;
    public static bool showDebug;

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

    static ScientistSlugcat() {     //打表x1
        int num = 0;
        ScientistSlugcat.objectsLibrary = new Dictionary<AbstractPhysicalObject.AbstractObjectType, int>();
        ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Rock] = num;
        num++;
        ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlareBomb] = num;
        num++;
        ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.VultureMask] = num;
        num++;
        ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.PuffBall] = num;
        num++;
        ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DangleFruit] = num;
        num++;
        ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SLOracleSwarmer] = num;
        num++;
        ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SSOracleSwarmer] = num;
        num++;
        ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DataPearl] = num;
        num++;
        ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.WaterNut] = num;
        num++;
        ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.JellyFish] = num;
        num++;
        ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Lantern] = num;
        num++;
        ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.KarmaFlower] = num;
        num++;
        ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Mushroom] = num;
        num++;
        ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant] = num;
        num++;
        ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SlimeMold] = num;
        num++;
        ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlyLure] = num;
        num++;
        ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb] = num; //手雷（炸弹）
        num++;
        ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant] = num;
        num++;
        ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg] = num;
        num++;
        ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg] = num;
        num++;
        ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass] = num;
        num++;
        ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass] = num;
        num++;
        ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb] = num;
        num++;
        ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg] = num;
        num++;
        ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed] = num;
        num++;
        ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck] = num;
        num++;
        ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck] = num;
        num++;
        ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed] = num;
        num++;
        ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach] = num;
        num++;
        //新增物品
        ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Spear] = num;
        num++;
        ScientistSlugcat.objectsLibrary[Scientist.Register.SharpSpear] = num;
        num++;

        int num2 = 0;
        ScientistSlugcat.critsLibrary = new Dictionary<CreatureTemplate.Type, int>();
        ScientistSlugcat.critsLibrary[CreatureTemplate.Type.VultureGrub] = num2;
        num2++;
        ScientistSlugcat.critsLibrary[CreatureTemplate.Type.SmallCentipede] = num2;
        num2++;
        ScientistSlugcat.critsLibrary[CreatureTemplate.Type.SmallNeedleWorm] = num2;
        num2++;
        ScientistSlugcat.critsLibrary[CreatureTemplate.Type.Hazer] = num2;
        num2++;
        ScientistSlugcat.critsLibrary[CreatureTemplate.Type.Fly] = num2;
        num2++;
        //新增生物

        ScientistSlugcat.craftingGrid_ObjectsOnly = new ScientistSlugcat.CraftDat[num, num];
        ScientistSlugcat.craftingGrid_CritterObjects = new ScientistSlugcat.CraftDat[num2, num];
        ScientistSlugcat.craftingGrid_CrittersOnly = new ScientistSlugcat.CraftDat[num2, num2];
        ScientistSlugcat.InitCraftingLibrary();
    }

    public static void SetLibraryData(AbstractPhysicalObject.AbstractObjectType objectA, AbstractPhysicalObject.AbstractObjectType objectB, AbstractPhysicalObject.AbstractObjectType resultType, CreatureTemplate.Type resultCritter)
    {
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[objectA], ScientistSlugcat.objectsLibrary[objectB], 0, resultType, resultCritter);
    }

    public static void SetLibraryData(CreatureTemplate.Type critterA, AbstractPhysicalObject.AbstractObjectType objectB, AbstractPhysicalObject.AbstractObjectType resultType, CreatureTemplate.Type resultCritter)
    {
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[critterA], ScientistSlugcat.objectsLibrary[objectB], 1, resultType, resultCritter);
    }

    public static void SetLibraryData(CreatureTemplate.Type critterA, CreatureTemplate.Type critterB, AbstractPhysicalObject.AbstractObjectType resultType, CreatureTemplate.Type resultCritter)
    {
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[critterA], ScientistSlugcat.critsLibrary[critterB], 2, resultType, resultCritter);
    }

    public static void SetLibraryData(int x, int y, int tableSelect, AbstractPhysicalObject.AbstractObjectType resultType, CreatureTemplate.Type resultCritter)
    {
        if (ScientistSlugcat.showDebug)
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
            ScientistSlugcat.craftingGrid_ObjectsOnly[x, y] = new ScientistSlugcat.CraftDat(resultType, resultCritter);
            ScientistSlugcat.craftingGrid_ObjectsOnly[y, x] = new ScientistSlugcat.CraftDat(resultType, resultCritter);
            return;
        }
        if (tableSelect == 1)
        {
            ScientistSlugcat.craftingGrid_CritterObjects[x, y] = new ScientistSlugcat.CraftDat(resultType, resultCritter);
            return;
        }
        if (tableSelect == 2)
        {
            ScientistSlugcat.craftingGrid_CrittersOnly[x, y] = new ScientistSlugcat.CraftDat(resultType, resultCritter);
            ScientistSlugcat.craftingGrid_CrittersOnly[y, x] = new ScientistSlugcat.CraftDat(resultType, resultCritter);
        }
    }

    public static ScientistSlugcat.CraftDat GetFilteredLibraryData(Creature.Grasp graspA, Creature.Grasp graspB)
    {
        AbstractPhysicalObject.AbstractObjectType abstractObjectType = graspA.grabbed.abstractPhysicalObject.type;
        AbstractPhysicalObject.AbstractObjectType abstractObjectType2 = graspB.grabbed.abstractPhysicalObject.type;
        if (abstractObjectType == AbstractPhysicalObject.AbstractObjectType.WaterNut && graspA.grabbed is WaterNut) //未泡开泡水果 -> 石头  替换为 未泡开的泡水果 -> null（无法合成）
        {
            abstractObjectType = /*AbstractPhysicalObject.AbstractObjectType.Rock*/ /*abstractObjectType2*/ null;
        }
        if (abstractObjectType2 == AbstractPhysicalObject.AbstractObjectType.WaterNut && graspB.grabbed is WaterNut)
        {
            abstractObjectType2 = /*AbstractPhysicalObject.AbstractObjectType.Rock*/ /*abstractObjectType*/ null;
        }
        if (abstractObjectType == AbstractPhysicalObject.AbstractObjectType.PebblesPearl) // 五块卵石的珍珠 -> 珍珠
        {
            abstractObjectType = AbstractPhysicalObject.AbstractObjectType.DataPearl;
        }
        if (abstractObjectType2 == AbstractPhysicalObject.AbstractObjectType.PebblesPearl)
        {
            abstractObjectType2 = AbstractPhysicalObject.AbstractObjectType.DataPearl;
        }
        if (abstractObjectType == AbstractPhysicalObject.AbstractObjectType.SLOracleSwarmer) // 月姐神经元 -> fp神经元
        {
            abstractObjectType = AbstractPhysicalObject.AbstractObjectType.SSOracleSwarmer;
        }
        if (abstractObjectType2 == AbstractPhysicalObject.AbstractObjectType.SLOracleSwarmer)
        {
            abstractObjectType2 = AbstractPhysicalObject.AbstractObjectType.SSOracleSwarmer;
        }
        if (graspA.grabbed is Creature && graspB.grabbed is Creature)
        {
            return ScientistSlugcat.GetLibraryData((graspA.grabbed as Creature).abstractCreature.creatureTemplate.type, (graspB.grabbed as Creature).abstractCreature.creatureTemplate.type);
        }
        if (graspA.grabbed is Creature)
        {
            return ScientistSlugcat.GetLibraryData((graspA.grabbed as Creature).abstractCreature.creatureTemplate.type, abstractObjectType2);
        }
        if (graspB.grabbed is Creature)
        {
            return ScientistSlugcat.GetLibraryData((graspB.grabbed as Creature).abstractCreature.creatureTemplate.type, abstractObjectType);
        }
        return ScientistSlugcat.GetLibraryData(abstractObjectType, abstractObjectType2);
    }

    public static void InitIndex()
    {
        ScientistSlugcat.s2t.Add("Rock", AbstractPhysicalObject.AbstractObjectType.Rock);
        ScientistSlugcat.s2t.Add("FlareBomb", AbstractPhysicalObject.AbstractObjectType.FlareBomb);
        ScientistSlugcat.s2t.Add("VultureMask", AbstractPhysicalObject.AbstractObjectType.VultureMask);
        ScientistSlugcat.s2t.Add("PuffBall", AbstractPhysicalObject.AbstractObjectType.PuffBall);
        ScientistSlugcat.s2t.Add("DangleFruit", AbstractPhysicalObject.AbstractObjectType.DangleFruit);
        ScientistSlugcat.s2t.Add("SLOracleSwarmer", AbstractPhysicalObject.AbstractObjectType.SLOracleSwarmer);
        ScientistSlugcat.s2t.Add("SSOracleSwarmer", AbstractPhysicalObject.AbstractObjectType.SSOracleSwarmer);
        ScientistSlugcat.s2t.Add("DataPearl", AbstractPhysicalObject.AbstractObjectType.DataPearl);
        ScientistSlugcat.s2t.Add("WaterNut", AbstractPhysicalObject.AbstractObjectType.WaterNut);
        ScientistSlugcat.s2t.Add("JellyFish", AbstractPhysicalObject.AbstractObjectType.JellyFish);
        ScientistSlugcat.s2t.Add("Lantern", AbstractPhysicalObject.AbstractObjectType.Lantern);
        ScientistSlugcat.s2t.Add("KarmaFlower", AbstractPhysicalObject.AbstractObjectType.KarmaFlower);
        ScientistSlugcat.s2t.Add("Mushroom", AbstractPhysicalObject.AbstractObjectType.Mushroom);
        ScientistSlugcat.s2t.Add("FirecrackerPlant", AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant);
        ScientistSlugcat.s2t.Add("SlimeMold", AbstractPhysicalObject.AbstractObjectType.SlimeMold);
        ScientistSlugcat.s2t.Add("FlyLure", AbstractPhysicalObject.AbstractObjectType.FlyLure);
        ScientistSlugcat.s2t.Add("ScavengerBomb", AbstractPhysicalObject.AbstractObjectType.ScavengerBomb);
        ScientistSlugcat.s2t.Add("SporePlant", AbstractPhysicalObject.AbstractObjectType.SporePlant);
        ScientistSlugcat.s2t.Add("EggBugEgg", AbstractPhysicalObject.AbstractObjectType.EggBugEgg);
        ScientistSlugcat.s2t.Add("NeedleEgg", AbstractPhysicalObject.AbstractObjectType.NeedleEgg);
        ScientistSlugcat.s2t.Add("BubbleGrass", AbstractPhysicalObject.AbstractObjectType.BubbleGrass);
        ScientistSlugcat.s2t.Add("OverseerCarcass", AbstractPhysicalObject.AbstractObjectType.OverseerCarcass);
        ScientistSlugcat.s2t.Add("SingularityBomb", MoreSlugcatsEnums.AbstractObjectType.SingularityBomb);
        ScientistSlugcat.s2t.Add("FireEgg", MoreSlugcatsEnums.AbstractObjectType.FireEgg);
        ScientistSlugcat.s2t.Add("Seed", MoreSlugcatsEnums.AbstractObjectType.Seed);
        ScientistSlugcat.s2t.Add("GooieDuck", MoreSlugcatsEnums.AbstractObjectType.GooieDuck);
        ScientistSlugcat.s2t.Add("LillyPuck", MoreSlugcatsEnums.AbstractObjectType.LillyPuck);
        ScientistSlugcat.s2t.Add("GlowWeed", MoreSlugcatsEnums.AbstractObjectType.GlowWeed);
        ScientistSlugcat.s2t.Add("DandelionPeach", MoreSlugcatsEnums.AbstractObjectType.DandelionPeach);
        ScientistSlugcat.s2t.Add("Spear", AbstractPhysicalObject.AbstractObjectType.Spear);
        ScientistSlugcat.s2t.Add("VultureGrub", CreatureTemplate.Type.VultureGrub);
        ScientistSlugcat.s2t.Add("SmallCentipede", CreatureTemplate.Type.SmallCentipede);
        ScientistSlugcat.s2t.Add("SmallNeedleWorm", CreatureTemplate.Type.SmallNeedleWorm);
        ScientistSlugcat.s2t.Add("Hazer", CreatureTemplate.Type.Hazer);
        ScientistSlugcat.s2t.Add("Fly", CreatureTemplate.Type.Fly);

        foreach (DictionaryEntry de in ScientistSlugcat.s2t) { ScientistSlugcat.t2s[de.Value] = de.Key; }
    }

    public static void InitCraftingLibrary()        //打表x2
    {
        for (int i = 0; i < ScientistSlugcat.objectsLibrary.Count; i++)
        {
            for (int j = 0; j < ScientistSlugcat.objectsLibrary.Count; j++)
            {
                ScientistSlugcat.SetLibraryData(i, j, 0, null, null);
            }
        }
        for (int i = 0; i < ScientistSlugcat.critsLibrary.Count; i++)
        {
            for (int j = 0; j < ScientistSlugcat.objectsLibrary.Count; j++)
            {
                ScientistSlugcat.SetLibraryData(i, j, 1, null, null);
            }

            for (int j = 0; j < ScientistSlugcat.critsLibrary.Count; j++)
            {
                ScientistSlugcat.SetLibraryData(i, j, 2, null, null);
            }
        }

        int tableSelect = 0;
        AbstractPhysicalObject.AbstractObjectType key = AbstractPhysicalObject.AbstractObjectType.Rock;
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Rock], tableSelect, null, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlareBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.Lantern, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.VultureMask], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.PuffBall], tableSelect, AbstractPhysicalObject.AbstractObjectType.SporePlant, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DangleFruit], tableSelect, AbstractPhysicalObject.AbstractObjectType.FlareBomb, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SSOracleSwarmer], tableSelect, AbstractPhysicalObject.AbstractObjectType.OverseerCarcass, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DataPearl], tableSelect, null, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.WaterNut], tableSelect, AbstractPhysicalObject.AbstractObjectType.JellyFish, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.JellyFish], tableSelect, MoreSlugcatsEnums.AbstractObjectType.LillyPuck, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Lantern], tableSelect, AbstractPhysicalObject.AbstractObjectType.FlareBomb, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.KarmaFlower], tableSelect, AbstractPhysicalObject.AbstractObjectType.ScavengerBomb, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Mushroom], tableSelect, AbstractPhysicalObject.AbstractObjectType.PuffBall, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.ScavengerBomb, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SlimeMold], tableSelect, AbstractPhysicalObject.AbstractObjectType.Lantern, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlyLure], tableSelect, AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.PuffBall, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.NeedleEgg, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, null, CreatureTemplate.Type.Fly);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.ScavengerBomb, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, null, CreatureTemplate.Type.Fly);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.SporePlant, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.Lantern, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, AbstractPhysicalObject.AbstractObjectType.Lantern, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.PuffBall, null);

        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Spear], tableSelect, Scientist.Register.SharpSpear, null);
        key = AbstractPhysicalObject.AbstractObjectType.FlareBomb;
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlareBomb], tableSelect, null, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.VultureMask], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.PuffBall], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DangleFruit], tableSelect, AbstractPhysicalObject.AbstractObjectType.SlimeMold, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SSOracleSwarmer], tableSelect, AbstractPhysicalObject.AbstractObjectType.OverseerCarcass, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DataPearl], tableSelect, null, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.WaterNut], tableSelect, AbstractPhysicalObject.AbstractObjectType.JellyFish, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.JellyFish], tableSelect, MoreSlugcatsEnums.AbstractObjectType.LillyPuck, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Lantern], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GlowWeed, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.KarmaFlower], tableSelect, AbstractPhysicalObject.AbstractObjectType.Lantern, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Mushroom], tableSelect, AbstractPhysicalObject.AbstractObjectType.PuffBall, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.Lantern, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SlimeMold], tableSelect, AbstractPhysicalObject.AbstractObjectType.Lantern, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlyLure], tableSelect, AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.Lantern, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.SporePlant, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, null, CreatureTemplate.Type.VultureGrub);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.ScavengerBomb, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, null, CreatureTemplate.Type.VultureGrub);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.Lantern, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.Lantern, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, Scientist.Register.ColorfulFruit, null);                        //AbstractPhysicalObject.AbstractObjectType.Lantern
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.SporePlant, null);

        key = AbstractPhysicalObject.AbstractObjectType.VultureMask;
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.VultureMask], tableSelect, null, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.PuffBall], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DangleFruit], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SSOracleSwarmer], tableSelect, AbstractPhysicalObject.AbstractObjectType.OverseerCarcass, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DataPearl], tableSelect, null, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.WaterNut], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.JellyFish], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Lantern], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.KarmaFlower], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Mushroom], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SlimeMold], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlyLure], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, null, CreatureTemplate.Type.VultureGrub);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.ScavengerBomb, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, null, CreatureTemplate.Type.VultureGrub);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        key = AbstractPhysicalObject.AbstractObjectType.PuffBall;
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.PuffBall], tableSelect, null, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DangleFruit], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SSOracleSwarmer], tableSelect, AbstractPhysicalObject.AbstractObjectType.OverseerCarcass, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DataPearl], tableSelect, null, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.WaterNut], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GooieDuck, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.JellyFish], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GooieDuck, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Lantern], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.KarmaFlower], tableSelect, AbstractPhysicalObject.AbstractObjectType.NeedleEgg, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Mushroom], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GooieDuck, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.ScavengerBomb, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SlimeMold], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlyLure], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GooieDuck, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GooieDuck, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GooieDuck, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, null, CreatureTemplate.Type.SmallCentipede);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.ScavengerBomb, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, null, CreatureTemplate.Type.SmallCentipede);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GooieDuck, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GooieDuck, null);
        key = AbstractPhysicalObject.AbstractObjectType.DangleFruit;
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DangleFruit], tableSelect, Scientist.Register.ConcentratedDangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SSOracleSwarmer], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DataPearl], tableSelect, null, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.WaterNut], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.JellyFish], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Lantern], tableSelect, AbstractPhysicalObject.AbstractObjectType.FlareBomb, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.KarmaFlower], tableSelect, MoreSlugcatsEnums.AbstractObjectType.Seed, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Mushroom], tableSelect, Scientist.Register.PainlessFruit, null);                  //MoreSlugcatsEnums.AbstractObjectType.GooieDuck
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.SporePlant, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SlimeMold], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlyLure], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GooieDuck, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, null, CreatureTemplate.Type.Fly);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, MoreSlugcatsEnums.AbstractObjectType.LillyPuck, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        key = AbstractPhysicalObject.AbstractObjectType.SSOracleSwarmer;
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SSOracleSwarmer], tableSelect, null, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DataPearl], tableSelect, AbstractPhysicalObject.AbstractObjectType.OverseerCarcass, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.WaterNut], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.JellyFish], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Lantern], tableSelect, AbstractPhysicalObject.AbstractObjectType.OverseerCarcass, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.KarmaFlower], tableSelect, AbstractPhysicalObject.AbstractObjectType.OverseerCarcass, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Mushroom], tableSelect, AbstractPhysicalObject.AbstractObjectType.OverseerCarcass, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.OverseerCarcass, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SlimeMold], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlyLure], tableSelect, AbstractPhysicalObject.AbstractObjectType.OverseerCarcass, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.OverseerCarcass, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.OverseerCarcass, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.OverseerCarcass, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, AbstractPhysicalObject.AbstractObjectType.OverseerCarcass, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.OverseerCarcass, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        key = AbstractPhysicalObject.AbstractObjectType.DataPearl;
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DataPearl], tableSelect, null, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.WaterNut], tableSelect, null, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.JellyFish], tableSelect, null, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Lantern], tableSelect, null, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.KarmaFlower], tableSelect, null, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Mushroom], tableSelect, null, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant], tableSelect, null, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SlimeMold], tableSelect, null, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlyLure], tableSelect, null, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb], tableSelect, null, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, null, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, null, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, null, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, null, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, null, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, null, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, null, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, null, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, null, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, null, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, null, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, null, null);
        key = AbstractPhysicalObject.AbstractObjectType.WaterNut;
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.WaterNut], tableSelect, null, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.JellyFish], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Lantern], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GlowWeed, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.KarmaFlower], tableSelect, MoreSlugcatsEnums.AbstractObjectType.Seed, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Mushroom], tableSelect, AbstractPhysicalObject.AbstractObjectType.NeedleEgg, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.BubbleGrass, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SlimeMold], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlyLure], tableSelect, AbstractPhysicalObject.AbstractObjectType.BubbleGrass, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb], tableSelect, null, CreatureTemplate.Type.Snail);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, null, CreatureTemplate.Type.Hazer);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, null, CreatureTemplate.Type.Hazer);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, MoreSlugcatsEnums.AbstractObjectType.LillyPuck, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        key = AbstractPhysicalObject.AbstractObjectType.JellyFish;
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.JellyFish], tableSelect, null, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Lantern], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GlowWeed, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.KarmaFlower], tableSelect, AbstractPhysicalObject.AbstractObjectType.NeedleEgg, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Mushroom], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GooieDuck, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.FlyLure, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SlimeMold], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlyLure], tableSelect, AbstractPhysicalObject.AbstractObjectType.BubbleGrass, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb], tableSelect, null, CreatureTemplate.Type.Snail);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, null, CreatureTemplate.Type.Hazer);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, null, CreatureTemplate.Type.Snail);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, MoreSlugcatsEnums.AbstractObjectType.LillyPuck, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        key = AbstractPhysicalObject.AbstractObjectType.Lantern;
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Lantern], tableSelect, null, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.KarmaFlower], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Mushroom], tableSelect, AbstractPhysicalObject.AbstractObjectType.PuffBall, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.ScavengerBomb, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SlimeMold], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlyLure], tableSelect, AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.NeedleEgg, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, null, CreatureTemplate.Type.VultureGrub);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GlowWeed, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.ScavengerBomb, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, null, CreatureTemplate.Type.VultureGrub);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, MoreSlugcatsEnums.AbstractObjectType.LillyPuck, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GlowWeed, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, MoreSlugcatsEnums.AbstractObjectType.LillyPuck, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GlowWeed, null);
        key = AbstractPhysicalObject.AbstractObjectType.KarmaFlower;
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.KarmaFlower], tableSelect, null, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Mushroom], tableSelect, AbstractPhysicalObject.AbstractObjectType.NeedleEgg, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.ScavengerBomb, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SlimeMold], tableSelect, MoreSlugcatsEnums.AbstractObjectType.Seed, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlyLure], tableSelect, MoreSlugcatsEnums.AbstractObjectType.Seed, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.ScavengerBomb, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.NeedleEgg, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.OverseerCarcass, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, MoreSlugcatsEnums.AbstractObjectType.Seed, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.SSOracleSwarmer, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, MoreSlugcatsEnums.AbstractObjectType.SingularityBomb, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, AbstractPhysicalObject.AbstractObjectType.OverseerCarcass, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, MoreSlugcatsEnums.AbstractObjectType.Seed, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, MoreSlugcatsEnums.AbstractObjectType.Seed, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, MoreSlugcatsEnums.AbstractObjectType.Seed, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, MoreSlugcatsEnums.AbstractObjectType.Seed, null);
        key = AbstractPhysicalObject.AbstractObjectType.Mushroom;
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Mushroom], tableSelect, null, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant], tableSelect, null, CreatureTemplate.Type.SmallCentipede);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SlimeMold], tableSelect, null, CreatureTemplate.Type.SmallNeedleWorm);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlyLure], tableSelect, null, CreatureTemplate.Type.Fly);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.SporePlant, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GooieDuck, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GooieDuck, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, null, CreatureTemplate.Type.SmallCentipede);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, null, CreatureTemplate.Type.Hazer);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, null, CreatureTemplate.Type.SmallCentipede);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.ScavengerBomb, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, null, CreatureTemplate.Type.SmallCentipede);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.PuffBall, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GooieDuck, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GooieDuck, null);                                             // MoreSlugcatsEnums.AbstractObjectType.GooieDuck
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GooieDuck, null);
        key = AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant;
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant], tableSelect, null, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SlimeMold], tableSelect, AbstractPhysicalObject.AbstractObjectType.Lantern, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlyLure], tableSelect, AbstractPhysicalObject.AbstractObjectType.SporePlant, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.Lantern, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.ScavengerBomb, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.SporePlant, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, null, CreatureTemplate.Type.SmallCentipede);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, null, CreatureTemplate.Type.Snail);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.ScavengerBomb, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, null, CreatureTemplate.Type.SmallCentipede);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.SporePlant, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.SporePlant, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, AbstractPhysicalObject.AbstractObjectType.SporePlant, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.SporePlant, null);
        key = AbstractPhysicalObject.AbstractObjectType.SlimeMold;
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SlimeMold], tableSelect, null, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlyLure], tableSelect, AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.Lantern, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, null, CreatureTemplate.Type.SmallCentipede);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        key = AbstractPhysicalObject.AbstractObjectType.FlyLure;
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlyLure], tableSelect, null, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.PuffBall, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GooieDuck, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, null, CreatureTemplate.Type.Fly);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.ScavengerBomb, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, null, CreatureTemplate.Type.Fly);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, null, CreatureTemplate.Type.Hazer);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, null, CreatureTemplate.Type.SmallCentipede);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, null, CreatureTemplate.Type.Fly);
        key = AbstractPhysicalObject.AbstractObjectType.ScavengerBomb;
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb], tableSelect, null, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.PuffBall, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, null, CreatureTemplate.Type.SmallCentipede);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, AbstractPhysicalObject.AbstractObjectType.PuffBall, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, MoreSlugcatsEnums.AbstractObjectType.SingularityBomb, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, null, CreatureTemplate.Type.SmallCentipede);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.PuffBall, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.SporePlant, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, AbstractPhysicalObject.AbstractObjectType.Lantern, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.PuffBall, null);

        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Spear], tableSelect, AbstractPhysicalObject.AbstractObjectType.Spear, null);
        key = AbstractPhysicalObject.AbstractObjectType.SporePlant;
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, null, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.PuffBall, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, null, CreatureTemplate.Type.TubeWorm);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, AbstractPhysicalObject.AbstractObjectType.PuffBall, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.ScavengerBomb, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, null, CreatureTemplate.Type.TubeWorm);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.PuffBall, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.JellyFish, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, AbstractPhysicalObject.AbstractObjectType.JellyFish, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        key = AbstractPhysicalObject.AbstractObjectType.EggBugEgg;
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, null, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, null, CreatureTemplate.Type.SmallNeedleWorm);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        key = AbstractPhysicalObject.AbstractObjectType.NeedleEgg;
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, null, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, null, CreatureTemplate.Type.Hazer);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, null, CreatureTemplate.Type.VultureGrub);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, null, CreatureTemplate.Type.VultureGrub);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, null, CreatureTemplate.Type.SmallNeedleWorm);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, null, CreatureTemplate.Type.TubeWorm);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, null, CreatureTemplate.Type.Hazer);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, null, CreatureTemplate.Type.Hazer);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, null, CreatureTemplate.Type.SmallNeedleWorm);
        key = AbstractPhysicalObject.AbstractObjectType.BubbleGrass;
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, null, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.ScavengerBomb, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, null, CreatureTemplate.Type.Hazer);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.PuffBall, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.JellyFish, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, MoreSlugcatsEnums.AbstractObjectType.LillyPuck, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GlowWeed, null);
        key = AbstractPhysicalObject.AbstractObjectType.OverseerCarcass;
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, null, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        key = MoreSlugcatsEnums.AbstractObjectType.SingularityBomb;
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, null, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.KarmaFlower, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        key = MoreSlugcatsEnums.AbstractObjectType.FireEgg;
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, null, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        key = MoreSlugcatsEnums.AbstractObjectType.Seed;
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, null, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        key = MoreSlugcatsEnums.AbstractObjectType.GooieDuck;
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, null, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        key = MoreSlugcatsEnums.AbstractObjectType.LillyPuck;
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, null, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        key = MoreSlugcatsEnums.AbstractObjectType.GlowWeed;
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, null, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Spear], tableSelect, Scientist.Register.InflatableGlowingShield, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[Scientist.Register.SharpSpear], tableSelect, Scientist.Register.InflatableGlowingShield, null);
        key = MoreSlugcatsEnums.AbstractObjectType.DandelionPeach;
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, null, null);
        tableSelect = 1;
        CreatureTemplate.Type key2 = CreatureTemplate.Type.Fly;
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Rock], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlareBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.VultureMask], tableSelect, MoreSlugcatsEnums.AbstractObjectType.Seed, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.PuffBall], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DangleFruit], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SSOracleSwarmer], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DataPearl], tableSelect, null, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.WaterNut], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.JellyFish], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Lantern], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.KarmaFlower], tableSelect, AbstractPhysicalObject.AbstractObjectType.FlyLure, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Mushroom], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GooieDuck, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.SporePlant, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SlimeMold], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlyLure], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.FlyLure, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, MoreSlugcatsEnums.AbstractObjectType.Seed, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, AbstractPhysicalObject.AbstractObjectType.FlyLure, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        key2 = CreatureTemplate.Type.SmallCentipede;
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Rock], tableSelect, AbstractPhysicalObject.AbstractObjectType.SporePlant, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlareBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.SporePlant, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.VultureMask], tableSelect, MoreSlugcatsEnums.AbstractObjectType.Seed, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.PuffBall], tableSelect, AbstractPhysicalObject.AbstractObjectType.SporePlant, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DangleFruit], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SSOracleSwarmer], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DataPearl], tableSelect, null, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.WaterNut], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.JellyFish], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Lantern], tableSelect, AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.KarmaFlower], tableSelect, AbstractPhysicalObject.AbstractObjectType.PuffBall, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Mushroom], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GooieDuck, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.SporePlant, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SlimeMold], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlyLure], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, null, CreatureTemplate.Type.TubeWorm);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, MoreSlugcatsEnums.AbstractObjectType.Seed, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, AbstractPhysicalObject.AbstractObjectType.JellyFish, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        key2 = CreatureTemplate.Type.VultureGrub;
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Rock], tableSelect, AbstractPhysicalObject.AbstractObjectType.Lantern, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlareBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.Lantern, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.VultureMask], tableSelect, MoreSlugcatsEnums.AbstractObjectType.Seed, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.PuffBall], tableSelect, AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DangleFruit], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SSOracleSwarmer], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DataPearl], tableSelect, null, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.WaterNut], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.JellyFish], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Lantern], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.KarmaFlower], tableSelect, AbstractPhysicalObject.AbstractObjectType.VultureMask, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Mushroom], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GooieDuck, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.SporePlant, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SlimeMold], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlyLure], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.FlareBomb, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, MoreSlugcatsEnums.AbstractObjectType.Seed, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GlowWeed, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        key2 = CreatureTemplate.Type.SmallNeedleWorm;
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Rock], tableSelect, AbstractPhysicalObject.AbstractObjectType.PuffBall, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlareBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.PuffBall, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.VultureMask], tableSelect, MoreSlugcatsEnums.AbstractObjectType.Seed, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.PuffBall], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DangleFruit], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SSOracleSwarmer], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DataPearl], tableSelect, null, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.WaterNut], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.JellyFish], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Lantern], tableSelect, AbstractPhysicalObject.AbstractObjectType.NeedleEgg, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.KarmaFlower], tableSelect, AbstractPhysicalObject.AbstractObjectType.NeedleEgg, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Mushroom], tableSelect, AbstractPhysicalObject.AbstractObjectType.PuffBall, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.SporePlant, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SlimeMold], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlyLure], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.PuffBall, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.PuffBall, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, MoreSlugcatsEnums.AbstractObjectType.Seed, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        key2 = CreatureTemplate.Type.Hazer;
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Rock], tableSelect, MoreSlugcatsEnums.AbstractObjectType.LillyPuck, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlareBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GlowWeed, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.VultureMask], tableSelect, MoreSlugcatsEnums.AbstractObjectType.Seed, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.PuffBall], tableSelect, AbstractPhysicalObject.AbstractObjectType.BubbleGrass, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DangleFruit], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SSOracleSwarmer], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DataPearl], tableSelect, null, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.WaterNut], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.JellyFish], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Lantern], tableSelect, null, CreatureTemplate.Type.Snail);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.KarmaFlower], tableSelect, AbstractPhysicalObject.AbstractObjectType.BubbleGrass, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Mushroom], tableSelect, AbstractPhysicalObject.AbstractObjectType.BubbleGrass, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.JellyFish, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SlimeMold], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlyLure], tableSelect, AbstractPhysicalObject.AbstractObjectType.BubbleGrass, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.FlareBomb, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.JellyFish, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, MoreSlugcatsEnums.AbstractObjectType.Seed, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GlowWeed, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        /*Custom.Log(new string[]
        {
                "--Setup Creature + Creature table"
        });*/
        tableSelect = 2;
        key2 = CreatureTemplate.Type.Fly;
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.critsLibrary[CreatureTemplate.Type.Fly], tableSelect, null, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.critsLibrary[CreatureTemplate.Type.VultureGrub], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.critsLibrary[CreatureTemplate.Type.SmallCentipede], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.critsLibrary[CreatureTemplate.Type.SmallNeedleWorm], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.critsLibrary[CreatureTemplate.Type.Hazer], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        key2 = CreatureTemplate.Type.VultureGrub;
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.critsLibrary[CreatureTemplate.Type.VultureGrub], tableSelect, null, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.critsLibrary[CreatureTemplate.Type.SmallCentipede], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.critsLibrary[CreatureTemplate.Type.SmallNeedleWorm], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.critsLibrary[CreatureTemplate.Type.Hazer], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        key2 = CreatureTemplate.Type.SmallCentipede;
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.critsLibrary[CreatureTemplate.Type.SmallCentipede], tableSelect, null, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.critsLibrary[CreatureTemplate.Type.SmallNeedleWorm], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.critsLibrary[CreatureTemplate.Type.Hazer], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        key2 = CreatureTemplate.Type.SmallNeedleWorm;
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.critsLibrary[CreatureTemplate.Type.SmallNeedleWorm], tableSelect, null, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.critsLibrary[CreatureTemplate.Type.Hazer], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        key2 = CreatureTemplate.Type.Hazer;
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.critsLibrary[CreatureTemplate.Type.Hazer], tableSelect, null, null);
    }

    public static AbstractPhysicalObject CraftingResults(PhysicalObject crafter, Creature.Grasp graspA, Creature.Grasp graspB)
    {
        AbstractPhysicalObject.AbstractObjectType abstractObjectType = ScientistSlugcat.CraftingResults_ObjectData(graspA, graspB, true);
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
            CreatureTemplate.Type type = ScientistSlugcat.CraftingResults_CreatureData(graspA, graspB);
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
            return ScientistSlugcat.GetLibraryData((graspA.grabbed as Creature).abstractCreature.creatureTemplate.type, (graspB.grabbed as Creature).abstractCreature.creatureTemplate.type).crit;
        }
        if (graspA.grabbed is Creature)
        {
            return ScientistSlugcat.GetLibraryData((graspA.grabbed as Creature).abstractCreature.creatureTemplate.type, graspB.grabbed.abstractPhysicalObject.type).crit;
        }
        if (graspB.grabbed is Creature)
        {
            return ScientistSlugcat.GetLibraryData((graspB.grabbed as Creature).abstractCreature.creatureTemplate.type, graspA.grabbed.abstractPhysicalObject.type).crit;
        }
        return ScientistSlugcat.GetLibraryData(graspA.grabbed.abstractPhysicalObject.type, graspB.grabbed.abstractPhysicalObject.type).crit;
    }

    public static ScientistSlugcat.CraftDat GetLibraryData(AbstractPhysicalObject.AbstractObjectType objectA, AbstractPhysicalObject.AbstractObjectType objectB)
    {
        if (objectA == null || objectB == null)
        {
            return new ScientistSlugcat.CraftDat(null, null);
        }
        if (ScientistSlugcat.objectsLibrary.ContainsKey(objectA) && ScientistSlugcat.objectsLibrary.ContainsKey(objectB))
        {
            int num = ScientistSlugcat.objectsLibrary[objectA];
            int num2 = ScientistSlugcat.objectsLibrary[objectB];
            return ScientistSlugcat.craftingGrid_ObjectsOnly[num, num2];
        }
        return new ScientistSlugcat.CraftDat(null, null);
    }

    public static ScientistSlugcat.CraftDat GetLibraryData(CreatureTemplate.Type critterA, AbstractPhysicalObject.AbstractObjectType objectB)
    {
        if (critterA == null || objectB == null)
        {
            return new ScientistSlugcat.CraftDat(null, null);
        }
        if (ScientistSlugcat.critsLibrary.ContainsKey(critterA) && ScientistSlugcat.objectsLibrary.ContainsKey(objectB))
        {
            int num = ScientistSlugcat.critsLibrary[critterA];
            int num2 = ScientistSlugcat.objectsLibrary[objectB];
            return ScientistSlugcat.craftingGrid_CritterObjects[num, num2];
        }
        return new ScientistSlugcat.CraftDat(null, null);
    }

    public static ScientistSlugcat.CraftDat GetLibraryData(CreatureTemplate.Type critterA, CreatureTemplate.Type critterB)
    {
        if (critterA == null || critterB == null)
        {
            return new ScientistSlugcat.CraftDat(null, null);
        }
        if (ScientistSlugcat.critsLibrary.ContainsKey(critterA) && ScientistSlugcat.critsLibrary.ContainsKey(critterB))
        {
            int num = ScientistSlugcat.critsLibrary[critterA];
            int num2 = ScientistSlugcat.critsLibrary[critterB];
            return ScientistSlugcat.craftingGrid_CrittersOnly[num, num2];
        }
        return new ScientistSlugcat.CraftDat(null, null);
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
        ScientistSlugcat.CraftDat filteredLibraryData = ScientistSlugcat.GetFilteredLibraryData(graspA, graspB);
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

    public static bool APO_Compare(AbstractPhysicalObject grasp1, AbstractPhysicalObject grasp2, AbstractPhysicalObject.AbstractObjectType item1, AbstractPhysicalObject.AbstractObjectType item2)
    {
        return APO_Compare(grasp1.type, grasp2.type, item1, item2);
    }

    public static bool APO_Compare(AbstractPhysicalObject.AbstractObjectType grasp1, AbstractPhysicalObject.AbstractObjectType grasp2, AbstractPhysicalObject.AbstractObjectType item1, AbstractPhysicalObject.AbstractObjectType item2)
    {
        return ((grasp1 == item1 && grasp2 == item2) || (grasp1 == item2 && grasp2 == item1));
    }

    public static AbstractPhysicalObject GetSpecialCraftingResult(Player player)
    {
        if (player.grasps[0] != null && player.grasps[1] != null)
        {
            return GetSpecialCraftingResult(player.grasps[0].grabbed.abstractPhysicalObject.type, player.grasps[1].grabbed.abstractPhysicalObject.type, player);
        }
        return null;
    }

    public static AbstractPhysicalObject GetSpecialCraftingResult(AbstractPhysicalObject.AbstractObjectType a, AbstractPhysicalObject.AbstractObjectType b, Player player)
    {
        if (APO_Compare(a, b, AbstractPhysicalObject.AbstractObjectType.Spear, AbstractPhysicalObject.AbstractObjectType.Rock))
        {
            return new items.AbstractPhysicalObjects.SharpSpearAbstract(player.room.world, null, player.abstractCreature.pos, player.room.game.GetNewID()); //自定义的ShapeSpearAbstract，覆写了realizedObject
        }
        if (APO_Compare(a, b, AbstractPhysicalObject.AbstractObjectType.Spear, AbstractPhysicalObject.AbstractObjectType.ScavengerBomb))
        {
            return new AbstractSpear(player.room.world, null, player.abstractCreature.pos, player.room.game.GetNewID(), true);
        }
        if (APO_Compare(a, b, AbstractPhysicalObject.AbstractObjectType.WaterNut, AbstractPhysicalObject.AbstractObjectType.WaterNut))
        {
            if (player.grasps[0].grabbed is WaterNut && player.grasps[1].grabbed is WaterNut)
            {
                return null;
            }
            if ( (player.grasps[0].grabbed is WaterNut && player.grasps[1].grabbed is SwollenWaterNut) || (player.grasps[0].grabbed is SwollenWaterNut && player.grasps[1].grabbed is WaterNut))
            {
                return new items.AbstractPhysicalObjects.SharpSpearAbstract(player.room.world, null, player.abstractCreature.pos, player.room.game.GetNewID());
            }
            if (player.grasps[0].grabbed is SwollenWaterNut && player.grasps[1].grabbed is SwollenWaterNut)
            {
                return null;
            }
        }
        return null;
    }
}
