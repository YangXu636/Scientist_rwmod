using System;
using System.Globalization;
using UnityEngine;

namespace Scientist.Items.AbstractPhysicalObjects
{
	// Token: 0x02000005 RID: 5
	internal sealed class StoneKnifeAbstract : AbstractPhysicalObject
    {
        public bool sharpen = true;

        public StoneKnifeAbstract(World world, PhysicalObject realizedObject, WorldCoordinate pos, EntityID ID) : base(world, Scientist.Enums.Items.StoneKnife, realizedObject, pos, ID)
        {

        }

        public StoneKnifeAbstract(World world, PhysicalObject realizedObject, WorldCoordinate pos, EntityID ID, bool sharpen) : base(world, Scientist.Enums.Items.StoneKnife, realizedObject, pos, ID)
        {
            this.sharpen = sharpen;
        }

        public override void Realize()
		{
            this.realizedObject ??= new Scientist.Items.StoneKnife(this, this.world);
		}
	}
}
