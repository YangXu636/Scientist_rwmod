using items.AbstractPhysicalObjects;
using MoreSlugcats;
using RWCustom;
using Scientist;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace items;

class StringShort : UpdatableAndDeletable, IClimbableVine, IDrawable
{
    public StringShortGraphic graphic;
    public Color baseColor;
    public float conRad;
    public float pushApart;
    public Vector2[,] segments;
    public KnotAbstract stuckPosA;
    public KnotAbstract stuckPosB;
    public Rope[] ropes;
    public List<Vector2> possList;

    public StringShort(Room room, int firstSprite, float length, KnotAbstract spawnPosA, KnotAbstract spawnPosB)
    {
        this.room = room;
        this.stuckPosA = spawnPosA;
        this.stuckPosB = spawnPosB;
        this.segments = new Vector2[(int)Mathf.Clamp(length / this.conRad, 1f, 200f), 3];
        for (int i = 0; i < this.segments.GetLength(0); i++)
        {
            float num = (float)i / (float)(this.segments.GetLength(0) - 1);
            this.segments[i, 0] = Vector2.Lerp(spawnPosA.position, spawnPosB.position, num) + Custom.RNV() * UnityEngine.Random.value;
            this.segments[i, 1] = this.segments[i, 0];
            this.segments[i, 2] = Custom.RNV() * UnityEngine.Random.value;
        }
        this.graphic = new StringShortGraphic(this, Custom.IntClamp((int)(length * 0.3f), 1, 200), firstSprite);
        if (room.climbableVines == null)
        {
            room.climbableVines = new ClimbableVinesSystem();
            room.AddObject(room.climbableVines);
        }
        room.climbableVines.vines.Add(this);
        this.ropes = new Rope[this.segments.GetLength(0) - 1];
        for (int i = 0; i < this.ropes.Length; i++)
        {
            this.ropes[i] = new Rope(room, this.segments[i, 0], this.segments[i + 1, 0], 2f);
        }
        this.conRad *= 3f;
        this.pushApart /= 3f;
        this.possList = new List<Vector2>();
        for (int j = 0; j < this.segments.GetLength(0); j++)
        {
            this.possList.Add(this.segments[j, 0]);
        }
    }

