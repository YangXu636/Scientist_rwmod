using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace items.AbstractPhysicalObjects
{
	// Token: 0x02000005 RID: 5
	public class KnotAbstract : AbstractPhysicalObject
    {
        public Vector2 position;
        public List<StringShort> ss;
        public AbstractPhysicalObject[] connectedObjects;
        public Creature[] connectedCreatures;

        public KnotAbstract(World world, PhysicalObject realizedObject, WorldCoordinate pos, EntityID ID) : base(world, Scientist.ScientistEnums.Items.Knot, realizedObject, pos, ID)
        {
            this.ss = new List<StringShort>(1);
        }

        public override void ChangeRooms(WorldCoordinate newCoord)
        {
            base.ChangeRooms(newCoord);
            if (ss != null & ss.Count > 0)
            {
                foreach (StringShort s in ss)
                {
                    s.ChangeRooms(newCoord, this.ID);
                }
            }
        }

        public override void Realize()
		{
            this.realizedObject ??= new items.Knot(this, this.world);
        }

        public override string ToString()
        {
            string text = $"{this.ID}<oA>{this.type}"/*<oA>{this.pos.SaveToString()}"*/;/*<oB>{String.Join("<oC>", this.connectedObjects.SelectMany(x => x.ToString().Replace("<oA>", "<oD>")))}<oB>{String.Join("<oC>", this.connectedCreatures.SelectMany(x => x.ToString().Replace("<oA>", "<oD>")))}";*/
            return SaveUtils.AppendUnrecognizedStringAttrs(text, "<oA>", this.unrecognizedAttributes);
        }
    }
}
