using RWCustom;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scientist.Effects;

public class SpiritWithAnimation : UpdatableAndDeletable, IDrawable
{
    public FSprite sprite;

    public SwAData rawData;

    public int counter;
    public int duration;

    public Func<SwAData, int, int, SwAData> animation;
    public FContainer contatiner;

    public SpiritWithAnimation(FSprite sprite, int duration, Func<SwAData, int, int, SwAData> animation, FContainer contatiner)
    {
        this.sprite = sprite;
        this.duration = duration;
        this.counter = 0;
        this.animation = animation;
        this.rawData = new SwAData(sprite.GetPosition(), ScientistTools.AngleToVector2(sprite.rotation), sprite.scaleX, sprite.scaleY, sprite.alpha, sprite.color);
        this.contatiner = contatiner;
    }

    public override void Update(bool eu)
    {
        base.Update(eu);
        if (counter <= duration)
        {
            counter++;
        }
        else
        {
            this.sprite?.RemoveFromContainer();
            this.Destroy();
        }
    }

    public void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
    {
        newContatiner ??= this.contatiner;
        for (int i = 0; i < sLeaser.sprites.Length; i++)
        {
            sLeaser.sprites[i].RemoveFromContainer();
            newContatiner.AddChild(sLeaser.sprites[i]);
        }
    }

    public void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        
    }

    public void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        SwAData data = this.animation.Invoke(rawData, counter, duration);
        sLeaser.sprites[0].SetPosition(data.pos - camPos);
        sLeaser.sprites[0].rotation = Custom.VecToDeg(data.rotation);
        sLeaser.sprites[0].scaleX = data.scaleX;
        sLeaser.sprites[0].scaleY = data.scaleY;
        sLeaser.sprites[0].alpha = data.alpha;
        sLeaser.sprites[0].color = data.color;
        if (base.slatedForDeletetion || this.room != rCam.room)
        {
            sLeaser.CleanSpritesAndRemove();
        }
    }

    public void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[] { sprite };
        this.AddToContainer(sLeaser, rCam, this.contatiner);
    }

    public struct SwAData
    {
        public Vector2 pos;
        public Vector2 rotation;
        public float scaleX;
        public float scaleY;
        public float alpha;
        public Color color;

        public SwAData(Vector2 pos, Vector2 rotation, float scaleX, float scaleY, float alpha, Color color)
        {
            this.pos = pos;
            this.rotation = rotation;
            this.scaleX = scaleX;
            this.scaleY = scaleY;
            this.alpha = alpha;
            this.color = color;
        }
    }
}