using System;
using System.Globalization;
using UnityEngine;

namespace items.AbstractPhysicalObjects
{
	internal sealed class AnesthesiaNeedleAbstract : AbstractSpear
	{

		public AnesthesiaNeedleAbstract(World world, Spear realizedObject, WorldCoordinate pos, EntityID ID) : base(world, realizedObject, pos, ID, false)
		{
            this.type = Scientist.ScientistEnums.Items.AnesthesiaNeedle;
        }

		public override void Realize()
		{
            this.realizedObject ??= new items.AnesthesiaNeedle(this, this.world);
		}
	}
}
