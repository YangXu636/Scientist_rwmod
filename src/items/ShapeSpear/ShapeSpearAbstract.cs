using Fisobs.Core;
using UnityEngine;

namespace items.ShapeSpears;

sealed class ShapeSpearAbstract : AbstractSpear
{
    public ShapeSpearAbstract(World world, Spear realizedObject, WorldCoordinate pos, EntityID ID) : base(world, realizedObject, pos, ID, false)
    {
        this.type = ShapeSpearFisob.ShapeSpear;
        this.damageTimes = 1.5f;
    }

    public ShapeSpearAbstract(World world, Spear realizedObject, WorldCoordinate pos, EntityID ID, float damageTimes) : base(world, realizedObject, pos, ID, false)
    {
        this.damageTimes = 1.5f;
    }

    public override void Realize()
    {
        base.Realize();
        realizedObject ??= new ShapeSpear(this, Room.realizedRoom.MiddleOfTile(pos.Tile), Vector2.zero);
    }

    public float damageTimes;

    public override string ToString()
    {
        return this.SaveToString($"{damageTimes}");
    }
}
