using IL.MoreSlugcats;
using RWCustom;
using Scientist;
using Scientist.Interface;
using System;
using UnityEngine;


namespace Scientist.Items;
public class InflatableGlowingShield : PlayerCarryableItem, IDrawable, IReflectionBeam
{
    public Vector2 rotation;
    public Vector2 lastRotation;
    public Vector2? setRotation;
    public float darkness;
    public float lastDarkness;
    public LightSource myLight;
    public float LightCounter;
    public float LightRad;

    public InflatableGlowingShield(AbstractPhysicalObject abstractPhysicalObject, World world) : base(abstractPhysicalObject)
    {
        base.bodyChunks = new BodyChunk[1];
        base.bodyChunks[0] = new BodyChunk(this, 0, new Vector2(0f, 0f), 18f, 0.3f);
        this.bodyChunkConnections = new PhysicalObject.BodyChunkConnection[0];
        base.airFriction = 0.999f;
        base.gravity = 0.9f;
        this.bounce = 0.2f;
        this.surfaceFriction = 0.7f;
        this.collisionLayer = 1;
        base.waterFriction = 0.95f;
        base.buoyancy = 1.1f;
        UnityEngine.Random.State state = UnityEngine.Random.state;
        UnityEngine.Random.InitState(abstractPhysicalObject.ID.RandomSeed);
        this.LightRad = UnityEngine.Random.Range(15f, 35f);
        UnityEngine.Random.state = state;
    }

    public override void Update(bool eu)
    {
        base.Update(eu);
        this.lastRotation = this.rotation;
        if (this.grabbedBy.Count > 0)
        {
            this.rotation = Vector2.up;
            this.bodyChunks[0].rad = 24f;
        }
        else
        {
            this.bodyChunks[0].rad = 18f;
        }
        if (this.setRotation != null)
        {
            this.rotation = this.setRotation.Value;
            this.setRotation = null;
        }
        float num = 7f + Mathf.Sin(this.LightCounter) * 0.3f;
        this.LightCounter += UnityEngine.Random.Range(0.01f, 0.2f);
        if (this.myLight != null && (this.myLight.room != this.room || !this.myLight.room.BeingViewed))
        {
            this.myLight.slatedForDeletetion = true;
            this.myLight = null;
        }
        if (this.myLight == null && this.room.BeingViewed)
        {
            this.LightCounter = UnityEngine.Random.Range(0f, 100f);
            this.myLight = new LightSource(base.firstChunk.pos, true, this.color, this);
            this.room.AddObject(this.myLight);
            this.myLight.colorFromEnvironment = false;
            this.myLight.noGameplayImpact = true;
            this.myLight.stayAlive = true;
            this.myLight.requireUpKeep = true;
            return;
        }
        if (this.myLight != null)
        {
            this.myLight.HardSetPos(base.firstChunk.pos);
            this.myLight.HardSetRad(this.LightRad * num);
            this.myLight.HardSetAlpha(Mathf.Lerp(0f, num / 2f, this.room.Darkness(this.myLight.Pos)) / 4f);
            if (this.myLight.rad > 5f)
            {
                this.myLight.stayAlive = true;
            }
        }
        if (base.firstChunk.ContactPoint.y != 0)
        {
            //this.rotationSpeed = (this.rotationSpeed * 2f + base.firstChunk.vel.x * 5f) / 3f;
        }
        if (base.firstChunk.ContactPoint.y < 0)
        {
            this.rotation = (this.rotation - Custom.PerpendicularVector(this.rotation) * 0.1f * base.firstChunk.vel.x).normalized;
            BodyChunk firstChunk = base.firstChunk;
            firstChunk.vel.x = firstChunk.vel.x * 0.8f;
        }
        if (UnityEngine.Random.value < 0.04f && base.Submersion > 0.5f && this.room.abstractRoom.creatures.Count > 0 && this.grabbedBy.Count == 0)
        {
            AbstractCreature abstractCreature = this.room.abstractRoom.creatures[UnityEngine.Random.Range(0, this.room.abstractRoom.creatures.Count)];
            if (abstractCreature.creatureTemplate.type == CreatureTemplate.Type.JetFish && abstractCreature.realizedCreature != null && !abstractCreature.realizedCreature.dead && (abstractCreature.realizedCreature as JetFish).AI.goToFood == null && (abstractCreature.realizedCreature as JetFish).AI.WantToEatObject(this))
            {
                (abstractCreature.realizedCreature as JetFish).AI.goToFood = this;
            }
        }
    }

