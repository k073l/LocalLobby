param(
    [string]$Mode = "host",               # "host" or "client"
    [string]$SteamId,
    [string]$Name
)

$defaultHostId = "76561199320154780"
$defaultClientId = "76561199485712034"
$defaultHostName = "Host"
$defaultClientName = "Client"

if ($Mode -ne "host" -and $Mode -ne "client") {
    Write-Error "Invalid mode specified. Use 'host' or 'client'."
    exit 1
}

$steamId = if ($SteamId) { $SteamId } elseif ($Mode -eq "host") { $defaultHostId } else { $defaultClientId }
$name = if ($Name) { $Name } elseif ($Mode -eq "host") { $defaultHostName } else { $defaultClientName }

$configDir = Join-Path $PSScriptRoot "Schedule I_Data\Plugins\x86_64\steam_settings"
$configFile = Join-Path $configDir "configs.user.ini"

if (-not (Test-Path $configDir)) {
    New-Item -ItemType Directory -Path $configDir -Force | Out-Null
}

$configContent = @"
[user::general]
account_name=$name
account_steamid=$steamId
language=english
"@

Set-Content -Path $configFile -Value $configContent -Encoding UTF8

Write-Host "Goldberg config written to $configFile"
