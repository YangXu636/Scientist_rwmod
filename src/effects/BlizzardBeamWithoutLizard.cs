using RWCustom;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Watcher;

namespace Scientist.Effects;

public class BlizzardBeamWithoutLizard : UpdatableAndDeletable, IDrawable
{
    public Vector2 pos;
    public Vector2 lastPos;
    public Vector2 hitPos;
    public Vector2 lastHitPos;
    public Vector2 angle;
    public Vector2 lastAngle;
    public bool isVisible;
    public float size;
    public float lastSize;
    public float intensity;
    public int time;
    public PhysicalObject owner;
    public int ownerBodychunkIndex;
    public bool isFromLizard;
    public bool isFromCreature;
    public int beamOwnerID;
    public int thisID;
    public Creature topOwner;

    public int index;
    public int lifetime = 0;
    public bool shouldRemove = false;

    public Vector2? SetPos;
    public Vector2? SetAngle;
    public float? SetIntensity;

    public BlizzardBeamWithoutLizard(PhysicalObject owner, int ownerBodychunkIndex, Vector2 startPos, Vector2 angle, int index, float intensity)
    {
        this.owner = owner;
        this.ownerBodychunkIndex = ownerBodychunkIndex;
        this.pos = startPos;
        this.lastPos = startPos;
        this.angle = angle;
        this.isFromLizard = false;
        this.isFromCreature = false;
        this.isVisible = true;
        this.index = index;
        this.intensity = intensity;
        this.lifetime = 5;
    }

    public BlizzardBeamWithoutLizard(Lizard owner, int ownerBodychunkIndex, Vector2 startPos, Vector2 angle, int index, float intensity = 1f) : this(owner as PhysicalObject, ownerBodychunkIndex, startPos, angle, index, intensity)
    {
        this.isFromLizard = true;
        this.isFromCreature = true;
    }

    public BlizzardBeamWithoutLizard(Creature owner, int ownerBodychunkIndex, Vector2 startPos, Vector2 angle, int index, float intensity = 1f) : this(owner as PhysicalObject, ownerBodychunkIndex, startPos, angle, index, intensity)
    {
        this.isFromLizard = false;
        this.isFromCreature = true;
    }

    public override void Update(bool eu)
    {
        base.Update(eu);
        if (this.shouldRemove) { return; }
        this.lastPos = this.pos;
        this.lastAngle = this.angle;
        this.lifetime--;
        if (this.SetPos != null) { this.pos = this.SetPos.Value; this.SetPos = null; this.lifetime = 2;  }
        if (this.SetAngle != null) { this.angle = this.SetAngle.Value; this.SetAngle = null; this.lifetime = 2; }
        if (this.SetIntensity != null) { this.intensity = this.SetIntensity.Value; this.SetIntensity = null; this.lifetime = 2; }
        if (Vector2.Distance(this.pos, this.owner.bodyChunks[this.ownerBodychunkIndex].pos) > this.owner.bodyChunks[this.ownerBodychunkIndex].rad + 5f || this.lifetime <= 0)
        {
            this.shouldRemove = true;
            return;
        }
        this.lastHitPos = this.hitPos;
        this.lastSize = this.size;
        this.size = Mathf.Abs(Mathf.Sin((float)this.time * 0.44f));
        this.hitPos = this.RayTraceBeamHitPos(out PhysicalObject po);
    }

