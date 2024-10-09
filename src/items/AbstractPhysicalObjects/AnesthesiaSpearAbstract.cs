using System;
using System.Globalization;
using UnityEngine;

namespace items.AbstractPhysicalObjects
{
	internal sealed class AnesthesiaSpearAbstract : AbstractSpear
	{

		public AnesthesiaSpearAbstract(World world, Spear realizedObject, WorldCoordinate pos, EntityID ID) : base(world, realizedObject, pos, ID, false)
		{
            this.type = Scientist.Register.AnesthesiaSpear;
        }

		public override void Realize()
		{
            this.realizedObject ??= new items.AnesthesiaSpear(this, this.world);
		}
	}
}
