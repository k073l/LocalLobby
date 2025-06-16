using System.Collections;
using MelonLoader;
using MelonLoader.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

#if MONO
using ScheduleOne.Persistence;
using Steamworks;

#else
using Il2CppScheduleOne.Persistence;
using Il2CppSteamworks;
#endif

[assembly: MelonInfo(
    typeof(LocalLobby.LocalLobby),
    LocalLobby.BuildInfo.Name,
    LocalLobby.BuildInfo.Version,
    LocalLobby.BuildInfo.Author
)]
[assembly: MelonColor(1, 255, 0, 0)]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace LocalLobby;

public static class BuildInfo
{
    public const string Name = "LocalLobby";
    public const string Description = "Allows for local Steam lobbies, requires Goldberg Emu";
    public const string Author = "k073l";
    public const string Version = "1.0.0";
}

public class LocalLobby : MelonMod
{
    private static readonly MelonLogger.Instance Logger = new("LocalLobby");
    private ArgParser _args;
    private CSteamID _lobbyId;

    private static readonly string SharedLobbyFile =
        Path.Combine(MelonEnvironment.UserDataDirectory, "LocalLobby", "lobby.txt");

    public bool Initialized = false;
    
    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        if (sceneName != "Menu") return;
        if (Object.FindObjectOfType<AudioComponent>() != null) return;
        var go2 = new GameObject("LocalLobbyAudioController");
        go2.AddComponent<AudioComponent>();
        Object.DontDestroyOnLoad(go2);
    }
    
    public override void OnApplicationQuit()
    {
        if (File.Exists(SharedLobbyFile))
        {
            File.Delete(SharedLobbyFile);
            MelonLogger.Msg("Deleted lobby file: " + SharedLobbyFile);
        }

        if (_lobbyId.IsValid())
        {
            SteamMatchmaking.LeaveLobby(_lobbyId);
            MelonLogger.Msg("Left lobby: " + _lobbyId);
        }

        SteamAPI.Shutdown();
    }

    public override void OnUpdate()
    {
        if (Initialized) return;

        if (!SteamAPI.Init())
        {
            MelonLogger.Msg("Waiting for SteamAPI...");
            return;
        }

        var steamId = SteamUser.GetSteamID();
        var appId = SteamUtils.GetAppID();

        MelonLogger.Msg($"SteamID: {steamId.m_SteamID}");
        MelonLogger.Msg($"AppID: {appId.m_AppId}");
        MelonLogger.Msg("Steam API DLL path: " + typeof(SteamAPI).Assembly.Location);

        _args = new ArgParser(this);
        _args.ParseArguments();

        Initialized = true;
    }


    public void CreateLobby()
    {
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, 2);
        Callback<LobbyCreated_t>.Create((Callback<LobbyCreated_t>.DispatchDelegate)OnLobbyCreated);
        MelonLogger.Msg("Creating lobby...");
    }

    private void OnLobbyCreated(LobbyCreated_t result)
    {
        if (result.m_eResult != EResult.k_EResultOK)
        {
            MelonLogger.Error("Failed to create lobby: " + result.m_eResult);
            return;
        }

        _lobbyId = new CSteamID(result.m_ulSteamIDLobby);
        if (!Directory.Exists(Path.GetDirectoryName(SharedLobbyFile)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(SharedLobbyFile));
        }

        File.WriteAllText(SharedLobbyFile, _lobbyId.m_SteamID.ToString());
        MelonLogger.Msg("Lobby created: " + _lobbyId + "and saved to " + SharedLobbyFile);

        if (_args.AutoLoad)
            TryStartGame();
    }

    public IEnumerator WaitForLobbyAndJoin()
    {
        while (!File.Exists(SharedLobbyFile))
        {
            yield return new WaitForSeconds(1f);
        }

        var lobbyStr = File.ReadAllText(SharedLobbyFile);
        if (ulong.TryParse(lobbyStr, out ulong lid))
        {
            _lobbyId = new CSteamID(lid);
            SteamMatchmaking.JoinLobby(_lobbyId);
            Callback<LobbyEnter_t>.Create((Callback<LobbyEnter_t>.DispatchDelegate)OnLobbyJoined);
            MelonLogger.Msg("Joining lobby: " + _lobbyId);
        }
        else
        {
            MelonLogger.Error("Invalid lobby.txt content: " + lobbyStr);
        }
    }

    private void OnLobbyJoined(LobbyEnter_t result)
    {
        MelonLogger.Msg("Lobby joined, starting game...");
    }

    private void TryStartGame()
    {
        var save = LoadManager.SaveGames[_args.SaveSlot];
        if (save != null)
        {
            LoadManager.Instance.StartGame(save);
        }
        else
        {
            MelonLogger.Error($"Save slot {_args.SaveSlot} is null.");
        }
    }
}