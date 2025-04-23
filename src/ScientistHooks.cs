using Scientist.Items;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.PlayerLoop;
using UnityEngine;
using ImprovedInput;
using System.Runtime.CompilerServices;
using BeastMasters;

namespace Scientist.ScientistHooks;

public static class BeastMasterHooks
{
    public static Hook hook;

    public static FieldInfo bmsInfo = typeof(BeastMaster).GetField("BMSInstance", BindingFlags.Public | BindingFlags.Instance);

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
        var bms = (BeastMaster)bmsInfo.GetValue(self);

        BeastMaster.RadialItemMenu radialItemMenu = new BeastMaster.RadialItemMenu
        {
            parent = bms.itemMenu,
            bms = bms
        };
        bms.itemMenu.subMenus.Add(radialItemMenu);
        radialItemMenu.iconName = "Symbol_ScientistIcon";
        Type type = typeof(Enums.Items);
        FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);
        foreach (FieldInfo field in fields)
        {
            AbstractPhysicalObject.AbstractObjectType apo = field.GetValue(null) as AbstractPhysicalObject.AbstractObjectType;
            if (apo != null)
            {
                radialItemMenu.items.Add(apo);
            }
        }
        for (int i = 0; i < radialItemMenu.items.Count; i++)
        {
            radialItemMenu.itemData.Add(0);
        }
    }
}

public static class ImprovedInputHooks
{
    public static Hook hook;

    public static Assembly iiAssembly;
    public static Type iiPlayerKeybind;
    public static object OpenSpIiKeybind;

    public static void HookOn(object iiInstance)
    {
        iiAssembly = iiInstance.GetType().Assembly;
        iiPlayerKeybind = iiAssembly.GetType("ImprovedInput.PlayerKeybind");
        OpenSpIiKeybind = iiPlayerKeybind.GetMethod("Register", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string), typeof(string), typeof(string), typeof(KeyCode), typeof(KeyCode) }, null).Invoke(null, new object[] { $"{Scientist.ScientistPlugin.MOD_ID}:openspiikeybind", "Scientist", "Open Scientist Panel", Scientist.ScientistPlugin.OpenSpKeycode, KeyCode.None });
    }

    public static void HookOff()
    {
        if (hook == null)
        {
            Scientist.ScientistLogger.Warning("ImprovedInputInit hook is not active.");
            return;
        }
        hook.Dispose();
    }

    public static void HookReapply()
    {
        if (hook == null)
        {
            Scientist.ScientistLogger.Warning("ImprovedInputInit hook is not active.");
            return;
        }
        try
        {
            hook.Dispose();
        }
        catch (Exception ex)
        {
            Scientist.ScientistLogger.Warning("Failed to dispose ImprovedInputInit hook: " + ex.Message);
        }
        try
        {
            hook.Apply();
        }
        catch (Exception ex)
        {
            Scientist.ScientistLogger.Error("Failed to apply ImprovedInputInit hook: " + ex.Message);
        }
    }

    public static void ChangedOspKeycode()
    {

    }
}

public static class FakeAchievementsHooks
{
    public static Hook hook;

    public static Assembly faAssembly;
    public static Type faAchievementsManager;

    public static void HookOn(object iiInstance)
    {
        faAssembly = iiInstance.GetType().Assembly;
        faAchievementsManager = faAssembly.GetType("FakeAchievements.AchievementsManager");
    }

    public static void HookOff()
    {
        if (hook == null)
        {
            Scientist.ScientistLogger.Warning("FakeAchievementsInit hook is not active.");
            return;
        }
        hook.Dispose();
    }

    public static void HookReapply()
    {
        if (hook == null)
        {
            Scientist.ScientistLogger.Warning("FakeAchievementsInit hook is not active.");
            return;
        }
        try
        {
            hook.Dispose();
        }
        catch (Exception ex)
        {
            Scientist.ScientistLogger.Warning("Failed to dispose FakeAchievementsInit hook: " + ex.Message);
        }
        try
        {
            hook.Apply();
        }
        catch (Exception ex)
        {
            Scientist.ScientistLogger.Error("Failed to apply FakeAchievementsInit hook: " + ex.Message);
        }
    }

    public static void ShowAchievement(string name)
    {
        faAchievementsManager.GetMethod("ShowAchievement", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string) }, null).Invoke(null, new object[] { $"{Scientist.ScientistPlugin.MOD_ID}/{name}" });
    }
}