using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RWCustom;
using UnityEngine;

namespace Scientist.Items;

/// <summary>
/// 绳子路径推断：在房间的"可通行 tile 图"上求两端之间的最短路径（A*），
/// 供 StringShort 做软引导（只约束垂直于路径切线的分量，不改变段长/下垂）。
///
/// 设计要点：
///  · 按 Room 对象缓存整张图（ConditionalWeakTable，房间销毁时自动回收）
///  · 8 邻接、禁止斜穿墙角；离墙越近代价越高，路径会自然走在通道中间
///  · 纯读操作，不改动房间与物理状态
/// </summary>
public static class RopePathSolver
{
    private const float DIAGONAL_COST = 1.41421356f;
    private const float WALL_COST_PER_NEIGHBOR = 0.5f;   // 每有一个实心邻居，这一步的代价增加
    private const int MAX_EXPAND = 60000;                // 单次求解展开上限，防极端情况卡死
    private const int SNAP_RADIUS = 4;                   // 端点落在实心格时，向外寻找可通行格的最大半径
    private const byte OPEN_TILE = 0;

    private class RoomGrid
    {
        public int width;
        public int height;
        public bool[] solid;
        public byte[] wallNeighbors;
        public float[] gScore;
        public int[] cameFrom;
        public bool[] closed;
    }

    private class MinHeap
    {
        private readonly List<int> items = new List<int>();
        private readonly List<float> keys = new List<float>();

        public int Count => this.items.Count;

        public void Clear()
        {
            this.items.Clear();
            this.keys.Clear();
        }

        public void Push(int item, float key)
        {
            this.items.Add(item);
            this.keys.Add(key);
            int i = this.items.Count - 1;
            while (i > 0)
            {
                int parent = (i - 1) / 2;
                if (this.keys[parent] <= this.keys[i])
                {
                    break;
                }
                this.Swap(parent, i);
                i = parent;
            }
        }

        public int Pop()
        {
            int result = this.items[0];
            int last = this.items.Count - 1;
            this.Swap(0, last);
            this.items.RemoveAt(last);
            this.keys.RemoveAt(last);
            int i = 0;
            while (true)
            {
                int left = i * 2 + 1;
                int right = left + 1;
                int smallest = i;
                if (left < this.items.Count && this.keys[left] < this.keys[smallest]) { smallest = left; }
                if (right < this.items.Count && this.keys[right] < this.keys[smallest]) { smallest = right; }
                if (smallest == i) { break; }
                this.Swap(i, smallest);
                i = smallest;
            }
            return result;
        }

        private void Swap(int a, int b)
        {
            int i = this.items[a];
            this.items[a] = this.items[b];
            this.items[b] = i;
            float k = this.keys[a];
            this.keys[a] = this.keys[b];
            this.keys[b] = k;
        }
    }

    private static ConditionalWeakTable<Room, RoomGrid> grids = new ConditionalWeakTable<Room, RoomGrid>();
    private static readonly MinHeap heap = new MinHeap();
    private static float lastSolveMs;

    /// <summary>最近一次求解耗时（毫秒），供调试命令输出。</summary>
    public static float LastSolveMs => lastSolveMs;

    /// <summary>清空所有房间的缓存（房间重载/换关时调用）。</summary>
    public static void ClearCache()
    {
        grids = new ConditionalWeakTable<Room, RoomGrid>();
    }

