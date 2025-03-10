using Scientist;
using System;
using System.Globalization;
using UnityEngine;

namespace Scientist.Items.AbstractPhysicalObjects;

public class BreathingBubbleAbstract : AbstractPhysicalObject
{
    public float oxygenLeft = 60.00000f;

    public BreathingBubbleAbstract(World world, PhysicalObject realizedObject, WorldCoordinate pos, EntityID ID) : base(world, Scientist.Enums.Items.BreathingBubble, realizedObject, pos, ID)
    {

    }

    public BreathingBubbleAbstract(World world, PhysicalObject realizedObject, WorldCoordinate pos, EntityID ID, float oxygenLeft) : this(world, realizedObject, pos, ID)
    {

        this.oxygenLeft = oxygenLeft;
    }

    public override void Realize()
    {
        this.realizedObject ??= new Items.BreathingBubble(this);
    }

    public override string ToString()
    {
        return $"{base.ToString()}<oA>{this.oxygenLeft}";
    }
}
