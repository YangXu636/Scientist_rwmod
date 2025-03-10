using System;
using System.Reflection;
using BepInEx.Logging;
using Menu.Remix.MixedUI;
using MonoMod.Utils;
using UnityEngine;

namespace Scientist
{
    public class ScientistOptions : OptionInterface
    {
        private bool _initializing = false;
        private readonly ManualLogSource Logger;
        public readonly Configurable<bool> EnableOldPf;
        public readonly Configurable<KeyCode> OpenScientistPanelKey;
        private UIelement[] UIArrPlayerOptions_Sundries;

        public ScientistOptions(ScientistPlugin modInstance, ManualLogSource loggerSource)
        {
            this.Logger = loggerSource;
            this.EnableOldPf = this.config.Bind("ScientistEnableOldPf", false);
            this.OpenScientistPanelKey = this.config.Bind("ScientistOpenScientistPanelKey", ScientistPlugin.OpenSpKeycode);
        }

        // Token: 0x06000020 RID: 32 RVA: 0x00004D44 File Offset: 0x00002F44
        public override void Initialize()
        {
            _initializing = true;
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
                    new OpLabel(40f, 520f, OptionInterface.Translate("Enable old version PainlessFruit"), false) { verticalAlignment = OpLabel.LabelVAlignment.Center },
                    new OpKeyBinder(this.OpenScientistPanelKey, new Vector2(10f, 480f), new Vector2(150f, 30f), true, OpKeyBinder.BindController.AnyController),
                    new OpLabel(166f, 480f, OptionInterface.Translate("Key used for opening the menu"), false) { verticalAlignment = OpLabel.LabelVAlignment.Center }
                };
                opTab.AddItems(this.UIArrPlayerOptions_Sundries);
            }
            catch (Exception ex)
            {
                this.Logger.LogInfo(ex);
                ex.LogDetailed();
            }
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
    }
}
