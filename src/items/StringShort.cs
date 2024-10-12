using items.AbstractPhysicalObjects;
using MoreSlugcats;
using RWCustom;
using System;
using System.Data;
using System.Diagnostics;
using UnityEngine;

namespace items;

sealed class StringShort : PlayerCarryableItem, IDrawable, IClimbableVine
{
    public items.AbstractPhysicalObjects.SharpSpearAbstract abstractSpear
    {
        get
        {
            return this.abstractPhysicalObject as SharpSpearAbstract;
        }
    }

    public StringShort(items.AbstractPhysicalObjects.SharpSpearAbstract abstractPhysicalObject, World world) : base(abstractPhysicalObject)
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

    public override void PickedUp(Creature upPicker)
    {
        //Console.WriteLine($"{(upPicker as Player).SlugCatClass}, {(upPicker as Player).SlugCatClass.value},  {(upPicker as Player).bodyMode}");
        //Player player = upPicker as Player;
        //if ( (player.isGourmand && player.gourmandExhausted) || (player.SlugCatClass.value == "xuyangjerry.Scientist") )
        base.PickedUp(upPicker);
    }

    public void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        throw new NotImplementedException();
    }

    public void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        throw new NotImplementedException();
    }

    public void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        throw new NotImplementedException();
    }

    public void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
    {
        throw new NotImplementedException();
    }

    public int TotalPositions()
    {
        throw new NotImplementedException();
    }

    public Vector2 Pos(int index)
    {
        throw new NotImplementedException();
    }

    public float Rad(int index)
    {
        throw new NotImplementedException();
    }

    public float Mass(int index)
    {
        throw new NotImplementedException();
    }

    public void Push(int index, Vector2 movement)
    {
        throw new NotImplementedException();
    }

    public void BeingClimbedOn(Creature crit)
    {
        throw new NotImplementedException();
    }

    public bool CurrentlyClimbable()
    {
        throw new NotImplementedException();
    }
}
