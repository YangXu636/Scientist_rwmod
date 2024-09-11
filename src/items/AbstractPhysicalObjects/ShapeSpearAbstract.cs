using System;
using System.Globalization;
using Fisobs.Core;
using UnityEngine;

namespace items.AbstractPhysicalObjects
{
	// Token: 0x02000005 RID: 5
	internal sealed class ShapeSpearAbstract : AbstractSpear
	{

		public ShapeSpearAbstract(World world, Spear realizedObject, WorldCoordinate pos, EntityID ID) : base(world, realizedObject, pos, ID, false)
		{
			this.type = Scientist.Register.ShapeSpear;

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
            this.realizedObject ??= new items.ShapeSpear(this, this.world);
		}
	}
}
