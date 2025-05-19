using Scientist.Items.AbstractPhysicalObjects;
using MoreSlugcats;
using RWCustom;
using Scientist;
using System;
using System.Data;
using System.Diagnostics;
using UnityEngine;
using Noise;
using Smoke;
using System.Collections.Generic;
using static MonoMod.InlineRT.MonoModRule;

namespace Scientist.Items;

class BlastingStoneBombFragment : Weapon
{
    public int counter = 0;
    public int counterMax = 240;
    public int terrainTimes = 0;
    //public bool inUpdateTraverse = false;
    public bool needDestroy = false;

    public override bool HeavyWeapon
    {
        get
        {
            return true;
        }
    }

    public BlastingStoneBombFragment(AbstractPhysicalObject abstractPhysicalObject, World world) : base(abstractPhysicalObject, world)
    {
        base.bodyChunks = new BodyChunk[1];
        base.bodyChunks[0] = new BodyChunk(this, 0, new Vector2(0f, 0f), 6f, 0.01f);
        this.bodyChunkConnections = new PhysicalObject.BodyChunkConnection[0];
        base.airFriction = 0.999f;
        base.gravity = 0.9f;
        this.bounce = 0.4f;
        this.surfaceFriction = 0.4f;
        this.collisionLayer = 1;
        base.waterFriction = 0.98f;
        base.buoyancy = 0.4f;
        base.firstChunk.loudness = 4f;
        this.tailPos = base.firstChunk.pos;
        this.soundLoop = new ChunkDynamicSoundLoop(base.firstChunk);
    }

    public override void Update(bool eu)
    {
        base.Update(eu);
        counter++;
        this.soundLoop.sound = SoundID.None;
        if (this.collisionLayer != 1)
        {
            base.ChangeCollisionLayer(1);
        }
        if (base.firstChunk.vel.magnitude > 5f)
        {
            if (base.firstChunk.ContactPoint.y < 0)
            {
                this.soundLoop.sound = SoundID.Rock_Skidding_On_Ground_LOOP;
            }
            else
            {
                this.soundLoop.sound = SoundID.Rock_Through_Air_LOOP;
            }
            this.soundLoop.Volume = Mathf.InverseLerp(5f, 15f, base.firstChunk.vel.magnitude);
        }
        this.soundLoop.Update();
        this.lastRotation = this.rotation;
        this.rotation = this.bodyChunks[0].vel.normalized;
        bool flag = false;
        List<AbstractCreature> acs = this?.room?.abstractRoom.creatures;
        for (int i = 0; i < acs.Count; i++)
        {
            if (acs[i].realizedCreature == null) { continue; }
            for (int j = 0; j < acs[i].realizedCreature.bodyChunks.Length; j++)
            {
                if (Vector2.Distance(acs[i].realizedCreature.bodyChunks[j].pos, this.firstChunk.pos) <= 20f)
                {
                    acs[i]?.realizedCreature?.SetKillTag(this?.thrownBy?.abstractCreature);
                    acs[i]?.realizedCreature?.Violence(this?.firstChunk, default, acs[i]?.realizedCreature?.bodyChunks[j], default, Creature.DamageType.Stab, 0.4f, 85f);
                    ScientistLogger.Log($"{this?.abstractPhysicalObject.ID.ToString() ?? "null"} hit creature {acs[i].ID}");
                    flag = true;
                }
            }
        }
        if (flag || this.needDestroy || counter >= counterMax)
        {
            this.Destroy();
        }
    }

    public override void TerrainImpact(int chunk, IntVector2 direction, float speed, bool firstContact)
    {
        base.TerrainImpact(chunk, direction, speed, firstContact);
        if (this.floorBounceFrames > 0 && (direction.x == 0 || this.room.GetTile(base.firstChunk.pos).Terrain == Room.Tile.TerrainType.Slope))
        {
            return;
        }
        this.terrainTimes++;
        List<AbstractCreature> acs = this?.room?.abstractRoom.creatures;
        for (int i = 0; i < acs.Count; i++)
        {
            if (acs[i].realizedCreature == null) { continue; }
            for (int j = 0; j < acs[i].realizedCreature.bodyChunks.Length; j++)
            {
                if (Vector2.Distance(acs[i].realizedCreature.bodyChunks[j].pos, this.firstChunk.pos) <= 20f)
                {
                    acs[i]?.realizedCreature?.SetKillTag(this?.thrownBy?.abstractCreature);
                    acs[i]?.realizedCreature?.Violence(this?.firstChunk, default, acs[i]?.realizedCreature?.bodyChunks[j], default, Creature.DamageType.Stab, 0.4f, 85f);
                    ScientistLogger.Log($"{this?.abstractPhysicalObject.ID.ToString() ?? "null"} hit creature {acs[i].ID}");
                    this.needDestroy = true;
                }
            }
        }
        if (this.terrainTimes >= 3)
        {
            this.needDestroy = true;
        }
    }

    public override void HitByWeapon(Weapon weapon)
    {
        if (weapon.mode == Weapon.Mode.Thrown && this.thrownBy == null && weapon.thrownBy != null)
        {
            this.thrownBy = weapon.thrownBy;
        }
        base.HitByWeapon(weapon);
    }

    public override void HitByExplosion(float hitFac, Explosion explosion, int hitChunk)
    {
        base.HitByExplosion(hitFac, explosion, hitChunk);
        this.bodyChunks[0].vel += hitFac * 5.0f * (this.bodyChunks[0].pos - explosion.pos);
    }

    public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[2];
        sLeaser.sprites[0] = new FSprite("BlastingStoneBombFragmentA", true);
        sLeaser.sprites[1] = new FSprite("BlastingStoneBombFragmentB", true);
        this.AddToContainer(sLeaser, rCam, null);
    }

    public override void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
    {
        newContatiner ??= rCam.ReturnFContainer("Items");
        for (int i = 0; i < sLeaser.sprites.Length; i++)
        {
            sLeaser.sprites[i].RemoveFromContainer();
            newContatiner.AddChild(sLeaser.sprites[i]);
        }
    }

    public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        sLeaser.sprites[0].color = ScientistTools.ColorFromHex("#15091a");
        sLeaser.sprites[1].color = ScientistTools.ColorFromHex("#ed0707");
    }

    public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        Vector2 vector = Vector2.Lerp(base.firstChunk.lastPos, base.firstChunk.pos, timeStacker);
        Vector2 v = Vector3.Slerp(this.lastRotation, this.rotation, timeStacker);
        for (int i = 0; i < sLeaser.sprites.Length; i++)
        {
            sLeaser.sprites[i].x = vector.x - camPos.x;
            sLeaser.sprites[i].y = vector.y - camPos.y;
            sLeaser.sprites[i].rotation = Custom.VecToDeg(v);
        }
        if (this.blink > 0)
        {
            if (this.blink > 1 && UnityEngine.Random.value < 0.5f)
            {
                sLeaser.sprites[0].color = base.blinkColor;
                sLeaser.sprites[1].color = base.blinkColor;
            }
            else
            {
                this.ApplyPalette(sLeaser, rCam, rCam.currentPalette);
            }
        }
        else
        {
            this.ApplyPalette(sLeaser, rCam, rCam.currentPalette);
        }
        if (base.slatedForDeletetion || this.room != rCam.room)
        {
            sLeaser.CleanSpritesAndRemove();
        }
    }
}
