$ErrorActionPreference = "Stop"

# ============================================================
# NIGHTFALL PROJECT CHECK
# ============================================================

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "        NIGHTFALL PROJECT CHECK" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

$failed = $false
$warnings = 0

# ============================================================
# CONFIGURATION
# ============================================================

$solution = ".\NightFall.sln"

# Godot 4.7.2 .NET executable.
#
# Change this path if your Godot executable is somewhere else.
#
# Example:
# $godotPath = "C:\Godot\Godot_v4.7.2-stable_mono_win64.exe"
#

$godotPath = "C:\Users\PULSE Electronics\Downloads\Godot_v4.7.2-stable_mono_win64\Godot_v4.7.2-stable_mono_win64.exe"

$inspectCode = "$env:USERPROFILE\.nuget\packages\jetbrains.resharper.globaltools\2026.2.0.2\tools\net8.0\any\inspectcode.exe"

# NightFall currently targets .NET 8.
$expectedTargetFramework = "net8.0"

# ============================================================
# HELPER FUNCTIONS
# ============================================================

function Write-Pass {
    param(
        [string]$Message
    )

    Write-Host "PASS  $Message" -ForegroundColor Green
}

function Write-Fail {
    param(
        [string]$Message
    )

    Write-Host "FAIL  $Message" -ForegroundColor Red
    $script:failed = $true
}

function Write-Warn {
    param(
        [string]$Message
    )

    Write-Host "WARN  $Message" -ForegroundColor Yellow
    $script:warnings++
}

function Write-Section {
    param(
        [string]$Title
    )

    Write-Host ""
    Write-Host "=== $Title ===" -ForegroundColor Cyan
}

