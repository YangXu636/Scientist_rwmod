using Scientist.Items.AbstractPhysicalObjects;
using MoreSlugcats;
using RWCustom;
using Scientist;
using System;
using System.Data;
using System.Diagnostics;
using UnityEngine;
using Smoke;
using static MonoMod.InlineRT.MonoModRule;

namespace Scientist.Items;

public class AirBag : Weapon
{
    public float darkness;
    public float lastDarkness;
    public bool isTriggered;
    public float expansionDegree;
    public Player ownerPlayer;
    public AirBreatherCreature ownerCreature;

    public float ScaleFactor => 0.40f + 6.60f * Mathf.Sin(Mathf.PI / 2.00f * this.expansionDegree);

    public AirBag(AbstractPhysicalObject abstractPhysicalObject, World world) : base(abstractPhysicalObject, world)
    {
        base.bodyChunks = new BodyChunk[1];
        base.bodyChunks[0] = new BodyChunk(this, 0, new Vector2(0f, 0f), this.ScaleFactor, 0.04f);
        this.bodyChunkConnections = new PhysicalObject.BodyChunkConnection[0];
        base.airFriction = 0.999f;
        base.gravity = 0.7f;
        this.bounce = 0.4f;
        this.surfaceFriction = 0.4f;
        this.collisionLayer = 2;
        base.waterFriction = 0.98f;
        base.buoyancy = 0.4f;
        base.firstChunk.loudness = 9f;
        this.isTriggered = false;
        this.expansionDegree = 0f;
        //this.ownerPlayer = null;
        this.ownerCreature = null;
    }

    public override void Update(bool eu)
    {
        base.Update(eu);
        this.lastRotation = this.rotation;
        if (this.grabbedBy.Count > 0)
        {
            this.rotation = Custom.PerpendicularVector(Custom.DirVec(base.firstChunk.pos, this.grabbedBy[0].grabber.mainBodyChunk.pos));
            this.rotation.y = Mathf.Abs(this.rotation.y);
            if (!this.isTriggered && this.grabbedBy[0].grabber is Player player)
            {
                this.ownerPlayer = player;
            }
            else if (!this.isTriggered && this.grabbedBy[0].grabber is AirBreatherCreature abCreature)
            {
                this.ownerCreature = abCreature;
            }
            if (this.isTriggered)
            {
                bool flag = false;
                for (int i = 0; i < this.grabbedBy.Count; i++)
                {
                    if (this.grabbedBy[i].grabber is Player player2 && player2 == this.ownerPlayer) { flag = true; }
                    this.grabbedBy[i].Release();
                }
                if (flag) { this.Explode(); }
            }
        }
        else if (!this.isTriggered)
        {
            this.ownerPlayer = null;
            this.ownerCreature = null;
        }
        if (this.setRotation != null)
        {
            this.rotation = this.setRotation.Value;
            this.setRotation = null;
        }
        if (base.firstChunk.ContactPoint.y != 0)
        {
            if (!this.isTriggered) { this.rotationSpeed = (this.rotationSpeed * 2f + base.firstChunk.vel.x * 5f) / 3f; }
        }
        if (base.firstChunk.ContactPoint.y < 0)
        {
            this.rotation = (this.rotation - Custom.PerpendicularVector(this.rotation) * 0.1f * base.firstChunk.vel.x).normalized;
            BodyChunk firstChunk = base.firstChunk;
            firstChunk.vel.x *= 0.8f;
        }
        if (this.isTriggered)
        {
            this.expansionDegree = Mathf.Min(1f, this.expansionDegree + Time.deltaTime * 2f);
            this.bodyChunks[0].rad = 2f * this.ScaleFactor;
            this.rotation = Vector2.right;
            if (this.Submersion > 0f)
            {
                this.bodyChunks[0].vel.y += 4f;
            }
            if (this.ownerPlayer != null)
            {
                this.ownerPlayer.mainBodyChunk.pos = this.bodyChunks[0].pos;
                this.ownerPlayer.airInLungs = 1f;
                this.bodyChunks[0].vel.x += this.ownerPlayer.input[0].x * 0.1f;
            }
            if (this.ownerCreature != null)
            {
                this.ownerCreature.mainBodyChunk.pos = this.bodyChunks[0].pos;
                this.ownerCreature.lungs = 1f;
            }
            if (this.Submersion <= 0f)
            {
                this.Explode();
            }
        }
        else
        {
            this.isTriggered = 
                (this.ownerPlayer != null && this.ownerPlayer.airInLungs < 0.15f)
                || (this.ownerCreature != null && this.ownerCreature.lungs < 0.15f)
                ;
            if (this.isTriggered && this.grabbedBy.Count > 0)
            {
                for (int i = 0; i < this.grabbedBy.Count; i++)
                {
                    this.grabbedBy[i].Release();
                }
            }
        }
    }

    public void Explode(float rad = 230f)
    {
        this.room.AddObject(new ShockWave(this.bodyChunks[0].pos, rad, 0.045f, 5, false));
        this.room.AddObject(new Explosion.ExplosionLight(this.bodyChunks[0].pos, rad, 1f, 3, Color.white));
        this.Destroy();
    }

    public override void PlaceInRoom(Room placeRoom)
    {
        base.PlaceInRoom(placeRoom);
        base.firstChunk.HardSetPosition(placeRoom.MiddleOfTile(this.abstractPhysicalObject.pos.Tile));
    }

    public override void TerrainImpact(int chunk, IntVector2 direction, float speed, bool firstContact)
    {
        base.TerrainImpact(chunk, direction, speed, firstContact);
    }

