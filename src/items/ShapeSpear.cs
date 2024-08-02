using MoreSlugcats;
using RWCustom;
using UnityEngine;

namespace items;

sealed class ShapeSpear : Spear
{
    public float darkness;
    public float lastDarkness;

    public ShapeSpear(AbstractPhysicalObject abstr, World world) : base(abstr, world)
    {
        bodyChunks = new BodyChunk[] { new BodyChunk(this, 0, Vector2.zero, 4, 0.05f) };
        //If there is only one BodyChunk, you need to create an empty array
        bodyChunkConnections = new BodyChunkConnection[0];
        gravity = 0.9f;
        airFriction = 0.999f;
        waterFriction = 0.9f;
        surfaceFriction = 0.5f;
        collisionLayer = 1;
        bounce = 0.15f;
        buoyancy = 0.9f;
    }

    public override void Update(bool eu)
    {
        base.Update(eu);
        lastRotation = rotation;
        /*
        if (grabbedBy.Count > 0) //如果有生物抓住了这个物体
        {
            //这样的数学不是必需的，但它是最正确的，它是一个模板
            rotation = Custom.PerpendicularVector(Custom.DirVec(firstChunk.pos, grabbedBy[0].grabber.mainBodyChunk.pos));
            //这行不允许你倒置物体
            rotation.y = Mathf.Abs(rotation.y);
        }
        if (firstChunk.contactPoint.y < 0) //如果物体接触到地面
        {
            //自定义。垂直矢量允许您以特定速度旋转对象，归一化后矢量的长度等于1，因为旋转指示方向
            rotation = (rotation - Custom.PerpendicularVector(rotation) * 0.1f * firstChunk.vel.x).normalized;
            //只是降低物体的速度：乘数越小，它就越快停止
            firstChunk.vel.x *= 0.8f;
        }
        */
    }

    public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[1];
        sLeaser.sprites[0] = new FSprite("Circle20");
        AddToContainer(sLeaser, rCam, null);
    }

    public override void PlaceInRoom(Room placeRoom)
    {
        //基本方法实际上只是将一个实体添加到房间中，如果你愿意，你可以手动完成
        base.PlaceInRoom(placeRoom);
        //根据抽象物理对象的坐标放置对象
        firstChunk.HardSetPosition(placeRoom.MiddleOfTile(abstractPhysicalObject.pos.Tile));
        //Custom.RNV() 只是设置一个随机方向
        rotation = Custom.RNV();
        lastRotation = rotation;
    }

    public override void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
    {
        newContatiner ??= rCam.ReturnFContainer("Midground");
        for (int i = sLeaser.sprites.Length - 1; i >= 0; i--)
        {
            sLeaser.sprites[i].RemoveFromContainer();
            newContatiner.AddChild(sLeaser.sprites[i]);
        }
    }

    public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        //You don't have to assign a value to this variable, you can specify the colors for each sprite separately
        color = Color.red;
        color = Color.Lerp(color, palette.blackColor, darkness); //The darker it is (the closer the darkness value is to 1f), the darker the sprite will be
        sLeaser.sprites[0].color = color;
    }

    public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        Vector2 pos = Vector2.Lerp(firstChunk.lastPos, firstChunk.pos, timeStacker);
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
            sLeaser.CleanSpritesAndRemove();
    }

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