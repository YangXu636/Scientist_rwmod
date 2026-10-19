using System.Collections.Generic;
using UnityEngine;

namespace Scientist.Items;

/// <summary>
/// 绳子的地形碰撞（几何层，替代 SharedPhysics 的逐轴扫描）。
///
/// 两条路径：
///  · <see cref="Sweep"/>：Verlet 预测之后，用 prevPos → targetPos 做圆形扫描，提前拦住穿入；
///  · <see cref="ResolvePenetration"/>：万一已经陷进去了，找最近 tile 边、取该边法线推出来。
///
/// 关键约定（防抖）：
///  · 法线一律轴对齐（只取最近的那一条边），不混合两个边的法线，墙角不会来回弹；
///  · 推出时固定加 EPSILON 余量，保证判定稳定处于空气侧；
///  · <see cref="ResolveMany"/> 先收集全部结果，再一次性写回，避免串行修正污染后续判定。
/// </summary>
public static class RopeCollision
{
    /// <summary>节点半径（与渲染半宽一致）。</summary>
    public const float DEFAULT_RADIUS = 2f;

    /// <summary>推出时向外留的余量，避免每帧在"里面/外面"之间反复判定。</summary>
    public const float EPSILON = 0.02f;

    /// <summary>
    /// 法线滞回（px）：与"上一帧法线"方向接近的线段会获得这么多距离折扣。
    /// 平台边缘处顶面/侧面几乎等距，没有滞回时最近线段会在两者间反复切换，
    /// 法线 90° 跳变 → 吸附方向翻转 → 表现为"陷入→跳出→陷入"。
    /// </summary>
    public const float NORMAL_HYSTERESIS = 0.75f;

    /// <summary>
    /// 单次推出的最大位移（px）。深陷时一次推几十像素会把碰撞变成"弹射"：
    /// 位置突变 → 速度回推成巨大速度 → 下个子步飞得更远。超出部分留到后续子步分摊。
    /// </summary>
    public const float MAX_PUSH = 8f;

    private const float TILE_SIZE = 20f;

    /// <summary>最近一次批量解算中"被推出"的节点数与最大穿透深度（诊断用）。</summary>
    public static int lastResolvedCount;
    public static float lastMaxDepth;

    /// <summary>
    /// 圆扫：把 prev → target 的位移按半径采样，任一采样点侵入地形就把结果推出来。
    /// 返回 true 表示被修正，resolved 为修正后的位置，normal 为命中边的外法线，depth 为穿透深度。
    /// </summary>
    public static bool Sweep(Room room, Vector2 prev, Vector2 target, float rad, out Vector2 resolved, out Vector2 normal, out float depth)
    {
        resolved = target;
        normal = Vector2.zero;
        depth = 0f;
        if (room == null)
        {
            return false;
        }
        Vector2 delta = target - prev;
        float dist = delta.magnitude;
        int steps = Mathf.Max(1, Mathf.CeilToInt(dist / Mathf.Max(0.5f, rad * 0.5f)));
        bool hit = false;
        for (int i = 1; i <= steps; i++)
        {
            Vector2 p = Vector2.Lerp(prev, target, (float)i / steps);
            Vector2 n;
            float d;
            if (!Probe(room, p, rad, out n, out d))
            {
                continue;
            }
            if (!hit || d > depth)
            {
                resolved = p + n * (Mathf.Min(d, MAX_PUSH) + EPSILON);
                normal = n;
                depth = d;
                hit = true;
            }
        }
        if (hit)
        {
            return true;
        }
        // 位移很小但目标点已经侵入（例如被约束瞬间拉动）：补一次目标点探测
        Vector2 nTarget;
        float dTarget;
        if (Probe(room, target, rad, out nTarget, out dTarget))
        {
            resolved = target + nTarget * (Mathf.Min(dTarget, MAX_PUSH) + EPSILON);
            normal = nTarget;
            depth = dTarget;
            return true;
        }
        return false;
    }