function Test-IsGeneratedPath {
    param(
        [string]$Path
    )

    $normalizedPath = $Path.Replace("/", "\")

    return (
        $normalizedPath -like "*\.godot\*" -or
        $normalizedPath -like "*\bin\*" -or
        $normalizedPath -like "*\obj\*"
    )
}

# ============================================================
# ENVIRONMENT
# ============================================================

Write-Section "ENVIRONMENT"

# ------------------------------------------------------------
# .NET SDK
# ------------------------------------------------------------

try {

    $dotnetVersion = dotnet --version

    if ($LASTEXITCODE -eq 0) {

        Write-Pass ".NET SDK: $dotnetVersion"

    }
    else {

        Write-Fail ".NET SDK could not be detected."

    }

}
catch {

    Write-Fail ".NET SDK is not installed or not available in PATH."

}

# ------------------------------------------------------------
# GODOT
# ------------------------------------------------------------

$godotAvailable = $false

try {

    $godotCommand = Get-Command $godotPath -ErrorAction SilentlyContinue

    if ($godotCommand) {

        $godotExecutable = $godotCommand.Source

    }
    elseif (Test-Path $godotPath -PathType Leaf) {

        $godotExecutable = (Resolve-Path $godotPath).Path

    }
    else {

        $godotExecutable = $null

    }

    if ($godotExecutable) {

        $godotVersionOutput = & $godotExecutable --version 2>&1

        if ($LASTEXITCODE -eq 0) {

            $godotAvailable = $true

            $godotVersionText = ($godotVersionOutput -join " ").Trim()

            if ([string]::IsNullOrWhiteSpace($godotVersionText)) {

                Write-Pass "Godot executable found."

            }
            else {

                Write-Pass "Godot: $godotVersionText"

            }

        }
        else {

            Write-Fail "Godot executable was found but could not be executed."

        }

    }
    else {

        Write-Warn "Godot executable was not found."

        Write-Host ""
        Write-Host "      Current Godot path:" -ForegroundColor Yellow
        Write-Host "      $godotPath" -ForegroundColor Yellow
        Write-Host ""
        Write-Host "      Set `$godotPath at the top of this script to your" -ForegroundColor Yellow
        Write-Host "      Godot 4.7.2 .NET executable." -ForegroundColor Yellow

    }

}
catch {

    Write-Warn "Could not determine the Godot executable."

}

# ------------------------------------------------------------
# GIT
# ------------------------------------------------------------

try {

    $gitVersion = git --version

    if ($LASTEXITCODE -eq 0) {

        Write-Pass "Git: $gitVersion"

    }
    else {

        Write-Warn "Git is not available."

    }

}
catch {

    Write-Warn "Git is not installed or not available in PATH."

}

# ============================================================
# PROJECT CONFIGURATION
# ============================================================

Write-Section "PROJECT CONFIGURATION"

# ------------------------------------------------------------
# Solution
# ------------------------------------------------------------

if (Test-Path $solution -PathType Leaf) {

    Write-Pass "NightFall.sln found."

}
else {

    Write-Fail "NightFall.sln NOT FOUND."

}

# ------------------------------------------------------------
# C# Project
# ------------------------------------------------------------

$csprojFiles = @(
    Get-ChildItem "." `
        -Filter "*.csproj" `
        -Recurse `
        -File |
        Where-Object {
            -not (Test-IsGeneratedPath $_.FullName)
        }
)

if ($csprojFiles.Count -eq 0) {

    Write-Fail "No .csproj file found."

}
elseif ($csprojFiles.Count -eq 1) {

    Write-Pass "C# project found: $($csprojFiles[0].FullName)"

}
else {

    Write-Warn "Multiple .csproj files found:"

    foreach ($project in $csprojFiles) {

        Write-Host "      $($project.FullName)" -ForegroundColor Yellow

    }

}

# ------------------------------------------------------------
# project.godot
# ------------------------------------------------------------

if (Test-Path ".\project.godot" -PathType Leaf) {

    Write-Pass "project.godot found."

}
else {

    Write-Fail "project.godot NOT FOUND."

}

# ============================================================
# TARGET FRAMEWORK
# ============================================================

Write-Section "TARGET FRAMEWORK"

$csproj = $csprojFiles | Select-Object -First 1

if ($null -ne $csproj) {

    $csprojContent = Get-Content $csproj.FullName -Raw

    if (
        $csprojContent -match
        '<TargetFramework>\s*net8\.0\s*</TargetFramework>'
    ) {

        Write-Pass "Target framework: net8.0"

    }
    elseif (
        $csprojContent -match
        '<TargetFramework>\s*(.*?)\s*</TargetFramework>'
    ) {

        $targetFramework = $matches[1]

        Write-Fail "Expected $expectedTargetFramework but found $targetFramework."

    }
    else {

        Write-Fail "Could not determine target framework."

    }

}
else {

    Write-Fail "Target framework could not be checked because no .csproj was found."

}

# ============================================================
# BUILD
# ============================================================

Write-Section "BUILD"

if (Test-Path $solution -PathType Leaf) {

    Write-Host "Running dotnet build..." -ForegroundColor Gray
    Write-Host ""

    try {

        dotnet build `
            $solution `
            --warnaserror `
            --nologo

        if ($LASTEXITCODE -ne 0) {

            Write-Fail "Build failed."

        }
        else {

            Write-Pass "Build passed with 0 errors and 0 warnings."

        }

    }
    catch {

        Write-Fail "dotnet build threw an exception."

    }

}
else {

    Write-Fail "Build skipped because NightFall.sln was not found."

}

# ============================================================
# R# INSPECTIONS
# ============================================================

Write-Section "R# INSPECTIONS"

if (Test-Path $inspectCode -PathType Leaf) {

    Write-Host "Running ReSharper InspectCode..." -ForegroundColor Gray
    Write-Host ""

    try {

        $rsharpOutput = & $inspectCode `
            --format=Text `
            --stdout `
            --no-build `
            $solution 2>&1

        $rsharpExitCode = $LASTEXITCODE

        if ($rsharpOutput) {

            $rsharpOutput | ForEach-Object {
                Write-Host $_
            }

        }

        Write-Host ""

        if ($rsharpExitCode -ne 0) {

            Write-Fail "R# InspectCode failed with exit code $rsharpExitCode."

        }
        else {

            Write-Pass "R# InspectCode completed."

        }

    }
    catch {

        Write-Fail "R# InspectCode threw an exception."

    }

}
else {

    Write-Warn "R# InspectCode was not found."
    Write-Host "      $inspectCode" -ForegroundColor Yellow

}

# ============================================================
# GODOT PROJECT VALIDATION
# ============================================================

Write-Section "GODOT PROJECT VALIDATION"