    public void Burst()
    {
        ScientistLogger.Log($"InflatableGlowingShield {{ID={this.abstractPhysicalObject.ID}}} Burst");
    }

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
        if (grabbedBy.Count > 0)
        {
            Creature grabber = grabbedBy[0].grabber;
            Vector2 push = firstChunk.vel * firstChunk.mass / grabber.firstChunk.mass;
            grabber.firstChunk.vel += push;
        }
        firstChunk.vel = Vector2.zero;
    }

    public override void HitByExplosion(float hitFac, Explosion explosion, int hitChunk)
    {
        base.HitByExplosion(hitFac, explosion, hitChunk);
        if (grabbedBy.Count > 0)
        {
            Creature grabber = grabbedBy[0].grabber;
            Vector2 push = firstChunk.vel * firstChunk.mass / grabber.firstChunk.mass;
            grabber.firstChunk.vel += push;
        }
    }

    public void ThrowByPlayer()
    {
        firstChunk.vel = Vector2.zero;
    }

    public void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
    {
        newContatiner ??= rCam.ReturnFContainer("Items");
        for (int i = 0; i < sLeaser.sprites.Length; i++)
        {
            sLeaser.sprites[i].RemoveFromContainer();
            newContatiner.AddChild(sLeaser.sprites[i]);
        }
        /*rCam.ReturnFContainer("Foreground").AddChild(sLeaser.sprites[0]);//GrabShaders
        rCam.ReturnFContainer("Foreground").AddChild(sLeaser.sprites[1]);
        rCam.ReturnFContainer("Foreground").AddChild(sLeaser.sprites[2]);
        rCam.ReturnFContainer("Foreground").AddChild(sLeaser.sprites[3]);
        sLeaser.sprites[0].MoveInFrontOfOtherNode(sLeaser.sprites[4]);
        //sLeaser.sprites[1].MoveInFrontOfOtherNode(sLeaser.sprites[0]);
        sLeaser.sprites[2].MoveInFrontOfOtherNode(sLeaser.sprites[0]);
        sLeaser.sprites[3].MoveInFrontOfOtherNode(sLeaser.sprites[2]);*/
    }

    public void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        this.color = new Color(0.8f, 1f, 0.4f);
        sLeaser.sprites[0].color = palette.blackColor;
        sLeaser.sprites[1].color = Scientist.ScientistTools.ColorFromHex("81E629");
        sLeaser.sprites[2].color = Scientist.ScientistTools.ColorFromHex("5D7432");
        sLeaser.sprites[3].color = Scientist.ScientistTools.ColorFromHex("7A9E31");
        sLeaser.sprites[4].color = Scientist.ScientistTools.ColorFromHex("A5E81E");
        sLeaser.sprites[5].color = Color.white;
    }

    public void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        Vector2 vector = Vector2.Lerp(base.firstChunk.lastPos, base.firstChunk.pos, timeStacker);
        Vector2 vector2 = Vector3.Slerp(this.lastRotation, this.rotation, timeStacker);
        this.lastDarkness = this.darkness;
        this.darkness = rCam.room.Darkness(vector) * (1f - rCam.room.LightSourceExposure(vector));
        for (int i = 0; i < sLeaser.sprites.Length; i++)
        {
            sLeaser.sprites[i].x = vector.x - camPos.x;
            sLeaser.sprites[i].y = vector.y - camPos.y;
            sLeaser.sprites[i].rotation = Custom.VecToDeg(vector2);
            sLeaser.sprites[i].scale = 1.5f;
        }
        if (this.blink > 0 && UnityEngine.Random.value < 0.5f)
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
        if (base.slatedForDeletetion || this.room != rCam.room)
        {
            sLeaser.CleanSpritesAndRemove();
        }
    }

    public void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[6];
        sLeaser.sprites[0] = new FSprite("InflatableGlowingShieldA", true);
        sLeaser.sprites[1] = new FSprite("InflatableGlowingShieldB", true);
        sLeaser.sprites[2] = new FSprite("InflatableGlowingShieldC", true);
        sLeaser.sprites[3] = new FSprite("InflatableGlowingShieldD", true);
        sLeaser.sprites[4] = new FSprite("InflatableGlowingShieldE", true);
        sLeaser.sprites[5] = new FSprite("InflatableGlowingShieldF", true);
        this.AddToContainer(sLeaser, rCam, null);
    }

    public bool CanBlockBeam() => true;

    public void HitByBeam(Vector2 hitPointAngle, Vector2 hitAngle, float intensity)
    {
        return;
    }

    public bool CanReflect() => true;

    public Vector2[] ReflectionAngles(Vector2 incidentPointAngle, Vector2 incidentDirection, out float[] intensities)
    {
        Vector2 vertical = new Vector2(-incidentPointAngle.y, incidentPointAngle.x);
        Vector2 normalV = vertical.normalized * Mathf.Sign(Vector2.Dot(incidentDirection, vertical)) * Mathf.Sqrt(incidentDirection.sqrMagnitude - Mathf.Pow(Vector2.Dot(incidentPointAngle, incidentDirection), 2.00f) / incidentPointAngle.sqrMagnitude);
        intensities = new float[1] { 1f };
        return new Vector2[1] { -incidentDirection + 2 * normalV };
    }
}
