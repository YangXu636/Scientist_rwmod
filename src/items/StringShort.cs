using RWCustom;
using Scientist;
using Scientist.Items.AbstractPhysicalObjects;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Scientist.Items;

/// <summary>
/// 绳身。要点：
///  1) 节点列表是动态的：相邻两节点连线若穿过实体地形，就在碰撞点插入新节点，避免"绳段卡进地里"；
///  2) 绳子无弹性、不可拉长：每段只做"不超过 segLen"的约束（segLen = maxLength / (节点数 - 1)），
///     因此插入/合并节点不会改变总长上限；
///  3) 本游戏没有加速度概念，节点之间用"按质量共速"处理：把沿连线方向的相对速度按质量加权抹平。
///     绳结端（被玩家/生物抓着）的等效质量还会乘系数——玩家有按键时视为极重（拽得动绳子、
///     自己不被绳子摆布），松开按键时按常规质量（会被绳子拖动），这样"按键"就进入了受力计算。
/// </summary>
public class StringShort : UpdatableAndDeletable, IClimbableVine, IDrawable
{
    public const float NODE_RADIUS = 2f;          // 节点半径（对齐 ClimbableVine.Rad）
    public const float NODE_MASS = 0.25f;         // 节点质量（对齐 ClimbableVine.Mass）
    public const int MAX_NODES = 200;             // 节点数上限
    public const float PLAYER_INPUT_MASS_FACTOR = 20f;   // 玩家有输入时的等效质量倍率
    private const int SUBSTEPS = 3;               // XPBD 子步数
    private const int SOLVER_ITERATIONS = 4;      // 每个子步的约束迭代次数
    private const float DAMPING = 0.97f;           // 速度阻尼（每条子步回推速度时应用，3 子步 ≈ 0.91/帧）
    private const float AIR_DAMPING = 0.985f;      // 空气阻力：每子步额外衰减（3 子步 ≈ 0.956/帧），让悬空自由端能停下来
    private const float NODE_SPACING = 2f;         // 固定节点间距（px）：节点数 = 绳长 / 2，不再动态增删
    private const float INTERNAL_FRICTION = 0.15f; // 段内沿绳方向的相对速度衰减（绳子的内摩擦）
    private const float COMPRESSION_FRICTION = 0.6f; // 挤压方向的额外阻尼（松弛时防止自由端上下弹跳）
    private const float MAX_CORRECTION = 4f;       // 单次约束迭代的最大位置修正（px），防止一步跨过整格地形
    private const float ENDPOINT_SHIFT_LIMIT = 6f; // 端点单次沿绳位移上限（px）
    private const int ENDPOINT_BISECT_STEPS = 3;   // 端点部分移动的二分次数
    private const float SEGMENT_CHECK_FACTOR = 1.5f; // 段长超过 SegLen 的多少倍才做段级检测
    private const float SEGMENT_CHECK_MIN = 8f;      // 段级检测的最小长度（px）
    private const int SEGMENT_BISECT_STEPS = 4;      // 二分求最大可行长度的迭代次数
    private const int STUCK_ESCAPE_SUBSTEPS = 3;     // 连续深陷多少子步后强制吸附到表面
    private const float MAX_NODE_SPEED = 20f;        // 节点速度硬上限（px/帧），防止数值爆炸
    private const float GROUND_SNAP_MARGIN = 1f;     // 判定"贴地"的额外余量（px）
    private const float GROUND_FRICTION = 0.5f;      // 贴地节点每子步的切向速度衰减（模拟静摩擦）
    private const float GROUND_SHRINK_EXCLUDE = 0.5f; // 贴地强度超过此值的节点不参与分布式收缩
    private const float GROUND_WALK_STEP = 0.5f;      // 沿表面排布的单子步移动上限（px）
    private const float GROUND_RELEASE_SPEED = 1.5f; // 法向向外速度超过此值就允许离开地面（px/帧）
    private const float SHRINK_STEP_MAX = 0.5f;      // 分布式收缩单子步上限（px）
    private const float SHRINK_ACCEL = 0.5f;         // 收缩速度系数（速度级，px/帧²）
    private const float SHRINK_TRIGGER_MARGIN = 2f;  // 触发分布式收缩的额外容差（px），避免正常波动被当成超长
    private const float SHRINK_LENGTH_RATIO = 0.98f; // 有效绳长 = 路径长度 × 该系数（弦长略小于弧长，留余量）
    private const float TENSION_ON_RATIO = 0.98f;    // 张力开启阈值：路径长度 > 绳长 × 该系数
    private const float TENSION_OFF_RATIO = 0.94f;   // 张力关闭阈值：低于此值才退出（形成滞回）
    private const int TENSION_CONFIRM_FRAMES = 5;    // 连续多少帧满足开启条件才真正算绷紧
    private const int MAX_WALK_STEPS = 128;          // 沿表面步进的最大步数（1px/步，防御死循环）
    private const float SHRINK_ACTIVE_RATIO = 2f;    // 质量比超过此值才算"有主动端"
    private static readonly bool USE_GROUND_WALK = true;  // 贴地吸附开关：false 时完全跳过（整体回退用）
    private const int HISTORY_FRAMES = 4;            // 每个节点保留的历史帧数（跳变抑制/拓扑保护用）
    private const float NODE_JUMP_LIMIT = 6f;        // 单子步允许的最大位置修正（px），超出则沿历史方向分步消化
    private const float SEGMENT_SHRINK_LIMIT = 4f;   // 段级收缩单子步上限（px），避免一次缩几十像素把绕行拓扑抹平
    private const float TOPO_ANGLE_COS = -0.5f;      // 段方向与历史方向夹角 > 120° 视为节点顺序被打乱
    private const float OVERLENGTH_TOLERANCE = 0.1f; // 允许短暂超长 maxLength×(1+该比例)，之后靠距离约束收回
    private const float GRAVITY_COEF = 0.9f;
    private const float TUNNEL_STEP = 2f;         // 穿地形检测步进
    private const float MERGE_FACTOR = 0.2f;      // 节点过密时的合并阈值（相对 segLen）
    private const float PIPE_DETOUR_FACTOR = 0.8f; // 走管道至少要近到这个比例以内才采用

    /// <summary>攀爬查询上下文：由 ScientistPlugin.Player_Update 在调用原版 Update 期间设置，用于判断"这绳是不是正被自己拿着"。</summary>
    public static Player climbQueryPlayer = null;

    public Color baseColor;
    public float maxLength;                       // 绳子总长上限（不可拉长）
    public KnotAbstract stuckPosA;
    public KnotAbstract stuckPosB;
    public Rope[] ropes;
    public TriangleMesh tm;
    public int afterChangeRoomTimer = 0;
    public EntityID afterChangeRoomBase;
    public WorldCoordinate afterChangeRoomCoord;
    public bool hasPlayer = true;
    public EntityID ID;

    private class Node
    {
        public Vector2 pos;
        public Vector2 lastPos;
        public Vector2 vel;
        public float invMass;    // 0 = 固定（绳结端 / 拐角吸附点）
        public float limit;      // 到下一个节点的最大距离（总长 = Σ limit，插入/合并不改变它）
        public bool pinned;      // 拐角吸附点：不参与物理，只被投影到"贴地形且不穿墙"的位置
        public int stuckFrames;  // 连续处于深陷状态的子步数（用于强制吸附）
        public float groundStrength; // 本子步的贴地强度 0..1（0 = 悬空，1 = 完全贴地）
        public Vector2 lastGroundNormal; // 上一子步的地形法线（法线滞回用，避免平台边缘 90° 跳变）

        // 历史位置（每帧一次）：用来判断"这个节点本来应该在哪"，
        // 任何大跳变都优先回退到自己的历史轨迹附近，而不是瞬移到几何上更省事的位置。
        public readonly Vector2[] history = new Vector2[HISTORY_FRAMES];
        public int historyCount;
        public int historyIndex;

        public Node(Vector2 p)
        {
            this.pos = p;
            this.lastPos = p;
            this.vel = Vector2.zero;
            this.invMass = 1f / NODE_MASS;
            this.limit = 0f;
            this.pinned = false;
            for (int i = 0; i < HISTORY_FRAMES; i++)
            {
                this.history[i] = p;
            }
        }

        /// <summary>记录一次历史位置（每帧末尾调用一次）。</summary>
        public void PushHistory()
        {
            this.history[this.historyIndex] = this.pos;
            this.historyIndex = (this.historyIndex + 1) % HISTORY_FRAMES;
            if (this.historyCount < HISTORY_FRAMES)
            {
                this.historyCount++;
            }
        }

        /// <summary>最近 frames 帧（含最新一帧）的平均位置。</summary>
        public Vector2 RecentPos(int frames)
        {
            if (this.historyCount <= 0)
            {
                return this.pos;
            }
            int n = Mathf.Min(frames, this.historyCount);
            Vector2 sum = Vector2.zero;
            int idx = this.historyIndex - 1;
            for (int i = 0; i < n; i++)
            {
                if (idx < 0)
                {
                    idx += HISTORY_FRAMES;
                }
                sum += this.history[idx];
                idx--;
            }
            return sum / n;
        }

        /// <summary>最近一帧的位移方向（单位向量）；历史不足时返回零。</summary>
        public Vector2 LastDirection()
        {
            if (this.historyCount < 2)
            {
                return Vector2.zero;
            }
            int i0 = this.historyIndex - 1;
            if (i0 < 0)
            {
                i0 += HISTORY_FRAMES;
            }
            int i1 = this.historyIndex - 2;
            if (i1 < 0)
            {
                i1 += HISTORY_FRAMES;
            }
            Vector2 d = this.history[i0] - this.history[i1];
            return d.sqrMagnitude > 1E-08f ? d.normalized : Vector2.zero;
        }
    }

    private readonly List<Node> nodes = new List<Node>();
    private int initialNodeCount = 4;

    // ==== 段的两端各可以是"绳结"或"固定锚点"（锚点 = 管道口/通道口）====
    private Vector2? fixedEndPosA;
    private Vector2? fixedEndPosB;
    // ==== 同一条绳的所有段共享总长；管道本身不建绳身，但按 pipeLength 占长度预算 ====
    private static readonly Dictionary<string, SharedRope> sharedRopes = new Dictionary<string, SharedRope>();
    private string sharedKey;
    private int sharedSlot = -1;
    private float effectiveMaxLength;

    // ==== 地形引导（RopePathSolver）：软约束，只修正垂直于路径切线的分量 ====
    private const float GUIDE_RESAMPLE_MIN = 20f;   // 端点移动超过此距离就重算路径
    private const int GUIDE_MAX_AGE = 30;           // 兜底重算间隔（帧）
    private const float GUIDE_WEIGHT = 0.02f;       // 引导强度（只作用于垂直于切线的分量，且仅贴墙节点）
    private static readonly bool GUIDE_ENABLED = false;  // 暂时关闭 A* 软引导：形状只由重力/段约束/总长/碰撞决定
    private static int nextGuideSolveFrame = -1;    // 全局限流：每帧最多求解一条绳

    private readonly List<Vector2> guidePath = new List<Vector2>();
    private Vector2[] guidePoints;
    private Vector2[] guideTangents;
    private bool[] guideValid;
    private int guideFrame = -100000;
    private Vector2 guideEndA;
    private Vector2 guideEndB;
    private float guidePathLength;
    // 碰撞前置：批量扫描的临时缓冲（避免每子步分配）
    private Vector2[] sweepTargets;
    private bool[] sweepSnap;        // 本子步被强制吸附的节点（清零速度）

    // ==== 诊断（rope_debug 读取）====
    public float debugMassA;
    public float debugMassB;
    public bool debugPinnedA;
    public bool debugPinnedB;
    public bool debugCanSlideA;
    public bool debugCanSlideB;
    public int debugSegmentsShrunk;
    public float debugMaxSpeed;
    public int debugSnapped;
    public int debugMinHistory;
    public float debugMaxJump;
    public int debugTopoBreaks;
    public float debugOverLength;
    public int debugCorners;
    public int debugOnCorner;
    public float debugCornerLen;
    public int debugCornerOwner;
    public float debugLenErr;
    public float debugShrink;
    public float debugPathLen;
    public int debugTension;
    private bool hasTension;
    private int tensionFrames;
    public int debugTensionFrames;
    private int groundStartIndex = -1;
    public int debugGroundStart;
    public float debugGroundErr;
    public float debugGroundWalk;
    private bool shrinkActiveIsA;
    private bool shrinkActiveIsB;
    private readonly List<RopeCollision.Corner> cornerList = new List<RopeCollision.Corner>();
    private readonly HashSet<Vector2> cornerSeen = new HashSet<Vector2>();

    private class SharedRope
    {
        public float maxLength;
        public readonly float[] needs = new float[8];
    }

    public StringShort(Room room, float length, KnotAbstract spawnPosA, KnotAbstract spawnPosB)
    {
        this.ID = room.game.GetNewID();
        this.room = room;
        this.stuckPosA = spawnPosA;
        this.stuckPosB = spawnPosB;
        this.maxLength = Mathf.Max(20f, length);
        this.effectiveMaxLength = this.maxLength;

        Vector2 a = spawnPosA != null ? spawnPosA.CurrentPosition : Vector2.zero;
        Vector2 b = spawnPosB != null ? spawnPosB.CurrentPosition : a;

        // 固定节点：每 2px 一个（至少 4 个、最多 100 个，超长绳间距自动放宽）。节点数从此不再变化。
        int n = Mathf.Clamp(Mathf.RoundToInt(this.maxLength / NODE_SPACING), 4, 100);
        n = Mathf.Min(n, MAX_NODES);
        this.initialNodeCount = n;
        float segLimit = this.maxLength / (n - 1);
        for (int i = 0; i < n; i++)
        {
            Node node = new Node(Vector2.Lerp(a, b, (float)i / (n - 1)));
            node.limit = segLimit;   // 总长 = Σ limit = maxLength
            this.nodes.Add(node);
        }
        this.nodes[0].invMass = 0f;
        this.nodes[n - 1].invMass = 0f;

        if (this.room.climbableVines == null)
        {
            this.room.climbableVines = new ClimbableVinesSystem();
            this.room.AddObject(this.room.climbableVines);
        }
        this.room.climbableVines.vines.Add(this);
    }

    /// <summary>段：A 端接绳结、B 端固定在锚点（管道口/通道口）。</summary>
    public StringShort(Room room, float length, KnotAbstract knotEnd, Vector2 fixedEnd) : this(room, length, knotEnd, null)
    {
        this.fixedEndPosB = fixedEnd;
        Node last = this.nodes[this.nodes.Count - 1];
        last.pos = fixedEnd;
        last.lastPos = fixedEnd;
    }

