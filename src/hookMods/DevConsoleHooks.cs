using MonoMod.RuntimeDetour;
using Scientist.Items.AbstractPhysicalObjects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using UnityEngine;

namespace Scientist.ScientistHooks;

/// <summary>
/// DevConsole 软依赖：让内置的 <c>spawn</c> 命令支持绳子 —— <c>spawn Rope</c> / <c>spawn Rope[length=120]</c>（也兼容 <c>spawn Knot length=120</c>）。
/// 没装 DevConsole 时 <see cref="RegisterCore"/> 的 JIT 会失败，异常在 <see cref="Register"/> 里吞掉，不影响本 mod 其它功能。
/// </summary>
public static class DevConsoleHooks
{
    private const float DefaultLength = 60f;
    private const float MinLength = 20f;
    private const float MaxLength = 400f;
    private const float TileSize = 20f;

    private static bool registered;
    private static Hook createHook;
    private static Hook autocompleteHook;

    public static void Register()
    {
        if (registered)
        {
            return;
        }
        registered = true;
        try
        {
            RegisterCore();
            Scientist.ScientistLogger.Log("DevConsole: spawn Rope[length=...] 已注册");
        }
        catch (Exception e)
        {
            Scientist.ScientistLogger.Log($"DevConsole 不可用，跳过绳子 spawn 注册（{e.GetType().Name}）");
        }
    }

