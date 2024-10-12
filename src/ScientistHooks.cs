﻿using items;
using MonoMod.RuntimeDetour;
using System;
using System.Reflection;

namespace Scientist.ScientistHooks;

public static class BeastMasterHooks
{
    public static Hook hook;

    public static FieldInfo bmsInfo = typeof(BeastMaster.BeastMaster).GetField("BMSInstance", BindingFlags.Public | BindingFlags.Instance);

    public static void HookOn(object bmsInstance)
    {
        bmsInfo = bmsInstance.GetType().GetField("BMSInstance", BindingFlags.Static | BindingFlags.Public);

        hook = new Hook(
            bmsInstance.GetType().GetMethod("BeastMasterInit", BindingFlags.Instance | BindingFlags.Public), 
            typeof(Scientist.ScientistHooks.BeastMasterHooks).GetMethod("BeastMaster_BeastMasterInit", BindingFlags.Static | BindingFlags.Public)
            );
    }

    public static void HookOff()
    {
        if (hook == null)
        {
            Scientist.ScientistLogger.Warning("BeastMasterInit hook is not active.");
            return;
        }
        hook.Dispose();
    }

    public static void HookReapply()
    {
        if (hook == null)
        {
            Scientist.ScientistLogger.Warning("BeastMasterInit hook is not active.");
            return;
        }
        try
        {
            hook.Dispose();
        }
        catch (Exception ex)
        {
            Scientist.ScientistLogger.Warning("Failed to dispose BeastMasterInit hook: " + ex.Message);
        }
        try
        {
            hook.Apply();
        }
        catch (Exception ex)
        {
            Scientist.ScientistLogger.Error("Failed to apply BeastMasterInit hook: " + ex.Message);
        }
    }

    public static void BeastMaster_BeastMasterInit(Action<object> orig, object self)
    {
        orig(self);
        var bms = (BeastMaster.BeastMaster)bmsInfo.GetValue(self);

        BeastMaster.BeastMaster.RadialItemMenu radialItemMenu = new BeastMaster.BeastMaster.RadialItemMenu
        {
            parent = bms.itemMenu,
            bms = bms
        };
        bms.itemMenu.subMenus.Add(radialItemMenu);
        radialItemMenu.iconName = ItemSymbol.SpriteNameForItem(Scientist.Register.ScientistIcon, 0);
        radialItemMenu.items.Add(Scientist.Register.SharpSpear);
        radialItemMenu.items.Add(Scientist.Register.ConcentratedDangleFruit);
        radialItemMenu.items.Add(Scientist.Register.PainlessFruit);
        radialItemMenu.items.Add(Scientist.Register.ColorfulFruit);
        radialItemMenu.items.Add(Scientist.Register.InflatableGlowingShield);
        radialItemMenu.items.Add(Scientist.Register.AnesthesiaSpear);
        radialItemMenu.items.Add(Scientist.Register.AnesthesiaNeedle);
        for (int i = 0; i < radialItemMenu.items.Count; i++)
        {
            radialItemMenu.itemData.Add(0);
        }
    }
}