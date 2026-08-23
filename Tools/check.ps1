$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "        NIGHTFALL PROJECT CHECK" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

$failed = $false

# ============================================================
# BUILD
# ============================================================

Write-Host ""
Write-Host "=== BUILD ===" -ForegroundColor Cyan

dotnet build ".\NightFall.sln" -warnaserror --nologo

if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "BUILD FAILED" -ForegroundColor Red
    $failed = $true
}
else {
    Write-Host ""
    Write-Host "Build passed with 0 errors and 0 warnings." -ForegroundColor Green
}

# ============================================================
# R# INSPECTIONS
# ============================================================

Write-Host ""
Write-Host "=== R# INSPECTIONS ===" -ForegroundColor Cyan

$inspectCode = "$env:USERPROFILE\.nuget\packages\jetbrains.resharper.globaltools\2026.2.0.2\tools\net8.0\any\inspectcode.exe"

if (Test-Path $inspectCode) {

    Write-Host "Running R#..." -ForegroundColor Gray

    & $inspectCode `
        --format=Text `
        --stdout `
        --no-build `
        ".\NightFall.sln"

    Write-Host ""
    Write-Host "R# inspection completed." -ForegroundColor Green

}
else {

    Write-Host "R# InspectCode was not found." -ForegroundColor Yellow
    Write-Host $inspectCode -ForegroundColor Yellow

}

# ============================================================
# PROJECT FILE
# ============================================================

Write-Host ""
Write-Host "=== PROJECT FILE ===" -ForegroundColor Cyan

if (Test-Path ".\project.godot") {

    Write-Host "project.godot found." -ForegroundColor Green

}
else {

    Write-Host "project.godot NOT FOUND." -ForegroundColor Red
    $failed = $true

}

# ============================================================
# REQUIRED FILES
# ============================================================

Write-Host ""
Write-Host "=== REQUIRED FILES ===" -ForegroundColor Cyan

$requiredFiles = @(
    ".\NightFall.sln",
    ".\project.godot",

    ".\Scripts\Run\RunConfig.cs",
    ".\Scripts\Run\RunSession.cs",
    ".\Scripts\Run\SeedTranslator.cs",

    ".\Scripts\Dungeon\Room.cs",
    ".\Scripts\Dungeon\RoomType.cs",
    ".\Scripts\Dungeon\RoomManager.cs",

    ".\Scripts\Core\GamePaths.cs",

    ".\Scenes\Game.tscn",

    ".\Scenes\Dungeon\CombatRoom\CombatRoom.tscn",
    ".\Scenes\Dungeon\EliteRoom\EliteRoom.tscn",
    ".\Scenes\Dungeon\ShopRoom\ShopRoom.tscn",
    ".\Scenes\Dungeon\BossRoom\BossRoom.tscn",
    ".\Scenes\Dungeon\HubRoom\HubRoom.tscn",
    ".\Scenes\Dungeon\StartRoom\StartRoom.tscn"
)

foreach ($file in $requiredFiles) {

    if (Test-Path $file) {

        Write-Host "OK   $file" -ForegroundColor Green

    }
    else {

        Write-Host "MISS $file" -ForegroundColor Red
        $failed = $true

    }

}

# ============================================================
# GAME PATHS
# ============================================================

Write-Host ""
Write-Host "=== GAME PATHS ===" -ForegroundColor Cyan

$gamePathsFile = ".\Scripts\Core\GamePaths.cs"

if (Test-Path $gamePathsFile) {

    $gamePaths = Get-Content $gamePathsFile -Raw

    $expectedPaths = @(
        "Scenes/Dungeon/CombatRoom/CombatRoom.tscn",
        "Scenes/Dungeon/EliteRoom/EliteRoom.tscn",
        "Scenes/Dungeon/ShopRoom/ShopRoom.tscn",
        "Scenes/Dungeon/BossRoom/BossRoom.tscn",
        "Scenes/Dungeon/HubRoom/HubRoom.tscn",
        "Scenes/Dungeon/StartRoom/StartRoom.tscn"
    )

    foreach ($path in $expectedPaths) {

        if ($gamePaths.Contains($path)) {

            Write-Host "OK   $path" -ForegroundColor Green

        }
        else {

            Write-Host "MISS $path" -ForegroundColor Red
            $failed = $true

        }

    }

}
else {

    Write-Host "GamePaths.cs NOT FOUND." -ForegroundColor Red
    $failed = $true

}

# ============================================================
# FINAL RESULT
# ============================================================

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan

if ($failed) {

    Write-Host "RESULT: FAILED" -ForegroundColor Red
    Write-Host "Fix the errors above before continuing." -ForegroundColor Red

}
else {

    Write-Host "RESULT: CLEAN" -ForegroundColor Green
    Write-Host "All required checks passed." -ForegroundColor Green

}

Write-Host "========================================" -ForegroundColor Cyan

if ($failed) {
    exit 1
}

exit 0