    /// <summary>段：A 端固定在锚点、B 端接绳结。</summary>
    public StringShort(Room room, float length, Vector2 fixedEnd, KnotAbstract knotEnd) : this(room, length, null, knotEnd)
    {
        this.fixedEndPosA = fixedEnd;
        Node first = this.nodes[0];
        first.pos = fixedEnd;
        first.lastPos = fixedEnd;
    }

    /// <summary>登记到共享长度池：同一条绳的所有段共用一个 key，各占一个 slot。管道长度不占用绳长，故不参与分配。</summary>
    public void SetSharedLength(string key, int slot, float maxLength)
    {
        this.sharedKey = key;
        this.sharedSlot = slot;
        SharedRope data;
        if (!sharedRopes.TryGetValue(key, out data))
        {
            data = new SharedRope { maxLength = maxLength };
            sharedRopes[key] = data;
        }
        if (slot < 0 || slot >= data.needs.Length)
        {
            this.sharedSlot = -1;
        }
    }

    /// <summary>按"各段当前所需长度"的比例瓜分总长（即"计算长度变化"）。管道内部不算绳长，只分配露在外面的部分。</summary>
    private void UpdateSharedLength()
    {
        SharedRope data;
        if (this.sharedKey == null || this.sharedSlot < 0 || !sharedRopes.TryGetValue(this.sharedKey, out data))
        {
            this.effectiveMaxLength = this.maxLength;
            return;
        }
        data.needs[this.sharedSlot] = Vector2.Distance(this.nodes[0].pos, this.nodes[this.nodes.Count - 1].pos) + 10f;
        float total = 0f;   // 管道长度不计入绳长
        for (int i = 0; i < data.needs.Length; i++)
        {
            total += data.needs[i];
        }
        float mine = data.needs[this.sharedSlot];
        this.effectiveMaxLength = total <= 0f ? this.maxLength : Mathf.Max(20f, data.maxLength * mine / total);
    }

    private int NodeCount => this.nodes.Count;

    /// <summary>每段的长度上限。节点数变化时它随之变化，因此"总长上限 = segLen × (n - 1)"恒等于可用总长。</summary>
    private float SegLen => this.nodes.Count <= 1 ? this.effectiveMaxLength : this.effectiveMaxLength / (this.nodes.Count - 1);

    public override void Update(bool eu)
    {
        base.Update(eu);
        if (this.afterChangeRoomTimer > 0 && this.hasPlayer)
        {
            this.afterChangeRoomTimer--;
        }
        if (this.nodes.Count < 2)
        {
            return;
        }

        this.UpdateSharedLength();             // 跨房间时与另一段瓜分总长
        this.UpdateShrinkMode();               // 判定这一帧是否有主动端（决定收缩模式）
        this.debugSegmentsShrunk = 0;          // 本帧段级缩短计数（rope_debug 读取）
        this.debugSnapped = 0;                 // 本帧强制吸附计数
        this.debugTopoBreaks = 0;              // 本帧拓扑回退计数
        this.debugShrink = 0f;                 // 本帧分布式收缩总量
        this.debugGroundErr = 0f;              // 本帧沿表面排布的目标偏差累计
        this.debugGroundWalk = 0f;             // 本帧沿表面排布的实际移动量
        this.TryUpdateGuide();                 // 地形路径推断（事件驱动 + 全局限流）
        this.UpdateDebugInfo();                // 诊断字段（rope_debug 读取）
        // 节点固定（每 2px 一个）：不再做"穿地形插点/过密合并/拐角吸附"，
        // 这些机制会改变段长，正是之前"永不静止"和"两端乱联动"的来源。

        // XPBD：每个子步「预测 → 位置约束迭代 → 由位置差回推速度」。
        float h = 1f / SUBSTEPS;
        for (int step = 0; step < SUBSTEPS; step++)
        {
            this.PredictStep(h);
            this.ConnectToKnots();                  // 端点贴合绳结/锚点（只改位置，不覆盖 lastPos）
            this.SweepNodes();                      // 碰撞前置：圆形扫描 + 最近边法线校正（批量写回）
            for (int it = 0; it < SOLVER_ITERATIONS; it++)
            {
                this.SolveDistanceConstraints();    // 不可拉长（只修拉伸）
                this.SolvePushApart();              // 相邻两段软推开，避免所有节点堆成一团
            }
            this.SolveGuideConstraints();           // 软引导：只修垂直于切线的分量，不改变段长
            // 旧的 SharedPhysics 碰撞（CollideNodesWithTerrain / PushOutOfSolid）已停用：
            // 现在由 SweepNodes（前置圆扫）与 CollideNodesFinish（末尾法线收尾）负责，方法保留以便回退。
            this.EnsureSegmentsClear();             // 段级检测：两端都在墙外、整段横穿地形的长段
            this.ProtectTopology();                 // 拓扑保护：节点顺序被打乱时回退到自己的历史轨迹
            this.CollideNodesFinish();              // 收尾：距离约束可能把节点拉回地形，这里再推一次（只推不扫）
            this.SnapGroundedNodes();               // 贴地吸附：投影到表面外 r 处，消除法向抖动
            // 贴地段起点（从 A 端起扫描）：沿表面排布以它为锚
            this.groundStartIndex = this.FindGroundStart();
            this.debugGroundStart = this.groundStartIndex;
            this.UpdateGroundWalk();                 // 沿表面排布（仅 hasTension 时生效，张力按帧判定）
            this.UpdateVelocitiesFromPositions(h);
            this.RemoveInwardVelocity();            // 去反弹：消掉速度里"朝地形内部"的分量
            this.DampGroundedVelocity();            // 贴地静摩擦：衰减贴地节点的切向速度
            // 分布式收缩（速度级，贴地时投影到切线，陷入时回退位置级）：放在摩擦之前，同子步内即被衰减
            float pathLen = 0f;
            for (int si = 0; si < this.nodes.Count - 1; si++)
            {
                pathLen += Vector2.Distance(this.nodes[si].pos, this.nodes[si + 1].pos);
            }
            float shrinkOver = pathLen * SHRINK_LENGTH_RATIO - this.effectiveMaxLength;
            if (shrinkOver > SHRINK_TRIGGER_MARGIN)
            {
                this.ShrinkNodesTowardActive(shrinkOver);
            }
            this.SolveFriction();                   // 内摩擦：衰减沿绳方向的相对速度，绳子才会真正停下
            // 每子步都夹一次总长：绳结被抓时它的 pos 每帧会被抓取逻辑覆写，
            // 只在帧末夹一次会被完全抵消（表现为"怎么拉都能变长"）。
            this.SolveTotalLengthConstraint();
        }
        // 诊断：本帧所有节点里的最大速度（用来确认"深陷后速度飙升"是否还在发生）
        float maxSpeed = 0f;
        for (int i = 0; i < this.nodes.Count; i++)
        {
            float v = this.nodes[i].vel.magnitude;
            if (v > maxSpeed)
            {
                maxSpeed = v;
            }
        }
        this.debugMaxSpeed = maxSpeed;

        // 端点节点不参与地形碰撞（位置由绳结决定），但渲染必须留在空气里，否则就是"绳子穿墙"。
        this.ProjectEndNodesOutOfTerrain();

        // 张力判定（每帧一次，带滞回）：高于 ON 开始计数，连续 TENSION_CONFIRM_FRAMES 帧才算绷紧；
        // 低于 OFF 立即退出。0.94~0.98 区间内保持上一次状态，避免每帧开关导致绳子反复收紧放松。
        float tensionLen = 0f;
        for (int pi = 0; pi < this.nodes.Count - 1; pi++)
        {
            tensionLen += Vector2.Distance(this.nodes[pi].pos, this.nodes[pi + 1].pos);
        }
        this.debugPathLen = tensionLen;
        float tensionOn = this.effectiveMaxLength * TENSION_ON_RATIO;
        float tensionOff = this.effectiveMaxLength * TENSION_OFF_RATIO;
        if (this.hasTension)
        {
            if (tensionLen < tensionOff)
            {
                this.hasTension = false;
                this.tensionFrames = 0;
            }
        }
        else if (tensionLen > tensionOn)
        {
            this.tensionFrames++;
            if (this.tensionFrames >= TENSION_CONFIRM_FRAMES)
            {
                this.hasTension = true;
            }
        }
        else
        {
            this.tensionFrames = 0;
        }
        this.debugTension = this.hasTension ? 1 : 0;
        this.debugTensionFrames = this.tensionFrames;

        // 诊断：当前超长像素（两端距离 - 有效绳长）
        Vector2 dbgA;
        Vector2 dbgB;
        BodyChunk dbgChunkA;
        BodyChunk dbgChunkB;
        if (this.TryGetEnd(this.stuckPosA, this.fixedEndPosA, out dbgA, out dbgChunkA) &&
            this.TryGetEnd(this.stuckPosB, this.fixedEndPosB, out dbgB, out dbgChunkB))
        {
            this.debugOverLength = Mathf.Max(0f, Vector2.Distance(dbgA, dbgB) - this.effectiveMaxLength);
        }

        // 诊断：拐角（只读，不改变行为）
        this.cornerSeen.Clear();
        int onCorner = 0;
        float cornerLen = 0f;
        int owner = -1;
        if (this.room != null)
        {
            for (int ni = 0; ni < this.nodes.Count; ni++)
            {
                Vector2 np = this.nodes[ni].pos;
                RopeCollision.CollectCorners(this.room, np, this.cornerList);
                bool near = false;
                for (int k = 0; k < this.cornerList.Count; k++)
                {
                    RopeCollision.Corner c = this.cornerList[k];
                    if (!near && Vector2.Distance(np, c.center) < RopeCollision.CORNER_RADIUS + NODE_RADIUS)
                    {
                        near = true;
                    }
                    if (this.cornerSeen.Add(c.center))
                    {
                        cornerLen += RopeCollision.CornerArcLength(c);   // 绕圆弧长固定 r·θ
                        if (owner < 0 && this.nodes.Count >= 2)
                        {
                            owner = this.FindSegmentAcross(c.center);    // 与"跨过它"的相邻段绑定
                        }
                    }
                }
                if (near)
                {
                    onCorner++;
                }
            }
        }
        this.debugCorners = this.cornerSeen.Count;
        this.debugOnCorner = onCorner;
        this.debugCornerLen = cornerLen;
        this.debugCornerOwner = owner;

        // 总长记账：相邻节点弦长之和 vs 有效绳长（恒 ≥ 0 表示没有超长；负值说明约束没收敛）
        float chordSum = 0f;
        for (int ci = 0; ci < this.nodes.Count - 1; ci++)
        {
            chordSum += Vector2.Distance(this.nodes[ci].pos, this.nodes[ci + 1].pos);
        }
        this.debugLenErr = this.effectiveMaxLength - chordSum;

        // 诊断 + 历史记录：历史长度 / 本帧最大帧间位移，然后写入历史
        int minHistory = int.MaxValue;
        float maxJump = 0f;
        for (int i = 0; i < this.nodes.Count; i++)
        {
            Node n = this.nodes[i];
            if (n.historyCount < minHistory)
            {
                minHistory = n.historyCount;
            }
            if (n.historyCount > 0)
            {
                float jump = Vector2.Distance(n.pos, n.RecentPos(1));
                if (jump > maxJump)
                {
                    maxJump = jump;
                }
            }
            n.PushHistory();
        }
        this.debugMinHistory = minHistory == int.MaxValue ? 0 : minHistory;
        this.debugMaxJump = maxJump;

        // 调试录制（只有 rope_start_debug 期间才生效）
        if (RopeDebugRecorder.Enabled)
        {
            RopeDebugRecorder.Capture(this);
        }
    }

    /// <summary>调试录制：把本帧状态写进 sb（rope 一行全局量 + 每个节点一行）。</summary>
    public void WriteDebugSnapshot(StringBuilder sb, int index)
    {
        int count = this.nodes.Count;
        sb.Append("rope ").Append(index)
          .Append(" idA=").Append(this.stuckPosA != null ? this.stuckPosA.ID.number : -1)
          .Append(" idB=").Append(this.stuckPosB != null ? this.stuckPosB.ID.number : -1)
          .Append(" nodes=").Append(count)
          .Append(" maxLen=").Append(this.effectiveMaxLength.ToString("0.00"))
          .Append(" segLen=").Append(this.SegLen.ToString("0.00"))
          .Append(" pathLen=").Append(this.debugPathLen.ToString("0.00"))
          .Append(" tension=").Append(this.debugTension)
          .Append(" tFrames=").Append(this.debugTensionFrames)
          .Append(" shrink=").Append(this.debugShrink.ToString("0.00"))
          .Append(" maxSpeed=").Append(this.debugMaxSpeed.ToString("0.00"))
          .Append(" over=").Append(this.debugOverLength.ToString("0.00"))
          .Append(" groundStart=").Append(this.debugGroundStart)
          .Append(" groundErr=").Append(this.debugGroundErr.ToString("0.00"))
          .Append('\n');
        for (int i = 0; i < count; i++)
        {
            Node n = this.nodes[i];
            sb.Append("  i=").Append(i)
              .Append(" pos=").Append(Vec(n.pos))
              .Append(" vel=").Append(Vec(n.vel))
              .Append(" last=").Append(Vec(n.lastPos))
              .Append(" invM=").Append(n.invMass.ToString("0.000"))
              .Append(" limit=").Append(n.limit.ToString("0.00"))
              .Append(" stuck=").Append(n.stuckFrames)
              .Append(" ground=").Append(n.groundStrength.ToString("0.00"))
              .Append('\n');
        }
    }

    private static string Vec(Vector2 v)
    {
        return v.x.ToString("0.00") + "," + v.y.ToString("0.00");
    }

    /// <summary>两端节点钉在绳结/锚点上：只覆盖位置，保留 lastPos，让速度回推能反映"被端点带动"。</summary>
    public void ConnectToKnots()
    {
        if (this.stuckPosA != null)
        {
            this.nodes[0].pos = this.stuckPosA.CurrentPosition;
        }
        else if (this.fixedEndPosA != null)
        {
            this.nodes[0].pos = this.fixedEndPosA.Value;
        }
        if (this.stuckPosB != null)
        {
            this.nodes[this.nodes.Count - 1].pos = this.stuckPosB.CurrentPosition;
        }
        else if (this.fixedEndPosB != null)
        {
            this.nodes[this.nodes.Count - 1].pos = this.fixedEndPosB.Value;
        }
        // 端点段强制收缩：绳结可能在一帧内被移动很远，若端点段超过 SegLen，
        // 这条长段就会横穿整格地形（20px），表现为"绳子卡进/穿过地形"。
        this.ClampEndSegment(0);
        this.ClampEndSegment(this.nodes.Count - 1);
    }

