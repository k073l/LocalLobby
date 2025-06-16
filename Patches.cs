using HarmonyLib;

#if MONO
using ScheduleOne.DevUtilities;
#else
using Il2CppScheduleOne.DevUtilities;
#endif

namespace LocalLobby;

[HarmonyPatch(typeof(Settings), nameof(Settings.ApplyDisplaySettings))]
public static class SettingsApplyDisplaySettingsPatch
{
    public static bool Prefix(Settings __instance)
    {
        var isNetworkMode = ArgParser.IsHost || ArgParser.IsClient;

        if (!isNetworkMode || !ArgParser.ShouldSetWindowPositionSize)
            return true;

        ArgParser.SetWindowPositionSize();
        return false;
    }
}