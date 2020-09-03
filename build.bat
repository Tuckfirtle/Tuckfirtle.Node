@echo off

SETLOCAL 

SET project=src\Tuckfirtle.Node.csproj
SET framework=netcoreapp3.1

SET configuration_linux=Release_linux
SET configuration_osx=Release_osx
SET configuration_win=Release_win

SET platform_arm=arm
SET platform_x64=x64
SET platform_x86=x86

SET output=./bin/Release

dotnet publish %project% /p:Platform=%platform_x64% -c %configuration_linux% -f %framework% -o %output%/net-core-win-x64 -r linux-x64 --self-contained

ENDLOCAL