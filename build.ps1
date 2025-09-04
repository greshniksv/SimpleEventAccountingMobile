# PowerShell Version

# Store the current directory
$START_DIR = Get-Location
$ESC = [char]27

# Define the path to the VersionIncreaser.exe
$EXE_PATH = ".\Tools\VersionIncreaser\VersionIncreaser\bin\Release\net8.0\publish\VersionIncreaser.exe"
$REN_EXE_PATH = ".\Tools\FileRenamer\FileRenamer\bin\Release\net8.0\publish\FileRenamer.exe"

# Check if FileRenamer.exe exists
if (-not (Test-Path $REN_EXE_PATH)) {
    Write-Host "[${ESC}[33m WARN ${ESC}[0m] FileRenamer.exe not found. Building it first..."
    
    # Navigate to the Tools\FileRenamer folder
    Set-Location "Tools\FileRenamer"
    
    # Restore dotnet project
    Write-Host "[${ESC}[32m OK ${ESC}[0m] Restoring NuGet packages..."
    dotnet restore FileRenamer.sln
    
    # Build and publish the project
    Write-Host "[${ESC}[32m OK ${ESC}[0m] Building and publishing FileRenamer..."
    dotnet publish -p:NoWarn="CS8600" FileRenamer.sln -c Release -f net8.0
    
    # Return to the start folder
    Set-Location $START_DIR
} else {
    Write-Host "[${ESC}[32m OK ${ESC}[0m] FileRenamer.exe found."
}

# Check if VersionIncreaser.exe exists
if (-not (Test-Path $EXE_PATH)) {
    Write-Host "[${ESC}[33m WARN ${ESC}[0m] VersionIncreaser.exe not found. Building it first..."
    
    # Navigate to the Tools\VersionIncreaser folder
    Set-Location "Tools\VersionIncreaser"
    
    # Restore dotnet project
    Write-Host "[${ESC}[32m OK ${ESC}[0m] Restoring NuGet packages..."
    dotnet restore VersionIncreaser.sln
    
    # Build and publish the project
    Write-Host "[${ESC}[32m OK ${ESC}[0m] Building and publishing VersionIncreaser..."
    dotnet publish -p:NoWarn="CS8600" VersionIncreaser.sln -c Release -f net8.0
    
    # Return to the start folder
    Set-Location $START_DIR
} else {
    Write-Host "[${ESC}[32m OK ${ESC}[0m] VersionIncreaser.exe found."
}

# Execute VersionIncreaser
if (Test-Path $EXE_PATH) {
    & $EXE_PATH
} else {
    Write-Host "[${ESC}[31m ERROR ${ESC}[0m] $EXE_PATH not found!"
    pause
    exit 1
}

# Execute the dotnet publish command
Write-Host "[${ESC}[32m OK ${ESC}[0m] Publishing Android app..."
dotnet publish -f:net9.0-android -c Release -o ./publish /p:AndroidPackageFormat=apk

# Rename the APK file
Rename-Item -Path ".\publish\com.greshniksv.simpleeventaccountingmobile-Signed.apk" -NewName "SimpleEventAccounting.apk"

# Execute FileRenamer if it exists
if (Test-Path $REN_EXE_PATH) {
    & $REN_EXE_PATH ".\SimpleEventAccountingMobile\SimpleEventAccountingMobile.csproj" ".\publish\SimpleEventAccounting.apk"
} else {
    Write-Host "[${ESC}[31m ERROR ${ESC}[0m] $REN_EXE_PATH not found!"
    pause
    exit 1
}

Write-Host "[${ESC}[32m OK ${ESC}[0m] Script completed."
pause