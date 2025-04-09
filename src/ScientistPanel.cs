using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

public class ScientistPanel : Menu.Menu, CheckBox.IOwnCheckBox
{
    public RainWorldGame game;
    public FSprite[] blackSprite;
    public float blackFade;
    public float lastBlackFade;
    public float[,] micVolumes;
    public ControlMap controlMap;
    public int counter;

    public bool isShowing = false;
    public Player playerOpen;
    public MenuLabel uselessMessage;
    public int uselessMessageChangeCounter = 0;
    public Scientist.ScientistPanel.PageModeIndex pageMode = Scientist.ScientistPanel.PageModeIndex.None;
    public SimpleButton itemsPageButton;
    public SimpleButton creaturesPageButton;
    public SimpleButton craftingtablePageButton;
    public SimpleButton advancementsPageButton;
    public SimpleButton debugPageButton;

    public List<SimpleButton> itemsObjectButtons;
    public List<KeyValuePair<AbstractPhysicalObject.AbstractObjectType, FSprite>> itemsObjectButtonIcons;
    public List<SimpleButton> itemsObjectChangeGroupIndexButtons;
    public List<FSprite> itemsObjectChangeGroupIndexButtonIcons;
    public const int itemsObjectGroupCount = 11;
    public int itemsObjectGroupIndex = 0;
    public Dictionary<string, SimpleButton> itemsObjectInfoButtons;
    public Dictionary<string, OpImage> itemsObjectInfoPic;
    public string selectedItemType = "";

