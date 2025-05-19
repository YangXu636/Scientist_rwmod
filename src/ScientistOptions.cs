using System;
using System.Reflection;
using BepInEx.Logging;
using Menu.Remix.MixedUI;
using MonoMod.Utils;
using Scientist.Chats;
using UnityEngine;

namespace Scientist
{
    public class ScientistOptions : OptionInterface
    {
        private bool _initializing = false;
        private readonly ManualLogSource Logger;
        public readonly Configurable<bool> EnableOldPf;                 //pf = painless fruit
        public readonly Configurable<bool> EnableTfKeepShaking;         //tf = trembling fruit
        public readonly Configurable<bool> EnableOpenPanelPauseGame;
        public readonly Configurable<KeyCode> OpenScientistPanelKey;
        public readonly Configurable<bool> UnlockAllContent;
        public readonly Configurable<bool> EnableDebug;
        private UIelement[] UIArrPlayerOptions_Sundries;
        public Color cheatColor = new Color(0.85f, 0.35f, 0.4f);

        public ScientistOptions(ScientistPlugin modInstance, ManualLogSource loggerSource)
        {
            this.Logger = loggerSource;
            this.EnableOldPf = this.config.Bind("ScientistEnableOldPf", false);
            this.EnableTfKeepShaking = this.config.Bind("ScientistEnableTfKeepShaking", false);
            this.EnableOpenPanelPauseGame = this.config.Bind("ScientistEnableOpenPanelPauseGame", true);
            this.OpenScientistPanelKey = this.config.Bind("ScientistOpenScientistPanelKey", ScientistPlugin.OpenSpKeycode);
            this.UnlockAllContent = this.config.Bind("ScientistUnlockAllContent", false);
            this.EnableDebug = this.config.Bind("ScientistEnableDebug", false);

            this.OnConfigChanged += ScientistOptions_OnConfigChanged;
        }

        // Token: 0x06000020 RID: 32 RVA: 0x00004D44 File Offset: 0x00002F44
        public override void Initialize()
        {
            try
            {
                OpTab opTab = new(this, OptionInterface.Translate("Sundries"));
                this.Tabs = new OpTab[]
                {
                    opTab
                };
                this.UIArrPlayerOptions_Sundries = new UIelement[]
                {
                    new OpLabel(10f, 550f, OptionInterface.Translate("Sundries"), true),
                    new OpCheckBox(this.EnableOldPf, 10f, 520f),
                    new OpLabel(40f, 520f, OptionInterface.Translate("UIArrPlayerOptions_Sundries_EnableOldPf"), false) { verticalAlignment = OpLabel.LabelVAlignment.Center },
                    new OpCheckBox(this.EnableTfKeepShaking, 10f, 480f),
                    new OpLabel(40f, 480f, OptionInterface.Translate("UIArrPlayerOptions_Sundries_EnableTfKeepShaking"), false) { verticalAlignment = OpLabel.LabelVAlignment.Center },
                    new OpCheckBox(this.EnableOpenPanelPauseGame, 10f, 440f),
                    new OpLabel(40f, 440f, OptionInterface.Translate("UIArrPlayerOptions_Sundries_EnableOpenPanelPauseGame"), false) { verticalAlignment = OpLabel.LabelVAlignment.Center },
                    new OpKeyBinder(this.OpenScientistPanelKey, new Vector2(10f, 400f), new Vector2(150f, 30f), true, OpKeyBinder.BindController.AnyController),
                    new OpLabel(166f, 400f, OptionInterface.Translate("UIArrPlayerOptions_Sundries_OpenScientistPanelKey"), false) { verticalAlignment = OpLabel.LabelVAlignment.Center },
                    new OpCheckBox(this.UnlockAllContent, 10f, 360f) { colorEdge = this.cheatColor },
                    new OpLabel(40f, 360f, OptionInterface.Translate("UIArrPlayerOptions_Sundries_UnlockAllContent"), false) { verticalAlignment = OpLabel.LabelVAlignment.Center, color = this.cheatColor },
                    new OpCheckBox(this.EnableDebug, 10f, 320f),
                    new OpLabel(40f, 320f, OptionInterface.Translate("UIArrPlayerOptions_Sundries_EnableDebug"), false) { verticalAlignment = OpLabel.LabelVAlignment.Center },
                };
                opTab.AddItems(this.UIArrPlayerOptions_Sundries);
            }
            catch (Exception ex)
            {
                this.Logger.LogInfo(ex);
                ex.LogDetailed();
            }
            this.SynchronousVariable();
            _initializing = true;
        }

