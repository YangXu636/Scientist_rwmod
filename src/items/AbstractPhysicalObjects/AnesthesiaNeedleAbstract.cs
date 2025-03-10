using System;
using System.Globalization;
using UnityEngine;

namespace Scientist.Items.AbstractPhysicalObjects
{
	internal sealed class AnesthesiaNeedleAbstract : AbstractSpear
	{

		public AnesthesiaNeedleAbstract(World world, Spear realizedObject, WorldCoordinate pos, EntityID ID) : base(world, realizedObject, pos, ID, false)
		{
            this.type = Scientist.Enums.Items.AnesthesiaNeedle;
        }

		public override void Realize()
		{
            this.realizedObject ??= new Items.AnesthesiaNeedle(this, this.world);
		}
	}
}
