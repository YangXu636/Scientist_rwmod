using Scientist;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scientist.Items.AbstractPhysicalObjects
{
	// Token: 0x02000005 RID: 5
	public class KnotAbstract : AbstractPhysicalObject
    {
        public Vector2 position;
        public List<StringShort> ss;
        public List<AbstractPhysicalObject> connectedObjects;
        public List<Creature> connectedCreatures;

        public KnotAbstract(World world, PhysicalObject realizedObject, WorldCoordinate pos, EntityID ID) : base(world, Scientist.Enums.Items.Knot, realizedObject, pos, ID)
        {
            this.ss = new List<StringShort>(1);
        }

        public override void ChangeRooms(WorldCoordinate newCoord)
        {
            base.ChangeRooms(newCoord);
            if (!ss.Empty())
            {
                for (int i = 0; i < ss.Count; i++)
                {
                    ss[i].ChangeRooms(newCoord, this.ID);
                }
            }
        }

        public override void Realize()
		{
            this.realizedObject ??= new Items.Knot(this, this.world);
        }

        public override string ToString()
        {
            string text = $"{this.ID}<oA>{this.type}<oA>{this.pos.SaveToString()}";/*<oB>{String.Join("<oC>", this.connectedObjects.SelectMany(x => x.ToString().Replace("<oA>", "<oD>")))}<oB>{String.Join("<oC>", this.connectedCreatures.SelectMany(x => x.ToString().Replace("<oA>", "<oD>")))}";*/
            return SaveUtils.AppendUnrecognizedStringAttrs(text, "<oA>", this.unrecognizedAttributes);
        }
    }
}