    /// <summary>单点解算：若已在地形半径内，沿最近边的外法线推出。</summary>
    public static bool ResolvePenetration(Room room, Vector2 pos, float rad, out Vector2 resolved)
    {
        Vector2 n;
        float d;
        if (!Probe(room, pos, rad, out n, out d))
        {
            resolved = pos;
            return false;
        }
        resolved = pos + n * (Mathf.Min(d, MAX_PUSH) + EPSILON);
        return true;
    }

    /// <summary>
    /// 批量解算：先对全部节点收集修正结果，再统一写回。
    /// fixedFlags 为 true 的位置（端点/钉住点）不参与。
    /// </summary>
    public static void ResolveMany(Room room, Vector2[] positions, bool[] fixedFlags, float rad)
    {
        lastResolvedCount = 0;
        lastMaxDepth = 0f;
        if (room == null || positions == null)
        {
            return;
        }
        int count = positions.Length;
        Vector2[] resolved = new Vector2[count];
        bool[] changed = new bool[count];
        for (int i = 0; i < count; i++)
        {
            if (fixedFlags != null && i < fixedFlags.Length && fixedFlags[i])
            {
                continue;
            }
            Vector2 n;
            float d;
            if (!Probe(room, positions[i], rad, out n, out d))
            {
                continue;
            }
            resolved[i] = positions[i] + n * (Mathf.Min(d, MAX_PUSH) + EPSILON);
            changed[i] = true;
            lastResolvedCount++;
            if (d > lastMaxDepth)
            {
                lastMaxDepth = d;
            }
        }
        for (int i = 0; i < count; i++)
        {
            if (changed[i])
            {
                positions[i] = resolved[i];
            }
        }
    }

    /// <summary>地形线段（对应伪代码里的 LineSegment）：normal 指向空气侧，只取该线段自身的法线，不做平均。</summary>
    private struct Segment
    {
        public Vector2 a;
        public Vector2 b;
        public Vector2 normal;
    }

    // 复用缓冲，避免每次探测都分配
    private static readonly List<Segment> scratchSegments = new List<Segment>();

    /// <summary>
    /// 地形拐角：圆心固定在 tile 顶点（坐标由网格精确决定，永不漂移），半径 = CORNER_RADIUS。
    /// 绳子绕过拐角时的实际路径是"沿边 → 沿圆弧 → 沿边"，
    /// 切换点就是圆弧与两条边的切点 = center + dirA * r / center + dirB * r（确定量，不靠最近点搜索）。
    /// </summary>
    public struct Corner
    {
        public Vector2 center;     // tile 顶点（固定）
        public Vector2 dirA;       // 圆弧起点方向（单位向量，从圆心出发）
        public Vector2 dirB;       // 圆弧终点方向
        public float angle;        // 圆弧张角（弧度）
        public float radius;       // 圆角半径（当前 = CORNER_RADIUS）
    }

    /// <summary>拐角圆半径（与节点半径同量级，视觉上就是"圆角"）。</summary>
    public const float CORNER_RADIUS = 3f;

    private static readonly List<Corner> scratchCorners = new List<Corner>();

    /// <summary>圆弧起点（= 与 dirA 那条边的切点）。</summary>
    public static Vector2 CornerTangentA(Corner c)
    {
        return c.center + c.dirA * (c.radius > 0f ? c.radius : CORNER_RADIUS);
    }

    /// <summary>圆弧终点（= 与 dirB 那条边的切点）。</summary>
    public static Vector2 CornerTangentB(Corner c)
    {
        return c.center + c.dirB * (c.radius > 0f ? c.radius : CORNER_RADIUS);
    }

    /// <summary>绕圆弧长（90° 拐角 = r·π/2）。</summary>
    public static float CornerArcLength(Corner c)
    {
        return CORNER_RADIUS * c.angle;
    }

