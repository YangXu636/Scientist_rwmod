using System;
using System.Globalization;
using UnityEngine;

namespace Scientist.items.AbstractPhysicalObjects
{
	internal sealed class SharpSpearAbstract : AbstractSpear
	{

		public SharpSpearAbstract(World world, Spear realizedObject, WorldCoordinate pos, EntityID ID) : base(world, realizedObject, pos, ID, false)
		{
			this.type = Scientist.Enums.Items.SharpSpear;

			base.stuckInWallCycles = 0;
			base.explosive = false;
			if (ModManager.MSC)
			{
				base.hue = 0f;
				base.electric = false;
				base.electricCharge = 0;
				base.needle = false;
			}
		}

		public override void Realize()
		{
            this.realizedObject ??= new items.SharpSpear(this, this.world);
		}
	}
}
