using System;
using System.Collections.Generic;
using System.IO;
using BepInEx.Logging;
using Expedition;
using Menu;
using Menu.Remix;
using Menu.Remix.MixedUI;
using MonoMod.Utils;
using MoreSlugcats;
using RWCustom;
using UnityEngine;

namespace Scientist;

public class ScientistPanel : Menu.Menu
{
    public RainWorldGame game;
    public OpScrollBox objectsScrollBox;
    public List<OpSimpleImageButton> objectButtons;
    public FSprite[] blackSprite;
    public float blackFade;
    public float lastBlackFade;
    private bool lastPauseButton;
    public float[,] micVolumes;
    public ControlMap controlMap;
    public int counter;

    public bool isShowing = false;
    public Player playerOpen;
    public MenuLabel uselessMessage;

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
        this.pages.Add(new Page(this, this.pages[0], "debug", 4));
        this.blackSprite = new FSprite[5];
        for (int i = 0; i < this.blackSprite.Length; i++)
        {
            this.blackSprite[i] = new FSprite("pixel", true);
            this.blackSprite[i].color = Menu.Menu.MenuRGB(Menu.Menu.MenuColors.Black);
            if (i == 0)
            {
                this.blackSprite[i].scaleX = 1400f;
                this.blackSprite[i].scaleY = 800f;
                this.blackSprite[i].alpha = 0.5f;
            }
            else
            {
                this.blackSprite[i].scaleX = 1100f;
                this.blackSprite[i].scaleY = 720f;
                this.blackSprite[i].alpha = 0.2f;
            }
            this.blackSprite[i].x = manager.rainWorld.options.ScreenSize.x / 2f;
            this.blackSprite[i].y = manager.rainWorld.options.ScreenSize.y / 2f;
        }
        for (int i = 0; i < this.blackSprite.Length; i++)
        {
            this.pages[i].Container.AddChild(this.blackSprite[i]);
        }
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
        /*for (int k = 0; k < game.cameras.Length; k++)
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
        }*/
        this.WarpInit(game);
    }

    private void SetupLayout()
    {
        this.uselessMessage = new MenuLabel(this, this.pages[0], $"{base.Translate("SCIENCE_PANEL_TITLE")}", new Vector2(0f, 0f), new Vector2(300f, 30f), false);
        this.pages[0].subObjects.Add(this.uselessMessage);
        SimpleButton objectsPageButton = new SimpleButton(this, this.pages[0], base.Translate("OBJECTS_PAGE"), "OBJECTS_PAGE", new Vector2(10f, manager.rainWorld.options.ScreenSize.y - 60f), new Vector2(110f, 30f));
        this.pages[0].subObjects.Add(objectsPageButton);
        SimpleButton craftingTablePageButton = new SimpleButton(this, this.pages[0], base.Translate("CRAFTING_TABLE_PAGE"), "CRAFTING_TABLE_PAGE", new Vector2(10f, manager.rainWorld.options.ScreenSize.y - 100f), new Vector2(110f, 30f));
        this.pages[0].subObjects.Add(craftingTablePageButton);
        SimpleButton advancementsPageButton = new SimpleButton(this, this.pages[0], base.Translate("ADVANCEMENTS_PAGE"), "ADVANCEMENTS_PAGE", new Vector2(10f, manager.rainWorld.options.ScreenSize.y - 140f), new Vector2(110f, 30f));
        this.pages[0].subObjects.Add(advancementsPageButton);
    }

    public override void Update()
    {
        this.counter++;
        if (this.game.IsArenaSession && this.game.GetArenaGameSession is SandboxGameSession && (this.game.GetArenaGameSession as SandboxGameSession).overlay != null)
        {
            (this.game.GetArenaGameSession as SandboxGameSession).overlay.Update();
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
        try
        {
            this.playerOpen = game.cameras[0].followAbstractCreature.realizedCreature as Player;
            this.uselessMessage.text = $"{base.Translate("SCIENCE_PANEL_TITLE")} {(this.playerOpen != null && this.playerOpen.room.game.session is StoryGameSession ? "" : "?")}{base.Translate(this.playerOpen != null && !this.playerOpen.inShortcut ? Region.GetRegionFullName(this.playerOpen.room.world.name, this.playerOpen.room.game.session is StoryGameSession ? this.playerOpen.room.game.GetStorySession.saveState.saveStateNumber : this.playerOpen.playerState.slugcatCharacter) : "NRE_Region")} {base.Translate(this.playerOpen != null ? this.playerOpen.room.abstractRoom.name : "NRE_Room")}  playerNumber = {(this.playerOpen == null ? "NRE_PlayerNumber" : this.playerOpen.playerState.playerNumber)}";
        }
        catch (Exception /*e*/) { /*ScientistLogger.Log(e); e.LogDetailed();*/ }
        //string regionName = base.Translate(this.playerOpen != null && !this.playerOpen.inShortcut ? Region.GetRegionFullName(this.playerOpen.room.world.name, this.playerOpen.room.game.session is StoryGameSession ? this.playerOpen.room.game.GetStorySession.saveState.saveStateNumber : this.playerOpen.playerState.slugcatCharacter) : "NRE_Region");
        //string roomName = this.playerOpen != null ? this.playerOpen.room.abstractRoom.name : "NRE_Room";
        //string playerName = $"{(this.playerOpen != null ? this.playerOpen.playerState.slugcatCharacter.value : "NREPlayerType")}_{(this.playerOpen == null ? "NREPlayerNumber" : this.playerOpen.playerState.playerNumber)}";
        this.uselessMessage.pos = new Vector2((manager.rainWorld.options.ScreenSize.x - this.uselessMessage.size.x) / 2.000f, 0f);
        base.Update();
        this.WarpUpdate();
    }

    public override void GrafUpdate(float timeStacker)
    {
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

    public void SetVisible(bool visible)
    {
        this.isShowing = visible;
        this.container.isVisible = visible;
    }

    public override void ShutDownProcess()
    {
        for (int j = 0; j < this.game.cameras.Length; j++)
        {
            for (int k = 0; k < this.game.cameras[j].virtualMicrophone.volumeGroups.Length; k++)
            {
                this.game.cameras[j].virtualMicrophone.volumeGroups[k] = this.micVolumes[j, k];
            }
        }
        for (int i = 0; i < this.blackSprite.Length; i++)
        {
            this.blackSprite[i].RemoveFromContainer();
        }
        for (int i = 0; i < this.pages.Count; i++)
        {
            this.pages[i].RemoveSprites();
        }
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