    public override void HitByWeapon(Weapon weapon)
    {
        base.HitByWeapon(weapon);
        if (weapon is Spear)
        {
            this.Explode(1000f);
        }
    }

    public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[5];
        sLeaser.sprites[0] = new FSprite("Futile_White", true);
        sLeaser.sprites[0].shader = rCam.game.rainWorld.Shaders["WaterNut"];
        sLeaser.sprites[0].scaleX = 1.2f;
        sLeaser.sprites[0].scaleY = 1.6f;
        sLeaser.sprites[1] = new FSprite("DangleFruit0A", true);
        sLeaser.sprites[2] = new FSprite("DangleFruit0B", true);
        for (int i = 1; i < 3; i++)
        {
            sLeaser.sprites[i].scaleX = 0.9f;
            sLeaser.sprites[i].scaleY = 1.3f;
        }
        sLeaser.sprites[3] = new FSprite("DangleFruit2A", true);
        sLeaser.sprites[3].scaleX = 1.1f;
        sLeaser.sprites[3].scaleY = -1.4f;
        sLeaser.sprites[4] = new FSprite("DangleFruit2A", true);
        sLeaser.sprites[4].scaleY = 1.4f;
        sLeaser.sprites[4].scaleX = 1.1f;
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
        rCam.ReturnFContainer("GrabShaders").AddChild(sLeaser.sprites[0]);
    }

    public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        Vector2 vector = Vector2.Lerp(base.firstChunk.lastPos, base.firstChunk.pos, timeStacker);
        Vector2 vector2 = Vector3.Slerp(this.lastRotation, this.rotation, timeStacker);
        sLeaser.sprites[0].x = vector.x - camPos.x;
        sLeaser.sprites[0].y = vector.y - camPos.y;
        sLeaser.sprites[0].rotation = Custom.VecToDeg(vector2);
        sLeaser.sprites[0].alpha = 0.6f + rCam.PaletteDarkness() / 2f;
        sLeaser.sprites[0].scaleX = /*1.2f **/ this.ScaleFactor;
        sLeaser.sprites[0].scaleY = /*1.6f **/ this.ScaleFactor;
        sLeaser.sprites[1].x = vector.x - camPos.x;
        sLeaser.sprites[1].y = vector.y - camPos.y;
        sLeaser.sprites[1].rotation = Custom.VecToDeg(vector2);
        sLeaser.sprites[1].scaleX = /*0.9f **/ this.ScaleFactor;
        sLeaser.sprites[1].scaleY = /*1.3f **/ this.ScaleFactor;
        sLeaser.sprites[2].x = vector.x - camPos.x;
        sLeaser.sprites[2].y = vector.y - camPos.y;
        sLeaser.sprites[2].rotation = Custom.VecToDeg(vector2);
        sLeaser.sprites[2].scaleX = /*0.9f **/ this.ScaleFactor;
        sLeaser.sprites[2].scaleY = /*1.3f **/ this.ScaleFactor;
        if (this.blink > 0 && UnityEngine.Random.value < 0.5f)
        {
            sLeaser.sprites[1].color = base.blinkColor;
            sLeaser.sprites[2].color = base.blinkColor;
            sLeaser.sprites[3].color = base.blinkColor;
            sLeaser.sprites[4].color = base.blinkColor;
        }
        else
        {
            sLeaser.sprites[1].color = this.color;
            sLeaser.sprites[2].color = this.color;
            sLeaser.sprites[3].color = Color.Lerp(this.color, rCam.currentPalette.blackColor, 0.4f);
            sLeaser.sprites[4].color = Color.Lerp(this.color, rCam.currentPalette.blackColor, 0.4f);
        }
        vector2 = Custom.DirVec(default(Vector2), vector2);
        sLeaser.sprites[3].x = vector.x + vector2.x * (10f * this.ScaleFactor) - camPos.x;
        sLeaser.sprites[3].y = vector.y + vector2.y * (10f * this.ScaleFactor) - camPos.y;
        sLeaser.sprites[3].rotation = sLeaser.sprites[0].rotation;
        sLeaser.sprites[4].x = vector.x + vector2.x * (-10f * this.ScaleFactor) - camPos.x;
        sLeaser.sprites[4].y = vector.y + vector2.y * (-10f * this.ScaleFactor) - camPos.y;
        sLeaser.sprites[4].rotation = sLeaser.sprites[0].rotation;
        if (base.slatedForDeletetion || this.room != rCam.room)
        {
            sLeaser.CleanSpritesAndRemove();
        }
    }

    public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        this.color = new Color(0.8f, 1f, 0.4f);
        sLeaser.sprites[0].color = Color.Lerp(palette.waterColor1, palette.waterColor2, 0.5f);
    }

    public class AbstractOnBackStick : AbstractPhysicalObject.AbstractObjectStick
    {
        public AbstractPhysicalObject Creature
        {
            get
            {
                return this.A;
            }
            set
            {
                this.A = value;
            }
        }

        public AbstractPhysicalObject Airbag
        {
            get
            {
                return this.B;
            }
            set
            {
                this.B = value;
            }
        }

        public AbstractOnBackStick(AbstractPhysicalObject creature, AbstractPhysicalObject airbag) : base(creature, airbag)
        {
        }

        // Token: 0x0600420E RID: 16910 RVA: 0x00498CDC File Offset: 0x00496EDC
        public override string SaveToString(int roomIndex)
        {
            return string.Concat(new string[]
            {
                roomIndex.ToString(),
                "<stkA>sprOnBackStick<stkA>",
                this.A.ID.ToString(),
                "<stkA>",
                this.B.ID.ToString()
            });
        }
    }
}
