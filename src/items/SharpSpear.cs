﻿using Scientist.Items.AbstractPhysicalObjects;
using MoreSlugcats;
using RWCustom;
using System;
using System.Data;
using System.Diagnostics;
using UnityEngine;

namespace Scientist.Items;

sealed class SharpSpear : Spear
{
    public int ReboundCount = 0;

    public new Scientist.Items.AbstractPhysicalObjects.SharpSpearAbstract abstractSpear
    {
        get
        {
            return this.abstractPhysicalObject as SharpSpearAbstract;
        }
    }

    public new bool bugSpear
    {
        get
        {
            return false;
        }
    }

    public SharpSpear(Scientist.Items.AbstractPhysicalObjects.SharpSpearAbstract abstractPhysicalObject, World world) : base(abstractPhysicalObject, world)
    {
        base.bodyChunks = new BodyChunk[1];
        base.bodyChunks[0] = new BodyChunk(this, 0, new Vector2(0f, 0f), 5f, 0.05f);
        this.bodyChunkConnections = new PhysicalObject.BodyChunkConnection[0];
        base.airFriction = 0.999f;
        base.gravity = 0.9f;
        this.bounce = 0.4f;
        this.surfaceFriction = 0.4f;
        this.collisionLayer = 2;
        base.waterFriction = 1f;
        base.buoyancy = 0.4f;
        this.pivotAtTip = false;
        this.lastPivotAtTip = false;
        this.stuckBodyPart = -1;
        base.firstChunk.loudness = 7f;
        this.tailPos = base.firstChunk.pos;
        this.soundLoop = new ChunkDynamicSoundLoop(base.firstChunk);
        this.wasHorizontalBeam = new bool[3];
        this.spearmasterNeedle = false;
        this.spearmasterNeedle_hasConnection = false;
        this.spearmasterNeedle_fadecounter_max = 400;
        this.spearmasterNeedle_fadecounter = this.spearmasterNeedle_fadecounter_max;
        this.spearmasterNeedleType = UnityEngine.Random.Range(0, 3);
        this.jollyCustomColor = null;
        this.alwaysStickInWalls = true;
    }

    public override void Update(bool eu)
    {
        base.Update(eu); 
        if (this.mode == Weapon.Mode.Thrown && Custom.DistLess(this.thrownPos, base.firstChunk.pos, 1200f * Mathf.Max(1f, this.spearDamageBonus)) && base.firstChunk.ContactPoint == this.throwDir && this.room.GetTile(base.firstChunk.pos).Terrain == Room.Tile.TerrainType.Air && this.room.GetTile(base.firstChunk.pos + this.throwDir.ToVector2() * 20f).Terrain == Room.Tile.TerrainType.Solid)
        {
            bool flag = true;
            foreach (AbstractWorldEntity abstractWorldEntity in this.room.abstractRoom.entities)
            {
                if (abstractWorldEntity is AbstractSpear && (abstractWorldEntity as AbstractSpear).realizedObject != null && ((abstractWorldEntity as AbstractSpear).realizedObject as Weapon).mode == Weapon.Mode.StuckInWall && abstractWorldEntity.pos.Tile == this.abstractPhysicalObject.pos.Tile)
                {
                    flag = false;
                    break;
                }
            }
            bool flag2 = false;
            if (flag)
            {
                if (this.abstractPhysicalObject.pos.Tile.y <= 0 || this.abstractPhysicalObject.pos.Tile.y >= this.abstractPhysicalObject.Room.size.y - 1 || this.abstractPhysicalObject.pos.Tile.x <= 0 || this.abstractPhysicalObject.pos.Tile.x >= this.abstractPhysicalObject.Room.size.x - 1)
                {
                    flag = false;
                    flag2 = true;
                }
            }
            if (flag && this.room.abstractRoom.shelter && this.room.shelterDoor != null && (this.room.shelterDoor.IsClosing || this.room.shelterDoor.IsOpening))
            {
                flag = false;
            }
            if (flag)
            {
                this.stuckInWall = new Vector2?(this.room.MiddleOfTile(base.firstChunk.pos));
                this.vibrate = 10;
                this.ChangeMode(Weapon.Mode.StuckInWall);
                this.room.PlaySound(SoundID.Spear_Stick_In_Wall, base.firstChunk);
                base.firstChunk.collideWithTerrain = false;
            }
            else if (ModManager.MMF && flag2)
            {
                this.vibrate = 10;
                this.room.PlaySound(SoundID.Spear_Bounce_Off_Creauture_Shell, base.firstChunk);
                for (int n = 17; n > 0; n--)
                {
                    this.room.AddObject(new Spark(base.firstChunk.pos, Custom.RNV(), Color.white, null, 10, 20));
                }
            }
        }

    }

