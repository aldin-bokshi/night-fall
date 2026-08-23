$ErrorActionPreference = "Stop"

Write-Host "=== BUILD ===" -ForegroundColor Cyan
dotnet build -warnaserror --nologo

if ($LASTEXITCODE -ne 0) {
    Write-Host "`nBuild failed." -ForegroundColor Red
    exit $LASTEXITCODE
}

Write-Host "`n=== R# INSPECTIONS ===" -ForegroundColor Cyan

$inspectCode = "$env:USERPROFILE\.nuget\packages\jetbrains.resharper.globaltools\2026.2.0.2\tools\net8.0\any\inspectcode.exe"

& $inspectCode --format=Text --stdout --sEverity=HINT --no-build ".\NightFall.sln"

if ($LASTEXITCODE -ne 0) {
    Write-Host "`nR# inspection failed." -ForegroundColor Red
    exit $LASTEXITCODE
}

Write-Host "`n✓ All checks completed." -ForegroundColor Green