using Scientist.Items.AbstractPhysicalObjects;
using MoreSlugcats;
using RWCustom;
using Scientist;
using System;
using System.Data;
using System.Diagnostics;
using UnityEngine;
using Smoke;
using Noise;

namespace Scientist.Items;

public class TremblingFruit : Weapon
{
    public float darkness;
    public float lastDarkness;
    public float swingDegree = 0f;
    public Vector2 lastRotationAsSwing = Vector2.zero;
    public float sleepTimer = 0f;
    public float tremblingTimer = 0f;

    public Items.AbstractPhysicalObjects.TremblingFruitAbstract tremblingFruitAbstract { get => this.abstractPhysicalObject as Items.AbstractPhysicalObjects.TremblingFruitAbstract; }

    public TremblingFruit(Items.AbstractPhysicalObjects.TremblingFruitAbstract abstractPhysicalObject, World world) : base(abstractPhysicalObject, world)
    {
        base.bodyChunks = new BodyChunk[1];
        base.bodyChunks[0] = new BodyChunk(this, 0, new Vector2(0f, 0f), 16f, 0.04f);
        this.bodyChunkConnections = new PhysicalObject.BodyChunkConnection[0];
        base.airFriction = 0.999f;
        base.gravity = 0.7f;
        this.bounce = 0.4f;
        this.surfaceFriction = 0.4f;
        this.collisionLayer = 1;
        base.waterFriction = 0.98f;
        base.buoyancy = 0.4f;
        base.firstChunk.loudness = 9f;
        UnityEngine.Random.InitState(this.tremblingFruitAbstract.ID.RandomSeed);
    }

    public override void Update(bool eu)
    {
        base.Update(eu);
        if (base.mode == Weapon.Mode.Free && this.collisionLayer != 1)
        {
            base.ChangeCollisionLayer(1);
        }
        else if (base.mode != Weapon.Mode.Free && this.collisionLayer != 2)
        {
            base.ChangeCollisionLayer(2);
        }
        if (this.grabbedBy.Count > 0)
        {
            this.rotation = Custom.PerpendicularVector(Custom.DirVec(base.firstChunk.pos, this.grabbedBy[0].grabber.mainBodyChunk.pos));
            this.lastRotationAsSwing = this.rotation;
            this.rotation.y = Mathf.Abs(this.rotation.y);
        }
        if (this.setRotation != null)
        {
            this.rotation = this.setRotation.Value;
            this.lastRotationAsSwing = this.rotation;
            this.setRotation = null;
        }
        if (base.firstChunk.ContactPoint.y != 0)
        {
            this.rotationSpeed = (this.rotationSpeed * 2f + base.firstChunk.vel.x * 5f) / 3f;
        }
        if (base.firstChunk.ContactPoint.y < 0)
        {
            this.rotation = (this.rotation - Custom.PerpendicularVector(this.rotation) * 0.1f * base.firstChunk.vel.x).normalized;
            BodyChunk firstChunk = base.firstChunk;
            firstChunk.vel.x *= 0.8f;
        }
        this.lastRotation = this.rotation;
        if (this.sleepTimer > 0f)
        {
            this.sleepTimer -= 1.000f / 40.000f;
            if (this.sleepTimer <= 0f)
            {
                this.sleepTimer = 0f;
                this.bodyChunks[0].vel += (this.tremblingTimer > 0f ? UnityEngine.Random.Range(5, 10) : UnityEngine.Random.Range(1, 5)) * ScientistTools.RandomAngleVector2(new float[1][] { new float[2] { 0f, 180f } });
                this.lastRotationAsSwing = this.rotation;
            }
        }
        else
        {
            float degree = Mathf.PI / 12.000f * Mathf.Sin(2.000f * Mathf.PI * this.swingDegree * (this.tremblingTimer <= 0f ? 1f : 2f)) * (this.tremblingTimer <= 0f ? 1f : 3f);
            this.rotation = this.lastRotationAsSwing + new Vector2(Mathf.Cos(degree), Mathf.Sin(degree));
            this.swingDegree += Mathf.PI / 20.000f * UnityEngine.Random.Range(1, 5);
            if (this.swingDegree > Mathf.PI * 2f * UnityEngine.Random.Range(1, 5))
            {
                this.swingDegree = 0f;
                this.sleepTimer = this.tremblingTimer > 0f ? 1.000f / 20.000f : UnityEngine.Random.value > 0.7f ? UnityEngine.Random.Range(3f, 10f) : UnityEngine.Random.Range(0.3f, 3f);
            }
        }
        if (this.tremblingTimer > 0f)
        {
            this.tremblingTimer += 1.000f / 40.000f;
            for (int i = 0; i < this.room.abstractRoom.creatures.Count; i++)
            {
                if (this.room.abstractRoom.creatures[i].realizedCreature != null && (Custom.DistLess(base.firstChunk.pos, this.room.abstractRoom.creatures[i].realizedCreature.mainBodyChunk.pos, 40f) || (Custom.DistLess(base.firstChunk.pos, this.room.abstractRoom.creatures[i].realizedCreature.mainBodyChunk.pos, 120f) && this.room.VisualContact(base.firstChunk.pos, this.room.abstractRoom.creatures[i].realizedCreature.mainBodyChunk.pos))))
                {
                    this.room.abstractRoom.creatures[i].realizedCreature.Stun(40);
                }
                float rad = 250f;
                if (this.room.abstractRoom.creatures[i].creatureTemplate.type == CreatureTemplate.Type.Leech && this.room.abstractRoom.creatures[i].realizedCreature != null && this.room.abstractRoom.creatures[i].realizedCreature.room == this.room && Custom.DistLess(this.bodyChunks[0].pos, this.room.abstractRoom.creatures[i].realizedCreature.mainBodyChunk.pos, rad) && (Custom.DistLess(this.bodyChunks[0].pos, this.room.abstractRoom.creatures[i].realizedCreature.mainBodyChunk.pos, rad / 4f) || this.room.VisualContact(this.bodyChunks[0].pos, this.room.abstractRoom.creatures[i].realizedCreature.mainBodyChunk.pos)))
                {
                    (this.room.abstractRoom.creatures[i].realizedCreature as Leech).HeardSnailClick(this.bodyChunks[0].pos);
                }
            }
            if (tremblingTimer >= 10f)
            {
                this.Bomb();
            }
        }
    }

