using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Scientist;
using UnityEngine;

namespace chats;
public class FivePebblesChats
{
    public static void FivePebbles_Update(On.SSOracleBehavior.orig_Update orig, SSOracleBehavior self, bool eu)
    {
        /*Console.WriteLine($"FivePebblesChats_Update  self = {self}, eu = {eu}");
        Console.WriteLine($"FivePebblesChats_Update  self.action = {self.action}");*/
        orig(self, eu);
    }

    public static void FivePebbles_InitateConversationInitiateConversation(On.SSOracleBehavior.orig_InitateConversation orig, SSOracleBehavior self, Conversation.ID convoId, SSOracleBehavior.ConversationBehavior convBehav)
    {
        Console.WriteLine($"FivePebblesChats_InitateConversation  self = {self}, convoId = {convoId}, convBehav = {convBehav}");
        foreach (Player player in self.PlayersInRoom)
        {
            if (player.slugcatStats.name.value == Scientist.Plugin.MOD_ID)
            {
                self.dialogBox.NewMessage(self.Translate("TEST_MESSAGE"), 60);
                self.dialogBox.Interrupt(self.Translate("我■■你很久了，■■的■■"), 60);
                self.dialogBox.NewMessage(self.Translate("你与你的同类不同，你格外的……聪明"), 60);
                self.dialogBox.NewMessage(self.Translate("你甚至已经理解了一些它们的基础文字，当然你可以把这些内容翻译给你的同类"), 60);
                self.dialogBox.NewMessage(self.Translate("自然演化一般来说不会诞生出像你这样的个体，但你的出现打破了这个概念"), 60);
                self.dialogBox.NewMessage(self.Translate("令我感到好奇"), 60);
                self.dialogBox.NewMessage(self.Translate("我不知道你的大脑为何会如此发达，但你现在来这里肯定是有些目的"), 60);
                self.dialogBox.NewMessage(self.Translate("如果你来这里是为了神经元的话，我不建议你深入我的设施里取"), 60);
                self.dialogBox.NewMessage(self.Translate("我的情况连我自己都说不上来有多糟糕，我不知道我还能支撑多久"), 60);
                self.dialogBox.NewMessage(self.Translate("让我想想"), 60);
                self.dialogBox.NewMessage(self.Translate("我动了点手脚，这应该能让你轻松一点"), 60);
                self.dialogBox.NewMessage(self.Translate("我会继续观察你的，但现在不管你有什么理由，离开"), 60);
            }
        }
    }

    public static void FivePebbles_AddEvents(On.SSOracleBehavior.PebblesConversation.orig_AddEvents orig, SSOracleBehavior.PebblesConversation self)
    {
        GetMethodInfo(6);
        Console.WriteLine($"FivePebblesChats_AddEvents  self = {self}");
        foreach (Player player in self.owner.PlayersInRoom)
        {
            if (player.slugcatStats.name.value == Scientist.Plugin.MOD_ID)
            { 
                self.dialogBox.NewMessage(self.Translate("我■■你很久了，■■的■■"), 60);
                self.dialogBox.NewMessage(self.Translate("你与你的同类不同，你格外的……聪明"), 60);
                self.dialogBox.NewMessage(self.Translate("你甚至已经理解了一些它们的基础文字，当然你可以把这些内容翻译给你的同类"), 60);
                self.dialogBox.NewMessage(self.Translate("自然演化一般来说不会诞生出像你这样的个体，但你的出现打破了这个概念"), 60);
                self.dialogBox.NewMessage(self.Translate("令我感到好奇"), 60);
                self.dialogBox.NewMessage(self.Translate("我不知道你的大脑为何会如此发达，但你现在来这里肯定是有些目的"), 60);
                self.dialogBox.NewMessage(self.Translate("如果你来这里是为了神经元的话，我不建议你深入我的设施里取"), 60);
                self.dialogBox.NewMessage(self.Translate("我的情况连我自己都说不上来有多糟糕，我不知道我还能支撑多久"), 60);
                self.dialogBox.NewMessage(self.Translate("让我想想"), 60);
                self.dialogBox.NewMessage(self.Translate("我动了点手脚，这应该能让你轻松一点"), 60);
                self.dialogBox.NewMessage(self.Translate("我会继续观察你的，但现在不管你有什么理由，离开"), 60);
                return;
            }
        }
        orig(self);
    }

