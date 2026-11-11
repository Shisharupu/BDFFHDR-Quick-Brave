using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;

namespace QuickBrave;

[BepInPlugin(PluginInfo.GUID, PluginInfo.NAME, PluginInfo.VERSION)]
public class Plugin : BasePlugin
{
    internal static new ManualLogSource Log;

    public override void Load()
    {
        Log = base.Log;
        Log.LogInfo($"Hello from {PluginInfo.NAME} v{PluginInfo.VERSION}!");

        // Harmony will auto-discover all [HarmonyPatch] classes in this assembly
        var harmony = new Harmony(PluginInfo.GUID);
        harmony.PatchAll();

        Log.LogInfo("Patches applied. Controls: RT / F = Brave x3 (turn character).");
    }
}

internal static class PluginInfo
{
    public const string GUID    = "com.bdffhd.quickbrave";
    public const string NAME    = "QuickBrave";
    public const string VERSION = "1.0.0";
}
