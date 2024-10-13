using System;
using BepInEx;
using SlugBase.Features;
using static SlugBase.Features.FeatureTypes;
using items;
using RWCustom;
using Expedition;
using MoreSlugcats;
using UnityEngine;
using items.AbstractPhysicalObjects;
using chats;
using SlugBase.DataTypes;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using MonoMod.RuntimeDetour;
using System.Reflection;
using BeastMaster;
using static BeastMaster.BeastMaster;
using UnityEditor.VersionControl;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using System.Text;
using System.Reflection.Emit;

// TODO 属性比起黄猫还要略低
// TODO 随身携带一只拾荒者精英保镖 已废除
// TODO 比起普通的阔鱼猫有着更多的手指
// TODO 可以骑乘在拾荒者上以操控
// TODO 不可以吃肉 OK!
// TODO 能从特定生物的尸体或物品中获得素材和灵感，每触摸到一个新的物品就可以获得相关敏感和制造物品
// TODO 初始自带三个制造：尖矛、绳矛、尖绳矛


namespace Scientist;

[BepInPlugin(MOD_ID, "Scientist", "0.1.0")]
class Plugin : BaseUnityPlugin
{
    public const string MOD_ID = "XuYangJerry.Scientist";

    public static readonly PlayerFeature<bool> IsScientist = PlayerBool("scientist/is_scientist");
    public static readonly PlayerFeature<bool> Power = PlayerBool("temp/power");
    public static readonly PlayerFeature<float> LowerJump = PlayerFloat("scientist/lower_jump");
    public static readonly PlayerFeature<bool> CanPickupShapespearStuckinwall = PlayerBool("scientist/can_pickup_shapespear_stuckinwall");

    public static readonly PlayerColor HatColor = new PlayerColor("Hat");
    public static readonly PlayerColor GlassesColor = new PlayerColor("Glasses");

    public static HatOnHead hat = new HatOnHead("atlases/slugcat/scientist_hat", "scientist_hat-", HatColor, IsScientist);
    public static HatOnHead glasses = new HatOnHead("atlases/slugcat/scientist_glasses", "scientist_glasses-", GlassesColor, IsScientist, new List<HatOnHead>() { Plugin.hat });

    public static string tempAnimation = "";

    private items.ShowCraftingResult scr;

    private static Dictionary<string, bool> modHooked = new Dictionary<string, bool>();

    public Plugin()
    {
            
    }

    public void Awake()
    {
        //每次加载mod时，您的对象类型都会被注册
        On.RainWorld.OnModsEnabled += RainWorld_OnModsEnabled;

        //在此方法中，您将在禁用mod时注销对象类型
        On.RainWorld.OnModsDisabled += RainWorld_OnModsDisabled;

        //此方法将允许您使用给定的抽象对象实现您的对象。如果你有自己的抽象对象类，可以在这个类中重写它
        On.AbstractPhysicalObject.Realize += AbstractPhysicalObject_Realize;

        //如果你的物品是PlayerCarryableItem，此方法确定slugcat将抓取你的物品的爪子数量
        On.Player.Grabability += Player_Grabability;
    }

    // Add hooks-添加钩子
    public void OnEnable()
    {
        /*Content.Register(new IContent[]
        {
            new items.ShapeSpears.ShapeSpearFisob()
        });*/
        Register.RegisterValues(); //注册，钩子有时候出问题

        // Load any resources, such as sprites or sounds-加载任何资源 包括图像素材和音效
        On.RainWorld.OnModsInit += Extras.WrapInit(LoadResources);
        On.RainWorld.PostModsInit += RainWorld_PostModsInit;

        //On.Room.AddObject += RoomAddObject;
        On.AbstractRoom.AddEntity += AbstractRoomAddEntity;

        hat.Hook();
        glasses.Hook();

        // Put your custom hooks here!-在此放置你自己的钩子
        On.Player.Jump += PlayerJump;
        On.Player.Die += Player_Die;
        On.Player.Stun += Player_Stun;
        On.Player.CanIPickThisUp += PlayerCanIPickThisUp;
        On.Player.Update += PlayerUpdate;
        On.Player.GrabUpdate += PlayerGrabUpdate_On;    //在玩家触发拾取时执行PlayerGrabUpdate
        IL.Player.GrabUpdate += PlayerGrabUpdate_IL;    //兼容原版，在玩家触发拾取时执行PlayerGrabUpdate_IL
        On.Player.ThrowObject += PlayerThrowObject;
        On.Player.ThrownSpear += PlayerThrownSpear;

        //On.PlayerGraphics.Update += PlayerGraphics_Update;

        On.GraphicsModule.Update += GraphicsModule_Update;
        On.GraphicsModule.DrawSprites += GraphicsModule_DrawSprites;

        //On.Lizard.ctor += Lizard_ctor;

        //On.SSOracleBehavior.InitateConversation += chats.FivePebblesChats.FivePebbles_InitateConversationInitiateConversation;
        //On.SSOracleBehavior.Update += chats.FivePebblesChats.FivePebbles_Update;
        //On.SSOracleBehavior.PebblesConversation.AddEvents += chats.FivePebblesChats.FivePebbles_AddEvents;
        On.SSOracleBehavior.SeePlayer += chats.FivePebblesChats.FivePebbles_SeePlayer;
        On.SSOracleBehavior.NewAction += chats.FivePebblesChats.FivePebbles_NewAction;

        On.ItemSymbol.SpriteNameForItem += ItemSymbolSpriteNameForItem;
        On.ItemSymbol.ColorForItem += ItemSymbolColorForItem;

        On.AbstractConsumable.IsTypeConsumable += AbstractConsumableIsTypeConsumable;

        //On.SaveState.ApplyCustomEndGame += SaveState_ApplyCustomEndGame;
        //On.SaveState.LoadGame += SaveState_LoadGame;
        //On.SaveState.SessionEnded += SaveState_SessionEnded;

        On.RainWorldGame.ctor += RainWorldGame_ctor;
        On.RainWorldGame.ShutDownProcess += RainWorldGame_ShutDownProcess;
        //On.RainWorldGame.GameOver += RainWorldGame_GameOver;
        //On.RainWorldGame.Win += RainWorldGame_Win;

        //On.JollyCoop.JollyHUD.JollyMeter.PlayerIcon.ctor += JollyCoopJollyHUDJollyMeterPlayerIcon_ctor;   //已移植为独立mod：DetailedIcon

        //On.Creature.Violence += Creature_Violence;
        On.Creature.Die += Creature_Die;

        On.AbstractCreature.Move += AbstractCreature_Move;

        On.MoreSlugcats.GlowWeed.HitByWeapon += MoreSlugcatsGlowWeed_HitByWeapon;

        On.KingTusks.Tusk.ShootUpdate += KingTusksTusk_ShootUpdate;
        On.BigNeedleWorm.Swish += BigNeedleWorm_Swish;
    }

    public void OnDisable()
    {
        // Remove hooks-删除钩子
        IL.Player.GrabUpdate -= PlayerGrabUpdate_IL;
        if (Scientist.ScientistTools.DictionaryKeyHasValue(Scientist.Plugin.modHooked, "BeastMaster", true))
        {
            Scientist.ScientistHooks.BeastMasterHooks.HookOff();
        }
    }

    private void RainWorld_PostModsInit(On.RainWorld.orig_PostModsInit orig, RainWorld self)
    {
        orig(self);
        var list = GameObject.FindObjectsOfType<BaseUnityPlugin>();
        foreach (var plugin in list)
        {
            if (plugin.GetType().Name == "BeastMaster" && !Scientist.ScientistTools.DictionaryKeyHasValue(Scientist.Plugin.modHooked, "BeastMaster", true) )     //"fyre.BeastMaster"
            {
                Scientist.ScientistHooks.BeastMasterHooks.HookOn(plugin);
                Scientist.ScientistLogger.Log("Hooked on BeastMaster");
                Scientist.Plugin.modHooked["BeastMaster"] = true;
            }
        }
    }

    private void AbstractCreature_Move(On.AbstractCreature.orig_Move orig, AbstractCreature self, WorldCoordinate newCoord)
    {
        if (Scientist.ScientistPlayer.anesthesiaCreatures.ContainsKey(Scientist.ScientistTools.FeaturesTypeString(self)) && Scientist.ScientistPlayer.anesthesiaCreatures[Scientist.ScientistTools.FeaturesTypeString(self)].IsEnabled())
        {
            Scientist.ScientistPlayer.anesthesiaCreatures[Scientist.ScientistTools.FeaturesTypeString(self)].AddCounter(Mathf.FloorToInt(self.realizedObject.TotalMass));
            return;
        }
        orig(self, newCoord);
    }

    private void Creature_Die(On.Creature.orig_Die orig, Creature self)
    {
        bool wadDead = self.dead;
        orig(self);
        if (wadDead != self.dead)
        {
            if (self is BigNeedleWorm)
            {
                items.AbstractPhysicalObjects.AnesthesiaSpearAbstract asa = new(self.room.world, null, self.abstractPhysicalObject.pos, self.room.world.game.GetNewID());
                self.room.abstractRoom.AddEntity(asa);
                asa.RealizeInRoom();
                asa.realizedObject.firstChunk.vel = Scientist.ScientistTools.RandomAngleVector2() * 10f;
            }
        }
    }

    private void GraphicsModule_Update(On.GraphicsModule.orig_Update orig, GraphicsModule self)
    {
        orig(self);
        string fts = Scientist.ScientistTools.FeaturesTypeString(self.owner);
        if (Scientist.ScientistPlayer.colorfulCreatures.ContainsKey(fts))
        {
            if (Scientist.ScientistPlayer.colorfulCreatures[fts].enabled)
            {
                Scientist.ScientistPlayer.colorfulCreatures[fts].AddCounter(UnityEngine.Random.Range(1, 41));
                Scientist.ScientistPlayer.colorfulCreatures[fts].SetLightSource();
            }
            else
            {
                Scientist.ScientistPlayer.colorfulCreatures.Remove(fts);
            }
        }
    }

    private void GraphicsModule_DrawSprites(On.GraphicsModule.orig_DrawSprites orig, GraphicsModule self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        orig(self, sLeaser, rCam, timeStacker, camPos);
        string fts = Scientist.ScientistTools.FeaturesTypeString(self.owner);
        if (Scientist.ScientistPlayer.colorfulCreatures.ContainsKey(fts) && Scientist.ScientistPlayer.colorfulCreatures[fts].enabled)
        {
            Scientist.ScientistPlayer.colorfulCreatures[fts].SetOriginalColors(sLeaser);
            if (Scientist.ScientistPlayer.colorfulCreatures[fts].lightSource == null)
            {
                Scientist.ScientistPlayer.colorfulCreatures[fts].SetLightSource();
            }
            for (int i = 0; i < sLeaser.sprites.Length; i++)
            {
                if (sLeaser.sprites[i] != null)
                {
                    sLeaser.sprites[i].color = Scientist.ScientistPlayer.colorfulCreatures[fts].lightSource.color;
                }
            }
        }
    }