    public override void Update(bool eu)
    {
        base.Update(eu);
        for (int i = 2; i < this.segments.GetLength(0); i++)
        {
            Vector2 vector = Custom.DirVec(this.segments[i - 2, 0], this.segments[i, 0]);
            this.segments[i - 2, 2] -= vector * this.pushApart;
            this.segments[i, 2] += vector * this.pushApart;
        }
        for (int j = 0; j < this.segments.GetLength(0); j++)
        {
            float num = (float)j / (float)(this.segments.GetLength(0) - 1);
            this.segments[j, 2].y -= 0.9f * this.room.gravity * this.GravityAffected(j);
            this.segments[j, 1] = this.segments[j, 0];
            this.segments[j, 0] += this.segments[j, 2];
            this.segments[j, 2] *= 0.999f;
            if (this.room.gravity < 1f && this.room.readyForAI && this.room.aimap.getTerrainProximity(this.segments[j, 0]) < 4)
            {
                IntVector2 tilePosition = this.room.GetTilePosition(this.segments[j, 0]);
                Vector2 vector2 = Vector2.zero;
                for (int k = 0; k < 4; k++)
                {
                    if (!this.room.GetTile(tilePosition + Custom.fourDirections[k]).Solid && !this.room.aimap.getAItile(tilePosition + Custom.fourDirections[k]).narrowSpace)
                    {
                        float num2 = 0f;
                        for (int l = 0; l < 4; l++)
                        {
                            num2 += (float)this.room.aimap.getTerrainProximity(tilePosition + Custom.fourDirections[k] + Custom.fourDirections[l]);
                        }
                        vector2 += Custom.fourDirections[k].ToVector2() * num2;
                    }
                }
                this.segments[j, 2] += vector2.normalized * ((!this.room.GetTile(this.segments[j, 0]).Solid) ? Custom.LerpMap((float)this.room.aimap.getTerrainProximity(this.segments[j, 0]), 0f, 3f, 2f, 0.2f) : 1f) * (1f - this.room.gravity);
            }
            if (j > 2 && this.room.aimap.getTerrainProximity(this.segments[j, 0]) < 3)
            {
                SharedPhysics.TerrainCollisionData terrainCollisionData = new SharedPhysics.TerrainCollisionData(this.segments[j, 0], this.segments[j, 1], this.segments[j, 2], 2f, new IntVector2(0, 0), true);
                terrainCollisionData = SharedPhysics.VerticalCollision(this.room, terrainCollisionData);
                terrainCollisionData = SharedPhysics.HorizontalCollision(this.room, terrainCollisionData);
                this.segments[j, 0] = terrainCollisionData.pos;
                this.segments[j, 2] = terrainCollisionData.vel;
                if (terrainCollisionData.contactPoint.x != 0)
                {
                    this.segments[j, 2].y *= 0.6f;
                }
                if (terrainCollisionData.contactPoint.y != 0)
                {
                    this.segments[j, 2].x *= 0.6f;
                }
            }
        }
        this.ConnectToKnots();
        for (int m = this.segments.GetLength(0) - 1; m > 0; m--)
        {
            this.Connect(m, m - 1);
        }
        this.ConnectToKnots();
        for (int n = 1; n < this.segments.GetLength(0); n++)
        {
            this.Connect(n, n - 1);
        }
        this.ConnectToKnots();
        this.graphic.Update();/*
        for (int i = 0; i < this.segments.GetLength(0); i++)
        {
            this.possList[i] = this.segments[i, 0];
            //this.segments[i, 2] += this.aMountDir * Mathf.InverseLerp(3f, 1f, (float)i) * Mathf.Lerp(0.5f, 0.1f, this.room.gravity);
            //this.segments[i, 2] += this.bMountDir * Mathf.InverseLerp((float)(this.segments.GetLength(0) - 4), (float)(this.segments.GetLength(0) - 2), (float)i) * Mathf.Lerp(0.5f, 0.1f, this.room.gravity);
            this.segments[i, 2] += Custom.RNV() * 0.15f * UnityEngine.Random.value * Mathf.Sin((float)i / (float)(this.segments.GetLength(0) - 1) * 3.1415927f) * (1f - this.room.gravity);
        }
        for (int j = 0; j < this.ropes.Length; j++)
        {
            if (this.ropes[j].bends.Count > 3)
            {
                this.ropes[j].Reset();
            }
            this.ropes[j].Update(this.segments[j, 0], this.segments[j + 1, 0]);
            if (this.ropes[j].totalLength > this.conRad)
            {
                Vector2 vector = Custom.DirVec(this.segments[j, 0], this.ropes[j].AConnect);
                this.segments[j, 0] += vector * (this.ropes[j].totalLength - this.conRad) * 0.5f;
                this.segments[j, 2] += vector * (this.ropes[j].totalLength - this.conRad) * 0.5f;
                vector = Custom.DirVec(this.segments[j + 1, 0], this.ropes[j].BConnect);
                this.segments[j + 1, 0] += vector * (this.ropes[j].totalLength - this.conRad) * 0.5f;
                this.segments[j + 1, 2] += vector * (this.ropes[j].totalLength - this.conRad) * 0.5f;
            }
        }
        if (ModManager.MMF)
        {
            this.graphic.Update();
        }*/
    }

    public void ConnectToKnots()
    {
        if (this.stuckPosA != null)
        {
            this.segments[0, 0] = this.stuckPosA.position;
            if (this.stuckPosA.realizedObject != null)
            {
                this.stuckPosA.realizedObject.bodyChunks[0].vel += this.segments[0, 2] * 0.1f;
                this.segments[0, 2] = this.stuckPosA.realizedObject.bodyChunks[0].vel;
            }
        }
        if (this.stuckPosB != null)
        {
            this.segments[this.segments.GetLength(0) - 1, 0] = this.stuckPosB.position;
            if (this.stuckPosB.realizedObject != null)
            {
                this.stuckPosB.realizedObject.bodyChunks[0].vel += this.segments[this.segments.GetLength(0) - 1, 2] * 0.1f;
                this.segments[this.segments.GetLength(0) - 1, 2] = this.stuckPosB.realizedObject.bodyChunks[0].vel;
            }
        }
    }

