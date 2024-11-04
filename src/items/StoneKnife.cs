using items.AbstractPhysicalObjects;
using MoreSlugcats;
using RWCustom;
using System;
using System.Data;
using System.Diagnostics;
using UnityEngine;

namespace items;


sealed class StoneKnife : Weapon
{

    public float damage = 0.1f;
    public int dc = 0;              //dc = 0.1s = 4

    public StoneKnife(AbstractPhysicalObject abstractPhysicalObject, World world) : base(abstractPhysicalObject, world)
    {
        base.bodyChunks = new BodyChunk[1];
        base.bodyChunks[0] = new BodyChunk(this, 0, new Vector2(0f, 0f), 5f, 0.07f);
        this.bodyChunkConnections = new PhysicalObject.BodyChunkConnection[0];
        base.airFriction = 0.999f;
        base.gravity = 0.9f;
        this.bounce = 0.4f;
        this.surfaceFriction = 0.4f;
        this.collisionLayer = 2;
        base.waterFriction = 0.98f;
        base.buoyancy = 0.4f;
    }

    public override void Update(bool eu)
    {
        base.Update(eu);
        if (this.dc > 0)
        {
            this.dc--;
        }
    }

    public void Attack(Player player, int direction)
    {
        if (this.dc > 0)
        {
            return;
        }
    }
}
