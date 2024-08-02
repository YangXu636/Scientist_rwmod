using Fisobs.Core;
using UnityEngine;

namespace ShapeSpears;

sealed class ShapeSpearAbstract : AbstractPhysicalObject
{
    public ShapeSpearAbstract(World world, WorldCoordinate pos, EntityID ID) : base(world, ShapeSpearFisob.ShapeSpear, null, pos, ID)
    {
        scaleX = 1;
        scaleY = 1;
        saturation = 0.5f;
        hue = 1f;
    }

    public override void Realize()
    {
        base.Realize();
        realizedObject ??= new ShapeSpear(this, Room.realizedRoom.MiddleOfTile(pos.Tile), Vector2.zero);
    }

    public float hue;
    public float saturation;
    public float scaleX;
    public float scaleY;
    public float damage;

    public override string ToString()
    {
        return this.SaveToString($"{hue};{saturation};{scaleX};{scaleY};{damage}");
    }
}
