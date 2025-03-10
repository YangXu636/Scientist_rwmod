using Scientist.Items.AbstractPhysicalObjects;
using MoreSlugcats;
using RWCustom;
using Scientist;
using System;
using System.Data;
using System.Diagnostics;
using UnityEngine;
using Smoke;

namespace Scientist.Items;

public class AirBag : PlayerCarryableItem, IDrawable
{
    public Vector2 rotation;
    public Vector2 lastRotation;
    public Vector2? setRotation;
    public float prop;
    public float lastProp;
    public float propSpeed;
    public float darkness;
    public float lastDarkness;
    public float plop;
    public float lastPlop;
    public bool isTriggered;
    public float expansionDegree;

    public AirBag(Items.AbstractPhysicalObjects.BreathingBubbleAbstract abstractPhysicalObject) : base(abstractPhysicalObject)
    {
        base.bodyChunks = new BodyChunk[1];
        base.bodyChunks[0] = new BodyChunk(this, 0, new Vector2(0f, 0f), 9f, 0.04f);
        this.bodyChunkConnections = new PhysicalObject.BodyChunkConnection[0];
        base.airFriction = 0.999f;
        base.gravity = 0.7f;
        this.bounce = 0.4f;
        this.surfaceFriction = 0.4f;
        this.collisionLayer = 2;
        base.waterFriction = 0.98f;
        base.buoyancy = 0.4f;
        base.firstChunk.loudness = 9f;
        this.prop = 0f;
        this.lastProp = 0f;
        this.plop = this.lastPlop = 0f;
        this.isTriggered = false;
    }

    public override void Update(bool eu)
    {
        base.Update(eu);
        this.lastRotation = this.rotation;
        if (this.grabbedBy.Count > 0)
        {
            this.rotation = Custom.PerpendicularVector(Custom.DirVec(base.firstChunk.pos, this.grabbedBy[0].grabber.mainBodyChunk.pos));
            this.rotation.y = Mathf.Abs(this.rotation.y);
        }
        if (this.setRotation != null)
        {
            this.rotation = this.setRotation.Value;
            this.setRotation = null;
        }
        if (base.firstChunk.ContactPoint.y < 0)
        {
            this.rotation = (this.rotation - Custom.PerpendicularVector(this.rotation) * 0.1f * base.firstChunk.vel.x).normalized;
            BodyChunk firstChunk = base.firstChunk;
            firstChunk.vel.x *= 0.8f;
        }
        this.lastProp = this.prop;
        this.prop += this.propSpeed;
        this.propSpeed *= 0.85f;
        this.propSpeed -= this.prop / 10f;
        this.prop = Mathf.Clamp(this.prop, -15f, 15f);
        if (this.grabbedBy.Count == 0)
        {
            this.prop += (base.firstChunk.lastPos.x - base.firstChunk.pos.x) / 15f;
            this.prop -= (base.firstChunk.lastPos.y - base.firstChunk.pos.y) / 15f;
        }
        this.lastPlop = this.plop;
        if (this.plop > 0f && this.plop < 1f)
        {
            this.plop = Mathf.Min(1f, this.plop + 0.1f);
        }
    }

    public override void PlaceInRoom(Room placeRoom)
    {
        base.PlaceInRoom(placeRoom);
        base.firstChunk.HardSetPosition(placeRoom.MiddleOfTile(this.abstractPhysicalObject.pos.Tile));
    }

    public override void TerrainImpact(int chunk, IntVector2 direction, float speed, bool firstContact)
    {
        base.TerrainImpact(chunk, direction, speed, firstContact);
        if (direction.y != 0)
        {
            this.prop += speed;
            this.propSpeed += speed / 10f;
        }
        else
        {
            this.prop -= speed;
            this.propSpeed -= speed / 10f;
        }
        if (speed > 1.2f && firstContact)
        {
            Vector2 pos = base.firstChunk.pos + direction.ToVector2() * base.firstChunk.rad;
            for (int i = 0; i < Mathf.RoundToInt(Custom.LerpMap(speed, 1.2f, 6f, 2f, 5f, 1.2f)); i++)
            {
                this.room.AddObject(new WaterDrip(pos, Custom.RNV() * (2f + speed) * UnityEngine.Random.value * 0.5f + -direction.ToVector2() * (3f + speed) * 0.35f, true));
            }
            this.room.PlaySound(SoundID.Swollen_Water_Nut_Terrain_Impact, pos, Custom.LerpMap(speed, 1.2f, 6f, 0.2f, 1f), 1f);
        }
    }

