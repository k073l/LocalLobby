# LocalLobby
A mod for Schedule 1, that allows you to test multiplayer functionalities requiring a Steam lobby (locally).

Requires MelonLoader as modloader and [gbe emu](https://github.com/Detanup01/gbe_fork) for Steam emulation.

This is not a replacement for [LocalMultiplayer](https://github.com/k073l/LocalMultiplayer), as LocalMultiplayer allows testing without modifying the SteamAPI files, but is limited to FishNet, while this mod allows you to test the Steam lobby functionalities, such as joining lobbies, starting games, setting/reading lobby data, etc.
## Installation
1. Install MelonLoader
2. Extract the zip file
3. Place the dll file into the Mods directory for your branch
    - For none/beta use IL2CPP
    - For alternate/alternate beta use Mono
4. Launch the game with command line arguments (more in [Usage](#usage))

## Usage
1. Install [gbe emu](https://github.com/Detanup01/gbe_fork). It's a maintained fork of [Goldberg Emulator](https://gitlab.com/Mr_Goldberg/goldberg_emulator), which allows you to emulate Steam.

To do that, download the release version from [releases](https://github.com/Detanup01/gbe_fork/releases/latest) and find `regular\x64\steam_api64.dll`. Copy it into `<Schedule 1 directory>\Schedule I_Data\Plugins\x86_64`, overwriting the existing `steam_api64.dll` (make a backup first!).

2. Place `start.bat` and `createGoldbergConfig.ps1` in the same directory as the game executable (`Schedule I.exe`).
3. Run `start.bat`. This should launch 2 instances of the game, one as host (left) and one as client (right).
4. Verify in MelonLoader consoles that logged SteamIDs are different for both instances.
5. The mod should automatically create a lobby and connect both instances to it. If it doesn't, double check the SteamIDs and lobbyID in the MelonLoader consoles.
6. You can now test multiplayer functionalities in the game. The host can start the game, and the client will join automatically.

## CLI Arguments Reference
For the mod itself (passed in to game executable):

`--host` - defines the instance as host

`--client`, `--join` - defines the instance as client

`--autoload` - loads the game in slot 0 (on host)

`--saveslot <slot>` - loads the game in specified slot (on host). Valid slots are 0-4.

`--adjust-window` - will adjust the window size to fit both screens

`--left-offset <offset>` - sets the offset from the left edge of the screen for the host instance, as well as the gap between instances.


For the `createGoldbergConfig.ps1` script:

`-Mode <mode>` - defines the mode of the script. Valid modes are `host` and `client`. This will provide defaults for IDs and names.

`-SteamId <steam64>` - defines the SteamID for the instance. Must be a valid SteamID.

`-Name <name>` - defines the name for the instance.


## Troubleshooting
### Where are my saves?
Saves are tied to the SteamID. If you're using default values (like in `start.bat`), you need to copy the saves. Locate where your saves are stored (if your steamID is `123`, it will be in `C:\Users\<username>\AppData\LocalLow\TVGS\Schedule I\Saves\123`), and copy the contents of the folder to the folder with host's SteamID (e.g. `C:\Users\<username>\AppData\LocalLow\Schedule I\76561199320154780`).
### In main menu, I see Lobby 0/4 on the client
It's likely the SteamIDs for both client and host are the same. Double check the SteamIDs in the MelonLoader consoles. If they are the same, tinker with `timeout` in `.bat` file, as we need to "predict" when SteamAPI will be loaded. If the timeout is too short, SteamAPI might grab the ID of the client. This is especially true when testing with a large modpack.
### What if I don't want to use `start.bat`?
Follow the same steps as in the `.bat` file. Run the Powershell script with parameters, load the game, wait for SteamAPI to load, regenerate the Goldberg config with the script and start the client.

## Credits
- [Detanup01 and the contributors](https://github.com/Detanup01/gbe_fork) for maintaining the fork of Goldberg Emulator
- [TVGS](https://store.steampowered.com/app/3164500/Schedule_I/) for making the game in a way that allows for stuff like that
- [Skippy](https://github.com/Skippeh/Schedule1RealRadioMod/blob/main/LocalMultiplayer/AudioComponent.cs) for AudioComponent used to mute non-active instance and for the idea for moving the windows
