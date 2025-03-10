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

namespace Scientist.Items;

class ExplosivePowder : ScavengerBomb
{
    public override bool HeavyWeapon
    {
        get
        {
            return false;
        }
    }

    public ExplosivePowder(AbstractPhysicalObject abstractPhysicalObject, World world) : base(abstractPhysicalObject, world)
    {
        base.bodyChunks = new BodyChunk[1];
        base.bodyChunks[0] = new BodyChunk(this, 0, new Vector2(0f, 0f), 3f, 0.1f);
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
        UnityEngine.Random.State state = UnityEngine.Random.state;
        UnityEngine.Random.InitState(abstractPhysicalObject.ID.RandomSeed);
        this.spikes = new float[UnityEngine.Random.Range(3, 8)];
        for (int i = 0; i < this.spikes.Length; i++)
        {
            this.spikes[i] = ((float)i + UnityEngine.Random.value) * (360f / (float)this.spikes.Length);
        }
        UnityEngine.Random.state = state;
    }

    public override void Update(bool eu)
    {
        base.Update(eu);
        if (this.Submersion > 0)
        {
            this.Dissolution();
        }
    }

    public new void Explode(BodyChunk hitChunk)
    {
        if (base.slatedForDeletetion)
        {
            return;
        }
        Vector2 vector = Vector2.Lerp(base.firstChunk.pos, base.firstChunk.lastPos, 0.35f);
        this.room.AddObject(new SootMark(this.room, vector, 80f, true));
        if (!this.explosionIsForShow)
        {
            this.room.AddObject(new Explosion(this.room, this, vector, 7, 160f, 6.2f, 0.98f, 140f, 0.1f, this.thrownBy, 0.7f, 160f, 1f));
        }
        this.room.AddObject(new Explosion.ExplosionLight(vector, 140f, 1f, 7, this.explodeColor));
        this.room.AddObject(new Explosion.ExplosionLight(vector, 110f, 1f, 3, new Color(1f, 1f, 1f)));
        this.room.AddObject(new ExplosionSpikes(this.room, vector, 14, 30f, 9f, 7f, 170f, this.explodeColor));
        this.room.AddObject(new ShockWave(vector, 200f, 0.045f, 5, false));
        for (int i = 0; i < 25; i++)
        {
            Vector2 vector2 = Custom.RNV();
            if (this.room.GetTile(vector + vector2 * 20f).Solid)
            {
                if (!this.room.GetTile(vector - vector2 * 20f).Solid)
                {
                    vector2 *= -1f;
                }
                else
                {
                    vector2 = Custom.RNV();
                }
            }
            for (int j = 0; j < 3; j++)
            {
                this.room.AddObject(new Spark(vector + vector2 * Mathf.Lerp(30f, 60f, UnityEngine.Random.value), vector2 * Mathf.Lerp(7f, 38f, UnityEngine.Random.value) + Custom.RNV() * 20f * UnityEngine.Random.value, Color.Lerp(this.explodeColor, new Color(1f, 1f, 1f), UnityEngine.Random.value), null, 11, 28));
            }
            this.room.AddObject(new Explosion.FlashingSmoke(vector + vector2 * 40f * UnityEngine.Random.value, vector2 * Mathf.Lerp(4f, 20f, Mathf.Pow(UnityEngine.Random.value, 2f)), 1f + 0.05f * UnityEngine.Random.value, new Color(1f, 1f, 1f), this.explodeColor, UnityEngine.Random.Range(3, 11)));
        }
        if (this.smoke != null)
        {
            for (int k = 0; k < 8; k++)
            {
                this.smoke.EmitWithMyLifeTime(vector + Custom.RNV(), Custom.RNV() * UnityEngine.Random.value * 17f);
            }
        }
        for (int l = 0; l < 6; l++)
        {
            this.room.AddObject(new ScavengerBomb.BombFragment(vector, Custom.DegToVec(((float)l + UnityEngine.Random.value) / 6f * 360f) * Mathf.Lerp(18f, 38f, UnityEngine.Random.value)));
        }
        this.room.ScreenMovement(new Vector2?(vector), default(Vector2), 1.3f);
        for (int m = 0; m < this.abstractPhysicalObject.stuckObjects.Count; m++)
        {
            this.abstractPhysicalObject.stuckObjects[m].Deactivate();
        }
        this.room.PlaySound(SoundID.Bomb_Explode, vector);
        this.room.InGameNoise(new InGameNoise(vector, 9000f, this, 1f));
        bool flag = hitChunk != null;
        for (int n = 0; n < 5; n++)
        {
            if (this.room.GetTile(vector + Custom.fourDirectionsAndZero[n].ToVector2() * 20f).Solid)
            {
                flag = true;
                break;
            }
        }
        if (flag)
        {
            if (this.smoke == null)
            {
                this.smoke = new BombSmoke(this.room, vector, null, this.explodeColor);
                this.room.AddObject(this.smoke);
            }
            if (hitChunk != null)
            {
                this.smoke.chunk = hitChunk;
            }
            else
            {
                this.smoke.chunk = null;
                this.smoke.fadeIn = 1f;
            }
            this.smoke.pos = vector;
            this.smoke.stationary = true;
            this.smoke.DisconnectSmoke();
        }
        else
        {
            this.smoke?.Destroy();
        }
        this.Destroy();
    }

    public void Dissolution()
    {
        if (this.grabbedBy.Count > 0)
        {
            for (int i = 0; i < this.grabbedBy.Count; i++)
            {
                this.grabbedBy[i].Release();
            }
        }
        Room room1 = this.room;
        room1.AddObject(new Explosion.ExplosionLight(this.firstChunk.pos, 90f, 1f, 7, this.explodeColor));
        room1.PlaySound(SoundID.Firecracker_Burn, base.firstChunk.pos);
        room1.abstractRoom.RemoveEntity(this.abstractPhysicalObject);
        this.Destroy();
    }

    public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[1];
        sLeaser.sprites[0] = new FSprite("Circle4", true);
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
        sLeaser.sprites[0].color = Color.red;
    }

    public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        Vector2 vector = Vector2.Lerp(base.firstChunk.lastPos, base.firstChunk.pos, timeStacker);
        Vector2 v = Vector3.Slerp(this.lastRotation, this.rotation, timeStacker);
        sLeaser.sprites[0].color = Color.red;
        for (int i = 0; i < sLeaser.sprites.Length; i++)
        {
            sLeaser.sprites[i].x = vector.x - camPos.x;
            sLeaser.sprites[i].y = vector.y - camPos.y;
            sLeaser.sprites[i].rotation = Custom.VecToDeg(v);
            sLeaser.sprites[i].scale = 3f;
        }
        if (this.blink > 0 && UnityEngine.Random.value < 0.5f)
        {
            sLeaser.sprites[0].color = base.blinkColor;
        }
        if (base.slatedForDeletetion || this.room != rCam.room)
        {
            sLeaser.CleanSpritesAndRemove();
        }
    }
}