    // Token: 0x06003908 RID: 14600 RVA: 0x00402764 File Offset: 0x00400964
    public void Connect(int A, int B)
    {
        Vector2 normalized = (this.segments[A, 0] - this.segments[B, 0]).normalized;
        float num = Vector2.Distance(this.segments[A, 0], this.segments[B, 0]);
        float num2 = Mathf.InverseLerp(0f, this.conRad, num);
        this.segments[A, 0] += normalized * (this.conRad - num) * 0.5f * num2;
        this.segments[A, 2] += normalized * (this.conRad - num) * 0.5f * num2;
        this.segments[B, 0] -= normalized * (this.conRad - num) * 0.5f * num2;
        this.segments[B, 2] -= normalized * (this.conRad - num) * 0.5f * num2;
    }

    public float GravityAffected(int seg)
    {
        return Mathf.Min(Mathf.InverseLerp(2f, 5f, (float)seg), Mathf.InverseLerp((float)(this.segments.GetLength(0) - 3), (float)(this.segments.GetLength(0) - 6), (float)seg));
    }

    public Vector2 OnTubePos(float ps)
    {
        int num = Custom.IntClamp(Mathf.FloorToInt(ps * (float)(this.segments.GetLength(0) - 1)), 0, this.segments.GetLength(0) - 1);
        int num2 = Custom.IntClamp(num + 1, 0, this.segments.GetLength(0) - 1);
        float f = Mathf.InverseLerp((float)num, (float)num2, ps * (float)(this.segments.GetLength(0) - 1));
        Vector2 cA = this.segments[num, 0] - (this.segments[Custom.IntClamp(num - 1, 0, this.segments.GetLength(0) - 1), 0] - this.segments[num, 0]).normalized * Vector2.Distance(this.segments[num, 0], this.segments[num2, 0]) * 0.25f;
        Vector2 cB = this.segments[num2, 0] - (this.segments[Custom.IntClamp(num2 + 1, 0, this.segments.GetLength(0) - 1), 0] - this.segments[num2, 0]).normalized * Vector2.Distance(this.segments[num, 0], this.segments[num2, 0]) * 0.25f;
        return Custom.Bezier(this.segments[num, 0], cA, this.segments[num2, 0], cB, f);
    }

    public void BeingClimbedOn(Creature crit)
    {
    }

    public bool CurrentlyClimbable() => true;

    public Vector2 Pos(int index) => this.segments[index, 0];

    public int TotalPositions() => this.segments.GetLength(0);

    public float Rad(int index) => 2f;

    public float Mass(int index) => 0.25f;

    public void Push(int index, Vector2 movement)
    {
        this.segments[index, 0] += movement;
        this.segments[index, 2] += movement;
    }

