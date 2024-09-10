using System;
using BepInEx;
using SlugBase.Features;
using static SlugBase.Features.FeatureTypes;
using items;
using RWCustom;
using Expedition;
using MoreSlugcats;
using UnityEngine;
using Fisobs;
using Fisobs.Core;
using items.AbstractPhysicalObjects;

// TODO 属性比起黄猫还要略低
// TODO 随身携带一只拾荒者精英保镖 已废除
// TODO 比起普通的阔鱼猫有着更多的手指
// TODO 可以骑乘在拾荒者上以操控
// TODO 不可以吃肉 OK!
// TODO 能从特定生物的尸体或物品中获得素材和灵感，每触摸到一个新的物品就可以获得相关敏感和制造物品
// TODO 初始自带三个制造：尖矛、绳矛、尖绳矛


namespace Scientist
{
    [BepInPlugin(MOD_ID, "Scientist", "0.1.0")]
    class Plugin : BaseUnityPlugin
    {
        private const string MOD_ID = "xuyangjerry.Scientist";

        /*public static readonly PlayerFeature<float> SuperJump = PlayerFloat("scientist/super_jump");
        public static readonly PlayerFeature<bool> ExplodeOnDeath = PlayerBool("scientist/explode_on_death");
        public static readonly GameFeature<float> MeanLizards = GameFloat("scientist/mean_lizards");*/
        public static readonly PlayerFeature<bool> IsScientist = PlayerBool("scientist/is_scientist");
        public static readonly PlayerFeature<bool> Power = PlayerBool("temp/power");
        public static readonly PlayerFeature<float> LowerJump = PlayerFloat("scientist/lower_jump");

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

            //On.Room.AddObject += RoomAddObject;
            //On.AbstractRoom.AddEntity += AbstractRoomAddEntity;

            // Put your custom hooks here!-在此放置你自己的钩子
            On.Player.Jump += PlayerJump;
            //在玩家触发跳跃时执行Player_Jump
            //On.Player.Die += Player_Die;
            //On.Lizard.ctor += Lizard_ctor;

            // On.Player.CanIPickThisUp += ;

            On.Player.Update += PlayerUpdate;
            On.Player.GrabUpdate += PlayerGrabUpdate;  //在玩家触发拾取时执行PlayerGrabUpdate
            On.Player.ThrowObject += PlayerThrowObject;
            On.Player.ThrownSpear += PlayerThrownSpear;
        }

        private void AbstractRoomAddEntity(On.AbstractRoom.orig_AddEntity orig, AbstractRoom self, AbstractWorldEntity ent)
        {
            orig(self, ent);
        }

        public void RoomAddObject(On.Room.orig_AddObject orig, Room self, UpdatableAndDeletable obj)
        {
            orig(self, obj);
        }

        // Load any resources, such as sprites or sounds-加载任何资源 包括图像素材和音效
        private void LoadResources(RainWorld rainWorld)
        {
            
        }

        public void PlayerUpdate(On.Player.orig_Update orig, Player self, bool eu) 
        {
            orig(self, eu);
            if (IsScientist.TryGet(self, out var isscientist) && isscientist)
            {
                if (self.input[0].pckp && self.room != null)
                {
                    /*RainWorldGame rainWorldGame = self.room.game.rainWorld.processManager.currentMainLoop as RainWorldGame;
                    IntVector2 tilePosition = self.room.game.cameras[0].room.GetTilePosition(new Vector2(300f, 300f) + rainWorldGame.cameras[0].pos);
                    WorldCoordinate worldCoordinate = self.room.game.cameras[0].room.GetWorldCoordinate(tilePosition);
                    AbstractPhysicalObject abstractPhysicalObject = new AbstractPhysicalObject(self.room.world, items.ShapeSpears.ShapeSpearFisob.ShapeSpear, null, worldCoordinate, self.room.game.GetNewID());
                    self.room.AddObject(new items.ShapeSpears.ShapeSpear(abstractPhysicalObject, self.room.world, new Vector2(0f, 0f)));*/
                }
            }
        }

        public void PlayerThrowObject(On.Player.orig_ThrowObject orig, Player self, int grasp, bool eu)
        {
            Console.WriteLine($"PlayerThrown  {self.grasps[grasp]}");
            orig(self, grasp, eu);
        }

        public void PlayerThrownSpear(On.Player.orig_ThrownSpear orig, Player self, Spear spear)
        {
            orig(self, spear);
            Console.WriteLine($"PlayerThrown  {spear}");
            if (IsScientist.TryGet(self, out var isscientist) && isscientist && Power.TryGet(self, out var power) && power)
            {
                spear.spearDamageBonus = 10000f;
            }
        }

