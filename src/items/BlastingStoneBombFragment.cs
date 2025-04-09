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

class BlastingStoneBombFragment : Weapon
{
    public BodyChunk hitChunk;
    public Color explodeColor = new Color(1f, 0.4f, 0.3f);
    public BombSmoke smoke;
    public bool explosionIsForShow = false;
    public float ignition = 0f;

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
        base.bodyChunks[0] = new BodyChunk(this, 0, new Vector2(0f, 0f), 3f, 0.01f);
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
        this.soundLoop.sound = SoundID.None;
        if (base.mode == Weapon.Mode.Free && this.collisionLayer != 1)
        {
            base.ChangeCollisionLayer(1);
        }
        else if (base.mode != Weapon.Mode.Free && this.collisionLayer != 2)
        {
            base.ChangeCollisionLayer(2);
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
        if (this.grabbedBy.Count > 0)
        {
            this.ignition = 0f;
        }
        if (base.firstChunk.ContactPoint.y != 0)
        {
            this.rotationSpeed = (this.rotationSpeed * 2f + base.firstChunk.vel.x * 5f) / 3f;
        }
        if (this.ignition > 0.00f)
        {
            for (int i = 0; i < 3; i++)
            {
                this.room.AddObject(new Spark(Vector2.Lerp(base.firstChunk.lastPos, base.firstChunk.pos, UnityEngine.Random.value), base.firstChunk.vel * 0.1f + Custom.RNV() * 3.2f * UnityEngine.Random.value, this.explodeColor, null, 7, 30));
            }
            if (this.smoke == null)
            {
                this.smoke = new BombSmoke(this.room, base.firstChunk.pos, base.firstChunk, this.explodeColor);
                this.room.AddObject(this.smoke);
            }
        }
        else
        {
            this.smoke?.Destroy();
            this.smoke = null;
        }
        if (this.ignition > 0.0f && this.ignition < 0.8f)
        {
            this.ignition += 0.05f;
            if (this.Submersion > 0.0f) { this.ignition = 0.0f; }
        }
        if (this.ignition >= 0.8f)
        {
            this.Bomb();
        }
    }

    public override void TerrainImpact(int chunk, IntVector2 direction, float speed, bool firstContact)
    {
        base.TerrainImpact(chunk, direction, speed, firstContact);
        if (this.floorBounceFrames > 0 && (direction.x == 0 || this.room.GetTile(base.firstChunk.pos).Terrain == Room.Tile.TerrainType.Slope))
        {
            return;
        }
        if (this.ignition > 0.00f)
        {
            this.Explode(null);
        }
    }

    public override bool HitSomething(SharedPhysics.CollisionResult result, bool eu)
    {
        if (result.obj == null)
        {
            return false;
        }
        this.vibrate = 20;
        this.ChangeMode(Weapon.Mode.Free);
        if (result.obj is Creature)
        {
            (result.obj as Creature).Violence(base.firstChunk, new Vector2?(base.firstChunk.vel * base.firstChunk.mass), result.chunk, result.onAppendagePos, Creature.DamageType.Explosion, 0.8f, 85f);
        }
        else if (result.chunk != null)
        {
            result.chunk.vel += base.firstChunk.vel * base.firstChunk.mass / result.chunk.mass;
        }
        else if (result.onAppendagePos != null)
        {
            (result.obj as PhysicalObject.IHaveAppendages).ApplyForceOnAppendage(result.onAppendagePos, base.firstChunk.vel * base.firstChunk.mass);
        }
        this.Explode(result.chunk);
        return true;
    }

    public override void Thrown(Creature thrownBy, Vector2 thrownPos, Vector2? firstFrameTraceFromPos, IntVector2 throwDir, float frc, bool eu)
    {
        base.Thrown(thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);
        Room room = this.room;
        room?.PlaySound(SoundID.Slugcat_Throw_Bomb, base.firstChunk);
        this.SetIgnition(0.01f);
    }