    /// <summary>
    /// 在拐角圆弧上按"弧长 / 段内节点数"均匀分布的位置（只计算，不写入任何节点）。
    /// 角度从 dirA 均分到 dirB，间距 = 弧长 / (nodeCount + 1)——因此不强制 2px，也不会插节点。
    /// </summary>
    public static void CornerNodePositions(Corner c, int nodeCount, List<Vector2> outPositions)
    {
        outPositions.Clear();
        if (nodeCount <= 0)
        {
            return;
        }
        float a0 = Mathf.Atan2(c.dirA.y, c.dirA.x);
        float a1 = Mathf.Atan2(c.dirB.y, c.dirB.x);
        float delta = Mathf.DeltaAngle(a0 * Mathf.Rad2Deg, a1 * Mathf.Rad2Deg) * Mathf.Deg2Rad;
        for (int i = 1; i <= nodeCount; i++)
        {
            float t = (float)i / (nodeCount + 1);
            float ang = a0 + delta * t;
            float r = c.radius > 0f ? c.radius : CORNER_RADIUS;
            outPositions.Add(c.center + new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * r);
        }
    }

    /// <summary>
    /// 收集 pos 周围 3×3 tile 内的拐角：
    /// 顶点周围 4 个 tile 里恰有 1 个实心（凸角）或恰有 3 个实心（凹角）时，该顶点是一个 90° 拐角。
    /// 顶点坐标完全由 tile 网格决定，不依赖绳子的任何状态 → 圆心天然稳定（这是与"最近点搜索"最本质的区别）。
    /// </summary>
    public static void CollectCorners(Room room, Vector2 pos, List<Corner> list)
    {
        list.Clear();
        int cx = Mathf.FloorToInt(pos.x / TILE_SIZE);
        int cy = Mathf.FloorToInt(pos.y / TILE_SIZE);
        for (int vx = cx - 1; vx <= cx + 2; vx++)
        {
            for (int vy = cy - 1; vy <= cy + 2; vy++)
            {
                bool s00 = IsSolidAt(room, vx - 1, vy - 1);   // 左下
                bool s10 = IsSolidAt(room, vx, vy - 1);       // 右下
                bool s01 = IsSolidAt(room, vx - 1, vy);       // 左上
                bool s11 = IsSolidAt(room, vx, vy);           // 右上
                int solid = (s00 ? 1 : 0) + (s10 ? 1 : 0) + (s01 ? 1 : 0) + (s11 ? 1 : 0);
                if (solid != 1 && solid != 3)
                {
                    continue;                  // 直边或对角接缝：不产生绕圆
                }
                Corner c = new Corner();
                c.center = new Vector2(vx * TILE_SIZE, vy * TILE_SIZE);   // 顶点固定
                c.angle = Mathf.PI * 0.5f;
                c.radius = CORNER_RADIUS;
                int ax;
                int ay;
                if (solid == 3)
                {
                    // 唯一空气象限就是圆弧所在象限
                    if (!s00) { ax = -1; ay = -1; }
                    else if (!s10) { ax = 1; ay = -1; }
                    else if (!s01) { ax = -1; ay = 1; }
                    else { ax = 1; ay = 1; }
                }
                else
                {
                    // 唯一实心象限的对角象限
                    if (s00) { ax = 1; ay = 1; }
                    else if (s10) { ax = -1; ay = 1; }
                    else if (s01) { ax = 1; ay = -1; }
                    else { ax = -1; ay = -1; }
                }
                c.dirA = new Vector2(ax, 0f);
                c.dirB = new Vector2(0f, ay);
                list.Add(c);
            }
        }
    }

    /// <summary>tile 是否实心；房间外一律视为实心，防止绳子从边界漏出去。</summary>
    private static bool IsSolidAt(Room room, int x, int y)
    {
        if (x < 0 || y < 0 || x >= room.TileWidth || y >= room.TileHeight)
        {
            return true;
        }
        return room.GetTile(x, y).IsSolid();
    }

