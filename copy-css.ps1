# Copy CSS files from KidsIdKit.Core to KidsIdKit.Web
Write-Host "Copying CSS files from KidsIdKit.Core to KidsIdKit.Web..." -ForegroundColor Green

$sourceDir = Join-Path $PSScriptRoot "KidsIdKit.Core\wwwroot\css"
$destDir = Join-Path $PSScriptRoot "KidsIdKit.Web\wwwroot\css"

if (Test-Path $sourceDir) {
    Copy-Item -Path "$sourceDir\*" -Destination $destDir -Recurse -Force
    Write-Host "CSS files copied successfully!" -ForegroundColor Green
} else {
    Write-Host "Source directory not found: $sourceDir" -ForegroundColor Red
}