    public static void GetMethodInfo(int index)
    {
        index++;//由于我是封装了方法，相当于上端想要获取本身，其实对于这里而言，上端的本身就是这里的上端，所以需要+1，以此类推
        var stack = new StackTrace(true);

        //0是本身，1是调用方，2是调用方的调用方...以此类推
        var method = stack.GetFrame(index).GetMethod();//想要获取关于方法的信息，可以自己断点调试这里

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

        /*dataList.ForeachLingbug(r =>
        {
            Console.WriteLine($"{r.Key}：{r.Value}");
        });*/

        foreach (var item in dataList)
        {
            Console.WriteLine($"{item.Key}：{item.Value}");
        }
    }
}

public class SSOracleMeetScientist : SSOracleBehavior.ConversationBehavior
{
    public static readonly SSOracleBehavior.SubBehavior.SubBehavID MeetScientist = new SSOracleBehavior.SubBehavior.SubBehavID("MeetScientist", true);
    public static readonly Conversation.ID Pebbles_Scientist = new Conversation.ID("Pebbles_Scientist", true);

    public SSOracleMeetScientist(SSOracleBehavior owner) : base(owner, MeetScientist, Pebbles_Scientist)
    {
        owner.getToWorking = 0f;
    }

    /*public override void Update()
    {
        if (base.player == null)
        {
            return;
        }
        this.owner.LockShortcuts();
        if (ModManager.MSC && (base.action == MoreSlugcatsEnums.SSOracleBehaviorAction.MeetWhite_ThirdCurious || base.action == MoreSlugcatsEnums.SSOracleBehaviorAction.MeetWhite_SecondImages))
        {
            Vector2 vector = base.oracle.room.MiddleOfTile(24, 14) - base.player.mainBodyChunk.pos;
            float num = Custom.Dist(base.oracle.room.MiddleOfTile(24, 14), base.player.mainBodyChunk.pos);
            base.player.mainBodyChunk.vel += Vector2.ClampMagnitude(vector, 40f) / 40f * Mathf.Clamp(16f - num / 100f * 16f, 4f, 16f);
            if (base.player.mainBodyChunk.vel.magnitude < 1f || num < 8f)
            {
                base.player.mainBodyChunk.vel = Vector2.zero;
                base.player.mainBodyChunk.HardSetPosition(base.oracle.room.MiddleOfTile(24, 14));
            }
        }
        if (base.action == SSOracleBehavior.Action.MeetWhite_Shocked)
        {
            this.owner.movementBehavior = SSOracleBehavior.MovementBehavior.KeepDistance;
            if (this.owner.oracle.room.game.manager.rainWorld.progression.miscProgressionData.redHasVisitedPebbles || this.owner.oracle.room.game.manager.rainWorld.options.validation)
            {
                if (base.inActionCounter > 40)
                {
                    this.owner.NewAction(SSOracleBehavior.Action.General_GiveMark);
                    this.owner.afterGiveMarkAction = SSOracleBehavior.Action.General_MarkTalk;
                    return;
                }
            }
            else if (this.owner.oracle.room.game.IsStorySession && this.owner.oracle.room.game.GetStorySession.saveState.deathPersistentSaveData.theMark)
            {
                if (base.inActionCounter > 40)
                {
                    this.owner.NewAction(SSOracleBehavior.Action.General_MarkTalk);
                    return;
                }
            }
            else if (base.inActionCounter > 120)
            {
                this.owner.NewAction(SSOracleBehavior.Action.MeetWhite_Curious);
                return;
            }
        }
        else if (base.action == SSOracleBehavior.Action.MeetWhite_Curious)
        {
            this.owner.movementBehavior = SSOracleBehavior.MovementBehavior.Investigate;
            if (base.inActionCounter > 360)
            {
                this.owner.NewAction(SSOracleBehavior.Action.MeetWhite_Talking);
                return;
            }
        }
        else if (base.action == SSOracleBehavior.Action.MeetWhite_Talking)
        {
            this.owner.movementBehavior = SSOracleBehavior.MovementBehavior.Talk;
            if (!this.CurrentlyCommunicating && this.communicationPause > 0)
            {
                this.communicationPause--;
            }
            if (!this.CurrentlyCommunicating && this.communicationPause < 1)
            {
                if (this.communicationIndex >= 4)
                {
                    this.owner.NewAction(SSOracleBehavior.Action.MeetWhite_Texting);
                }
                else if (this.owner.allStillCounter > 20)
                {
                    this.NextCommunication();
                }
            }
            if (!this.CurrentlyCommunicating)
            {
                this.owner.nextPos += Custom.RNV();
                return;
            }
        }
        else
        {
            if (base.action == SSOracleBehavior.Action.MeetWhite_Texting)
            {
                base.movementBehavior = SSOracleBehavior.MovementBehavior.ShowMedia;
                if (base.oracle.graphicsModule != null)
                {
                    (base.oracle.graphicsModule as OracleGraphics).halo.connectionsFireChance = 0f;
                }
                if (!this.CurrentlyCommunicating && this.communicationPause > 0)
                {
                    this.communicationPause--;
                }
                if (!this.CurrentlyCommunicating && this.communicationPause < 1)
                {
                    if (this.communicationIndex >= 6 || (ModManager.MSC && this.owner.oracle.ID == MoreSlugcatsEnums.OracleID.DM && this.communicationIndex >= 4))
                    {
                        this.owner.NewAction(SSOracleBehavior.Action.MeetWhite_Images);
                    }
                    else if (this.owner.allStillCounter > 20)
                    {
                        this.NextCommunication();
                    }
                }
                this.chatLabel.setPos = new Vector2?(this.showMediaPos);
                return;
            }
            if (base.action == SSOracleBehavior.Action.MeetWhite_Images || (ModManager.MSC && base.action == MoreSlugcatsEnums.SSOracleBehaviorAction.MeetWhite_SecondImages))
            {
                base.movementBehavior = SSOracleBehavior.MovementBehavior.ShowMedia;
                if (this.communicationPause > 0)
                {
                    this.communicationPause--;
                }
                if (ModManager.MSC && base.action == MoreSlugcatsEnums.SSOracleBehaviorAction.MeetWhite_SecondImages)
                {
                    this.myProjectionCircle.pos = new Vector2(base.player.mainBodyChunk.pos.x - 10f, base.player.mainBodyChunk.pos.y);
                }
                if (base.inActionCounter > 150 && this.communicationPause < 1)
                {
                    if (base.action == SSOracleBehavior.Action.MeetWhite_Images && (this.communicationIndex >= 3 || (ModManager.MSC && this.owner.oracle.ID == MoreSlugcatsEnums.OracleID.DM && this.communicationIndex >= 1)))
                    {
                        this.owner.NewAction(SSOracleBehavior.Action.MeetWhite_SecondCurious);
                    }
                    else if (ModManager.MSC && base.action == MoreSlugcatsEnums.SSOracleBehaviorAction.MeetWhite_SecondImages && this.communicationIndex >= 2)
                    {
                        this.owner.NewAction(MoreSlugcatsEnums.SSOracleBehaviorAction.MeetWhite_StartDialog);
                    }
                    else
                    {
                        this.NextCommunication();
                    }
                }
                if (this.showImage != null)
                {
                    this.showImage.setPos = new Vector2?(this.showMediaPos);
                }
                if (Random.value < 0.033333335f)
                {
                    this.idealShowMediaPos += Custom.RNV() * Random.value * 30f;
                    this.showMediaPos += Custom.RNV() * Random.value * 30f;
                    return;
                }
            }
            else if (base.action == SSOracleBehavior.Action.MeetWhite_SecondCurious)
            {
                base.movementBehavior = SSOracleBehavior.MovementBehavior.Investigate;
                if (base.inActionCounter == 80)
                {
                    Custom.Log(new string[]
                    {
                            "extra talk"
                    });
                    if (ModManager.MSC && this.owner.oracle.ID == MoreSlugcatsEnums.OracleID.DM)
                    {
                        this.voice = base.oracle.room.PlaySound(SoundID.SL_AI_Talk_5, base.oracle.firstChunk);
                    }
                    else
                    {
                        this.voice = base.oracle.room.PlaySound(SoundID.SS_AI_Talk_5, base.oracle.firstChunk);
                    }
                    this.voice.requireActiveUpkeep = true;
                }
                if (base.inActionCounter > 240)
                {
                    this.owner.NewAction(SSOracleBehavior.Action.General_GiveMark);
                    this.owner.afterGiveMarkAction = SSOracleBehavior.Action.General_MarkTalk;
                    return;
                }
            }
            else if (base.action == SSOracleBehavior.Action.General_MarkTalk)
            {
                base.movementBehavior = SSOracleBehavior.MovementBehavior.Talk;
                if (this.owner.conversation != null && this.owner.conversation.id == this.convoID && this.owner.conversation.slatedForDeletion)
                {
                    this.owner.conversation = null;
                    this.owner.NewAction(SSOracleBehavior.Action.ThrowOut_ThrowOut);
                    return;
                }
            }
            else if (ModManager.MSC && base.action == MoreSlugcatsEnums.SSOracleBehaviorAction.MeetWhite_ThirdCurious)
            {
                this.owner.movementBehavior = SSOracleBehavior.MovementBehavior.Investigate;
                if (base.inActionCounter % 180 == 1)
                {
                    this.owner.investigateAngle = Random.value * 360f;
                }
                if (base.inActionCounter == 180)
                {
                    base.dialogBox.NewMessage(base.Translate("Hello there."), 0);
                    base.dialogBox.NewMessage(base.Translate("Are my words reaching you?"), 0);
                }
                if (base.inActionCounter == 460)
                {
                    this.myProjectionCircle = new ProjectionCircle(base.player.mainBodyChunk.pos, 0f, 3f);
                    base.oracle.room.AddObject(this.myProjectionCircle);
                }
                if (base.inActionCounter > 460)
                {
                    float num2 = Mathf.Lerp(0f, 1f, ((float)base.inActionCounter - 460f) / 150f);
                    this.myProjectionCircle.radius = 18f * Mathf.Clamp(num2 * 2f, 0f, 1f);
                    this.myProjectionCircle.pos = new Vector2(base.player.mainBodyChunk.pos.x - 10f, base.player.mainBodyChunk.pos.y);
                    if (base.oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiConversationsHad == 0)
                    {
                        (base.player.graphicsModule as PlayerGraphics).bodyPearl.visible = true;
                        (base.player.graphicsModule as PlayerGraphics).bodyPearl.globalAlpha = num2;
                    }
                }
                if (base.inActionCounter > 770)
                {
                    this.owner.NewAction(MoreSlugcatsEnums.SSOracleBehaviorAction.MeetWhite_SecondImages);
                    return;
                }
            }
            else if (ModManager.MSC && base.action == MoreSlugcatsEnums.SSOracleBehaviorAction.MeetWhite_StartDialog)
            {
                if (base.inActionCounter < 48)
                {
                    base.player.mainBodyChunk.vel += Vector2.ClampMagnitude(base.oracle.room.MiddleOfTile(24, 14) - base.player.mainBodyChunk.pos, 40f) / 40f * (6f - (float)base.inActionCounter / 8f);
                    float num3 = 1f - (float)base.inActionCounter / 48f;
                    if (base.oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiConversationsHad == 0)
                    {
                        (base.player.graphicsModule as PlayerGraphics).bodyPearl.globalAlpha = num3;
                    }
                    this.myProjectionCircle.radius = 18f * Mathf.Clamp(num3 * 2f, 0f, 3f);
                    this.myProjectionCircle.pos = new Vector2(base.player.mainBodyChunk.pos.x - 10f, base.player.mainBodyChunk.pos.y);
                }
                if (base.inActionCounter == 48)
                {
                    this.myProjectionCircle.Destroy();
                    (base.player.graphicsModule as PlayerGraphics).bodyPearl.visible = false;
                }
                if (base.inActionCounter == 180)
                {
                    SLOrcacleState sloracleState = base.oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SLOracleState;
                    int playerEncounters = sloracleState.playerEncounters;
                    sloracleState.playerEncounters = playerEncounters + 1;
                    this.owner.NewAction(MoreSlugcatsEnums.SSOracleBehaviorAction.Moon_AfterGiveMark);
                }
            }
        }
    }*/
}