        public void PlayerJump(On.Player.orig_Jump orig, Player self)
        {
            orig(self);
            if (IsScientist.TryGet(self, out var isscientist) && isscientist)
            {
                if (LowerJump.TryGet(self, out float power))
                {
                    self.jumpBoost *= power;
                }
            }
            
        }

        private void RainWorld_OnModsEnabled(On.RainWorld.orig_OnModsEnabled orig, RainWorld self, ModManager.Mod[] newlyEnabledMods)
        {
            //虽然原来的方法是空的，但其他模组也可以勾选这个方法，所以这里也需要orig
            orig(self, newlyEnabledMods);
            Console.WriteLine("Scientist is coming!");
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
            Console.WriteLine($"self = {self}    type = {self.type}");
            if (self.type == AbstractPhysicalObject.AbstractObjectType.Spear && UnityEngine.Random.Range(0, 100) < 50)
            {
                print("realize shape spear");
                self.realizedObject = new items.ShapeSpear(self, self.world);
            }
            orig(self);
            if (self.type == Register.ShapeSpear)
            {
                print("realize shape spear!");
                self.realizedObject = new items.ShapeSpear(self, self.world);
            } //与任何物理对象一样，您的对象将采用抽象对象作为参数。稍后将对此进行更详细的说明
        }

        private Player.ObjectGrabability Player_Grabability(On.Player.orig_Grabability orig, Player self, PhysicalObject obj)
        {
            //您还可以指定抓取对象的不同选项的条件。例如，根据物品的重量
            if (obj is items.ShapeSpear)
                return Player.ObjectGrabability.OneHand;
            return orig(self, obj);
        }

        public void PlayerGrabUpdate(On.Player.orig_GrabUpdate orig, Player self, bool eu)
        {
            if (IsScientist.TryGet(self, out var isscientist) && isscientist) {     //判断是否为科学家，如果是，则执行修改后的GrabUpdate
                ScientistGrabUpdate(self, eu);
            }
            else {                                                                  //不是科学家，则执行原函数（这也会导致其他mod的GrabUpdate钩子在科学家上无法生效）（既然叫科学家，那么能屏蔽外界干扰和更改是很正常的吧~）
                orig(self, eu); 
            }
        }

        public void SpitUpCraftedObject(Player player)
        {
            player.craftingTutorial = true;
            player.room.PlaySound(SoundID.Slugcat_Swallow_Item, player.mainBodyChunk);
            if (player.grasps.Length == 2)
            {
                AbstractPhysicalObject abstractPhysicalObjectOne = player.grasps[0].grabbed.abstractPhysicalObject;
                AbstractPhysicalObject abstractPhysicalObjectTwo = player.grasps[1].grabbed.abstractPhysicalObject;
                if ((abstractPhysicalObjectOne.type == AbstractPhysicalObject.AbstractObjectType.Spear && abstractPhysicalObjectTwo.type == AbstractPhysicalObject.AbstractObjectType.Rock)
                    || (abstractPhysicalObjectOne.type == AbstractPhysicalObject.AbstractObjectType.Rock && abstractPhysicalObjectTwo.type == AbstractPhysicalObject.AbstractObjectType.Spear))
                {
                    player.ReleaseGrasp(0);
                    player.ReleaseGrasp(1);
                    abstractPhysicalObjectOne.realizedObject.RemoveFromRoom();
                    abstractPhysicalObjectTwo.realizedObject.RemoveFromRoom();
                    player.room.abstractRoom.RemoveEntity(abstractPhysicalObjectOne);
                    player.room.abstractRoom.RemoveEntity(abstractPhysicalObjectTwo);
                    //player.SubtractFood(1);
                    items.AbstractPhysicalObjects.ShapeSpearAbstract abstractSpear = new ShapeSpearAbstract(player.room.world, null, player.abstractCreature.pos, player.room.game.GetNewID() ); //自定义的ShapeSpearAbstract，覆写了realizedObject
                    player.room.abstractRoom.AddEntity(abstractSpear);
                    abstractSpear.RealizeInRoom();
                    if (player.FreeHand() != -1)
                    {
                        player.SlugcatGrab(abstractSpear.realizedObject, player.FreeHand());
                    }
                    return;
                }
            }
            AbstractPhysicalObject abstractPhysicalObject2 = null;
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
            return (player.input[0].y == 1 && CraftingResults(player) != null);     //CraftingResults替换成自定义
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
                return Scientist.Register.ShapeSpear;
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
                    if (player.FoodInStomach < player.MaxFoodInStomach || player.grasps[num5].grabbed is KarmaFlower || player.grasps[num5].grabbed is Mushroom)
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
            if (ModManager.MSC && SlugcatStats.SlugcatCanMaul(player.SlugCatClass))
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
                    else if (player.objectInStomach == null && player.swallowAndRegurgitateCounter > 90)
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
                                            /*Custom.LogWarning(new string[]
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