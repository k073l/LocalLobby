using MelonLoader;
using UnityEngine;

namespace LocalLobby;

public class ArgParser(LocalLobby mod)
{
    public bool AutoLoad = false;
    public int SaveSlot = 0;
    public static bool IsHost = false;
    public static bool IsClient = false;
    public static bool ShouldSetWindowPositionSize = false;
    public static int LeftOffset = 0;

    private static readonly MelonLogger.Instance Logger = new("LocalLobby.ArgParser");
    
    public void ParseArguments()
    {
        var args = Environment.GetCommandLineArgs();
        var isHost = Array.Exists(args, a => a == "--host");

        if (Array.Exists(args, a => a == "--autoload"))
        {
            AutoLoad = true;
            Logger.Msg("Auto-load enabled. Will load save slot 0 on lobby join.");
        }
        
        var saveSlotArg = GetArgValue("--saveslot");
        if (int.TryParse(saveSlotArg, out var saveSlot) && saveSlot is >= 0 and < 5)
        {
            AutoLoad = true;
            SaveSlot = saveSlot;
            Logger.Msg($"Save slot set to {SaveSlot}.");
        }
        else if (saveSlotArg != null)
        {
            Logger.Error($"Invalid save slot: {saveSlotArg}. Must be a non-negative integer.");
        }

        if (isHost)
        {
            Console.Title += " (HOST)";
            IsHost = true;
        }
        else if (Array.Exists(args, a => a == "--client") ||
                 Array.Exists(args, a => a == "--join"))
        {
            Console.Title += " (CLIENT)";
            IsClient = true;
        }

        if (Array.Exists(args, a => a == "--adjust-window"))
        {
            ShouldSetWindowPositionSize = true;
            var leftOffsetArg = GetArgValue("--left-offset");
            if (int.TryParse(leftOffsetArg, out var offset) && offset >= 0)
            {
                LeftOffset = offset;
                Logger.Msg($"Left offset set to {LeftOffset} pixels.");
            }
            else if (leftOffsetArg != null)
            {
                Logger.Error($"Invalid left offset: {leftOffsetArg}. Must be a non-negative integer.");
            }
            SetWindowPositionSize();
            Logger.Msg("Window position adjustment enabled.");
        }

        if (IsHost)
        {
            mod.CreateLobby();
        }
        else if (IsClient)
        {
            Logger.Msg("Client mode. Waiting for lobby.txt...");
            MelonCoroutines.Start(mod.WaitForLobbyAndJoin());
        }
    }

    public static void SetWindowPositionSize()
    {
        var offsetLeft = Math.Max(0, LeftOffset);
        
        var mainDisplay = Screen.mainWindowDisplayInfo;
        var workArea = mainDisplay.workArea;
        
        if (offsetLeft > workArea.width)
        {
            Logger.Warning("Left offset is greater than the width of the screen, cancelling window adjustment");
            return;
        }

        workArea.width -= offsetLeft;
        var windowSize = new Vector2Int(workArea.width / 2, workArea.height);
        var position = Vector2Int.zero;

        if (ArgParser.IsHost)
            position.x = offsetLeft;
        else if (ArgParser.IsClient)
            position.x = (workArea.width / 2) + offsetLeft;

        Screen.fullScreenMode = FullScreenMode.Windowed;
        Screen.fullScreen = false;
        Screen.SetResolution(windowSize.x, windowSize.y, fullscreen: false);
        var mainDisplayInfo = (DisplayInfo)mainDisplay;
#if MONO
        Screen.MoveMainWindowTo(mainDisplayInfo, position);
#else
        Screen.MoveMainWindowTo(ref mainDisplayInfo, position);
#endif
        Logger.Msg($"Window position set to {position.x}, {position.y} with size {windowSize.x}x{windowSize.y}.");
    }
    
    private static string GetArgValue(string key)
    {
        var args = Environment.GetCommandLineArgs();
        for (var i = 0; i < args.Length; i++)
        {
            if (args[i] == key && i + 1 < args.Length)
                return args[i + 1];
        }

        return null;
    }
}