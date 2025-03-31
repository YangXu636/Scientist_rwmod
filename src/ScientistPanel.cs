using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BepInEx.Logging;
using Expedition;
using Menu;
using Menu.Remix;
using Menu.Remix.MixedUI;
using MonoMod.Utils;
using MoreSlugcats;
using RWCustom;
using UnityEngine;
using static BeastMaster.BeastMaster;

namespace Scientist;

public class ScientistPanel : Menu.Menu
{
    public RainWorldGame game;
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
    public Scientist.ScientistPanel.PageModeIndex pageMode;
    public SimpleButton objectsPageButton;
    public SimpleButton craftingTablePageButton;
    public SimpleButton advancementsPageButton;
    public SimpleButton debugPageButton;
    public MenuTabWrapper objectsTabwrapper;
    public OpScrollBox objectsScrollBox;
    public List<OpSimpleImageButton> objectButtons;

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
        this.pages.Add(new Page(this, this.pages[0], "Objects", 1));
        this.pages.Add(new Page(this, this.pages[0], "CraftingTable", 2));
        this.pages.Add(new Page(this, this.pages[0], "Advancements", 3));
        this.pages.Add(new Page(this, this.pages[0], "Debug", 4));
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
            this.blackSprite[i].x = this.manager.rainWorld.options.ScreenSize.x / 2f;
            this.blackSprite[i].y = this.manager.rainWorld.options.ScreenSize.y / 2f;
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
        this.container.MoveToFront();
        for (int i = 1; i < this.pages.Count; i++)
        {
            this.pages[i].Container.isVisible = false;
        }
        base.currentPage = 0;
    }

    private void SetupLayout()
    {
        this.uselessMessage = new MenuLabel(this, this.pages[0], $"{base.Translate("SCIENCE_PANEL_TITLE")}", new Vector2(0f, 0f), new Vector2(300f, 30f), false);
        this.pages[0].subObjects.Add(this.uselessMessage);
        this.objectsPageButton = new SimpleButton(this, this.pages[0], base.Translate("OBJECTS_PAGE"), "OBJECTS_PAGE", new Vector2(10f, manager.rainWorld.options.ScreenSize.y - 60f), new Vector2(110f, 30f));
        this.pages[0].subObjects.Add(this.objectsPageButton);
        this.craftingTablePageButton = new SimpleButton(this, this.pages[0], base.Translate("CRAFTINGTABLE_PAGE"), "CRAFTINGTABLE_PAGE", new Vector2(10f, manager.rainWorld.options.ScreenSize.y - 100f), new Vector2(110f, 30f));
        this.pages[0].subObjects.Add(this.craftingTablePageButton);
        this.advancementsPageButton = new SimpleButton(this, this.pages[0], base.Translate("ADVANCEMENTS_PAGE"), "ADVANCEMENTS_PAGE", new Vector2(10f, manager.rainWorld.options.ScreenSize.y - 140f), new Vector2(110f, 30f));
        this.pages[0].subObjects.Add(this.advancementsPageButton);
        this.debugPageButton = new SimpleButton(this, this.pages[0], base.Translate("DEBUG_PAGE"), "DEBUG_PAGE", new Vector2(10f, 30f), new Vector2(110f, 30f));
        this.pages[0].subObjects.Add(this.debugPageButton);

        this.objectsPageButton.nextSelectable[1] = this.objectsPageButton.nextSelectable[2] = this.debugPageButton;                  //1:up 2:left 3:down 4:right
        this.objectsPageButton.nextSelectable[3] = this.craftingTablePageButton;
        this.craftingTablePageButton.nextSelectable[1] = this.craftingTablePageButton.nextSelectable[2] = this.objectsPageButton;
        this.craftingTablePageButton.nextSelectable[3] = this.advancementsPageButton;
        this.advancementsPageButton.nextSelectable[1] = this.advancementsPageButton.nextSelectable[2] = this.craftingTablePageButton;
        this.advancementsPageButton.nextSelectable[3] = this.debugPageButton;
        this.debugPageButton.nextSelectable[1] = this.debugPageButton.nextSelectable[2] = this.advancementsPageButton;
        this.debugPageButton.nextSelectable[3] = this.objectsPageButton;

        this.objectButtons = new();
        FieldInfo[] fields = typeof(Enums.Items).GetFields(BindingFlags.Public | BindingFlags.Static);
        AbstractPhysicalObject.AbstractObjectType apo = null;
        foreach (FieldInfo field in fields)
        {
            apo = field.GetValue(null) as AbstractPhysicalObject.AbstractObjectType;
            if (apo != null)
            {
                this.objectButtons.Add(new OpSimpleImageButton(new Vector2(100f, this.objectButtons.Count * 60f + 10f), new Vector2(40f, 40f), ItemSymbol.SpriteNameForItem(apo, 0))
                {
                    description = apo.ToString(),
                    colorEdge = ItemSymbol.ColorForItem(apo, 0)
                });
            }
        }
        this.objectsTabwrapper = new MenuTabWrapper(this, this.pages[1]);
        this.pages[1].subObjects.Add(this.objectsTabwrapper);
        this.objectsScrollBox = new OpScrollBox(new Vector2(300f, (manager.rainWorld.options.ScreenSize.y - 750f) / 2f), new Vector2(80f, 700f), (float)objectButtons.Count * 40f + 40f);
        new UIelementWrapper(this.objectsTabwrapper, this.objectsScrollBox);
        for (int i = 0; i < this.objectButtons.Count; i++)
        {
            new UIelementWrapper(this.objectsTabwrapper, this.objectButtons[i]);
        }
        for (int i = 0; i < this.objectButtons.Count; i++)
        {
            this.objectsScrollBox.AddItems(this.objectButtons[i]);
        }
        /*this.pages[1].subObjects.Add(objectsScrollBox.wrapper);*/
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
            this.playerOpen = this.game.cameras[0].followAbstractCreature.realizedCreature as Player;
            this.uselessMessage.text = $"{base.Translate("SCIENCE_PANEL_TITLE")} {(this.playerOpen != null && this.playerOpen.room.game.session is StoryGameSession ? "" : "?")}{base.Translate(this.playerOpen != null && !this.playerOpen.inShortcut ? Region.GetRegionFullName(this.playerOpen.room.world.name, this.playerOpen.room.game.session is StoryGameSession ? this.playerOpen.room.game.GetStorySession.saveState.saveStateNumber : this.playerOpen.playerState.slugcatCharacter) : "NRE_Region")} {base.Translate(this.playerOpen != null ? this.playerOpen.room.abstractRoom.name : "NRE_Room")}  playerNumber = {(this.playerOpen == null ? "NRE_PlayerNumber" : this.playerOpen.playerState.playerNumber)}";
        }
        catch (Exception /*e*/) { /*ScientistLogger.Log(e); e.LogDetailed();*/ }
        //string regionName = base.Translate(this.playerOpen != null && !this.playerOpen.inShortcut ? Region.GetRegionFullName(this.playerOpen.room.world.name, this.playerOpen.room.game.session is StoryGameSession ? this.playerOpen.room.game.GetStorySession.saveState.saveStateNumber : this.playerOpen.playerState.slugcatCharacter) : "NRE_Region");
        //string roomName = this.playerOpen != null ? this.playerOpen.room.abstractRoom.name : "NRE_Room";
        //string playerName = $"{(this.playerOpen != null ? this.playerOpen.playerState.slugcatCharacter.value : "NREPlayerType")}_{(this.playerOpen == null ? "NREPlayerNumber" : this.playerOpen.playerState.playerNumber)}";
        this.uselessMessage.pos = new Vector2((this.manager.rainWorld.options.ScreenSize.x - this.uselessMessage.size.x) / 2.000f, 0f);
        base.Update();
        this.WarpUpdate();
    }

    public override void GrafUpdate(float timeStacker)
    {
        base.GrafUpdate(timeStacker);
        if (this.game.IsArenaSession && this.game.GetArenaGameSession is SandboxGameSession sgs && sgs.overlay != null)
        {
            sgs.overlay.GrafUpdate(timeStacker);
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
            if (message.EndsWith("_PAGE"))
            {
                this.ChangePage(new Scientist.ScientistPanel.PageModeIndex(message.Substring(0, message.Length - 5).ToLower().FirstCharToUpper()));
            }
        }
        this.WarpSignal(sender, message);
    }

    public void SetVisible(bool visible)
    {
        this.isShowing = visible;
        this.container.isVisible = visible;
        if (visible) { this.container.MoveToFront(); }
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
        this.SetVisible(false);
        base.ShutDownProcess();
    }

    public void ChangePage(Scientist.ScientistPanel.PageModeIndex mode)
    {
        if (this.pageMode == mode) { return; }
        for (int i = 1; i < this.pages.Count; i++)
        {
            if (pages[i].name.ToUpper() == mode.ToString().ToUpper())
            {
                this.pages[i].Container.isVisible = true;
                base.currentPage = i;
            }
            else
            {
                this.pages[i].Container.isVisible = false;
            }
        }
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

    public class PageModeIndex : ExtEnum<Scientist.ScientistPanel.PageModeIndex>
    {

        public PageModeIndex(string value, bool register = false) : base(value, register)
        {
        }

        public static readonly Scientist.ScientistPanel.PageModeIndex Objects = new Scientist.ScientistPanel.PageModeIndex("Objects", true);
        public static readonly Scientist.ScientistPanel.PageModeIndex CraftingTable = new Scientist.ScientistPanel.PageModeIndex("CraftingTable", true);
        public static readonly Scientist.ScientistPanel.PageModeIndex Advancements = new Scientist.ScientistPanel.PageModeIndex("Advancements", true);
        public static readonly Scientist.ScientistPanel.PageModeIndex Debug = new Scientist.ScientistPanel.PageModeIndex("Debug", true);
    }
}
