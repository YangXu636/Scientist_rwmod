using MonoMod.RuntimeDetour;
using System;
using System.Reflection;

namespace Scientist.ScientistHooks;

/// <summary>FakeAchievements（假成就）软依赖：通过反射触发本 mod 的自定义成就。</summary>
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