    /// <summary>
    /// 收集 pos 周围 3×3 tile 内的地形线段：
    ///  · 实心 tile：只收集与空气相邻的四条边（内部边不产生法线）
    ///  · 斜坡 tile：收集那条对角线
    /// </summary>
    private static void CollectSegments(Room room, Vector2 pos, List<Segment> list)
    {
        list.Clear();
        int cx = Mathf.FloorToInt(pos.x / TILE_SIZE);
        int cy = Mathf.FloorToInt(pos.y / TILE_SIZE);
        for (int x = cx - 1; x <= cx + 1; x++)
        {
            for (int y = cy - 1; y <= cy + 1; y++)
            {
                Room.Tile tile = room.GetTile(x, y);
                if (tile.Terrain == Room.Tile.TerrainType.Slope)
                {
                    AddSlopeSegment(room, x, y, list);
                    continue;
                }
                if (!tile.IsSolid())
                {
                    continue;
                }
                float left = x * TILE_SIZE;
                float right = left + TILE_SIZE;
                float bottom = y * TILE_SIZE;
                float top = bottom + TILE_SIZE;
                if (!IsSolidAt(room, x - 1, y))
                {
                    list.Add(new Segment { a = new Vector2(left, bottom), b = new Vector2(left, top), normal = new Vector2(-1f, 0f) });
                }
                if (!IsSolidAt(room, x + 1, y))
                {
                    list.Add(new Segment { a = new Vector2(right, bottom), b = new Vector2(right, top), normal = new Vector2(1f, 0f) });
                }
                if (!IsSolidAt(room, x, y - 1))
                {
                    list.Add(new Segment { a = new Vector2(left, bottom), b = new Vector2(right, bottom), normal = new Vector2(0f, -1f) });
                }
                if (!IsSolidAt(room, x, y + 1))
                {
                    list.Add(new Segment { a = new Vector2(left, top), b = new Vector2(right, top), normal = new Vector2(0f, 1f) });
                }
            }
        }
    }

    /// <summary>斜坡 tile 的对角线（法线指向空气侧，与 ProbeSlope 保持一致）。</summary>
    private static void AddSlopeSegment(Room room, int tileX, int tileY, List<Segment> list)
    {
        Room.SlopeDirection dir = room.IdentifySlope(tileX, tileY);
        if (dir == Room.SlopeDirection.Broken)
        {
            return;
        }
        float left = tileX * TILE_SIZE;
        float right = left + TILE_SIZE;
        float bottom = tileY * TILE_SIZE;
        float top = bottom + TILE_SIZE;
        bool slash = dir == Room.SlopeDirection.UpLeft || dir == Room.SlopeDirection.DownRight;
        bool airAbove = dir == Room.SlopeDirection.UpLeft || dir == Room.SlopeDirection.UpRight;
        Vector2 gradient = slash ? new Vector2(-1f, 1f) : new Vector2(1f, 1f);
        Vector2 normal = airAbove ? gradient.normalized : (-gradient).normalized;
        Segment seg = new Segment();
        if (slash)
        {
            seg.a = new Vector2(left, bottom);
            seg.b = new Vector2(right, top);
        }
        else
        {
            seg.a = new Vector2(left, top);
            seg.b = new Vector2(right, bottom);
        }
        seg.normal = normal;
        list.Add(seg);
    }

    /// <summary>线段模式开关：false 时退回旧的逐 tile 扫描实现（便于对比与回退）。</summary>
    public static readonly bool USE_SEGMENT_COLLISION = true;

    /// <summary>
    /// 探测一个点是否侵入地形（线段版，对应伪代码的 CircleSweep 兜底 + FindNearestSegment）：
    /// 收集周围的地形线段，取**最近的那一条**，用它自身的法线；不做多法线平均（墙角不会来回震）。
    /// 点在实心内部时深度 = 到最近线段的距离 + rad，保证能推出去。
    /// </summary>
    public static bool Probe(Room room, Vector2 pos, float rad, out Vector2 normal, out float depth)
    {
        return Probe(room, pos, rad, Vector2.zero, out normal, out depth);
    }

