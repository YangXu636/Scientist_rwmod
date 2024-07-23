/*using System;
using BepInEx.Logging;
using Menu.Remix.MixedUI;
using UnityEngine;

namespace Scientist
{
    public class ScientistOptions : OptionInterface
    {

        private readonly ManualLogSource Logger;
        public readonly Configurable<bool> Power;
        //public readonly Configurable<KeyCode> OpenMenuKey;
        private UIelement[] UIArrPlayerOptions;

        public ScientistOptions(Plugin modInstance, ManualLogSource loggerSource)
        {
            this.Logger = loggerSource;
            this.Power = this.config.Bind("Power", false);
            //this.OpenMenuKey = this.config.Bind<KeyCode>("OpenMenuKey", (KeyCode)98);
        }

        // Token: 0x06000020 RID: 32 RVA: 0x00004D44 File Offset: 0x00002F44
        public override void Initialize()
        {
            try
            {
                OpTab opTab = new OpTab(this, "Options");
                this.Tabs = new OpTab[]
                {
                    opTab
                };
                this.UIArrPlayerOptions = new UIelement[]
                {
                    new OpLabel(10f, 550f, "Options", true),
                    new OpCheckBox(this.Power, 10f, 520f),
                    new OpLabel(40f, 520f, "Powerful slugcat", false)
                    {
                        verticalAlignment = OpLabel.LabelVAlignment.Center
                    }
                    *//*new OpKeyBinder(this.OpenMenuKey, new Vector2(10f, 480f), new Vector2(150f, 30f), true, OpKeyBinder.BindController.AnyController),
                    new OpLabel(166f, 480f, "Key used for opening the menu", false)
                    {
                        verticalAlignment = OpLabel.LabelVAlignment.Center
                    }*//*
                };
                opTab.AddItems(this.UIArrPlayerOptions);
            }
            catch (Exception ex)
            {
                this.Logger.LogInfo(ex);
            }
        }

        // Token: 0x06000021 RID: 33 RVA: 0x00004E4C File Offset: 0x0000304C
        public override void Update()
        {
        }
    }
}
*/