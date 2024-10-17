using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using RWCustom;
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
                self.NewAction(Scientist.ScientistEnums.Action_Fp.MeetScientist_Init);
                if (!ModManager.MSC || self.oracle.ID != MoreSlugcatsEnums.OracleID.DM)
                {
                    self.SlugcatEnterRoomReaction();
                }
            }
            else if (self.oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiConversationsHad >= 1 &&(!ModManager.MSC || (ModManager.MSC && self.oracle.ID != MoreSlugcatsEnums.OracleID.DM)))
            {
                Player[] players = self.oracle.room.PlayersInRoom.ToArray();
                PhysicalObject[][] posWithPlayers = players.Select( p => new PhysicalObject[]{p.grasps?[0]?.grabbed, p.grasps?[1]?.grabbed}).Where(p => p != null).ToArray();
                PhysicalObject[] poS = posWithPlayers.SelectMany(p => p).Where(p => p != null && p.abstractPhysicalObject.type != AbstractPhysicalObject.AbstractObjectType.PebblesPearl).ToArray();
                string posWithPlayersToString = posWithPlayers.SelectMany(innerList => innerList)
                                                   .Select(item => item == null ? "null" : item.ToString())
                                                   .Aggregate((current, next) => current + ", " + next);
                string poSToString = string.Join(", ", poS.Select(item => item == null ? "null" : item.ToString()));
                ScientistLogger.Log($"posWithPlayersToString: {posWithPlayersToString}, poSToString: {poSToString}");
                if (poS == null || poS.Length == 0)
                {
                    self.NewAction(Scientist.ScientistEnums.Action_Fp.MeetScientist_ThrowOut_First);
                }
                if (!ModManager.MSC || self.oracle.ID != MoreSlugcatsEnums.OracleID.DM)
                {
                    self.SlugcatEnterRoomReaction();
                }
            }
            else
            {
                /*if (self.oracle.room.game.GetStorySession.saveStateNumber.value == Scientist.Plugin.MOD_ID)
                {
                    
                }*/
                self.NewAction(Scientist.ScientistEnums.Action_Fp.MeetScientist_ThrowOut_Second);
                self.oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiThrowOuts++;
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
            if (nextAction ==  Scientist.ScientistEnums.Action_Fp.MeetScientist_Init)
            {
                subBehavID = Scientist.ScientistEnums.SubBehavID_Fp.FirstMeetScientist;
            }
            else if (nextAction == Scientist.ScientistEnums.Action_Fp.MeetScientist_SeeObject)
            {

            }
            else if (nextAction == Scientist.ScientistEnums.Action_Fp.MeetScientist_ThrowOut_First || nextAction == Scientist.ScientistEnums.Action_Fp.MeetScientist_ThrowOut_Second)
            {
                subBehavID = Scientist.ScientistEnums.SubBehavID_Fp.ThrowOutScientist;
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
                    if (subBehavID == Scientist.ScientistEnums.SubBehavID_Fp.FirstMeetScientist)
                    {
                        subBehavior = new chats.SSOracleMeetScientist(self);
                    }
                    else if (subBehavID == Scientist.ScientistEnums.SubBehavID_Fp.ThrowOutScientist)
                    {
                        subBehavior = new chats.ThrowOutScientistBehavior(self);
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

    public static void FivePebbles_AddEvents(On.SSOracleBehavior.PebblesConversation.orig_AddEvents orig, SSOracleBehavior.PebblesConversation self)
    {

        orig(self);
        if (self.id == Scientist.ScientistEnums.ConversationID_Fp.Pebbles_Scientist_Meet_First)
        {
            self.dialogBox.NewMessage(self.Translate(".  .  ."), 0);
            self.events.Add(new Conversation.TextEvent(self, 40, self.Translate("Pebbles_Scientist_Meet_First_Line1"), 80));
            self.events.Add(new Conversation.TextEvent(self, 80, self.Translate("Pebbles_Scientist_Meet_First_Line2"), 80));
            self.events.Add(new Conversation.TextEvent(self, 80, self.Translate("Pebbles_Scientist_Meet_First_Line3"), 80));
            self.events.Add(new Conversation.TextEvent(self, 80, self.Translate("Pebbles_Scientist_Meet_First_Line4"), 80));
            self.events.Add(new Conversation.TextEvent(self, 40, self.Translate("Pebbles_Scientist_Meet_First_Line5"), 80));
            //self.events.Add(new Conversation.SpecialEvent(self, 80, "unlock"));
            self.events.Add(new Conversation.TextEvent(self, 80, self.Translate("Pebbles_Scientist_Meet_First_Line6"), 80));
            self.events.Add(new Conversation.TextEvent(self, 80, self.Translate("Pebbles_Scientist_Meet_First_Line7"), 80));
            self.events.Add(new Conversation.TextEvent(self, 80, self.Translate("Pebbles_Scientist_Meet_First_Line8"), 80));
            self.events.Add(new Conversation.TextEvent(self, 40, self.Translate("Pebbles_Scientist_Meet_First_Line9"), 80));
            self.events.Add(new Conversation.TextEvent(self, 40, self.Translate("Pebbles_Scientist_Meet_First_Line10"), 80));
            self.events.Add(new Conversation.TextEvent(self, 80, self.Translate("Pebbles_Scientist_Meet_First_Line11"), 80));
            return;
        }
    }
}

public class SSOracleMeetScientist : SSOracleBehavior.ConversationBehavior
{

    public SSOracleMeetScientist(SSOracleBehavior owner) : base(owner, Scientist.ScientistEnums.SubBehavID_Fp.FirstMeetScientist, Scientist.ScientistEnums.ConversationID_Fp.Pebbles_Scientist_Meet_First)
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
        if (this.player == null)
        {
            return;
        }
        this.owner.LockShortcuts();
        if (base.action == Scientist.ScientistEnums.Action_Fp.MeetScientist_Init)
        {
            if (this.owner.playerEnteredWithMark)
            {
                base.movementBehavior = SSOracleBehavior.MovementBehavior.Talk;
                this.owner.InitateConversation(Scientist.ScientistEnums.ConversationID_Fp.Pebbles_Scientist_Meet_First, this);
                return;
            }
            this.owner.NewAction(SSOracleBehavior.Action.General_GiveMark);
            this.owner.afterGiveMarkAction = Scientist.ScientistEnums.Action_Fp.MeetScientist_Talk_FirstMeet;
            return;

        }
        else if (base.action == Scientist.ScientistEnums.Action_Fp.MeetScientist_Talk_FirstMeet)
        {
            base.movementBehavior = SSOracleBehavior.MovementBehavior.Talk;
            if (base.inActionCounter == 15 && (this.owner.conversation == null || this.owner.conversation.id != this.convoID))
            {
                this.owner.InitateConversation(Scientist.ScientistEnums.ConversationID_Fp.Pebbles_Scientist_Meet_First, this);
            }
            if (this.owner.conversation != null && this.owner.conversation.id == this.convoID && this.owner.conversation.slatedForDeletion)
            {
                this.owner.conversation = null;
                //this.owner.NewAction(SSOracleBehavior.Action.ThrowOut_ThrowOut);
                this.owner.getToWorking = 1f;
                this.owner.NewAction(Scientist.ScientistEnums.Action_Fp.MeetScientist_ThrowOut_First);
            }
        }
    }
}

public class ThrowOutScientistBehavior : SSOracleBehavior.TalkBehavior
{
    // Token: 0x0600379E RID: 14238 RVA: 0x003EC775 File Offset: 0x003EA975
    public ThrowOutScientistBehavior(SSOracleBehavior owner) : base(owner, Scientist.ScientistEnums.SubBehavID_Fp.ThrowOutScientist)
    {
    }

    // Token: 0x0600379F RID: 14239 RVA: 0x003EC783 File Offset: 0x003EA983
    public override void Activate(SSOracleBehavior.Action oldAction, SSOracleBehavior.Action newAction)
    {
        base.Activate(oldAction, newAction);
        this.owner.pearlPickupReaction = true;
    }

    public override void NewAction(SSOracleBehavior.Action oldAction, SSOracleBehavior.Action newAction)
    {
        base.NewAction(oldAction, newAction);
        if (newAction == SSOracleBehavior.Action.ThrowOut_KillOnSight)
        {
            if (this.owner.conversation != null)
            {
                this.owner.conversation.Interrupt("...", 0);
                this.owner.conversation.Destroy();
                this.owner.conversation = null;
                return;
            }
            if (base.dialogBox.ShowingAMessage)
            {
                base.dialogBox.Interrupt("...", 0);
            }
        }
    }

    public override void Update()
    {
        base.Update();
        this.owner.UnlockShortcuts();
        if (base.player == null)
        {
            return;
        }
        if (base.player.room == base.oracle.room || (ModManager.MSC && base.oracle.room.abstractRoom.creatures.Count > 0))
        {
            if (ModManager.MMF && !MMF.cfgVanillaExploits.Value && this.owner.greenNeuron != null && this.owner.greenNeuron.room == null)
            {
                this.owner.greenNeuron = null;
            }
            if (this.owner.greenNeuron == null && this.owner.action != SSOracleBehavior.Action.ThrowOut_KillOnSight && this.owner.throwOutCounter < 900)
            {
                Vector2 vector = base.oracle.room.MiddleOfTile(28, 33);
                if (ModManager.MSC && base.oracle.ID == MoreSlugcatsEnums.OracleID.DM)
                {
                    vector = base.oracle.room.MiddleOfTile(24, 33);
                }
                using (List<AbstractCreature>.Enumerator enumerator = base.oracle.room.abstractRoom.creatures.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        AbstractCreature abstractCreature = enumerator.Current;
                        if (abstractCreature.realizedCreature != null)
                        {
                            if (!base.oracle.room.aimap.getAItile(abstractCreature.realizedCreature.mainBodyChunk.pos).narrowSpace || abstractCreature.realizedCreature != base.player)
                            {
                                abstractCreature.realizedCreature.mainBodyChunk.vel += Custom.DirVec(abstractCreature.realizedCreature.mainBodyChunk.pos, vector) * 0.2f * (1f - base.oracle.room.gravity) * Mathf.InverseLerp(220f, 280f, (float)base.inActionCounter);
                            }
                            else if (abstractCreature.realizedCreature != null && abstractCreature.realizedCreature != base.player && abstractCreature.realizedCreature.enteringShortCut == null && abstractCreature.pos == this.owner.oracle.room.ToWorldCoordinate(vector))
                            {
                                abstractCreature.realizedCreature.enteringShortCut = new IntVector2?(this.owner.oracle.room.ToWorldCoordinate(vector).Tile);
                                if (abstractCreature.abstractAI.RealAI != null)
                                {
                                    abstractCreature.abstractAI.RealAI.SetDestination(this.owner.oracle.room.ToWorldCoordinate(vector));
                                }
                            }
                        }
                    }
                    goto IL_722;
                }
            }
            if (this.owner.greenNeuron != null && this.owner.action != SSOracleBehavior.Action.ThrowOut_KillOnSight && this.owner.greenNeuron.grabbedBy.Count < 1 && this.owner.throwOutCounter < 900)
            {
                base.player.mainBodyChunk.vel *= Mathf.Lerp(0.9f, 1f, base.oracle.room.gravity);
                base.player.bodyChunks[1].vel *= Mathf.Lerp(0.9f, 1f, base.oracle.room.gravity);
                base.player.mainBodyChunk.vel += Custom.DirVec(base.player.mainBodyChunk.pos, new Vector2(base.oracle.room.PixelWidth / 2f, base.oracle.room.PixelHeight / 2f)) * 0.5f * (1f - base.oracle.room.gravity);
                if (UnityEngine.Random.value < 0.033333335f)
                {
                    this.owner.greenNeuron.storyFly = true;
                }
                if (this.owner.greenNeuron.storyFly)
                {
                    this.owner.greenNeuron.storyFlyTarget = base.player.firstChunk.pos;
                    if (Custom.DistLess(this.owner.greenNeuron.firstChunk.pos, base.player.firstChunk.pos, 40f))
                    {
                        base.player.ReleaseGrasp(1);
                        base.player.SlugcatGrab(this.owner.greenNeuron, 1);
                        this.owner.greenNeuron.storyFly = false;
                    }
                }
            }
            else if (this.telekinThrowOut)
            {
                Vector2 vector2 = base.oracle.room.MiddleOfTile(28, 33);
                if (ModManager.MSC && base.oracle.ID == MoreSlugcatsEnums.OracleID.DM)
                {
                    vector2 = base.oracle.room.MiddleOfTile(24, 33);
                }
                foreach (AbstractCreature abstractCreature2 in base.oracle.room.abstractRoom.creatures)
                {
                    if (abstractCreature2.realizedCreature != null)
                    {
                        if (!base.oracle.room.aimap.getAItile(abstractCreature2.realizedCreature.mainBodyChunk.pos).narrowSpace || abstractCreature2.realizedCreature != base.player)
                        {
                            abstractCreature2.realizedCreature.mainBodyChunk.vel += Custom.DirVec(base.player.mainBodyChunk.pos, base.oracle.room.MiddleOfTile(28, 32)) * 0.2f * (1f - base.oracle.room.gravity) * Mathf.InverseLerp(220f, 280f, (float)base.inActionCounter);
                        }
                        else if (abstractCreature2.realizedCreature != base.player && abstractCreature2.realizedCreature.enteringShortCut == null && abstractCreature2.pos == this.owner.oracle.room.ToWorldCoordinate(vector2))
                        {
                            abstractCreature2.realizedCreature.enteringShortCut = new IntVector2?(this.owner.oracle.room.ToWorldCoordinate(vector2).Tile);
                            if (abstractCreature2.abstractAI.RealAI != null)
                            {
                                abstractCreature2.abstractAI.RealAI.SetDestination(this.owner.oracle.room.ToWorldCoordinate(vector2));
                            }
                        }
                    }
                }
            }
        }
    IL_722:
        if (base.player.room == base.oracle.room || (ModManager.MSC && base.oracle.room.abstractRoom.creatures.Count > 0))
        {
            if (base.action == Scientist.ScientistEnums.Action_Fp.MeetScientist_ThrowOut_First)
            {
                this.owner.getToWorking = 1f;
                if (base.player.room == base.oracle.room)
                {
                    this.owner.throwOutCounter++;
                }
                base.movementBehavior = SSOracleBehavior.MovementBehavior.KeepDistance;
                this.telekinThrowOut = (base.inActionCounter > 220);
                if (this.owner.inspectPearl != null)
                {
                    this.owner.NewAction(MoreSlugcatsEnums.SSOracleBehaviorAction.Pebbles_SlumberParty);
                    this.owner.getToWorking = 1f;
                    return;
                }
                if (this.owner.throwOutCounter == 1300)
                {
                    base.dialogBox.Interrupt(base.Translate("Pebbles_Scientist_ThrowOut_Line1"), 80);
                }
                else if (this.owner.throwOutCounter == 2100)
                {
                    base.dialogBox.NewMessage(base.Translate("LEAVE."), 0);
                }
                else if (this.owner.throwOutCounter == 2900)
                {
                    this.owner.getToWorking = 1f;
                    base.movementBehavior = SSOracleBehavior.MovementBehavior.KeepDistance;
                    if (base.player.room != base.oracle.room && base.oracle.oracleBehavior.PlayersInRoom.Count <= 0)
                    {
                        this.owner.NewAction(SSOracleBehavior.Action.General_Idle);
                        return;
                    }
                    if (base.oracle.ID == Oracle.OracleID.SS)
                    {
                        if (!ModManager.CoopAvailable)
                        {
                            base.player.mainBodyChunk.vel += Custom.DirVec(base.player.mainBodyChunk.pos, base.oracle.room.MiddleOfTile(28, 32)) * 0.6f * (1f - base.oracle.room.gravity);
                            if (base.oracle.room.GetTilePosition(base.player.mainBodyChunk.pos) == new IntVector2(28, 32) && base.player.enteringShortCut == null)
                            {
                                base.player.enteringShortCut = new IntVector2?(base.oracle.room.ShortcutLeadingToNode(1).StartTile);
                                return;
                            }
                            return;
                        }
                        else
                        {
                            using (List<Player>.Enumerator enumerator2 = base.oracle.oracleBehavior.PlayersInRoom.GetEnumerator())
                            {
                                while (enumerator2.MoveNext())
                                {
                                    Player player = enumerator2.Current;
                                    player.mainBodyChunk.vel += Custom.DirVec(player.mainBodyChunk.pos, base.oracle.room.MiddleOfTile(28, 32)) * 0.6f * (1f - base.oracle.room.gravity);
                                    if (base.oracle.room.GetTilePosition(player.mainBodyChunk.pos) == new IntVector2(28, 32) && player.enteringShortCut == null)
                                    {
                                        player.enteringShortCut = new IntVector2?(base.oracle.room.ShortcutLeadingToNode(1).StartTile);
                                    }
                                }
                                return;
                            }
                        }
                    }
                    if (ModManager.MSC && base.oracle.ID == MoreSlugcatsEnums.OracleID.DM)
                    {
                        base.player.mainBodyChunk.vel += Custom.DirVec(base.player.mainBodyChunk.pos, base.oracle.room.MiddleOfTile(24, 32)) * 0.6f * (1f - base.oracle.room.gravity);
                        if (base.oracle.room.GetTilePosition(base.player.mainBodyChunk.pos) == new IntVector2(24, 32) && base.player.enteringShortCut == null)
                        {
                            base.player.enteringShortCut = new IntVector2?(base.oracle.room.ShortcutLeadingToNode(1).StartTile);
                            return;
                        }
                    }
                }
                if ((this.owner.playerOutOfRoomCounter > 100 && this.owner.throwOutCounter > 400) || this.owner.throwOutCounter > 3200)
                {
                    this.owner.NewAction(SSOracleBehavior.Action.General_Idle);
                    this.owner.getToWorking = 1f;
                    return;
                }
            }
            else if (base.action == SSOracleBehavior.Action.ThrowOut_SecondThrowOut)
            {
                if (base.player.room == base.oracle.room)
                {
                    this.owner.throwOutCounter++;
                }
                base.movementBehavior = SSOracleBehavior.MovementBehavior.KeepDistance;
                this.telekinThrowOut = (base.inActionCounter > 220);
                if (this.owner.throwOutCounter == 50)
                {
                    if (base.oracle.room.game.GetStorySession.saveStateNumber == MoreSlugcatsEnums.SlugcatStatsName.Gourmand && base.oracle.room.game.GetStorySession.saveState.deathPersistentSaveData.altEnding)
                    {
                        base.dialogBox.Interrupt(base.Translate("Oh, it's you again? I had told you to leave and never return."), 0);
                    }
                    else
                    {
                        base.dialogBox.Interrupt(base.Translate("You again? I have nothing for you."), 0);
                    }
                }
                else if (this.owner.throwOutCounter == 250)
                {
                    int num = 0;
                    if (ModManager.MSC)
                    {
                        for (int i = 0; i < base.oracle.room.physicalObjects.Length; i++)
                        {
                            for (int j = 0; j < base.oracle.room.physicalObjects[i].Count; j++)
                            {
                                if (base.oracle.room.physicalObjects[i][j] is Player && (base.oracle.room.physicalObjects[i][j] as Player).isNPC)
                                {
                                    num++;
                                }
                            }
                        }
                    }
                    if (num > 0)
                    {
                        base.dialogBox.Interrupt(base.Translate("Leave immediately and don't come back. And take THEM with you!"), 0);
                    }
                    else
                    {
                        base.dialogBox.Interrupt(base.Translate("I won't tolerate this. Leave immediately and don't come back."), 0);
                    }
                }
                else if (this.owner.throwOutCounter == 700)
                {
                    base.dialogBox.Interrupt(base.Translate("You had your chances."), 0);
                }
                else if (this.owner.throwOutCounter > 770)
                {
                    this.owner.NewAction(SSOracleBehavior.Action.ThrowOut_KillOnSight);
                }
                if (this.owner.playerOutOfRoomCounter > 100 && this.owner.throwOutCounter > 400)
                {
                    this.owner.NewAction(SSOracleBehavior.Action.General_Idle);
                    this.owner.getToWorking = 1f;
                    return;
                }
            }
            else if (base.action == SSOracleBehavior.Action.ThrowOut_Polite_ThrowOut)
            {
                this.owner.getToWorking = 1f;
                if (base.inActionCounter < 200)
                {
                    base.movementBehavior = SSOracleBehavior.MovementBehavior.Idle;
                }
                else if (base.inActionCounter < 530)
                {
                    base.movementBehavior = SSOracleBehavior.MovementBehavior.Talk;
                }
                else if (base.inActionCounter < 1050)
                {
                    base.movementBehavior = SSOracleBehavior.MovementBehavior.Idle;
                }
                else
                {
                    base.movementBehavior = SSOracleBehavior.MovementBehavior.KeepDistance;
                }
                if (this.owner.playerOutOfRoomCounter > 100 && base.inActionCounter > 400)
                {
                    this.owner.NewAction(SSOracleBehavior.Action.General_Idle);
                    return;
                }
                if (base.inActionCounter == 500)
                {
                    base.dialogBox.Interrupt(base.Translate("Thank you little creature. I must resume my work."), 0);
                    return;
                }
                if (base.inActionCounter == 1100)
                {
                    base.dialogBox.NewMessage(base.Translate("I appreciate what you have done but it is time for you to leave."), 0);
                    if (this.owner.oracle.room.game.StoryCharacter == SlugcatStats.Name.Red)
                    {
                        base.dialogBox.NewMessage(base.Translate("As I mentioned you do not have unlimited time."), 0);
                        return;
                    }
                }
                else if (base.inActionCounter > 1400)
                {
                    this.owner.NewAction(SSOracleBehavior.Action.ThrowOut_ThrowOut);
                    this.owner.getToWorking = 0f;
                    return;
                }
            }
            else if (ModManager.MSC && base.action == MoreSlugcatsEnums.SSOracleBehaviorAction.ThrowOut_Singularity)
            {
                if (base.inActionCounter == 10)
                {
                    if ((base.oracle.oracleBehavior as SSOracleBehavior).conversation != null)
                    {
                        (base.oracle.oracleBehavior as SSOracleBehavior).conversation.Destroy();
                        (base.oracle.oracleBehavior as SSOracleBehavior).conversation = null;
                    }
                    base.dialogBox.Interrupt(base.Translate(". . . !"), 0);
                }
                this.owner.getToWorking = 1f;
                if (base.player.room != base.oracle.room && !base.player.inShortcut)
                {
                    if (base.player.grasps[0] != null && base.player.grasps[0].grabbed is SingularityBomb)
                    {
                        (base.player.grasps[0].grabbed as SingularityBomb).Thrown(base.player, base.player.firstChunk.pos, null, new IntVector2(0, -1), 1f, true);
                        (base.player.grasps[0].grabbed as SingularityBomb).ignited = true;
                        (base.player.grasps[0].grabbed as SingularityBomb).activateSucktion = true;
                        (base.player.grasps[0].grabbed as SingularityBomb).counter = 50f;
                        (base.player.grasps[0].grabbed as SingularityBomb).floatLocation = base.player.firstChunk.pos;
                        (base.player.grasps[0].grabbed as SingularityBomb).firstChunk.pos = base.player.firstChunk.pos;
                    }
                    if (base.player.grasps[1] != null && base.player.grasps[1].grabbed is SingularityBomb)
                    {
                        (base.player.grasps[1].grabbed as SingularityBomb).Thrown(base.player, base.player.firstChunk.pos, null, new IntVector2(0, -1), 1f, true);
                        (base.player.grasps[1].grabbed as SingularityBomb).ignited = true;
                        (base.player.grasps[1].grabbed as SingularityBomb).activateSucktion = true;
                        (base.player.grasps[1].grabbed as SingularityBomb).counter = 50f;
                        (base.player.grasps[1].grabbed as SingularityBomb).floatLocation = base.player.firstChunk.pos;
                        (base.player.grasps[1].grabbed as SingularityBomb).firstChunk.pos = base.player.firstChunk.pos;
                    }
                    base.player.Stun(200);
                    this.owner.NewAction(SSOracleBehavior.Action.General_Idle);
                    return;
                }
                base.movementBehavior = SSOracleBehavior.MovementBehavior.KeepDistance;
                if (base.oracle.ID == Oracle.OracleID.SS)
                {
                    base.player.mainBodyChunk.vel += Custom.DirVec(base.player.mainBodyChunk.pos, base.oracle.room.MiddleOfTile(28, 32)) * 1.3f;
                    base.player.mainBodyChunk.pos = Vector2.Lerp(base.player.mainBodyChunk.pos, base.oracle.room.MiddleOfTile(28, 32), 0.08f);
                    if (base.player.enteringShortCut == null && base.player.mainBodyChunk.pos.x < 560f && base.player.mainBodyChunk.pos.y > 630f)
                    {
                        base.player.mainBodyChunk.pos.y = 630f;
                    }
                    if ((base.oracle.oracleBehavior as SSOracleBehavior).dangerousSingularity != null)
                    {
                        (base.oracle.oracleBehavior as SSOracleBehavior).dangerousSingularity.activateSucktion = false;
                        (base.oracle.oracleBehavior as SSOracleBehavior).dangerousSingularity.firstChunk.vel += Custom.DirVec((base.oracle.oracleBehavior as SSOracleBehavior).dangerousSingularity.firstChunk.pos, base.player.mainBodyChunk.pos) * 1.3f;
                        (base.oracle.oracleBehavior as SSOracleBehavior).dangerousSingularity.firstChunk.pos = Vector2.Lerp((base.oracle.oracleBehavior as SSOracleBehavior).dangerousSingularity.firstChunk.pos, base.player.mainBodyChunk.pos, 0.1f);
                        if (Vector2.Distance((base.oracle.oracleBehavior as SSOracleBehavior).dangerousSingularity.firstChunk.pos, base.player.mainBodyChunk.pos) < 10f)
                        {
                            if (base.player.grasps[0] == null)
                            {
                                base.player.SlugcatGrab((base.oracle.oracleBehavior as SSOracleBehavior).dangerousSingularity, 0);
                            }
                            if (base.player.grasps[1] == null)
                            {
                                base.player.SlugcatGrab((base.oracle.oracleBehavior as SSOracleBehavior).dangerousSingularity, 1);
                            }
                        }
                    }
                    if (base.oracle.room.GetTilePosition(base.player.mainBodyChunk.pos) == new IntVector2(28, 32) && base.player.enteringShortCut == null)
                    {
                        bool flag = false;
                        if (base.player.grasps[0] != null && base.player.grasps[0].grabbed == (base.oracle.oracleBehavior as SSOracleBehavior).dangerousSingularity)
                        {
                            flag = true;
                        }
                        if (base.player.grasps[1] != null && base.player.grasps[1].grabbed == (base.oracle.oracleBehavior as SSOracleBehavior).dangerousSingularity)
                        {
                            flag = true;
                        }
                        if (flag)
                        {
                            base.player.enteringShortCut = new IntVector2?(base.oracle.room.ShortcutLeadingToNode(1).StartTile);
                            return;
                        }
                        base.player.ReleaseGrasp(0);
                        base.player.ReleaseGrasp(1);
                    }
                }
                if (base.oracle.ID == MoreSlugcatsEnums.OracleID.DM)
                {
                    base.player.mainBodyChunk.vel += Custom.DirVec(base.player.mainBodyChunk.pos, base.oracle.room.MiddleOfTile(24, 32)) * 1.3f;
                    base.player.mainBodyChunk.pos = Vector2.Lerp(base.player.mainBodyChunk.pos, base.oracle.room.MiddleOfTile(24, 32), 0.08f);
                    if ((base.oracle.oracleBehavior as SSOracleBehavior).dangerousSingularity != null)
                    {
                        (base.oracle.oracleBehavior as SSOracleBehavior).dangerousSingularity.activateSucktion = false;
                        (base.oracle.oracleBehavior as SSOracleBehavior).dangerousSingularity.firstChunk.vel += Custom.DirVec((base.oracle.oracleBehavior as SSOracleBehavior).dangerousSingularity.firstChunk.pos, base.player.mainBodyChunk.pos) * 1.3f;
                        (base.oracle.oracleBehavior as SSOracleBehavior).dangerousSingularity.firstChunk.pos = Vector2.Lerp((base.oracle.oracleBehavior as SSOracleBehavior).dangerousSingularity.firstChunk.pos, base.player.mainBodyChunk.pos, 0.1f);
                        if (Vector2.Distance((base.oracle.oracleBehavior as SSOracleBehavior).dangerousSingularity.firstChunk.pos, base.player.mainBodyChunk.pos) < 10f)
                        {
                            if (base.player.grasps[0] == null)
                            {
                                base.player.SlugcatGrab((base.oracle.oracleBehavior as SSOracleBehavior).dangerousSingularity, 0);
                            }
                            if (base.player.grasps[1] == null)
                            {
                                base.player.SlugcatGrab((base.oracle.oracleBehavior as SSOracleBehavior).dangerousSingularity, 1);
                            }
                        }
                    }
                    if (base.oracle.room.GetTilePosition(base.player.mainBodyChunk.pos) == new IntVector2(28, 32) && base.player.enteringShortCut == null)
                    {
                        bool flag2 = false;
                        if (base.player.grasps[0] != null && base.player.grasps[0].grabbed == (base.oracle.oracleBehavior as SSOracleBehavior).dangerousSingularity)
                        {
                            flag2 = true;
                        }
                        if (base.player.grasps[1] != null && base.player.grasps[1].grabbed == (base.oracle.oracleBehavior as SSOracleBehavior).dangerousSingularity)
                        {
                            flag2 = true;
                        }
                        if (flag2)
                        {
                            base.player.enteringShortCut = new IntVector2?(base.oracle.room.ShortcutLeadingToNode(1).StartTile);
                            return;
                        }
                        base.player.ReleaseGrasp(0);
                        base.player.ReleaseGrasp(1);
                    }
                }
            }
        }
    }

    public override bool Gravity
    {
        get
        {
            return base.action != SSOracleBehavior.Action.ThrowOut_ThrowOut && base.action != SSOracleBehavior.Action.ThrowOut_SecondThrowOut;
        }
    }

    public bool telekinThrowOut;
}