if ($godotAvailable -and (Test-Path ".\project.godot" -PathType Leaf)) {

    Write-Host "Loading project through Godot headlessly..." -ForegroundColor Gray
    Write-Host ""

    try {

        $godotOutput = & $godotExecutable `
            --headless `
            --path "." `
            --editor `
            --quit 2>&1

        $godotExitCode = $LASTEXITCODE

        if ($godotOutput) {

            $godotOutput | ForEach-Object {
                Write-Host $_
            }

        }

        Write-Host ""

        if ($godotExitCode -ne 0) {

            Write-Fail "Godot project validation failed with exit code $godotExitCode."

        }
        else {

            Write-Pass "Godot project loaded successfully."

        }

        # ----------------------------------------------------
        # Serious Godot errors
        # ----------------------------------------------------

        $godotErrors = @(
            $godotOutput |
                Where-Object {
                    $_ -match `
                    "ERROR|" +
                    "SCRIPT ERROR|" +
                    "Parse Error|" +
                    "Cannot instantiate|" +
                    "Invalid UID|" +
                    "Resource file not found|" +
                    "Failed to load resource|" +
                    "Failed to load script"
                }
        )

        if ($godotErrors.Count -gt 0) {

            Write-Host ""
            Write-Host "Godot reported possible errors:" -ForegroundColor Red

            foreach ($errorLine in $godotErrors) {

                Write-Host "  $errorLine" -ForegroundColor Red

            }

            $failed = $true

        }

    }
    catch {

        Write-Fail "Godot project validation threw an exception."

    }

}
else {

    Write-Warn "Godot validation skipped."

}

# ============================================================
# REQUIRED DIRECTORIES
# ============================================================

Write-Section "REQUIRED DIRECTORIES"

$requiredDirectories = @(
    ".\Scripts",
    ".\Scripts\Core",
    ".\Scripts\Dungeon",
    ".\Scripts\Run",
    ".\Scenes",
    ".\Scenes\Dungeon",
    ".\Data"
)

foreach ($directory in $requiredDirectories) {

    if (Test-Path $directory -PathType Container) {

        Write-Pass $directory

    }
    else {

        Write-Fail "Missing directory: $directory"

    }

}

# ============================================================
# REQUIRED FILES
# ============================================================

Write-Section "REQUIRED FILES"

$requiredFiles = @(
    ".\NightFall.sln",
    ".\NightFall.csproj",
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

    if (Test-Path $file -PathType Leaf) {

        Write-Pass $file

    }
    else {

        Write-Fail "Missing file: $file"

    }

}

# ============================================================
# GAME PATHS
# ============================================================

Write-Section "GAME PATHS"

$gamePathsFile = ".\Scripts\Core\GamePaths.cs"

if (Test-Path $gamePathsFile -PathType Leaf) {

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

            Write-Pass $path

        }
        else {

            Write-Fail "GamePaths missing: $path"

        }

    }

}
else {

    Write-Fail "GamePaths.cs NOT FOUND."

}

# ============================================================
# JSON VALIDATION
# ============================================================

Write-Section "JSON VALIDATION"

$jsonFiles = @(
    Get-ChildItem "." `
        -Filter "*.json" `
        -Recurse `
        -File |
        Where-Object {
            -not (Test-IsGeneratedPath $_.FullName)
        }
)

if ($jsonFiles.Count -eq 0) {

    Write-Warn "No project JSON files found."

}
else {

    foreach ($file in $jsonFiles) {

        try {

            Get-Content $file.FullName -Raw |
                ConvertFrom-Json |
                Out-Null

            Write-Pass $file.FullName

        }
        catch {

            Write-Fail "Invalid JSON: $($file.FullName)"
            Write-Host "      $($_.Exception.Message)" -ForegroundColor Red

        }

    }

}

# ============================================================
# CASE CONFLICT CHECK
# ============================================================

Write-Section "CASE CONFLICTS"

$allFiles = @(
    Get-ChildItem "." `
        -Recurse `
        -File |
        Where-Object {
            -not (Test-IsGeneratedPath $_.FullName)
        }
)

$caseGroups = $allFiles |
    Group-Object {
        $_.FullName.ToLowerInvariant()
    }

$caseConflicts = @(
    $caseGroups |
        Where-Object {
            $_.Count -gt 1
        }
)

if ($caseConflicts.Count -eq 0) {

    Write-Pass "No case-sensitive filename conflicts found."

}
else {

    foreach ($group in $caseConflicts) {

        Write-Host ""
        Write-Host "CASE CONFLICT:" -ForegroundColor Red

        foreach ($item in $group.Group) {

            Write-Host "      $($item.FullName)" -ForegroundColor Red

        }

    }

    $failed = $true

}

# ============================================================
# PROJECT.GODOT VALIDATION
# ============================================================

Write-Section "PROJECT.GODOT"

if (Test-Path ".\project.godot" -PathType Leaf) {

    $projectGodot = Get-Content ".\project.godot" -Raw

    $requiredProjectSettings = @(
        "[application]",
        "[display]",
        "[rendering]"
    )

    foreach ($setting in $requiredProjectSettings) {

        if ($projectGodot.Contains($setting)) {

            Write-Pass "$setting found."

        }
        else {

            Write-Warn "$setting section not found."

        }

    }

}

# ============================================================
# SCENE FILE CHECK
# ============================================================

Write-Section "SCENE FILES"

$sceneFiles = @(
    Get-ChildItem "." `
        -Filter "*.tscn" `
        -Recurse `
        -File |
        Where-Object {
            -not (Test-IsGeneratedPath $_.FullName)
        }
)

if ($sceneFiles.Count -eq 0) {

    Write-Fail "No .tscn scenes found."

}
else {

    Write-Pass "$($sceneFiles.Count) scene files found."

}

# ============================================================
# RESOURCE FILE CHECK
# ============================================================

Write-Section "RESOURCE FILES"

$resourceFiles = @(
    Get-ChildItem "." `
        -Recurse `
        -File |
        Where-Object {
            $_.Extension -in @(
                ".tres",
                ".res"
            ) -and
            -not (Test-IsGeneratedPath $_.FullName)
        }
)

Write-Pass "$($resourceFiles.Count) Godot resource files found."

# ============================================================
# DEBUG / TODO SCAN
# ============================================================

Write-Section "DEBUG / TODO SCAN"

$debugPatterns = @(
    "TODO",
    "FIXME",
    "Console.WriteLine",
    "GD.Print",
    "Debugger.Break"
)

$sourceFiles = @(
    Get-ChildItem "." `
        -Recurse `
        -File |
        Where-Object {
            $_.Extension -eq ".cs" -and
            -not (Test-IsGeneratedPath $_.FullName)
        }
)

foreach ($pattern in $debugPatterns) {

    $matches = @(
        $sourceFiles |
            Select-String -Pattern $pattern
    )

    if ($matches.Count -gt 0) {

        Write-Warn "Found $($matches.Count) occurrence(s) of '$pattern'."

        foreach ($match in $matches) {

            Write-Host `
                "      $($match.Path):$($match.LineNumber)" `
                -ForegroundColor Yellow

        }

    }

}

# ============================================================
# GIT STATUS
# ============================================================

Write-Section "GIT"

if (Test-Path ".\.git" -PathType Container) {

    try {

        $gitStatus = @(
            git status --short
        )

        if ($gitStatus.Count -eq 0) {

            Write-Pass "Git working tree is clean."

        }
        else {

            Write-Warn "Git working tree contains changes."

            foreach ($line in $gitStatus) {

                Write-Host "      $line" -ForegroundColor Yellow

            }

        }

    }
    catch {

        Write-Warn "Could not determine Git status."

    }

}
else {

    Write-Warn "This directory is not a Git repository."

}

# ============================================================
# FINAL RESULT
# ============================================================

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan

if ($failed) {

    Write-Host "RESULT: FAILED" -ForegroundColor Red
    Write-Host ""
    Write-Host "NightFall has one or more blocking problems." -ForegroundColor Red
    Write-Host "Fix the failures above before continuing." -ForegroundColor Red

}
else {

    Write-Host "RESULT: CLEAN" -ForegroundColor Green
    Write-Host ""
    Write-Host "All blocking checks passed." -ForegroundColor Green

}

if ($warnings -gt 0) {

    Write-Host ""
    Write-Host "Warnings: $warnings" -ForegroundColor Yellow

}
else {

    Write-Host ""
    Write-Host "Warnings: 0" -ForegroundColor Green

}

Write-Host "========================================" -ForegroundColor Cyan

if ($failed) {
    exit 1
}

exit 0