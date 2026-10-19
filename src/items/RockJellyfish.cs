using Scientist.Items.AbstractPhysicalObjects;
using MoreSlugcats;
using RWCustom;
using Scientist;
using UnityEngine;
using System;

namespace Scientist.Items;

class RockJellyfish : Weapon
{
    public float darkness;
    public float lastDarkness;
    private float deathDarken;
    public int electricCounter;

    public bool Electric
    {
        get
        {
            return this.electricCounter > 0;
        }
    }

    public override bool HeavyWeapon
    {
        get
        {
            return false;
        }
    }

    public RockJellyfish(AbstractPhysicalObject abstractPhysicalObject, World world) : base(abstractPhysicalObject, world)
    {
        base.bodyChunks = new BodyChunk[1];
        base.bodyChunks[0] = new BodyChunk(this, 0, new Vector2(0f, 0f), 9.5f, 0.22f);
        this.bodyChunkConnections = new PhysicalObject.BodyChunkConnection[0];
        base.airFriction = 0.999f;
        base.gravity = 0.9f;
        this.bounce = 0.4f;
        this.surfaceFriction = 0.4f;
        this.collisionLayer = 2;
        base.waterFriction = 0.98f;
        base.buoyancy = 0.4f;
        base.firstChunk.loudness = 9f;
        this.tailPos = base.firstChunk.pos;
        this.soundLoop = new ChunkDynamicSoundLoop(base.firstChunk);
    }

    public override void Update(bool eu)
    {
        base.Update(eu);
        this.soundLoop.sound = SoundID.None;
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
            BodyChunk firstChunk = base.firstChunk;
            firstChunk.vel.x = firstChunk.vel.x * 0.8f;
        }
        if (base.firstChunk.ContactPoint.y != 0)
        {
            this.rotationSpeed = (this.rotationSpeed * 2f + base.firstChunk.vel.x * 5f) / 3f;
        }