    public void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        this.graphic.InitiateSprites(sLeaser, rCam);
    }

    public void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        this.graphic.DrawSprite(sLeaser, rCam, timeStacker, camPos);
    }

    public void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        
    }

    public void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
    {
        
    }

    public class StringShortGraphic : RopeGraphic
    {
        public struct Bump
        {
            public Vector2 pos;
            public float size;
            public float eyeSize;

            public Bump(Vector2 pos, float size, float eyeSize)
            {
                this.pos = pos;
                this.size = size;
                this.eyeSize = eyeSize;
            }
        }

        public StringShort owner;
        public int firstSprite;
        public int totalSprites;
        public int sprites;
        public Rect segmentBounds;
        public bool lastVisible;
        public StringShortGraphic.Bump[] bumps;

        public StringShortGraphic(StringShort owner, int parts, int firstSprite) : base(parts)
        {
            this.owner = owner;
            this.firstSprite = firstSprite;
            this.lastVisible = true;
            this.segmentBounds = default(Rect);
            this.sprites = 1;
            if (ModManager.MMF && owner.room.abstractRoom.singleRealizedRoom)
            {
                this.bumps = new StringShortGraphic.Bump[Math.Max(0, parts / 2 + UnityEngine.Random.Range(-1, 6))];
            }
            else
            {
                this.bumps = new StringShortGraphic.Bump[Math.Max(0, parts + UnityEngine.Random.Range(-5, 6))];
            }
            for (int i = 0; i < this.bumps.Length; i++)
            {
                this.bumps[i] = new StringShortGraphic.Bump(new Vector2(Mathf.Lerp(-1f, 1f, UnityEngine.Random.value) * 2f, UnityEngine.Random.value), Mathf.Pow(UnityEngine.Random.value, 1.4f), (UnityEngine.Random.value < 0.2f) ? Mathf.Lerp(0.2f, 0.8f, Mathf.Pow(UnityEngine.Random.value, 0.5f)) : 0f);
                this.sprites++;
                if (this.bumps[i].eyeSize > 0f)
                {
                    this.sprites++;
                }
            }
        }

        public override void Update()
        {
            for (int i = 0; i < this.segments.Length; i++)
            {
                this.segments[i].lastPos = this.segments[i].pos;
                this.segments[i].pos = (this.owner as StringShort).OnTubePos((float)i / (float)(this.segments.Length - 1));
                this.UpdateSegmentBounds(i == 0, this.segments[i].pos);
            }
        }

        public void UpdateSegmentBounds(bool init, Vector2 pos)
        {
            if (init)
            {
                this.segmentBounds.xMin = pos.x;
                this.segmentBounds.xMax = pos.x;
                this.segmentBounds.yMin = pos.y;
                this.segmentBounds.yMax = pos.y;
                return;
            }
            this.segmentBounds.xMax = Mathf.Max(this.segmentBounds.xMax, pos.x);
            this.segmentBounds.yMax = Mathf.Max(this.segmentBounds.yMax, pos.y);
            this.segmentBounds.xMin = Mathf.Min(this.segmentBounds.xMin, pos.x);
            this.segmentBounds.yMin = Mathf.Min(this.segmentBounds.yMin, pos.y);
        }

        public override void ConnectPhase(float totalRopeLength)
        {
        }

        public override void MoveSegment(int segment, Vector2 goalPos, Vector2 smoothedGoalPos)
        {
            this.segments[segment].vel *= 0f;
            if (this.owner.room.GetTile(smoothedGoalPos).Solid && !this.owner.room.GetTile(goalPos).Solid)
            {
                FloatRect floatRect = Custom.RectCollision(smoothedGoalPos, goalPos, this.owner.room.TileRect(this.owner.room.GetTilePosition(smoothedGoalPos)).Grow(3f));
                this.segments[segment].pos = new Vector2(floatRect.left, floatRect.bottom);
            }
            else
            {
                this.segments[segment].pos = smoothedGoalPos;
            }
            this.UpdateSegmentBounds(false, this.segments[segment].pos);
        }

        public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            sLeaser.sprites[this.firstSprite] = TriangleMesh.MakeLongMeshAtlased(this.segments.Length, false, true);
            int num = 0;
            for (int i = 0; i < this.bumps.Length; i++)
            {
                sLeaser.sprites[this.firstSprite + 1 + i] = new FSprite("Circle4", false);
                sLeaser.sprites[this.firstSprite + 1 + i].scale = Mathf.Lerp(2f, 6f, this.bumps[i].size) / 5f;
                if (this.bumps[i].eyeSize > 0f)
                {
                    sLeaser.sprites[this.firstSprite + 1 + this.bumps.Length + num] = new FSprite("Circle20", false);
                    sLeaser.sprites[this.firstSprite + 1 + this.bumps.Length + num].scale = Mathf.Lerp(2f, 6f, this.bumps[i].size) * this.bumps[i].eyeSize / 10f;
                    num++;
                }
            }
            this.totalSprites = 1 + this.bumps.Length + num;
            this.ApplyPalette(sLeaser, rCam, rCam.currentPalette);
        }

        public override void DrawSprite(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            float num = 2f;
            bool flag = rCam.room.ViewedByAnyCamera(this.segmentBounds, num);
            if (flag != this.lastVisible)
            {
                for (int i = this.firstSprite; i < this.firstSprite + this.totalSprites; i++)
                {
                    sLeaser.sprites[i].isVisible = flag;
                }
                this.lastVisible = flag;
            }
            if (!flag)
            {
                return;
            }
            Vector2 vector = Vector2.Lerp(this.segments[0].lastPos, this.segments[0].pos, timeStacker);
            vector += Custom.DirVec(Vector2.Lerp(this.segments[1].lastPos, this.segments[1].pos, timeStacker), vector) * 1f;
            for (int j = 0; j < this.segments.Length; j++)
            {
                Vector2 vector2 = Vector2.Lerp(this.segments[j].lastPos, this.segments[j].pos, timeStacker);
                Vector2 vector3 = Custom.PerpendicularVector((vector - vector2).normalized);
                (sLeaser.sprites[this.firstSprite] as TriangleMesh).MoveVertice(j * 4, vector - vector3 * num - camPos);
                (sLeaser.sprites[this.firstSprite] as TriangleMesh).MoveVertice(j * 4 + 1, vector + vector3 * num - camPos);
                (sLeaser.sprites[this.firstSprite] as TriangleMesh).MoveVertice(j * 4 + 2, vector2 - vector3 * num - camPos);
                (sLeaser.sprites[this.firstSprite] as TriangleMesh).MoveVertice(j * 4 + 3, vector2 + vector3 * num - camPos);
                vector = vector2;
            }
            int num2 = 0;
            for (int k = 0; k < this.bumps.Length; k++)
            {
                Vector2 vector4 = this.OnTubePos(this.bumps[k].pos, timeStacker);
                sLeaser.sprites[this.firstSprite + 1 + k].x = vector4.x - camPos.x;
                sLeaser.sprites[this.firstSprite + 1 + k].y = vector4.y - camPos.y;
                if (this.bumps[k].eyeSize > 0f)
                {
                    sLeaser.sprites[this.firstSprite + 1 + this.bumps.Length + num2].x = vector4.x - camPos.x;
                    sLeaser.sprites[this.firstSprite + 1 + this.bumps.Length + num2].y = vector4.y - camPos.y;
                    num2++;
                }
            }
        }

        public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            Color effectColor = ScientistTools.ColorFromHex("FFFFFF");
            for (int i = 0; i < (sLeaser.sprites[this.firstSprite] as TriangleMesh).vertices.Length; i++)
            {
                float floatPos = (float)i / (float)((sLeaser.sprites[this.firstSprite] as TriangleMesh).vertices.Length - 1);
                (sLeaser.sprites[this.firstSprite] as TriangleMesh).verticeColors[i] = Color.Lerp(palette.blackColor, effectColor, this.OnTubeEffectColorFac(floatPos));
            }
            int num = 0;
            for (int j = 0; j < this.bumps.Length; j++)
            {
                sLeaser.sprites[this.firstSprite + 1 + j].color = Color.Lerp(palette.blackColor, effectColor, this.OnTubeEffectColorFac(this.bumps[j].pos.y));
                if (this.bumps[j].eyeSize > 0f)
                {
                    sLeaser.sprites[this.firstSprite + 1 + this.bumps.Length + num].color = effectColor;
                    num++;
                }
            }
        }

        public float OnTubeEffectColorFac(float floatPos)
        {
            return Mathf.Sin(floatPos * 3.1415927f) * 0.4f;
        }

        public Vector2 OnTubePos(Vector2 pos, float timeStacker)
        {
            Vector2 p = this.OneDimensionalTubePos(pos.y - 1f / (float)(this.segments.Length - 1), timeStacker);
            Vector2 p2 = this.OneDimensionalTubePos(pos.y + 1f / (float)(this.segments.Length - 1), timeStacker);
            return this.OneDimensionalTubePos(pos.y, timeStacker) + Custom.PerpendicularVector(Custom.DirVec(p, p2)) * pos.x;
        }

        public Vector2 OneDimensionalTubePos(float floatPos, float timeStacker)
        {
            int num = Custom.IntClamp(Mathf.FloorToInt(floatPos * (float)(this.segments.Length - 1)), 0, this.segments.Length - 1);
            int num2 = Custom.IntClamp(num + 1, 0, this.segments.Length - 1);
            float num3 = Mathf.InverseLerp((float)num, (float)num2, floatPos * (float)(this.segments.Length - 1));
            return Vector2.Lerp(Vector2.Lerp(this.segments[num].lastPos, this.segments[num2].lastPos, num3), Vector2.Lerp(this.segments[num].pos, this.segments[num2].pos, num3), timeStacker);
        }
    }
}