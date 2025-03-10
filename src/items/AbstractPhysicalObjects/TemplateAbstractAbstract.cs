using System;
using System.Globalization;
using UnityEngine;

namespace Scientist.Items.AbstractPhysicalObjects
{
	// Token: 0x02000005 RID: 5
	internal sealed class TemplateAbstract : AbstractConsumable
    {

        public TemplateAbstract(World world, AbstractPhysicalObject.AbstractObjectType type, PhysicalObject realizedObject, WorldCoordinate pos, EntityID ID, int originRoom, int placedObjectIndex, PlacedObject.ConsumableObjectData consumableData) : base(world, type, realizedObject, pos, ID, originRoom, placedObjectIndex, consumableData)
        {

        }

        public override void Realize()
		{
            
		}
	}
}
