using Scientist;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

namespace Scientist.Items.AbstractPhysicalObjects
{
	public class KnotAbstract : AbstractPhysicalObject
    {
        public Vector2 position;
        public List<StringShort> ss;
        public List<AbstractPhysicalObject> connectedObjects;
        public List<Creature> connectedCreatures;

        // ==== 绳身（StringShort）的持久化信息 ====
        // 绳身不是 AbstractPhysicalObject，无法单独进存档，因此由两端绳结各自记录"对端 ID + 绳长"。
        // 读取时由 ID 较小的一端负责把整条绳接回来；两端不在同一房间时，两侧各接出"半截绳"
        // （一端是本绳结，另一端固定在该房间通往对端房间的通道口）。
        public bool hasRopePartner = false;
        public EntityID ropePartnerID = new EntityID(0, -1);
        public float ropeLength = 0f;

        public KnotAbstract(World world, PhysicalObject realizedObject, WorldCoordinate pos, EntityID ID) : base(world, Scientist.Enums.Items.Knot, realizedObject, pos, ID)
        {
            this.ss = new List<StringShort>(1);
        }

        /// <summary>绳结的实际坐标：优先取已实现物体的位置（未实现时退回上次缓存值）。</summary>
        public Vector2 CurrentPosition => this.realizedObject != null ? this.realizedObject.firstChunk.pos : this.position;

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
            string text = $"{this.ID}<oA>{this.type}<oA>{this.pos.SaveToString()}";
            if (this.hasRopePartner)
            {
                text += $"<oA>{this.ropePartnerID}<oA>{this.ropeLength.ToString(CultureInfo.InvariantCulture)}";
            }
            return SaveUtils.AppendUnrecognizedStringAttrs(text, "<oA>", this.unrecognizedAttributes);
        }
    }
}
