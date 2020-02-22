cd /d %~dp0

dotnet publish -r win-x64 -c Release -p:PublishSingleFile=true -p:PublishTrimmed=true -o:.\Publish\SelfContained\win
dotnet publish -r win-x64 -c Release -p:PublishSingleFile=true --no-self-contained -o:.\Publish\FrameworkDependentSingleFile\win

REM dotnet publish -r linux-x64 -c Release -p:PublishSingleFile=true -p:PublishTrimmed=true -o:.\Publish\SelfContained\linux
REM dotnet publish -r linux-x64 -c Release -p:PublishSingleFile=true --no-self-contained -o:.\Publish\FrameworkDependentSingleFile\linux

REM dotnet publish -r osx-x64 -c Release -p:PublishSingleFile=true -p:PublishTrimmed=true -o:.\Publish\SelfContained\osx
REM dotnet publish -r osx-x64 -c Release -p:PublishSingleFile=true --no-self-contained -o:.\Publish\FrameworkDependentSingleFile\osx

REM dotnet publish -c Release -o:.\Publish\CrossPlatform
pause