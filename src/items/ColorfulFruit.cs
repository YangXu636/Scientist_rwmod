using IL.MoreSlugcats;
using RWCustom;
using Scientist;
using System;
using UnityEngine;


namespace items;
public class ColorfulFruit : PlayerCarryableItem, IDrawable, IPlayerEdible
{
    public Vector2 rotation;
    public Vector2 lastRotation;
    public Vector2? setRotation;
    public float darkness;
    public float lastDarkness;
    public bool normal;
    public int bites = 3;
    public int counter = 0;
    public readonly float colorSpeed = 0.1f;
    public float v;
    public float s;

    public Creature thrownBy;
    public LightSource light;
    public Vector2 flickerDir;
    public Vector2 lastFlickerDir;
    public float flashRad;
    public float lastFlashRad;
    public float flashAplha;
    public float lastFlashAlpha;
    public float burning;
    private bool charged;

    public AbstractConsumable AbstrConsumable
    {
        get
        {
            return this.abstractPhysicalObject as AbstractConsumable;
        }
    }

    public float LightIntensity
    {
        get
        {
            return Mathf.Pow(Mathf.Sin(this.burning * 3.1415927f), 0.4f);
        }
    }

    public ColorfulFruit(AbstractPhysicalObject abstractPhysicalObject, bool normal = true) : base(abstractPhysicalObject)
    {
        base.bodyChunks = new BodyChunk[1];
        base.bodyChunks[0] = new BodyChunk(this, 0, new Vector2(0f, 0f), 8f, 0.2f);
        this.bodyChunkConnections = new PhysicalObject.BodyChunkConnection[0];
        base.airFriction = 0.999f;
        base.gravity = 0.9f;
        this.bounce = 0.2f;
        this.surfaceFriction = 0.7f;
        this.collisionLayer = 1;
        base.waterFriction = 0.95f;
        base.buoyancy = 1.1f;
        this.v = UnityEngine.Random.Range(0.6f, 1f);
        this.s = UnityEngine.Random.Range(0.6f, 1f);
        this.normal = normal;
    }

    public override void Update(bool eu)
    {
        base.Update(eu);
        counter = counter >= 560 ? 0 : counter + 1;
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

        if (this.burning > 0f)
        {
            this.burning += 0.016666668f;
            if (this.burning > 1f)
            {
                this.Destroy();
            }
            this.lastFlickerDir = this.flickerDir;
            this.flickerDir = Custom.DegToVec(UnityEngine.Random.value * 360f) * 50f * this.LightIntensity;
            this.lastFlashAlpha = this.flashAplha;
            this.flashAplha = Mathf.Pow(UnityEngine.Random.value, 0.3f) * this.LightIntensity;
            this.lastFlashRad = this.flashRad;
            this.flashRad = Mathf.Pow(UnityEngine.Random.value, 0.3f) * this.LightIntensity * 200f * 16f;
            for (int i = 0; i < this.room.abstractRoom.creatures.Count; i++)
            {
                if (this.room.abstractRoom.creatures[i].realizedCreature != null && (Custom.DistLess(base.firstChunk.pos, this.room.abstractRoom.creatures[i].realizedCreature.mainBodyChunk.pos, this.LightIntensity * 600f) || (Custom.DistLess(base.firstChunk.pos, this.room.abstractRoom.creatures[i].realizedCreature.mainBodyChunk.pos, this.LightIntensity * 1600f) && this.room.VisualContact(base.firstChunk.pos, this.room.abstractRoom.creatures[i].realizedCreature.mainBodyChunk.pos))))
                {
                    if (this.room.abstractRoom.creatures[i].creatureTemplate.type == CreatureTemplate.Type.Spider && !this.room.abstractRoom.creatures[i].realizedCreature.dead)
                    {
                        this.room.abstractRoom.creatures[i].realizedCreature.firstChunk.vel += Custom.DegToVec(UnityEngine.Random.value * 360f) * UnityEngine.Random.value * 7f;
                        this.room.abstractRoom.creatures[i].realizedCreature.Die();
                    }
                    else if (this.room.abstractRoom.creatures[i].creatureTemplate.type == CreatureTemplate.Type.BigSpider)
                    {
                        (this.room.abstractRoom.creatures[i].realizedCreature as BigSpider).poison = 1f;
                        (this.room.abstractRoom.creatures[i].realizedCreature as BigSpider).State.health -= UnityEngine.Random.value * 0.2f;
                        this.room.abstractRoom.creatures[i].realizedCreature.Stun(UnityEngine.Random.Range(10, 20));
                        if (this.thrownBy != null)
                        {
                            this.room.abstractRoom.creatures[i].realizedCreature.SetKillTag(this.thrownBy.abstractCreature);
                        }
                    }
                    this.room.abstractRoom.creatures[i].realizedCreature.Blind((int)Custom.LerpMap(Vector2.Distance(base.firstChunk.pos, this.room.abstractRoom.creatures[i].realizedCreature.VisionPoint), 60f, 600f, 400f, 20f));

                }
            }
        }
    }

