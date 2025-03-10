using Scientist;
using System;
using System.Globalization;
using UnityEngine;

namespace Scientist.Items.AbstractPhysicalObjects
{
	// Token: 0x02000005 RID: 5
	internal sealed class SmallRockAbstract : AbstractPhysicalObject
    {

        public SmallRockAbstract(World world, AbstractPhysicalObject.AbstractObjectType type, PhysicalObject realizedObject, WorldCoordinate pos, EntityID ID) : base(world, type, realizedObject, pos, ID)
        {
            this.type = Scientist.Enums.Items.SmallRock;
        }

        public override void Realize()
		{
            this.realizedObject ??= new Items.SmallRock(this, this.world);
        }
	}
}
