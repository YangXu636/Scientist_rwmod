using items.AbstractPhysicalObjects;
using MoreSlugcats;
using RWCustom;
using Scientist;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace items;

public class StringShort : UpdatableAndDeletable, IClimbableVine, IDrawable
{
    //public StringShortGraphic graphic;
    public Color baseColor;
    public float conRad;
    public float pushApart;
    public Vector2[,] segments;
    public Vector2[] parts;
    public KnotAbstract stuckPosA;
    public KnotAbstract stuckPosB;
    public Rope[] ropes;
    public List<Vector2> possList;
    public TriangleMesh tm;
    public int afterChangeRoomTimer = 0;
    public EntityID afterChangeRoomBase;
    public WorldCoordinate afterChangeRoomCoord;
    public bool hasPlayer = true;

    public StringShort(Room room, float length, KnotAbstract spawnPosA, KnotAbstract spawnPosB)
    {
        this.afterChangeRoomTimer = 0;
        this.room = room;
        this.stuckPosA = spawnPosA;
        this.stuckPosB = spawnPosB;
        this.conRad = this.pushApart = 3f;
        this.segments = new Vector2[(int)Mathf.Clamp(length / this.conRad, 1f, 200f), 3];
        for (int i = 0; i < this.segments.GetLength(0); i++)
        {
            float num = (float)i / (float)(this.segments.GetLength(0) - 1);
            this.segments[i, 0] = Vector2.Lerp(spawnPosA.position, spawnPosB.position, num);//+ Custom.RNV() * UnityEngine.Random.value;
            this.segments[i, 1] = this.segments[i, 0];
            this.segments[i, 2] = Vector2.zero;//Custom.RNV() * UnityEngine.Random.value;
        }
        //this.graphic = new StringShortGraphic(this, Custom.IntClamp((int)(length / this.conRad), 1, 200), firstSprite);
        if (this.room.climbableVines == null)
        {
            this.room.climbableVines = new ClimbableVinesSystem();
            this.room.AddObject(this.room.climbableVines);
        }
        this.room.climbableVines.vines.Add(this);
        //this.ropes = new Rope[this.segments.GetLength(0) - 1];
        /*for (int i = 0; i < this.ropes.Length; i++)
        {
            this.ropes[i] = new Rope(room, this.segments[i, 0], this.segments[i + 1, 0], 2f);
        }*/
        this.conRad /= 2f;
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
        //this.graphic.Update();
        if (this.afterChangeRoomTimer > 0 && this.hasPlayer)
        {
            ScientistLogger.Log("StringShort.Update: afterChangeRoomTimer > 0");
            this.afterChangeRoomTimer--;
            if (this.afterChangeRoomBase == this.stuckPosA.ID)
            {
                for (int i = 0; i < this.stuckPosB.realizedObject.bodyChunks.Length; i++)
                {
                    this.stuckPosB.realizedObject.bodyChunks[i].HardSetPosition(this.stuckPosA.realizedObject.bodyChunks[i].pos);
                }
                this.segments.SetAll(this.stuckPosA.position, 0, 1);
                this.segments.SetAll(Vector2.zero, 2);
            }
            else if (this.afterChangeRoomBase == this.stuckPosB.ID)
            {
                for (int i = 0; i < this.stuckPosA.realizedObject.bodyChunks.Length; i++)
                {
                    this.stuckPosA.realizedObject.bodyChunks[i].HardSetPosition(this.stuckPosB.realizedObject.bodyChunks[i].pos);
                }
            }
        }
    }

    public override void Destroy()
    {
        try
        {
            if (this.stuckPosA != null && !this.stuckPosA.ss.HasItemBesides(this))
            {
                this.stuckPosA.Room.realizedRoom.RemoveObject(this.stuckPosA.realizedObject);
                this.room.abstractRoom.RemoveEntity(this.stuckPosA);
            }
            this.stuckPosA = null;
        } catch (Exception) { }
        try
        {
            if (this.stuckPosB != null && !this.stuckPosB.ss.HasItemBesides(this))
            {
                this.stuckPosB.realizedObject.RemoveFromRoom();
                this.room.abstractRoom.RemoveEntity(this.stuckPosB);
            }
            this.stuckPosB = null;
        } catch (Exception) { }
        try
        {
            this.stuckPosA = null;
            this.stuckPosB = null;
            this.room.RemoveObject(this);
        } catch (Exception) { }
        this.segments.SetAll(new Vector2(10000, 10000));
        base.Destroy();
    }

    public void ChangeRooms(WorldCoordinate newCoord, EntityID knotBase)
    {
        if (afterChangeRoomTimer >= 38) { return; }
        Room thisRoom = this.room;
        Room newRoom = this.room.world.GetAbstractRoom(newCoord).realizedRoom;
        thisRoom.RemoveObject(this);
        newRoom.AddObject(this);
        this.afterChangeRoomTimer = 40;
        this.afterChangeRoomBase = knotBase;
        this.afterChangeRoomCoord = newCoord;
        if (knotBase == this.stuckPosA.ID)
        {
            this.stuckPosB.ChangeRooms(newCoord);
            thisRoom.RemoveObject(this.stuckPosB.realizedObject);
            newRoom.AddObject(this.stuckPosB.realizedObject);
        }
        else if (knotBase == this.stuckPosB.ID)
        {
            this.stuckPosA.ChangeRooms(newCoord);
            thisRoom.RemoveObject(this.stuckPosA.realizedObject);
            newRoom.AddObject(this.stuckPosA.realizedObject);
        }
    }

