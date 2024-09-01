using RWCustom;
using System.Collections.Generic;
using UnityEngine;

namespace items.ShapeSpears;

sealed class ShapeSpear : Spear
{
    private static float Rand => Random.value;

    public float rotVel;
    public float lastDarkness = -1f;
    public float darkness;
    public int craftCounter;
    public Color explodeColor = new Color(0.3f, 0.4f, 1f);
    public float damageTimes;
    public ChunkDynamicSoundLoop secondSoundLoop;

    new public float rotation;
    new public float lastRotation;

    public ShapeSpearAbstract Abstr { get; }

    public ShapeSpear(ShapeSpearAbstract abstr, Vector2 pos, Vector2 vel) : base(abstr, abstr.world)
    {
        this.Abstr = abstr;
        base.bodyChunks = new BodyChunk[1];
        base.bodyChunks[0] = new BodyChunk(this, 0, new Vector2(0f, 0f), 5f, 0.07f);
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
        this.jollyCustomColor = null;
        this.damageTimes = abstr.damageTimes;
    }

    public void HitEffect(Vector2 impactVelocity)
    {
        var num = Random.Range(3, 8);
        for (int k = 0; k < num; k++) {
            Vector2 pos = firstChunk.pos + Custom.DegToVec(Rand * 360f) * 5f * Rand;
            Vector2 vel = -impactVelocity * -0.1f + Custom.DegToVec(Rand * 360f) * Mathf.Lerp(0.2f, 0.4f, Rand) * impactVelocity.magnitude;
            room.AddObject(new Spark(pos, vel, new Color(1f, 1f, 1f), null, 10, 170));
        }

        room.AddObject(new StationaryEffect(firstChunk.pos, new Color(1f, 1f, 1f), null, StationaryEffect.EffectType.FlashingOrb));
    }

    public override void Thrown(Creature thrownBy, Vector2 thrownPos, Vector2? firstFrameTraceFromPos, IntVector2 throwDir, float frc, bool eu)
    {
        base.Thrown(thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);
    }

    public override bool HitSomething(SharedPhysics.CollisionResult result, bool eu)
    {
        bool flag = base.HitSomething(result, eu);
        bool flag2 = flag && result.obj is Creature;
        if (flag2)
        {
            this.spearDamageBonus *= this.damageTimes;
        }
        return flag;
    }

    public override void Update(bool eu)
    {
        base.Update(eu);

        if (this.stuckInWall != null)
        {

        }
        if (this.stuckInObject is Creature creature && !creature.dead)
        {

        }
    }

    public override void HitByWeapon(Weapon weapon)
    {
        base.HitByWeapon(weapon);
    }


    public override void ChangeMode(Mode newMode)
    { }

    public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        base.InitiateSprites(sLeaser, rCam);
    }

    public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        base.DrawSprites(sLeaser, rCam, timeStacker, camPos);
    }

    public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        Color waterShineColor = palette.waterShineColor;
        Color blackColor = palette.blackColor;
        this.color = Color.Lerp(waterShineColor, blackColor, 0.6f);
        sLeaser.sprites[0].color = this.color;
    }

    public override void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer? newContainer)
    {
        /*newContainer ??= rCam.ReturnFContainer("Items");

        foreach (FSprite fsprite in sLeaser.sprites) {
            fsprite.RemoveFromContainer();
            newContainer.AddChild(fsprite);
        }*/
        base.AddToContainer(sLeaser, rCam, newContainer);
    }
}