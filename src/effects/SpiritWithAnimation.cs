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
        this.rawData = new SwAData(sprite.GetPosition(), ScientistTools.AngleToVector2(sprite.rotation), sprite.scale, sprite.scaleX, sprite.scaleY, sprite.alpha, sprite.color);
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
        SwAData? data = this.animation?.Invoke(rawData, counter, duration);
        if (!data.HasValue) { return; }
        sprite.SetPosition(data.Value.pos - camPos);
        sprite.rotation = Custom.VecToDeg(data.Value.rotation);
        sprite.scale = data.Value.scale;
        sprite.scaleX = data.Value.scaleX;
        sprite.scaleY = data.Value.scaleY;
        sprite.alpha = data.Value.alpha;
        sprite.color = data.Value.color;
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
        public float scale;
        public float scaleX;
        public float scaleY;
        public float alpha;
        public Color color;

        public SwAData(Vector2 pos, Vector2 rotation, float scale, float scaleX, float scaleY, float alpha, Color color)
        {
            this.pos = pos;
            this.rotation = rotation;
            this.scale = scale;
            this.scaleX = scaleX;
            this.scaleY = scaleY;
            this.alpha = alpha;
            this.color = color;
        }
    }
}