        private void ScientistOptions_OnConfigChanged()
        {
            if (ScientistPlugin.hookedOn.KeyIsValue("ImprovedInput", true))
            {
                FieldInfo OpenSpIiKeybindFieldinfo = ScientistHooks.ImprovedInputHooks.OpenSpIiKeybind.GetType().GetField("keyboard", BindingFlags.Instance | BindingFlags.NonPublic);
                ScientistPlugin.OspKeycodeChangedByConfig = true;
                KeyCode[] oldOspikKeycode = (KeyCode[])OpenSpIiKeybindFieldinfo.GetValue(ScientistHooks.ImprovedInputHooks.OpenSpIiKeybind);
                if (this.OpenScientistPanelKey.Value != oldOspikKeycode[0] && !_initializing)
                {
                    KeyCode[] newOspikKeycode = new KeyCode[16];
                    newOspikKeycode.SetAll(this.OpenScientistPanelKey.Value);
                    OpenSpIiKeybindFieldinfo.SetValue(ScientistHooks.ImprovedInputHooks.OpenSpIiKeybind, newOspikKeycode);
                }
                if (_initializing)
                {
                    this.OpenScientistPanelKey.Value = oldOspikKeycode[0];
                    _initializing = false;
                }
            }
            this.SynchronousVariable();
            ScientistLogger.Log("ScientistOptions.ScientistOptions_OnConfigChanged run");
        }

        // Token: 0x06000021 RID: 33 RVA: 0x00004E4C File Offset: 0x0000304C
        public override void Update()
        {
        }

        public void ChangedInConfig()
        {
            if (ScientistPlugin.hookedOn.KeyIsValue("ImprovedInput", true))
            {
                FieldInfo OpenSpIiKeybindFieldinfo = ScientistHooks.ImprovedInputHooks.OpenSpIiKeybind.GetType().GetField("keyboard", BindingFlags.Instance | BindingFlags.NonPublic);
                ScientistPlugin.OspKeycodeChangedByConfig = true;
                KeyCode[] oldOspikKeycode = (KeyCode[])OpenSpIiKeybindFieldinfo.GetValue(ScientistHooks.ImprovedInputHooks.OpenSpIiKeybind);
                if (this.OpenScientistPanelKey.Value != oldOspikKeycode[0] && !_initializing)
                {
                    KeyCode[] newOspikKeycode = new KeyCode[16];
                    newOspikKeycode.SetAll(this.OpenScientistPanelKey.Value);
                    OpenSpIiKeybindFieldinfo.SetValue(ScientistHooks.ImprovedInputHooks.OpenSpIiKeybind, newOspikKeycode);
                }
                if (_initializing)
                {
                    this.OpenScientistPanelKey.Value = oldOspikKeycode[0];
                    _initializing = false;
                }
            }
        }

        public void SynchronousVariable()
        {
            Scientist.Data.GolbalVariables.SEnableOldPf = this.EnableOldPf.Value;
            Scientist.Data.GolbalVariables.SEnableTfKeepShaking = this.EnableTfKeepShaking.Value;
            Scientist.Data.GolbalVariables.SEnableOpenPanelPauseGame = this.EnableOpenPanelPauseGame.Value;
            Scientist.Data.GolbalVariables.SOpenScientistPanelKey = this.OpenScientistPanelKey.Value;
            Scientist.Data.GolbalVariables.SUnlockAllContent = this.UnlockAllContent.Value;
            Scientist.Data.GolbalVariables.SEnableDebug = this.EnableDebug.Value;
        }
    }
}