    public override void ChangeMode(Mode newMode)
    {
        if (newMode == Weapon.Mode.StuckInWall && this.abstractSpear.stuckInWallCycles == 0)
        {
            this.abstractSpear.stuckInWallCycles = UnityEngine.Random.Range(8, 13) * ((this.throwDir.y != 0) ? -1 : 1);
        }
        base.ChangeMode(newMode);
    }

    public override bool HitSomething(SharedPhysics.CollisionResult result, bool eu)
    {
        if (result.obj is Creature)
        {
            this.spearDamageBonus *= 1.5f;
        }
        return base.HitSomething(result, eu);
    }

    public override void HitWall()
    {
        this.bodyChunks[0].vel *= 0.3f;
        base.HitWall();
        /*try
        {
            this.stuckInWall = new Vector2?(this.room.MiddleOfTile(this.abstractPhysicalObject.pos.Tile));
            this.ChangeMode(Weapon.Mode.StuckInWall);
        }
        catch (Exception ex)
        {
            Scientist.ScientistLogger.Error($"{ex}, {this.abstractPhysicalObject.pos.Tile}, SharpSpear.HitWall(), line 86, SharpSpear.cs");
            this.ChangeMode(Weapon.Mode.Free);
        }*/
    }

    public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        if (this.stuckIns != null)
        {
            rCam.ReturnFContainer("HUD").AddChild(this.stuckIns.label);
        }
        sLeaser.sprites = new FSprite[2];
        sLeaser.sprites[0] = new FSprite("SmallSharpSpear", true);
        sLeaser.sprites[1] = new FSprite("SmallSharpSpearHead", true);
        this.AddToContainer(sLeaser, rCam, null);
    }

    public override void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
    {
        base.AddToContainer(sLeaser, rCam, newContatiner);
        //rCam.ReturnFContainer("Foreground").AddChild(sLeaser.sprites[1]);
        sLeaser.sprites[1].MoveInFrontOfOtherNode(sLeaser.sprites[0]);
    }

    public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        if (this.stuckIns != null && this.room != null)
        {
            if (this.room.game.devToolsActive && Input.GetKeyDown("l"))
            {
                sLeaser.RemoveAllSpritesFromContainer();
                this.InitiateSprites(sLeaser, rCam);
                this.ApplyPalette(sLeaser, rCam, rCam.currentPalette);
            }
            if (this.stuckIns.relativePos)
            {
                this.stuckIns.label.x = base.bodyChunks[0].pos.x + this.stuckIns.pos.x - camPos.x;
                this.stuckIns.label.y = base.bodyChunks[0].pos.y + this.stuckIns.pos.y - camPos.y;
            }
            else
            {
                this.stuckIns.label.x = this.stuckIns.pos.x;
                this.stuckIns.label.y = this.stuckIns.pos.y;
            }
        }
        Vector2 vector = Vector2.Lerp(base.firstChunk.lastPos, base.firstChunk.pos, timeStacker);
        if (this.vibrate > 0)
        {
            vector += Custom.DegToVec(UnityEngine.Random.value * 360f) * 2f * UnityEngine.Random.value;
        }
        Vector3 vector2 = Vector3.Slerp(this.lastRotation, this.rotation, timeStacker);
        for (int i = 0; i <= 1; i++)
        {
            sLeaser.sprites[i].x = vector.x - camPos.x;
            sLeaser.sprites[i].y = vector.y - camPos.y;
            sLeaser.sprites[i].anchorY = Mathf.Lerp(this.lastPivotAtTip ? 0.85f : 0.5f, this.pivotAtTip ? 0.85f : 0.5f, timeStacker);
            sLeaser.sprites[i].rotation = Custom.AimFromOneVectorToAnother(new Vector2(0f, 0f), vector2);
        }
        sLeaser.sprites[0].color = (this.blink > 0 && UnityEngine.Random.value < 0.5f) ? base.blinkColor : this.color;
        sLeaser.sprites[1].color = Scientist.ScientistTools.ColorFromHex("4f4f4f");
        if (base.slatedForDeletetion || this.room != rCam.room)
        {
            sLeaser.CleanSpritesAndRemove();
        }
    }
}
