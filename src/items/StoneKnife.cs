using Scientist.Items.AbstractPhysicalObjects;
using MoreSlugcats;
using RWCustom;
using Scientist;
using System;
using System.Data;
using System.Diagnostics;
using UnityEngine;
using System.Linq;

namespace Scientist.Items;


sealed class StoneKnife : Weapon
{

    public float damage = 0.1f;
    public int dc = 0;              //dc = 0.1s = 4
    public float darkness;
    public float lastDarkness;
    public float direction = 1f;

    public StoneKnifeAbstract abstractSk
    {
        get { return this.abstractPhysicalObject as StoneKnifeAbstract; }
    }

    public StoneKnife(Scientist.Items.AbstractPhysicalObjects.StoneKnifeAbstract abstractPhysicalObject, World world) : base(abstractPhysicalObject, world)
    {
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
    }

    public override void Update(bool eu)
    {
        base.Update(eu);
        if (this.dc > 0)
        {
            this.dc--;
        }
        if (base.firstChunk.ContactPoint.y != 0)
        {
            this.rotationSpeed = (this.rotationSpeed * 2f + base.firstChunk.vel.x * 5f) / 3f;
        }
        if (!this.grabbedBy.Empty())
        {
            if (this.grabbedBy[0].grabber is Player player)
            {
                this.direction = player.ThrowDirection;
                if (player.input[0].thrw)
                {
                    this.Attack(player, player.ThrowDirection);
                }
            }
        }
    }

    public override void PlaceInRoom(Room placeRoom)
    {
        base.PlaceInRoom(placeRoom);
        base.firstChunk.HardSetPosition(placeRoom.MiddleOfTile(this.abstractPhysicalObject.pos));
    }

    public void Attack(Player player, int direction)
    {
        if (this.dc > 0)
        {
            return;
        }
        ScientistLogger.Log("StoneKnife.Attack");
        if (player.animation == Player.AnimationIndex.Flip)
        {
            player.mainBodyChunk.vel += new Vector2(0f, direction * 10f);
        }
        else
        {
            player.mainBodyChunk.vel += new Vector2(direction * 10f, 5f);
        }
        this.dc = 4;
        player.room.PlaySound(SoundID.Spear_Thrown_Through_Air_LOOP, player.mainBodyChunk, false, 1.7f, 1f);
    }

    public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[4];
        sLeaser.sprites[0] = new FSprite("StoneKnife0", true);
        sLeaser.sprites[1] = new FSprite("StoneKnife1A", true);
        sLeaser.sprites[2] = new FSprite("StoneKnife2", true);
        sLeaser.sprites[3] = new FSprite("StoneKnife3", true);
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
        base.ApplyPalette(sLeaser, rCam, palette);
        sLeaser.sprites[0].color = palette.blackColor;
        sLeaser.sprites[1].color = Scientist.ScientistTools.ColorFromHex(sLeaser.sprites[2].isVisible ? "616161" : "1f1f1f");
        sLeaser.sprites[2].color = Scientist.ScientistTools.ColorFromHex("363636");
        sLeaser.sprites[3].color = Scientist.ScientistTools.ColorFromHex("1f1f1f");
    }

    public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        Vector2 vector = Vector2.Lerp(base.firstChunk.lastPos, base.firstChunk.pos, timeStacker);
        Vector2 v = Vector3.Slerp(this.lastRotation, this.rotation, timeStacker);
        this.lastDarkness = this.darkness;
        this.darkness = rCam.room.Darkness(vector) * (1f - rCam.room.LightSourceExposure(vector));
        for (int i = 0; i < sLeaser.sprites.Length; i++)
        {
            sLeaser.sprites[i].x = vector.x - camPos.x;
            sLeaser.sprites[i].y = vector.y - camPos.y;
            sLeaser.sprites[i].scale = this.direction;
            sLeaser.sprites[i].rotation = Custom.AimFromOneVectorToAnother(v, new Vector2(0f, 0f));
            if (i == 1) { sLeaser.sprites[i].element = Futile.atlasManager._allElementsByName.Keys.Contains($"StoneKnife1{(this.abstractSk.sharpen ? "A" : "B")}") ? Futile.atlasManager.GetElementWithName($"StoneKnife1{(this.abstractSk.sharpen ? "A" : "B")}") : Futile.atlasManager.GetElementWithName("Futile_White"); }
            if (i >= 2)
            {
                sLeaser.sprites[i].isVisible = this.abstractSk.sharpen;
            }
        }
        this.ApplyPalette(sLeaser, rCam, rCam.currentPalette);
        if (this.blink > 0 && UnityEngine.Random.value < 0.5f)
        {
            sLeaser.sprites[1].color = base.blinkColor;
        }
        if (base.slatedForDeletetion || this.room != rCam.room)
        {
            sLeaser.CleanSpritesAndRemove();
        }
    }
}
