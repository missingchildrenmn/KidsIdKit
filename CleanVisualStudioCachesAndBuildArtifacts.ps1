# CleanVisualStudioCachesAndBuildArtifacts.ps1

# Run this from the root of your solution folder

Write-Host "Cleaning Visual Studio caches and build artifacts..."

# Kill any running Visual Studio processes
Get-Process devenv -ErrorAction SilentlyContinue | ForEach-Object { $_.Kill() }

# Remove hidden .vs folder
if (Test-Path ".vs") {
    Remove-Item ".vs" -Recurse -Force
    Write-Host "Deleted .vs folder"
}

# Remove bin and obj folders recursively
Get-ChildItem -Recurse -Directory | Where-Object { $_.Name -in @("bin","obj") } |
    ForEach-Object { Remove-Item $_.FullName -Recurse -Force; Write-Host "Deleted $($_.FullName)" }

# Scan for old project name references
# $oldName = Read-Host "Enter OLD project name"
# $matches = Select-String -Path *.sln, *.csproj, **\launchSettings.json -Pattern $oldName -SimpleMatch
# if ($matches) {
#     Write-Host "Found references to old project name:"
#     $matches | ForEach-Object { Write-Host $_.Path ":" $_.Line }
# } else {
#     Write-Host "No old project name references found."
# }

# Restore NuGet packages
Write-Host "Running dotnet restore..."
# dotnet restore

# Restore all solution files in the root
Get-ChildItem -Filter *.sln | ForEach-Object {
    Write-Host "Restoring solution $($_.Name)..."
    dotnet restore $_.FullName
}

Write-Host "Cleanup and restore complete. Reopen Visual Studio and rebuild."
