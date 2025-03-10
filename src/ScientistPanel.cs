using System;
using System.Collections.Generic;
using System.IO;
using BepInEx.Logging;
using Expedition;
using Menu;
using Menu.Remix;
using Menu.Remix.MixedUI;
using MoreSlugcats;
using RWCustom;
using UnityEngine;

namespace Scientist;

public class ScientistPanel : Menu.Menu
{
    public RainWorldGame game;
    public OpScrollBox objectsScrollBox;
    public List<OpSimpleImageButton> objectButtons;
    public MenuLabel confirmMessage;
    public FSprite blackSprite;
    public float blackFade;
    public float lastBlackFade;
    private bool wantToContinue;
    private bool lastPauseButton;
    public float[,] micVolumes;
    public ControlMap controlMap;
    public int counter;

    public void WarpPreInit(RainWorldGame game)
    {

    }

    public void WarpInit(RainWorldGame game)
    {

    }

    public void WarpUpdate()
    {

    }

    public void WarpSignal(MenuObject sender, string message)
    {

    }

    public ScientistPanel(ProcessManager manager, RainWorldGame game) : base(manager, ProcessManager.ProcessID.PauseMenu)
    {
        this.WarpPreInit(game);
        this.game = game;
        this.pages.Add(new Page(this, null, "main", 0));
        this.pages.Add(new Page(this, this.pages[0], "objects", 1));
        this.pages.Add(new Page(this, this.pages[0], "craftingtable", 2));
        this.pages.Add(new Page(this, this.pages[0], "advancements", 3));
        this.blackSprite = new FSprite("pixel", true);
        this.blackSprite.color = Menu.Menu.MenuRGB(Menu.Menu.MenuColors.Black);
        this.blackSprite.scaleX = 1400f;
        this.blackSprite.scaleY = 800f;
        this.blackSprite.x = manager.rainWorld.options.ScreenSize.x / 2f;
        this.blackSprite.y = manager.rainWorld.options.ScreenSize.y / 2f;
        this.blackSprite.alpha = 0.5f;
        this.pages[0].Container.AddChild(this.blackSprite);
        this.SetupLayout();
        this.selectedObject = null;
        this.blackFade = 0f;
        this.lastBlackFade = 0f;
        this.micVolumes = new float[game.cameras.Length, game.cameras[0].virtualMicrophone.volumeGroups.Length];
        for (int i = 0; i < game.cameras.Length; i++)
        {
            for (int j = 0; j < game.cameras[0].virtualMicrophone.volumeGroups.Length; j++)
            {
                this.micVolumes[i, j] = game.cameras[0].virtualMicrophone.volumeGroups[j];
            }
        }
        base.PlaySound(SoundID.HUD_Pause_Game);
        for (int k = 0; k < game.cameras.Length; k++)
        {
            if (game.cameras[k].hud != null && game.cameras[k].hud.textPrompt != null)
            {
                game.cameras[k].hud.textPrompt.pausedMode = true;
                if (game.IsStorySession && game.GetStorySession.saveState.cycleNumber > 0)
                {
                    if (game.clock > 200 && (game.GetStorySession.RedIsOutOfCycles || (ModManager.Expedition && game.rainWorld.ExpeditionMode && game.GetStorySession.saveState.deathPersistentSaveData.karma == 0)))
                    {
                        game.cameras[k].hud.textPrompt.pausedWarningText = true;
                    }
                    else if (game.clock > 1200)
                    {
                        if (game.manager.rainWorld.progression.miscProgressionData.warnedAboutKarmaLossOnExit < 4)
                        {
                            game.cameras[k].hud.textPrompt.pausedWarningText = true;
                            PlayerProgression.MiscProgressionData miscProgressionData = game.manager.rainWorld.progression.miscProgressionData;
                            int warnedAboutKarmaLossOnExit = miscProgressionData.warnedAboutKarmaLossOnExit;
                            miscProgressionData.warnedAboutKarmaLossOnExit = warnedAboutKarmaLossOnExit + 1;
                        }
                    }
                }
                else
                {
                    game.cameras[k].hud.textPrompt.pausedWarningText = false;
                }
            }
        }
        this.WarpInit(game);
    }

    private void SetupLayout()
    {
        
    }