    private void MoreSlugcatsGlowWeed_HitByWeapon(On.MoreSlugcats.GlowWeed.orig_HitByWeapon orig, GlowWeed self, Weapon weapon)
    {
        orig(self, weapon);
        if (weapon is items.SharpSpear || weapon is Spear)
        {
            float Rand = UnityEngine.Random.value;
            for (int k = 0; k < UnityEngine.Random.Range(3, 8); k++)
            {
                Vector2 pos = weapon.firstChunk.pos + Custom.DegToVec(Rand * 360f) * 5f * Rand;
                Vector2 vel = -weapon.firstChunk.vel * -0.1f + Custom.DegToVec(Rand * 360f) * Mathf.Lerp(0.2f, 0.4f, Rand) * weapon.firstChunk.vel.magnitude;
                self.room.AddObject(new Spark(pos, vel, new Color(1f, 1f, 1f), null, 10, 170));
            }
            self.room.AddObject(new StationaryEffect(weapon.firstChunk.pos, new Color(1f, 1f, 1f), null, StationaryEffect.EffectType.FlashingOrb));
            AbstractPhysicalObject apo = new AbstractPhysicalObject(self.room.world, Scientist.Register.InflatableGlowingShield, null, self.abstractPhysicalObject.pos, self.room.world.game.GetNewID());
            self.room.abstractRoom.AddEntity(apo);
            apo.RealizeInRoom();
            apo.realizedObject.firstChunk.vel = -weapon.firstChunk.vel / 3f;
            self.Destroy();
            weapon.Destroy();
        }
    }

    private void RainWorldGame_ShutDownProcess(On.RainWorldGame.orig_ShutDownProcess orig, RainWorldGame self)
    {
        orig(self);
        ScientistLogger.Log("RainWorldGame_ShutDownProcess");
    }

    private void RainWorldGame_ctor(On.RainWorldGame.orig_ctor orig, RainWorldGame self, ProcessManager manager)
    {
        orig(self, manager);
        ScientistLogger.Log("RainWorldGame_ctor");
        Scientist.ScientistPlayer.pfEatTimesInACycle = Enumerable.Repeat(0, self.Players.Count()).ToArray();
        Scientist.ScientistPlayer.pfAfterActiveDie = Enumerable.Repeat(false, self.Players.Count()).ToArray();
        Scientist.ScientistPlayer.pfTime = Enumerable.Repeat(0, self.Players.Count()).ToArray();
    }

    private void RainWorldGame_Win(On.RainWorldGame.orig_Win orig, RainWorldGame self, bool malnourished)
    {
        orig(self, malnourished);
        ScientistLogger.Log("RainWorldGame_Win");
    }

    private void RainWorldGame_GameOver(On.RainWorldGame.orig_GameOver orig, RainWorldGame self, Creature.Grasp dependentOnGrasp)
    {
        orig(self, dependentOnGrasp);
        ScientistLogger.Log("RainWorldGame_GameOver");
    }

    private void SaveState_SessionEnded(On.SaveState.orig_SessionEnded orig, SaveState self, RainWorldGame game, bool survived, bool newMalnourished)
    {
        orig(self, game, survived, newMalnourished);
        ScientistLogger.Log("SaveState_SessionEnded");
    }

    private void SaveState_LoadGame(On.SaveState.orig_LoadGame orig, SaveState self, string str, RainWorldGame game)
    {
        orig(self, str, game);
        ScientistLogger.Log("SaveState_LoadGame");
    }

    private void SaveState_ApplyCustomEndGame(On.SaveState.orig_ApplyCustomEndGame orig, SaveState self, RainWorldGame game, bool addFiveCycles)
    {
        orig(self, game, addFiveCycles);
        ScientistLogger.Log("SaveState_ApplyCustomEndGame");
    }

    private void Player_Stun(On.Player.orig_Stun orig, Player self, int st)
    {
        if (Scientist.ScientistPlayer.pfTime[ScientistTools.PlayerIndex(self)] > 0)
        {
            return;
        }
        orig(self, st);
    }

    private void Player_Die(On.Player.orig_Die orig, Player self)
    {
        bool wasDead = self.dead;
        if (Scientist.ScientistPlayer.pfTime[ScientistTools.PlayerIndex(self)] > 0 && Scientist.ScientistPlayer.pfEatTimesInACycle[ScientistTools.PlayerIndex(self)] < 3)
        {
            Scientist.ScientistPlayer.pfAfterActiveDie[ScientistTools.PlayerIndex(self)] = true;
            return;
        }
        orig(self);
        Scientist.ScientistPlayer.pfAfterActiveDie[ScientistTools.PlayerIndex(self)] = false;
        Scientist.ScientistPlayer.pfEatTimesInACycle[ScientistTools.PlayerIndex(self)] = 0;
    }

    private bool PlayerCanIPickThisUp(On.Player.orig_CanIPickThisUp orig, Player self, PhysicalObject obj)
    {
        if (obj is items.SharpSpear)
        {
            if ( (obj as items.SharpSpear).stuckInWall != null)
            {
                if (self.SlugCatClass == MoreSlugcatsEnums.SlugcatStatsName.Gourmand && !self.gourmandExhausted)                        //胖猫不力竭时可以拾取插入墙中的尖矛
                {
                    return true;
                }
                if (self.SlugCatClass.value == MOD_ID && CanPickupShapespearStuckinwall.TryGet(self, out var canPickup) && canPickup)   //科学家仅在配置允许时可以拾取插入墙中的尖矛
                {
                    return true;
                }
                return false;
            }
        }
        return orig(self, obj);
    }

    private void AbstractRoomAddEntity(On.AbstractRoom.orig_AddEntity orig, AbstractRoom self, AbstractWorldEntity ent)
    { 
        //Console.WriteLine($"AbstractRoomAddEntity  ent = {ent}");
        orig(self, ent);
    }

    public void RoomAddObject(On.Room.orig_AddObject orig, Room self, UpdatableAndDeletable obj)
    {
        orig(self, obj);
    }