    public void ChangeKnot(KnotAbstract oldKnot, KnotAbstract newKnot)
    {
        if (this.stuckPosA.ID == oldKnot.ID)
        {
            this.stuckPosA.ss.Remove(this);
            this.stuckPosA = newKnot;
            this.stuckPosA.ss.Add(this);
        }
        if (this.stuckPosB.ID == oldKnot.ID)
        {
            this.stuckPosB.ss.Remove(this);
            this.stuckPosB = newKnot;
            this.stuckPosB.ss.Add(this);
        }
    }

    public void ConnectToKnots()
    {
        if (this.stuckPosA != null)
        {
            this.segments[0, 0] = this.stuckPosA.position;
            this.segments[0, 2] *= 0.1f;
            if (this.stuckPosA.realizedObject != null)
            {
                this.stuckPosA.realizedObject.bodyChunks[0].vel += this.segments[0, 2].magnitude < 7 ? this.segments[0, 2] : Vector2.zero;
                //this.segments[0, 2] = this.stuckPosA.realizedObject.bodyChunks[0].vel;
            }
        }
        if (this.stuckPosB != null)
        {
            this.segments[this.segments.GetLength(0) - 1, 0] = this.stuckPosB.position;
            this.segments[this.segments.GetLength(0) - 1, 2] *= 0.1f;
            if (this.stuckPosB.realizedObject != null)
            {
                this.stuckPosB.realizedObject.bodyChunks[0].vel += this.segments[this.segments.GetLength(0) - 1, 2].magnitude < 7 ? this.segments[this.segments.GetLength(0) - 1, 2] : Vector2.zero;
                //this.segments[this.segments.GetLength(0) - 1, 2] = this.stuckPosB.realizedObject.bodyChunks[0].vel;
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

    public bool CurrentlyClimbable()
    {
        var tmpA = this.stuckPosA;
        var tmpB = this.stuckPosB;
        if (tmpA == null || tmpB == null)
        {
            return false;
        }
        var tmpC = this.stuckPosA.realizedObject?.grabbedBy;
        var tmpD = this.stuckPosB.realizedObject?.grabbedBy;
        if ((tmpC == null || tmpC.Count == 0) && (tmpD == null || tmpD.Count == 0))
        {
            return true;
        }
        return false;
    }

    public Vector2 Pos(int index) => this.segments[index, 0];

    public int TotalPositions() => this.segments.GetLength(0);

    public float Rad(int index) => 2f;

    public float Mass(int index) => 0.5f;

    public void Push(int index, Vector2 movement)
    {
        this.segments[index, 0] += movement;
        this.segments[index, 2] += movement;
    }

    public void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[1];
        TriangleMesh.Triangle[] tmt = new TriangleMesh.Triangle[this.segments.GetLength(0) * 2 - 1];
        for (int i = 0; i < this.segments.GetLength(0) * 2 - 1; i++)
        {
            tmt[i] = new TriangleMesh.Triangle(i, i + 1, i + 2);
        }
        if (this.segments.GetLength(0) >= 2)
        {
            tmt[this.segments.GetLength(0) * 2 - 2] = new TriangleMesh.Triangle(this.segments.GetLength(0) * 2 - 3, this.segments.GetLength(0) * 2 - 2, this.segments.GetLength(0) * 2 - 1);
        }
        tm = new TriangleMesh("pixel", tmt, false, true);
        sLeaser.sprites[0] = tm;
        this.AddToContainer(sLeaser, rCam, null);
    }

    public void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        int j = this.segments.GetLength(0);
        for (int i = 0; i < j - 1; i++)
        {
            this.tm.MoveVertice(i * 2, this.segments[i, 0] - camPos + (this.segments[i, 0] - this.segments[i + 1, 0]).VerticalNormalized() * -2f);
            this.tm.MoveVertice(i * 2 + 1, this.segments[i, 0] - camPos + (this.segments[i, 0] - this.segments[i + 1, 0]).VerticalNormalized() * 2f);
        }
        if (j >= 2)
        {
            this.tm.MoveVertice(j * 2 - 2, this.segments[j - 1, 0] - camPos + (this.segments[j - 2, 0] - this.segments[j - 1, 0]).VerticalNormalized() * -2f);
            this.tm.MoveVertice(j * 2 - 1, this.segments[j - 1, 0] - camPos + (this.segments[j - 2, 0] - this.segments[j - 1, 0]).VerticalNormalized() * 2f);
        }
    }

    public void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        sLeaser.sprites[0].color = ScientistTools.ColorFromHex("804000");
    }

    public void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
    {
        newContatiner ??= rCam.ReturnFContainer("Items");
        for (int i = 0; i < sLeaser.sprites.Length; i++)
        {
            sLeaser.sprites[i].RemoveFromContainer();
            newContatiner.AddChild(sLeaser.sprites[i]);
        }
    }
}