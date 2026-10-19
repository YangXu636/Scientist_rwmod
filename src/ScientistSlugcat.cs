using System.Collections;
using System;
using System.Collections.Generic;
using MoreSlugcats;
using RWCustom;
using UnityEngine;
using System.Reflection;
using Scientist.Items;
using Unity.Mathematics;

namespace Scientist;

public class ScientistSlugcat
{
    public static Dictionary<AbstractPhysicalObject.AbstractObjectType, int> objectsLibrary;
    public static Dictionary<CreatureTemplate.Type, int> critsLibrary;
    public static ScientistSlugcat.CraftDat[,] craftingGrid_ObjectsOnly;
    public static int[,] craftingGrid_ObjectsOnly_Enable;
    public static ScientistSlugcat.CraftDat[,] craftingGrid_CritterObjects;
    public static int[,] craftingGrid_CritterObjects_Enable;
    public static ScientistSlugcat.CraftDat[,] craftingGrid_CrittersOnly;
    public static int[,] craftingGrid_CrittersOnly_Enable;
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
        ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb] = num; //手雷（炸弹）17
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
        ScientistSlugcat.objectsLibrary[DLCSharedEnums.AbstractObjectType.SingularityBomb] = num;
        num++;
        ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg] = num;
        num++;
        ScientistSlugcat.objectsLibrary[DLCSharedEnums.AbstractObjectType.Seed] = num;
        num++;
        ScientistSlugcat.objectsLibrary[DLCSharedEnums.AbstractObjectType.GooieDuck] = num;
        num++;
        ScientistSlugcat.objectsLibrary[DLCSharedEnums.AbstractObjectType.LillyPuck] = num;
        num++;
        ScientistSlugcat.objectsLibrary[DLCSharedEnums.AbstractObjectType.GlowWeed] = num;
        num++;
        ScientistSlugcat.objectsLibrary[DLCSharedEnums.AbstractObjectType.DandelionPeach] = num;
        num++;
        //新增物品
        ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Spear] = num;
        num++;
        ScientistSlugcat.objectsLibrary[Scientist.Enums.Items.SharpSpear] = num;
        num++;
        ScientistSlugcat.objectsLibrary[Scientist.Enums.Items.Knot] = num;
        num++;
        ScientistSlugcat.objectsLibrary[Scientist.Enums.Items._WaterNut] = num;
        num++;
        ScientistSlugcat.objectsLibrary[Scientist.Enums.Items.SmallRock] = num;     //小石头（设计表中作为合成材料：小石头+石头→切割石刀 等）
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
        ScientistSlugcat.critsLibrary[CreatureTemplate.Type.Snail] = num2;
        num2++;

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
        if (abstractObjectType == AbstractPhysicalObject.AbstractObjectType.WaterNut && graspA.grabbed is WaterNut) //未泡开泡水果 -> 石头  替换为 未泡开的泡水果 -> null（无法合成） 替换为 未泡开的泡水果 -> 科学家泡水果（区分）
        {
            abstractObjectType = /*AbstractPhysicalObject.AbstractObjectType.Rock*/ /*abstractObjectType2*/ /*null*/ Enums.Items._WaterNut;
        }
        if (abstractObjectType2 == AbstractPhysicalObject.AbstractObjectType.WaterNut && graspB.grabbed is WaterNut)
        {
            abstractObjectType2 = /*AbstractPhysicalObject.AbstractObjectType.Rock*/ /*abstractObjectType*/ /*null*/ Enums.Items._WaterNut;
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
        ScientistSlugcat.s2t.Add("SingularityBomb", DLCSharedEnums.AbstractObjectType.SingularityBomb);
        ScientistSlugcat.s2t.Add("FireEgg", MoreSlugcatsEnums.AbstractObjectType.FireEgg);
        ScientistSlugcat.s2t.Add("Seed", DLCSharedEnums.AbstractObjectType.Seed);
        ScientistSlugcat.s2t.Add("GooieDuck", DLCSharedEnums.AbstractObjectType.GooieDuck);
        ScientistSlugcat.s2t.Add("LillyPuck", DLCSharedEnums.AbstractObjectType.LillyPuck);
        ScientistSlugcat.s2t.Add("GlowWeed", DLCSharedEnums.AbstractObjectType.GlowWeed);
        ScientistSlugcat.s2t.Add("DandelionPeach", DLCSharedEnums.AbstractObjectType.DandelionPeach);
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
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Rock], tableSelect, Enums.Items.SmallRock, null);                                               //null
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlareBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.Lantern, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.VultureMask], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.PuffBall], tableSelect, AbstractPhysicalObject.AbstractObjectType.SporePlant, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DangleFruit], tableSelect, AbstractPhysicalObject.AbstractObjectType.FlareBomb, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SSOracleSwarmer], tableSelect, AbstractPhysicalObject.AbstractObjectType.OverseerCarcass, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DataPearl], tableSelect, null, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.WaterNut], tableSelect, AbstractPhysicalObject.AbstractObjectType.JellyFish, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.JellyFish], tableSelect, MoreSlugcatsEnums.AbstractObjectType.LillyPuck, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Lantern], tableSelect, AbstractPhysicalObject.AbstractObjectType.FlareBomb, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.KarmaFlower], tableSelect, AbstractPhysicalObject.AbstractObjectType.ScavengerBomb, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Mushroom], tableSelect, AbstractPhysicalObject.AbstractObjectType.PuffBall, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant], tableSelect, Enums.Items.ExplosivePowder, null);                             //AbstractPhysicalObject.AbstractObjectType.ScavengerBomb
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SlimeMold], tableSelect, AbstractPhysicalObject.AbstractObjectType.Lantern, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlyLure], tableSelect, AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb], tableSelect, Enums.Items.BlastingStoneBomb, null);                              //AbstractPhysicalObject.AbstractObjectType.DataPearl
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.PuffBall, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.NeedleEgg, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, null, CreatureTemplate.Type.Fly);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.ScavengerBomb, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, null, CreatureTemplate.Type.Fly);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.SporePlant, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.Lantern, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, AbstractPhysicalObject.AbstractObjectType.Lantern, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.PuffBall, null);

        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Spear], tableSelect, Scientist.Enums.Items.SharpSpear, null);
        key = AbstractPhysicalObject.AbstractObjectType.FlareBomb;
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlareBomb], tableSelect, null, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.VultureMask], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.PuffBall], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DangleFruit], tableSelect, AbstractPhysicalObject.AbstractObjectType.SlimeMold, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SSOracleSwarmer], tableSelect, AbstractPhysicalObject.AbstractObjectType.OverseerCarcass, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DataPearl], tableSelect, null, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.WaterNut], tableSelect, AbstractPhysicalObject.AbstractObjectType.JellyFish, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.JellyFish], tableSelect, MoreSlugcatsEnums.AbstractObjectType.LillyPuck, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Lantern], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GlowWeed, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.KarmaFlower], tableSelect, AbstractPhysicalObject.AbstractObjectType.Lantern, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Mushroom], tableSelect, Scientist.Enums.Items.ColorfulFruit, null);                                            // AbstractPhysicalObject.AbstractObjectType.PuffBall
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.Lantern, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SlimeMold], tableSelect, AbstractPhysicalObject.AbstractObjectType.Lantern, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlyLure], tableSelect, AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.Lantern, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.SporePlant, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, null, CreatureTemplate.Type.VultureGrub);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.ScavengerBomb, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, null, CreatureTemplate.Type.VultureGrub);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.Lantern, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.Lantern, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, AbstractPhysicalObject.AbstractObjectType.Lantern, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.SporePlant, null);

        key = AbstractPhysicalObject.AbstractObjectType.VultureMask;
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.VultureMask], tableSelect, null, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.PuffBall], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DangleFruit], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SSOracleSwarmer], tableSelect, AbstractPhysicalObject.AbstractObjectType.OverseerCarcass, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DataPearl], tableSelect, null, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.WaterNut], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.JellyFish], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Lantern], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.KarmaFlower], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Mushroom], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SlimeMold], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlyLure], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, null, CreatureTemplate.Type.VultureGrub);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.ScavengerBomb, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, null, CreatureTemplate.Type.VultureGrub);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        key = AbstractPhysicalObject.AbstractObjectType.PuffBall;
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.PuffBall], tableSelect, null, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DangleFruit], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SSOracleSwarmer], tableSelect, AbstractPhysicalObject.AbstractObjectType.OverseerCarcass, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DataPearl], tableSelect, null, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.WaterNut], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GooieDuck, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.JellyFish], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GooieDuck, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Lantern], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.KarmaFlower], tableSelect, AbstractPhysicalObject.AbstractObjectType.NeedleEgg, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Mushroom], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GooieDuck, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.ScavengerBomb, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SlimeMold], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlyLure], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GooieDuck, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GooieDuck, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GooieDuck, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, null, CreatureTemplate.Type.SmallCentipede);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.ScavengerBomb, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, null, CreatureTemplate.Type.SmallCentipede);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GooieDuck, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GooieDuck, null);
        key = AbstractPhysicalObject.AbstractObjectType.DangleFruit;
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DangleFruit], tableSelect, Scientist.Enums.Items.ConcentratedDangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SSOracleSwarmer], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DataPearl], tableSelect, null, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.WaterNut], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.JellyFish], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Lantern], tableSelect, AbstractPhysicalObject.AbstractObjectType.FlareBomb, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.KarmaFlower], tableSelect, MoreSlugcatsEnums.AbstractObjectType.Seed, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Mushroom], tableSelect, Scientist.Enums.Items.PainlessFruit, null);                  //MoreSlugcatsEnums.AbstractObjectType.GooieDuck
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.SporePlant, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SlimeMold], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlyLure], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GooieDuck, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, null, CreatureTemplate.Type.Fly);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, MoreSlugcatsEnums.AbstractObjectType.LillyPuck, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        key = AbstractPhysicalObject.AbstractObjectType.SSOracleSwarmer;
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SSOracleSwarmer], tableSelect, null, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DataPearl], tableSelect, AbstractPhysicalObject.AbstractObjectType.OverseerCarcass, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.WaterNut], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.JellyFish], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Lantern], tableSelect, AbstractPhysicalObject.AbstractObjectType.OverseerCarcass, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.KarmaFlower], tableSelect, AbstractPhysicalObject.AbstractObjectType.OverseerCarcass, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Mushroom], tableSelect, AbstractPhysicalObject.AbstractObjectType.OverseerCarcass, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.OverseerCarcass, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SlimeMold], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlyLure], tableSelect, AbstractPhysicalObject.AbstractObjectType.OverseerCarcass, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.OverseerCarcass, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.OverseerCarcass, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.OverseerCarcass, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, AbstractPhysicalObject.AbstractObjectType.OverseerCarcass, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.OverseerCarcass, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        key = AbstractPhysicalObject.AbstractObjectType.DataPearl;
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DataPearl], tableSelect, null, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.WaterNut], tableSelect, null, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.JellyFish], tableSelect, null, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Lantern], tableSelect, null, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.KarmaFlower], tableSelect, null, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Mushroom], tableSelect, null, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant], tableSelect, null, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SlimeMold], tableSelect, null, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlyLure], tableSelect, null, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb], tableSelect, null, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, null, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, null, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, null, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, null, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, null, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, null, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, null, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, null, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, null, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, null, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, null, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, null, null);
        key = AbstractPhysicalObject.AbstractObjectType.WaterNut;
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.WaterNut], tableSelect, null, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.JellyFish], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Lantern], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GlowWeed, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.KarmaFlower], tableSelect, MoreSlugcatsEnums.AbstractObjectType.Seed, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Mushroom], tableSelect, AbstractPhysicalObject.AbstractObjectType.NeedleEgg, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant], tableSelect, Scientist.Enums.Items.ExpansionBomb, null);                 //AbstractPhysicalObject.AbstractObjectType.BubbleGrass
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SlimeMold], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlyLure], tableSelect, AbstractPhysicalObject.AbstractObjectType.BubbleGrass, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb], tableSelect, null, CreatureTemplate.Type.Snail);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, null, CreatureTemplate.Type.Hazer);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, null, CreatureTemplate.Type.Hazer);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, Scientist.Enums.Items.BreathingBubble, null);                    //MoreSlugcatsEnums.AbstractObjectType.LillyPuck
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[DLCSharedEnums.AbstractObjectType.GlowWeed], tableSelect, Scientist.Enums.Items.KongmingLantern, null);                            //AbstractPhysicalObject.AbstractObjectType.DangleFruit
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        key = AbstractPhysicalObject.AbstractObjectType.JellyFish;
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.JellyFish], tableSelect, null, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Lantern], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GlowWeed, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.KarmaFlower], tableSelect, AbstractPhysicalObject.AbstractObjectType.NeedleEgg, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Mushroom], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GooieDuck, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.FlyLure, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SlimeMold], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlyLure], tableSelect, AbstractPhysicalObject.AbstractObjectType.BubbleGrass, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb], tableSelect, null, CreatureTemplate.Type.Snail);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, null, CreatureTemplate.Type.Hazer);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, null, CreatureTemplate.Type.Snail);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, MoreSlugcatsEnums.AbstractObjectType.LillyPuck, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        key = AbstractPhysicalObject.AbstractObjectType.Lantern;
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Lantern], tableSelect, null, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.KarmaFlower], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Mushroom], tableSelect, AbstractPhysicalObject.AbstractObjectType.PuffBall, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.ScavengerBomb, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SlimeMold], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlyLure], tableSelect, AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.NeedleEgg, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, null, CreatureTemplate.Type.VultureGrub);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GlowWeed, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.ScavengerBomb, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, null, CreatureTemplate.Type.VultureGrub);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, MoreSlugcatsEnums.AbstractObjectType.LillyPuck, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GlowWeed, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, MoreSlugcatsEnums.AbstractObjectType.LillyPuck, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GlowWeed, null);
        key = AbstractPhysicalObject.AbstractObjectType.KarmaFlower;
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.KarmaFlower], tableSelect, null, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Mushroom], tableSelect, AbstractPhysicalObject.AbstractObjectType.NeedleEgg, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.ScavengerBomb, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SlimeMold], tableSelect, MoreSlugcatsEnums.AbstractObjectType.Seed, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlyLure], tableSelect, MoreSlugcatsEnums.AbstractObjectType.Seed, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.ScavengerBomb, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.NeedleEgg, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.OverseerCarcass, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, MoreSlugcatsEnums.AbstractObjectType.Seed, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.SSOracleSwarmer, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, MoreSlugcatsEnums.AbstractObjectType.SingularityBomb, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, AbstractPhysicalObject.AbstractObjectType.OverseerCarcass, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, MoreSlugcatsEnums.AbstractObjectType.Seed, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, MoreSlugcatsEnums.AbstractObjectType.Seed, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, MoreSlugcatsEnums.AbstractObjectType.Seed, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, MoreSlugcatsEnums.AbstractObjectType.Seed, null);
        key = AbstractPhysicalObject.AbstractObjectType.Mushroom;
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Mushroom], tableSelect, null, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant], tableSelect, null, CreatureTemplate.Type.SmallCentipede);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SlimeMold], tableSelect, null, CreatureTemplate.Type.SmallNeedleWorm);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlyLure], tableSelect, null, CreatureTemplate.Type.Fly);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.SporePlant, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GooieDuck, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GooieDuck, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, null, CreatureTemplate.Type.SmallCentipede);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, null, CreatureTemplate.Type.Hazer);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, null, CreatureTemplate.Type.SmallCentipede);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.ScavengerBomb, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, null, CreatureTemplate.Type.SmallCentipede);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.PuffBall, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GooieDuck, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GooieDuck, null);                       // MoreSlugcatsEnums.AbstractObjectType.GooieDuck
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GooieDuck, null);
        key = AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant;
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant], tableSelect, null, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SlimeMold], tableSelect, AbstractPhysicalObject.AbstractObjectType.Lantern, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlyLure], tableSelect, AbstractPhysicalObject.AbstractObjectType.SporePlant, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.Lantern, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.ScavengerBomb, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.SporePlant, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, null, CreatureTemplate.Type.SmallCentipede);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, null, CreatureTemplate.Type.Snail);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.ScavengerBomb, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, null, CreatureTemplate.Type.SmallCentipede);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.SporePlant, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.SporePlant, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, AbstractPhysicalObject.AbstractObjectType.SporePlant, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.SporePlant, null);
        key = AbstractPhysicalObject.AbstractObjectType.SlimeMold;
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SlimeMold], tableSelect, null, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlyLure], tableSelect, AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.Lantern, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, null, CreatureTemplate.Type.SmallCentipede);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        key = AbstractPhysicalObject.AbstractObjectType.FlyLure;
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlyLure], tableSelect, null, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.PuffBall, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GooieDuck, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, null, CreatureTemplate.Type.Fly);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.ScavengerBomb, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, null, CreatureTemplate.Type.Fly);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, null, CreatureTemplate.Type.Hazer);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, null, CreatureTemplate.Type.SmallCentipede);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, null, CreatureTemplate.Type.Fly);
        key = AbstractPhysicalObject.AbstractObjectType.ScavengerBomb;
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb], tableSelect, null, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.PuffBall, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, null, CreatureTemplate.Type.SmallCentipede);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, AbstractPhysicalObject.AbstractObjectType.PuffBall, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, MoreSlugcatsEnums.AbstractObjectType.SingularityBomb, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, null, CreatureTemplate.Type.SmallCentipede);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.PuffBall, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.SporePlant, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, AbstractPhysicalObject.AbstractObjectType.Lantern, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.PuffBall, null);

        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Spear], tableSelect, AbstractPhysicalObject.AbstractObjectType.Spear, null);
        key = AbstractPhysicalObject.AbstractObjectType.SporePlant;
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, null, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.PuffBall, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, null, CreatureTemplate.Type.TubeWorm);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, AbstractPhysicalObject.AbstractObjectType.PuffBall, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.ScavengerBomb, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, null, CreatureTemplate.Type.TubeWorm);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.PuffBall, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.JellyFish, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, AbstractPhysicalObject.AbstractObjectType.JellyFish, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        key = AbstractPhysicalObject.AbstractObjectType.EggBugEgg;
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, null, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, null, CreatureTemplate.Type.SmallNeedleWorm);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        key = AbstractPhysicalObject.AbstractObjectType.NeedleEgg;
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, null, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, null, CreatureTemplate.Type.Hazer);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, null, CreatureTemplate.Type.VultureGrub);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, null, CreatureTemplate.Type.VultureGrub);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, null, CreatureTemplate.Type.SmallNeedleWorm);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, null, CreatureTemplate.Type.TubeWorm);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, null, CreatureTemplate.Type.Hazer);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, null, CreatureTemplate.Type.Hazer);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, null, CreatureTemplate.Type.SmallNeedleWorm);
        key = AbstractPhysicalObject.AbstractObjectType.BubbleGrass;
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, null, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.ScavengerBomb, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, null, CreatureTemplate.Type.Hazer);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.PuffBall, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.JellyFish, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, MoreSlugcatsEnums.AbstractObjectType.LillyPuck, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GlowWeed, null);
        key = AbstractPhysicalObject.AbstractObjectType.OverseerCarcass;
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, null, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        key = DLCSharedEnums.AbstractObjectType.SingularityBomb;
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, null, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.KarmaFlower, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        key = MoreSlugcatsEnums.AbstractObjectType.FireEgg;
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, null, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        key = DLCSharedEnums.AbstractObjectType.Seed;
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, null, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        key = DLCSharedEnums.AbstractObjectType.GooieDuck;
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, null, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        key = DLCSharedEnums.AbstractObjectType.LillyPuck;
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, null, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        key = DLCSharedEnums.AbstractObjectType.GlowWeed;
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[DLCSharedEnums.AbstractObjectType.GlowWeed], tableSelect, null, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Spear], tableSelect, Scientist.Enums.Items.InflatableGlowingShield, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[Scientist.Enums.Items.SharpSpear], tableSelect, Scientist.Enums.Items.InflatableGlowingShield, null);
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[Scientist.Enums.Items._WaterNut], tableSelect, Scientist.Enums.Items.AirBag, null);
        key = DLCSharedEnums.AbstractObjectType.DandelionPeach;
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, null, null);

        key = Enums.Items.Knot;
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[key], ScientistSlugcat.objectsLibrary[Enums.Items.Knot], tableSelect, Enums.Items.Knot, null);

        tableSelect = 1;
        CreatureTemplate.Type key2 = CreatureTemplate.Type.Fly;
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Rock], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlareBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.VultureMask], tableSelect, MoreSlugcatsEnums.AbstractObjectType.Seed, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.PuffBall], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DangleFruit], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SSOracleSwarmer], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DataPearl], tableSelect, null, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.WaterNut], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.JellyFish], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Lantern], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.KarmaFlower], tableSelect, AbstractPhysicalObject.AbstractObjectType.FlyLure, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Mushroom], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GooieDuck, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.SporePlant, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SlimeMold], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlyLure], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.FlyLure, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, MoreSlugcatsEnums.AbstractObjectType.Seed, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, AbstractPhysicalObject.AbstractObjectType.FlyLure, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        key2 = CreatureTemplate.Type.SmallCentipede;
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Rock], tableSelect, AbstractPhysicalObject.AbstractObjectType.SporePlant, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlareBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.SporePlant, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.VultureMask], tableSelect, MoreSlugcatsEnums.AbstractObjectType.Seed, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.PuffBall], tableSelect, AbstractPhysicalObject.AbstractObjectType.SporePlant, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DangleFruit], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SSOracleSwarmer], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DataPearl], tableSelect, null, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.WaterNut], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.JellyFish], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Lantern], tableSelect, AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.KarmaFlower], tableSelect, AbstractPhysicalObject.AbstractObjectType.PuffBall, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Mushroom], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GooieDuck, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.SporePlant, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SlimeMold], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlyLure], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, null, CreatureTemplate.Type.TubeWorm);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, MoreSlugcatsEnums.AbstractObjectType.Seed, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, AbstractPhysicalObject.AbstractObjectType.JellyFish, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        key2 = CreatureTemplate.Type.VultureGrub;
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Rock], tableSelect, AbstractPhysicalObject.AbstractObjectType.Lantern, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlareBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.Lantern, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.VultureMask], tableSelect, MoreSlugcatsEnums.AbstractObjectType.Seed, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.PuffBall], tableSelect, AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DangleFruit], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SSOracleSwarmer], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DataPearl], tableSelect, null, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.WaterNut], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.JellyFish], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Lantern], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.KarmaFlower], tableSelect, AbstractPhysicalObject.AbstractObjectType.VultureMask, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Mushroom], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GooieDuck, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.SporePlant, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SlimeMold], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlyLure], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.FlareBomb, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, MoreSlugcatsEnums.AbstractObjectType.Seed, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GlowWeed, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        key2 = CreatureTemplate.Type.SmallNeedleWorm;
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Rock], tableSelect, AbstractPhysicalObject.AbstractObjectType.PuffBall, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlareBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.PuffBall, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.VultureMask], tableSelect, MoreSlugcatsEnums.AbstractObjectType.Seed, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.PuffBall], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DangleFruit], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SSOracleSwarmer], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DataPearl], tableSelect, null, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.WaterNut], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.JellyFish], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Lantern], tableSelect, AbstractPhysicalObject.AbstractObjectType.NeedleEgg, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.KarmaFlower], tableSelect, AbstractPhysicalObject.AbstractObjectType.NeedleEgg, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Mushroom], tableSelect, AbstractPhysicalObject.AbstractObjectType.PuffBall, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.SporePlant, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SlimeMold], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlyLure], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.PuffBall, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.PuffBall, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, MoreSlugcatsEnums.AbstractObjectType.Seed, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, AbstractPhysicalObject.AbstractObjectType.Mushroom, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        key2 = CreatureTemplate.Type.Hazer;
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Rock], tableSelect, MoreSlugcatsEnums.AbstractObjectType.LillyPuck, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlareBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GlowWeed, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.VultureMask], tableSelect, MoreSlugcatsEnums.AbstractObjectType.Seed, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.PuffBall], tableSelect, AbstractPhysicalObject.AbstractObjectType.BubbleGrass, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DangleFruit], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SSOracleSwarmer], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DataPearl], tableSelect, null, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.WaterNut], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.JellyFish], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Lantern], tableSelect, null, CreatureTemplate.Type.Snail);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.KarmaFlower], tableSelect, AbstractPhysicalObject.AbstractObjectType.BubbleGrass, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Mushroom], tableSelect, AbstractPhysicalObject.AbstractObjectType.BubbleGrass, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.JellyFish, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SlimeMold], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FlyLure], tableSelect, AbstractPhysicalObject.AbstractObjectType.BubbleGrass, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.ScavengerBomb], tableSelect, AbstractPhysicalObject.AbstractObjectType.FlareBomb, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.SporePlant], tableSelect, AbstractPhysicalObject.AbstractObjectType.JellyFish, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.NeedleEgg], tableSelect, MoreSlugcatsEnums.AbstractObjectType.Seed, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.BubbleGrass], tableSelect, MoreSlugcatsEnums.AbstractObjectType.GlowWeed, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.OverseerCarcass], tableSelect, AbstractPhysicalObject.AbstractObjectType.DataPearl, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.SingularityBomb], tableSelect, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.FireEgg], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.Seed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GooieDuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.LillyPuck], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.GlowWeed], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[MoreSlugcatsEnums.AbstractObjectType.DandelionPeach], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        
        key2 = CreatureTemplate.Type.Snail;
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.objectsLibrary[DLCSharedEnums.AbstractObjectType.GlowWeed], tableSelect, Scientist.Enums.Items.TremblingFruit, null);
        /*Custom.Log(new string[]
        {
                "--Setup Creature + Creature table"
        });*/
        tableSelect = 2;
        key2 = CreatureTemplate.Type.Fly;
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.critsLibrary[CreatureTemplate.Type.Fly], tableSelect, null, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.critsLibrary[CreatureTemplate.Type.VultureGrub], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.critsLibrary[CreatureTemplate.Type.SmallCentipede], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.critsLibrary[CreatureTemplate.Type.SmallNeedleWorm], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.critsLibrary[CreatureTemplate.Type.Hazer], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        key2 = CreatureTemplate.Type.VultureGrub;
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.critsLibrary[CreatureTemplate.Type.VultureGrub], tableSelect, null, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.critsLibrary[CreatureTemplate.Type.SmallCentipede], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.critsLibrary[CreatureTemplate.Type.SmallNeedleWorm], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.critsLibrary[CreatureTemplate.Type.Hazer], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        key2 = CreatureTemplate.Type.SmallCentipede;
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.critsLibrary[CreatureTemplate.Type.SmallCentipede], tableSelect, null, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.critsLibrary[CreatureTemplate.Type.SmallNeedleWorm], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.critsLibrary[CreatureTemplate.Type.Hazer], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        key2 = CreatureTemplate.Type.SmallNeedleWorm;
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.critsLibrary[CreatureTemplate.Type.SmallNeedleWorm], tableSelect, null, null);
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.critsLibrary[CreatureTemplate.Type.Hazer], tableSelect, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null);
        key2 = CreatureTemplate.Type.Hazer;
        //ScientistSlugcat.SetLibraryData(ScientistSlugcat.critsLibrary[key2], ScientistSlugcat.critsLibrary[CreatureTemplate.Type.Hazer], tableSelect, null, null);

        /* ==============================================================================================
         *                                    设计表（合成表.xlsx）整表留档
         * ----------------------------------------------------------------------------------------------
         * 材料名 → 代码类型对照（"生物"表示位于 critsLibrary 的生物材料）：
         *   石头         = Rock                  小石头       = SmallRock(本项目)        矛       = Spear
         *   绳子         = Knot(本项目)          蓝果         = DangleFruit              弹球     = 【本项目·未实现】
         *   泡水果o(未泡) = _WaterNut(本项目)     泡水果O(泡开) = WaterNut                 迷幻蘑   = Mushroom
         *   果蝠         = Fly(生物)             蝉乌贼       = CicadaA(生物)            蜂巢     = SporePlant
         *   鞭炮草       = FirecrackerPlant       炸弹         = ScavengerBomb            秃鹫面具 = VultureMask
         *   蛋虫卵       = EggBugEgg             黏菌         = SlimeMold                水母     = JellyFish
         *   气泡草       = BubbleGrass           发光果       = GlowWeed(发光草)          闪光果   = FlareBomb(闪光弹)
         *   业力花       = KarmaFlower           信号虫       = VultureGrub(生物·待确认)   珍珠     = DataPearl
         *   百合花灯     = LillyPuck             管虫         = TubeWorm(生物)           悬丝霉   = 【含义待确认】
         * 状态标记：
         *   【已启用】  代码中已有实际配方（上方 object 表 / 本节 / GetSpecialCraftingResult）
         *   【待实现】  材料或产物在代码中尚不存在，先按设计表留档
         *   【特殊合成】走 ScientistSlugcat.GetSpecialCraftingResult，而非合成表
         *   【原版机制】由原版 Gourmand 式 meal 流程处理（SpitUpCraftedObject 的 DangleFruit 分支）
         * 表中共 26 行材料；下列按设计表的材料顺序逐行留档，已启用的配方保留实际调用。
         * ============================================================================================== */

        // ------------------------------------------------------------ 石头 ----
        // 石头 + 石头   → 小石头     【已启用】object 表 + 特殊合成（产出 5 个）
        // 石头 + 小石头 → 切割石刀   【已启用】
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.Rock], ScientistSlugcat.objectsLibrary[Scientist.Enums.Items.SmallRock], 0, Scientist.Enums.Items.StoneKnife, null);
        // 石头 + 矛     → 尖矛       【已启用】object 表 + 特殊合成
        // 石头 + 绳子   → 飞石索     【待实现：飞石索】
        // 石头 + 鞭炮草 → 爆炸粉     【已启用】object 表
        // 石头 + 炸弹   → 爆破石     【已启用】object 表
        // 石头 + 业力花 → 圣石       【待实现：圣石】

        // ------------------------------------------------------------ 小石头 ----
        // 小石头 + 石头   → 切割石刀 【已启用】见上方"石头"行
        // 小石头 + 小石头 → 磨刀石   【待实现：磨刀石】
        // 小石头 + 珍珠   → 透镜     【待实现：透镜】

        // ------------------------------------------------------------ 矛 ----
        // 矛 + 石头     → 尖矛       【已启用】见上方"石头"行
        // 矛 + 绳子     → 绳矛       【待实现：绳矛】
        // 矛 + 炸弹     → 炸矛       【特殊合成】AbstractSpear(bugSpear = true)
        // 矛 + 发光果   → 胀光盾牌   【已启用】object 表（GlowWeed + Spear / SharpSpear）
        // 矛 + 百合花灯 → 吹箭       【待实现：吹箭】

        // ------------------------------------------------------------ 绳子 ----
        // 绳子 + 石头     → 飞石索   【待实现：飞石索】
        // 绳子 + 矛       → 绳矛     【待实现：绳矛】
        // 绳子 + 绳子     → 网       【待实现：网】
        // 绳子 + 果蝠     → 绑果蝠   【待实现：绑果蝠】
        // 绳子 + 蝉乌贼   → 绑乌贼   【待实现：绑乌贼】
        // 绳子 + 蜂巢     → 飞蜂索巢 【待实现：飞蜂索巢】
        // 绳子 + 秃鹫面具 → 绑面具   【待实现：绑面具】
        // 绳子 + 水母     → 绑水母   【待实现：绑水母】
        // 绳子 + 管虫     → 双向管虫 【待实现：双向管虫】
        // 注意：设计表中没有"绳结"，Knot + Knot → Knot 是本项目额外保留的（合并绳长）

        // ------------------------------------------------------------ 蓝果 ----
        // 蓝果 + 蓝果   → 浓缩蓝果   【已启用】
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DangleFruit], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.DangleFruit], 0, Scientist.Enums.Items.ConcentratedDangleFruit, null);
        // 蓝果 + 迷幻蘑 → 无痛果     【已启用】object 表
        // 蓝果 + 蛋虫卵 → 蓝果卵     【待实现：蓝果卵】
        // 蓝果 + 黏菌   → 素食大餐   【原版机制】
        // 蓝果 + 水母   → 电果       【待实现：电果】
        // 蓝果 + 发光果 → 素食大餐   【原版机制】

        // ------------------------------------------------------------ 泡水果o（未泡开） ----
        // 泡水果o + 泡水果O → 未胀弹球 【待实现：未胀弹球】
        // 泡水果o + 炸弹    → 水爆弹   【待实现：水爆弹】

        // ------------------------------------------------------------ 泡水果O（泡开） ----
        // 泡水果O + 矛       → 滑矛     【待实现：滑矛】
        // 泡水果O + 鞭炮草   → 膨胀炸弹 【已启用】object 表
        // 泡水果O + 蛋虫卵   → 素食大餐 【原版机制】
        // 泡水果O + 黏菌     → 素食大餐 【原版机制】
        // 泡水果O + 气泡草   → 呼吸泡   【已启用】object 表
        // 泡水果O + 发光果   → 孔明灯   【已启用】object 表
        // 泡水果O + 业力花   → 储存业力 【待实现：储存业力】
        // 泡水果O + 百合花灯 → 气压塞子 【待实现：气压塞子】
        // 泡水果O + 管虫     → 抓捕器   【待实现：抓捕器】

        // ------------------------------------------------------------ 弹球（本项目·未实现） ----
        // 弹球 + 迷幻蘑 → 迷幻球   【待实现：弹球、迷幻球】
        // 弹球 + 蜂巢   → 蜂球     【待实现：弹球、蜂球】
        // 弹球 + 炸弹   → 弹爆球   【待实现：弹球、弹爆球】
        // 弹球 + 水母   → 电击球   【待实现：弹球、电击球】
        // 弹球 + 业力花 → 慈悲弹球 【待实现：弹球、慈悲弹球】
        // 弹球 + 珍珠   → 逗猴球   【待实现：弹球、逗猴球】
        // 弹球 + 管虫   → 啊打！   【待实现：弹球、啊打】

        // ------------------------------------------------------------ 果蝠（Fly，生物） ----
        // 果蝠 + 绳子   → 绑果蝠   【待实现：绑果蝠】
        // 果蝠 + 蛋虫卵 → 素食大餐 【原版机制】
        // 果蝠 + 黏菌   → 素食大餐 【原版机制】
        // 果蝠 + 发光果 → 素食大餐 【原版机制】

        // ------------------------------------------------------------ 蝉乌贼（CicadaA，生物） ----
        // 蝉乌贼 + 绳子     → 绑乌贼   【待实现：绑乌贼】
        // 蝉乌贼 + 秃鹫面具 → 面具乌贼 【待实现：面具乌贼】

        // ------------------------------------------------------------ 迷幻蘑 ----
        // 迷幻蘑 + 蓝果   → 无痛果     【已启用】见上方"蓝果"行
        // 迷幻蘑 + 弹球   → 迷幻球     【待实现：弹球、迷幻球】
        // 迷幻蘑 + 闪光果 → 彩光果     【已启用】object 表（FlareBomb + Mushroom）
        // 迷幻蘑 + 业力花 → 飞升快感   【待实现：飞升快感】
        // 迷幻蘑 + 管虫   → 喷吐管虫   【待实现：喷吐管虫】

        // ------------------------------------------------------------ 蜂巢 ----
        // 蜂巢 + 绳子   → 飞蜂索巢   【待实现：飞蜂索巢】
        // 蜂巢 + 弹球   → 蜂球       【待实现：弹球、蜂球】
        // 蜂巢 + 炸弹   → 爆破蜂巢   【待实现：爆破蜂巢】
        // 蜂巢 + 闪光果 → 荧光蜂巢   【待实现：荧光蜂巢】
        // 蜂巢 + 业力花 → 蜂香       【待实现：蜂香】
        // 蜂巢 + 信号虫 → 合作蜂巢   【待实现：合作蜂巢】
        // 蜂巢 + 管虫   → 喷网       【待实现：喷网】

        // ------------------------------------------------------------ 鞭炮草 ----
        // 鞭炮草 + 石头   → 爆炸粉   【已启用】见上方"石头"行
        // 鞭炮草 + 泡水果O → 膨胀炸弹 【已启用】见上方"泡水果O"行
        // 鞭炮草 + 炸弹   → 空爆弹   【待实现：空爆弹】
        // 鞭炮草 + 蛋虫卵 → 火虫卵   【已启用】
        ScientistSlugcat.SetLibraryData(ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant], ScientistSlugcat.objectsLibrary[AbstractPhysicalObject.AbstractObjectType.EggBugEgg], 0, MoreSlugcatsEnums.AbstractObjectType.FireEgg, null);
        // 鞭炮草 + 闪光果 → 爆闪草   【待实现：爆闪草】
        // 鞭炮草 + 百合花灯 → 烟花   【待实现：烟花】

        // ------------------------------------------------------------ 炸弹 ----
        // 炸弹 + 石头   → 爆破石   【已启用】见上方"石头"行
        // 炸弹 + 矛     → 炸矛     【特殊合成】见上方"矛"行
        // 炸弹 + 泡水果o → 水爆弹  【待实现：水爆弹】
        // 炸弹 + 弹球   → 弹爆球   【待实现：弹球、弹爆球】
        // 炸弹 + 蜂巢   → 爆破蜂巢 【待实现：爆破蜂巢】
        // 炸弹 + 鞭炮草 → 空爆弹   【待实现：空爆弹】
        // 炸弹 + 珍珠   → 诱惑弹   【待实现：诱惑弹】

        // ------------------------------------------------------------ 秃鹫面具 ----
        // 秃鹫面具 + 绳子   → 绑面具   【待实现：绑面具】
        // 秃鹫面具 + 蝉乌贼 → 面具乌贼 【待实现：面具乌贼】

        // ------------------------------------------------------------ 蛋虫卵 ----
        // 蛋虫卵 + 蓝果     → 蓝果卵   【待实现：蓝果卵】
        // 蛋虫卵 + 泡水果O  → 素食大餐 【原版机制】
        // 蛋虫卵 + 果蝠     → 素食大餐 【原版机制】
        // 蛋虫卵 + 鞭炮草   → 火虫卵   【已启用】见上方"鞭炮草"行
        // 蛋虫卵 + 黏菌     → 素食大餐 【原版机制】
        // 蛋虫卵 + 发光果   → 素食大餐 【原版机制】

        // ------------------------------------------------------------ 黏菌 ----
        // 黏菌 + 蓝果     → 素食大餐 【原版机制】
        // 黏菌 + 泡水果O  → 素食大餐 【原版机制】
        // 黏菌 + 果蝠     → 素食大餐 【原版机制】
        // 黏菌 + 蛋虫卵   → 素食大餐 【原版机制】
        // 黏菌 + 发光果   → 素食大餐 【原版机制】
        // 黏菌 + 管虫     → 大管虫   【待实现：大管虫】

        // ------------------------------------------------------------ 水母 ----
        // 水母 + 绳子   → 绑水母   【待实现：绑水母】
        // 水母 + 蓝果   → 电果     【待实现：电果】
        // 水母 + 弹球   → 电击球   【待实现：弹球、电击球】
        // 水母 + 珍珠   → 晶水母   【待实现：晶水母】
        // 水母 + 管虫   → 电舌     【待实现：电舌】

        // ------------------------------------------------------------ 气泡草 ----
        // 气泡草 + 泡水果O → 呼吸泡   【已启用】见上方"泡水果O"行
        // 气泡草 + 业力花 → 神圣气盾 【待实现：神圣气盾】

        // ------------------------------------------------------------ 发光果（GlowWeed） ----
        // 发光果 + 矛       → 胀光盾牌 【已启用】见上方"矛"行
        // 发光果 + 蓝果     → 素食大餐 【原版机制】
        // 发光果 + 泡水果O  → 孔明灯   【已启用】见上方"泡水果O"行
        // 发光果 + 果蝠     → 素食大餐 【原版机制】
        // 发光果 + 蛋虫卵   → 素食大餐 【原版机制】
        // 发光果 + 黏菌     → 素食大餐 【原版机制】
        // 发光果 + 信号虫   → 警告果   【待实现：警告果】
        // 发光果 + 管虫     → 撞弹     【待实现：撞弹】

        // ------------------------------------------------------------ 闪光果（FlareBomb） ----
        // 闪光果 + 迷幻蘑 → 彩光果   【已启用】见上方"迷幻蘑"行
        // 闪光果 + 蜂巢   → 荧光蜂巢 【待实现：荧光蜂巢】
        // 闪光果 + 鞭炮草 → 爆闪草   【待实现：爆闪草】
        // 闪光果 + 闪光果 → 强光果   【待实现：强光果】
        // 闪光果 + 信号虫 → 标记果   【待实现：标记果】
        // 闪光果 + 珍珠   → 夜明珠   【待实现：夜明珠】

        // ------------------------------------------------------------ 业力花 ----
        // 业力花 + 石头     → 圣石     【待实现：圣石】
        // 业力花 + 泡水果O  → 储存业力 【待实现：储存业力】
        // 业力花 + 弹球     → 慈悲弹球 【待实现：弹球、慈悲弹球】
        // 业力花 + 迷幻蘑   → 飞升快感 【待实现：飞升快感】
        // 业力花 + 炸弹     → 神圣手雷 【待实现：神圣手雷】
        // 业力花 + 气泡草   → 神圣气盾 【待实现：神圣气盾】
        // 业力花 + 珍珠     → 佛珠     【待实现：佛珠】

        // ------------------------------------------------------------ 信号虫（VultureGrub，生物） ----
        // 信号虫 + 蜂巢   → 合作蜂巢 【待实现：合作蜂巢】
        // 信号虫 + 发光果 → 警告果   【待实现：警告果】
        // 信号虫 + 闪光果 → 标记果   【待实现：标记果】
        // 信号虫 + 珍珠   → 信号灯   【待实现：信号灯】

        // ------------------------------------------------------------ 珍珠 ----
        // 珍珠 + 小石头   → 透镜     【待实现：透镜】
        // 珍珠 + 弹球     → 逗猴球   【待实现：弹球、逗猴球】
        // 珍珠 + 炸弹     → 诱惑弹   【待实现：诱惑弹】
        // 珍珠 + 水母     → 晶水母   【待实现：晶水母】
        // 珍珠 + 闪光果   → 夜明珠   【待实现：夜明珠】
        // 珍珠 + 业力花   → 佛珠     【待实现：佛珠】
        // 珍珠 + 信号虫   → 信号灯   【待实现：信号灯】

        // ------------------------------------------------------------ 百合花灯 ----
        // 百合花灯 + 矛       → 吹箭     【待实现：吹箭】
        // 百合花灯 + 泡水果O  → 气压塞子 【待实现：气压塞子】
        // 百合花灯 + 鞭炮草   → 烟花     【待实现：烟花】
        // 百合花灯 + 管虫     → 管矛     【待实现：管矛】

        // ------------------------------------------------------------ 管虫（TubeWorm，生物） ----
        // 管虫 + 绳子     → 双向管虫 【待实现：双向管虫】
        // 管虫 + 泡水果O  → 抓捕器   【待实现：抓捕器】
        // 管虫 + 弹球     → 啊打！   【待实现：弹球、啊打】
        // 管虫 + 迷幻蘑   → 喷吐管虫 【待实现：喷吐管虫】
        // 管虫 + 蜂巢     → 喷网     【待实现：喷网】
        // 管虫 + 黏菌     → 大管虫   【待实现：大管虫】
        // 管虫 + 发光果   → 撞弹     【待实现：撞弹】
        // 管虫 + 百合花灯 → 管矛     【待实现：管矛】

        // ------------------------------------------------------------ 悬丝霉 ----
        // 设计表中只有材料名，没有对应任何配方（含义与用途待确认）

        // ------------------------------------------------------------ 高级合成（设计表 sheet2） ----
        // 绳子 + 绳子 + 水母 + 矛        → 电棍     【待实现：三方合成；ElectricProd 目前未注册进 Enums.Items】
        // 麻醉矛 + 麻醉气囊 + 珍珠       → 基础针剑 【待实现：三方合成、麻醉气囊】

        // ------------------------------------------------------------ 代码额外保留（设计表中没有） ----
        // Knot + Knot                    → Knot   【特殊合成】（合并绳长）
        // Spear + Spear                  → Spear  【object 表】
        // GlowWeed + _WaterNut           → AirBag 【object 表】
        // GlowWeed + SharpSpear          → InflatableGlowingShield 【object 表】
        // Snail + GlowWeed               → TremblingFruit 【生物×物品表】（保留蜗牛壳色）
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
            return null;
            //return new AbstractPhysicalObject(crafter.room.world, AbstractPhysicalObject.AbstractObjectType.Rock, null, crafter.abstractPhysicalObject.pos, crafter.room.game.GetNewID());
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
        if (abstractObjectType == DLCSharedEnums.AbstractObjectType.LillyPuck) //百合花灯
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

    public static bool APO_Compare(CreatureTemplate.Type grasp1, CreatureTemplate.Type grasp2, CreatureTemplate.Type item1, CreatureTemplate.Type item2)
    {
        return ((grasp1 == item1 && grasp2 == item2) || (grasp1 == item2 && grasp2 == item1));
    }

    public static bool APO_Compare(AbstractPhysicalObject.AbstractObjectType grasp1, AbstractPhysicalObject.AbstractObjectType grasp2, AbstractPhysicalObject.AbstractObjectType item1, AbstractPhysicalObject.AbstractObjectType item2)
    {
        return ((grasp1 == item1 && grasp2 == item2) || (grasp1 == item2 && grasp2 == item1));
    }

    public static AbstractPhysicalObject[] GetSpecialCraftingResult(Player player)
    {
        if (player.grasps[0] == null || player.grasps[1] == null || player.grasps[0].grabbed == null || player.grasps[1].grabbed == null) { return null; }
        if (player.grasps[0].grabbed is Creature && player.grasps[1].grabbed is Creature)
        {
            return GetSpecialCraftingResult((player.grasps[0].grabbed.abstractPhysicalObject as AbstractCreature).creatureTemplate.type, (player.grasps[1].grabbed.abstractPhysicalObject as AbstractCreature).creatureTemplate.type, player);
        }
        else if ((player.grasps[0]?.grabbed is Creature && player.grasps[1].grabbed is PhysicalObject) || (player.grasps[0].grabbed is PhysicalObject && player.grasps[1].grabbed is Creature))
        {
            return GetSpecialCraftingResult(player.grasps[0].grabbed is Creature ? (player.grasps[0].grabbed.abstractPhysicalObject as AbstractCreature).creatureTemplate.type : (player.grasps[1].grabbed.abstractPhysicalObject as AbstractCreature).creatureTemplate.type, player.grasps[0].grabbed is Creature ? player.grasps[1].grabbed.abstractPhysicalObject.type : player.grasps[0].grabbed.abstractPhysicalObject.type, player.grasps[0].grabbed is Creature ? 0 : 1, player.grasps[0].grabbed is Creature ? 1 : 0, player);
        }
        else if (player.grasps[0].grabbed is PhysicalObject && player.grasps[1].grabbed is PhysicalObject)
        {
            return GetSpecialCraftingResult(player.grasps[0].grabbed.abstractPhysicalObject.type, player.grasps[1].grabbed.abstractPhysicalObject.type, player);
        }
        return null;
    }

    public static AbstractPhysicalObject[] GetSpecialCraftingResult(CreatureTemplate.Type a, CreatureTemplate.Type b, Player player)
    {
        return null;
    }

    public static AbstractPhysicalObject[] GetSpecialCraftingResult(CreatureTemplate.Type a, AbstractPhysicalObject.AbstractObjectType b, int ci, int poi, Player player)
    {
        if (a == CreatureTemplate.Type.Snail && b == DLCSharedEnums.AbstractObjectType.GlowWeed)
        {
            Snail snail = player.grasps[ci].grabbed as Snail;
            return new AbstractPhysicalObject[] { new Scientist.Items.AbstractPhysicalObjects.TremblingFruitAbstract(player.room.world, null, player.abstractCreature.pos, snail.abstractCreature.ID, snail.shellColor) };
        }
        return null;
    }

    public static AbstractPhysicalObject[] GetSpecialCraftingResult(AbstractPhysicalObject.AbstractObjectType a, AbstractPhysicalObject.AbstractObjectType b, Player player)
    {
        if (APO_Compare(a, b, AbstractPhysicalObject.AbstractObjectType.Spear, AbstractPhysicalObject.AbstractObjectType.Rock))
        {
            return new AbstractPhysicalObject[] { new Scientist.Items.AbstractPhysicalObjects.SharpSpearAbstract(player.room.world, null, player.abstractCreature.pos, player.room.game.GetNewID()) }; //自定义的ShapeSpearAbstract，覆写了realizedObject
        }
        if (APO_Compare(a, b, AbstractPhysicalObject.AbstractObjectType.Spear, AbstractPhysicalObject.AbstractObjectType.ScavengerBomb))
        {
            return new AbstractPhysicalObject[] { new AbstractSpear(player.room.world, null, player.abstractCreature.pos, player.room.game.GetNewID(), true) };
        }
        if (APO_Compare(a, b, AbstractPhysicalObject.AbstractObjectType.WaterNut, AbstractPhysicalObject.AbstractObjectType.WaterNut))
        {
            if (player.grasps[0].grabbed is WaterNut && player.grasps[1].grabbed is WaterNut)
            {
                return null;
            }
            if ( (player.grasps[0].grabbed is WaterNut && player.grasps[1].grabbed is SwollenWaterNut) || (player.grasps[0].grabbed is SwollenWaterNut && player.grasps[1].grabbed is WaterNut))
            {
                return new AbstractPhysicalObject[] { new Scientist.Items.AbstractPhysicalObjects.SharpSpearAbstract(player.room.world, null, player.abstractCreature.pos, player.room.game.GetNewID()) };
            }
            if (player.grasps[0].grabbed is SwollenWaterNut && player.grasps[1].grabbed is SwollenWaterNut)
            {
                return null;
            }
        }
        if (APO_Compare(a, b, AbstractPhysicalObject.AbstractObjectType.Rock, AbstractPhysicalObject.AbstractObjectType.Rock))
        {
            return new AbstractPhysicalObject[] {
                new Scientist.Items.AbstractPhysicalObjects.SmallRockAbstract(player.room.world, Enums.Items.SmallRock, null, player.abstractCreature.pos, player.room.game.GetNewID()),
                new Scientist.Items.AbstractPhysicalObjects.SmallRockAbstract(player.room.world, Enums.Items.SmallRock, null, player.abstractCreature.pos, player.room.game.GetNewID()),
                new Scientist.Items.AbstractPhysicalObjects.SmallRockAbstract(player.room.world, Enums.Items.SmallRock, null, player.abstractCreature.pos, player.room.game.GetNewID()),
                new Scientist.Items.AbstractPhysicalObjects.SmallRockAbstract(player.room.world, Enums.Items.SmallRock, null, player.abstractCreature.pos, player.room.game.GetNewID()),
                new Scientist.Items.AbstractPhysicalObjects.SmallRockAbstract(player.room.world, Enums.Items.SmallRock, null, player.abstractCreature.pos, player.room.game.GetNewID())
            };
        }
        if (APO_Compare(a, b, Enums.Items.Knot, Enums.Items.Knot))
        {
            if (!(player.grasps[0].grabbed as Items.Knot).knotAbstract.ss.Empty() && !(player.grasps[1].grabbed as Items.Knot).knotAbstract.ss.Empty())
            {
                Scientist.Items.AbstractPhysicalObjects.KnotAbstract kaNew = new(player.room.world, null, player.abstractCreature.pos, player.room.game.GetNewID());
                Items.StringShort[] ss = (player.grasps[0].grabbed as Items.Knot).knotAbstract.ss.ToArray();
                foreach (Items.StringShort s in ss)
                {
                    s.ChangeKnot((player.grasps[0].grabbed as Items.Knot).knotAbstract, kaNew);
                }
                ss = (player.grasps[1].grabbed as Items.Knot).knotAbstract.ss.ToArray();
                foreach (Items.StringShort s in ss)
                {
                    s.ChangeKnot((player.grasps[1].grabbed as Items.Knot).knotAbstract, kaNew);
                }
                (player.grasps[0].grabbed as Items.Knot).knotAbstract.ss.Clear();
                (player.grasps[1].grabbed as Items.Knot).knotAbstract.ss.Clear();
                return new AbstractPhysicalObject[] { kaNew };
            }
            else
            {
                Items.StringShort stringshort = new(player.room, 20.0f, (player.grasps[0].grabbed as Items.Knot).knotAbstract, (player.grasps[1].grabbed as Items.Knot).knotAbstract);
                (player.grasps[0].grabbed as Items.Knot).knotAbstract.ss.Add(stringshort);
                (player.grasps[1].grabbed as Items.Knot).knotAbstract.ss.Add(stringshort);
                player.room.AddObject(stringshort);
                return null;
            }
        }
        return null;
    }
}
