using System;
using System.Security.Permissions;
using UnityEngine;
using Scientist;
using System.Collections.Generic;
using Scientist.Animations;

namespace Scientist.Rss;

public class CC_C07_Spawn : UpdatableAndDeletable
{
    public readonly Vector2 spawnPos = new(890.0f, 2200.0f);
    public Dictionary<int, int> time = new();
    public Dictionary<int, bool> spawned = new();
    public Dictionary<int, Animations.PlayerAnimations> animations = new();

    public AbstractCreature[] aps;
    public Player p;

    public CC_C07_Spawn(Room room)
    {
        this.room = room;
    }

    public override void Update(bool eu)
    {
        base.Update(eu);
        aps ??= this.room.game.Players.ToArray();
        foreach (AbstractCreature ap in aps)
        {
            if (ap.realizedCreature != null)
            {
                int index = ScientistTools.PlayerIndex(ap.realizedCreature as Player);
                if (!this.time.ContainsKey(index))
                {
                    this.time.Add(index, 0);
                }
                if (!this.spawned.ContainsKey(index))
                {
                    this.spawned.Add(index, false);
                }
                if (!this.animations.ContainsKey(index))
                {
                    this.animations.Add(index, new Animations.PlayerAnimations(ap.realizedCreature as Player));
                }
            }
            if (
            this.room.game.session is StoryGameSession
            && this.room.game.Players.Count > 0
            && ap != null && ap.realizedCreature != null
            && ap.realizedCreature.room == this.room
            && this.room.game.GetStorySession.saveState.cycleNumber == 0
            && !ap.realizedCreature.dead
            )
            {
                int index = ScientistTools.PlayerIndex(ap.realizedCreature as Player);
                p = ap.realizedCreature as Player;
                this.time[index]++;
                if (!this.spawned[index])
                {
                    this.spawned[index] = true;
                    p.SuperHardSetPosition(spawnPos);
                    Animations.PlayerAnimations pa = this.animations[index];
                    this.animations[index].AddAnimationFollow(() => pa.Wait(40));
                    this.animations[index].AddAnimationFollow(() => pa.VerticallyMove(spawnPos, 45f, p.bodyChunks));
                    this.animations[index].AddAnimationFollow(() => pa.Wait(10));
                    this.animations[index].AddAnimationFollow(() => pa.HorizontallyMove(new Vector2(895.0f, 2240.0f), -1, 20, p.bodyChunks));
                    this.animations[index].AddAnimationFollow(() => pa.HorizontallyMove(new Vector2(895.0f, 2240.0f), -40f, p.bodyChunks));
                    this.animations[index].AddAnimationFollow(() => pa.Wait(10));
                    this.animations[index].AddAnimationFollow(() => pa.VerticallyMove(new Vector2(855.0f, 2240.0f), 20f, p.bodyChunks));
                    this.animations[index].AddAnimationFollow(() => pa.Wait(10));
                    this.animations[index].AddAnimationFollow(() => pa.HorizontallyMove(new Vector2(855.0f, 2270.0f), -485f, p.bodyChunks));
                    this.animations[index].AddAnimationFollow(() => pa.Wait(10));
                    this.animations[index].AddAnimationFollow(() => pa.VerticallyMove(new Vector2(370.0f, 2270.0f), -20f, p.bodyChunks));
                }
                if (index == 0)
                {
                    RoomCamera roomCamera = p.abstractCreature.world.game.cameras[0];
                    roomCamera.ChangeCameraToPlayer(p.abstractCreature);
                }
                if (this.time[index] == 120 + (index * 40)) //80 + (index * 5)
                {
                    this.animations[index].StartPlaying(spawnPos);
                }
                if (this.time[index] > 120 + (index * 40))
                {
                    this.animations[index].Update();
                }
                /*
                if (this.time[pIndex] == 100 + (pIndex * 20))
                {
                    p.Jump();
                }
                if (this.time[pIndex] >= 120 + (pIndex * 20) && this.time[pIndex] <= 800 + (pIndex * 20))
                {
                    p.bodyMode = PlayerVariables.BodyModeIndex.ClimbingOnBeam;
                    p.animation = PlayerVariables.AnimationIndex.ClimbOnBeam;
                    p.input[0].y = 1;
                }*/
            }
        }
        for (int k = 0; k < this.room.physicalObjects.Length; k++)
        {
            for (int l = 0; l < this.room.physicalObjects[k].Count; l++)
            {
                PhysicalObject obj = this.room.physicalObjects[k][l];
                if (obj is Creature)
                {
                    if (obj is Lizard)
                    {
                        (obj as Lizard).Die();
                        (obj as Lizard).Destroy();
                    }
                    if (obj is Vulture)
                    {
                        (obj as Vulture).Die();
                        (obj as Vulture).Destroy();
                    }
                }
            }                
        }
    }
}