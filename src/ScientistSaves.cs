using SlugBase.DataTypes;
using System;
using System.Collections.Generic;
using UnityEngine;
using EmgTx;
using System.Text.RegularExpressions;

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
            string[] cres = Regex.Split(data, "<creA>");
            if (cres.Length != 4) return;
            if (cres[1].Length == ScientistSlugcat.craftingGrid_ObjectsOnly.GetLength(0) * ScientistSlugcat.craftingGrid_ObjectsOnly.GetLength(1))
            {
                for (int i = 0; i < ScientistSlugcat.craftingGrid_ObjectsOnly.GetLength(0); i++)
                {
                    for (int j = 0; j < ScientistSlugcat.craftingGrid_ObjectsOnly.GetLength(1); j++)
                    {
                        ScientistSlugcat.craftingGrid_ObjectsOnly[i, j].enabled = cres[1][i * ScientistSlugcat.craftingGrid_ObjectsOnly.GetLength(1) + j] == '1';
                    }
                }
            }
            if (cres[2].Length == ScientistSlugcat.craftingGrid_CritterObjects.GetLength(0) * ScientistSlugcat.craftingGrid_CritterObjects.GetLength(1))
            {
                for (int i = 0; i < ScientistSlugcat.craftingGrid_CritterObjects.GetLength(0); i++)
                {
                    for (int j = 0; j < ScientistSlugcat.craftingGrid_CritterObjects.GetLength(1); j++)
                    {
                        ScientistSlugcat.craftingGrid_CritterObjects[i, j].enabled = cres[2][i * ScientistSlugcat.craftingGrid_CritterObjects.GetLength(1) + j] == '1';
                    }
                }
            }
            if (cres[3].Length == ScientistSlugcat.craftingGrid_CrittersOnly.GetLength(0) * ScientistSlugcat.craftingGrid_CrittersOnly.GetLength(1))
            {
                for (int i = 0; i < ScientistSlugcat.craftingGrid_CrittersOnly.GetLength(0); i++)
                {
                    for (int j = 0; j < ScientistSlugcat.craftingGrid_CrittersOnly.GetLength(1); j++)
                    {
                        ScientistSlugcat.craftingGrid_CrittersOnly[i, j].enabled = cres[3][i * ScientistSlugcat.craftingGrid_CrittersOnly.GetLength(1) + j] == '1';
                    }
                }
            }
        }

        public override string SaveToString(bool saveAsIfPlayerDied, bool saveAsIfPlayerQuit)
        {
            if (saveAsIfPlayerDied || saveAsIfPlayerQuit) return base.origSaveData;
            else
            {
                string cre = "<creA>";
                foreach (ScientistSlugcat.CraftDat dat in ScientistSlugcat.craftingGrid_ObjectsOnly)
                {
                    cre += dat.enabled ? '1' : '0';
                }
                cre += "<creA>";
                foreach (ScientistSlugcat.CraftDat dat in ScientistSlugcat.craftingGrid_CritterObjects)
                {
                    cre += dat.enabled ? '1' : '0';
                }
                cre += "<creA>";
                foreach (ScientistSlugcat.CraftDat dat in ScientistSlugcat.craftingGrid_CrittersOnly)
                {
                    cre += dat.enabled ? '1' : '0';
                }
                return cre;
            }
        }

        public override void ClearDataForNewSaveState(SlugcatStats.Name newSlugName)
        {
            base.ClearDataForNewSaveState(newSlugName);
            base.origSaveData = "<creA>1111101011111111111111111111110101110101111111111111111111110011011010111111111111111111111001110101011111111111111111111100111110101111111111111111111110000000000000000000000000000000001111100111111111111111111111100000000100000000000000000000000011111010011111111111111111111001111101010111111111111111111100111110101101111111111111111110011111010111011111111111111111001111101011110111111111111111100111110101111101111111111111110011111010111111011111111111111001111101011111110111111111111100111110101111111101111111111111011111010111111111011111111111001111101011111111110111111111100111110101111111111101111111110011111010111111111111011111111001111101011111111111110111111100111110101111111111111101111110011111010111111111111111011111001111101011111111111111110111100111110101111111111111111101110011111010111111111111111111011001111101011111111111111111110111111110101111111111111111111100010000000000000001000000000010000000000000000000000000000001000<creA>11111010111111111111111111111001111101011111111111111111111100111110101111111111111111111110011111010111111111111111111111001111101011111111111111111111100<creA>0111110111110111110111110";
        }

        public override string ToString()
        {
            return base.ToString() + " load:ScientistSaves.CraftingTableEnabledState";
        }
    }
}
