using MoreSlugcats;
using RWCustom;
using System.Diagnostics;
using UnityEngine;

namespace items;

sealed class ShapeSpear : Spear
{
    

    public ShapeSpear(AbstractPhysicalObject abstractPhysicalObject, World world) : base(abstractPhysicalObject, world)
    {
        
    }

    /*public override void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
    {
        *//*newContatiner ??= rCam.ReturnFContainer("Midground");
        newContatiner.AddChild(sLeaser.sprites[0]);*//*
        newContatiner ??= rCam.ReturnFContainer("Items");
        for (int i = sLeaser.sprites.Length - 1; i >= 0; i--)
        {
            sLeaser.sprites[i].RemoveFromContainer();
            newContatiner.AddChild(sLeaser.sprites[i]);
        }
    }*/

    /*public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[1];
        sLeaser.sprites[0] = new FSprite("SmallSpear")
        {
            x = 100f,
            y = 100f
        };
        AddToContainer(sLeaser, rCam, null);
    }*/
        /*sLeaser.sprites = new FSprite[1];
        sLeaser.sprites[0] = new FSprite("Circle20");
        AddToContainer(sLeaser, rCam, null);*/

    /*public *//*override*//* void PlaceInRoom(Room placeRoom)
    {
        *//*//基本方法实际上只是将一个实体添加到房间中，如果你愿意，你可以手动完成
        base.PlaceInRoom(placeRoom);
        //根据抽象物理对象的坐标放置对象
        firstChunk.HardSetPosition(placeRoom.MiddleOfTile(abstractPhysicalObject.pos.Tile));
        //Custom.RNV() 只是设置一个随机方向
        rotation = Custom.RNV();
        lastRotation = rotation;*//*
    }*/




   /* public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        sLeaser.sprites[0].color = Color.white;
        *//*//You don't have to assign a value to this variable, you can specify the colors for each sprite separately
        color = Color.red;
        color = Color.Lerp(color, palette.blackColor, darkness); //The darker it is (the closer the darkness value is to 1f), the darker the sprite will be
        sLeaser.sprites[0].color = color;*//*
    }*/

    /*public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        *//*Vector2 pos = Vector2.Lerp(firstChunk.lastPos, firstChunk.pos, timeStacker);
        Vector2 rt = Vector3.Slerp(lastRotation, rotation, timeStacker);
        lastDarkness = darkness;
        //The formula for determining darkness is a template
        darkness = rCam.room.Darkness(pos) * (1f - rCam.room.LightSourceExposure(pos));
        if (darkness != lastDarkness)
            ApplyPalette(sLeaser, rCam, rCam.currentPalette);
        foreach (FSprite sprite in sLeaser.sprites)
        {
            sprite.x = pos.x - camPos.x;
            sprite.y = pos.y - camPos.y;
            sprite.rotation = Custom.VecToDeg(rt);
        }
        //If your object is PlayerCarryableItem, then when approaching an object, it can "flash" in one frame, hinting that it can be grabbed
        if (blink > 0 && Random.value < 0.5f)
            sLeaser.sprites[0].color = blinkColor;
        else sLeaser.sprites[0].color = color;
        if (slatedForDeletetion || rCam.room != room)
            sLeaser.CleanSpritesAndRemove();*//*
    }*/

    /*
    public override void Thrown(Creature thrownBy, Vector2 thrownPos, Vector2? firstFrameTraceFromPos, IntVector2 throwDir, float frc, bool eu)
    {
        base.Thrown(thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);
        if (this.bugSpear)
        {
            float value = Random.value;
            Room room = this.room;
            room?.PlaySound(MoreSlugcatsEnums.MSCSoundID.Throw_FireSpear, base.firstChunk.pos, 1f, Random.Range(0.8f, 1.2f));
        }
        else
        {
            Room room2 = this.room;
            if (room2 != null)
            {
                room2.PlaySound(SoundID.Slugcat_Throw_Spear, base.firstChunk);
            }
        }
        this.alwaysStickInWalls = true;
        if (this.bugSpear)
        {
            Room room3 = this.room;
            if (room3 != null)
            {
                room3.AddObject(new Explosion.ExplosionLight(base.firstChunk.pos, 280f, 1f, 7, Color.white));
            }
            Room room4 = this.room;
            if (room4 != null)
            {
                room4.AddObject(new ExplosionSpikes(this.room, base.firstChunk.pos, 14, 15f, 9f, 5f, 90f, Custom.HSL2RGB(Custom.Decimal(this.abstractSpear.hue + EggBugGraphics.HUE_OFF), 1f, 0.5f)));
            }
        }
        if (this.Spear_NeedleCanFeed())
        {
            Room room5 = this.room;
            if (room5 == null)
            {
                return;
            }
            room5.AddObject(new Spear.Umbilical(this.room, this, thrownBy as Player, base.firstChunk.vel));
        }
    }*/
}