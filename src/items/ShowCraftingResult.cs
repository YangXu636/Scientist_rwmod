using items.AbstractPhysicalObjects;
using MoreSlugcats;
using RWCustom;
using Scientist;
using System;
using System.Data;
using System.Diagnostics;
using UnityEngine;

namespace items;

class ShowCraftingResult : UpdatableAndDeletable, IDrawable
{
    Player player;
    string spriteName;
    Color spriteColor;
    Vector2 offsetV2;
    float offsetR;

    int counter = 0;

    public ShowCraftingResult(Player player, string spriteName, Color spriteColor, Vector2 offsetV2, float r)
    {
        this.player = player;
        this.spriteName = spriteName;
        this.spriteColor = spriteColor;
        this.offsetV2 = offsetV2;
        this.offsetR = r;
    }

    public override void Update(bool eu)
    {
        base.Update(eu);
        counter = counter > 30 ? counter : counter + 1;
    }

    public void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
    {
        newContatiner ??= rCam.ReturnFContainer("HUD");
        for (int i = 0; i < sLeaser.sprites.Length; i++)
        {
            sLeaser.sprites[i].RemoveFromContainer();
            newContatiner.AddChild(sLeaser.sprites[i]);
        }
    }

    public void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        sLeaser.sprites[0].color = Scientist.ScientistTools.ColorContrast(this.spriteColor);
        sLeaser.sprites[1].color = this.spriteColor;
    }

    public void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        ScientistLogger.Log($"timeStacker = {timeStacker}");
        ApplyPalette(sLeaser, rCam, rCam.currentPalette);
        sLeaser.sprites[0].x = player.mainBodyChunk.pos.x - camPos.x + this.offsetV2.x * offsetR;
        sLeaser.sprites[0].y = player.mainBodyChunk.pos.y - camPos.y + this.offsetV2.y * offsetR;
        sLeaser.sprites[0].scale = 1.5f;
        sLeaser.sprites[0].color = ScientistTools.ColorChangeAlpha(sLeaser.sprites[0].color, counter / 37.5f);
        sLeaser.sprites[1].x = player.mainBodyChunk.pos.x - camPos.x + this.offsetV2.x * offsetR;
        sLeaser.sprites[1].y = player.mainBodyChunk.pos.y - camPos.y + this.offsetV2.y * offsetR;
        if (base.slatedForDeletetion || this.room != rCam.room)
        {
            sLeaser.CleanSpritesAndRemove();
        }
    }

    public void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[2];
        sLeaser.sprites[0] = new FSprite("Circle20", true);
        sLeaser.sprites[1] = new FSprite(this.spriteName, true);
        this.AddToContainer(sLeaser, rCam, null);
    }
}
