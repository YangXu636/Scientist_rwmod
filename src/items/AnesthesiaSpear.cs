using items.AbstractPhysicalObjects;
using MoreSlugcats;
using RWCustom;
using System;
using System.Data;
using System.Diagnostics;
using UnityEngine;

namespace items;


sealed class AnesthesiaSpear : Spear
{
    public new items.AbstractPhysicalObjects.AnesthesiaSpearAbstract abstractSpear
    {
        get
        {
            return this.abstractPhysicalObject as AnesthesiaSpearAbstract;
        }
    }

    public new bool bugSpear
    {
        get
        {
            return false;
        }
    }

    public AnesthesiaSpear(items.AbstractPhysicalObjects.AnesthesiaSpearAbstract abstractPhysicalObject, World world) : base(abstractPhysicalObject, world)
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
        if (result.obj is Creature)
        {
            Creature c = result.obj as Creature;
            if (c is Player)
            {
                this.spearDamageBonus *= 0;
            }
            if (!c.dead)
            {
                c.Stun(800);
            }
        }
        return base.HitSomething(result, eu);
    }

    public override void HitSomethingWithoutStopping(PhysicalObject obj, BodyChunk chunk, Appendage appendage)
    {
        bool flag = false;
        if (obj is Creature)
        {
            flag = (obj as Creature).dead;
        }
        base.HitSomethingWithoutStopping(obj, chunk, appendage);
        if (obj is Creature && !flag)
        {
            Creature c = obj as Creature;
            if (c is Player)
            {
                c.Stun(800);
            }
            else
            {
                Scientist.ScientistPlayer.anesthesiaCreatures[Scientist.ScientistTools.FeaturesTypeString(c)] = new Scientist.AnesthesiaCreature( (x, y) => x < y);
            }
        }
        if (obj is items.AnesthesiaSpear)
        {
            for (int i = 0; i < UnityEngine.Random.Range(2, 4); i++)
            {
                items.AbstractPhysicalObjects.AnesthesiaSpearAbstract apo = new(this.room.world, null, this.abstractPhysicalObject.pos, this.room.world.game.GetNewID());
                this.room.abstractRoom.AddEntity(apo);
                apo.RealizeInRoom();
                apo.realizedObject.firstChunk.vel = -this.firstChunk.vel / 3f;
            }
            for (int i = 0; i < UnityEngine.Random.Range(2, 4); i++)
            {
                items.AbstractPhysicalObjects.AnesthesiaSpearAbstract apo = new(this.room.world, null, this.abstractPhysicalObject.pos, this.room.world.game.GetNewID());
                this.room.abstractRoom.AddEntity(apo);
                apo.RealizeInRoom();
                apo.realizedObject.firstChunk.vel = this.firstChunk.vel / 3f;
            }
            this.Destroy();
            (obj as items.AnesthesiaSpear).Destroy();
        }
    }

    public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        base.InitiateSprites(sLeaser, rCam);
    }

    public override void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
    {
        base.AddToContainer(sLeaser, rCam, newContatiner);
    }

    public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        base.ApplyPalette(sLeaser, rCam, palette);
    }

    public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        base.DrawSprites(sLeaser, rCam, timeStacker, camPos);
    }
}
