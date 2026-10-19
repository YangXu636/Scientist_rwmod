using MonoMod.RuntimeDetour;
using System;
using System.Reflection;
using UnityEngine;

namespace Scientist.ScientistHooks;

/// <summary>ImprovedInput（按键设置）软依赖：注册"打开科学家面板"的按键，并保持配置与按键同步。</summary>
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
        OpenSpIiKeybind = iiPlayerKeybind.GetMethod("Register", BindingFlags.Public | BindingFlags.Static, null,
            new Type[] { typeof(string), typeof(string), typeof(string), typeof(KeyCode), typeof(KeyCode) }, null)
            .Invoke(null, new object[] { $"{Scientist.ScientistPlugin.MOD_ID}:openspiikeybind", "Scientist", "Open Scientist Panel", Scientist.ScientistPlugin.OpenSpKeycode, KeyCode.None });
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