    /// <summary>把端点相邻的内部节点拉回，保证端点段不会长到能跨过地形。</summary>
    private void ClampEndSegment(int endIndex)
    {
        int count = this.nodes.Count;
        if (count < 2 || endIndex < 0 || endIndex >= count)
        {
            return;
        }
        int inner = endIndex == 0 ? 1 : count - 2;
        float limit = this.SegLen;
        Vector2 d = this.nodes[inner].pos - this.nodes[endIndex].pos;
        float dist = d.magnitude;
        // 阈值放宽到 3 倍段长（绳子有惯性，端点一帧移动大时相邻节点本就该被拉开）；
        // 但每次最多收缩 SEGMENT_SHRINK_LIMIT，避免"节点 1 被瞬移到绳结旁边"把绕行拓扑抹平。
        if (dist <= limit * 3f || dist < 1E-05f)
        {
            return;
        }
        Node innerNode = this.nodes[inner];
        float newDist = Mathf.Max(limit * 2.9f, dist - SEGMENT_SHRINK_LIMIT);
        Vector2 target = this.nodes[endIndex].pos + d / dist * newDist;
        Vector2 histDir = innerNode.LastDirection();
        if (histDir.sqrMagnitude > 0.1f && Vector2.Dot(d / dist, histDir) < 0f)
        {
            // 段方向与本节点上帧运动相反：优先退回它自己的历史轨迹
            target = innerNode.RecentPos(2);
        }
        target = this.LimitJump(inner, target);
        Vector2 delta = target - innerNode.pos;
        innerNode.pos = target;
        innerNode.lastPos += delta;   // 同步平移：端点段收缩不再产生反向速度
        // 速度不清零：保留惯性，让距离约束在后续迭代/子步里自然收敛
    }

    /// <summary>子步预测：施加重力并按子步长积分（速度随后由位置差回推）。</summary>
    private void PredictStep(float h)
    {
        for (int i = 0; i < this.nodes.Count; i++)
        {
            Node n = this.nodes[i];
            if (n.invMass <= 0f)
            {
                continue;
            }
            n.vel.y -= GRAVITY_COEF * this.room.gravity * this.GravityAffected(i) * h;

            // 低重力环境下的空气扰动（与原版 Vine 一致）
            if (this.room.gravity < 1f && this.room.readyForAI && this.room.aimap != null &&
                this.room.aimap.getTerrainProximity(n.pos) < 4)
            {
                IntVector2 tilePosition = this.room.GetTilePosition(n.pos);
                Vector2 push = Vector2.zero;
                for (int k = 0; k < 4; k++)
                {
                    if (!this.room.GetTile(tilePosition + Custom.fourDirections[k]).IsSolid() &&
                        !this.room.aimap.getAItile(tilePosition + Custom.fourDirections[k]).narrowSpace)
                    {
                        float near = 0f;
                        for (int l = 0; l < 4; l++)
                        {
                            near += this.room.aimap.getTerrainProximity(tilePosition + Custom.fourDirections[k] + Custom.fourDirections[l]);
                        }
                        push += Custom.fourDirections[k].ToVector2() * near;
                    }
                }
                n.vel += push.normalized *
                    ((!this.room.GetTile(n.pos).IsSolid()) ?
                     Custom.LerpMap((float)this.room.aimap.getTerrainProximity(n.pos), 0f, 3f, 2f, 0.2f) : 1f) *
                    (1f - this.room.gravity) * h;
            }

            n.lastPos = n.pos;
            n.pos += n.vel * h;
        }
    }

    /// <summary>
    /// 碰撞前置：对每个内部节点用 prevPos → pos 做圆形扫描，命中就沿最近边法线推出。
    /// 所有节点的扫描结果先存进缓冲，全部算完再统一写回（避免串行修正污染后续判定）。
    /// 端点由绳结位置决定，不参与扫描。
    /// </summary>
    private void SweepNodes()
    {
        int count = this.nodes.Count;
        if (count < 3 || this.room == null)
        {
            return;
        }
        if (this.sweepTargets == null || this.sweepTargets.Length != count)
        {
            this.sweepTargets = new Vector2[count];
        }
        for (int i = 1; i < count - 1; i++)
        {
            Vector2 resolved;
            Vector2 normal;
            float depth;
            bool hit = RopeCollision.Sweep(
                this.room,
                this.nodes[i].lastPos,
                this.nodes[i].pos,
                RopeCollision.DEFAULT_RADIUS,
                out resolved,
                out normal,
                out depth);
            this.sweepTargets[i] = hit ? resolved : this.nodes[i].pos;
        }
        for (int i = 1; i < count - 1; i++)
        {
            Node n = this.nodes[i];
            Vector2 limited = this.LimitJump(i, this.sweepTargets[i]);
            Vector2 delta = limited - n.pos;
            if (delta.sqrMagnitude <= 1E-10f)
            {
                continue;
            }
            n.pos = limited;
            n.lastPos += delta;   // 同步平移 lastPos：碰撞复位不再产生虚假速度
        }
    }

    /// <summary>
    /// 末尾收尾：距离约束可能把节点重新拉回地形，这里用一次浅层法线校正（只推、不再扫描），
    /// 保证"最终位置在空气侧"。放在迭代之后、速度回推之前。
    /// </summary>
    private void CollideNodesFinish()
    {
        int count = this.nodes.Count;
        if (count < 3 || this.room == null)
        {
            return;
        }
        if (this.sweepTargets == null || this.sweepTargets.Length != count)
        {
            this.sweepTargets = new Vector2[count];
        }
        // sweepSnap 必须独立判断：SweepNodes 也会重建 sweepTargets（但不建这个数组），
        // 若只在 sweepTargets 重建时一起初始化，这里就会拿到 null 并抛 NRE。
        if (this.sweepSnap == null || this.sweepSnap.Length != count)
        {
            this.sweepSnap = new bool[count];
        }
        for (int i = 0; i < count; i++)
        {
            this.sweepTargets[i] = this.nodes[i].pos;
            this.sweepSnap[i] = false;
        }
        for (int i = 1; i < count - 1; i++)
        {
            Node n = this.nodes[i];
            Vector2 normal;
            float depth;
            if (!RopeCollision.Probe(this.room, n.pos, RopeCollision.DEFAULT_RADIUS, n.lastGroundNormal, out normal, out depth))
            {
                n.stuckFrames = 0;
                continue;
            }
            n.lastGroundNormal = normal;        // 与贴地吸附共用同一法线记忆，避免平台边缘跳变
            n.stuckFrames = depth > NODE_RADIUS ? n.stuckFrames + 1 : 0;
            if (n.stuckFrames >= STUCK_ESCAPE_SUBSTEPS)
            {
                // 连续多个子步都推不出来：直接吸附到表面并清零速度（优先不抖、不崩，允许长度瞬时偏差）
                this.sweepTargets[i] = n.pos + normal * (depth + RopeCollision.EPSILON);
                this.sweepSnap[i] = true;
                n.stuckFrames = 0;
                this.debugSnapped++;
                continue;
            }
            // 常规：单次推出限幅（超出部分留到后续子步分摊）
            this.sweepTargets[i] = n.pos + normal * (Mathf.Min(depth, RopeCollision.MAX_PUSH) + RopeCollision.EPSILON);
        }
        for (int i = 1; i < count - 1; i++)
        {
            Node n = this.nodes[i];
            Vector2 limited = this.LimitJump(i, this.sweepTargets[i]);
            if (this.sweepSnap[i])
            {
                n.pos = limited;
                n.lastPos = n.pos;   // 速度归零：不再带着穿透深度飞出去
                n.vel = Vector2.zero;
                continue;
            }
            Vector2 delta = limited - n.pos;
            if (delta.sqrMagnitude <= 1E-10f)
            {
                continue;
            }
            n.pos = limited;
            n.lastPos += delta;   // 同步平移 lastPos：收尾推出不再产生虚假速度
        }
    }

    /// <summary>
    /// 贴地吸附：把"贴着地形"的节点投影到表面外 r 处，并同步 lastPos（消除法向速度）。
    ///  · 贴地节点不再被重力反复压入/推出——这正是躺地绳微微蠕动的来源
    ///  · 切向速度保留 → 拖动时沿边平滑滑动；静止时摩擦会让它彻底停下
    /// 判定用 r + GROUND_SNAP_MARGIN 的探测半径：只有"几乎贴地"的节点参与，悬空节点完全不受影响。
    /// </summary>
    private void SnapGroundedNodes()
    {
        if (!USE_GROUND_WALK)
        {
            return;
        }
        int count = this.nodes.Count;
        if (count < 3 || this.room == null)
        {
            return;
        }
        float probeRad = RopeCollision.DEFAULT_RADIUS + GROUND_SNAP_MARGIN;
        for (int i = 1; i < count - 1; i++)
        {
            Node n = this.nodes[i];
            n.groundStrength = 0f;
            if (n.invMass <= 0f)
            {
                continue;
            }
            Vector2 normal;
            float depth;
            if (!RopeCollision.Probe(this.room, n.pos, probeRad, n.lastGroundNormal, out normal, out depth))
            {
                continue;                       // 悬空：交给 Verlet
            }
            n.lastGroundNormal = normal;        // 记住本子步法线，供下一子步滞回
            // 贴地强度：法向向外速度越大，吸附/摩擦越弱（连续过渡）。
            // 不用硬阈值——硬阈值会让节点每子步在"吸附/不吸附"之间来回切换，表现为剧烈震荡。
            float outward = Vector2.Dot(n.vel, normal);
            float strength = Mathf.Clamp01(1f - outward / GROUND_RELEASE_SPEED);
            if (strength <= 0.001f)
            {
                continue;                       // 明确离地：不吸附、不摩擦
            }
            n.groundStrength = strength;
            float push = (depth - GROUND_SNAP_MARGIN) * strength;
            if (push <= 0.001f)
            {
                continue;
            }
            Vector2 delta = normal * push;
            n.pos += delta;
            n.lastPos += delta;                 // 同步：法向速度归零，只剩切向
        }
    }