    /// <summary>只有这个方法引用 DevConsole 的类型，保证它是独立的 JIT 边界。</summary>
    private static void RegisterCore()
    {
        // 1) 给 Knot 注册一个 spawner：spawn Knot length=120
        DevConsole.ObjectSpawner.RegisterSpawner(
            Scientist.Enums.Items.Knot,
            new DevConsole.ObjectSpawner.SimpleSpawnerInfo(
                delegate (AbstractPhysicalObject.AbstractObjectType type, string[] args)
                {
                    if (args == null || args.Length == 0)
                    {
                        return new string[] { "length=20", "length=60", "length=120", "length=240" };
                    }
                    return null;
                },
                delegate (AbstractPhysicalObject.AbstractObjectType type, string[] args, EntityID id, AbstractRoom room, WorldCoordinate pos)
                {
                    return SpawnRopePair(args, room, pos, id);
                }));

        // 1.5) 调试：查看绳子路径推断的最近一次求解耗时
        new DevConsole.Commands.CommandBuilder("rope_path_info")
            .Help("查看绳子地形路径推断（A*）的最近一次求解耗时")
            .Run(delegate (string[] args)
            {
                DevConsole.GameConsole.WriteLine($"RopePathSolver: last solve = {Scientist.Items.RopePathSolver.LastSolveMs:0.00} ms");
            })
            .Register();

        // 1.6) 调试：探测玩家周围地形的最近边法线（用于确认墙角法线是单轴）
        new DevConsole.Commands.CommandBuilder("rope_collision_probe")
            .Help("探测玩家周围 8 个方向：rope_collision_probe [dist=12]")
            .RunGame(delegate (RainWorldGame game, string[] args)
            {
                float probeDist = 12f;
                if (args != null && args.Length > 0 && float.TryParse(args[0], out float parsed))
                {
                    probeDist = parsed;
                }
                Player player = null;
                for (int i = 0; i < game.Players.Count; i++)
                {
                    if (game.Players[i] != null && game.Players[i].realizedCreature is Player p)
                    {
                        player = p;
                        break;
                    }
                }
                if (player?.room == null)
                {
                    DevConsole.GameConsole.WriteLine("rope_collision_probe: 找不到玩家");
                    return;
                }
                Vector2 origin = player.mainBodyChunk.pos;
                for (int i = 0; i < 8; i++)
                {
                    float ang = i * 45f * Mathf.Deg2Rad;
                    Vector2 sample = origin + new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * probeDist;
                    Vector2 normal;
                    float depth;
                    if (Scientist.Items.RopeCollision.Probe(player.room, sample, Scientist.Items.RopeCollision.DEFAULT_RADIUS, out normal, out depth))
                    {
                        DevConsole.GameConsole.WriteLine($"{i * 45}°: hit depth={depth:0.00} normal=({normal.x:0},{normal.y:0})");
                    }
                    else
                    {
                        DevConsole.GameConsole.WriteLine($"{i * 45}°: clear");
                    }
                }
                DevConsole.GameConsole.WriteLine($"last: resolved={Scientist.Items.RopeCollision.lastResolvedCount} maxDepth={Scientist.Items.RopeCollision.lastMaxDepth:0.00}");
            })
            .Register();

        // 1.7) 调试：输出房间里每条绳子的两端质量/钉死/可滑动状态
        new DevConsole.Commands.CommandBuilder("rope_debug")
            .Help("输出房间里每条绳子的两端质量、是否钉死、是否可沿绳滑动")
            .RunGame(delegate (RainWorldGame game, string[] args)
            {
                Player player = null;
                for (int i = 0; i < game.Players.Count; i++)
                {
                    if (game.Players[i] != null && game.Players[i].realizedCreature is Player p)
                    {
                        player = p;
                        break;
                    }
                }
                Room room = player?.room;
                if (room?.climbableVines == null)
                {
                    DevConsole.GameConsole.WriteLine("rope_debug: 房间里没有绳子");
                    return;
                }
                int count = 0;
                for (int i = 0; i < room.climbableVines.vines.Count; i++)
                {
                    if (!(room.climbableVines.vines[i] is Scientist.Items.StringShort rope))
                    {
                        continue;
                    }
                    count++;
                    DevConsole.GameConsole.WriteLine($"#{count} len={rope.maxLength:0} over={rope.debugOverLength:0.0} lenErr={rope.debugLenErr:0.0} tension={rope.debugTension} tensionFrames={rope.debugTensionFrames} pathLen={rope.debugPathLen:0.0} groundStart={rope.debugGroundStart} groundErr={rope.debugGroundErr:0.0} groundWalk={rope.debugGroundWalk:0.0} shrink={rope.debugShrink:0.0} corners={rope.debugCorners} onCorner={rope.debugOnCorner} cornerLen={rope.debugCornerLen:0.0} cornerOwner={rope.debugCornerOwner} shrunk={rope.debugSegmentsShrunk} snapped={rope.debugSnapped} topo={rope.debugTopoBreaks} hist={rope.debugMinHistory} maxJump={rope.debugMaxJump:0.0} maxSpeed={rope.debugMaxSpeed:0.0} | A mass={rope.debugMassA:0.00} pinned={rope.debugPinnedA} slide={rope.debugCanSlideA} | B mass={rope.debugMassB:0.00} pinned={rope.debugPinnedB} slide={rope.debugCanSlideB}");
                }
                DevConsole.GameConsole.WriteLine($"rope_debug: total={count}");
            })
            .Register();

        // 1.8) 调试录制：把房间里所有绳子的每个节点状态写入 D:\Temp\rwropedebug.txt
        new DevConsole.Commands.CommandBuilder("rope_start_debug")
            .Help("开始录制所有绳子的节点 pos/vel/lastPos/invMass 等，写入 D:\\Temp\\rwropedebug.txt")
            .RunGame(delegate (RainWorldGame game, string[] args)
            {
                string error = Scientist.Items.RopeDebugRecorder.Start();
                DevConsole.GameConsole.WriteLine(error ??
                    $"绳子调试录制已开始 → {Scientist.Items.RopeDebugRecorder.FilePath}（用 rope_end_debug 结束）");
            })
            .Register();

        new DevConsole.Commands.CommandBuilder("rope_end_debug")
            .Help("停止绳子调试录制并落盘")
            .RunGame(delegate (RainWorldGame game, string[] args)
            {
                int frames = Scientist.Items.RopeDebugRecorder.Stop();
                DevConsole.GameConsole.WriteLine(
                    $"绳子调试录制已停止：{frames} 帧 → {Scientist.Items.RopeDebugRecorder.FilePath}");
            })
            .Register();

        // 2) 拦截 spawn 的参数解析：把 Rope[length=120] 规范成 Knot + length=120
        MethodInfo createTarget = typeof(DevConsole.ObjectSpawner).GetMethod(
            "CreateAbstractObjectSafe",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static,
            null,
            new Type[] { typeof(string[]), typeof(AbstractRoom), typeof(WorldCoordinate) },
            null);
        // 3) 补全：类型名补 Rope，参数位补 Rope[length=...]
        MethodInfo autocompleteTarget = typeof(DevConsole.ObjectSpawner).GetMethod(
            "AutocompleteSafe",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static,
            null,
            new Type[] { typeof(string[]) },
            null);
        if (createTarget == null || autocompleteTarget == null)
        {
            throw new MissingMethodException($"ObjectSpawner 目标方法未找到（create={createTarget != null}, autocomplete={autocompleteTarget != null}）");
        }

        createHook = new Hook(
            createTarget,
            typeof(DevConsoleHooks).GetMethod(nameof(CreateAbstractObjectSafeHook), BindingFlags.Static | BindingFlags.NonPublic));
        autocompleteHook = new Hook(
            autocompleteTarget,
            typeof(DevConsoleHooks).GetMethod(nameof(AutocompleteSafeHook), BindingFlags.Static | BindingFlags.NonPublic));
    }