    public int BitesLeft => this.bites;

    public int FoodPoints => 1;

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
            if (!Scientist.ScientistPlayer.colorfulCreatures.ContainsKey(Scientist.ScientistTools.FeaturesTypeString(grasp.grabber as Player)))
            {
                Scientist.ScientistPlayer.colorfulCreatures[Scientist.ScientistTools.FeaturesTypeString(grasp.grabber as Player)] = new(grasp.grabber as Player);
            }
            Scientist.ScientistPlayer.colorfulCreatures[Scientist.ScientistTools.FeaturesTypeString(grasp.grabber as Player)].SetEnabled(true);
            ScientistLogger.Log($"Player ate a Colorful Fruit! {Scientist.ScientistTools.FeaturesTypeString(grasp.grabber as Player)}");
            (grasp.grabber as Player).mushroomCounter += 320;
            (grasp.grabber as Player).ObjectEaten(this);
            grasp.Release();
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
        if (this.normal)
        {
            sLeaser.sprites[2].RemoveFromContainer();
            sLeaser.sprites[3].RemoveFromContainer();
            rCam.ReturnFContainer("GrabShaders").AddChild(sLeaser.sprites[2]);
            rCam.ReturnFContainer("Water").AddChild(sLeaser.sprites[3]);
        }
    }

    public void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        for (int i = 0; i < sLeaser.sprites.Length; i++)
        {
            sLeaser.sprites[i].color = Color.HSVToRGB(this.counter / 560f, this.v, this.s);
        }
        sLeaser.sprites[2].alpha = 0.5f;
    }

    public void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        Vector2 vector = Vector2.Lerp(base.firstChunk.lastPos, base.firstChunk.pos, timeStacker);
        Vector2 v = Vector3.Slerp(this.lastRotation, this.rotation, timeStacker);
        this.lastDarkness = this.darkness;
        this.darkness = rCam.room.Darkness(vector) * (1f - rCam.room.LightSourceExposure(vector));
        this.ApplyPalette(sLeaser, rCam, rCam.currentPalette);
        for (int i = 0; i < sLeaser.sprites.Length; i++)
        {
            sLeaser.sprites[i].x = vector.x - camPos.x;
            sLeaser.sprites[i].y = vector.y - camPos.y;
            sLeaser.sprites[i].rotation = Custom.VecToDeg(v);
            if ( (i != 2 && i != 3 && this.normal) || ((i == 0 || i == 1) && !this.normal) )
            {
                sLeaser.sprites[i].element = Futile.atlasManager.GetElementWithName("ColorfulFruit" + Custom.IntClamp(3 - this.bites, 0, 2).ToString() + ((i == 0) ? "A" : "B"));
            }
        }
        sLeaser.sprites[2].scale = 3.5f;
        if (this.normal)
        {
            sLeaser.sprites[3].scale = 35f;
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
        if (this.normal)
        {
            sLeaser.sprites = new FSprite[4];
            sLeaser.sprites[0] = new FSprite("ColorfulFruit0A", true);
            sLeaser.sprites[1] = new FSprite("ColorfulFruit0B", true);
            sLeaser.sprites[2] = new FSprite("Futile_White", true);
            sLeaser.sprites[2].shader = rCam.game.rainWorld.Shaders["FlatLightBehindTerrain"];
            sLeaser.sprites[3] = new FSprite("Futile_White", true);
            sLeaser.sprites[3].shader = rCam.game.rainWorld.Shaders["LightSource"];
        }
        else
        {
            sLeaser.sprites = new FSprite[3];
            sLeaser.sprites[0] = new FSprite("ColorfulFruit0A", true);
            sLeaser.sprites[1] = new FSprite("ColorfulFruit0B", true);
            sLeaser.sprites[2] = new FSprite("Futile_White", true);
            sLeaser.sprites[2].shader = rCam.game.rainWorld.Shaders["FlareBomb"];
        }
        this.AddToContainer(sLeaser, rCam, null);
    }
}