    public void PlayerUpdate(On.Player.orig_Update orig, Player self, bool eu) 
    {
        ScientistPlayer.offlineTime = self.input[0].AnyInput || self.input[1].AnyInput ?  0 : ScientistPlayer.offlineTime + 1;
        orig(self, eu);

        if (self.SlugCatClass.value == MOD_ID && (ScientistPlayer.offlineTime <= 10 || ScientistPlayer.offlineTime > 640) && scr != null)
        {
            scr.Destroy();
            scr = null;
        }
        if (ScientistPlayer.offlineTime > 320 && ScientistPlayer.offlineTime <= 640)
        {
            if (self.SlugCatClass.value == MOD_ID  && (CraftingResults(self) != null || Scientist.ScientistSlugcat.GetSpecialCraftingResult(self) != null) && scr == null)
            {
                AbstractPhysicalObject apo = ( CraftingResults(self) != null ) ? ScientistSlugcat.CraftingResults(self, self.grasps[0], self.grasps[1]) : Scientist.ScientistSlugcat.GetSpecialCraftingResult(self);
                IconSymbol.IconSymbolData isd = apo.type != AbstractPhysicalObject.AbstractObjectType.Creature ? (IconSymbol.IconSymbolData)ItemSymbol.SymbolDataFromItem(apo) : (IconSymbol.IconSymbolData)CreatureSymbol.SymbolDataFromCreature((apo.realizedObject as Creature).abstractCreature);
                IconSymbol iconSymbol = IconSymbol.CreateIconSymbol(isd, new FContainer());
                scr = new items.ShowCraftingResult(self, iconSymbol.spriteName, iconSymbol.myColor, ScientistTools.RandomAngleVector2(new float[2][] { new float[2] { 0f, 60f }, new float[2] { 120f, 180f } }), 40f);
                self.room.AddObject(scr);

                ScientistLogger.Log($"apo = {apo}, apo.type = {apo.type}");
                ScientistLogger.Log($"isd = {isd}");
            }
        }

        if (Scientist.ScientistPlayer.pfTime[ScientistTools.PlayerIndex(self)] > 0)   //无痛果效果
        {
            if (!self.inShortcut)
            {
                Scientist.ScientistPlayer.pfTime[ScientistTools.PlayerIndex(self)]--;
                if (Scientist.ScientistPlayer.pfTime[ScientistTools.PlayerIndex(self)] == 0)
                {
                    Scientist.ScientistPlayer.pfTime[ScientistTools.PlayerIndex(self)]--;
                }
            }
            //self.mushroomEffect = Custom.LerpAndTick(self.mushroomEffect, 1f, 0.05f, 0.025f);
            if (!ModManager.CoopAvailable)
            {
                goto IL_1S;
            }
            using (List<AbstractCreature>.Enumerator enumerator = self.abstractCreature.world.game.AlivePlayers.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    AbstractCreature abstractCreature2 = enumerator.Current;
                    if (abstractCreature2.realizedCreature != null)
                    {
                        //(abstractCreature2.realizedCreature as Player).mushroomEffect = Mathf.Max(self.mushroomEffect, (abstractCreature2.realizedCreature as Player).mushroomEffect);
                    }
                }
                goto IL_1S;
            }
        }
        //self.mushroomEffect = Custom.LerpAndTick(self.mushroomEffect, 0f, 0.025f, 0.014285714f);
        IL_1S:
        if (Scientist.ScientistPlayer.pfTime[ScientistTools.PlayerIndex(self)] < 0 && Scientist.ScientistPlayer.pfAfterActiveDie[ScientistTools.PlayerIndex(self)])
        {
            self.Die();
        }
        if (Scientist.ScientistPlayer.pfEatTimesInACycle[ScientistTools.PlayerIndex(self)] >= 3)
        {
            self.Die();
        }
    }

    public void PlayerThrowObject(On.Player.orig_ThrowObject orig, Player self, int grasp, bool eu)
    {
        ScientistLogger.Log($"PlayerThrown  {self.grasps[grasp].grabbed.abstractPhysicalObject}");
        /*Console.WriteLine($"PlayerThrown  {self.grasps[grasp].grabbed is WaterNut}  {self.grasps[grasp].grabbed is SwollenWaterNut}");*/
        orig(self, grasp, eu);
    }

    public void PlayerThrownSpear(On.Player.orig_ThrownSpear orig, Player self, Spear spear)
    {
        orig(self, spear);
        if (self.SlugCatClass.value == MOD_ID && Power.TryGet(self, out var power) && power)
        {
            spear.spearDamageBonus = 10000f;
        }
    }

    public void PlayerJump(On.Player.orig_Jump orig, Player self)
    {
        orig(self);
        if (self.SlugCatClass.value == MOD_ID)
        {
            if (LowerJump.TryGet(self, out float power))
            {
                self.jumpBoost *= power;
            }
        }

    }

    // Load any resources, such as sprites or sounds-加载任何资源 包括图像素材和音效
    private void LoadResources(RainWorld rainWorld)
    {
        Scientist.ScientistLogger.Log("Loading Scientist resources...");

        //Futile.atlasManager.LoadAtlas("atlases/icons/Kill_Slugcats");
        Futile.atlasManager.LoadAtlas("atlases/slugcat/scientist_icon");

        Futile.atlasManager.LoadAtlas("atlases/icons/Symbol_ScientistIcon");
        Futile.atlasManager.LoadAtlas("atlases/icons/Symbol_SharpSpear");
        Futile.atlasManager.LoadAtlas("atlases/icons/Symbol_ConcentratedDangleFruit");
        Futile.atlasManager.LoadAtlas("atlases/icons/Symbol_PainlessFruit");
        Futile.atlasManager.LoadAtlas("atlases/icons/Symbol_ColorfulFruit");
        Futile.atlasManager.LoadAtlas("atlases/icons/Symbol_InflatableGlowingShield");

        Futile.atlasManager.LoadAtlas("atlases/textures/SharpSpear");
        Futile.atlasManager.LoadAtlas("atlases/textures/ConcentratedDangleFruit");
        Futile.atlasManager.LoadAtlas("atlases/textures/PainlessFruit");
        Futile.atlasManager.LoadAtlas("atlases/textures/ColorfulFruit");
        Futile.atlasManager.LoadAtlas("atlases/textures/InflatableGlowingShield");
    }

    private void RainWorld_OnModsEnabled(On.RainWorld.orig_OnModsEnabled orig, RainWorld self, ModManager.Mod[] newlyEnabledMods)
    {
        //虽然原来的方法是空的，但其他模组也可以勾选这个方法，所以这里也需要orig
        orig(self, newlyEnabledMods);
        ScientistLogger.Log("Scientist is coming!");
        Register.RegisterValues();
    }

    private void RainWorld_OnModsDisabled(On.RainWorld.orig_OnModsDisabled orig, RainWorld self, ModManager.Mod[] newlyDisabledMods)
    {
        orig(self, newlyDisabledMods);
        foreach (ModManager.Mod mod in newlyDisabledMods)
            if (mod.id == MOD_ID) //MOD_ID - 以字符串变量形式表示的mod ID
                Register.UnregisterValues();
    }

    private void AbstractPhysicalObject_Realize(On.AbstractPhysicalObject.orig_Realize orig, AbstractPhysicalObject self)
    {
        ScientistLogger.Log($"self = {self}    type = {self.type}");
        orig(self);
        if (self.type == Register.SharpSpear)
        {
            self.realizedObject = new items.SharpSpear(new items.AbstractPhysicalObjects.SharpSpearAbstract(self.world, null, self.pos, self.ID), self.world);
        }
        if (self.type == Register.ConcentratedDangleFruit)
        {
            self.realizedObject = new items.ConcentratedDangleFruit(self);
        }
        if (self.type == Register.PainlessFruit)
        {
            self.realizedObject = new items.PainlessFruit(self);
        }
        if (self.type == Register.ColorfulFruit)
        {
            self.realizedObject = new items.ColorfulFruit(self, self.world, !ScientistTools.Probability(0.1f));
        }
        if (self.type == Register.InflatableGlowingShield)
        {
            self.realizedObject = new items.InflatableGlowingShield(self, self.world);
        }
        if (self.type == Register.AnesthesiaSpear)
        {
            self.realizedObject = new items.AnesthesiaSpear(new items.AbstractPhysicalObjects.AnesthesiaSpearAbstract(self.world, null, self.pos, self.ID), self.world);
        }
    }

    private Player.ObjectGrabability Player_Grabability(On.Player.orig_Grabability orig, Player self, PhysicalObject obj)
    {
        if (obj is items.SharpSpear)
        {
            if (self.SlugCatClass == SlugcatStats.Name.Red)
            {
                return Player.ObjectGrabability.OneHand;
            }
            return Player.ObjectGrabability.BigOneHand;
        }
        if (obj is items.ConcentratedDangleFruit)
        {
            return Player.ObjectGrabability.OneHand;
        }
        if (obj is items.PainlessFruit)
        {
            return Player.ObjectGrabability.OneHand;
        }
        if (obj is items.ColorfulFruit)
        {
            return Player.ObjectGrabability.OneHand;
        }
        if (obj is items.InflatableGlowingShield)
        {
            return Player.ObjectGrabability.OneHand;
        }
        if (obj is items.AnesthesiaSpear)
        {
            if (self.SlugCatClass == SlugcatStats.Name.Red)
            {
                return Player.ObjectGrabability.OneHand;
            }
            return Player.ObjectGrabability.BigOneHand;
        }
        return orig(self, obj);
    }

    private string ItemSymbolSpriteNameForItem(On.ItemSymbol.orig_SpriteNameForItem orig, AbstractPhysicalObject.AbstractObjectType itemType, int intData)
    {
        if (itemType == Scientist.Register.ScientistIcon)
        {
            return "Symbol_ScientistIcon";
        }
        if (itemType == Scientist.Register.SharpSpear)
        {
            return "Symbol_ShapeSpear";
        }
        if (itemType == Scientist.Register.ConcentratedDangleFruit)
        {
            return "Symbol_ConcentratedDangleFruit";
        }
        if (itemType == Scientist.Register.PainlessFruit)
        {
            return "Symbol_PainlessFruit";
        }
        if (itemType == Scientist.Register.ColorfulFruit)
        {
            return "Symbol_ColorfulFruit";
        }
        if (itemType == Scientist.Register.InflatableGlowingShield)
        {
            return "Symbol_InflatableGlowingShield";
        }
        return orig(itemType, intData);
            
    }

    private Color ItemSymbolColorForItem(On.ItemSymbol.orig_ColorForItem orig, AbstractPhysicalObject.AbstractObjectType itemType, int intData)
    {
        if (itemType == Scientist.Register.ScientistIcon)
        {
            return Color.gray;
        }
        if (itemType == Scientist.Register.SharpSpear)
        {
            return Color.white;
        }
        if (itemType == Scientist.Register.ConcentratedDangleFruit)
        {
            return Color.blue;
        }
        if (itemType == Scientist.Register.PainlessFruit)
        {
            return Color.white;//ScientistTools.ColorFromHex("bd4848");
        }
        if (itemType == Scientist.Register.ColorfulFruit)
        {
            return Color.white;
        }
        if (itemType == Scientist.Register.InflatableGlowingShield)
        {
            return Color.white;
        }
        return orig(itemType, intData);
    }

    private bool AbstractConsumableIsTypeConsumable(On.AbstractConsumable.orig_IsTypeConsumable orig, AbstractPhysicalObject.AbstractObjectType type)
    {
        if (type == Scientist.Register.ConcentratedDangleFruit)
        {
            return true;
        }
        if (type == Scientist.Register.PainlessFruit)
        {
            return true;
        }
        if (type == Scientist.Register.ColorfulFruit)
        {
            return true;
        }
        return orig(type);
    }

    public void PlayerGrabUpdate_On(On.Player.orig_GrabUpdate orig, Player self, bool eu)
    {
        if (self.SlugCatClass.value == MOD_ID) {     //判断是否为科学家，如果是，则执行修改后的GrabUpdate
            ScientistGrabUpdate(self, eu);
        }
        else {                                       //不是科学家，则执行原函数（这也会导致其他mod的GrabUpdate钩子在科学家上无法生效）（既然叫科学家，那么能屏蔽外界干扰和更改是很正常的吧~）
            orig(self, eu); 
        }
    }

    public void PlayerGrabUpdate_IL(ILContext il)
    {
        ILCursor cur = new(il);

        /*if (cur.TryGotoNext(ins => ins.MatchCallOrCallvirt(typeof(Player).GetMethod("AddFood")), ins => ins.MatchLdcI4(1), ins => ins.MatchStloc(0), ins => ins.MatchLdarg(0), ins => ins.MatchCallOrCallvirt(typeof(Player).GetProperty("FoodInStomach").GetGetMethod()), ins => ins.MatchLdarg(0), ins => ins.MatchCallOrCallvirt(typeof(Player).GetProperty("MaxFoodInStomach").GetGetMethod())))
        {
            Scientist.ScientistLogger.Log($"Player.GrabUpdate IL hooked, index = {cur.Index}");
            cur.Index += 3;
            cur.RemoveRange(50);
            cur.EmitDelegate(new Func<Player, bool, bool>((player, eu) =>
            {
                int num5 = 0;
                if 
            player.grasps[2].grabbed is IPlayerEdible && (player.grasps[2].grabbed as IPlayerEdible).Edible && (player.grasps[2].grabbed as IPlayerEdible).FoodPoints == 0) || player.FoodInStomach < player.MaxFoodInStomach || player.grasps[2].grabbed is KarmaFlower || player.grasps[2].grabbed is Mushroom
            }));
        }*/
    }
    
    public void SpitUpCraftedObject(Player player)
    {
        player.craftingTutorial = true;
        player.room.PlaySound(SoundID.Slugcat_Swallow_Item, player.mainBodyChunk);
        if (player.grasps.Length == 2)
        {
            AbstractPhysicalObject ApoOne = player.grasps[0].grabbed.abstractPhysicalObject;
            AbstractPhysicalObject ApoTwo = player.grasps[1].grabbed.abstractPhysicalObject;
            AbstractPhysicalObject Apo = ScientistSlugcat.GetSpecialCraftingResult(ApoOne.type, ApoTwo.type, player);
            ScientistLogger.Log($"Apo = {Apo}");
            if (Apo != null)
            {
                player.ReleaseGrasp(0);
                ApoOne.realizedObject.RemoveFromRoom();
                player.room.abstractRoom.RemoveEntity(ApoOne);
                player.ReleaseGrasp(1);
                ApoTwo.realizedObject.RemoveFromRoom();
                player.room.abstractRoom.RemoveEntity(ApoTwo);

                player.room.abstractRoom.AddEntity(Apo);
                Apo.RealizeInRoom();
                if (player.FreeHand() != -1)
                {
                    player.SlugcatGrab(Apo.realizedObject, player.FreeHand());
                }
                return;
            }
        }                                       //实现特殊合成（炸猫式合成）
        AbstractPhysicalObject abstractPhysicalObject2 = null;                  //实现正常合成（胖猫式合成）
        if (ScientistSlugcat.CraftingResults_ObjectData(player.grasps[0], player.grasps[1], true) == AbstractPhysicalObject.AbstractObjectType.DangleFruit) //使用自定义的CraftingResults_ObjectData
        {
            if (ModManager.Expedition && ModManager.MSC && Custom.rainWorld.ExpeditionMode && ExpeditionGame.activeUnlocks.Contains("unl-crafting") && player.SlugCatClass == MoreSlugcatsEnums.SlugcatStatsName.Spear)
            {
                if (abstractPhysicalObject2 != null && player.FreeHand() != -1)
                {
                    player.SlugcatGrab(abstractPhysicalObject2.realizedObject, player.FreeHand());
                }
                return;
            }
            while ((player.grasps[0] != null && player.grasps[0].grabbed is IPlayerEdible) || (player.grasps[1] != null && player.grasps[1].grabbed is IPlayerEdible))
            {
                player.BiteEdibleObject(true);
            }
            player.AddFood(1);
        }
        else
        {
            abstractPhysicalObject2 = ScientistSlugcat.CraftingResults(player, player.grasps[0], player.grasps[1]);     //配方结果自定义CraftingResults
            player.room.abstractRoom.AddEntity(abstractPhysicalObject2);
            abstractPhysicalObject2.RealizeInRoom();
            for (int j = 0; j < player.grasps.Length; j++)
            {
                AbstractPhysicalObject abstractPhysicalObject3 = player.grasps[j].grabbed.abstractPhysicalObject;
                if (player.room.game.session is StoryGameSession)
                {
                    (player.room.game.session as StoryGameSession).RemovePersistentTracker(abstractPhysicalObject3);
                }
                player.ReleaseGrasp(j);
                for (int k = abstractPhysicalObject3.stuckObjects.Count - 1; k >= 0; k--)
                {
                    if (abstractPhysicalObject3.stuckObjects[k] is AbstractPhysicalObject.AbstractSpearStick && abstractPhysicalObject3.stuckObjects[k].A.type == AbstractPhysicalObject.AbstractObjectType.Spear && abstractPhysicalObject3.stuckObjects[k].A.realizedObject != null)
                    {
                        (abstractPhysicalObject3.stuckObjects[k].A.realizedObject as Spear).ChangeMode(Weapon.Mode.Free);
                    }
                }
                abstractPhysicalObject3.LoseAllStuckObjects();
                abstractPhysicalObject3.realizedObject.RemoveFromRoom();
                player.room.abstractRoom.RemoveEntity(abstractPhysicalObject3);
            }
        }
        if (abstractPhysicalObject2 != null && player.FreeHand() != -1)
        {
            player.SlugcatGrab(abstractPhysicalObject2.realizedObject, player.FreeHand());
        }
    }

    public bool GraspsCanBeCrafted(Player player)
    {
        return (player.input[0].y == 1 && CraftingResults(player) != null) || (player.input[0].y == 1 && Scientist.ScientistSlugcat.GetSpecialCraftingResult(player) != null);     //CraftingResults替换成自定义
    }

    public AbstractPhysicalObject.AbstractObjectType CraftingResults(Player player)
    {
        AbstractPhysicalObject.AbstractObjectType result = ScientistSlugcat.CraftingResults_ObjectData(player.grasps[0], player.grasps[1], true);   //CraftingResults_ObjectData替换成自定义
        if (result != null)
        {
            return result;
        }
        /*Creature.Grasp[] grasps = player.grasps;
        for (int i = 0; i < grasps.Length; i++)
        {
            if (grasps[i] != null && grasps[i].grabbed is IPlayerEdible && (grasps[i].grabbed as IPlayerEdible).Edible)
            {
                return null;
            }
        }
        if ( (grasps[0] != null && grasps[0].grabbed is Spear && grasps[1] != null && grasps[1].grabbed is Rock)
            || (grasps[0] != null && grasps[0].grabbed is Rock && grasps[1] != null && grasps[1].grabbed is Spear) )
        {
            return Scientist.Register.SharpSpear;
        }*/
        return null;
    }

    public void Regurgitate(Player player)
    {
        if (ModManager.MSC && player.SlugCatClass == MoreSlugcatsEnums.SlugcatStatsName.Spear)
        {
            player.objectInStomach = new SpearMasterPearl.AbstractSpearMasterPearl(player.room.world, null, player.room.GetWorldCoordinate(player.mainBodyChunk.pos), player.room.game.GetNewID(), -1, -1, null);
        }
        if (player.objectInStomach == null)
        {
            player.objectInStomach = ScientistSlugcat.RandomStomachItem(player);    //随机吐出物品 替换成 自定义
        }
        player.room.abstractRoom.AddEntity(player.objectInStomach);
        player.objectInStomach.pos = player.abstractCreature.pos;
        player.objectInStomach.RealizeInRoom();
        if (ModManager.MMF && MMF.cfgKeyItemTracking.Value && AbstractPhysicalObject.UsesAPersistantTracker(player.objectInStomach) && player.room.game.IsStorySession)
        {
            (player.room.game.session as StoryGameSession).AddNewPersistentTracker(player.objectInStomach);
            if (player.room.abstractRoom.NOTRACKERS)
            {
                player.objectInStomach.tracker.lastSeenRegion = player.lastGoodTrackerSpawnRegion;
                player.objectInStomach.tracker.lastSeenRoom = player.lastGoodTrackerSpawnRoom;
                player.objectInStomach.tracker.ChangeDesiredSpawnLocation(player.lastGoodTrackerSpawnCoord);
            }
        }
        Vector2 vector = player.bodyChunks[0].pos;
        Vector2 vector2 = Custom.DirVec(player.bodyChunks[1].pos, player.bodyChunks[0].pos);
        bool flag = false;
        if (Mathf.Abs(player.bodyChunks[0].pos.y - player.bodyChunks[1].pos.y) > Mathf.Abs(player.bodyChunks[0].pos.x - player.bodyChunks[1].pos.x) && player.bodyChunks[0].pos.y > player.bodyChunks[1].pos.y)
        {
            vector += Custom.DirVec(player.bodyChunks[1].pos, player.bodyChunks[0].pos) * 5f;
            vector2 *= -1f;
            vector2.x += 0.4f * (float)player.flipDirection;
            vector2.Normalize();
            flag = true;
        }
        player.objectInStomach.realizedObject.firstChunk.HardSetPosition(vector);
        player.objectInStomach.realizedObject.firstChunk.vel = Vector2.ClampMagnitude((vector2 * 2f + Custom.RNV() * UnityEngine.Random.value) / player.objectInStomach.realizedObject.firstChunk.mass, 6f);
        player.bodyChunks[0].pos -= vector2 * 2f;
        player.bodyChunks[0].vel -= vector2 * 2f;
        if (player.graphicsModule != null)
        {
            (player.graphicsModule as PlayerGraphics).head.vel += Custom.RNV() * UnityEngine.Random.value * 3f;
        }
        for (int i = 0; i < 3; i++)
        {
            player.room.AddObject(new WaterDrip(vector + Custom.RNV() * UnityEngine.Random.value * 1.5f, Custom.RNV() * 3f * UnityEngine.Random.value + vector2 * Mathf.Lerp(2f, 6f, UnityEngine.Random.value), false));
        }
        player.room.PlaySound(SoundID.Slugcat_Regurgitate_Item, player.mainBodyChunk);
        if (player.objectInStomach.realizedObject is Hazer && player.graphicsModule != null)
        {
            (player.objectInStomach.realizedObject as Hazer).SpitOutByPlayer(PlayerGraphics.SlugcatColor(player.playerState.slugcatCharacter));
        }
        if (flag && player.FreeHand() > -1)
        {
            if (ModManager.MMF && (player.grasps[0] != null ^ player.grasps[1] != null) && player.Grabability(player.objectInStomach.realizedObject) == Player.ObjectGrabability.BigOneHand)
            {
                int num = 0;
                if (player.FreeHand() == 0)
                {
                    num = 1;
                }
                if (player.Grabability(player.grasps[num].grabbed) != Player.ObjectGrabability.BigOneHand)
                {
                    player.SlugcatGrab(player.objectInStomach.realizedObject, player.FreeHand());
                }
            }
            else
            {
                player.SlugcatGrab(player.objectInStomach.realizedObject, player.FreeHand());
            }
        }
        player.objectInStomach = null;
    }

    public void ScientistGrabUpdate(Player player, bool eu)
    {
        if (player.spearOnBack != null)
        {
            player.spearOnBack.Update(eu);
        }
        if ((ModManager.MSC || ModManager.CoopAvailable) && player.slugOnBack != null)
        {
            player.slugOnBack.Update(eu);
        }
        bool flag = ((player.input[0].x == 0 && player.input[0].y == 0 && !player.input[0].jmp && !player.input[0].thrw) || (ModManager.MMF && player.input[0].x == 0 && player.input[0].y == 1 && !player.input[0].jmp && !player.input[0].thrw && (player.bodyMode != Player.BodyModeIndex.ClimbingOnBeam || player.animation == Player.AnimationIndex.BeamTip || player.animation == Player.AnimationIndex.StandOnBeam))) && (player.mainBodyChunk.submersion < 0.5f || player.isRivulet);
        bool flag2 = false;
        bool flag3 = false;
        player.craftingObject = false;
        int num = -1;
        int num2 = -1;
        bool flag4 = false;
        if (ModManager.MSC && !player.input[0].pckp && player.SlugCatClass == MoreSlugcatsEnums.SlugcatStatsName.Spear)
        {
            PlayerGraphics.TailSpeckles tailSpecks = (player.graphicsModule as PlayerGraphics).tailSpecks;
            if (tailSpecks.spearProg > 0f)
            {
                tailSpecks.setSpearProgress(Mathf.Lerp(tailSpecks.spearProg, 0f, 0.05f));
                if (tailSpecks.spearProg < 0.025f)
                {
                    tailSpecks.setSpearProgress(0f);
                }
            }
            else
            {
                player.smSpearSoundReady = false;
            }
        }
        if (player.input[0].pckp && !player.input[1].pckp && player.switchHandsProcess == 0f && !player.isSlugpup)
        {
            bool flag5 = player.grasps[0] != null || player.grasps[1] != null;
            if (player.grasps[0] != null && (player.Grabability(player.grasps[0].grabbed) == Player.ObjectGrabability.TwoHands || player.Grabability(player.grasps[0].grabbed) == Player.ObjectGrabability.Drag))
            {
                flag5 = false;
            }
            if (flag5)
            {
                if (player.switchHandsCounter == 0)
                {
                    player.switchHandsCounter = 15;
                }
                else
                {
                    player.room.PlaySound(SoundID.Slugcat_Switch_Hands_Init, player.mainBodyChunk);
                    player.switchHandsProcess = 0.01f;
                    player.wantToPickUp = 0;
                    player.noPickUpOnRelease = 20;
                }
            }
            else
            {
                player.switchHandsProcess = 0f;
            }
        }
        if (player.switchHandsProcess > 0f)
        {
            float num3 = player.switchHandsProcess;
            player.switchHandsProcess += 0.083333336f;
            if (num3 < 0.5f && player.switchHandsProcess >= 0.5f)
            {
                player.room.PlaySound(SoundID.Slugcat_Switch_Hands_Complete, player.mainBodyChunk);
                player.SwitchGrasps(0, 1);
            }
            if (player.switchHandsProcess >= 1f)
            {
                player.switchHandsProcess = 0f;
            }
        }
        int num4 = -1;
        int num5 = -1;
        int num6 = -1;
        if (flag)
        {
            int num7 = -1;
            if (ModManager.MSC)
            {
                for (int i = 0; i < 2; i++)
                {
                    if (player.grasps[i] != null)
                    {
                        if (player.grasps[i].grabbed is JokeRifle)
                        {
                            num2 = i;
                        }
                        else if (JokeRifle.IsValidAmmo(player.grasps[i].grabbed))
                        {
                            num = i;
                        }
                    }
                }
            }
            int num8 = 0;
            while (num5 < 0 && num8 < 2 && (!ModManager.MSC || player.SlugCatClass != MoreSlugcatsEnums.SlugcatStatsName.Spear))
            {
                if (player.grasps[num8] != null && player.grasps[num8].grabbed is IPlayerEdible && (player.grasps[num8].grabbed as IPlayerEdible).Edible)
                {
                    num5 = num8;
                }
                num8++;
            }
            if ((num5 == -1 || (player.FoodInStomach >= player.MaxFoodInStomach && !(player.grasps[num5].grabbed is KarmaFlower) && !(player.grasps[num5].grabbed is Mushroom))) && (player.objectInStomach == null || player.CanPutSpearToBack || player.CanPutSlugToBack))
            {
                int num9 = 0;
                while (num7 < 0 && num4 < 0 && num6 < 0 && num9 < 2)
                {
                    if (player.grasps[num9] != null)
                    {
                        if ((player.CanPutSlugToBack && player.grasps[num9].grabbed is Player && !(player.grasps[num9].grabbed as Player).dead) || player.CanIPutDeadSlugOnBack(player.grasps[num9].grabbed as Player))
                        {
                            num6 = num9;
                        }
                        else if (player.CanPutSpearToBack && player.grasps[num9].grabbed is Spear)
                        {
                            num4 = num9;
                        }
                        else if (player.CanBeSwallowed(player.grasps[num9].grabbed))
                        {
                            num7 = num9;
                        }
                    }
                    num9++;
                }
            }
            if (num5 > -1 && player.noPickUpOnRelease < 1)
            {
                if (!player.input[0].pckp)
                {
                    int num10 = 1;
                    while (num10 < 10 && player.input[num10].pckp)
                    {
                        num10++;
                    }
                    if (num10 > 1 && num10 < 10)
                    {
                        player.PickupPressed();
                    }
                }
            }
            else if (player.input[0].pckp && !player.input[1].pckp)
            {
                player.PickupPressed();
            }
            if (player.input[0].pckp)
            {
                if (ModManager.MSC && (player.FreeHand() == -1 || player.SlugCatClass == MoreSlugcatsEnums.SlugcatStatsName.Artificer) && GraspsCanBeCrafted(player))   //使用修改后的GraspsCanBeCrafted
                {
                    player.craftingObject = true;
                    flag3 = true;
                    num5 = -1;
                }
                if (num6 > -1 || player.CanRetrieveSlugFromBack)
                {
                    player.slugOnBack.increment = true;
                }
                else if (num4 > -1 || player.CanRetrieveSpearFromBack)
                {
                    player.spearOnBack.increment = true;
                }
                else if ((num7 > -1 || player.objectInStomach != null || (player.isGourmand || true)) && (!ModManager.MSC || player.SlugCatClass != MoreSlugcatsEnums.SlugcatStatsName.Spear))  //所有player.isGourmand改为 (player.isGourmand || true)   注:其实可以直接删掉
                {
                    flag3 = true;
                }
                if (num > -1 && num2 > -1)
                {
                    flag4 = true;
                }
                if (ModManager.MSC && player.SlugCatClass == MoreSlugcatsEnums.SlugcatStatsName.Spear && (player.grasps[0] == null || player.grasps[1] == null) && num5 == -1 && player.input[0].y == 0)
                {
                    PlayerGraphics.TailSpeckles tailSpecks2 = (player.graphicsModule as PlayerGraphics).tailSpecks;
                    if (tailSpecks2.spearProg == 0f)
                    {
                        tailSpecks2.newSpearSlot();
                    }
                    if (tailSpecks2.spearProg < 0.1f)
                    {
                        tailSpecks2.setSpearProgress(Mathf.Lerp(tailSpecks2.spearProg, 0.11f, 0.1f));
                    }
                    else
                    {
                        if (!player.smSpearSoundReady)
                        {
                            player.smSpearSoundReady = true;
                            player.room.PlaySound(MoreSlugcatsEnums.MSCSoundID.SM_Spear_Pull, 0f, 1f, 1f + UnityEngine.Random.value * 0.5f);
                        }
                        tailSpecks2.setSpearProgress(Mathf.Lerp(tailSpecks2.spearProg, 1f, 0.05f));
                    }
                    if (tailSpecks2.spearProg > 0.6f)
                    {
                        (player.graphicsModule as PlayerGraphics).head.vel += Custom.RNV() * ((tailSpecks2.spearProg - 0.6f) / 0.4f) * 2f;
                    }
                    if (tailSpecks2.spearProg > 0.95f)
                    {
                        tailSpecks2.setSpearProgress(1f);
                    }
                    if (tailSpecks2.spearProg == 1f)
                    {
                        player.room.PlaySound(MoreSlugcatsEnums.MSCSoundID.SM_Spear_Grab, 0f, 1f, 0.5f + UnityEngine.Random.value * 1.5f);
                        player.smSpearSoundReady = false;
                        Vector2 pos = (player.graphicsModule as PlayerGraphics).tail[(int)((float)(player.graphicsModule as PlayerGraphics).tail.Length / 2f)].pos;
                        for (int j = 0; j < 4; j++)
                        {
                            Vector2 vector = Custom.DirVec(pos, player.bodyChunks[1].pos);
                            player.room.AddObject(new WaterDrip(pos + Custom.RNV() * UnityEngine.Random.value * 1.5f, Custom.RNV() * 3f * UnityEngine.Random.value + vector * Mathf.Lerp(2f, 6f, UnityEngine.Random.value), false));
                        }
                        for (int k = 0; k < 5; k++)
                        {
                            Vector2 vector2 = Custom.RNV();
                            player.room.AddObject(new Spark(pos + vector2 * UnityEngine.Random.value * 40f, vector2 * Mathf.Lerp(4f, 30f, UnityEngine.Random.value), Color.white, null, 4, 18));
                        }
                        int spearType = tailSpecks2.spearType;
                        tailSpecks2.setSpearProgress(0f);
                        AbstractSpear abstractSpear = new AbstractSpear(player.room.world, null, player.room.GetWorldCoordinate(player.mainBodyChunk.pos), player.room.game.GetNewID(), false);
                        player.room.abstractRoom.AddEntity(abstractSpear);
                        abstractSpear.pos = player.abstractCreature.pos;
                        abstractSpear.RealizeInRoom();
                        Vector2 vector3 = player.bodyChunks[0].pos;
                        Vector2 vector4 = Custom.DirVec(player.bodyChunks[1].pos, player.bodyChunks[0].pos);
                        if (Mathf.Abs(player.bodyChunks[0].pos.y - player.bodyChunks[1].pos.y) > Mathf.Abs(player.bodyChunks[0].pos.x - player.bodyChunks[1].pos.x) && player.bodyChunks[0].pos.y > player.bodyChunks[1].pos.y)
                        {
                            vector3 += Custom.DirVec(player.bodyChunks[1].pos, player.bodyChunks[0].pos) * 5f;
                            vector4 *= -1f;
                            vector4.x += 0.4f * (float)player.flipDirection;
                            vector4.Normalize();
                        }
                        abstractSpear.realizedObject.firstChunk.HardSetPosition(vector3);
                        abstractSpear.realizedObject.firstChunk.vel = Vector2.ClampMagnitude((vector4 * 2f + Custom.RNV() * UnityEngine.Random.value) / abstractSpear.realizedObject.firstChunk.mass, 6f);
                        if (player.FreeHand() > -1)
                        {
                            player.SlugcatGrab(abstractSpear.realizedObject, player.FreeHand());
                        }
                        if (abstractSpear.type == AbstractPhysicalObject.AbstractObjectType.Spear)
                        {
                            (abstractSpear.realizedObject as Spear).Spear_makeNeedle(spearType, true);
                            if ((player.graphicsModule as PlayerGraphics).useJollyColor)
                            {
                                (abstractSpear.realizedObject as Spear).jollyCustomColor = new Color?(PlayerGraphics.JollyColor(player.playerState.playerNumber, 2));
                            }
                        }
                        player.wantToThrow = 0;
                    }
                }
            }
            if (num5 > -1 && player.wantToPickUp < 1 && (player.input[0].pckp || player.eatCounter <= 15) && player.Consious && Custom.DistLess(player.mainBodyChunk.pos, player.mainBodyChunk.lastPos, 3.6f))
            {
                if (player.graphicsModule != null)
                {
                    (player.graphicsModule as PlayerGraphics).LookAtObject(player.grasps[num5].grabbed);
                }
                if (ModManager.MSC && player.SlugCatClass == MoreSlugcatsEnums.SlugcatStatsName.Saint && (player.KarmaCap == 9 || (player.room.game.IsArenaSession && player.room.game.GetArenaGameSession.arenaSitting.gameTypeSetup.gameType != MoreSlugcatsEnums.GameTypeID.Challenge) || (player.room.game.session is ArenaGameSession && player.room.game.GetArenaGameSession.arenaSitting.gameTypeSetup.gameType == MoreSlugcatsEnums.GameTypeID.Challenge && player.room.game.GetArenaGameSession.arenaSitting.gameTypeSetup.challengeMeta.ascended)) && player.grasps[num5].grabbed is Fly && player.eatCounter < 1)
                {
                    player.room.PlaySound(SoundID.Snail_Pop, player.mainBodyChunk, false, 1f, 1.5f + UnityEngine.Random.value);
                    player.eatCounter = 30;
                    player.room.AddObject(new ShockWave(player.grasps[num5].grabbed.firstChunk.pos, 25f, 0.8f, 4, false));
                    for (int l = 0; l < 5; l++)
                    {
                        player.room.AddObject(new Spark(player.grasps[num5].grabbed.firstChunk.pos, Custom.RNV() * 3f, Color.yellow, null, 25, 90));
                    }
                    player.grasps[num5].grabbed.Destroy();
                    player.grasps[num5].grabbed.abstractPhysicalObject.Destroy();
                    if (player.room.game.IsArenaSession)
                    {
                        player.AddFood(1);
                    }
                }
                flag2 = true;
                if (player.FoodInStomach < player.MaxFoodInStomach || player.grasps[num5].grabbed is KarmaFlower || player.grasps[num5].grabbed is Mushroom || (player.grasps[num5].grabbed is IPlayerEdible && (player.grasps[num5].grabbed as IPlayerEdible).Edible && (player.grasps[num5].grabbed as IPlayerEdible).FoodPoints == 0))
                {
                    flag3 = false;
                    if (player.spearOnBack != null)
                    {
                        player.spearOnBack.increment = false;
                    }
                    if ((ModManager.MSC || ModManager.CoopAvailable) && player.slugOnBack != null)
                    {
                        player.slugOnBack.increment = false;
                    }
                    if (player.eatCounter < 1)
                    {
                        player.eatCounter = 15;
                        player.BiteEdibleObject(eu);
                    }
                }
                else if (player.eatCounter < 20 && player.room.game.cameras[0].hud != null)
                {
                    player.room.game.cameras[0].hud.foodMeter.RefuseFood();
                }
            }
        }
        else if (player.input[0].pckp && !player.input[1].pckp)
        {
            player.PickupPressed();
        }
        else
        {
            if (player.CanPutSpearToBack)
            {
                for (int m = 0; m < 2; m++)
                {
                    if (player.grasps[m] != null && player.grasps[m].grabbed is Spear)
                    {
                        num4 = m;
                        break;
                    }
                }
            }
            if (player.CanPutSlugToBack)
            {
                for (int n = 0; n < 2; n++)
                {
                    if (player.grasps[n] != null && player.grasps[n].grabbed is Player && !(player.grasps[n].grabbed as Player).dead)
                    {
                        num6 = n;
                        break;
                    }
                }
            }
            if (player.input[0].pckp && (num6 > -1 || player.CanRetrieveSlugFromBack))
            {
                player.slugOnBack.increment = true;
            }
            if (player.input[0].pckp && (num4 > -1 || player.CanRetrieveSpearFromBack))
            {
                player.spearOnBack.increment = true;
            }
        }
        int num11 = 0;
        if (ModManager.MMF && (player.grasps[0] == null || !(player.grasps[0].grabbed is Creature)) && player.grasps[1] != null && player.grasps[1].grabbed is Creature)
        {
            num11 = 1;
        }
        if (ModManager.MSC && SlugcatStats.SlugcatCanMaul(player.SlugCatClass))  //撕咬
        {
            if (player.input[0].pckp && player.grasps[num11] != null && player.grasps[num11].grabbed is Creature && (player.CanMaulCreature(player.grasps[num11].grabbed as Creature) || player.maulTimer > 0))
            {
                player.maulTimer++;
                (player.grasps[num11].grabbed as Creature).Stun(60);
                player.MaulingUpdate(num11);
                if (player.spearOnBack != null)
                {
                    player.spearOnBack.increment = false;
                    player.spearOnBack.interactionLocked = true;
                }
                if (player.slugOnBack != null)
                {
                    player.slugOnBack.increment = false;
                    player.slugOnBack.interactionLocked = true;
                }
                if (player.grasps[num11] != null && player.maulTimer % 40 == 0)
                {
                    player.room.PlaySound(SoundID.Slugcat_Eat_Meat_B, player.mainBodyChunk);
                    player.room.PlaySound(SoundID.Drop_Bug_Grab_Creature, player.mainBodyChunk, false, 1f, 0.76f);
                    /*Custom.Log(new string[]
                    {
                    "Mauled target"
                    });*/
                    if (!(player.grasps[num11].grabbed as Creature).dead)
                    {
                        for (int num12 = UnityEngine.Random.Range(8, 14); num12 >= 0; num12--)
                        {
                            player.room.AddObject(new WaterDrip(Vector2.Lerp(player.grasps[num11].grabbedChunk.pos, player.mainBodyChunk.pos, UnityEngine.Random.value) + player.grasps[num11].grabbedChunk.rad * Custom.RNV() * UnityEngine.Random.value, Custom.RNV() * 6f * UnityEngine.Random.value + Custom.DirVec(player.grasps[num11].grabbed.firstChunk.pos, (player.mainBodyChunk.pos + (player.graphicsModule as PlayerGraphics).head.pos) / 2f) * 7f * UnityEngine.Random.value + Custom.DegToVec(Mathf.Lerp(-90f, 90f, UnityEngine.Random.value)) * UnityEngine.Random.value * player.EffectiveRoomGravity * 7f, false));
                        }
                        Creature creature = player.grasps[num11].grabbed as Creature;
                        creature.SetKillTag(player.abstractCreature);
                        creature.Violence(player.bodyChunks[0], new Vector2?(new Vector2(0f, 0f)), player.grasps[num11].grabbedChunk, null, Creature.DamageType.Bite, 1f, 15f);
                        creature.stun = 5;
                        if (creature.abstractCreature.creatureTemplate.type == MoreSlugcatsEnums.CreatureTemplateType.Inspector)
                        {
                            creature.Die();
                        }
                    }
                    player.maulTimer = 0;
                    player.wantToPickUp = 0;
                    if (player.grasps[num11] != null)
                    {
                        player.TossObject(num11, eu);
                        player.ReleaseGrasp(num11);
                    }
                    player.standing = true;
                }
                return;
            }
            if (player.grasps[num11] != null && player.grasps[num11].grabbed is Creature && (player.grasps[num11].grabbed as Creature).Consious && !player.IsCreatureLegalToHoldWithoutStun(player.grasps[num11].grabbed as Creature))
            {
                /*Custom.Log(new string[]
                {
                "Lost hold of live mauling target"
                });*/
                player.maulTimer = 0;
                player.wantToPickUp = 0;
                player.ReleaseGrasp(num11);
                return;
            }
        }
        if (player.input[0].pckp && player.grasps[num11] != null && player.grasps[num11].grabbed is Creature && player.CanEatMeat(player.grasps[num11].grabbed as Creature) && (player.grasps[num11].grabbed as Creature).Template.meatPoints > 0)
        {
            player.eatMeat++;
            player.EatMeatUpdate(num11);
            if (!ModManager.MMF)
            {
            }
            if (player.spearOnBack != null)
            {
                player.spearOnBack.increment = false;
                player.spearOnBack.interactionLocked = true;
            }
            if ((ModManager.MSC || ModManager.CoopAvailable) && player.slugOnBack != null)
            {
                player.slugOnBack.increment = false;
                player.slugOnBack.interactionLocked = true;
            }
            if (player.grasps[num11] != null && player.eatMeat % 80 == 0 && ((player.grasps[num11].grabbed as Creature).State.meatLeft <= 0 || player.FoodInStomach >= player.MaxFoodInStomach))
            {
                player.eatMeat = 0;
                player.wantToPickUp = 0;
                player.TossObject(num11, eu);
                player.ReleaseGrasp(num11);
                player.standing = true;
            }
            return;
        }
        if (!player.input[0].pckp && player.grasps[num11] != null && player.eatMeat > 60)
        {
            player.eatMeat = 0;
            player.wantToPickUp = 0;
            player.TossObject(num11, eu);
            player.ReleaseGrasp(num11);
            player.standing = true;
            return;
        }
        player.eatMeat = Custom.IntClamp(player.eatMeat - 1, 0, 50);
        player.maulTimer = Custom.IntClamp(player.maulTimer - 1, 0, 20);
        if (!ModManager.MMF || player.input[0].y == 0)
        {
            if (flag2 && player.eatCounter > 0)
            {
                if (ModManager.MSC)
                {
                    if (num5 <= -1 || player.grasps[num5] == null || !(player.grasps[num5].grabbed is GooieDuck) || (player.grasps[num5].grabbed as GooieDuck).bites != 6 || player.timeSinceSpawned % 2 == 0)
                    {
                        player.eatCounter--;
                    }
                    if (num5 > -1 && player.grasps[num5] != null && player.grasps[num5].grabbed is GooieDuck && (player.grasps[num5].grabbed as GooieDuck).bites == 6 && player.FoodInStomach < player.MaxFoodInStomach)
                    {
                        (player.graphicsModule as PlayerGraphics).BiteStruggle(num5);
                    }
                }
                else
                {
                    player.eatCounter--;
                }
            }
            else if (!flag2 && player.eatCounter < 40)
            {
                player.eatCounter++;
            }
        }
        if (flag4 && player.input[0].y == 0)
        {
            player.reloadCounter++;
            if (player.reloadCounter > 40)
            {
                (player.grasps[num2].grabbed as JokeRifle).ReloadRifle(player.grasps[num].grabbed);
                BodyChunk mainBodyChunk = player.mainBodyChunk;
                mainBodyChunk.vel.y = mainBodyChunk.vel.y + 4f;
                player.room.PlaySound(SoundID.Gate_Clamp_Lock, player.mainBodyChunk, false, 0.5f, 3f + UnityEngine.Random.value);
                AbstractPhysicalObject abstractPhysicalObject = player.grasps[num].grabbed.abstractPhysicalObject;
                player.ReleaseGrasp(num);
                abstractPhysicalObject.realizedObject.RemoveFromRoom();
                abstractPhysicalObject.Room.RemoveEntity(abstractPhysicalObject);
                player.reloadCounter = 0;
            }
        }
        else
        {
            player.reloadCounter = 0;
        }
        if (ModManager.MMF && player.mainBodyChunk.submersion >= 0.5f)
        {
            flag3 = false;
        }
        if (flag3)
        {
            if (player.craftingObject)
            {
                player.swallowAndRegurgitateCounter++;
                if (player.swallowAndRegurgitateCounter > 105)
                {
                    SpitUpCraftedObject(player);                //使用修改后的SpitUpCraftedObject
                    player.swallowAndRegurgitateCounter = 0;
                }
            }
            else if (!ModManager.MMF || player.input[0].y == 0)
            {
                player.swallowAndRegurgitateCounter++;
                if ((player.objectInStomach != null || (player.isGourmand || true) || (ModManager.MSC && player.SlugCatClass == MoreSlugcatsEnums.SlugcatStatsName.Spear)) && player.swallowAndRegurgitateCounter > 110)
                {
                    bool flag6 = false;
                    if ((player.isGourmand || true) && player.objectInStomach == null)
                    {
                        flag6 = true;
                    }
                    if (!flag6 || (flag6 && player.FoodInStomach >= 1))
                    {
                        if (flag6)
                        {
                            player.SubtractFood(1);
                        }
                        Regurgitate(player);        //使用修改后的Regurgitate
                    }
                    else
                    {
                        player.firstChunk.vel += new Vector2(UnityEngine.Random.Range(-1f, 1f), 0f);
                        player.Stun(30);
                    }
                    if (player.spearOnBack != null)
                    {
                        player.spearOnBack.interactionLocked = true;
                    }
                    if ((ModManager.MSC || ModManager.CoopAvailable) && player.slugOnBack != null)
                    {
                        player.slugOnBack.interactionLocked = true;
                    }
                    player.swallowAndRegurgitateCounter = 0;
                }
                else if (player.objectInStomach == null && player.swallowAndRegurgitateCounter > 90 && false)   //禁止使用胃袋存储
                {
                    for (int num13 = 0; num13 < 2; num13++)
                    {
                        if (player.grasps[num13] != null && player.CanBeSwallowed(player.grasps[num13].grabbed))
                        {
                            player.bodyChunks[0].pos += Custom.DirVec(player.grasps[num13].grabbed.firstChunk.pos, player.bodyChunks[0].pos) * 2f;
                            player.SwallowObject(num13);
                            if (player.spearOnBack != null)
                            {
                                player.spearOnBack.interactionLocked = true;
                            }
                            if ((ModManager.MSC || ModManager.CoopAvailable) && player.slugOnBack != null)
                            {
                                player.slugOnBack.interactionLocked = true;
                            }
                            player.swallowAndRegurgitateCounter = 0;
                            (player.graphicsModule as PlayerGraphics).swallowing = 20;
                            break;
                        }
                    }
                }
            }
            else
            {
                if (player.swallowAndRegurgitateCounter > 0)
                {
                    player.swallowAndRegurgitateCounter--;
                }
                if (player.eatCounter > 0)
                {
                    player.eatCounter--;
                }
            }
        }
        else
        {
            player.swallowAndRegurgitateCounter = 0;
        }
        for (int num14 = 0; num14 < player.grasps.Length; num14++)
        {
            if (player.grasps[num14] != null && player.grasps[num14].grabbed.slatedForDeletetion)
            {
                player.ReleaseGrasp(num14);
            }
        }
        if (player.grasps[0] != null && player.Grabability(player.grasps[0].grabbed) == Player.ObjectGrabability.TwoHands)
        {
            player.pickUpCandidate = null;
        }
        else
        {
            PhysicalObject physicalObject = (player.dontGrabStuff < 1) ? player.PickupCandidate(20f) : null;
            if (player.pickUpCandidate != physicalObject && physicalObject != null && physicalObject is PlayerCarryableItem)
            {
                (physicalObject as PlayerCarryableItem).Blink();
            }
            player.pickUpCandidate = physicalObject;
        }
        if (player.switchHandsCounter > 0)
        {
            player.switchHandsCounter--;
        }
        if (player.wantToPickUp > 0)
        {
            player.wantToPickUp--;
        }
        if (player.wantToThrow > 0)
        {
            player.wantToThrow--;
        }
        if (player.noPickUpOnRelease > 0)
        {
            player.noPickUpOnRelease--;
        }
        if (player.input[0].thrw && !player.input[1].thrw && (!ModManager.MSC || !player.monkAscension))
        {
            player.wantToThrow = 5;
        }
        if (player.wantToThrow > 0)
        {
            if (ModManager.MSC && MMF.cfgOldTongue.Value && player.grasps[0] == null && player.grasps[1] == null && player.SaintTongueCheck())
            {
                Vector2 vector5;
                vector5 = new Vector2((float)player.flipDirection, 0.7f);/*vector5..ctor((float)player.flipDirection, 0.7f);*/
                Vector2 normalized = vector5.normalized;
                if (player.input[0].y > 0)
                {
                    normalized = new Vector2(0f, 1f);/*normalized..ctor(0f, 1f);*/
                }
                normalized = (normalized + player.mainBodyChunk.vel.normalized * 0.2f).normalized;
                player.tongue.Shoot(normalized);
                player.wantToThrow = 0;
            }
            else
            {
                for (int num15 = 0; num15 < 2; num15++)
                {
                    if (player.grasps[num15] != null && player.IsObjectThrowable(player.grasps[num15].grabbed))
                    {
                        player.ThrowObject(num15, eu);
                        player.wantToThrow = 0;
                        break;
                    }
                }
            }
            if ((ModManager.MSC || ModManager.CoopAvailable) && player.wantToThrow > 0 && player.slugOnBack != null && player.slugOnBack.HasASlug)
            {
                Player slugcat = player.slugOnBack.slugcat;
                player.slugOnBack.SlugToHand(eu);
                player.ThrowObject(0, eu);
                float num16 = (player.ThrowDirection >= 0) ? Mathf.Max(player.bodyChunks[0].pos.x, player.bodyChunks[1].pos.x) : Mathf.Min(player.bodyChunks[0].pos.x, player.bodyChunks[1].pos.x);
                for (int num17 = 0; num17 < slugcat.bodyChunks.Length; num17++)
                {
                    slugcat.bodyChunks[num17].pos.y = player.firstChunk.pos.y + 20f;
                    if (player.ThrowDirection < 0)
                    {
                        if (slugcat.bodyChunks[num17].pos.x > num16 - 8f)
                        {
                            slugcat.bodyChunks[num17].pos.x = num16 - 8f;
                        }
                        if (slugcat.bodyChunks[num17].vel.x > 0f)
                        {
                            slugcat.bodyChunks[num17].vel.x = 0f;
                        }
                    }
                    else if (player.ThrowDirection > 0)
                    {
                        if (slugcat.bodyChunks[num17].pos.x < num16 + 8f)
                        {
                            slugcat.bodyChunks[num17].pos.x = num16 + 8f;
                        }
                        if (slugcat.bodyChunks[num17].vel.x < 0f)
                        {
                            slugcat.bodyChunks[num17].vel.x = 0f;
                        }
                    }
                }
            }
        }
        if (player.wantToPickUp > 0)
        {
            bool flag7 = true;
            if (player.animation == Player.AnimationIndex.DeepSwim)
            {
                if (player.grasps[0] == null && player.grasps[1] == null)
                {
                    flag7 = false;
                }
                else
                {
                    for (int num18 = 0; num18 < 10; num18++)
                    {
                        if (player.input[num18].y > -1 || player.input[num18].x != 0)
                        {
                            flag7 = false;
                            break;
                        }
                    }
                }
            }
            else
            {
                for (int num19 = 0; num19 < 5; num19++)
                {
                    if (player.input[num19].y > -1)
                    {
                        flag7 = false;
                        break;
                    }
                }
            }
            if (ModManager.MSC)
            {
                if (player.grasps[0] != null && player.grasps[0].grabbed is EnergyCell && player.mainBodyChunk.submersion > 0f)
                {
                    flag7 = false;
                }
                else if (player.grasps[0] != null && player.grasps[0].grabbed is EnergyCell && player.canJump <= 0 && player.bodyMode != Player.BodyModeIndex.Crawl && player.bodyMode != Player.BodyModeIndex.CorridorClimb && player.bodyMode != Player.BodyModeIndex.ClimbIntoShortCut && player.animation != Player.AnimationIndex.HangFromBeam && player.animation != Player.AnimationIndex.ClimbOnBeam && player.animation != Player.AnimationIndex.AntlerClimb && player.animation != Player.AnimationIndex.VineGrab && player.animation != Player.AnimationIndex.ZeroGPoleGrab)
                {
                    (player.grasps[0].grabbed as EnergyCell).Use(false);
                }
            }
            if (!ModManager.MMF && player.grasps[0] != null && player.HeavyCarry(player.grasps[0].grabbed))
            {
                flag7 = true;
            }
            if (flag7)
            {
                int num20 = -1;
                for (int num21 = 0; num21 < 2; num21++)
                {
                    if (player.grasps[num21] != null)
                    {
                        num20 = num21;
                        break;
                    }
                }
                if (num20 > -1)
                {
                    player.wantToPickUp = 0;
                    if (!ModManager.MSC || player.SlugCatClass != MoreSlugcatsEnums.SlugcatStatsName.Artificer || !(player.grasps[num20].grabbed is Scavenger))
                    {
                        player.pyroJumpDropLock = 0;
                    }
                    if (player.pyroJumpDropLock == 0 && (!ModManager.MSC || player.wantToJump == 0))
                    {
                        player.ReleaseObject(num20, eu);
                        return;
                    }
                }
                else
                {
                    if (player.spearOnBack != null && player.spearOnBack.spear != null && player.mainBodyChunk.ContactPoint.y < 0)
                    {
                        player.room.socialEventRecognizer.CreaturePutItemOnGround(player.spearOnBack.spear, player);
                        player.spearOnBack.DropSpear();
                        return;
                    }
                    if ((ModManager.MSC || ModManager.CoopAvailable) && player.slugOnBack != null && player.slugOnBack.slugcat != null && player.mainBodyChunk.ContactPoint.y < 0)
                    {
                        player.room.socialEventRecognizer.CreaturePutItemOnGround(player.slugOnBack.slugcat, player);
                        player.slugOnBack.DropSlug();
                        player.wantToPickUp = 0;
                        return;
                    }
                    if (ModManager.MSC && player.room != null && player.room.game.IsStorySession && player.room.game.GetStorySession.saveState.wearingCloak && player.AI == null)
                    {
                        player.room.game.GetStorySession.saveState.wearingCloak = false;
                        AbstractConsumable abstractConsumable = new AbstractConsumable(player.room.game.world, MoreSlugcatsEnums.AbstractObjectType.MoonCloak, null, player.room.GetWorldCoordinate(player.mainBodyChunk.pos), player.room.game.GetNewID(), -1, -1, null);
                        player.room.abstractRoom.AddEntity(abstractConsumable);
                        abstractConsumable.pos = player.abstractCreature.pos;
                        abstractConsumable.RealizeInRoom();
                        (abstractConsumable.realizedObject as MoonCloak).free = true;
                        for (int num22 = 0; num22 < abstractConsumable.realizedObject.bodyChunks.Length; num22++)
                        {
                            abstractConsumable.realizedObject.bodyChunks[num22].HardSetPosition(player.mainBodyChunk.pos);
                        }
                        player.dontGrabStuff = 15;
                        player.wantToPickUp = 0;
                        player.noPickUpOnRelease = 20;
                        return;
                    }
                }
            }
            else if (player.pickUpCandidate != null)
            {
                if (player.pickUpCandidate is Spear && player.CanPutSpearToBack && ((player.grasps[0] != null && player.Grabability(player.grasps[0].grabbed) >= Player.ObjectGrabability.BigOneHand) || (player.grasps[1] != null && player.Grabability(player.grasps[1].grabbed) >= Player.ObjectGrabability.BigOneHand) || (player.grasps[0] != null && player.grasps[1] != null)))
                {
                    /*Custom.Log(new string[]
                    {
                    "spear straight to back"
                    });*/
                    player.room.PlaySound(SoundID.Slugcat_Switch_Hands_Init, player.mainBodyChunk);
                    player.spearOnBack.SpearToBack(player.pickUpCandidate as Spear);
                }
                else if (player.CanPutSlugToBack && player.pickUpCandidate is Player && (!(player.pickUpCandidate as Player).dead || player.CanIPutDeadSlugOnBack(player.pickUpCandidate as Player)) && ((player.grasps[0] != null && (player.Grabability(player.grasps[0].grabbed) > Player.ObjectGrabability.BigOneHand || player.grasps[0].grabbed is Player)) || (player.grasps[1] != null && (player.Grabability(player.grasps[1].grabbed) > Player.ObjectGrabability.BigOneHand || player.grasps[1].grabbed is Player)) || (player.grasps[0] != null && player.grasps[1] != null) || player.bodyMode == Player.BodyModeIndex.Crawl))
                {
                    /*Custom.Log(new string[]
                    {
                    "slugpup/player straight to back"
                    });*/
                    player.room.PlaySound(SoundID.Slugcat_Switch_Hands_Init, player.mainBodyChunk);
                    player.slugOnBack.SlugToBack(player.pickUpCandidate as Player);
                }
                else
                {
                    int num23 = 0;
                    for (int num24 = 0; num24 < 2; num24++)
                    {
                        if (player.grasps[num24] == null)
                        {
                            num23++;
                        }
                    }
                    if (player.Grabability(player.pickUpCandidate) == Player.ObjectGrabability.TwoHands && num23 < 4)
                    {
                        for (int num25 = 0; num25 < 2; num25++)
                        {
                            if (player.grasps[num25] != null)
                            {
                                player.ReleaseGrasp(num25);
                            }
                        }
                    }
                    else if (num23 == 0)
                    {
                        for (int num26 = 0; num26 < 2; num26++)
                        {
                            if (player.grasps[num26] != null && player.grasps[num26].grabbed is Fly)
                            {
                                player.ReleaseGrasp(num26);
                                break;
                            }
                        }
                    }
                    int num27 = 0;
                    while (num27 < 2)
                    {
                        if (player.grasps[num27] == null)
                        {
                            if (player.pickUpCandidate is Creature)
                            {
                                player.room.PlaySound(SoundID.Slugcat_Pick_Up_Creature, player.pickUpCandidate.firstChunk, false, 1f, 1f);
                            }
                            else if (player.pickUpCandidate is PlayerCarryableItem)
                            {
                                for (int num28 = 0; num28 < player.pickUpCandidate.grabbedBy.Count; num28++)
                                {
                                    if (player.pickUpCandidate.grabbedBy[num28].grabber.room == player.pickUpCandidate.grabbedBy[num28].grabbed.room)
                                    {
                                        player.pickUpCandidate.grabbedBy[num28].grabber.GrabbedObjectSnatched(player.pickUpCandidate.grabbedBy[num28].grabbed, player);
                                    }
                                    else
                                    {
                                        /*Custom.Warning(new string[]
                                        {
                                        string.Format("Item theft room mismatch? {0}", player.pickUpCandidate.grabbedBy[num28].grabbed.abstractPhysicalObject)
                                        });*/
                                    }
                                    player.pickUpCandidate.grabbedBy[num28].grabber.ReleaseGrasp(player.pickUpCandidate.grabbedBy[num28].graspUsed);
                                }
                                (player.pickUpCandidate as PlayerCarryableItem).PickedUp(player);
                            }
                            else
                            {
                                player.room.PlaySound(SoundID.Slugcat_Pick_Up_Misc_Inanimate, player.pickUpCandidate.firstChunk, false, 1f, 1f);
                            }
                            player.SlugcatGrab(player.pickUpCandidate, num27);
                            if (player.pickUpCandidate.graphicsModule != null && player.Grabability(player.pickUpCandidate) < (Player.ObjectGrabability)5)
                            {
                                player.pickUpCandidate.graphicsModule.BringSpritesToFront();
                                break;
                            }
                            break;
                        }
                        else
                        {
                            num27++;
                        }
                    }
                }
                player.wantToPickUp = 0;
            }
        }
    }

    private void BigNeedleWorm_Swish(On.BigNeedleWorm.orig_Swish orig, BigNeedleWorm self)
    {
        self.flying = 0f;
        self.lookDir = self.swishDir.Value;
        self.dodgeDelay = 30;
        self.swishCounter--;
        if (self.swishCounter < 1 || self.swishDir == null)
        {
            for (int i = 0; i < self.TotalSegments; i++)
            {
                self.SetSegmentVel(i, Vector2.ClampMagnitude(self.GetSegmentVel(i) * 0.75f, 20f));
            }
            self.swishCounter = 0;
            self.swishDir = null;
            self.lameCounter = 7;
            return;
        }
        float num = 90f + 90f * Mathf.Sin(Mathf.InverseLerp(1f, 5f, (float)self.swishCounter) * 3.1415927f);
        Vector2 value = self.swishDir.Value;
        Vector2 vector = self.bodyChunks[0].pos + value * self.fangLength;
        Vector2 vector2 = self.bodyChunks[0].pos + value * (self.fangLength + num);
        Vector2 vector3 = self.bodyChunks[0].lastPos + value * self.fangLength;
        Vector2 vector4 = self.bodyChunks[0].pos + value * (self.fangLength + num);
        FloatRect? floatRect = SharedPhysics.ExactTerrainRayTrace(self.room, vector3, vector4);
        Vector2 vector5 = default;
        if (floatRect != null)
        {
            vector5 = new Vector2(floatRect.Value.left, floatRect.Value.bottom);
        }
        Vector2 vector6 = vector4;
        SharedPhysics.CollisionResult collisionResult = SharedPhysics.TraceProjectileAgainstBodyChunks(self, self.room, vector3, ref vector6, 1f, 1, self, false);
        if (floatRect != null && collisionResult.chunk != null)
        {
            if (Vector2.Distance(vector3, vector5) < Vector2.Distance(vector3, collisionResult.collisionPoint))
            {
                collisionResult.chunk = null;
            }
            else
            {
                floatRect = null;
            }
        }
        if (floatRect == null && collisionResult.chunk != null)
        {
            vector2 = collisionResult.collisionPoint - value * self.fangLength * 0.7f;
            self.stuckDir = Vector3.Slerp(self.swishDir.Value, Custom.DirVec(vector2, collisionResult.chunk.pos), 0.4f);
            self.swishCounter = 0;
            self.swishDir = null;
            self.impaleChunk = collisionResult.chunk;
            float num2 = -self.fangLength / 4f;
            for (int k = 0; k < self.impaleDistances.GetLength(0); k++)
            {
                if (k == 1)
                {
                    num2 += self.fangLength / 4f;
                }
                if (k > 0)
                {
                    num2 += self.GetSegmentRadForRopeLength(k - 1) + self.GetSegmentRadForRopeLength(k) + self.fangLength / 4f;
                }
                self.impaleDistances[k, 0] = Vector2.Distance(vector2 - value * num2, self.impaleChunk.pos);
                if (self.impaleChunk.rotationChunk != null)
                {
                    self.impaleDistances[k, 1] = Vector2.Distance(vector2 - value * num2, self.impaleChunk.rotationChunk.pos);
                }
            }
            if (self.impaleChunk.owner is Creature)
            {
                if (self.impaleChunk.owner is Player)
                {
                    Player player = self.impaleChunk.owner as Player;
                    if (
                            !player.dead
                            && (
                                (player.grasps[0] != null && player.grasps[0].grabbed.abstractPhysicalObject.realizedObject is items.InflatableGlowingShield && (self.bodyChunks[0].pos - self.impaleChunk.pos).x < 0)
                                || (player.grasps[1] != null && player.grasps[1].grabbed.abstractPhysicalObject.realizedObject is items.InflatableGlowingShield && (self.bodyChunks[0].pos - self.impaleChunk.pos).x > 0)
                            )
                        )
                    {
                        self.LoseAllGrasps();
                        return;
                    }
                }
                (self.impaleChunk.owner as Creature).Violence(self.mainBodyChunk, null, self.impaleChunk, null, Creature.DamageType.Stab, 1.22f, 60f);
            }
            self.impaleChunk.vel += value * 12f / self.impaleChunk.mass;
            self.impaleChunk.pos += value * 7f / self.impaleChunk.mass;
            for (int l = 0; l < self.TotalSegments; l++)
            {
                self.SetSegmentVel(l, Vector2.ClampMagnitude(self.GetSegmentVel(l), 6f));
            }
            self.room.PlaySound(SoundID.Big_Needle_Worm_Impale_Creature, vector2);
            self.stuckTime = 0f;
            return;
        }
        orig(self);
    }

    private void KingTusksTusk_ShootUpdate(On.KingTusks.Tusk.orig_ShootUpdate orig, KingTusks.Tusk self, float speed)
    {
        float num = 20f;
        Vector2 vector = self.chunkPoints[0, 0] + self.shootDir * num;
        Vector2 vector2 = self.chunkPoints[0, 0] + self.shootDir * (num + speed);
        FloatRect? floatRect = SharedPhysics.ExactTerrainRayTrace(self.room, vector, vector2);
        Vector2 vector3 = default;
        if (floatRect != null)
        {
            vector3 = new Vector2(floatRect.Value.left, floatRect.Value.bottom);
        }
        Vector2 vector4 = vector2;
        SharedPhysics.CollisionResult collisionResult = SharedPhysics.TraceProjectileAgainstBodyChunks(self, self.room, vector, ref vector4, 5f, 1, self.owner.vulture, false);
        if (floatRect != null && collisionResult.chunk != null)
        {
            if (Vector2.Distance(vector, vector3) < Vector2.Distance(vector, collisionResult.collisionPoint))
            {
                collisionResult.chunk = null;
            }
            else
            {
                floatRect = null;
            }
        }
        if (floatRect == null && collisionResult.chunk != null)
        {
            vector2 = collisionResult.collisionPoint - self.shootDir * num * 0.7f;
            self.chunkPoints[0, 0] = vector2 - self.shootDir * num;
            self.chunkPoints[1, 0] = vector2 - self.shootDir * (num + KingTusks.Tusk.length);
            if (self.room.BeingViewed)
            {
                for (int k = 0; k < 6; k++)
                {
                    if (UnityEngine.Random.value < Mathf.InverseLerp(0f, 0.5f, self.room.roomSettings.CeilingDrips))
                    {
                        self.room.AddObject(new WaterDrip(collisionResult.collisionPoint, -self.shootDir * Mathf.Lerp(5f, 15f, UnityEngine.Random.value) + Custom.RNV() * UnityEngine.Random.value * 10f, false));
                    }
                }
            }
            self.SwitchMode(KingTusks.Tusk.Mode.StuckInCreature);
            self.room.PlaySound(SoundID.King_Vulture_Tusk_Impale_Creature, vector2);
            self.impaleChunk = collisionResult.chunk;
            self.impaleChunk.vel += self.shootDir * 12f / self.impaleChunk.mass;
            self.impaleChunk.vel = Vector2.ClampMagnitude(self.impaleChunk.vel, 50f);
            if (self.impaleChunk.owner is Creature)
            {
                if (self.impaleChunk.owner is Player)
                {
                    Player player = self.impaleChunk.owner as Player;
                    if (
                            (player.grasps[0] != null && player.grasps[0].grabbed.abstractPhysicalObject.realizedObject is items.InflatableGlowingShield && (self.chunkPoints[0, 0] - self.impaleChunk.pos).x < 0)
                            || (player.grasps[1] != null && player.grasps[1].grabbed.abstractPhysicalObject.realizedObject is items.InflatableGlowingShield && (self.chunkPoints[0, 0] - self.impaleChunk.pos).x > 0)
                        )
                    {
                        self.SwitchMode(KingTusks.Tusk.Mode.Dangling);
                        return;
                    }
                }
                (self.impaleChunk.owner as Creature).Violence(null, null, self.impaleChunk, null, Creature.DamageType.Stab, 1.5f, 0f);
            }
            self.shootDir = Vector3.Slerp(self.shootDir, Custom.DirVec(vector2, self.impaleChunk.pos), 0.4f);
            if (self.impaleChunk.rotationChunk != null)
            {
                self.shootDir = Custom.RotateAroundOrigo(self.shootDir, -Custom.AimFromOneVectorToAnother(self.impaleChunk.pos, self.impaleChunk.rotationChunk.pos));
            }
            if (self.impaleChunk.owner.graphicsModule != null)
            {
                self.impaleChunk.owner.graphicsModule.BringSpritesToFront();
            }
            return;
        }
        orig(self, speed);
    }
} 