    /// <summary>
    /// 带"法线偏好"的探测：与 preferred 方向接近（点积 &gt; 0.7）的线段获得 NORMAL_HYSTERESIS 的距离折扣。
    /// 平台边缘处顶面/侧面几乎等距时，上一子步用的那条边会继续保持，避免法线 90° 反复跳变
    /// （那会让吸附方向每子步翻转，表现为"陷入→跳出→陷入"）。
    /// </summary>
    public static bool Probe(Room room, Vector2 pos, float rad, Vector2 preferred, out Vector2 normal, out float depth)
    {
        if (!USE_SEGMENT_COLLISION)
        {
            return ProbeTiles(room, pos, rad, out normal, out depth);
        }
        normal = Vector2.zero;
        depth = 0f;
        if (room == null)
        {
            return false;
        }
        CollectSegments(room, pos, scratchSegments);
        if (scratchSegments.Count == 0)
        {
            return false;
        }
        bool hasPreferred = preferred.sqrMagnitude > 0.1f;
        float bestScore = float.MaxValue;
        float bestDist = 0f;
        Vector2 bestDelta = Vector2.zero;
        Vector2 bestSegNormal = Vector2.zero;
        for (int i = 0; i < scratchSegments.Count; i++)
        {
            Segment s = scratchSegments[i];
            Vector2 ab = s.b - s.a;
            float len2 = ab.sqrMagnitude;
            float t = len2 < 1E-06f ? 0f : Mathf.Clamp01(Vector2.Dot(pos - s.a, ab) / len2);
            Vector2 closest = s.a + ab * t;
            Vector2 delta = pos - closest;
            float d = delta.magnitude;
            float score = d;
            if (hasPreferred && Vector2.Dot(s.normal, preferred) > 0.7f)
            {
                score -= NORMAL_HYSTERESIS;      // 偏好线段的距离折扣（滞回）
            }
            if (score < bestScore)
            {
                bestScore = score;
                bestDist = d;
                bestDelta = delta;
                bestSegNormal = s.normal;
            }
        }
        bool inside = room.GetTile(pos).IsSolid();
        if (!inside && bestDist >= rad)
        {
            return false;
        }
        // 圆角法线：
        //  · 最近点在直边内部时，delta 沿边法线方向 → 得到轴向法线
        //  · 最近点是端点（拐角）时，delta 指向顶点 → 得到径向法线，即半径 rad 的圆弧
        if (bestDelta.sqrMagnitude > 1E-06f)
        {
            normal = (inside ? -bestDelta : bestDelta).normalized;
        }
        else
        {
            normal = bestSegNormal;          // 正好落在边界上：退回线段自身的法线
        }
        depth = inside ? bestDist + rad : rad - bestDist;
        return true;
    }

