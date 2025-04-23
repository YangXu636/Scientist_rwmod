using RWCustom;
using UnityEngine;
using Watcher;

namespace Scientist.Effects;

public class BlizzardBeamWithoutLizard : UpdatableAndDeletable, IDrawable
{
    public Vector2 pos;
    public Vector2 lastPos;
    public Vector2 hitPos;
    public Vector2 lastHitPos;
    public Vector2 angle;
    public bool isVisible;
    public float size;
    public float lastSize;
    public int time;
    public Lizard lizard;
    public BlizzardBeam blizzardBeam;

    public BlizzardBeamWithoutLizard(Lizard owner, Vector2 startPos, Vector2 angle)
    {
        this.lizard = owner;
        this.pos = startPos;
        this.lastPos = startPos;
        this.angle = angle;
        this.isVisible = true;
    }

    public void Update()
    {
        this.lastHitPos = this.hitPos;
        this.lastSize = this.size;
        this.size = Mathf.Abs(Mathf.Sin((float)this.time * 0.44f));
        this.hitPos = this.lizard.blizzardModule.RayTraceBeamHitPos();
    }

    public void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites[0] = new FSprite("pixel", true);
        sLeaser.sprites[0].shader = rCam.game.rainWorld.Shaders["HologramBehindTerrain"];
        sLeaser.sprites[0].scaleX = 44f;
        sLeaser.sprites[0].anchorY = 0f;
        sLeaser.sprites[0].alpha = 0.5f;
        sLeaser.sprites[1] = new FSprite("pixel", true);
        sLeaser.sprites[1].shader = rCam.game.rainWorld.Shaders["HologramBehindTerrain"];
        sLeaser.sprites[1].scaleX = 18f;
        sLeaser.sprites[1].anchorY = 0f;
        sLeaser.sprites[1].alpha = 0.8f;
        sLeaser.sprites[2] = new FSprite("pixel", true);
        sLeaser.sprites[2].shader = rCam.game.rainWorld.Shaders["HologramBehindTerrain"];
        sLeaser.sprites[2].scaleX = 5f;
        sLeaser.sprites[2].anchorY = 0f;
        sLeaser.sprites[2].alpha = 1f;
    }

    public void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        Vector2 vector = Vector2.Lerp(this.lastPos, this.pos, timeStacker);
        Vector2 vector2 = Vector2.Lerp(this.lastHitPos, this.hitPos, timeStacker);
        Vector2 v = Custom.DirVec(vector, vector2);
        float num = Mathf.Lerp(this.lastSize, this.size, timeStacker);
        for (int i = 0; i < sLeaser.sprites.Length; i++)
        {
            sLeaser.sprites[i].x = vector.x - camPos.x;
            sLeaser.sprites[i].y = vector.y - camPos.y;
            sLeaser.sprites[i].rotation = Custom.VecToDeg(v);
            sLeaser.sprites[i].scaleY = Vector2.Distance(vector, vector2);
            sLeaser.sprites[i].isVisible = this.isVisible;
        }
        sLeaser.sprites[0].scaleX = 40f + 20f * num;
        sLeaser.sprites[1].scaleX = 25f - 10f * num;
        sLeaser.sprites[2].scaleX = 5f + 5f * num; 
        if (base.slatedForDeletetion || this.room != rCam.room)
        {
            sLeaser.CleanSpritesAndRemove();
        }
    }

    public void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        Color color = new Color(0.9f, 0.85f, 1f);
        sLeaser.sprites[2].color = color;
        sLeaser.sprites[1].color = Color.Lerp(color, palette.fogColor, 0.2f);
        sLeaser.sprites[0].color = Color.Lerp(color, palette.fogColor, 0.5f);
    }

    public void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
    {
        newContatiner ??= rCam.ReturnFContainer("Midground");
    }
}