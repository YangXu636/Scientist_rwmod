using items.AbstractPhysicalObjects;
using MoreSlugcats;
using RWCustom;
using Scientist;
using System;
using System.Data;
using System.Diagnostics;
using UnityEngine;

namespace items;

class SmallRock : Weapon
{
    public override bool HeavyWeapon
    {
        get
        {
            return false;
        }
    }

    public SmallRock(AbstractPhysicalObject abstractPhysicalObject, World world) : base(abstractPhysicalObject, world)
    {
        base.bodyChunks = new BodyChunk[1];
        base.bodyChunks[0] = new BodyChunk(this, 0, new Vector2(0f, 0f), 2f, 0.02f);
        this.bodyChunkConnections = new PhysicalObject.BodyChunkConnection[0];
        base.airFriction = 0.999f;
        base.gravity = 0.9f;
        this.bounce = 0.4f;
        this.surfaceFriction = 0.4f;
        this.collisionLayer = 2;
        base.waterFriction = 0.98f;
        base.buoyancy = 0.4f;
        base.firstChunk.loudness = 9f;
        this.tailPos = base.firstChunk.pos;
        this.soundLoop = new ChunkDynamicSoundLoop(base.firstChunk);
    }

    public override void Update(bool eu)
    {
        base.Update(eu);
        this.soundLoop.sound = SoundID.None;
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
        if (base.firstChunk.ContactPoint.y != 0)
        {
            this.rotationSpeed = (this.rotationSpeed * 2f + base.firstChunk.vel.x * 5f) / 3f;
        }
    }

    public override bool HitSomething(SharedPhysics.CollisionResult result, bool eu)
    {
        if (result.obj == null)
        {
            return false;
        }
        this.vibrate = 10;
        this.ChangeMode(Weapon.Mode.Free);
        if (result.obj is Creature)
        {
            (result.obj as Creature).Violence(base.firstChunk, new Vector2?(base.firstChunk.vel * base.firstChunk.mass), result.chunk, result.onAppendagePos, Creature.DamageType.Blunt, 0.002f, 0f);
            if (result.obj is Lizard)
            {
                (result.obj as Lizard).spawnDataEvil += 0.4f;
            }
        }
        else if (result.chunk != null)
        {
            result.chunk.vel += base.firstChunk.vel * base.firstChunk.mass / result.chunk.mass;
        }
        else if (result.onAppendagePos != null)
        {
            (result.obj as PhysicalObject.IHaveAppendages).ApplyForceOnAppendage(result.onAppendagePos, base.firstChunk.vel * base.firstChunk.mass);
        }
        base.firstChunk.vel = base.firstChunk.vel * -0.5f + Custom.DegToVec(UnityEngine.Random.value * 360f) * Mathf.Lerp(0.1f, 0.4f, UnityEngine.Random.value) * base.firstChunk.vel.magnitude;
        this.room.PlaySound(SoundID.Rock_Hit_Creature, base.firstChunk);
        if (result.chunk != null)
        {
            this.room.AddObject(new ExplosionSpikes(this.room, result.chunk.pos + Custom.DirVec(result.chunk.pos, result.collisionPoint) * result.chunk.rad, 5, 2f, 4f, 4.5f, 30f, new Color(1f, 1f, 1f, 0.5f)));
        }
        this.SetRandomSpin();
        return true;
    }

    public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[2];
        UnityEngine.Random.State state = UnityEngine.Random.state;
        UnityEngine.Random.InitState(this.abstractPhysicalObject.ID.RandomSeed);
        sLeaser.sprites[0] = new FSprite("Pebble" + UnityEngine.Random.Range(1, 15).ToString(), true);
        UnityEngine.Random.state = state;
        TriangleMesh.Triangle[] tris = new TriangleMesh.Triangle[]
        {
            new TriangleMesh.Triangle(0, 1, 2)
        };
        TriangleMesh triangleMesh = new TriangleMesh("Futile_White", tris, false, false);
        sLeaser.sprites[1] = triangleMesh;
        this.AddToContainer(sLeaser, rCam, null);
    }

    public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        Vector2 vector = Vector2.Lerp(base.firstChunk.lastPos, base.firstChunk.pos, timeStacker);
        if (this.vibrate > 0)
        {
            vector += Custom.DegToVec(UnityEngine.Random.value * 360f) * 2f * UnityEngine.Random.value;
        }
        sLeaser.sprites[0].x = vector.x - camPos.x;
        sLeaser.sprites[0].y = vector.y - camPos.y;
        Vector3 vector2 = Vector3.Slerp(this.lastRotation, this.rotation, timeStacker);
        sLeaser.sprites[0].rotation = Custom.AimFromOneVectorToAnother(new Vector2(0f, 0f), vector2);
        if (base.mode == Weapon.Mode.Thrown)
        {
            sLeaser.sprites[1].isVisible = true;
            Vector2 vector3 = Vector2.Lerp(this.tailPos, base.firstChunk.lastPos, timeStacker);
            Vector2 vector4 = Custom.PerpendicularVector((vector - vector3).normalized);
            (sLeaser.sprites[1] as TriangleMesh).MoveVertice(0, vector + vector4 * 3f - camPos);
            (sLeaser.sprites[1] as TriangleMesh).MoveVertice(1, vector - vector4 * 3f - camPos);
            (sLeaser.sprites[1] as TriangleMesh).MoveVertice(2, vector3 - camPos);
        }
        else
        {
            sLeaser.sprites[1].isVisible = false;
        }
        if (this.blink > 0)
        {
            if (this.blink > 1 && UnityEngine.Random.value < 0.5f)
            {
                sLeaser.sprites[0].color = base.blinkColor;
            }
            else
            {
                sLeaser.sprites[0].color = this.color;
            }
        }
        else if (sLeaser.sprites[0].color != this.color)
        {
            sLeaser.sprites[0].color = this.color;
        }
        sLeaser.sprites[0].scale = sLeaser.sprites[1].scale = 0.7f;
        if (base.slatedForDeletetion || this.room != rCam.room)
        {
            sLeaser.CleanSpritesAndRemove();
        }
    }

    public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        this.color = palette.blackColor;
        sLeaser.sprites[0].color = this.color;
        sLeaser.sprites[1].color = this.color;
    }
}