    public Dictionary<string, CheckBox> debugCheckboxes;

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
        this.pages.Add(new Page(this, null, "Main", 0));
        this.blackSprite = new FSprite[2] { new FSprite("pixel", true) { color = Menu.Menu.MenuRGB(Menu.Menu.MenuColors.Black), scaleX = 1400f, scaleY = 800f, alpha = 0.5f, x = this.manager.rainWorld.options.ScreenSize.x / 2f, y = this.manager.rainWorld.options.ScreenSize.y / 2f }, new FSprite("pixel", true) { color = Menu.Menu.MenuRGB(Menu.Menu.MenuColors.Black), scaleX = 1100f, scaleY = 720f, alpha = 0.4f, x = this.manager.rainWorld.options.ScreenSize.x / 2f, y = this.manager.rainWorld.options.ScreenSize.y / 2f } };
        this.pages[0].Container.AddChild(this.blackSprite[0]);
        this.pages[0].Container.AddChild(this.blackSprite[1]);
        this.SetupChangePageButton();
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
        this.ChangePage(Scientist.ScientistPanel.PageModeIndex.None);
    }

    public void SetupChangePageButton()
    {
        this.uselessMessage = new MenuLabel(this, this.pages[0], $"{base.Translate("SCIENCEPANEL_TITLE")}", new Vector2(0f, 0f), new Vector2(300f, 30f), false);

        this.itemsPageButton = new SimpleButton(this, this.pages[0], base.Translate("SCIENCEPANEL_ITEMS_CHANGEtoPAGE"), "ITEMS_CHANGEtoPAGE", new Vector2(10f, manager.rainWorld.options.ScreenSize.y - 75f), new Vector2(110f, 30f));
        this.creaturesPageButton = new SimpleButton(this, this.pages[0], base.Translate("SCIENCEPANEL_CREATURES_CHANGEtoPAGE"), "CREATURES_CHANGEtoPAGE", new Vector2(10f, manager.rainWorld.options.ScreenSize.y - 115f), new Vector2(110f, 30f));
        this.craftingtablePageButton = new SimpleButton(this, this.pages[0], base.Translate("SCIENCEPANEL_CRAFTINGTABLE_CHANGEtoPAGE"), "CRAFTINGTABLE_CHANGEtoPAGE", new Vector2(10f, manager.rainWorld.options.ScreenSize.y - 155f), new Vector2(110f, 30f));
        this.advancementsPageButton = new SimpleButton(this, this.pages[0], base.Translate("SCIENCEPANEL_ADVANCEMENTS_CHANGEtoPAGE"), "ADVANCEMENTS_CHANGEtoPAGE", new Vector2(10f, manager.rainWorld.options.ScreenSize.y - 195f), new Vector2(110f, 30f));
        this.debugPageButton = new SimpleButton(this, this.pages[0], base.Translate("SCIENCEPANEL_DEBUG_CHANGEtoPAGE"), "DEBUG_CHANGEtoPAGE", new Vector2(10f, 35f), new Vector2(110f, 30f));

        this.itemsPageButton.nextSelectable[1] = this.itemsPageButton.nextSelectable[2] = this.debugPageButton;                  //1:up 2:left 3:down 4:right
        this.itemsPageButton.nextSelectable[3] = this.craftingtablePageButton;
        this.creaturesPageButton.nextSelectable[1] = this.creaturesPageButton.nextSelectable[2] = this.itemsPageButton;                  
        this.creaturesPageButton.nextSelectable[3] = this.craftingtablePageButton;
        this.craftingtablePageButton.nextSelectable[1] = this.craftingtablePageButton.nextSelectable[2] = this.creaturesPageButton;
        this.craftingtablePageButton.nextSelectable[3] = this.advancementsPageButton;
        this.advancementsPageButton.nextSelectable[1] = this.advancementsPageButton.nextSelectable[2] = this.craftingtablePageButton;
        this.advancementsPageButton.nextSelectable[3] = this.debugPageButton;
        this.debugPageButton.nextSelectable[1] = this.debugPageButton.nextSelectable[2] = this.advancementsPageButton;
        this.debugPageButton.nextSelectable[3] = this.itemsPageButton;

        this.pages[0].subObjects.AddSafe(this.uselessMessage); 
        this.pages[0].subObjects.AddSafe(this.itemsPageButton);
        this.pages[0].subObjects.AddSafe(this.creaturesPageButton);
        this.pages[0].subObjects.AddSafe(this.craftingtablePageButton);
        this.pages[0].subObjects.AddSafe(this.advancementsPageButton);
        this.pages[0].subObjects.AddSafe(this.debugPageButton);
    }

    public void SetupItemsPage()
    {
        this.itemsObjectButtons = new();
        this.itemsObjectButtonIcons = new();
        this.itemsObjectInfoButtons = new();
        FieldInfo[] fields = typeof(Enums.Items).GetFields(BindingFlags.Public | BindingFlags.Static);
        foreach (FieldInfo field in fields)
        {
            AbstractPhysicalObject.AbstractObjectType apo = field.GetValue(null) as AbstractPhysicalObject.AbstractObjectType;
            if (apo != null)
            {
                this.itemsObjectButtonIcons.Add(new KeyValuePair<AbstractPhysicalObject.AbstractObjectType, FSprite>(apo, new FSprite(ItemSymbol.SpriteNameForItem(apo, 0), true) { color = ItemSymbol.ColorForItem(apo, 0) }));
            }
        }
        this.ChangeItemsObjectButtonIcon();
        this.itemsObjectChangeGroupIndexButtons = new() { 
            new SimpleButton(this, this.pages[0], "", "ITEMS_itemsObjectGroupIndex_LEFT", new Vector2(150f, 40f), new Vector2(25f, 25f)),
            new SimpleButton(this, this.pages[0], "", "ITEMS_itemsObjectGroupIndex_RIGHT", new Vector2(175f, 40f), new Vector2(25f, 25f))
        };
        this.itemsObjectChangeGroupIndexButtonIcons = new()
        {
            new FSprite("ShortcutArrow", true) {x = this.itemsObjectChangeGroupIndexButtons[0].pos.x + this.itemsObjectChangeGroupIndexButtons[0].size.x / 2f, y = this.itemsObjectChangeGroupIndexButtons[0].pos.y + this.itemsObjectChangeGroupIndexButtons[0].size.y / 2f, color = Menu.Menu.MenuRGB(Menu.Menu.MenuColors.White), rotation = 270f, scale = 1.3f},
            new FSprite("ShortcutArrow", true) {x = this.itemsObjectChangeGroupIndexButtons[1].pos.x + this.itemsObjectChangeGroupIndexButtons[1].size.x / 2f, y = this.itemsObjectChangeGroupIndexButtons[1].pos.y + this.itemsObjectChangeGroupIndexButtons[1].size.y / 2f, color = Menu.Menu.MenuRGB(Menu.Menu.MenuColors.White), rotation = 90f, scale = 1.3f}
        };
        this.pages[0].Container.AddChild(this.itemsObjectChangeGroupIndexButtonIcons[0]);
        this.pages[0].Container.AddChild(this.itemsObjectChangeGroupIndexButtonIcons[1]);
        this.pages[0].subObjects.AddSafe(this.itemsObjectChangeGroupIndexButtons[0]);
        this.pages[0].subObjects.AddSafe(this.itemsObjectChangeGroupIndexButtons[1]);
        this.ChangeItemsObjectInfo("None");
    }

    public void RemoveItemsPage()
    {
        this.RemoveItemsObjectButtonIcon();
        for (int i = 0; i < this.itemsObjectChangeGroupIndexButtons.Count; i++)
        {
            if (this.itemsObjectChangeGroupIndexButtons[i] == null) { continue; }
            this.itemsObjectChangeGroupIndexButtons[i].RemoveSprites();
            this.pages[0].RemoveSubObject(this.itemsObjectChangeGroupIndexButtons[i]);
            this.itemsObjectChangeGroupIndexButtons[i] = null;
        }
        for (int i = 0; i < this.itemsObjectChangeGroupIndexButtonIcons.Count; i++) 
        { 
            if (this.itemsObjectChangeGroupIndexButtonIcons[i] == null) { continue; } 
            this.itemsObjectChangeGroupIndexButtonIcons[i].RemoveFromContainer(); 
        }
        this.RemoveItemsObjectInfo();
    }

    public void SetupCreaturesPage()
    {

    }

    public void RemoveCreaturesPage()
    {

    }

    public void SetupCraftingTablePage()
    {

    }

    public void RemoveCraftingTablePage()
    {

    }

    public void SetupAdvancementsPage()
    {

    }

    public void RemoveAdvancementsPage()
    {

    }

    public void SetupDebugPage()
    {
        this.debugCheckboxes = new();
        this.debugCheckboxes["showBodyChunks"] = new CheckBox(this, this.pages[0], this, new Vector2(150f, (manager.rainWorld.options.ScreenSize.y + 720f) / 2f - 70f), -25f, this.Translate("DEBUG_showBodyChunks"), "DEBUG_showBodyChunks", false);
        this.pages[0].subObjects.AddSafe(this.debugCheckboxes["showBodyChunks"]);
    }

    public void RemoveDebugPage()
    {
        List<string> keys = this.debugCheckboxes.Keys.ToList();
        for (int i = 0; i < keys.Count; i++)
        {
            if (this.debugCheckboxes[keys[i]] == null) { continue; }
            this.debugCheckboxes[keys[i]].RemoveSprites();
            this.pages[0].RemoveSubObject(this.debugCheckboxes[keys[i]]);
            this.debugCheckboxes[keys[i]] = null;
        }
        this.debugCheckboxes.Clear();
    }

    public void ChangeItemsObjectButtonIcon()
    {
        this.RemoveItemsObjectButtonIcon();
        this.itemsObjectButtons = new();
        for (int i = 0; i < itemsObjectGroupCount; i++)
        {
            bool flag = this.itemsObjectButtonIcons.TryGetWithIndex(this.itemsObjectGroupIndex * itemsObjectGroupCount + i, out KeyValuePair<AbstractPhysicalObject.AbstractObjectType, FSprite> kvp);
            this.itemsObjectButtons.Add(new SimpleButton(this, this.pages[0], "", $"ITEMS_itemsObjectButtons_{ (flag ? kvp.Key.value : "None") }_CHOSEN", new Vector2(150f, (manager.rainWorld.options.ScreenSize.y + 720f) / 2f - 60f * (this.itemsObjectButtons.Count + 1) - 10f), new Vector2(50f, 50f)));
            this.pages[0].subObjects.AddSafe(this.itemsObjectButtons[i]);
            if (flag) { kvp.Value.RemoveFromContainer(); this.pages[0].Container.AddChild(kvp.Value); kvp.Value.MoveToFront(); kvp.Value.x = this.itemsObjectButtons[i].pos.x + this.itemsObjectButtons[i].size.x / 2.00f; kvp.Value.y = this.itemsObjectButtons[i].pos.y + this.itemsObjectButtons[i].size.y / 2.00f; }
        }
    }

    public void RemoveItemsObjectButtonIcon()
    {
        for (int i = 0; i < this.itemsObjectButtons.Count; i++)
        {
            if (this.itemsObjectButtons[i] == null) { continue; }
            this.itemsObjectButtons[i].RemoveSprites();
            this.pages[0].RemoveSubObject(this.itemsObjectButtons[i]);
            this.itemsObjectButtons[i] = null;
        }
        for (int i = 0; i < this.itemsObjectButtonIcons.Count; i++) 
        { 
            if (this.itemsObjectButtonIcons[i].Value == null) { continue; } 
            this.itemsObjectButtonIcons[i].Value.RemoveFromContainer();
        }
        this.itemsObjectButtons.Clear();

        this.selectedItemType = "";
    }

    public void ChangeItemsObjectInfo(string itemType)
    {
        if (itemType == "" || itemType == this.selectedItemType || this.itemsObjectButtonIcons == null || (itemType != "None" && !this.itemsObjectButtonIcons.Select(x => x.Key.value).Contains(itemType))) { return; }
        this.RemoveItemsObjectInfo();
        this.itemsObjectInfoButtons = new();
        this.itemsObjectInfoButtons["objectImage"] = new SimpleButton(this, this.pages[0], "", $"ITEMS_objectImage_{itemType}_CLICK", new Vector2(210f, (manager.rainWorld.options.ScreenSize.y + 720f) / 2f - 190f), new Vector2(170f, 170f));
        this.pages[0].subObjects.AddSafe(this.itemsObjectInfoButtons["objectImage"]);
        this.itemsObjectInfoButtons["objectDescription"] = new SimpleButton(this, this.pages[0], base.Translate($"ITEMS_objectDescription_{itemType}"), $"ITEMS_objectDescription_{itemType}_CLICK", new Vector2(390f, (manager.rainWorld.options.ScreenSize.y + 720f) / 2f - 190f), new Vector2(820f, 170f));
        this.pages[0].subObjects.AddSafe(this.itemsObjectInfoButtons["objectDescription"]);
        this.selectedItemType = itemType;
    }

    public void RemoveItemsObjectInfo()
    {
        if (this.itemsObjectInfoButtons != null)
        {
            List<string> keys = new List<string>(this.itemsObjectInfoButtons.Keys);
            for (int i = 0; i < keys.Count; i++)
            {
                if (itemsObjectInfoButtons[keys[i]] == null) { continue; }
                this.itemsObjectInfoButtons[keys[i]].RemoveSprites();
                this.pages[0].RemoveSubObject(this.itemsObjectInfoButtons[keys[i]]);
                this.itemsObjectInfoButtons[keys[i]] = null;
            }
            this.itemsObjectInfoButtons.Clear();
        }
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
        if (this.uselessMessageChangeCounter > 0) { this.uselessMessageChangeCounter--; }
        try
        {
            this.playerOpen = this.game.cameras[0].followAbstractCreature.realizedCreature as Player;
            if (this.selectedObject is SimpleButton button)
            {
                this.uselessMessage.text = base.Translate($"{button.signalText}_USELESSMESSAGE");
                this.uselessMessageChangeCounter = 20;
            }
            else if (this.selectedObject == null && this.uselessMessageChangeCounter <= 0)
            {
                this.uselessMessage.text = $"{base.Translate("SCIENCEPANEL_TITLE")} {(this.playerOpen != null && this.playerOpen.room.game.IsStorySession ? "" : "?")}{base.Translate(this.playerOpen != null && !this.playerOpen.inShortcut ? Region.GetRegionFullName(this.playerOpen.room.world.name, this.playerOpen.room.game.session is StoryGameSession ? this.playerOpen.room.game.GetStorySession.saveState.saveStateNumber : this.playerOpen.playerState.slugcatCharacter) : "NRE_Region")} {base.Translate(this.playerOpen != null ? this.playerOpen.room.abstractRoom.name : "NRE_Room")}  playerNumber = {(this.playerOpen == null ? "NRE_PlayerNumber" : this.playerOpen.playerState.playerNumber)}";
                this.uselessMessageChangeCounter = 0;
            }
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
            if (message.EndsWith("_CHANGEtoPAGE"))
            {
                this.ChangePage(new Scientist.ScientistPanel.PageModeIndex(message.Substring(0, message.Length - 13).ToLower().FirstCharToUpper()));
            }
            else if (message.StartsWith("ITEMS_itemsObjectGroupIndex"))
            {
                int tmp = (int)Mathf.Ceil(itemsObjectButtonIcons.Count / (itemsObjectGroupCount * 1.00f));
                this.itemsObjectGroupIndex = Mathf.Max(0, this.itemsObjectGroupIndex + (message.EndsWith("_LEFT") ? -1 : 1) + tmp) % tmp;
                this.ChangeItemsObjectButtonIcon();
            }
            else if (message.StartsWith("ITEMS_itemsObjectButtons_") && message.EndsWith("_CHOSEN"))
            {
                string itemType = message.Substring(25, message.Length - 32);
                this.ChangeItemsObjectInfo(itemType);
            }
        }
        this.WarpSignal(sender, message);
    }

    public void SetVisible(bool visible)
    {
        this.isShowing = visible;
        this.container.isVisible = visible;
        if (visible) { this.container.MoveToFront(); base.currentPage = 0; }
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
        if (this.pageMode == Scientist.ScientistPanel.PageModeIndex.None) { }
        else if (this.pageMode == Scientist.ScientistPanel.PageModeIndex.Items) { this.RemoveItemsPage(); }
        else if (this.pageMode == Scientist.ScientistPanel.PageModeIndex.Creatures) { this.RemoveCreaturesPage(); }
        else if (this.pageMode == Scientist.ScientistPanel.PageModeIndex.CraftingTable) { this.RemoveCraftingTablePage(); }
        else if (this.pageMode == Scientist.ScientistPanel.PageModeIndex.Advancements) { this.RemoveAdvancementsPage(); }
        else if (this.pageMode == Scientist.ScientistPanel.PageModeIndex.Debug) { this.RemoveDebugPage(); }
        else { ScientistLogger.Log("Invalid page mode: " + mode); }
        //this.SetupChangePageButton();
        if (mode == Scientist.ScientistPanel.PageModeIndex.None) { }
        else if (mode == Scientist.ScientistPanel.PageModeIndex.Items) { this.SetupItemsPage(); }
        else if (mode == Scientist.ScientistPanel.PageModeIndex.Creatures) { this.SetupCreaturesPage(); }
        else if (mode == Scientist.ScientistPanel.PageModeIndex.CraftingTable) { this.SetupCraftingTablePage(); }
        else if (mode == Scientist.ScientistPanel.PageModeIndex.Advancements) { this.SetupAdvancementsPage(); }
        else if (mode == Scientist.ScientistPanel.PageModeIndex.Debug) { this.SetupDebugPage(); }
        else { return; }
        this.pageMode = mode;
    }

    public bool GetChecked(CheckBox box)
    {
        if (box.IDString.StartsWith("DEBUG_"))
        {
            FieldInfo[] fields = typeof(Scientist.Data.DebugVariables).GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (FieldInfo field in fields)
            {
                bool? b = field.GetValue(null) as bool?;
                if (b != null && field.Name == box.IDString.Replace("DEBUG_", ""))
                {
                    return b.Value;
                }
            }
        }
        return false;
    }

    public void SetChecked(CheckBox box, bool c)
    {
        if (box.IDString.StartsWith("DEBUG_"))
        {
            FieldInfo[] fields = typeof(Scientist.Data.DebugVariables).GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (FieldInfo field in fields)
            {
                bool? b = field.GetValue(null) as bool?;
                if (b != null && field.Name == box.IDString.Replace("DEBUG_", ""))
                {
                    field.SetValue(null, c);
                    Scientist.Data.DebugVariables.changed = true;
                }
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

        //public static readonly Scientist.ScientistPanel.PageModeIndex Main = new Scientist.ScientistPanel.PageModeIndex("Main", true);
        public static readonly Scientist.ScientistPanel.PageModeIndex None = new Scientist.ScientistPanel.PageModeIndex("None", true);
        public static readonly Scientist.ScientistPanel.PageModeIndex Items = new Scientist.ScientistPanel.PageModeIndex("Items", true);
        public static readonly Scientist.ScientistPanel.PageModeIndex Creatures = new Scientist.ScientistPanel.PageModeIndex("Creatures", true);
        public static readonly Scientist.ScientistPanel.PageModeIndex CraftingTable = new Scientist.ScientistPanel.PageModeIndex("Craftingtable", true);
        public static readonly Scientist.ScientistPanel.PageModeIndex Advancements = new Scientist.ScientistPanel.PageModeIndex("Advancements", true);
        public static readonly Scientist.ScientistPanel.PageModeIndex Debug = new Scientist.ScientistPanel.PageModeIndex("Debug", true);
    }
}
