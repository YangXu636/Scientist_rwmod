using items.AbstractPhysicalObjects;
using MoreSlugcats;
using RWCustom;
using System;
using System.Data;
using System.Diagnostics;
using UnityEngine;

namespace items;

//TODO 1.5矛的伤害 OK!
//TODO 如果你一不小心把削尖的矛插到墙中，它会插的很深，只有胖猫体型且不在力竭状态才能把它拨出 OK!



sealed class SharpSpear : Spear
{
    public new items.AbstractPhysicalObjects.SharpSpearAbstract abstractSpear
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

    public SharpSpear(items.AbstractPhysicalObjects.SharpSpearAbstract abstractPhysicalObject, World world) : base(abstractPhysicalObject, world)
    {
        base.bodyChunks = new BodyChunk[1];
        base.bodyChunks[0] = new BodyChunk(this, 0, new Vector2(0f, 0f), 5f, 0.05f);
        this.bodyChunkConnections = new PhysicalObject.BodyChunkConnection[0];
        base.airFriction = 0.999f;
        base.gravity = 0.9f;
        this.bounce = 0.4f;
        this.surfaceFriction = 0.4f;
        this.collisionLayer = 2;
        base.waterFriction = 0.98f;
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
    }

    public override void Thrown(Creature thrownBy, Vector2 thrownPos, Vector2? firstFrameTraceFromPos, IntVector2 throwDir, float frc, bool eu)
    {
        base.Thrown(thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);
    }

    public override bool HitSomething(SharedPhysics.CollisionResult result, bool eu)
    {
        this.spearDamageBonus *= 1.5f;
        return base.HitSomething(result, eu);
    }
    public override void HitSomethingWithoutStopping(PhysicalObject obj, BodyChunk chunk, Appendage appendage)
    {
        base.HitSomethingWithoutStopping(obj, chunk, appendage);
    }

    public override void HitWall()
    {
        base.HitWall();
        try
        {
            this.stuckInWall = new Vector2?(this.room.MiddleOfTile(this.abstractPhysicalObject.pos.Tile));
            this.ChangeMode(Weapon.Mode.StuckInWall);
        }
        catch (Exception ex)
        {
            ScientistLogger.LogError($"{ex}, {this.abstractPhysicalObject.pos.Tile}, SharpSpear.HitWall(), line 86, SharpSpear.cs");
            this.ChangeMode(Weapon.Mode.Free);
        }
    }

    public override void PickedUp(Creature upPicker)
    {
        //Console.WriteLine($"{(upPicker as Player).SlugCatClass}, {(upPicker as Player).SlugCatClass.value},  {(upPicker as Player).bodyMode}");
        //Player player = upPicker as Player;
        //if ( (player.isGourmand && player.gourmandExhausted) || (player.SlugCatClass.value == "xuyangjerry.Scientist") )
        base.PickedUp(upPicker);
    }

    public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        base.InitiateSprites(sLeaser, rCam);
        if (sLeaser.sprites[0].element.name == "SmallSpear")
        {
            sLeaser.sprites[0].element = Futile.atlasManager.GetElementWithName("SmallSharpSpear");
        }
    }
}