    /// <summary>
    /// 求 from → to 的路径。成功时 outPoints 是 tile 中心点折线（首尾已替换为 from/to），pathLength 为折线总长。
    /// 失败（不同连通域、房间为空等）返回 false，调用方应退回纯物理。
    /// </summary>
    public static bool TrySolve(Room room, Vector2 from, Vector2 to, List<Vector2> outPoints, out float pathLength)
    {
        pathLength = 0f;
        lastSolveMs = 0f;
        if (outPoints == null)
        {
            return false;
        }
        outPoints.Clear();
        if (room == null)
        {
            return false;
        }
        System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            RoomGrid grid = GetGrid(room);
            int w = grid.width;
            int h = grid.height;
            if (w <= 0 || h <= 0)
            {
                return false;
            }

            IntVector2 start = room.GetTilePosition(from);
            IntVector2 goal = room.GetTilePosition(to);
            if (!SnapToPassable(grid, ref start, w, h) || !SnapToPassable(grid, ref goal, w, h))
            {
                return false;
            }

            int startIndex = Index(start.x, start.y, h);
            int goalIndex = Index(goal.x, goal.y, h);

            if (startIndex == goalIndex)
            {
                outPoints.Add(from);
                outPoints.Add(to);
                pathLength = Vector2.Distance(from, to);
                return true;
            }

            int total = w * h;
            for (int i = 0; i < total; i++)
            {
                grid.gScore[i] = float.MaxValue;
                grid.cameFrom[i] = -1;
                grid.closed[i] = false;
            }
            heap.Clear();
            grid.gScore[startIndex] = 0f;
            heap.Push(startIndex, Heuristic(start.x, start.y, goal.x, goal.y));

            bool found = false;
            int expand = 0;
            while (heap.Count > 0 && expand++ < MAX_EXPAND)
            {
                int current = heap.Pop();
                if (grid.closed[current])
                {
                    continue;
                }
                grid.closed[current] = true;
                if (current == goalIndex)
                {
                    found = true;
                    break;
                }
                int cx = current / h;
                int cy = current % h;
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        if (dx == 0 && dy == 0)
                        {
                            continue;
                        }
                        int nx = cx + dx;
                        int ny = cy + dy;
                        if (!InBounds(nx, ny, w, h) || grid.solid[Index(nx, ny, h)])
                        {
                            continue;
                        }
                        // 禁止斜穿墙角：对角移动时两条正交邻居必须都可通行
                        if (dx != 0 && dy != 0 &&
                            (grid.solid[Index(cx + dx, cy, h)] || grid.solid[Index(cx, cy + dy, h)]))
                        {
                            continue;
                        }
                        int nIndex = Index(nx, ny, h);
                        if (grid.closed[nIndex])
                        {
                            continue;
                        }
                        float step = (dx == 0 || dy == 0) ? 1f : DIAGONAL_COST;
                        step *= 1f + grid.wallNeighbors[nIndex] * WALL_COST_PER_NEIGHBOR;
                        float candidate = grid.gScore[current] + step;
                        if (candidate >= grid.gScore[nIndex])
                        {
                            continue;
                        }
                        grid.gScore[nIndex] = candidate;
                        grid.cameFrom[nIndex] = current;
                        heap.Push(nIndex, candidate + Heuristic(nx, ny, goal.x, goal.y));
                    }
                }
            }

            if (!found)
            {
                return false;
            }

            // 回溯（goal → start），存成 tile 中心点，最后反转
            List<int> reversed = new List<int>(32);
            for (int at = goalIndex; at != -1; at = grid.cameFrom[at])
            {
                reversed.Add(at);
                if (at == startIndex)
                {
                    break;
                }
            }
            reversed.Reverse();
            for (int i = 0; i < reversed.Count; i++)
            {
                int x = reversed[i] / h;
                int y = reversed[i] % h;
                outPoints.Add(room.MiddleOfTile(x, y));
            }
            if (outPoints.Count == 0)
            {
                return false;
            }
            outPoints[0] = from;
            outPoints[outPoints.Count - 1] = to;

            for (int i = 1; i < outPoints.Count; i++)
            {
                pathLength += Vector2.Distance(outPoints[i - 1], outPoints[i]);
            }
            return true;
        }
        finally
        {
            watch.Stop();
            lastSolveMs = (float)watch.Elapsed.TotalMilliseconds;
        }
    }

    private static float Heuristic(int x, int y, int gx, int gy)
    {
        float dx = Mathf.Abs(gx - x);
        float dy = Mathf.Abs(gy - y);
        return (dx > dy) ? (dx - dy + DIAGONAL_COST * dy) : (dy - dx + DIAGONAL_COST * dx);
    }

    private static int Index(int x, int y, int height)
    {
        return x * height + y;
    }

    private static bool InBounds(int x, int y, int w, int h)
    {
        return x >= 0 && y >= 0 && x < w && y < h;
    }

    private static bool SnapToPassable(RoomGrid grid, ref IntVector2 tile, int w, int h)
    {
        if (InBounds(tile.x, tile.y, w, h) && !grid.solid[Index(tile.x, tile.y, h)])
        {
            return true;
        }
        int bestX = -1;
        int bestY = -1;
        int bestDist = int.MaxValue;
        for (int dx = -SNAP_RADIUS; dx <= SNAP_RADIUS; dx++)
        {
            for (int dy = -SNAP_RADIUS; dy <= SNAP_RADIUS; dy++)
            {
                int x = tile.x + dx;
                int y = tile.y + dy;
                if (!InBounds(x, y, w, h) || grid.solid[Index(x, y, h)])
                {
                    continue;
                }
                int d = dx * dx + dy * dy;
                if (d < bestDist)
                {
                    bestDist = d;
                    bestX = x;
                    bestY = y;
                }
            }
        }
        if (bestX < 0)
        {
            return false;
        }
        tile = new IntVector2(bestX, bestY);
        return true;
    }

    private static RoomGrid GetGrid(Room room)
    {
        RoomGrid grid;
        if (grids.TryGetValue(room, out grid))
        {
            return grid;
        }
        int w = room.TileWidth;
        int h = room.TileHeight;
        grid = new RoomGrid
        {
            width = w,
            height = h,
            solid = new bool[Mathf.Max(1, w * h)],
            wallNeighbors = new byte[Mathf.Max(1, w * h)],
            gScore = new float[Mathf.Max(1, w * h)],
            cameFrom = new int[Mathf.Max(1, w * h)],
            closed = new bool[Mathf.Max(1, w * h)]
        };
        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                grid.solid[Index(x, y, h)] = room.GetTile(x, y).IsSolid();
            }
        }
        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                if (grid.solid[Index(x, y, h)])
                {
                    grid.wallNeighbors[Index(x, y, h)] = 0;
                    continue;
                }
                byte count = 0;
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        if (dx == 0 && dy == 0)
                        {
                            continue;
                        }
                        int nx = x + dx;
                        int ny = y + dy;
                        if (!InBounds(nx, ny, w, h) || grid.solid[Index(nx, ny, h)])
                        {
                            count++;
                        }
                    }
                }
                grid.wallNeighbors[Index(x, y, h)] = count;
            }
        }
        grids.Add(room, grid);
        return grid;
    }
}
