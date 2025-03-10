using System;
using System.Collections;
using UnityEngine;
using Scientist;

namespace Scientist.Animations;

public class PlayerAnimations
{
    public int pIndex
    {
        get => this.player.playerState.playerNumber;
    }

    public Player player;
    public Queue queue;
    public bool isPlaying;
    public int totalAnimationIndex;
    public int lastAnimationIndex;
    public int waitTime;
    public bool isJumped;
    public float whRightMost;
    public TutorialControlsPageOwner tutCntrlPgOwner;

    public bool isHardSetStartPos = false;

    public int x = 0;
    public int y = 0;
    public bool jmp = false;
    public bool pckp = false;
    public bool thrw = false;

    public PlayerAnimations(Player player)
    {
        this.player = player;
        this.queue = new Queue();
        this.isPlaying = false;
        this.totalAnimationIndex = 0;
        this.lastAnimationIndex = 0;
        this.waitTime = -1;
        this.isJumped = false;
        this.whRightMost = 0f;
    }

    public void Update()
    {
        if (!this.isPlaying)
        {
            return;
        }
        if (this.queue.Empty())
        {
            this.isPlaying = false;
            this.ControlOn();
            return;
        }
        if (this.waitTime > 0)
        {
            this.waitTime--;
        }
        Func<bool> func = (Func<bool>)this.queue.Peek();
        if (func())
        {
            this.ResetInput();
            ScientistLogger.Log($"Animation {func.Method.Name}({this.lastAnimationIndex}) finished, time = {DateTime.Now}");
            this.isHardSetStartPos = false;
            this.queue.Dequeue();
            this.lastAnimationIndex++;
        }
    }

    public void StartPlaying(Vector2 spawnPos)
    {
        ScientistLogger.Log($"Animation started, time = {DateTime.Now}");
        player.SuperHardSetPosition(spawnPos);
        player.bodyChunks[0].HardSetPosition(spawnPos);
        player.bodyChunks[1].HardSetPosition(spawnPos - new Vector2(0f, 0.5f));
        this.ControlOff();
        this.isPlaying = true;
    }

    public Player.InputPackage GetInput()
    {
        if (!this.isPlaying)
        {
            return new Player.InputPackage(false, Options.ControlSetup.Preset.None, 0, 0, false, false, false, false, false);
        }
        return new Player.InputPackage(false, Options.ControlSetup.Preset.None, this.x, this.y, this.jmp, this.pckp, this.thrw, false, false);
    }

    public void ResetInput()
    {
        this.x = 0;
        this.y = 0;
        this.jmp = false;
        this.pckp = false;
        this.thrw = false;
    }

    /// <summary>
    /// ��Ӷ��������У�˳��ִ�У�
    /// </summary>
    /// <param name="func">��������</param>
    public void AddAnimationFollow(Func<bool> func)
    {
        this.queue.Enqueue(func);
        this.totalAnimationIndex++;
    }

    /// <summary>
    /// ��Ӷ��������У�����ִ�У���ģʽ���������в��ж���������true������ɣ��ص����ݵĴ���Ϊ����ĺ�������ǰ��ĺ�����
    /// </summary>
    /// <param name="funcs">��������</param>
    public void AddAnimationFollowAnd(params Func<bool>[] funcs)
    {
        this.queue.Enqueue(() => _AnimationsAnd(funcs));
        this.totalAnimationIndex++;
    }

    private bool _AnimationsAnd(params Func<bool>[] funcs)
    {
        bool flag = true;
        for (int i = 0; i < funcs.Length; i++)
        {
            Func<bool> func = funcs[i];
            flag &= func();
        }
        return flag;
    }

    /// <summary>
    /// ����è������ҿ���
    /// </summary>
    /// <param name="setNull"></param>
    public void ControlOff()
    {
        this.player.controller = new ProgramController(this);
    }