    /// <summary>Rope[...] / Knot[...] 走自己的生成逻辑，其余交回原版。</summary>
    private static AbstractPhysicalObject CreateAbstractObjectSafeHook(
        Func<string[], AbstractRoom, WorldCoordinate, AbstractPhysicalObject> orig,
        string[] args,
        AbstractRoom room,
        WorldCoordinate pos)
    {
        string[] normalized = NormalizeArgs(args, out bool isRope);
        if (isRope)
        {
            return SpawnRopePair(normalized, room, pos, null);
        }
        return orig(args, room, pos);
    }

    private static IEnumerable<string> AutocompleteSafeHook(Func<string[], IEnumerable<string>> orig, string[] args)
    {
        List<string> result = new List<string>();
        try
        {
            IEnumerable<string> original = orig(args);
            if (original != null)
            {
                result.AddRange(original);
            }
        }
        catch
        {
        }

        if (args == null || args.Length <= 1)
        {
            // 正在输入类型名：把 Rope 和带参数的写法一起列出来
            result.Add("Rope");
            result.Add("Rope[length=120]");
            return result;
        }
        if (args[0] != null && args[0].StartsWith("Rope", StringComparison.OrdinalIgnoreCase))
        {
            return new string[] { "Rope[length=20]", "Rope[length=60]", "Rope[length=120]", "Rope[length=240]" };
        }
        return result;
    }

    /// <summary>
    /// 把 <c>Rope[length=120,foo]</c> 拆成 <c>["Knot", "length=120", "foo", ...其余参数]</c>。
    /// 不是 Rope/Knot[...] 时 isRope = false，调用方走原版逻辑。
    /// </summary>
    private static string[] NormalizeArgs(string[] args, out bool isRope)
    {
        isRope = false;
        if (args == null || args.Length == 0 || args[0] == null)
        {
            return args;
        }
        string first = args[0];
        int open = first.IndexOf('[');
        string name = open >= 0 ? first.Substring(0, open) : first;
        bool ropeName = name.Equals("Rope", StringComparison.OrdinalIgnoreCase);
        bool knotWithArgs = open >= 0 && name.Equals("Knot", StringComparison.OrdinalIgnoreCase);
        if (!ropeName && !knotWithArgs)
        {
            return args;
        }

        isRope = true;
        List<string> list = new List<string>();
        list.Add("Knot");
        if (open >= 0)
        {
            string inner = first.Substring(open + 1).TrimEnd(']');
            foreach (string part in inner.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string trimmed = part.Trim();
                if (trimmed.Length > 0)
                {
                    list.Add(trimmed);
                }
            }
        }
        for (int i = 1; i < args.Length; i++)
        {
            list.Add(args[i]);
        }
        return list.ToArray();
    }

    /// <summary>
    /// 生成一对绳结（返回第一个，第二个直接挂进房间，绳身在第一个实现时由 TryRebuildFor 一并建出）。
    /// </summary>
    private static AbstractPhysicalObject SpawnRopePair(string[] args, AbstractRoom room, WorldCoordinate pos, EntityID? forcedId)
    {
        float length = ParseLength(args, DefaultLength);
        Vector2 spawnPos = room.realizedRoom != null
            ? room.realizedRoom.MiddleOfTile(pos.Tile)
            : new Vector2(pos.Tile.x * TileSize + TileSize / 2f, pos.Tile.y * TileSize + TileSize / 2f);

        EntityID idA = forcedId ?? room.world.game.GetNewID();
        EntityID idB = room.world.game.GetNewID();
        KnotAbstract a = new KnotAbstract(room.world, null, pos, idA);
        KnotAbstract b = new KnotAbstract(room.world, null, pos, idB);
        a.position = spawnPos + new Vector2(-8f, 0f);
        b.position = spawnPos + new Vector2(8f, 0f);
        a.hasRopePartner = true;
        a.ropePartnerID = idB;
        a.ropeLength = length;
        b.hasRopePartner = true;
        b.ropePartnerID = idA;
        b.ropeLength = length;

        room.AddEntity(b);   // 第二个绳结先挂进房间，等第一个 realize 时由 TryRebuildFor 一并 realize
        return a;
    }

    /// <summary>解析 length=数值（容忍 [length=数值] 写法），超出范围夹到 20~400。</summary>
    private static float ParseLength(string[] args, float fallback)
    {
        if (args == null)
        {
            return fallback;
        }
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == null)
            {
                continue;
            }
            string token = args[i].Trim().Trim('[', ']');
            int eq = token.IndexOf('=');
            if (eq < 0)
            {
                continue;
            }
            if (!token.Substring(0, eq).Trim().Equals("length", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }
            string value = token.Substring(eq + 1).Trim();
            if (float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out float parsed))
            {
                return Mathf.Clamp(parsed, MinLength, MaxLength);
            }
        }
        return fallback;
    }
}
