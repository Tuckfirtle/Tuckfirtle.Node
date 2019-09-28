@echo off

SETLOCAL 

SET project=src/Tuckfirtle.Node.Builder.csproj
SET configuration=Release

SET framework_core=netcoreapp3.0

SET output=./bin/Release

dotnet publish %project% /p:PublishSingleFile=true -c %configuration% -f %framework_core% -o %output%/net-core-win-x64 -r win-x64 --self-contained

copy include\net-core-win-x64\liboqs.dll bin\Release\net-core-win-x64\liboqs.dll

ENDLOCAL