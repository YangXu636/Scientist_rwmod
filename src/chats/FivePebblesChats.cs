using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MoreSlugcats;
using Scientist;
using UnityEngine;

namespace chats;
public class FivePebblesChats
{
    public static void FivePebbles_SeePlayer(On.SSOracleBehavior.orig_SeePlayer orig, SSOracleBehavior self)
    {
        if (self.oracle.room.game.StoryCharacter.value == Scientist.Plugin.MOD_ID)
        {
            if (self.oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiConversationsHad == 0 || (ModManager.MSC && self.oracle.ID == MoreSlugcatsEnums.OracleID.DM))
            {
                self.NewAction(MoreSlugcatsEnums.SSOracleBehaviorAction.MeetGourmand_Init);
                if (!ModManager.MSC || self.oracle.ID != MoreSlugcatsEnums.OracleID.DM)
                {
                    self.SlugcatEnterRoomReaction();
                }
            }
            else
            {
                if (self.oracle.room.game.GetStorySession.saveStateNumber.value == Scientist.Plugin.MOD_ID)
                {
                    self.NewAction(MoreSlugcatsEnums.SSOracleBehaviorAction.Pebbles_SlumberParty);
                    self.oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiThrowOuts++;
                }
            }
            self.oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiConversationsHad++;
        }
        else
        {
            orig(self);
        }
    }

    public static void FivePebbles_NewAction(On.SSOracleBehavior.orig_NewAction orig, SSOracleBehavior self, SSOracleBehavior.Action nextAction)
    {
        if (self.oracle.room.game.StoryCharacter.value == Scientist.Plugin.MOD_ID)
        {
            if (nextAction == self.action)
            {
                return;
            }

            SSOracleBehavior.SubBehavior.SubBehavID subBehavID = SSOracleBehavior.SubBehavior.SubBehavID.General;
            if (nextAction ==  Scientist.Register.MeetScientist_Init)
            {
                subBehavID = Scientist.Register.MeetScientist;
            }
            else if (nextAction == SSOracleBehavior.Action.ThrowOut_SecondThrowOut || nextAction == SSOracleBehavior.Action.ThrowOut_ThrowOut || nextAction == SSOracleBehavior.Action.ThrowOut_Polite_ThrowOut)
            {
                subBehavID = SSOracleBehavior.SubBehavior.SubBehavID.ThrowOut;
            }
            else if (ModManager.MSC && nextAction == MoreSlugcatsEnums.SSOracleBehaviorAction.ThrowOut_Singularity)
            {
                subBehavID = SSOracleBehavior.SubBehavior.SubBehavID.ThrowOut;
            }
            else
            {
                subBehavID = SSOracleBehavior.SubBehavior.SubBehavID.General;
            }

            self.currSubBehavior.NewAction(self.action, nextAction);

            if (subBehavID != SSOracleBehavior.SubBehavior.SubBehavID.General && subBehavID != self.currSubBehavior.ID)
            {
                SSOracleBehavior.SubBehavior subBehavior = null;
                for (int i = 0; i < self.allSubBehaviors.Count; i++)
                {
                    if (self.allSubBehaviors[i].ID == subBehavID)
                    {
                        subBehavior = self.allSubBehaviors[i];
                        break;
                    }
                }
                if (subBehavior == null)
                {
                    self.LockShortcuts();
                    if (subBehavID == Scientist.Register.MeetScientist)
                    {
                        subBehavior = new chats.SSOracleMeetScientist(self);
                    }
                    else if (subBehavID == SSOracleBehavior.SubBehavior.SubBehavID.ThrowOut)
                    {
                        subBehavior = new SSOracleBehavior.ThrowOutBehavior(self);
                    }
                    self.allSubBehaviors.Add(subBehavior);
                }
                subBehavior.Activate(self.action, nextAction);
                self.currSubBehavior.Deactivate();
                self.currSubBehavior = subBehavior;
            }
            self.inActionCounter = 0;
            self.action = nextAction;
        }
        else
        {
            orig(self, nextAction);
        }
    }

    public static void FivePebbles_Update(On.SSOracleBehavior.orig_Update orig, SSOracleBehavior self, bool eu)
    {
        /*Console.WriteLine($"FivePebblesChats_Update  self = {self}, eu = {eu}");
        Console.WriteLine($"FivePebblesChats_Update  self.action = {self.action}");*/
        orig(self, eu);
    }

    public static void FivePebbles_InitateConversationInitiateConversation(On.SSOracleBehavior.orig_InitateConversation orig, SSOracleBehavior self, Conversation.ID convoId, SSOracleBehavior.ConversationBehavior convBehav)
    {
        Console.WriteLine($"FivePebblesChats_InitateConversation  self = {self}, convoId = {convoId}, convBehav = {convBehav}");
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
    }
}

public class SSOracleMeetScientist : SSOracleBehavior.ConversationBehavior
{

    public SSOracleMeetScientist(SSOracleBehavior owner) : base(owner, Scientist.Register.MeetScientist, Scientist.Register.Pebbles_Scientist)
    {
        owner.getToWorking = 0f;
        if (ModManager.MMF && owner.oracle.room.game.IsStorySession && owner.oracle.room.game.GetStorySession.saveState.miscWorldSaveData.memoryArraysFrolicked && base.oracle.room.world.rainCycle.timer > base.oracle.room.world.rainCycle.cycleLength / 4)
        {
            base.oracle.room.world.rainCycle.timer = base.oracle.room.world.rainCycle.cycleLength / 4;
            base.oracle.room.world.rainCycle.dayNightCounter = 0;
        }
    }

    public override void Update()
    {
        base.Update();
        this.owner.LockShortcuts();
        if (base.action == Scientist.Register.MeetScientist_Init)
        {
            if (this.owner.playerEnteredWithMark)
            {
                this.owner.NewAction(SSOracleBehavior.Action.General_MarkTalk);
                return;
            }
            this.owner.NewAction(SSOracleBehavior.Action.General_GiveMark);
            this.owner.afterGiveMarkAction = SSOracleBehavior.Action.General_MarkTalk;
            return;
            
        }
        else if (base.action == SSOracleBehavior.Action.General_MarkTalk)
        {
            base.movementBehavior = SSOracleBehavior.MovementBehavior.Talk;
            if (base.inActionCounter == 15 && (this.owner.conversation == null || this.owner.conversation.id != this.convoID))
            {
                this.owner.InitateConversation(this.convoID, this);
            }
            if (this.owner.conversation != null && this.owner.conversation.id == this.convoID && this.owner.conversation.slatedForDeletion)
            {
                this.owner.conversation = null;
                this.owner.NewAction(SSOracleBehavior.Action.ThrowOut_ThrowOut);
            }
        }
    }
}