    public override void Thrown(Creature thrownBy, Vector2 thrownPos, Vector2? firstFrameTraceFromPos, IntVector2 throwDir, float frc, bool eu)
    {
        base.Thrown(thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);
        this.tremblingTimer = 1.000f / 40.000f;
        this.sleepTimer = 0f;
    }

    public void Bomb()
    {
        if (base.slatedForDeletetion)
        {
            return;
        }
        if (this.room.BeingViewed)
        {
            if (base.bodyChunks[0].submersion == 1f)
            {
                this.room.AddObject(new ShockWave(this.bodyChunks[0].pos, 400f, 0.07f, 9, false));
            }
            else
            {
                this.room.AddObject(new ShockWave(this.bodyChunks[0].pos, 330f, 0.07f, 6, false));
                for (int i = 0; i < 10; i++)
                {
                    this.room.AddObject(new WaterDrip(this.bodyChunks[0].pos, Custom.DegToVec(UnityEngine.Random.value * 360f) * Mathf.Lerp(4f, 21f, UnityEngine.Random.value), false));
                }
            }
        }
        if (ModManager.MSC && this.room.game.IsStorySession && this.room.game.GetStorySession.saveStateNumber == MoreSlugcatsEnums.SlugcatStatsName.Sofanthiel && this.room.world.region != null && this.room.world.region.name == "DS")
        {
            Vector2 vector = Vector2.Lerp(this.firstChunk.pos, base.firstChunk.lastPos, 0.35f);
            Color color = this.tremblingFruitAbstract.snailShellColors[0];
            this.room.AddObject(new Explosion(this.room, this, vector, 7, 280f, 4.2f, 50f, 280f, 0.25f, this.thrownBy, 0.7f, 160f, 1f));
            this.room.AddObject(new Explosion.ExplosionLight(vector, 280f, 1f, 7, color));
            this.room.AddObject(new Explosion.ExplosionLight(vector, 230f, 1f, 3, new Color(1f, 1f, 1f)));
            this.room.AddObject(new ExplosionSpikes(this.room, vector, 14, 30f, 9f, 7f, 170f, color));
            this.room.AddObject(new ShockWave(vector, 240f, 0.045f, 5, false));
            for (int j = 0; j < 10; j++)
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
                for (int k = 0; k < 3; k++)
                {
                    this.room.AddObject(new Spark(vector + vector2 * Mathf.Lerp(30f, 60f, UnityEngine.Random.value), vector2 * Mathf.Lerp(7f, 38f, UnityEngine.Random.value) + Custom.RNV() * 20f * UnityEngine.Random.value, Color.Lerp(color, new Color(1f, 1f, 1f), UnityEngine.Random.value), null, 11, 28));
                }
                this.room.AddObject(new Explosion.FlashingSmoke(vector + vector2 * 40f * UnityEngine.Random.value, vector2 * Mathf.Lerp(4f, 20f, Mathf.Pow(UnityEngine.Random.value, 2f)), 1f + 0.05f * UnityEngine.Random.value, new Color(1f, 1f, 1f), color, UnityEngine.Random.Range(3, 11)));
            }
            for (int l = 0; l < this.abstractPhysicalObject.stuckObjects.Count; l++)
            {
                this.abstractPhysicalObject.stuckObjects[l].Deactivate();
            }
            this.room.PlaySound(SoundID.Bomb_Explode, vector);
            this.room.InGameNoise(new InGameNoise(vector, 9000f, this, 1f));
        }
        this.room.PlaySound(SoundID.Snail_Pop, this.bodyChunks[0]);
        float num = 60f;
        for (int m = 0; m < this.room.physicalObjects.Length; m++)
        {
            foreach (PhysicalObject physicalObject in this.room.physicalObjects[m])
            {
                if (physicalObject != this)
                {
                    foreach (BodyChunk bodyChunk in physicalObject.bodyChunks)
                    {
                        float num2 = 1f + bodyChunk.submersion * this.bodyChunks[0].submersion * 4.5f;
                        if (Custom.DistLess(bodyChunk.pos, this.bodyChunks[0].pos, num * num2 + bodyChunk.rad + this.bodyChunks[0].rad) && this.room.VisualContact(bodyChunk.pos, this.bodyChunks[0].pos))
                        {
                            float num3 = Mathf.InverseLerp(num * num2 + bodyChunk.rad + this.bodyChunks[0].rad, (num * num2 + bodyChunk.rad + this.bodyChunks[0].rad) / 2f, Vector2.Distance(bodyChunk.pos, this.bodyChunks[0].pos));
                            bodyChunk.vel += Custom.DirVec(this.bodyChunks[0].pos + new Vector2(0f, base.IsTileSolid(0, 0, -1) ? -20f : 0f), bodyChunk.pos) * num3 * num2 * 3f / bodyChunk.mass;
                            if (ModManager.MSC && physicalObject is Player && (physicalObject as Player).SlugCatClass == MoreSlugcatsEnums.SlugcatStatsName.Saint)
                            {
                                (physicalObject as Player).SaintStagger(1500);
                            }
                            else if (physicalObject is Player)
                            {
                                (physicalObject as Player).Stun((int)(60f * num3));
                            }
                            else if (physicalObject is Creature)
                            {
                                (physicalObject as Creature).Stun((int)(((ModManager.MMF && MMF.cfgIncreaseStuns.Value) ? 300f : 60f) * num3));
                            }
                            if (physicalObject is Leech)
                            {
                                if (UnityEngine.Random.value < 0.033333335f || Custom.DistLess(this.bodyChunks[0].pos, bodyChunk.pos, this.bodyChunks[0].rad + bodyChunk.rad + 5f))
                                {
                                    (physicalObject as Leech).Die();
                                }
                                else
                                {
                                    (physicalObject as Leech).Stun((int)(num3 * bodyChunk.submersion * Mathf.Lerp(800f, 900f, UnityEngine.Random.value)));
                                }
                            }
                        }
                    }
                }
            }
        }
        if (this.room.waterObject != null)
        {
            float num4 = 1f + this.bodyChunks[0].submersion * 1.5f;
            this.room.waterObject.Explosion(this.bodyChunks[0].pos, num * num4 * 1.2f, num4 * 3f);
        }
        for (int num5 = 0; num5 < 1; num5++)
        {
            if (this.IsTileSolid(num5, 0, -1))
            {
                this.bodyChunks[num5].vel += Custom.DegToVec(-50f + 100f * UnityEngine.Random.value) * 10f;
            }
            else
            {
                this.bodyChunks[num5].vel += Custom.DegToVec(UnityEngine.Random.value * 360f) * 10f;
            }
        }
        //this.VibrateLeeches(1000f);
        for (int i = 0; i < this.room.abstractRoom.creatures.Count; i++)
        {
            if (this.room.abstractRoom.creatures[i].creatureTemplate.type == CreatureTemplate.Type.Leech)
            {
                (this.room.abstractRoom.creatures[i].realizedCreature as Leech).HeardSnailClick(this.bodyChunks[0].pos);
            }
        }
        this.Destroy();
    }

    public override void PlaceInRoom(Room placeRoom)
    {
        base.PlaceInRoom(placeRoom);
        base.firstChunk.HardSetPosition(placeRoom.MiddleOfTile(this.abstractPhysicalObject.pos.Tile));
        this.setRotation = new Vector2?(Vector2.zero);
    }

    public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[5];
        sLeaser.sprites[0] = new FSprite("SnailShellA", true);
        sLeaser.sprites[0].color = this.tremblingFruitAbstract.snailShellColors[0];
        sLeaser.sprites[1] = new FSprite("SnailShellB", true);
        sLeaser.sprites[1].color = this.tremblingFruitAbstract.snailShellColors[1];
        sLeaser.sprites[2] = new FSprite("TremblingFruitA", true);
        sLeaser.sprites[3] = new FSprite("TremblingFruitB", true);
        sLeaser.sprites[4] = new FSprite("TremblingFruitC", true);
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
        //rCam.ReturnFContainer("GrabShaders").AddChild(sLeaser.sprites[9]);
    }

    public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        Vector2 vector = Vector2.Lerp(base.firstChunk.lastPos, base.firstChunk.pos, timeStacker);
        Vector2 v = Vector3.Slerp(this.lastRotation, this.rotation, timeStacker);
        this.lastDarkness = this.darkness;
        this.darkness = rCam.room.Darkness(vector) * (1f - rCam.room.LightSourceExposure(vector));
        if (this.darkness != this.lastDarkness)
        {
            this.ApplyPalette(sLeaser, rCam, rCam.currentPalette);
        }
        for (int i = 0; i < sLeaser.sprites.Length; i++)
        {
            sLeaser.sprites[i].x = vector.x - camPos.x;
            sLeaser.sprites[i].y = vector.y - camPos.y;
            sLeaser.sprites[i].rotation = Custom.VecToDeg(v);
            if (i >= 2)
            {
                sLeaser.sprites[i].scale = 1.5f;
            }
        }
        if (this.blink > 0)
        {
            if (this.blink > 1 && UnityEngine.Random.value < 0.5f)
            {
                for (int i = 0; i < sLeaser.sprites.Length; i++)
                {
                    sLeaser.sprites[i].color = base.blinkColor;
                }
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

    public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        this.color = palette.blackColor;
        sLeaser.sprites[0].color = this.tremblingFruitAbstract.snailShellColors[0];
        sLeaser.sprites[1].color = this.tremblingFruitAbstract.snailShellColors[1];
        sLeaser.sprites[2].color = ScientistTools.ColorFromHex("#849a52");
        sLeaser.sprites[3].color = ScientistTools.ColorFromHex("#849a52");
        sLeaser.sprites[4].color = ScientistTools.ColorFromHex("#9bcf2b");
    }
}
