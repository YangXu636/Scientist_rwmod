﻿using System;
using System.Globalization;
using UnityEngine;

namespace Scientist.Items.AbstractPhysicalObjects
{
	internal sealed class AnesthesiaSpearAbstract : AbstractSpear
	{

		public AnesthesiaSpearAbstract(World world, Spear realizedObject, WorldCoordinate pos, EntityID ID) : base(world, realizedObject, pos, ID, false)
		{
            this.type = Scientist.Enums.Items.AnesthesiaSpear;
        }

		public override void Realize()
		{
            this.realizedObject ??= new Items.AnesthesiaSpear(this, this.world);
		}
	}
}