    /// <summary>
    /// 找贴地段起点（从 A 端起扫描）：第一个 groundStrength &gt; GROUND_SHRINK_EXCLUDE 的节点。
    /// index 0 本身贴地时起点即 0；没有贴地段返回 -1。
    /// </summary>
    private int FindGroundStart()
    {
        int count = this.nodes.Count;
        for (int i = 0; i < count; i++)
        {
            if (this.nodes[i].groundStrength > GROUND_SHRINK_EXCLUDE)
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// 沿地形表面走 dist 距离：从 from 出发，每步沿切向前进（步长均匀），并用 Probe 把点投影回"距表面 r"处。
    /// 切向由调用方给出（起点法线的垂线），每步用新法线修正但保持同向，避免拐角处来回翻。
    /// </summary>
    private Vector2 WalkOnSurface(Vector2 from, float dist, Vector2 tangentHint)
    {
        Vector2 p = from;
        Vector2 tangent = tangentHint;
        if (tangent.sqrMagnitude < 1E-06f || dist <= 0.01f)
        {
            return p;
        }
        tangent.Normalize();
        int steps = Mathf.Clamp(Mathf.CeilToInt(dist), 1, MAX_WALK_STEPS);
        float stepLen = dist / steps;
        for (int s = 0; s < steps; s++)
        {
            Vector2 next = p + tangent * stepLen;
            Vector2 normal;
            float depth;
            if (RopeCollision.Probe(this.room, next, RopeCollision.DEFAULT_RADIUS, out normal, out depth))
            {
                next += normal * depth;                     // 投影回"距表面 r"处
                Vector2 t2 = new Vector2(-normal.y, normal.x);
                if (Vector2.Dot(t2, tangent) < 0f)
                {
                    t2 = -t2;
                }
                tangent = t2;                              // 拐角处切向随法线平滑转动
            }
            p = next;
        }
        return p;
    }

    /// <summary>
    /// 沿表面排布（仅在张力时）：以贴地段起点为锚，第 k 个贴地节点朝"沿表面走 k×SegLen"的位置靠拢。
    /// 位置级、单子步 ≤ GROUND_WALK_STEP，同步 lastPos；松弛时不介入（hasTension 为假直接返回）。
    /// </summary>
    private void UpdateGroundWalk()
    {
        if (!USE_GROUND_WALK || !this.hasTension || this.room == null)
        {
            return;
        }
        int start = this.groundStartIndex;
        if (start < 0 || start >= this.nodes.Count - 1)
        {
            return;
        }
        Vector2 anchor = this.nodes[start].pos;
        Vector2 normal;
        float depth;
        if (!RopeCollision.Probe(this.room, anchor, RopeCollision.DEFAULT_RADIUS + GROUND_SNAP_MARGIN, out normal, out depth))
        {
            return;                                     // 起点拿不到贴地法线：不排布
        }
        // 切向在起点定一次：法线的垂线，方向朝"远离 A 端"一侧
        Vector2 tangent = new Vector2(-normal.y, normal.x);
        Vector2 rough = this.nodes[Mathf.Min(start + 1, this.nodes.Count - 1)].pos - anchor;
        if (Vector2.Dot(tangent, rough) < 0f)
        {
            tangent = -tangent;
        }
        int k = 0;
        for (int i = start + 1; i < this.nodes.Count - 1; i++)
        {
            Node n = this.nodes[i];
            if (n.groundStrength <= GROUND_SHRINK_EXCLUDE)
            {
                break;                                  // 贴地段结束
            }
            k++;
            Vector2 want = this.WalkOnSurface(anchor, k * this.SegLen, tangent);
            Vector2 delta = want - n.pos;
            float d = delta.magnitude;
            if (d <= 0.001f)
            {
                continue;
            }
            this.debugGroundErr += d;
            Vector2 step = delta / d * Mathf.Min(d, GROUND_WALK_STEP);
            n.pos += step;
            n.lastPos += step;                          // 同步：不注入速度
            this.debugGroundWalk += step.magnitude;
        }
    }

    /// <summary>
    /// 贴地节点的切向摩擦（模拟静摩擦）：只衰减 grounded 节点的速度，悬空节点不受影响。
    /// 没有这一层，躺在斜面上的绳子会被重力一直往下拽、永远微微滑动。
    /// </summary>
    private void DampGroundedVelocity()
    {
        if (!USE_GROUND_WALK)
        {
            return;
        }
        for (int i = 0; i < this.nodes.Count; i++)
        {
            Node n = this.nodes[i];
            float s = n.groundStrength;
            if (s <= 0.001f)
            {
                continue;
            }
            n.vel *= Mathf.Lerp(1f, GROUND_FRICTION, s);
            if (n.vel.sqrMagnitude < 0.0004f)
            {
                n.vel = Vector2.zero;
            }
        }
    }

    /// <summary>
    /// 段级地形检测：Sweep 只检查单个节点。一段若被拉长到超过地形厚度（两端节点都在墙外），
    /// 整段会横穿墙体而没有任何节点进入实体，单点碰撞永远发现不了。
    /// 这里对过长的段做射线检测，二分找出"刚好不穿墙"的长度，把远端节点缩回来。
    /// </summary>
    private void EnsureSegmentsClear()
    {
        int count = this.nodes.Count;
        if (count < 2 || this.room == null)
        {
            return;
        }
        float minLength = Mathf.Max(this.SegLen * SEGMENT_CHECK_FACTOR, SEGMENT_CHECK_MIN);
        for (int i = 0; i < count - 1; i++)
        {
            Vector2 a = this.nodes[i].pos;
            Vector2 b = this.nodes[i + 1].pos;
            float dist = Vector2.Distance(a, b);
            if (dist <= minLength || !this.LineBlocked(a, b))
            {
                continue;
            }
            Vector2 dir = (b - a) / dist;
            float lo = 0f;
            float hi = dist;
            for (int it = 0; it < SEGMENT_BISECT_STEPS; it++)
            {
                float mid = (lo + hi) * 0.5f;
                if (this.LineBlocked(a, a + dir * mid))
                {
                    hi = mid;
                }
                else
                {
                    lo = mid;
                }
            }
            Vector2 shrunkPos = a + dir * Mathf.Max(lo, dist - SEGMENT_SHRINK_LIMIT);
            Node far = this.nodes[i + 1];
            Vector2 target = shrunkPos;
            Vector2 histDir = far.LastDirection();
            if (histDir.sqrMagnitude > 0.1f && Vector2.Dot(dir, histDir) < 0f)
            {
                // 段方向与本节点上帧运动方向相反：它正被"拽回"，优先退回自己的历史轨迹
                target = far.RecentPos(2);
            }
            target = this.LimitJump(i + 1, target);
            Vector2 delta = target - far.pos;
            far.pos = target;
            far.lastPos += delta;   // 同步平移：段收缩不再产生反向速度
            far.vel = Vector2.zero;
            this.debugSegmentsShrunk++;
        }
    }

    /// <summary>
    /// 拓扑保护：节点顺序被打乱时（下一段方向与本节点上帧运动方向夹角 > 120°），
    /// 说明它被强行拽到了绳子另一侧——这正是"整根绳子塌成两绳结直线"的来源。
    /// 这里把该节点回退到最近几帧的平均位置，保住"绕平台"的绕行拓扑。
    /// </summary>
    private void ProtectTopology()
    {
        int count = this.nodes.Count;
        if (count < 3 || this.room == null)
        {
            return;
        }
        for (int i = 1; i < count - 1; i++)
        {
            Node n = this.nodes[i];
            if (n.invMass <= 0f)
            {
                continue;
            }
            // 只在贴地时启用：空中自由摆动时"下一段方向 vs 上帧运动方向"本来就会反向，
            // 继续回退到两帧前会把绳子往过去拽（表现为甩不开、一顿一顿）
            if (n.groundStrength <= 0.001f)
            {
                continue;
            }
            Vector2 hist = n.LastDirection();
            if (hist.sqrMagnitude < 0.1f)
            {
                continue;
            }
            Vector2 segDir = this.nodes[i + 1].pos - n.pos;
            if (segDir.sqrMagnitude < 1E-06f)
            {
                continue;
            }
            if (Vector2.Dot(segDir.normalized, hist) >= TOPO_ANGLE_COS)
            {
                continue;
            }
            Vector2 target = this.LimitJump(i, n.RecentPos(2));
            Vector2 delta = target - n.pos;
            if (delta.sqrMagnitude <= 1E-10f)
            {
                continue;
            }
            n.pos = target;
            n.lastPos += delta;
            this.debugTopoBreaks++;
        }
    }

    /// <summary>子步收尾：用位置变化量回推速度（XPBD 的标准做法，天然消除长度漂移）。</summary>
    private void UpdateVelocitiesFromPositions(float h)
    {
        for (int i = 0; i < this.nodes.Count; i++)
        {
            Node n = this.nodes[i];
            if (n.invMass <= 0f)
            {
                n.vel = Vector2.zero;
                continue;
            }
            n.vel = (n.pos - n.lastPos) / h * DAMPING * AIR_DAMPING;
            if (n.vel.sqrMagnitude > MAX_NODE_SPEED * MAX_NODE_SPEED)
            {
                n.vel = n.vel.normalized * MAX_NODE_SPEED;   // 硬上限：任何异常都不允许把整根绳子甩飞
            }
            if (n.vel.sqrMagnitude < 0.0004f)
            {
                n.vel = Vector2.zero;      // 微小残余速度直接归零，绳子才能真正停下来
            }
        }
    }

    /// <summary>
    /// 去反弹：速度回推之后，把速度里"朝地形内部"的分量清零（只保留切向分量）。
    /// 否则撞墙速度会被原样保留，下一子步又撞出去，表现为弹飞与抖动。
    /// 法线由 Probe 给出（指向空气侧），朝内分量 = -Dot(vel, normal)。
    /// </summary>
    private void RemoveInwardVelocity()
    {
        int count = this.nodes.Count;
        if (count < 3 || this.room == null)
        {
            return;
        }
        for (int i = 1; i < count - 1; i++)
        {
            Node n = this.nodes[i];
            if (n.invMass <= 0f)
            {
                continue;
            }
            Vector2 normal;
            float depth;
            if (!RopeCollision.Probe(this.room, n.pos, RopeCollision.DEFAULT_RADIUS, out normal, out depth))
            {
                continue;
            }
            float inward = -Vector2.Dot(n.vel, normal);
            if (inward > 0f)
            {
                n.vel += normal * inward;
            }
        }
    }

    /// <summary>
    /// 跳变抑制：把一次位置修正限制在 NODE_JUMP_LIMIT 以内。
    /// 若修正方向与本节点上一帧的运动方向相反（典型的"被拽回/瞬移"），
    /// 就改成沿历史方向移动，避免节点横跨地形跳到几何上更省事的位置；
    /// 剩余量会在后续子步里继续消化（每个子步都会重新计算）。
    /// </summary>
    private Vector2 LimitJump(int index, Vector2 target)
    {
        Node n = this.nodes[index];
        Vector2 delta = target - n.pos;
        float dist = delta.magnitude;
        if (dist <= NODE_JUMP_LIMIT)
        {
            return target;
        }
        Vector2 dir = delta / dist;
        Vector2 histDir = n.LastDirection();
        if (histDir.sqrMagnitude > 0.1f && Vector2.Dot(dir, histDir) < 0f)
        {
            dir = histDir;      // 往回跳：改沿原路小步走
        }
        return n.pos + dir * NODE_JUMP_LIMIT;
    }

    /// <summary>找到"跨过该点"的相邻段：到线段距离最小的 i（段 i..i+1）。找不到返回 -1。</summary>
    private int FindSegmentAcross(Vector2 point)
    {
        int bestI = -1;
        float bestD = float.MaxValue;
        for (int i = 0; i < this.nodes.Count - 1; i++)
        {
            Vector2 a = this.nodes[i].pos;
            Vector2 ab = this.nodes[i + 1].pos - a;
            float len2 = ab.sqrMagnitude;
            float t = len2 < 1E-06f ? 0f : Mathf.Clamp01(Vector2.Dot(point - a, ab) / len2);
            float d = Vector2.Distance(point, a + ab * t);
            if (d < bestD)
            {
                bestD = d;
                bestI = i;
            }
        }
        return bestI;
    }

    /// <summary>
    /// 相邻两节点之间若被实体地形穿过，就在碰撞点前插入一个节点（贴在地形外侧）。
    /// 插入后 SegLen 变小、总长上限不变，因此绳子不会被"撑长"。
    /// </summary>
    private void InsertNodesForTerrainTunneling()
    {
        if (this.nodes.Count >= MAX_NODES)
        {
            return;
        }
        // 每帧最多补一个节点：一次插多个会让形状突变（配合 total 长度守恒，表现为抽动）
        const int maxInsertPerFrame = 1;
        int inserted = 0;
        for (int i = 0; i < this.nodes.Count - 1; i++)
        {
            Vector2 a = this.nodes[i].pos;
            Vector2 b = this.nodes[i + 1].pos;
            if (Vector2.Distance(a, b) < TUNNEL_STEP)
            {
                continue;
            }
            // 段已经切得很短就不再切：每次插入都会把 limit 对半，继续切会让段长远小于 NODE_RADIUS，
            // 绳身变成高频抖动的短链（表现为"一直在扭"）。
            if (this.nodes[i].limit < NODE_RADIUS * 3f)
            {
                continue;
            }
            if (!this.SegmentHitsTerrain(a, b))
            {
                continue;
            }
            if (!this.FindTerrainHit(a, b, out Vector2 hit))
            {
                continue;
            }

            Vector2 dir = (b - a).normalized;
            // 插入点再往空气一侧退一点，确保新节点本身不在实体里
            Vector2 insertPos = hit - dir * (NODE_RADIUS + 2f);
            if (this.room.GetTile(insertPos).IsSolid())
            {
                insertPos = a;
                if (this.room.GetTile(insertPos).IsSolid())
                {
                    continue;
                }
            }
            Node insert = new Node(insertPos);
            insert.vel = Vector2.Lerp(this.nodes[i].vel, this.nodes[i + 1].vel, 0.5f);
            insert.lastPos = insert.pos;
            insert.pinned = true;                // 拐角吸附点：不参与物理，只贴合地形
            insert.invMass = 0f;
            // 把被切开的那一段的长度上限一分为二，总长保持不变
            float split = this.nodes[i].limit * 0.5f;
            this.nodes[i].limit = split;
            insert.limit = split;
            this.nodes.Insert(i + 1, insert);
            inserted++;
            if (this.nodes.Count >= MAX_NODES || inserted >= maxInsertPerFrame)
            {
                return;
            }
            i++;
        }
    }

    /// <summary>
    /// 维护拐角吸附点：不再需要时解除吸附；仍需要时投影到"两侧邻居中点附近、但不在实体里"的位置。
    /// 它们不参与积分/地形碰撞，因此不会在墙角来回弹（拐角抖动的另一半原因）。
    /// </summary>
    private void UpdatePinnedNodes()
    {
        for (int i = 1; i < this.nodes.Count - 1; i++)
        {
            Node n = this.nodes[i];
            if (!n.pinned)
            {
                continue;
            }
            Vector2 mid = (this.nodes[i - 1].pos + this.nodes[i + 1].pos) * 0.5f;
            if (!this.SegmentHitsTerrain(this.nodes[i - 1].pos, mid) && !this.SegmentHitsTerrain(mid, this.nodes[i + 1].pos))
            {
                n.pinned = false;                // 绕开了，交回普通物理
                n.invMass = 1f / NODE_MASS;
                continue;
            }
            Vector2 target = mid;
            if (this.room.GetTile(target).IsSolid())
            {
                target = this.FindAirPointToward(this.nodes[i - 1].pos, mid);
            }
            n.pos = target;
            n.lastPos = target;
            n.vel = Vector2.zero;
        }
    }

    /// <summary>从 from 朝 to 走，返回最后一个仍处于空气的采样点。</summary>
    private Vector2 FindAirPointToward(Vector2 from, Vector2 to)
    {
        Vector2 lastAir = from;
        float dist = Vector2.Distance(from, to);
        int steps = Mathf.Max(1, Mathf.CeilToInt(dist / TUNNEL_STEP));
        for (int s = 1; s <= steps; s++)
        {
            Vector2 cur = Vector2.Lerp(from, to, (float)s / steps);
            if (this.room.GetTile(cur).IsSolid())
            {
                break;
            }
            lastAir = cur;
        }
        return lastAir;
    }

    /// <summary>
    /// 带半径的段—地形检测：中心线之外还检查两条偏移 NODE_RADIUS 的平行线。
    /// 只测中心线时，绳子贴墙擦过（半个身子在墙里）会被判为"没穿墙"。
    /// </summary>
    private bool SegmentHitsTerrain(Vector2 a, Vector2 b)
    {
        if (this.LineBlocked(a, b))
        {
            return true;
        }
        Vector2 perpendicular = Custom.PerpendicularVector((b - a).normalized) * NODE_RADIUS;
        if (this.room.GetTile(a + perpendicular).IsSolid() || this.room.GetTile(b + perpendicular).IsSolid())
        {
            return true;
        }
        if (this.room.GetTile(a - perpendicular).IsSolid() || this.room.GetTile(b - perpendicular).IsSolid())
        {
            return true;
        }
        return this.LineBlocked(a + perpendicular, b + perpendicular) || this.LineBlocked(a - perpendicular, b - perpendicular);
    }

    /// <summary>
    /// 原版语义容易搞反：SharedPhysics.RayTraceTilesForTerrain 返回 true 表示"一路畅通"，
    /// 返回 false 才是被地形挡住（源码里扫到 IsSolid 的格子就 return false）。
    /// </summary>
    private bool LineBlocked(Vector2 a, Vector2 b)
    {
        return !SharedPhysics.RayTraceTilesForTerrain(this.room, a, b);
    }

    /// <summary>沿线段步进找到第一个落在实体瓦片上的采样点。</summary>
    private bool FindTerrainHit(Vector2 a, Vector2 b, out Vector2 hit)
    {
        hit = a;
        float dist = Vector2.Distance(a, b);
        int steps = Mathf.Max(1, Mathf.CeilToInt(dist / TUNNEL_STEP));
        for (int s = 1; s <= steps; s++)
        {
            Vector2 cur = Vector2.Lerp(a, b, (float)s / steps);
            if (this.room.GetTile(cur).IsSolid())
            {
                hit = cur;
                return true;
            }
        }
        return false;
    }

    private void CollideNodesWithTerrain()
    {
        for (int i = 1; i < this.nodes.Count - 1; i++)
        {
            Node n = this.nodes[i];
            if (n.pinned)
            {
                continue;   // 拐角吸附点由 UpdatePinnedNodes 统一投影，不参与碰撞（否则会在墙角来回弹）
            }
            SharedPhysics.TerrainCollisionData data = new SharedPhysics.TerrainCollisionData(
                n.pos, n.lastPos, n.vel, NODE_RADIUS, new IntVector2(0, 0), true);
            data = SharedPhysics.VerticalCollision(this.room, data);
            data = SharedPhysics.SlopesVertically(this.room, data);
            data = SharedPhysics.HorizontalCollision(this.room, data);
            n.pos = data.pos;
            n.vel = data.vel;
            if (data.contactPoint.x != 0)
            {
                n.vel.y *= 0.6f;
            }
            if (data.contactPoint.y != 0)
            {
                n.vel.x *= 0.6f;
            }
            if (this.room.GetTile(n.pos).IsSolid())
            {
                this.PushOutOfSolid(n);
            }
        }
    }

    /// <summary>节点仍陷在实体瓦片里时，直接挪到附近最近的空气瓦片中心（兜底，防止卡进墙里）。</summary>
    private void PushOutOfSolid(Node n)
    {
        IntVector2 tile = this.room.GetTilePosition(n.pos);
        Vector2 best = n.pos;
        float bestDist = float.MaxValue;
        for (int dx = -2; dx <= 2; dx++)
        {
            for (int dy = -2; dy <= 2; dy++)
            {
                IntVector2 t = new IntVector2(tile.x + dx, tile.y + dy);
                if (this.room.GetTile(t).IsSolid())
                {
                    continue;
                }
                Vector2 center = this.room.MiddleOfTile(t);
                float d = Vector2.Distance(center, n.pos);
                if (d < bestDist)
                {
                    bestDist = d;
                    best = center;
                }
            }
        }
        if (bestDist < float.MaxValue)
        {
            // 不要瞬移到瓦片中心（那是一次十几单位的跳跃，会直接表现为抽动），每帧只推一小步
            Vector2 delta = best - n.pos;
            const float maxStep = NODE_RADIUS * 1.5f;
            if (delta.magnitude > maxStep)
            {
                delta = delta.normalized * maxStep;
            }
            n.pos += delta;
            n.vel = Vector2.zero;
        }
    }

    /// <summary>相邻两节点最小间距（只防止完全重合，远小于段长上限，因此不会与距离约束死锁）。</summary>
    private void SolvePushApart()
    {
        float minDist = this.SegLen * 0.3f;
        for (int i = 1; i < this.nodes.Count; i++)
        {
            Node a = this.nodes[i - 1];
            Node b = this.nodes[i];
            Vector2 d = b.pos - a.pos;
            float dist = d.magnitude;
            if (dist >= minDist)
            {
                continue;
            }
            if (dist < 1E-05f)
            {
                b.pos += new Vector2(0.01f, 0f);   // 完全重合时给个固定扰动，避免方向除零
                continue;
            }
            Vector2 n = d / dist;
            float push = (minDist - dist) * 0.05f;
            if (a.invMass > 0f)
            {
                a.pos -= n * push;
                a.lastPos -= n * push;   // 同步：推开不注入速度（否则挤压会让自由端上下弹跳）
            }
            if (b.invMass > 0f)
            {
                b.pos += n * push;
                b.lastPos += n * push;
            }
        }
    }

    /// <summary>
    /// 绳子的内摩擦：衰减相邻节点沿连线方向的"相对速度"（即拉伸/收缩振动），
    /// 但不动整体平动与摆动。缺了这层，绳身会像理想弹簧链一样一直微微摆动。
    /// </summary>
    private void SolveFriction()
    {
        for (int i = 0; i < this.nodes.Count - 1; i++)
        {
            Node a = this.nodes[i];
            Node b = this.nodes[i + 1];
            Vector2 d = b.pos - a.pos;
            float dist = d.magnitude;
            if (dist < 1E-05f)
            {
                continue;
            }
            Vector2 n = d / dist;
            float rel = Vector2.Dot(a.vel - b.vel, n);
            // rel > 0 表示两节点在互相靠近（压缩）：用更强的阻尼压住松弛后的上下弹跳
            float strength = rel > 0f ? COMPRESSION_FRICTION : INTERNAL_FRICTION;
            float k = rel * strength * 0.5f;
            a.vel -= n * k;
            b.vel += n * k;
        }
    }

    /// <summary>
    /// 不可拉长：只处理超过"该段自己的上限"的段（绳子不抗压，松弛时不做任何修正）。
    /// 用 A.limit 而不是"总长 / 段数"：插入/合并节点时上限会跟着分裂/合并，总长始终等于 maxLength，
    /// 不会因为撞地形插入节点导致整条绳突然被压缩（那正是拐角抖动的来源）。
    /// </summary>
    private void SolveDistanceConstraints()
    {
        int count = this.nodes.Count;
        for (int i = 0; i < count - 1; i++)
        {
            this.SolveOneSegment(i, false);
        }
    }

    /// <summary>单段约束：只修拉伸（绳子不抗压），单次修正受 MAX_CORRECTION 限幅。</summary>
    private void SolveOneSegment(int i, bool reverse)
    {
        Node A = this.nodes[i];
        Node B = this.nodes[i + 1];
        float l = this.SegLen;      // 节点固定后每段上限统一（= 绳长 / (节点数-1)）
        if (l <= 0f)
        {
            l = this.maxLength;
        }
        Vector2 d = B.pos - A.pos;
        float dist = d.magnitude;
        if (dist <= l || dist < 1E-05f)
        {
            return;
        }
        float invSum = A.invMass + B.invMass;
        if (invSum <= 0f)
        {
            return;
        }
        Vector2 n = d / dist;
        float corr = (dist - l) / invSum;
        // 单次迭代最多挪 4px：一次纠正几十像素会直接跳过整格地形（20px），
        // 碰撞扫描只检查 pos/lastPos 两端，跳过的那一格就"穿"过去了。
        float deltaA = corr * A.invMass;
        float deltaB = corr * B.invMass;
        float worst = Mathf.Max(deltaA, deltaB);
        if (worst > MAX_CORRECTION)
        {
            float scale = MAX_CORRECTION / worst;
            deltaA *= scale;
            deltaB *= scale;
        }
        A.pos += n * deltaA;
        B.pos -= n * deltaB;
    }

    /// <summary>
    /// 按质量共速：拉紧的段上，把沿连线方向的相对速度按质量加权抹平（完全非弹性）。
    /// 端点侧的质量来自绳结（玩家有按键时视为极重），并把冲量按质量比传回绳结，形成"拖拽感"。
    /// </summary>
    private void SolveVelocityConstraints()
    {
        float l = this.SegLen;
        for (int i = 0; i < this.nodes.Count - 1; i++)
        {
            Node A = this.nodes[i];
            Node B = this.nodes[i + 1];
            Vector2 d = B.pos - A.pos;
            float dist = d.magnitude;
            if (dist < 1E-05f || dist < l * 0.999f)
            {
                continue;   // 只有绷紧的段才做共速，松弛的段互不干涉
            }
            Vector2 n = d / dist;
            float vA = Vector2.Dot(A.vel, n);
            float vB = Vector2.Dot(B.vel, n);
            float rel = vB - vA;
            // rel > 0：段被拉长 → 不可拉长，共速。
            // rel <= 0：段被压缩；若已经到下限（dist ≈ l，没有压缩余量）也要共速，
            //           否则"收缩"灌进来的向内速度会一路累积，撞上距离约束后反弹成纵波。
            //           前置过滤已保证 dist >= l × 0.999，所以实际两种情形都处理。
            if (rel < 0f && dist < l * 0.999f)
            {
                continue;
            }

            bool fixedA = A.invMass <= 0f;
            bool fixedB = B.invMass <= 0f;
            float mA = fixedA ? this.EndpointMass(i == 0 ? this.stuckPosA : null) : NODE_MASS;
            float mB = fixedB ? this.EndpointMass(i + 1 == this.nodes.Count - 1 ? this.stuckPosB : null) : NODE_MASS;
            float sum = mA + mB;
            if (sum <= 0f)
            {
                continue;
            }

            float vCommon = (mA * vA + mB * vB) / sum;
            if (fixedA)
            {
                this.ApplyEndpointReaction(this.stuckPosA, n, vCommon - vA, mA);
            }
            else
            {
                A.vel += n * (vCommon - vA);
            }
            if (fixedB)
            {
                this.ApplyEndpointReaction(this.stuckPosB, n, vCommon - vB, mB);
            }
            else
            {
                B.vel += n * (vCommon - vB);
            }
        }
    }

    /// <summary>
    /// 端点的有效质量：越小越容易被绳子拖动。
    /// 被生物抓着时取**抓取者的总质量**（所以绳子能拖走小生物、拖不动大生物）；
    /// 玩家按下方向键时再 ×PLAYER_INPUT_MASS_FACTOR（是玩家拽绳子，而不是绳子拽玩家）。
    /// </summary>
    private float EndpointMass(KnotAbstract knot)
    {
        return this.EndpointMass(knot, null);
    }

    /// <summary>
    /// 端点的有效质量：越小越容易被绳子拖动。
    ///  · 固定锚点（管道口）→ +∞：完全不可移动
    ///  · 被生物抓着 → 抓取者质量（玩家按键 ×PLAYER_INPUT_MASS_FACTOR）
    ///  · 自由绳结 → 自身质量
    /// </summary>
    private float EndpointMass(KnotAbstract knot, Vector2? fixedPos)
    {
        if (knot == null)
        {
            return fixedPos != null ? float.PositiveInfinity : NODE_MASS * 4f;
        }
        if (knot.realizedObject == null)
        {
            return NODE_MASS * 4f;
        }
        if (this.IsPinned(knot))
        {
            return float.PositiveInfinity;   // 被钉进地形（绳矛的矛插入墙/地面）：完全不可移动
        }
        if (Scientist.ScientistHooks.MouseDragHooks.IsDragging(knot.realizedObject.bodyChunks[0]))
        {
            return float.PositiveInfinity;   // MouseDrag 正拖着它：最高级主动端，绳子只能被它牵着走
        }
        if (knot.realizedObject.grabbedBy.Count > 0)
        {
            Creature grabber = knot.realizedObject.grabbedBy[0].grabber;
            if (grabber != null)
            {
                float mass = Mathf.Max(0.1f, grabber.TotalMass);
                if (grabber is Player player && player.input[0].AnyInput)
                {
                    mass *= PLAYER_INPUT_MASS_FACTOR;
                }
                return mass;
            }
        }
        return Mathf.Max(0.01f, knot.realizedObject.bodyChunks[0].mass);
    }

    /// <summary>
    /// 该端是否被"钉死"（完全不可移动）：本身是插进墙里的矛，或与插墙的矛相连（绳矛场景）。
    /// 这是为绳矛预留的规则：矛钉入地面/墙壁后，绑在矛上的绳端就不可被拖动。
    /// </summary>
    private bool IsPinned(KnotAbstract knot)
    {
        if (knot == null)
        {
            return false;
        }
        AbstractPhysicalObject apo = knot;
        if (apo is AbstractSpear spear && spear.stuckInWall)
        {
            return true;
        }
        List<AbstractPhysicalObject.AbstractObjectStick> sticks = knot.stuckObjects;
        if (sticks != null)
        {
            for (int i = 0; i < sticks.Count; i++)
            {
                AbstractPhysicalObject other = sticks[i].A == knot ? sticks[i].B : sticks[i].A;
                if (other is AbstractSpear otherSpear && otherSpear.stuckInWall)
                {
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// 该端能否沿 dir 方向滑动 dist 距离。
    ///  · 被钉死/固定锚点 → 不行；
    ///  · 目标点必须在空气里；
    ///  · 中途不能被地形挡住（原版 RayTraceTilesForTerrain 返回 true = 一路畅通）。
    /// dir 应取"沿绳方向"（端点 → index 相邻节点），而不是两端连线方向。
    /// </summary>
    private bool CanSlide(KnotAbstract knot, Vector2 from, Vector2 dir, float dist, out Vector2 target)
    {
        target = from;
        if (this.room == null || dist <= 0.01f || dir.sqrMagnitude < 1E-08f)
        {
            return false;
        }
        if (this.IsPinned(knot))
        {
            return false;
        }
        Vector2 to = from + dir.normalized * dist;
        if (this.room.GetTile(to).IsSolid())
        {
            return false;
        }
        if (!SharedPhysics.RayTraceTilesForTerrain(this.room, from, to))
        {
            return false;
        }
        target = to;
        return true;
    }

    /// <summary>把绳子产生的速度改变按质量比传回绳结（重物/玩家被拉得少）。</summary>
    private void ApplyEndpointReaction(KnotAbstract knot, Vector2 n, float dv, float endpointMass)
    {
        if (knot?.realizedObject == null)
        {
            return;
        }
        float k = NODE_MASS / (NODE_MASS + endpointMass);
        knot.realizedObject.bodyChunks[0].vel += n * (dv * k);
    }

    /// <summary>
    /// 总长硬约束：两端距离超过绳长时，按"有效质量"分配位移。
    ///  · 轻的一端被拖得多，重的一端几乎不动（绳子能拖走小生物、拖不动大生物）
    ///  · 锚点 / 被钉死的端质量为 ∞ → share = 0，完全不动
    ///  · 位移沿"绳身切向"（端点 → 相邻节点），绕过地形时会顺着绳子路径被拖
    ///  · 两端都动不了时，退化为"挡住主动端"（只消沿绳向外的速度分量，横向不动）
    /// </summary>
    /// <summary>
    /// 每帧判定一次"是否有主动端"：
    ///  · 一端质量为 ∞（锚点/钉死）→ 该端是主动端
    ///  · 两端都有限时，质量比 > SHRINK_ACTIVE_RATIO → 重的一端是主动端
    ///  · 否则视为"无主动端"（对称收缩）
    /// </summary>
    private void UpdateShrinkMode()
    {
        float mA = this.EndpointMass(this.stuckPosA, this.fixedEndPosA);
        float mB = this.EndpointMass(this.stuckPosB, this.fixedEndPosB);
        bool aInf = float.IsInfinity(mA);
        bool bInf = float.IsInfinity(mB);
        this.shrinkActiveIsA = false;
        this.shrinkActiveIsB = false;
        if (aInf && !bInf)
        {
            this.shrinkActiveIsA = true;
            return;
        }
        if (bInf && !aInf)
        {
            this.shrinkActiveIsB = true;
            return;
        }
        if (aInf || bInf)
        {
            return;                        // 两端都不可动：收缩无意义
        }
        if (mA > mB * SHRINK_ACTIVE_RATIO)
        {
            this.shrinkActiveIsA = true;
        }
        else if (mB > mA * SHRINK_ACTIVE_RATIO)
        {
            this.shrinkActiveIsB = true;
        }
    }

    /// <summary>
    /// 节点 i 的收缩权重：
    ///  · 有主动端：w_i = 2i/N（i 从主动端起算，i=0 权重 0，越靠近被动端越大）
    ///  · 无主动端：w_i = |i - (N-1)/2| · 2/N（两端最大、中间 0）
    /// </summary>
    private float ShrinkWeight(int i, int count)
    {
        float n = count - 1;
        if (n <= 0f)
        {
            return 0f;
        }
        if (this.shrinkActiveIsA)
        {
            return 2f * i / n;
        }
        if (this.shrinkActiveIsB)
        {
            return 2f * (n - i) / n;
        }
        return Mathf.Abs(i - n * 0.5f) * 2f / n;
    }

    /// <summary>节点 i 的收缩方向：指向"主动端方向"的相邻节点；对称模式下指向中点一侧。</summary>
    private Vector2 ShrinkDirection(int i, int count)
    {
        int target;
        if (this.shrinkActiveIsA)
        {
            target = i - 1;
        }
        else if (this.shrinkActiveIsB)
        {
            target = i + 1;
        }
        else
        {
            target = i < (count - 1) * 0.5f ? i - 1 : i + 1;
        }
        if (target < 0 || target >= count)
        {
            return Vector2.zero;
        }
        Vector2 d = this.nodes[target].pos - this.nodes[i].pos;
        return d.sqrMagnitude > 1E-08f ? d.normalized : Vector2.zero;
    }

    /// <summary>
    /// 分布式收缩：绳子超长时，让内部节点按权重向主动端方向靠拢。
    /// 单节点单子步最多 SHRINK_STEP_MAX，剩余量留给后续子步；端点不参与（仍由 TryShiftEnd 处理）；
    /// 位移同步 lastPos，不注入速度。
    /// </summary>
    /// <summary>
    /// 收缩（速度级优先）：
    ///  · 正常：给节点加一个朝主动端/中点的速度增量；
    ///  · 贴地：方向投影到地形切线（避免朝地面推进）；
    ///  · 已陷入地形：本子步回退为位置级——沿法线推出并同步 lastPos，不加速度。
    /// 由摩擦/空气阻尼收敛，不再硬推位置。
    /// </summary>
    private void ShrinkNodesTowardActive(float over)
    {
        int count = this.nodes.Count;
        if (count < 3 || over <= 0.01f)
        {
            return;
        }
        float sumW = 0f;
        for (int i = 1; i < count - 1; i++)
        {
            sumW += this.ShrinkWeight(i, count);
        }
        if (sumW <= 1E-04f)
        {
            return;
        }
        float applied = 0f;
        for (int i = 1; i < count - 1; i++)
        {
            float w = this.ShrinkWeight(i, count);
            if (w <= 0f)
            {
                continue;
            }
            // 余量闸门：该节点两侧的段若已经绷紧，再往里灌速度只会让它撞上距离约束再被弹回来，
            // 这正是"拉长时出现剧烈纵波"的能量来源。没有可收缩余量就这一步不加加速度。
            float slack = 0f;
            Vector2 cur = this.nodes[i].pos;
            if (i > 0)
            {
                slack += Mathf.Max(0f, this.SegLen - Vector2.Distance(this.nodes[i - 1].pos, cur));
            }
            if (i + 1 < count)
            {
                slack += Mathf.Max(0f, this.SegLen - Vector2.Distance(cur, this.nodes[i + 1].pos));
            }
            if (slack <= 0.01f)
            {
                continue;
            }
            float step = Mathf.Min(over * w / sumW, SHRINK_STEP_MAX);
            step = Mathf.Min(step, slack);
            if (step <= 0.001f)
            {
                continue;
            }
            Vector2 dir = this.ShrinkDirection(i, count);
            if (dir.sqrMagnitude < 1E-06f)
            {
                continue;
            }
            Node n = this.nodes[i];
            Vector2 normal;
            float depth;
            if (this.room != null &&
                RopeCollision.Probe(this.room, n.pos, RopeCollision.DEFAULT_RADIUS, n.lastGroundNormal, out normal, out depth))
            {
                if (depth > 0.01f)
                {
                    // 已陷入：位置级回退（沿法线推出并同步 lastPos），本子步不加速度
                    Vector2 push = normal * Mathf.Min(depth, SHRINK_STEP_MAX);
                    n.pos += push;
                    n.lastPos += push;
                    applied += push.magnitude;
                    continue;
                }
                // 贴地：方向投影到切线，避免朝地面推
                float along = Vector2.Dot(dir, normal);
                dir -= normal * along;
                if (dir.sqrMagnitude < 1E-06f)
                {
                    continue;
                }
                dir.Normalize();
            }
            n.vel += dir * (step * SHRINK_ACCEL);
            applied += step;
        }
        this.debugShrink += applied;
    }

    private void SolveTotalLengthConstraint()
    {
        Vector2 endA;
        Vector2 endB;
        BodyChunk chunkA;
        BodyChunk chunkB;
        if (!this.TryGetEnd(this.stuckPosA, this.fixedEndPosA, out endA, out chunkA))
        {
            return;
        }
        if (!this.TryGetEnd(this.stuckPosB, this.fixedEndPosB, out endB, out chunkB))
        {
            return;
        }
        // 有效绳长 = 路径长度 × SHRINK_LENGTH_RATIO（弦长之和略小于真实弧长，留 4% 余量）。
        // 用路径长度而不是两端直线距离：绕行时两端距离会严重低估实际用掉的绳长。
        float pathLen = 0f;
        for (int i = 0; i < this.nodes.Count - 1; i++)
        {
            pathLen += Vector2.Distance(this.nodes[i].pos, this.nodes[i + 1].pos);
        }
        float max = this.effectiveMaxLength;
        float over = pathLen * SHRINK_LENGTH_RATIO - max;
        // 只留 0.5px 容差：绳子没有弹性，不允许"超过最大长度再被拉回"。
        if (over <= 0.5f || pathLen < 1E-04f)
        {
            return;
        }
        // 方向仍用两端连线（仅用于归一化）
        float dist = Vector2.Distance(endA, endB);
        if (dist < 1E-04f)
        {
            return;
        }

        float massA = this.EndpointMass(this.stuckPosA, this.fixedEndPosA);
        float massB = this.EndpointMass(this.stuckPosB, this.fixedEndPosB);
        bool aMovable = !float.IsInfinity(massA);
        bool bMovable = !float.IsInfinity(massB);

        if (!aMovable && !bMovable)
        {
            // 两端都钉死：绳子只能绷紧，挡住主动端（速度投影，横向不受影响）
            Vector2 nFixed = (endB - endA) / dist;
            this.ClampActiveEnd(this.stuckPosA, chunkA, nFixed, over);
            this.ClampActiveEnd(this.stuckPosB, chunkB, -nFixed, over);
            return;
        }

        // 质量分配：轻的一端分到更多位移
        float shareA = (aMovable && bMovable) ? massB / (massA + massB) : (aMovable ? 1f : 0f);
        float shareB = 1f - shareA;

        float movedA = aMovable ? this.TryShiftEnd(this.stuckPosA, chunkA, true, over * shareA) : 0f;
        float movedB = bMovable ? this.TryShiftEnd(this.stuckPosB, chunkB, false, over * shareB) : 0f;

        // 一端被地形挡住、没走完的部分转给另一端（只重分配一次，避免来回抖动）
        float remain = over - movedA - movedB;
        if (remain > 0.5f)
        {
            if (aMovable)
            {
                movedA += this.TryShiftEnd(this.stuckPosA, chunkA, true, remain);
            }
            else if (bMovable)
            {
                movedB += this.TryShiftEnd(this.stuckPosB, chunkB, false, remain);
            }
        }

        // 只有"两端完全动不了、且已明显超出允许的短暂超长"时才去挡主动端。
        // 允许短暂超长（maxLength × (1 + OVERLENGTH_TOLERANCE)），之后再靠距离约束慢慢收回；
        // 这样被动端哪怕只能挪一点也会挪，不会出现"主动端被硬卡住"。
        if (movedA + movedB <= 0.5f && over > max * OVERLENGTH_TOLERANCE)
        {
            Vector2 nBlocked = (endB - endA) / dist;
            this.ClampActiveEnd(this.stuckPosA, chunkA, nBlocked, over);
            this.ClampActiveEnd(this.stuckPosB, chunkB, -nBlocked, over);
        }
    }

    /// <summary>
    /// 把该端沿**绳身切向**移动 dist，返回实际移动距离。
    /// 目标点/路径被地形挡住时返回 0（由调用方重分配或去挡另一端）。
    /// 位移同时平移 lastPos，保持速度连续（不会产生回弹）。
    /// </summary>
    private float TryShiftEnd(KnotAbstract knot, BodyChunk chunk, bool isA, float dist)
    {
        if (chunk == null || dist <= 0.01f || knot?.realizedObject == null)
        {
            return 0f;
        }
        int count = this.nodes.Count;
        if (count < 2)
        {
            return 0f;
        }
        int index = isA ? 0 : count - 1;
        int inner = isA ? 1 : count - 2;
        Vector2 dir = this.nodes[inner].pos - this.nodes[index].pos;   // 沿绳方向
        if (dir.sqrMagnitude < 1E-06f)
        {
            return 0f;
        }
        dir.Normalize();
        float step = Mathf.Min(dist, ENDPOINT_SHIFT_LIMIT);
        Vector2 target;
        if (!this.CanSlide(knot, chunk.pos, dir, step, out target))
        {
            // 整段不可行：二分找最大可行位移，哪怕只有 1px 也返回。
            // 否则 remain 会全部压到另一端，表现为"被动端能动，主动端却被硬卡住"。
            float lo = 0f;
            float hi = step;
            for (int it = 0; it < ENDPOINT_BISECT_STEPS; it++)
            {
                float mid = (lo + hi) * 0.5f;
                Vector2 probe;
                if (this.CanSlide(knot, chunk.pos, dir, mid, out probe))
                {
                    lo = mid;
                }
                else
                {
                    hi = mid;
                }
            }
            if (lo < 1f || !this.CanSlide(knot, chunk.pos, dir, lo, out target))
            {
                return 0f;
            }
            step = lo;
        }
        Vector2 delta = target - chunk.pos;
        // 绳结被抓着时它的 pos 每帧会被抓取逻辑覆写，所以改抓取者
        BodyChunk mover = chunk;
        if (knot.realizedObject.grabbedBy.Count > 0)
        {
            Creature grabber = knot.realizedObject.grabbedBy[0].grabber;
            if (grabber != null)
            {
                mover = grabber.mainBodyChunk;
            }
        }
        Vector2 newPos = mover.pos + delta;
        if (this.room.GetTile(newPos).IsSolid())
        {
            return 0f;
        }
        mover.pos = newPos;
        mover.lastPos += delta;
        return delta.magnitude;
    }

    /// <summary>
    /// 主动端的刚性约束：绳子只能承受"沿绳方向"的拉力，所以这里只做两件事——
    ///  1. 消掉"继续远离另一端"的速度分量（沿绳方向），切向/横向速度完全保留；
    ///  2. 极小的位置回退（≤1px）防止一帧内累积越界。
    /// 这样人只能沿着"以另一端为球心、半径 = 绳长"的球面绕行，而不是被硬拉回边界——
    /// 后者会把位移的横向分量也一起施加，表现为"非沿绳方向也跟着同步动"。
    /// </summary>
    private void ClampActiveEnd(KnotAbstract knot, BodyChunk chunk, Vector2 towardOther, float over)
    {
        if (chunk == null || towardOther.sqrMagnitude < 1E-08f)
        {
            return;
        }
        BodyChunk target = chunk;
        if (knot?.realizedObject != null && knot.realizedObject.grabbedBy.Count > 0)
        {
            Creature grabber = knot.realizedObject.grabbedBy[0].grabber;
            if (grabber != null)
            {
                target = grabber.mainBodyChunk;
            }
        }
        // 1) 只抹掉朝外（远离另一端）的速度分量
        float vAway = Vector2.Dot(target.vel, -towardOther);
        if (vAway > 0f)
        {
            target.vel += towardOther * vAway;
        }
        // 2) 极小回退，且目标点若在实体里就不回退（不把玩家/绳结塞进墙）
        Vector2 back = towardOther * Mathf.Min(over, 1f);
        Vector2 newPos = target.pos + back;
        if (!this.room.GetTile(newPos).IsSolid())
        {
            target.pos = newPos;
            target.lastPos += back;
        }
    }

    /// <summary>
    /// 位置级夹紧（不是速度冲量）：用于"被动端"——它没有输入，只能被绳子拖动。
    /// 同时平移 lastPos，保持速度连续；目标点若落在实体里，沿空气路径退回到墙外。
    /// 返回实际移动的距离，供调用方判断"被动端是不是被卡住了"。
    /// </summary>
    private float ClampEnd(KnotAbstract knot, BodyChunk chunk, Vector2 delta)
    {
        if (chunk == null || delta.sqrMagnitude < 1E-08f)
        {
            return 0f;
        }
        BodyChunk target = chunk;
        if (knot?.realizedObject != null && knot.realizedObject.grabbedBy.Count > 0)
        {
            Creature grabber = knot.realizedObject.grabbedBy[0].grabber;
            if (grabber != null)
            {
                target = grabber.mainBodyChunk;
            }
        }
        Vector2 newPos = target.pos + delta;
        if (this.room.GetTile(newPos).IsSolid())
        {
            Vector2 air = this.FindAirPointToward(target.pos, newPos);
            if ((air - target.pos).sqrMagnitude < 1E-08f)
            {
                return 0f;                  // 已经贴死在墙上，再夹只会穿模
            }
            delta = air - target.pos;
        }
        // 单次最多挪 6px：一次瞬移几十像素会让端点段横穿地形（节点追不上），
        // 也会让绳结直接跳过墙体落到另一侧。
        const float maxStep = 6f;
        if (delta.magnitude > maxStep)
        {
            delta = delta.normalized * maxStep;
        }
        Vector2 before = target.pos;
        target.pos += delta;
        target.lastPos += delta;
        return Vector2.Distance(target.pos, before);
    }

    /// <summary>端点节点只受绳结位置控制，不参与地形碰撞；这里把渲染用的端点投影回空气，避免视觉穿墙。</summary>
    private void ProjectEndNodesOutOfTerrain()
    {
        if (this.nodes.Count < 2)
        {
            return;
        }
        this.ProjectEndOut(this.nodes[0], this.nodes[1].pos);
        this.ProjectEndOut(this.nodes[this.nodes.Count - 1], this.nodes[this.nodes.Count - 2].pos);
    }

    private void ProjectEndOut(Node end, Vector2 innerPos)
    {
        if (!this.room.GetTile(end.pos).IsSolid())
        {
            return;
        }
        Vector2 air = this.FindAirPointToward(innerPos, end.pos);
        if ((air - end.pos).sqrMagnitude < 1E-08f)
        {
            return;
        }
        end.pos = air;
        end.lastPos = air;
    }

    /// <summary>把两端质量/钉死/可沿绳滑动状态写进 public 字段，供 rope_debug 读取。</summary>
    private void UpdateDebugInfo()
    {
        int count = this.nodes.Count;
        if (count < 2)
        {
            return;
        }
        this.debugPinnedA = this.stuckPosA != null && this.IsPinned(this.stuckPosA);
        this.debugPinnedB = this.stuckPosB != null && this.IsPinned(this.stuckPosB);
        this.debugMassA = this.EndpointMass(this.stuckPosA, this.fixedEndPosA);
        this.debugMassB = this.EndpointMass(this.stuckPosB, this.fixedEndPosB);
        Vector2 dirA = this.nodes[1].pos - this.nodes[0].pos;                  // 端点 A → 相邻节点（沿绳方向）
        Vector2 dirB = this.nodes[count - 2].pos - this.nodes[count - 1].pos;   // 端点 B → 相邻节点
        const float probe = 6f;
        Vector2 target;
        this.debugCanSlideA = this.stuckPosA != null && this.CanSlide(this.stuckPosA, this.nodes[0].pos, dirA, probe, out target);
        this.debugCanSlideB = this.stuckPosB != null && this.CanSlide(this.stuckPosB, this.nodes[count - 1].pos, dirB, probe, out target);
    }

    /// <summary>
    /// 事件驱动的路径推断：端点移动超过阈值、超过 30 帧、或还没有引导时重算一次；
    /// 全局限流保证每帧最多一条绳求解（A* 是 tile 级，多条绳同时算会掉帧）。
    /// </summary>
    private void TryUpdateGuide()
    {
        if (!GUIDE_ENABLED)
        {
            this.guideValid = null;      // 引导关闭：完全不参与物理
            return;
        }
        if (this.stuckPosA == null || this.stuckPosB == null || this.nodes.Count < 4 || this.room == null)
        {
            this.guideValid = null;
            return;
        }
        Vector2 endA = this.stuckPosA.CurrentPosition;
        Vector2 endB = this.stuckPosB.CurrentPosition;
        bool stale = this.guideValid == null
            || Time.frameCount - this.guideFrame > GUIDE_MAX_AGE
            || (endA - this.guideEndA).sqrMagnitude > GUIDE_RESAMPLE_MIN * GUIDE_RESAMPLE_MIN
            || (endB - this.guideEndB).sqrMagnitude > GUIDE_RESAMPLE_MIN * GUIDE_RESAMPLE_MIN;
        if (!stale || Time.frameCount <= nextGuideSolveFrame)
        {
            return;
        }
        nextGuideSolveFrame = Time.frameCount;
        if (RopePathSolver.TrySolve(this.room, endA, endB, this.guidePath, out this.guidePathLength) && this.guidePath.Count >= 2)
        {
            this.guideEndA = endA;
            this.guideEndB = endB;
            this.guideFrame = Time.frameCount;
            this.ResampleGuide();
        }
        else
        {
            this.guideValid = null;          // 求解失败（不同连通域等）→ 退回纯物理
        }
    }

    /// <summary>
    /// 把路径按弧长重采样到每个节点：从 A 端和 B 端各自向前覆盖，
    /// 中间"两端都够不到"的部分不设引导 —— 那里的绳子余量应该自由下垂。
    /// </summary>
    private void ResampleGuide()
    {
        int count = this.nodes.Count;
        if (this.guidePoints == null || this.guidePoints.Length != count)
        {
            this.guidePoints = new Vector2[count];
            this.guideTangents = new Vector2[count];
            this.guideValid = new bool[count];
        }
        float total = this.effectiveMaxLength;
        // 路径比绳子还长 → 绳子其实已经是"绷直"状态，强行把节点铺到整条路径上会超过段长上限，
        // 与距离约束互相对抗（表现为绳子被顶成拱形、抖动）。这种情况直接不引导。
        if (this.guidePathLength > total * 0.95f)
        {
            this.guideValid = null;
            return;
        }
        float step = count > 1 ? total / (count - 1) : 0f;
        for (int i = 0; i < count; i++)
        {
            float fromA = i * step;
            float fromB = total - fromA;
            Vector2 point;
            Vector2 tangent;
            bool ok;
            if (fromA <= this.guidePathLength)
            {
                ok = this.SamplePath(fromA, out point, out tangent);
            }
            else if (fromB <= this.guidePathLength)
            {
                ok = this.SamplePath(this.guidePathLength - fromB, out point, out tangent);
                tangent = -tangent;          // B 侧：切线朝 A 端
            }
            else
            {
                point = this.nodes[i].pos;
                tangent = Vector2.right;
                ok = false;
            }
            this.guideValid[i] = ok;
            this.guidePoints[i] = point;
            this.guideTangents[i] = tangent;
        }
    }

    /// <summary>沿路径取"距 A 端 distanceFromA"处的点与切线。</summary>
    private bool SamplePath(float distanceFromA, out Vector2 point, out Vector2 tangent)
    {
        point = Vector2.zero;
        tangent = Vector2.right;
        float total = 0f;
        for (int i = 1; i < this.guidePath.Count; i++)
        {
            Vector2 a = this.guidePath[i - 1];
            Vector2 b = this.guidePath[i];
            float seg = Vector2.Distance(a, b);
            if (seg <= 1E-05f)
            {
                continue;
            }
            if (total + seg >= distanceFromA)
            {
                float t = Mathf.Clamp01((distanceFromA - total) / seg);
                point = Vector2.Lerp(a, b, t);
                tangent = (b - a) / seg;
                return true;
            }
            total += seg;
        }
        return false;
    }

    /// <summary>
    /// 软引导：把节点往引导路径上拉，但只修正"垂直于切线"的分量。
    /// 沿切线方向完全不动——否则会改变段长、压制下垂与摆动。
    /// 端点（绳结）不参与，由绳结位置决定。
    /// </summary>
    private void SolveGuideConstraints()
    {
        if (!GUIDE_ENABLED)
        {
            return;                      // 引导关闭：不参与任何约束
        }
        Vector2[] points = this.guidePoints;
        Vector2[] tangents = this.guideTangents;
        bool[] valid = this.guideValid;
        int count = this.nodes.Count;
        if (points == null || tangents == null || valid == null || points.Length != count)
        {
            return;
        }
        for (int i = 1; i < count - 1; i++)
        {
            if (!valid[i])
            {
                continue;
            }
            Node n = this.nodes[i];
            if (n.invMass <= 0f)
            {
                continue;
            }
            // 只在节点贴墙时才引导：远离地形时绳子应该只受重力与段约束，
            // 否则会被"通道中心的路径"硬拉成拱形（就是之前那种诡异的悬空弧线）。
            if (this.room.aimap != null && this.room.aimap.getTerrainProximity(n.pos) > 2)
            {
                continue;
            }
            Vector2 delta = points[i] - n.pos;
            Vector2 tangent = tangents[i];
            Vector2 perpendicular = delta - tangent * Vector2.Dot(delta, tangent);
            n.pos += perpendicular * GUIDE_WEIGHT;
        }
    }

    /// <summary>
    /// 把被动端沿绳身切向拉近，返回实际移动的距离（用于判断它是否被卡住）。
    /// 方向取"端点 → 相邻内部节点"，即绳子实际伸出去的方向：
    /// 绳子绕过障碍时，被动端会顺着这条路径被拖，而不是朝另一端直线冲（那会直接穿地形）。
    /// </summary>
    private float PullPassiveEnd(bool endIsB, float over)
    {
        int count = this.nodes.Count;
        if (count < 2)
        {
            return 0f;
        }
        int index = endIsB ? count - 1 : 0;
        int inner = endIsB ? count - 2 : 1;
        KnotAbstract knot = endIsB ? this.stuckPosB : this.stuckPosA;
        if (knot?.realizedObject == null)
        {
            return 0f;
        }
        Vector2 dir = this.nodes[inner].pos - this.nodes[index].pos;
        if (dir.sqrMagnitude < 1E-06f)
        {
            return 0f;
        }
        dir.Normalize();
        BodyChunk chunk = knot.realizedObject.bodyChunks[0];
        // 位置级夹紧（无速度冲量）：被拖的一端必须当场停在绳长边界上，不能"先拉长再回弹"。
        return this.ClampEnd(knot, chunk, dir * over);
    }

    private bool TryGetEnd(KnotAbstract knot, Vector2? fixedPos, out Vector2 pos, out BodyChunk chunk)
    {
        chunk = null;
        if (knot?.realizedObject != null)
        {
            chunk = knot.realizedObject.bodyChunks[0];
            pos = chunk.pos;
            return true;
        }
        if (fixedPos != null)
        {
            pos = fixedPos.Value;
            return true;
        }
        pos = Vector2.zero;
        return false;
    }

    /// <summary>把修正量作为速度冲量施加（若绳结被生物抓着则作用到抓取者），并限幅。</summary>
    private void ApplyEndpointImpulse(KnotAbstract knot, BodyChunk chunk, Vector2 delta)
    {
        BodyChunk target = chunk;
        if (knot?.realizedObject != null && knot.realizedObject.grabbedBy.Count > 0)
        {
            Creature grabber = knot.realizedObject.grabbedBy[0].grabber;
            if (grabber != null)
            {
                target = grabber.mainBodyChunk;
            }
        }
        const float max = 6f;
        if (delta.magnitude > max)
        {
            delta = delta.normalized * max;
        }
        target.vel += delta;
    }

    /// <summary>节点过密时合并（每帧只合并一处），避免节点数只增不减；合并后若会穿墙则不合并。</summary>
    private void MergeRedundantNodes()
    {
        int floor = Mathf.Max(4, this.initialNodeCount / 2);
        if (this.nodes.Count <= floor)
        {
            return;
        }
        for (int i = 1; i < this.nodes.Count - 2; i++)
        {
            Node a = this.nodes[i];
            Node b = this.nodes[i + 1];
            if (a.pinned || b.pinned)
            {
                continue;                                  // 拐角吸附点不合并，否则和插入逻辑打架
            }
            float threshold = Mathf.Max(0.3f, a.limit * 0.35f);
            if (Vector2.Distance(a.pos, b.pos) >= threshold)
            {
                continue;
            }
            Vector2 mid = (a.pos + b.pos) * 0.5f;
            if (this.room.GetTile(mid).IsSolid())
            {
                continue;                                  // 中点在地形里，合并会把绳子塞进墙
            }
            if (this.SegmentHitsTerrain(this.nodes[i - 1].pos, mid) || this.SegmentHitsTerrain(mid, this.nodes[i + 2].pos))
            {
                continue;                                  // 合并后会穿墙
            }
            a.pos = mid;
            a.lastPos = mid;
            a.limit += b.limit;                            // 长度上限合并，总长不变
            this.nodes.RemoveAt(i + 1);
            return;
        }
    }

    public override void Destroy()
    {
        if (this.sharedKey != null)
        {
            SharedRope data;
            if (sharedRopes.TryGetValue(this.sharedKey, out data))
            {
                if (this.sharedSlot >= 0 && this.sharedSlot < data.needs.Length)
                {
                    data.needs[this.sharedSlot] = 0f;
                }
                int alive = 0;
                for (int i = 0; i < data.needs.Length; i++)
                {
                    if (data.needs[i] > 0f) { alive++; }
                }
                if (alive == 0)
                {
                    sharedRopes.Remove(this.sharedKey);
                }
            }
            this.sharedKey = null;
        }
        try
        {
            if (this.stuckPosA != null && this.stuckPosA.ss != null && !this.stuckPosA.ss.HasItemBesides(this))
            {
                Room aRoom = this.stuckPosA.Room?.realizedRoom;
                aRoom?.RemoveObject(this.stuckPosA.realizedObject);
                this.room.abstractRoom.RemoveEntity(this.stuckPosA);
            }
            this.stuckPosA = null;
        }
        catch (Exception) { }
        try
        {
            if (this.stuckPosB != null && this.stuckPosB.ss != null && !this.stuckPosB.ss.HasItemBesides(this))
            {
                Room bRoom = this.stuckPosB.Room?.realizedRoom;
                bRoom?.RemoveObject(this.stuckPosB.realizedObject);
                this.room.abstractRoom.RemoveEntity(this.stuckPosB);
            }
            this.stuckPosB = null;
        }
        catch (Exception) { }
        try
        {
            this.room?.RemoveObject(this);
        }
        catch (Exception) { }
        base.Destroy();
    }

    public void ChangeRooms(WorldCoordinate newCoord, EntityID knotBase)
    {
        if (this.afterChangeRoomTimer > 0)
        {
            return;
        }
        Room thisRoom = this.room;
        Room newRoom = this.room.world.GetAbstractRoom(newCoord)?.realizedRoom;
        if (newRoom == null)
        {
            return;
        }
        thisRoom.RemoveObject(this);
        newRoom.AddObject(this);
        if (newRoom.climbableVines == null)
        {
            newRoom.climbableVines = new ClimbableVinesSystem();
            newRoom.AddObject(newRoom.climbableVines);
        }
        newRoom.climbableVines.vines.Add(this);
        this.afterChangeRoomTimer = 2;
        this.afterChangeRoomBase = knotBase;
        this.afterChangeRoomCoord = newCoord;
        if (this.stuckPosA != null && knotBase == this.stuckPosA.ID)
        {
            this.stuckPosB?.ChangeRooms(newCoord);
            thisRoom.RemoveObject(this.stuckPosB?.realizedObject);
            newRoom.AddObject(this.stuckPosB?.realizedObject);
        }
        else if (this.stuckPosB != null && knotBase == this.stuckPosB.ID)
        {
            this.stuckPosA?.ChangeRooms(newCoord);
            thisRoom.RemoveObject(this.stuckPosA?.realizedObject);
            newRoom.AddObject(this.stuckPosA?.realizedObject);
        }
    }

    public void ChangeKnot(KnotAbstract oldKnot, KnotAbstract newKnot)
    {
        if (this.stuckPosA != null && this.stuckPosA.ID == oldKnot.ID)
        {
            this.stuckPosA.ss.Remove(this);
            this.stuckPosA = newKnot;
            this.stuckPosA.ss.Add(this);
        }
        if (this.stuckPosB != null && this.stuckPosB.ID == oldKnot.ID)
        {
            this.stuckPosB.ss.Remove(this);
            this.stuckPosB = newKnot;
            this.stuckPosB.ss.Add(this);
        }
    }

    public void SetHasplayer(bool hasPlayer)
    {
        if (Scientist.ScientistPlugin.dfsStringshortVisited.Contains(this.ID)) { return; }
        this.hasPlayer = hasPlayer;
        Scientist.ScientistPlugin.dfsStringshortVisited.Add(this.ID);
        if (this.stuckPosA?.ss != null)
        {
            for (int i = 0; i < this.stuckPosA.ss.Count; i++)
            {
                this.stuckPosA.ss[i].SetHasplayer(hasPlayer);
            }
        }
        if (this.stuckPosB?.ss != null)
        {
            for (int j = 0; j < this.stuckPosB.ss.Count; j++)
            {
                this.stuckPosB.ss[j].SetHasplayer(hasPlayer);
            }
        }
    }

    public void SetPositionExitedShortcut(Vector2 pos)
    {
        if (Scientist.ScientistPlugin.dfsSsAndKaVisited.Contains(this.ID)) { return; }
        this.HardSetPositions(pos);
        Scientist.ScientistPlugin.dfsSsAndKaVisited.Add(this.ID);
        if (this.stuckPosA?.ss != null)
        {
            for (int i = 0; i < this.stuckPosA.ss.Count; i++)
            {
                this.stuckPosA.ss[i].SetPositionExitedShortcut(pos);
            }
        }
        if (this.stuckPosB?.ss != null)
        {
            for (int j = 0; j < this.stuckPosB.ss.Count; j++)
            {
                this.stuckPosB.ss[j].SetPositionExitedShortcut(pos);
            }
        }
    }

    private void HardSetPositions(Vector2 pos)
    {
        for (int i = 0; i < this.nodes.Count; i++)
        {
            this.nodes[i].pos = pos;
            this.nodes[i].lastPos = pos;
            this.nodes[i].vel = Vector2.zero;
        }
        if (this.stuckPosA?.realizedObject != null)
        {
            for (int i = 0; i < this.stuckPosA.realizedObject.bodyChunks.Length; i++)
            {
                this.stuckPosA.realizedObject.bodyChunks[i].HardSetPosition(pos);
            }
        }
        if (this.stuckPosB?.realizedObject != null)
        {
            for (int i = 0; i < this.stuckPosB.realizedObject.bodyChunks.Length; i++)
            {
                this.stuckPosB.realizedObject.bodyChunks[i].HardSetPosition(pos);
            }
        }
    }

    public float GravityAffected(int seg)
    {
        if (this.nodes.Count <= 6)
        {
            return 1f;
        }
        return Mathf.Min(
            Mathf.InverseLerp(2f, 5f, (float)seg),
            Mathf.InverseLerp((float)(this.nodes.Count - 3), (float)(this.nodes.Count - 6), (float)seg));
    }

    public Vector2 OnTubePos(float ps)
    {
        int count = this.nodes.Count;
        int num = Custom.IntClamp(Mathf.FloorToInt(ps * (count - 1)), 0, count - 1);
        int num2 = Custom.IntClamp(num + 1, 0, count - 1);
        float f = Mathf.InverseLerp((float)num, (float)num2, ps * (float)(count - 1));
        Vector2 cA = this.nodes[num].pos - (this.nodes[Custom.IntClamp(num - 1, 0, count - 1)].pos - this.nodes[num].pos).normalized * Vector2.Distance(this.nodes[num].pos, this.nodes[num2].pos) * 0.25f;
        Vector2 cB = this.nodes[num2].pos - (this.nodes[Custom.IntClamp(num2 + 1, 0, count - 1)].pos - this.nodes[num2].pos).normalized * Vector2.Distance(this.nodes[num].pos, this.nodes[num2].pos) * 0.25f;
        return Custom.Bezier(this.nodes[num].pos, cA, this.nodes[num2].pos, cB, f);
    }

    /// <summary>攀爬时给绳身一点横向推力，制造"挂上去绳子会晃"的反馈。</summary>
    public void BeingClimbedOn(Creature crit)
    {
        if (crit == null || this.nodes.Count < 3)
        {
            return;
        }
        Vector2 dir = Custom.DirVec(this.nodes[0].pos, this.nodes[this.nodes.Count - 1].pos);
        Vector2 perpendicular = Custom.PerpendicularVector(dir);
        float lean = Mathf.Sign(Vector2.Dot(crit.mainBodyChunk.pos - this.nodes[0].pos, perpendicular));
        for (int i = 1; i < this.nodes.Count - 1; i++)
        {
            this.nodes[i].vel += perpendicular * (lean * 0.15f);
        }
    }

    /// <summary>恒可攀爬；只有"正被发起查询的玩家自己拿在手里"时才不可爬（其它生物拿着不影响）。</summary>
    public bool CurrentlyClimbable()
    {
        if (climbQueryPlayer == null)
        {
            return true;
        }
        return !this.IsHeldBy(climbQueryPlayer);
    }

    private bool IsHeldBy(Creature crit)
    {
        return this.IsKnotHeldBy(this.stuckPosA, crit) || this.IsKnotHeldBy(this.stuckPosB, crit);
    }

    private bool IsKnotHeldBy(KnotAbstract knot, Creature crit)
    {
        PhysicalObject po = knot?.realizedObject;
        if (po?.grabbedBy == null)
        {
            return false;
        }
        for (int i = 0; i < po.grabbedBy.Count; i++)
        {
            if (po.grabbedBy[i].grabber == crit)
            {
                return true;
            }
        }
        return false;
    }

    public Vector2 Pos(int index) => this.nodes[Custom.IntClamp(index, 0, this.nodes.Count - 1)].pos;

    public int TotalPositions() => this.nodes.Count;

    public float Rad(int index) => NODE_RADIUS;

    public float Mass(int index) => NODE_MASS;

    public void Push(int index, Vector2 movement)
    {
        if (this.nodes.Count == 0)
        {
            return;
        }
        Node n = this.nodes[Custom.IntClamp(index, 0, this.nodes.Count - 1)];
        n.pos += movement;
        n.vel += movement;
    }

    public void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[1];
        TriangleMesh.Triangle[] tmt = new TriangleMesh.Triangle[MAX_NODES * 2 - 1];
        for (int i = 0; i < MAX_NODES * 2 - 1; i++)
        {
            tmt[i] = new TriangleMesh.Triangle(i, i + 1, i + 2);
        }
        this.tm = new TriangleMesh("pixel", tmt, false, true);
        sLeaser.sprites[0] = this.tm;
        this.AddToContainer(sLeaser, rCam, null);
    }

    public void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        int j = this.nodes.Count;
        if (this.tm == null || j <= 0)
        {
            return;
        }
        // 多余的顶点必须重合到"末节点"，不能移到远处：
        // TriangleMesh 的三角形是固定连号的，(2j-2, 2j-1, 2j) 这个三角形会把最后一节和第一个隐藏顶点连起来，
        // 移到 (-9999,-9999) 就会在画面上拉出一条贯穿屏幕的长线。
        Vector2 tail = this.nodes[j - 1].pos - camPos;
        for (int i = 0; i < j - 1; i++)
        {
            Vector2 perpendicular = (this.nodes[i].pos - this.nodes[i + 1].pos).VerticalNormalized();
            this.tm.MoveVertice(i * 2, this.nodes[i].pos - camPos + perpendicular * -NODE_RADIUS);
            this.tm.MoveVertice(i * 2 + 1, this.nodes[i].pos - camPos + perpendicular * NODE_RADIUS);
        }
        if (j >= 2)
        {
            Vector2 perpendicular = (this.nodes[j - 2].pos - this.nodes[j - 1].pos).VerticalNormalized();
            this.tm.MoveVertice(j * 2 - 2, this.nodes[j - 1].pos - camPos + perpendicular * -NODE_RADIUS);
            this.tm.MoveVertice(j * 2 - 1, this.nodes[j - 1].pos - camPos + perpendicular * NODE_RADIUS);
        }
        for (int i = j * 2; i < MAX_NODES * 2 + 1; i++)
        {
            this.tm.MoveVertice(i, tail);
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

    // ==================== 绳身重建（持久化 / 跨房间） ====================

    /// <summary>
    /// 由 Knot.PlaceInRoom（进房）或 ScientistPlugin（读档）触发：按绳结记录的"对端 ID + 绳长"把绳身接回来。
    /// 两端同房间 → 接一条整绳（由 ID 较小的一端负责，避免重复）；
    /// 两端不同房间 → 两侧各接半截绳，固定端锚在该房间通往对端房间的通道口，两段共享总长。
    /// </summary>
    /// <summary>段计划：room 内的一段绳身，两端各是绳结或固定锚点。</summary>
    private class SegmentPlan
    {
        public Room room;
        public KnotAbstract knotA;
        public KnotAbstract knotB;
        public Vector2 anchorA;
        public Vector2 anchorB;
        public bool fixedA;
        public bool fixedB;
    }

    /// <summary>
    /// 统一重建：把一条绳拆成"沿途经过管道口/通道口锚点"的若干段。
    ///  · 同房间、直连不被地形挡住 → 一段（绳结↔绳结）
    ///  · 同房间、被挡住且存在明显更近的房内管道 → 两段（绳结→口1、口2→绳结）
    ///  · 跨房间 → A 侧"绳结→出房通道口"、B 侧"入房通道口→绳结"
    /// 管道本身不建绳身，只把长度计入共享预算，所以"过管道"和"跨房间"是同一套逻辑。
    /// </summary>
    public static void TryRebuildFor(KnotAbstract knot)
    {
        if (knot?.Room?.realizedRoom == null || knot.realizedObject == null || !knot.hasRopePartner)
        {
            return;
        }
        if (!knot.ss.Empty())
        {
            return;
        }
        KnotAbstract partner = FindKnotById(knot.world, knot.ropePartnerID);
        if (partner == null)
        {
            return;
        }
        if (partner.realizedObject == null && partner.Room == knot.Room)
        {
            // 对端已经挂进同一房间但还没实现（例如命令/脚本生成），这里主动实现它
            partner.RealizeInRoom();
        }
        if (partner.realizedObject == null || !partner.ss.Empty())
        {
            return;                       // 对端还没就绪，或已经由对端接上了
        }

        Room roomA = knot.Room.realizedRoom;
        List<SegmentPlan> plans = new List<SegmentPlan>();

        if (partner.Room == knot.Room)
        {
            Vector2 anchorA, anchorB;
            if (TryUseInRoomShortcut(roomA, knot, partner, out anchorA, out anchorB, out _))
            {
                plans.Add(new SegmentPlan { room = roomA, knotA = knot, anchorB = anchorA, fixedB = true });
                plans.Add(new SegmentPlan { room = roomA, anchorA = anchorB, fixedA = true, knotB = partner });
            }
            else
            {
                plans.Add(new SegmentPlan { room = roomA, knotA = knot, knotB = partner });
            }
        }
        else
        {
            IntVector2 startTile;
            WorldCoordinate destCoord;
            if (!TryFindShortcutToRoom(roomA, partner.Room.index, out startTile, out destCoord, out _))
            {
                return;                   // 找不到出房通道 → 先不接
            }
            Vector2 exit = roomA.MiddleOfTile(startTile);
            plans.Add(new SegmentPlan { room = roomA, knotA = knot, anchorB = exit, fixedB = true });
            Room roomB = partner.Room.realizedRoom;
            if (roomB != null)
            {
                Vector2 entry = roomB.MiddleOfTile(destCoord.Tile);
                plans.Add(new SegmentPlan { room = roomB, anchorA = entry, fixedA = true, knotB = partner });
            }
        }

        string key = RopeKey(knot);
        bool shared = plans.Count > 1;
        for (int i = 0; i < plans.Count; i++)
        {
            SegmentPlan plan = plans[i];
            if (plan.room == null)
            {
                continue;
            }
            StringShort segment = plan.fixedA
                ? new StringShort(plan.room, knot.ropeLength, plan.anchorA, plan.knotB)
                : (plan.fixedB
                    ? new StringShort(plan.room, knot.ropeLength, plan.knotA, plan.anchorB)
                    : new StringShort(plan.room, knot.ropeLength, plan.knotA, plan.knotB));
            if (shared)
            {
                segment.SetSharedLength(key, i, knot.ropeLength);
            }
            if (plan.knotA != null) { plan.knotA.ss.Add(segment); }
            if (plan.knotB != null) { plan.knotB.ss.Add(segment); }
            plan.room.AddObject(segment);
        }
    }

    private static KnotAbstract FindKnotById(World world, EntityID id)
    {
        if (world?.abstractRooms == null)
        {
            return null;
        }
        for (int i = 0; i < world.abstractRooms.Length; i++)
        {
            AbstractRoom ar = world.abstractRooms[i];
            if (ar?.entities == null)
            {
                continue;
            }
            for (int j = 0; j < ar.entities.Count; j++)
            {
                if (ar.entities[j] is KnotAbstract k && k.ID == id)
                {
                    return k;
                }
            }
        }
        return null;
    }

    /// <summary>
    /// 同房间内是否需要走管道：直连被地形挡住、且存在一条"到两个管口都通畅、且总路程明显更短"的房内管道。
    /// </summary>
    private static bool TryUseInRoomShortcut(Room room, KnotAbstract a, KnotAbstract b, out Vector2 anchorA, out Vector2 anchorB, out float pipeLength)
    {
        anchorA = Vector2.zero;
        anchorB = Vector2.zero;
        pipeLength = 0f;
        if (room.shortcuts == null)
        {
            return false;
        }
        Vector2 pa = a.CurrentPosition;
        Vector2 pb = b.CurrentPosition;
        if (SharedPhysics.RayTraceTilesForTerrain(room, pa, pb))
        {
            return false;                  // 一路畅通（原版返回 true = 没被挡），不需要管道
        }

        float best = Vector2.Distance(pa, pb) * PIPE_DETOUR_FACTOR;   // 只有明显更近才采用
        bool found = false;
        for (int i = 0; i < room.shortcuts.Length; i++)
        {
            ShortcutData sc = room.shortcuts[i];
            if (sc.destinationCoord.room != room.abstractRoom.index)
            {
                continue;                  // 只管同房间内的管道
            }
            Vector2 p1 = room.MiddleOfTile(sc.StartTile);
            Vector2 p2 = room.MiddleOfTile(sc.DestTile);
            TryShortcutCandidate(room, pa, pb, p1, p2, sc.length, ref best, ref found, ref anchorA, ref anchorB, ref pipeLength);
            TryShortcutCandidate(room, pa, pb, p2, p1, sc.length, ref best, ref found, ref anchorA, ref anchorB, ref pipeLength);
        }
        return found;
    }

    private static void TryShortcutCandidate(Room room, Vector2 pa, Vector2 pb, Vector2 from, Vector2 to, int scLength, ref float best, ref bool found, ref Vector2 anchorA, ref Vector2 anchorB, ref float pipeLength)
    {
        if (!SharedPhysics.RayTraceTilesForTerrain(room, pa, from) || !SharedPhysics.RayTraceTilesForTerrain(room, to, pb))
        {
            return;                        // 到管口的这两截被挡（原版返回 false = 被挡），跳过
        }
        float total = Vector2.Distance(pa, from) + Vector2.Distance(to, pb);
        if (total < best)
        {
            best = total;
            found = true;
            anchorA = from;
            anchorB = to;
            pipeLength = scLength;
        }
    }

    /// <summary>找到本房间通往目标房间的通道（跨房间绳段的锚点）。</summary>
    private static bool TryFindShortcutToRoom(Room room, int destRoomIndex, out IntVector2 startTile, out WorldCoordinate destCoord, out int pipeLength)
    {
        startTile = default(IntVector2);
        destCoord = default(WorldCoordinate);
        pipeLength = 0;
        if (room.shortcuts == null)
        {
            return false;
        }
        for (int i = 0; i < room.shortcuts.Length; i++)
        {
            if (room.shortcuts[i].destinationCoord.room == destRoomIndex)
            {
                startTile = room.shortcuts[i].StartTile;
                destCoord = room.shortcuts[i].destinationCoord;
                pipeLength = room.shortcuts[i].length;
                return true;
            }
        }
        return false;
    }

    private static bool IsLowerId(EntityID a, EntityID b)
    {
        if (a.number != b.number)
        {
            return a.number < b.number;
        }
        return a.spawner <= b.spawner;
    }

    private static string RopeKey(KnotAbstract knot)
    {
        EntityID a = knot.ID;
        EntityID b = knot.ropePartnerID;
        return IsLowerId(a, b) ? $"{a}|{b}" : $"{b}|{a}";
    }
}