    public override void PickedUp(Creature upPicker)
    {
        this.room.PlaySound(SoundID.Slugcat_Pick_Up_Bomb, base.firstChunk);
    }

    public override void HitByWeapon(Weapon weapon)
    {
        if (weapon.mode == Weapon.Mode.Thrown && this.thrownBy == null && weapon.thrownBy != null)
        {
            this.thrownBy = weapon.thrownBy;
        }
        base.HitByWeapon(weapon);
        this.SetIgnition(0.01f);
    }

    public override void WeaponDeflect(Vector2 inbetweenPos, Vector2 deflectDir, float bounceSpeed)
    {
        base.WeaponDeflect(inbetweenPos, deflectDir, bounceSpeed);
        if (UnityEngine.Random.value < 0.5f)
        {
            this.Explode(null);
            return;
        }
        this.SetIgnition(0.01f);
    }

    public override void HitByExplosion(float hitFac, Explosion explosion, int hitChunk)
    {
        base.HitByExplosion(hitFac, explosion, hitChunk);
        this.bodyChunks[0].vel += hitFac * 5.0f * (this.bodyChunks[0].pos - explosion.pos);
    }

    public void Explode(BodyChunk hitChunk)
    {
        if (this.ignition > 0.0f)
        {
            return;
        }
        this.SetIgnition(0.01f);
        this.hitChunk = hitChunk;
    }

    public void SetIgnition(float ignition)
    {
        if (this.ignition != 0f) { return; }
        this.ignition = ignition;
    }

    public void Bomb()
    {
        if (base.slatedForDeletetion)
        {
            return;
        }
        Vector2 vector = Vector2.Lerp(base.firstChunk.pos, base.firstChunk.lastPos, 0.35f);
        this.room.AddObject(new SootMark(this.room, vector, 80f, true));
        if (!this.explosionIsForShow)
        {
            this.room.AddObject(new Explosion(this.room, this, vector, 7, 100f, 6.2f, 0.2f, 140f, 0.1f, this.thrownBy, 0.7f, 160f, 1f));
        }
        this.room.AddObject(new Explosion.ExplosionLight(vector, 80f, 1f, 7, this.explodeColor));
        this.room.AddObject(new Explosion.ExplosionLight(vector, 60f, 1f, 3, new Color(1f, 1f, 1f)));
        this.room.AddObject(new ExplosionSpikes(this.room, vector, 14, 30f, 9f, 7f, 170f, this.explodeColor));
        this.room.AddObject(new ShockWave(vector, 130f, 0.045f, 5, false));
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
        this.room.ScreenMovement(new Vector2?(vector), default(Vector2), 1.1f);
        for (int m = 0; m < this.abstractPhysicalObject.stuckObjects.Count; m++)
        {
            this.abstractPhysicalObject.stuckObjects[m].Deactivate();
        }
        this.room.PlaySound(SoundID.Bomb_Explode, vector);
        this.room.InGameNoise(new InGameNoise(vector, 9000f, this, 1f));
        bool flag = this.hitChunk != null;
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
            if (this.hitChunk != null)
            {
                this.smoke.chunk = this.hitChunk;
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
        this.ignition = float.MinValue;
    }

    public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[4];
        sLeaser.sprites[0] = new FSprite("ExpansionBombA", true);
        sLeaser.sprites[1] = new FSprite("ExpansionBombB", true);
        sLeaser.sprites[2] = new FSprite("ExpansionBombC", true);
        sLeaser.sprites[3] = new FSprite("ExpansionBombD", true);
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
        sLeaser.sprites[0].color = palette.blackColor;
        sLeaser.sprites[1].color = ScientistTools.ColorFromHex("#4123a5");
        sLeaser.sprites[2].color = ScientistTools.ColorFromHex("#fa5260");
        sLeaser.sprites[3].color = ScientistTools.ColorFromHex("#fc475c");
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
                sLeaser.sprites[2].color = base.blinkColor;
                sLeaser.sprites[3].color = base.blinkColor;
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
