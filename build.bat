@echo off
setlocal

:: Store the current directory
set "START_DIR=%CD%"

:: Define the path to the VersionIncreaser.exe
set "EXE_PATH=.\Tools\VersionIncreaser\VersionIncreaser\bin\Release\net8.0\publish\VersionIncreaser.exe"

:: Check if VersionIncreaser.exe exists
if exist "%EXE_PATH%" (
    echo VersionIncreaser.exe found.
    call "%EXE_PATH%"
    
    :: Execute the dotnet publish command
    echo Publishing Android app...
    dotnet publish -f:net9.0-android -c Release -o ./publish /p:AndroidPackageFormat=apk
    
) else (
    echo VersionIncreaser.exe not found. Building it first...
    
    :: Navigate to the Tools\VersionIncreaser folder
    cd "Tools\VersionIncreaser"
    
    :: Restore dotnet project
    echo Restoring NuGet packages...
    dotnet restore VersionIncreaser.sln
    
    :: Build and publish the project
    echo Building and publishing VersionIncreaser...
    dotnet publish VersionIncreaser.sln -c Release -f net8.0 -o "VersionIncreaser\bin\Release\net8.0\publish\"
    
    :: Return to the start folder
    cd "%START_DIR%"
    
    :: Now execute VersionIncreaser.exe
    echo Running VersionIncreaser...
    call "%EXE_PATH%"
    
    :: Execute the dotnet publish command
    echo Publishing Android app...
    dotnet publish -f:net9.0-android -c Release -o ./publish /p:AndroidPackageFormat=apk
)

echo Script completed.
pause
endlocal