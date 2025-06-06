﻿using IL.MoreSlugcats;
using RWCustom;
using Scientist;
using System;
using UnityEngine;


namespace Scientist.Items;
public class PainlessFruit : PlayerCarryableItem, IDrawable, IPlayerEdible
{
    public Vector2 rotation;
    public Vector2 lastRotation;
    public Vector2? setRotation;
    public float darkness;
    public float lastDarkness;
    public int bites = 3;

    public AbstractConsumable AbstrConsumable
    {
        get
        {
            return this.abstractPhysicalObject as AbstractConsumable;
        }
    }

    public PainlessFruit(AbstractPhysicalObject abstractPhysicalObject) : base(abstractPhysicalObject)
    {
        base.bodyChunks = new BodyChunk[1];
        base.bodyChunks[0] = new BodyChunk(this, 0, new Vector2(0f, 0f), 10f, 0.2f);
        this.bodyChunkConnections = new PhysicalObject.BodyChunkConnection[0];
        base.airFriction = 0.999f;
        base.gravity = 0.9f;
        this.bounce = 0.2f;
        this.surfaceFriction = 0.7f;
        this.collisionLayer = 1;
        base.waterFriction = 0.95f;
        base.buoyancy = 1.1f;
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
            firstChunk.vel.x = firstChunk.vel.x * 0.8f;
        }
    }

    public int BitesLeft => this.bites;

    public int FoodPoints => 0;

    public bool Edible => true;

    public bool AutomaticPickUp => true;

    public override void PlaceInRoom(Room placeRoom)
    {
        base.PlaceInRoom(placeRoom);
        base.firstChunk.HardSetPosition(placeRoom.MiddleOfTile(this.abstractPhysicalObject.pos));
        this.rotation = Custom.RNV();
        this.lastRotation = this.rotation;
    }

    public override void HitByWeapon(Weapon weapon)
    {
        base.HitByWeapon(weapon);
    }

    public void ThrowByPlayer()
    {
        
    }

    public void BitByPlayer(Creature.Grasp grasp, bool eu)
    {
        this.bites--;
        this.room.PlaySound((this.bites == 0) ? SoundID.Slugcat_Eat_Dangle_Fruit : SoundID.Slugcat_Bite_Dangle_Fruit, base.firstChunk.pos);
        base.firstChunk.MoveFromOutsideMyUpdate(eu, grasp.grabber.mainBodyChunk.pos);
        if (this.bites < 1)
        {
            (grasp.grabber as Player).ObjectEaten(this);
            grasp.Release();
            Scientist.Data.PlayerVariables.pfTime.AddAll(3200);
            Scientist.Data.PlayerVariables.pfEatTimesInACycle[ScientistTools.FeaturesTypeString(grasp.grabber as Player)] += 1;
            this.Destroy();
        }
    }

    public void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
    {
        newContatiner ??= rCam.ReturnFContainer("Items");
        for (int i = 0; i < sLeaser.sprites.Length; i++)
        {
            sLeaser.sprites[i].RemoveFromContainer();
            newContatiner.AddChild(sLeaser.sprites[i]);
        }
    }

    public void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        if (Scientist.Data.GolbalVariables.SEnableOldPf)
        {
            sLeaser.sprites[0].color = new Color(0.87f, 1f, 0.9f);
            sLeaser.sprites[1].color = new Color(0.95f, 0.85f, 1f);
            sLeaser.sprites[2].color = new Color(1f, 0f, 0f);
        }
        else
        {
            sLeaser.sprites[0].color = Color.white;
            sLeaser.sprites[1].color = ScientistTools.ColorFromHex("#00BBFF");
            sLeaser.sprites[2].color = ScientistTools.ColorFromHex("#52F218");
            sLeaser.sprites[3].color = ScientistTools.ColorFromHex("#C22FF7");
            sLeaser.sprites[4].color = ScientistTools.ColorFromHex("#F70A0A");
        }
        if (ModManager.MSC && rCam.room.game.session is StoryGameSession && rCam.room.world.name == "HR")
        {
            this.color = Color.Lerp(RainWorld.SaturatedGold, palette.blackColor, this.darkness);
            return;
        }
        this.color = Color.Lerp(new Color(0f, 0f, 1f), palette.blackColor, this.darkness);
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
        if (Scientist.Data.GolbalVariables.SEnableOldPf)
        {
            sLeaser.sprites[0].color = sLeaser.sprites[0].color.ColorChangeAlpha((this.bites >= 3) ? 1f : 0f);
            sLeaser.sprites[1].color = sLeaser.sprites[1].color.ColorChangeAlpha((this.bites >= 2) ? 1f : 0f);
            sLeaser.sprites[2].color = sLeaser.sprites[2].color.ColorChangeAlpha((this.bites >= 1) ? 1f : 0f);
        }
        else
        {
            sLeaser.sprites[0].color = sLeaser.sprites[0].color.ColorChangeAlpha((this.bites >= 1) ? 1f : 0f);
            sLeaser.sprites[1].color = sLeaser.sprites[1].color.ColorChangeAlpha((this.bites >= 1) ? 1f : 0f);
            sLeaser.sprites[2].color = sLeaser.sprites[2].color.ColorChangeAlpha((this.bites >= 1) ? 1f : 0f);
            sLeaser.sprites[3].color = sLeaser.sprites[3].color.ColorChangeAlpha((this.bites >= 2) ? 1f : 0f);
            sLeaser.sprites[4].color = sLeaser.sprites[4].color.ColorChangeAlpha((this.bites >= 3) ? 1f : 0f);
        }
        for (int i = 0; i < sLeaser.sprites.Length; i++)
        {
            sLeaser.sprites[i].x = vector.x - camPos.x;
            sLeaser.sprites[i].y = vector.y - camPos.y;
            sLeaser.sprites[i].rotation = Custom.VecToDeg(v);
        }
        if (this.blink > 0 && UnityEngine.Random.value < 0.5f)
        {
            sLeaser.sprites[1].color = base.blinkColor;
        }
        if (base.slatedForDeletetion || this.room != rCam.room)
        {
            sLeaser.CleanSpritesAndRemove();
        }
    }

    public void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        if (Scientist.Data.GolbalVariables.SEnableOldPf)
        {
            sLeaser.sprites = new FSprite[3];
            sLeaser.sprites[0] = new FSprite("old_PainlessFruitLeft", true);
            sLeaser.sprites[1] = new FSprite("old_PainlessFruitRight", true);
            sLeaser.sprites[2] = new FSprite("old_PainlessFruitHeart", true);
        }
        else
        {
            sLeaser.sprites = new FSprite[5];
            sLeaser.sprites[0] = new FSprite("PainlessFruitBottom", true);
            sLeaser.sprites[1] = new FSprite("PainlessFruitGap", true);
            sLeaser.sprites[2] = new FSprite("PainlessFruitLeft", true);
            sLeaser.sprites[3] = new FSprite("PainlessFruitRight", true);
            sLeaser.sprites[4] = new FSprite("PainlessFruitHeart", true);
        }
        this.AddToContainer(sLeaser, rCam, null);
    }
}