    /// <summary>
    /// 旧的逐 tile 实现：命中时给出"最近那条 tile 边"的轴对齐外法线，以及需要推出的深度。
    /// 多格命中时取穿透最深的一格（保证能一次推出去）。
    /// </summary>
    private static bool ProbeTiles(Room room, Vector2 pos, float rad, out Vector2 normal, out float depth)
    {
        normal = Vector2.zero;
        depth = 0f;
        if (room == null)
        {
            return false;
        }
        int minX = Mathf.FloorToInt((pos.x - rad) / TILE_SIZE);
        int maxX = Mathf.FloorToInt((pos.x + rad) / TILE_SIZE);
        int minY = Mathf.FloorToInt((pos.y - rad) / TILE_SIZE);
        int maxY = Mathf.FloorToInt((pos.y + rad) / TILE_SIZE);
        bool found = false;
        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                Room.Tile tile = room.GetTile(x, y);
                // 斜坡：tile 内是一条对角线，不是矩形。按 IdentifySlope 的方向取对角法线。
                if (tile.Terrain == Room.Tile.TerrainType.Slope)
                {
                    Vector2 slopeNormal;
                    float slopeDepth;
                    if (ProbeSlope(room, pos, x, y, rad, out slopeNormal, out slopeDepth) && (!found || slopeDepth > depth))
                    {
                        found = true;
                        depth = slopeDepth;
                        normal = slopeNormal;
                    }
                    continue;
                }
                if (!tile.IsSolid())
                {
                    continue;
                }
                float left = x * TILE_SIZE;
                float right = left + TILE_SIZE;
                float bottom = y * TILE_SIZE;
                float top = bottom + TILE_SIZE;
                bool inside = pos.x > left && pos.x < right && pos.y > bottom && pos.y < top;
                Vector2 closest = new Vector2(Mathf.Clamp(pos.x, left, right), Mathf.Clamp(pos.y, bottom, top));
                float d = Vector2.Distance(pos, closest);
                if (!inside && d >= rad)
                {
                    continue;
                }
                // 只取最近的一条边 → 轴对齐法线（不把两个边的法线混在一起，否则墙角会来回震）
                float toLeft = pos.x - left;
                float toRight = right - pos.x;
                float toBottom = pos.y - bottom;
                float toTop = top - pos.y;
                float nearest = Mathf.Min(Mathf.Min(toLeft, toRight), Mathf.Min(toBottom, toTop));
                Vector2 n;
                if (nearest == toLeft) { n = new Vector2(-1f, 0f); }
                else if (nearest == toRight) { n = new Vector2(1f, 0f); }
                else if (nearest == toBottom) { n = new Vector2(0f, -1f); }
                else { n = new Vector2(0f, 1f); }
                float penetration = inside ? nearest + rad : rad - d;
                if (!found || penetration > depth)
                {
                    found = true;
                    depth = penetration;
                    normal = n;
                }
            }
        }
        return found;
    }

    /// <summary>
    /// 斜坡：tile 内是一条对角线，实心侧由 <see cref="Room.IdentifySlope"/> 决定。
    ///  · UpLeft  /（左下→右上），空气在左上侧
    ///  · UpRight \（左上→右下），空气在右上侧
    ///  · DownLeft \（左上→右下），空气在左下侧
    ///  · DownRight /（左下→右上），空气在右下侧
    /// 法线取对角线的外法线——斜坡不能用轴对齐法线，否则会把球体推进斜面里。
    /// </summary>
    private static bool ProbeSlope(Room room, Vector2 pos, int tileX, int tileY, float rad, out Vector2 normal, out float depth)
    {
        normal = Vector2.zero;
        depth = 0f;
        Room.SlopeDirection dir = room.IdentifySlope(tileX, tileY);
        if (dir == Room.SlopeDirection.Broken)
        {
            return false;                     // 孤立的斜坡格（邻居不匹配），交给上层跳过
        }
        float left = tileX * TILE_SIZE;
        float right = left + TILE_SIZE;
        float bottom = tileY * TILE_SIZE;
        bool slash = dir == Room.SlopeDirection.UpLeft || dir == Room.SlopeDirection.DownRight;
        bool airAbove = dir == Room.SlopeDirection.UpLeft || dir == Room.SlopeDirection.UpRight;
        // 有符号距离（未归一化）：f > 0 表示在"上方"这一侧
        float f = slash
            ? pos.y - ((pos.x - left) + bottom)        // y = x - left + bottom
            : pos.y - ((right - pos.x) + bottom);      // y = right - x + bottom
        Vector2 gradient = slash ? new Vector2(-1f, 1f) : new Vector2(1f, 1f);
        Vector2 airNormal = airAbove ? gradient.normalized : (-gradient).normalized;
        float distance = Mathf.Abs(f) * 0.70710678f;   // |f| / sqrt(2)：到对角线的法向距离
        if (distance >= rad)
        {
            return false;
        }
        normal = airNormal;
        depth = rad - distance;
        return true;
    }
}
