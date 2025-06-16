// Copied from Skippy's Radio Mod (https://github.com/Skippeh/Schedule1RealRadioMod/blob/main/LocalMultiplayer/AudioComponent.cs)

using MelonLoader;
using UnityEngine;

namespace LocalLobby;

[RegisterTypeInIl2Cpp]
public class AudioComponent : MonoBehaviour
{
    private void Update()
    {
        var windowFocused = Application.isFocused;
        AudioListener.volume = windowFocused ? 1 : 0;
    }
}