public static class ScientistLogger
{
    public static readonly bool Enabled = true;

    public static void Log(string msg, bool getCallChain = false)
    {
        if (!Enabled) { return; }
        UnityEngine.Debug.Log($" [Scientist]: {msg}");
        if (getCallChain)
        {
            ScientistLogger.Log("以下为调用信息: ");
            GetMethodInfo(1);
        }
    }

    public static void Warning(string msg, bool getCallChain = false)
    {
        if (!Enabled) { return; }
        UnityEngine.Debug.LogWarning($" [Scientist]: {msg}");
        if (getCallChain)
        {
            ScientistLogger.Warning("以下为调用信息: ");
            GetMethodInfo(1);
        }
    }

    public static void Error(string msg, bool getCallChain = false)
    {
        if (!Enabled) { return; }
        UnityEngine.Debug.LogError($" [Scientist]: {msg}");
        if (getCallChain)
        {
            ScientistLogger.Error("以下为调用信息: ");
            GetMethodInfo(1);
        }
    }

    public static void GetMethodInfo(int index)
    {
        index++;
        var stack = new StackTrace(true);

        //0是本身，1是调用方，2是调用方的调用方...以此类推
        var method = stack.GetFrame(index).GetMethod();

        var dataList = new Dictionary<string, string>();
        var module = method.Module;
        dataList.Add("模块", module.Name);
        var deClearType = method.DeclaringType;
        dataList.Add("命名空间", deClearType.Namespace);
        dataList.Add("类名", deClearType.Name);
        dataList.Add("完整类名", deClearType.FullName);
        dataList.Add("方法名", method.Name);
        dataList.Add("行数", stack.GetFrame(index).GetFileLineNumber().ToString());
        var stackFrames = stack.GetFrames();
        dataList.Add("调用链", string.Join("\n -> ", stackFrames.Select((r, i) =>
        {
            if (i == 0) return null;
            var m = r.GetMethod();
            return $"{m.DeclaringType.FullName}.{m.Name} Line {r.GetFileLineNumber()}";
        }).Where(r => !string.IsNullOrWhiteSpace(r)).Reverse()));
        foreach (var item in dataList)
        {
            Console.WriteLine($"{item.Key}：{item.Value}");
        }
    }
}