        if (this.Electric)
        {
            this.electricCounter--;
            if (this.electricCounter < 80)
            {
                this.thrownBy = null;
            }
        }
        base.CollideWithTerrain = (this.grabbedBy.Count == 0);
        if (this.room.game.devToolsActive && Input.GetKey("b"))
        {
            base.firstChunk.vel += Custom.DirVec(base.firstChunk.pos, Futile.mousePosition) * 3f;
        }
        this.lastRotation = this.rotation;
    }

    public override bool HitSomething(SharedPhysics.CollisionResult result, bool eu)
    {
        if (result.obj == null)
        {
            return false;
        }
        this.vibrate = 10;
        this.ChangeMode(Weapon.Mode.Free);
        if (result.obj is Creature creature && creature != this.thrownBy && this.Electric)
        {
            if (creature is not BigEel)
            {
                creature.Violence(base.firstChunk, new Vector2?(Custom.DirVec(base.firstChunk.pos, creature.bodyChunks[result.chunk.index].pos) * 5f), creature.bodyChunks[result.chunk.index], null, Creature.DamageType.Electric, 0.1f, (creature is Player) ? 140f : (320f * Mathf.Lerp(creature.Template.baseStunResistance, 1f, 0.5f)));
                this.room.AddObject(new CreatureSpasmer(creature, false, creature.stun));
                if (ModManager.MSC && creature is Player player && player.SlugCatClass == MoreSlugcatsEnums.SlugcatStatsName.Saint)
                {
                    player.SaintStagger(520);
                }
            }
            creature.Stun(Mathf.RoundToInt(Custom.LerpMap(creature.TotalMass, 0.1f, 5f, this.Electric ? 75f : 50f, 0f)));
            this.room.AddObject(new ShockWave(creature.bodyChunks[result.chunk.index].pos, Mathf.Lerp(40f, 60f, UnityEngine.Random.value), 0.07f, 6, false));
            this.room.PlaySound(SoundID.Jelly_Fish_Tentacle_Stun, base.firstChunk);
            this.room.AddObject(new Explosion.ExplosionLight(base.firstChunk.pos, 200f, 1f, 4, new Color(0.7f, 1f, 1f)));
            if (this.electricCounter > 5)
            {
                for (int i = 0; i < 15; i++)
                {
                    Vector2 a = Custom.DegToVec(360f * UnityEngine.Random.value);
                    this.room.AddObject(new MouseSpark(base.firstChunk.pos + a * 9f, base.firstChunk.vel + a * 36f * UnityEngine.Random.value, 20f, new Color(0.7f, 1f, 1f)));
                }
            }
            this.electricCounter = Math.Min(this.electricCounter, 5);
        }
        if (result.obj is Creature)
        {
            (result.obj as Creature).Violence(base.firstChunk, new Vector2?(base.firstChunk.vel * base.firstChunk.mass), result.chunk, result.onAppendagePos, Creature.DamageType.Blunt, 0.002f, 0f);
            if (result.obj is Lizard)
            {
                (result.obj as Lizard).spawnDataEvil += 0.4f;
            }
        }
        else if (result.chunk != null)
        {
            result.chunk.vel += base.firstChunk.vel * base.firstChunk.mass / result.chunk.mass;
        }
        else if (result.onAppendagePos != null)
        {
            (result.obj as PhysicalObject.IHaveAppendages).ApplyForceOnAppendage(result.onAppendagePos, base.firstChunk.vel * base.firstChunk.mass);
        }
        base.firstChunk.vel = base.firstChunk.vel * -0.5f + Custom.DegToVec(UnityEngine.Random.value * 360f) * Mathf.Lerp(0.1f, 0.4f, UnityEngine.Random.value) * base.firstChunk.vel.magnitude;
        this.room.PlaySound(SoundID.Rock_Hit_Creature, base.firstChunk);
        if (result.chunk != null)
        {
            this.room.AddObject(new ExplosionSpikes(this.room, result.chunk.pos + Custom.DirVec(result.chunk.pos, result.collisionPoint) * result.chunk.rad, 5, 2f, 4f, 4.5f, 30f, new Color(1f, 1f, 1f, 0.5f)));
        }
        this.SetRandomSpin();
        return true;
    }

    public override void Thrown(Creature thrownBy, Vector2 thrownPos, Vector2? firstFrameTraceFromPos, IntVector2 throwDir, float frc, bool eu)
    {
        base.Thrown(thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);
        this.Tossed(thrownBy);
    }

    public void Tossed(Creature tossedBy)
    {
        this.electricCounter = 120;
        this.thrownBy = tossedBy;
    }

    public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[4];
        sLeaser.sprites[0] = new FSprite("JellyFish0A", true);
        sLeaser.sprites[1] = new FSprite("JellyFish0B", true);
        sLeaser.sprites[2] = new FSprite("JetFishEyeA", true);
        sLeaser.sprites[2].scaleX = 0.45f;
        sLeaser.sprites[2].scaleY = 0.8f;
        UnityEngine.Random.State state = UnityEngine.Random.state;
        UnityEngine.Random.InitState(this.abstractPhysicalObject.ID.RandomSeed);
        sLeaser.sprites[3] = new FSprite("Pebble" + UnityEngine.Random.Range(1, 15).ToString(), true);
        UnityEngine.Random.state = state;
        this.AddToContainer(sLeaser, rCam, null);
    }

    public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        Vector2 vector = Vector2.Lerp(base.firstChunk.lastPos, base.firstChunk.pos, timeStacker);
        if (this.vibrate > 0)
        {
            vector += Custom.DegToVec(UnityEngine.Random.value * 360f) * 2f * UnityEngine.Random.value;
        }
        Vector3 vector2 = Vector3.Slerp(this.lastRotation, this.rotation, timeStacker);
        this.deathDarken = ModManager.MMF ? Mathf.Lerp(this.deathDarken, 1f, 0.025f) : 0f;
        this.lastDarkness = this.darkness;
        this.darkness = rCam.room.Darkness(vector) * (1f - rCam.room.LightSourceExposure(vector));
        if (this.darkness != this.lastDarkness)
        {
            this.ApplyPalette(sLeaser, rCam, rCam.currentPalette);
        }
        for (int i = 0; i < 2; i++)
        {
            sLeaser.sprites[i].x = vector.x - camPos.x;
            sLeaser.sprites[i].y = vector.y - camPos.y;
            sLeaser.sprites[i].rotation = Custom.VecToDeg(vector2);
        }
        Vector2 vector3 = vector + (Vector2)vector2 * 4f + Custom.DegToVec(-35f) * 3f;
        sLeaser.sprites[2].x = vector3.x - camPos.x;
        sLeaser.sprites[2].y = vector3.y - camPos.y;
        sLeaser.sprites[2].rotation = Custom.AimFromOneVectorToAnother(vector3, vector + (Vector2)vector2 * 9.5f);
        sLeaser.sprites[3].x = vector.x - camPos.x;
        sLeaser.sprites[3].y = vector.y - camPos.y;
        sLeaser.sprites[3].rotation = Custom.AimFromOneVectorToAnother(new Vector2(0f, 0f), vector2); 
        sLeaser.sprites[3].scale = 0.7f;
        if (this.Electric)
        {
            this.ApplyPalette(sLeaser, rCam, rCam.currentPalette);
        }
        if (this.blink > 0 && UnityEngine.Random.value < 0.5f)
        {
            if (this.blink > 1)
            {
                sLeaser.sprites[3].color = base.blinkColor;
            }
            else
            {
                sLeaser.sprites[3].color = rCam.currentPalette.blackColor;
            }
            sLeaser.sprites[1].color = base.blinkColor;
        }
        else
        {
            sLeaser.sprites[1].color = Color.Lerp(this.color, Color.black, this.deathDarken * 0.3f);
            if (this.deathDarken > 0f)
            {
                sLeaser.sprites[0].color = Color.Lerp(this.color, Color.black, this.deathDarken * 0.2f);
                sLeaser.sprites[2].color = Color.Lerp(this.color, Color.black, this.deathDarken * 0.1f);
            }
            sLeaser.sprites[3].color = rCam.currentPalette.blackColor;
        }
        if (base.slatedForDeletetion || this.room != rCam.room)
        {
            sLeaser.CleanSpritesAndRemove();
        }
    }

    public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        this.color = palette.blackColor;
        sLeaser.sprites[3].color = this.color;

        Color color = new Color(0.6f, 0.3f, 0.6f);
        float num = rCam.PaletteDarkness();
        Color color2 = Color.Lerp(Color.Lerp(palette.fogColor, color, 0.3f), palette.blackColor, Mathf.Pow(num, 2f));
        Color color3 = Color.Lerp(Color.Lerp(color2, new Color(1f, 1f, 1f), 0.5f), palette.blackColor, Mathf.Pow(num, 2f));
        Color color4 = Color.Lerp(color, palette.blackColor, num);
        if (this.electricCounter > 5)
        {
            color4 = Color.Lerp(color4, new Color(1f, 1f, 1f), UnityEngine.Random.value);
            color2 = Color.Lerp(color2, new Color(1f, 1f, 1f), UnityEngine.Random.value);
            color3 = Color.Lerp(color3, new Color(1f, 1f, 1f), UnityEngine.Random.value);
        }
        sLeaser.sprites[0].color = color3;
        this.color = color2;
        sLeaser.sprites[2].color = color3 + new Color(0.2f, 0.2f, 0.2f) * (1f - Mathf.Pow(num, 4f));
    }
}
