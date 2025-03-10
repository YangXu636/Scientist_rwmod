using Scientist.Items.AbstractPhysicalObjects;
using MoreSlugcats;
using RWCustom;
using Scientist;
using System;
using System.Data;
using System.Diagnostics;
using UnityEngine;

namespace Scientist.Items;


sealed class AnesthesiaNeedle : Spear
{
    public new Scientist.Items.AbstractPhysicalObjects.AnesthesiaNeedleAbstract abstractSpear
    {
        get
        {
            return this.abstractPhysicalObject as AnesthesiaNeedleAbstract;
        }
    }

    public new bool bugSpear
    {
        get
        {
            return false;
        }
    }

    public AnesthesiaNeedle(Scientist.Items.AbstractPhysicalObjects.AnesthesiaNeedleAbstract abstractPhysicalObject, World world) : base(abstractPhysicalObject, world)
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

    public override void Update(bool eu)
    {
        if (this.addPoles && this.room.readyForAI)
        {
            this.addPoles = false;
        }
        base.Update(eu);
    }

    public override bool HitSomething(SharedPhysics.CollisionResult result, bool eu)
    {
        if (result.obj is Creature)
        {
            Creature c = result.obj as Creature;
            this.spearDamageBonus *= 0.5f;
            if (c is Player)
            {
                this.spearDamageBonus *= 0;
            }
            if (!c.dead)
            {
                c.Stun(200);
                if (c is not Player)
                {
                    Scientist.Data.Player.anesthesiaCreatures[Scientist.ScientistTools.FeaturesTypeString(c)] = new Scientist.Data.AnesthesiaCreature((x, y) => x < y && ((1600.00f / x) * Mathf.Sin(x / 40.00f)) > 0, 800);
                }
            }
        }
        return base.HitSomething(result, eu);
    }

    public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        if (this.stuckIns != null)
        {
            rCam.ReturnFContainer("HUD").AddChild(this.stuckIns.label);
        }
        sLeaser.sprites = new FSprite[2];
        sLeaser.sprites[0] = new FSprite("SmallAnesthesiaSpearA", true);
        sLeaser.sprites[1] = new FSprite("SmallAnesthesiaSpearB", true);
        this.AddToContainer(sLeaser, rCam, null);
    }

    public override void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
    {
        base.AddToContainer(sLeaser, rCam, newContatiner);
        sLeaser.sprites[1].MoveInFrontOfOtherNode(sLeaser.sprites[0]);
    }

    public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        base.ApplyPalette(sLeaser, rCam, palette);
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
            sLeaser.sprites[i].x = vector.x - camPos.x + this.rotation.x * 7f;
            sLeaser.sprites[i].y = vector.y - camPos.y + this.rotation.x * 7f;
            sLeaser.sprites[i].anchorY = Mathf.Lerp(this.lastPivotAtTip ? 0.85f : 0.5f, this.pivotAtTip ? 0.85f : 0.5f, timeStacker);
            sLeaser.sprites[i].rotation = Custom.AimFromOneVectorToAnother(new Vector2(0f, 0f), vector2);
            sLeaser.sprites[i].scaleY = 0.6f;
        }
        sLeaser.sprites[0].color = (this.blink > 0 && UnityEngine.Random.value < 0.5f) ? base.blinkColor : Scientist.ScientistTools.ColorFromHex("F04D52");
        sLeaser.sprites[1].color = Scientist.ScientistTools.ColorFromHex("636363");
        if (base.slatedForDeletetion || this.room != rCam.room)
        {
            sLeaser.CleanSpritesAndRemove();
        }
    }
}
