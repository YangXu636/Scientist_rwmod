using RWCustom;
using UnityEngine;
using MoreSlugcats;

namespace items;

sealed class ShapeSpear : Spear
{
    //这些是必要的变量，稍后对我们有用
    //public Vector2 rotation;
    //public Vector2 lastRotation;
    public float darkness;
    public float lastDarkness;

    public ShapeSpear(AbstractPhysicalObject abstr, World world) : base(abstr, world)
    {
        
    }

    public override void Thrown(Creature thrownBy, Vector2 thrownPos, Vector2? firstFrameTraceFromPos, IntVector2 throwDir, float frc, bool eu)
    {
        base.Thrown(thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);
        if (this.bugSpear)
        {
            float value = Random.value;
            Room room = this.room;
            if (room != null)
            {
                room.PlaySound(MoreSlugcatsEnums.MSCSoundID.Throw_FireSpear, base.firstChunk.pos, 1f, Random.Range(0.8f, 1.2f));
            }
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
    }
}