    public Vector2 RayTraceBeamHitPos(out PhysicalObject po)
    {
        Vector2 result;
        po = null;
        Vector2 a1 = angle.normalized;
        Vector2 corner = Custom.RectCollision(this.pos, this.pos + a1 * 100000f, this.owner.room.RoomRect).GetCorner(FloatRect.CornerLabel.D);
        IntVector2? intVector = SharedPhysics.RayTraceTilesForTerrainReturnFirstSolid(this.owner.room, this.pos, corner);
        if (intVector != null)
        {
            result = this.owner.room.MiddleOfTile(intVector.Value);
        }
        else
        {
            result = a1 * 10000f;
        }
        Vector2 a = angle;
        List<PhysicalObject> poList = this.owner.room.physicalObjects.SelectMany(x => x).Where(x => x is Scientist.Interface.ICanBeamHit).ToList();
        if (poList.Count == 0) { return result; }
        for (int i = 0; i < poList.Count; i++)
        {
            if (poList[i] is Interface.ICanBeamHit icbh) { icbh.HitBeamID.Remove(this.thisID); }
        }
        float kN = a.y / a.x, kV = -a.x / a.y;
        float beamFunc(float x, float k) => k * x + this.pos.y - k * this.pos.x; // k = kN 为射线，k = kV 为以过嘴的射线垂线
        int hitBodyChunkIndex = -1;
        for (int i = 0; i < poList.Count; i++)
        {
            if (poList[i] is Interface.ICanBeamHit icbh && !icbh.CanBlockBeam) { continue; }
            for (int j = 0; j < poList[i].bodyChunks.Length; j++)
            {
                Vector2 bcPos = poList[i].bodyChunks[j].pos;
                if (
                    Custom.Dist(this.pos, bcPos) < Custom.Dist(this.pos, result)
                    && (
                        (Mathf.Abs(a.x) < 1E-05f) && bcPos.y * Mathf.Sign(a.y) > this.pos.y * Mathf.Sign(a.y) && Mathf.Abs(bcPos.x - this.pos.x) < poList[i].bodyChunks[j].rad
                        || (Mathf.Abs(a.y) < 1E-05f && bcPos.x * Mathf.Sign(a.x) > this.pos.x * Mathf.Sign(a.x) && Mathf.Abs(bcPos.y - this.pos.y) < poList[i].bodyChunks[j].rad)
                        || (bcPos.y * Mathf.Sign(a.y) > beamFunc(bcPos.x, kV) * Mathf.Sign(a.y) && Mathf.Abs(beamFunc(bcPos.x, kN) - bcPos.y) / Mathf.Sqrt(1 + kN * kN) <= poList[i].bodyChunks[j].rad)
                    )
                )
                {
                    result = bcPos;
                    po = poList[i];
                    hitBodyChunkIndex = j;
                }
            }
        }
        if (po == null || hitBodyChunkIndex == -1) { return result; }
        Vector2 incidentPointAngle = Vector2.zero;
        if (Mathf.Abs(a.x) < 1E-05f)
        {
            incidentPointAngle = new Vector2(0, -po.bodyChunks[hitBodyChunkIndex].rad) * Mathf.Sign(a.y);
        }
        else if (Mathf.Abs(a.y) < 1E-05f)
        {
            incidentPointAngle = new Vector2(-po.bodyChunks[hitBodyChunkIndex].rad, 0) * Mathf.Sign(a.x);
        }
        else
        {
            incidentPointAngle = new Vector2(-a.y, a.x).normalized * Mathf.Sign(Vector2.Dot(new Vector2(-a.y, a.x), this.pos - result)) * Mathf.Abs(result.y - beamFunc(result.x, kN)) / Mathf.Sqrt(1 + kN * kN)
                - a.normalized * Mathf.Sqrt(Mathf.Pow(po.bodyChunks[hitBodyChunkIndex].rad, 2.00f) - Mathf.Pow(beamFunc(result.x, kN) - result.y, 2.00f) / (1 + kN * kN));
        }
        if (float.IsNaN(incidentPointAngle.x) || float.IsNaN(incidentPointAngle.y) || float.IsInfinity(incidentPointAngle.x) || float.IsInfinity(incidentPointAngle.y)) { incidentPointAngle = Vector2.zero; ScientistLogger.Warning($"incidentPointAngle is NaN or Inf!"); }
        if (po is Interface.ICanBeamHit icbh2)
        {
            icbh2.HitBeamID.AddSafe(this.thisID);
            icbh2.HitByBeam(incidentPointAngle, a, this.intensity);
        }
        if (po is Interface.IReflectionBeam irb && irb.CanReflect)
        {
            if (!Scientist.Data.Miscellaneous.beInRoom.ContainsKey(this.room.abstractRoom.name))
            {
                Scientist.Data.Miscellaneous.beInRoom[this.room.abstractRoom.name] = new Handle.BeamExpansion(this.room);
                this.room.AddObject(Scientist.Data.Miscellaneous.beInRoom[this.room.abstractRoom.name]);
            }
            Vector2[] angles = irb.ReflectionAngles(incidentPointAngle, a, out float[] intensities);
            string ft = /*ScientistTools.FeaturesTypeString(this.owner)*/ this.thisID.ToString();
            if (!Scientist.Data.Miscellaneous.beInRoom[this.room.abstractRoom.name].blizzardBeamWL.ContainsKey(ft))
            {
                Scientist.Data.Miscellaneous.beInRoom[this.room.abstractRoom.name].blizzardBeamWL[ft] = new();
            }
            if (Scientist.Data.Miscellaneous.beInRoom[this.room.abstractRoom.name].blizzardBeamWL[ft].Count < angles.Length)
            {
                for (int k = Scientist.Data.Miscellaneous.beInRoom[this.room.abstractRoom.name].blizzardBeamWL[ft].Count; k < angles.Length; k++)
                {
                    Scientist.Data.Miscellaneous.beInRoom[this.room.abstractRoom.name].blizzardBeamWL[ft].Add(new Effects.BlizzardBeamWithoutLizard(po, hitBodyChunkIndex, result + incidentPointAngle, angles[k], k, intensities[k]) { topOwner = this.topOwner, beamOwnerID = this.thisID, thisID = Scientist.Data.Miscellaneous.GetNewBeamID() });
                    this.room.AddObject(Scientist.Data.Miscellaneous.beInRoom[this.room.abstractRoom.name].blizzardBeamWL[ft][k]);
                }
            }
            List<Effects.BlizzardBeamWithoutLizard> bblwl2 = Scientist.Data.Miscellaneous.beInRoom[this.room.abstractRoom.name].blizzardBeamWL[ft];
            for (int k = 0; k < angles.Length; k++)
            {
                bblwl2[k].SetPos = result + incidentPointAngle;
                bblwl2[k].SetAngle = angles[k];
                bblwl2[k].SetIntensity = intensities[k];
            }
        }
        return result + incidentPointAngle;
    }

