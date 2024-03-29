@echo off

SETLOCAL 

SET project=src/Tuckfirtle.Node.csproj
SET configuration=Release

SET framework_core=netcoreapp3.0

SET output=./bin/Release

dotnet publish %project% /p:PublishSingleFile=true -c %configuration% -f %framework_core% -o %output%/net-core-linux-arm -r linux-arm --self-contained
dotnet publish %project% /p:PublishSingleFile=true -c %configuration% -f %framework_core% -o %output%/net-core-linux-x64 -r linux-x64 --self-contained
dotnet publish %project% /p:PublishSingleFile=true -c %configuration% -f %framework_core% -o %output%/net-core-osx-x64 -r osx-x64 --self-contained
dotnet publish %project% /p:PublishSingleFile=true -c %configuration% -f %framework_core% -o %output%/net-core-win-arm -r win-arm --self-contained
dotnet publish %project% /p:PublishSingleFile=true -c %configuration% -f %framework_core% -o %output%/net-core-win-x64 -r win-x64 --self-contained
dotnet publish %project% /p:PublishSingleFile=true -c %configuration% -f %framework_core% -o %output%/net-core-win-x86 -r win-x86 --self-contained

copy include\net-core-linux-arm\liboqs.so bin\Release\net-core-linux-arm\liboqs.so
copy include\net-core-linux-x64\liboqs.so bin\Release\net-core-linux-x64\liboqs.so
copy include\net-core-osx-x64\liboqs.dylib bin\Release\net-core-osx-x64\liboqs.dylib
copy include\net-core-win-x64\liboqs.dll bin\Release\net-core-win-x64\liboqs.dll
copy include\net-core-win-x86\liboqs.dll bin\Release\net-core-win-x86\liboqs.dll

7z a %output%/net-core-linux-arm.tar %output%/net-core-linux-arm
7z a %output%/net-core-linux-arm.tar.gz %output%/net-core-linux-arm.tar -mx=9

7z a %output%/net-core-linux-x64.tar %output%/net-core-linux-x64
7z a %output%/net-core-linux-x64.tar.gz %output%/net-core-linux-x64.tar -mx=9

7z a %output%/net-core-osx-x64.tar %output%/net-core-osx-x64
7z a %output%/net-core-osx-x64.tar.gz %output%/net-core-osx-x64.tar -mx=9

7z a %output%/net-core-win-arm.zip %output%/net-core-win-arm -mx=9
7z a %output%/net-core-win-x64.zip %output%/net-core-win-x64 -mx=9
7z a %output%/net-core-win-x86.zip %output%/net-core-win-x86 -mx=9

ENDLOCAL