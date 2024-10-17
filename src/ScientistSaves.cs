using SlugBase.DataTypes;
using System;
using System.Collections.Generic;
using UnityEngine;
using EmgTx;

namespace Scientist;

public class ScientistSaves
{
    public class CraftingTableEnabledState : CustomSaveTx.DeathPersistentSaveDataTx
    {
        public CraftingTableEnabledState() : base(new SlugcatStats.Name(Scientist.Plugin.MOD_ID))
        {

        }

        public override string header => "XUYANGJERRY_SCIENTIST_CRAFTINGTABLEENABLEDSTATE";

        public override void LoadDatas(string data)
        {
            base.LoadDatas(data);

            loadThisForHowManyTimes = int.Parse(data);
        }

        public override string SaveToString(bool saveAsIfPlayerDied, bool saveAsIfPlayerQuit)
        {
            if (saveAsIfPlayerDied || saveAsIfPlayerQuit) return base.origSaveData;
            else
            {
                string oo = "<oaoA>"
            }
        }

        public override void ClearDataForNewSaveState(SlugcatStats.Name newSlugName)
        {
            base.ClearDataForNewSaveState(newSlugName);
            loadThisForHowManyTimes = 0;
        }

        public override string ToString()
        {
            return base.ToString() + " loadThisForHowManyTimes:" + loadThisForHowManyTimes.ToString();
        }
    }
}
