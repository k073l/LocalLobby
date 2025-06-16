powershell -ExecutionPolicy Bypass -File "createGoldbergConfig.ps1" -Mode host
start "" "Schedule I.exe" --host --adjust-window --left-offset 0
timeout /t 10
powershell -ExecutionPolicy Bypass -File "createGoldbergConfig.ps1" -Mode client
start "" "Schedule I.exe" --join --adjust-window --left-offset 20