    public void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[3];
        sLeaser.sprites[0] = new FSprite("JetFishEyeA", true);
        sLeaser.sprites[0].scaleX = 1.2f;
        sLeaser.sprites[0].scaleY = 1.4f;
        sLeaser.sprites[1] = new FSprite("tinyStar", true);
        sLeaser.sprites[1].scaleX = 1.5f;
        sLeaser.sprites[1].scaleY = 2.4f;
        sLeaser.sprites[2] = new FSprite("Futile_White", true);
        sLeaser.sprites[2].shader = rCam.game.rainWorld.Shaders["WaterNut"];
        this.AddToContainer(sLeaser, rCam, null);
    }

    public void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
    {
        newContatiner ??= rCam.ReturnFContainer("Items");
        for (int i = 0; i < sLeaser.sprites.Length; i++)
        {
            sLeaser.sprites[i].RemoveFromContainer();
        }
        newContatiner.AddChild(sLeaser.sprites[0]);
        newContatiner.AddChild(sLeaser.sprites[1]);
        rCam.ReturnFContainer("GrabShaders").AddChild(sLeaser.sprites[2]);
    }

    public void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        Vector2 vector = Vector2.Lerp(base.firstChunk.lastPos, base.firstChunk.pos, timeStacker);
        Vector2 v = Vector3.Slerp(this.lastRotation, this.rotation, timeStacker);
        this.lastDarkness = this.darkness;
        this.darkness = rCam.room.Darkness(vector) * (1f - rCam.room.LightSourceExposure(vector));
        if (this.darkness != this.lastDarkness)
        {
            this.ApplyPalette(sLeaser, rCam, rCam.currentPalette);
        }
        for (int i = 0; i < 3; i++)
        {
            sLeaser.sprites[i].x = vector.x - camPos.x;
            sLeaser.sprites[i].y = vector.y - camPos.y;
        }
        sLeaser.sprites[0].rotation = Custom.VecToDeg(v);
        sLeaser.sprites[1].rotation = Custom.VecToDeg(v);
        sLeaser.sprites[2].alpha = (1f - this.darkness) * (1f - base.firstChunk.submersion);
        float num = Mathf.Lerp(this.lastPlop, this.plop, timeStacker);
        num = Mathf.Lerp(0f, 1f + Mathf.Sin(num * 3.1415927f), num);
        sLeaser.sprites[0].scaleX = 1.2f * (0.5f + 2f * this.plop);
        sLeaser.sprites[0].scaleY = 1.4f * 2f;
        sLeaser.sprites[1].scaleX = 1.5f * 2f;
        sLeaser.sprites[1].scaleY = 2.4f * 2f;
        sLeaser.sprites[2].scaleX = (1.2f * Custom.LerpMap(3f, 3f, 1f, 1f, 0.2f) * 1f + Mathf.Lerp(this.lastProp, this.prop, timeStacker) / 20f);
        sLeaser.sprites[2].scaleY = (1.2f * Custom.LerpMap(3f, 3f, 1f, 1f, 0.2f) * 1f - Mathf.Lerp(this.lastProp, this.prop, timeStacker) / 20f);
        if (this.blink > 0 && UnityEngine.Random.value < 0.5f)
        {
            sLeaser.sprites[0].color = new Color(1f, 1f, 1f);
        }
        else
        {
            sLeaser.sprites[0].color = this.color;
        }
        if (base.slatedForDeletetion || this.room != rCam.room)
        {
            sLeaser.CleanSpritesAndRemove();
        }
    }

    public void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        this.color = palette.blackColor;
        sLeaser.sprites[1].color = Color.Lerp(Color.green, palette.blackColor, Mathf.Lerp(0f, 0.5f, rCam.PaletteDarkness()));
        sLeaser.sprites[2].color = Color.Lerp(palette.waterColor1, palette.waterColor2, 0.5f);
    }
}