    /// <summary>
    /// �ָ���ҿ�������è
    /// </summary>
    public void ControlOn()
    {
        player.controller = null;
        RWInput.PlayerRecentController(pIndex);
        if (this.player.room.game.cameras[0].hud != null && this.player.room.game.rainWorld.options.controls[0].GetActivePreset() != Options.ControlSetup.Preset.None)
        {
            this.tutCntrlPgOwner = new TutorialControlsPageOwner(this.player.room.game);
            this.player.room.AddObject(this.tutCntrlPgOwner);
        }
        if (this.tutCntrlPgOwner != null)
        {
            this.tutCntrlPgOwner.controlsPage.wantToContinue = true;
        }
    }

    /// <summary>
    /// �ȴ�ָ��ʱ��
    /// </summary>
    /// <param name="time">ʱ�䣬40֡Ϊ1��</param>
    /// <returns>�Ƿ����</returns>
    public bool Wait(int time)
    {
        if (!isPlaying)
        {
            return false;
        }
        if (this.waitTime == -1)
        {
            this.waitTime = time;
        }
        this.ResetInput();
        bool flag = this.waitTime == 0;
        if (flag)
        {
            this.waitTime = -1;
        }
        return flag;
    }

    /// <summary>
    /// ����èˮƽ�ƶ�����
    /// </summary>
    /// <param name="startPos">��ʼλ��</param>
    /// <param name="dis">����</param>
    /// <returns>�Ƿ񵽴�</returns>
    public bool HorizontallyMove(Vector2 startPos, float dis, BodyChunk[] bodyChunks)
    {
        if (!isPlaying)
        {
            return false;
        }
        if (!this.isHardSetStartPos) { player.mainBodyChunk.HardSetPosition(startPos); this.isHardSetStartPos = true; }
        this.x = (Mathf.Sign(dis) == 1 ? this.player.bodyChunks[0].pos.x < startPos.x + dis : this.player.bodyChunks[0].pos.x > startPos.x + dis).ToInt() * (int)Mathf.Sign(dis);
        return Mathf.Sign(dis) == 1 ? this.player.bodyChunks[0].pos.x > startPos.x + dis : this.player.bodyChunks[0].pos.x < startPos.x + dis;
    }

    /// <summary>
    /// ����è��ֱ�ƶ�����
    /// </summary>
    /// <param name="startPos">��ʼ����</param>
    /// <param name="dis">����</param>
    /// <param name="bodyChunks">��ʼ��������</param>
    /// <returns></returns>
    public bool VerticallyMove(Vector2 startPos, float dis, BodyChunk[] bodyChunks)
    {
        if (!isPlaying)
        {
            return false;
        }
        if (!this.isHardSetStartPos) { player.mainBodyChunk.HardSetPosition(startPos); this.isHardSetStartPos = true; }
        this.y = (Mathf.Sign(dis) == 1 ? this.player.bodyChunks[0].pos.y < startPos.y + dis : this.player.bodyChunks[0].pos.y > startPos.y + dis).ToInt() * (int)Mathf.Sign(dis);
        return Mathf.Sign(dis) == 1 ? this.player.bodyChunks[0].pos.y > startPos.y + dis : this.player.bodyChunks[0].pos.y < startPos.y + dis;
    }

    /// <summary>
    /// ����è��Ծ����
    /// </summary>
    /// <returns></returns>
    public bool Jump()
    {
        if (!isPlaying)
        {
            return false;
        }
        if (player.canJump > 0 && !this.isJumped)
        {
            this.jmp = true;
            this.isJumped = true;
            return false;
        }
        return player.canJump > 0  && this.isJumped;
    }

    public bool Stand()
    {
        if (!isPlaying)
        {
            return false;
        }
        this.player.standing = true;
        this.y = 1;
        return true;
    }
}

public class ProgramController : Player.PlayerController
{

    public readonly PlayerAnimations owner;

    public ProgramController(PlayerAnimations owner)
    {
        this.owner = owner;
    }

    public override Player.InputPackage GetInput()
    {
        return this.owner.GetInput();
    }
}