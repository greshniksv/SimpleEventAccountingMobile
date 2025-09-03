@echo off
setlocal

:: Store the current directory
set "START_DIR=%CD%"

:: Define the path to the VersionIncreaser.exe
set "EXE_PATH=.\Tools\VersionIncreaser\VersionIncreaser\bin\Release\net8.0\publish\VersionIncreaser.exe"
set "REN_EXE_PATH=.\Tools\FileRenamer\FileRenamer\bin\Release\net8.0\publish\FileRenamer.exe"

:: Check if FileRenamer.exe exists
if not exist "%REN_EXE_PATH%" (
   echo [ Warn ] FileRenamer.exe not found. Building it first...
    
    :: Navigate to the Tools\FileRenamer folder
    cd "Tools\FileRenamer"
    
    :: Restore dotnet project
    echo [ OK ] Restoring NuGet packages...
    dotnet restore FileRenamer.sln
    
    :: Build and publish the project
    echo [ OK ] Building and publishing FileRenamer...
    dotnet publish FileRenamer.sln -c Release -f net8.0
    
    :: Return to the start folder
    cd "%START_DIR%"
) else (
    echo [ OK ] FileRenamer.exe found.
)

:: Check if VersionIncreaser.exe exists
if not exist "%EXE_PATH%" (
   echo [ WARN ] VersionIncreaser.exe not found. Building it first...
    
    :: Navigate to the Tools\VersionIncreaser folder
    cd "Tools\VersionIncreaser"
    
    :: Restore dotnet project
    echo [ OK ] Restoring NuGet packages...
    dotnet restore VersionIncreaser.sln
    
    :: Build and publish the project
    echo [ OK ] Building and publishing VersionIncreaser...
    dotnet publish VersionIncreaser.sln -c Release -f net8.0
    
    :: Return to the start folder
    cd "%START_DIR%"
) else (
    echo [ OK ] VersionIncreaser.exe found.
)

call "%EXE_PATH%"

:: Execute the dotnet publish command
echo [ OK ] Publishing Android app...
dotnet publish -f:net9.0-android -c Release -o ./publish /p:AndroidPackageFormat=apk

ren ".\publish\com.greshniksv.simpleeventaccountingmobile-Signed.apk" "SimpleEventAccounting.apk"

if exist "%REN_EXE_PATH%" (
    "%REN_EXE_PATH%" ".\SimpleEventAccountingMobile\SimpleEventAccountingMobile.csproj" ".\publish\SimpleEventAccounting.apk"
) else (
    echo [ ERROR ] %REN_EXE_PATH% not found!
    pause
    exit /b 1
)

echo [ OK ] Script completed.
pause
endlocal