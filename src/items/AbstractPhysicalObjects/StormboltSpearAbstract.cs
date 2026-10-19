using System;
using System.Globalization;
using UnityEngine;

namespace Scientist.Items.AbstractPhysicalObjects
{
	internal sealed class StormboltSpearAbstract : AbstractSpear
	{
		public int stormboltCharge = 0;

		public StormboltSpearAbstract(World world, Spear realizedObject, WorldCoordinate pos, EntityID ID) : base(world, realizedObject, pos, ID, false)
		{
			this.type = Scientist.Enums.Items.StormboltSpear;

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

		public StormboltSpearAbstract(World world, Spear realizedObject, WorldCoordinate pos, EntityID ID, int stormboltCharge) : this(world, realizedObject, pos, ID)
		{
			this.stormboltCharge = stormboltCharge;
		}

        public override void Realize()
		{
            this.realizedObject ??= new Items.StormboltSpear(this, this.world);
		}

        public override string ToString()
        {
            return $"{base.ToString()}<oA>{this.stormboltCharge}";
        }
    }
}