/*
        // Implement MeanLizards-实现激怒蜥蜴的效果
        private void Lizard_ctor(On.Lizard.orig_ctor orig, Lizard self, AbstractCreature abstractCreature, World world)
        {
            orig(self, abstractCreature, world);

            if(MeanLizards.TryGet(world.game, out float meanness))
            {
                self.spawnDataEvil = Mathf.Min(self.spawnDataEvil, meanness);
            }
        }


        // Implement SuperJump-实现超高跳跃的效果
        private void Player_Jump(On.Player.orig_Jump orig, Player self)
        {
            orig(self);//总不能挂完钩子把原本该执行的东西给弄丢吧 这一句就是为了再把它塞进来让它正常运行

            if (SuperJump.TryGet(self, out var power))
            {
                self.jumpBoost *= 1f + power;
            }
        }

        // Implement ExlodeOnDeath-实现死亡自爆效果
        private void Player_Die(On.Player.orig_Die orig, Player self)
        {
            bool wasDead = self.dead;
            //布尔值wasDead判断玩家是否死亡

            orig(self);

            if(!wasDead && self.dead
                && ExplodeOnDeath.TryGet(self, out bool explode)
                && explode)
            {
                // Adapted from ScavengerBomb.Explode-改编自ScavengerBomb.Explode，即拾荒者炸弹的爆炸效果
                var room = self.room;
                var pos = self.mainBodyChunk.pos;
                var color = self.ShortCutColor();
                //这三行分别获取了房间 身体位置和ShortCutColor，也就是这个生物通过管道时显示的颜色
                room.AddObject(new Explosion(room, self, pos, 7, 250f, 6.2f, 2f, 280f, 0.25f, self, 0.7f, 160f, 1f));
                //在 当前房间 从自己身体 在当前身体的位置 生成一个 持续时长7 半径250，力度6.2，伤害2，眩晕280，致聋0.25，判定击杀由自己造成，伤害乘数0.7，最小眩晕160，背景噪声1的爆炸
                room.AddObject(new Explosion.ExplosionLight(pos, 280f, 1f, 7, color));
                //玩家位置 半径280，透明度1，持续时长7，发光颜色就是上面的shortcutcolor的爆炸光效
                room.AddObject(new Explosion.ExplosionLight(pos, 230f, 1f, 3, new Color(1f, 1f, 1f)));
                //和上面差不多，玩家位置 半径230，透明度1，持续时长3，发光颜色1f1f1f
                room.AddObject(new ExplosionSpikes(room, pos, 14, 30f, 9f, 7f, 170f, color));
                //自己解释下这玩意罢咱累了（

                room.AddObject(new ShockWave(pos, 330f, 0.045f, 5, false));
                //或许没那么累 在自身位置生成冲击波效果 大小330 强度0.045 时长5 false表示绘制顺序（绘制到HUD图层，True就是HUD2
                //思考一下 如果把持续时长改成180会怎么样
                //再想想如何让这玩意影响面积更大（？

                room.ScreenMovement(pos, default, 1.3f);
                //屏幕震动
                room.PlaySound(SoundID.Bomb_Explode, pos);
                //播放爆炸音效
                room.InGameNoise(new Noise.InGameNoise(pos, 9000f, self, 1f));
                //游戏内噪声效果
            }
        }
    }
}
*/