    public override void Update()
    {
        this.counter++;
        /*if (this.game.IsStorySession && this.continueButton != null && this.exitButton != null)
        {
            this.continueButton.buttonBehav.greyedOut = this.wantToContinue;
            this.exitButton.buttonBehav.greyedOut = (this.wantToContinue || this.counter < 40);
        }
        else
        {
            for (int i = 0; i < this.pages[0].subObjects.Count; i++)
            {
                if (this.pages[0].subObjects[i] is SimpleButton)
                {
                    (this.pages[0].subObjects[i] as SimpleButton).buttonBehav.greyedOut = this.wantToContinue;
                }
            }
        }*/
        if (this.game.IsArenaSession && this.game.GetArenaGameSession is SandboxGameSession && (this.game.GetArenaGameSession as SandboxGameSession).overlay != null)
        {
            (this.game.GetArenaGameSession as SandboxGameSession).overlay.Update();
        }
        bool flag = RWInput.CheckPauseButton(0);
        if (flag && !this.lastPauseButton && this.counter > 10)
        {
            this.wantToContinue = true;
            base.PlaySound(SoundID.HUD_Unpause_Game);
        }
        this.lastPauseButton = flag;
        this.lastBlackFade = this.blackFade;
        if (this.wantToContinue)
        {
            this.blackFade = Mathf.Max(0f, this.blackFade - 0.125f);
            if (this.blackFade <= 0f)
            {
                this.game.ContinuePaused();
            }
        }
        else
        {
            this.blackFade = Mathf.Min(1f, this.blackFade + 0.0625f);
        }
        for (int j = 0; j < this.game.cameras.Length; j++)
        {
            for (int k = 0; k < this.game.cameras[j].virtualMicrophone.volumeGroups.Length; k++)
            {
                if (k == 1)
                {
                    this.game.cameras[j].virtualMicrophone.volumeGroups[k] = this.micVolumes[j, k];
                }
                else
                {
                    this.game.cameras[j].virtualMicrophone.volumeGroups[k] = this.micVolumes[j, k] * (1f - this.blackFade * 0.5f);
                }
            }
        }
        base.Update();
        this.WarpUpdate();
    }

    public override void GrafUpdate(float timeStacker)
    {
        float num = Custom.SCurve(Mathf.Lerp(this.lastBlackFade, this.blackFade, timeStacker), 0.6f);
        this.blackSprite.alpha = num * 0.25f;
        base.GrafUpdate(timeStacker);
        if (this.game.IsArenaSession && this.game.GetArenaGameSession is SandboxGameSession && (this.game.GetArenaGameSession as SandboxGameSession).overlay != null)
        {
            (this.game.GetArenaGameSession as SandboxGameSession).overlay.GrafUpdate(timeStacker);
        }
        for (int i = 0; i < this.game.cameras.Length; i++)
        {
            this.game.cameras[i].virtualMicrophone.DrawUpdate(timeStacker, 1f - 0.3f * Mathf.Lerp(this.lastBlackFade, this.blackFade, timeStacker));
        }
    }

    public override void Singal(MenuObject sender, string message)
    {
        if (message != null)
        {
            ScientistLogger.Log(message);
        }
        this.WarpSignal(sender, message);
    }

    public override void ShutDownProcess()
    {
        for (int i = 0; i < 4; i++)
        {
            PlayerHandler playerHandler = this.game.rainWorld.GetPlayerHandler(i);
            if (playerHandler != null)
            {
                playerHandler.ControllerHandler.SetRumblePaused(false);
            }
        }
        for (int j = 0; j < this.game.cameras.Length; j++)
        {
            for (int k = 0; k < this.game.cameras[j].virtualMicrophone.volumeGroups.Length; k++)
            {
                this.game.cameras[j].virtualMicrophone.volumeGroups[k] = this.micVolumes[j, k];
            }
        }
        this.blackSprite.RemoveFromContainer();
        base.ShutDownProcess();
    }

    //以下代码均参考了FakeAchievements
    public static Texture2D LoadTexture(string path)
    {
        Texture2D texture2D = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        return AssetManager.SafeWWWLoadTexture(ref texture2D, path, false, true);
    }

    public static FAtlas LoadObjectsIcon(string id)
    {
        FAtlasManager atlasManager = Futile.atlasManager;
        bool flag = atlasManager.DoesContainAtlas(id);
        FAtlas result = null;
        if (flag)
        {
            result = atlasManager.GetAtlasWithName(id);
        }
        else
        {
            string filePath = Path.Combine(ScientistPlugin.MOD_PATH, $"panelDocs\\objects\\{id}\\icon.png");
            Texture2D texture = Scientist.ScientistPanel.LoadTexture(filePath);
            FAtlas atlas = new FAtlas(id, texture, FAtlasManager._nextAtlasIndex++, false);
            new FAtlas(id, texture, FAtlasManager._nextAtlasIndex++, false);
            atlasManager.AddAtlas(atlas);
            result = atlas;
        }
        return result;
    }

    public static FAtlas LoadObjectsImage(string id)
    {
        FAtlasManager atlasManager = Futile.atlasManager;
        bool flag = atlasManager.DoesContainAtlas(id);
        FAtlas result = null;
        if (flag)
        {
            result = atlasManager.GetAtlasWithName(id);
        }
        else
        {
            string filePath = Path.Combine(ScientistPlugin.MOD_PATH, $"panelDocs\\objects\\{id}\\image.png");
            Texture2D texture = Scientist.ScientistPanel.LoadTexture(filePath);
            FAtlas atlas = new FAtlas(id, texture, FAtlasManager._nextAtlasIndex++, false);
            new FAtlas(id, texture, FAtlasManager._nextAtlasIndex++, false);
            atlasManager.AddAtlas(atlas);
            result = atlas;
        }
        return result;
    }
}