    public void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[3];
        sLeaser.sprites[0] = new FSprite("pixel", true);
        sLeaser.sprites[0].shader = rCam.game.rainWorld.Shaders["HologramBehindTerrain"];
        sLeaser.sprites[0].scaleX = 44f;
        sLeaser.sprites[0].anchorY = 0f;
        sLeaser.sprites[0].alpha = 0.5f;
        sLeaser.sprites[1] = new FSprite("pixel", true);
        sLeaser.sprites[1].shader = rCam.game.rainWorld.Shaders["HologramBehindTerrain"];
        sLeaser.sprites[1].scaleX = 18f;
        sLeaser.sprites[1].anchorY = 0f;
        sLeaser.sprites[1].alpha = 0.8f;
        sLeaser.sprites[2] = new FSprite("pixel", true);
        sLeaser.sprites[2].shader = rCam.game.rainWorld.Shaders["HologramBehindTerrain"];
        sLeaser.sprites[2].scaleX = 5f;
        sLeaser.sprites[2].anchorY = 0f;
        sLeaser.sprites[2].alpha = 1f;
        this.AddToContainer(sLeaser, rCam, null);
    }

    public void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        Vector2 vector = Vector2.Lerp(this.lastPos, this.pos, timeStacker);
        Vector2 vector2 = Vector2.Lerp(this.lastHitPos, this.hitPos, timeStacker);
        Vector2 vector3 = Vector2.Lerp(this.lastAngle, this.angle, timeStacker);
        float num = Mathf.Lerp(this.lastSize, this.size, timeStacker);
        for (int i = 0; i < sLeaser.sprites.Length; i++)
        {
            sLeaser.sprites[i].x = vector.x - camPos.x;
            sLeaser.sprites[i].y = vector.y - camPos.y;
            sLeaser.sprites[i].rotation = Custom.VecToDeg(vector3);
            sLeaser.sprites[i].scaleY = Vector2.Distance(vector, vector2);
            sLeaser.sprites[i].isVisible = this.isVisible;
        }
        sLeaser.sprites[0].scaleX = 40f + 20f * num;
        sLeaser.sprites[1].scaleX = 25f - 10f * num;
        sLeaser.sprites[2].scaleX = 5f + 5f * num;
        this.ApplyPalette(sLeaser, rCam, rCam.currentPalette);
        if (base.slatedForDeletetion || this.room != rCam.room)
        {
            sLeaser.CleanSpritesAndRemove();
        }
    }

    public void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        Color color = new Color(0.9f, 0.85f, 1f);
        sLeaser.sprites[2].color = color;
        sLeaser.sprites[1].color = Color.Lerp(color, palette.fogColor, 0.2f);
        sLeaser.sprites[0].color = Color.Lerp(color, palette.fogColor, 0.5f);
    }

    public void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
    {
        newContatiner ??= rCam.ReturnFContainer("Midground");
        for (int i = 0; i < sLeaser.sprites.Length; i++)
        {
            sLeaser.sprites[i].RemoveFromContainer();
            newContatiner.AddChild(sLeaser.sprites[i]);
        }
    }
}