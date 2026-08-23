# $ErrorActionPreference = "Stop"

# Write-Host "=== BUILD ===" -ForegroundColor Cyan
# dotnet build -warnaserror --nologo

# if ($LASTEXITCODE -ne 0) {
#     Write-Host "`nBuild failed." -ForegroundColor Red
#     exit $LASTEXITCODE
# }

# Write-Host "`n=== R# INSPECTIONS ===" -ForegroundColor Cyan

# $inspectCode = "$env:USERPROFILE\.nuget\packages\jetbrains.resharper.globaltools\2026.2.0.2\tools\net8.0\any\inspectcode.exe"

# & $inspectCode --format=Text --stdout --sEverity=HINT --no-build ".\NightFall.sln"

# if ($LASTEXITCODE -ne 0) {
#     Write-Host "`nR# inspection failed." -ForegroundColor Red
#     exit $LASTEXITCODE
# }

# Write-Host "`n✓ All checks completed." -ForegroundColor Green

$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "        NIGHTFALL PROJECT CHECK" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

# ============================================================
# BUILD
# ============================================================

Write-Host "`n=== BUILD / COMPILER ANALYSIS ===" -ForegroundColor Cyan

$buildOutput = dotnet build -warnaserror --nologo 2>&1
$buildExitCode = $LASTEXITCODE

$buildOutput | ForEach-Object {
    Write-Host $_
}

if ($buildExitCode -ne 0) {
    Write-Host "`nBUILD FAILED" -ForegroundColor Red
    exit $buildExitCode
}

# Count compiler errors/warnings from the output.
$buildErrors = @(
    $buildOutput | Where-Object {
        $_ -match "error [A-Z]{2,}[0-9]+:"
    }
)

$buildWarnings = @(
    $buildOutput | Where-Object {
        $_ -match "warning [A-Z]{2,}[0-9]+:"
    }
)

Write-Host "`nBuild Results:" -ForegroundColor White
Write-Host "  Errors:   $($buildErrors.Count)" -ForegroundColor $(if ($buildErrors.Count -gt 0) { "Red" } else { "Green" })
Write-Host "  Warnings: $($buildWarnings.Count)" -ForegroundColor $(if ($buildWarnings.Count -gt 0) { "Yellow" } else { "Green" })

# ============================================================
# R# INSPECTIONS
# ============================================================

Write-Host "`n=== R# INSPECTIONS ===" -ForegroundColor Cyan

$inspectCode = "$env:USERPROFILE\.nuget\packages\jetbrains.resharper.globaltools\2026.2.0.2\tools\net8.0\any\inspectcode.exe"

if (-not (Test-Path $inspectCode)) {
    Write-Host "R# InspectCode was not found:" -ForegroundColor Red
    Write-Host $inspectCode
    exit 1
}

# Capture R# output instead of immediately printing everything.
$inspectionOutput = @(
    & $inspectCode `
        --format=Text `
        --stdout `
        --severity=HINT `
        --no-build `
        ".\NightFall.sln" 2>&1
)

if ($LASTEXITCODE -ne 0) {
    Write-Host "`nR# inspection failed." -ForegroundColor Red
    exit $LASTEXITCODE
}

# ============================================================
# CLASSIFY R# RESULTS
# ============================================================

# These are generally worth investigating.
$importantPatterns = @(
    "never used",
    "never instantiated",
    "possible null reference",
    "null reference",
    "not initialized",
    "unreachable",
    "condition is always",
    "exception is never",
    "assignment is never used",
    "value is never used",
    "dead code",
    "disposed",
    "resource leak",
    "thread",
    "race condition"
)

# These are generally cleanup/style suggestions.
$stylePatterns = @(
    "Use 'var'",
    "Add comma after the last value",
    "Qualifier is redundant",
    "Redundant parentheses",
    "Convert into 'return' statement",
    "Invert 'if' statement",
    "Use null propagation",
    "Use pattern matching",
    "Convert into auto-property",
    "can be made private",
    "can be made get-only",
    "Accessor .* can be made private",
    "Name the parameter to improve code readability",
    "same default value",
    "Code body does not conform to code style",
    "Convert 'switch' statement",
    "Loop can be converted into LINQ",
    "Use an overload with char"
)

$important = @()
$style = @()
$other = @()

foreach ($line in $inspectionOutput) {

    # Ignore empty lines and R# headers.
    if ([string]::IsNullOrWhiteSpace($line)) {
        continue
    }

    if ($line -match "^Solution " -or
        $line -match "^    Project ") {
        continue
    }

    $isImportant = $false
    $isStyle = $false

    foreach ($pattern in $importantPatterns) {
        if ($line -match $pattern) {
            $isImportant = $true
            break
        }
    }

    if (-not $isImportant) {
        foreach ($pattern in $stylePatterns) {
            if ($line -match $pattern) {
                $isStyle = $true
                break
            }
        }
    }

    if ($isImportant) {
        $important += $line
    }
    elseif ($isStyle) {
        $style += $line
    }
    else {
        $other += $line
    }
}

# ============================================================
# RESULTS
# ============================================================

Write-Host "`n=== R# RESULTS ===" -ForegroundColor Cyan

Write-Host "`nImportant issues: $($important.Count)" `
    -ForegroundColor $(if ($important.Count -gt 0) { "Yellow" } else { "Green" })

Write-Host "Style / cleanup hints: $($style.Count)" `
    -ForegroundColor Gray

Write-Host "Other inspections: $($other.Count)" `
    -ForegroundColor Gray

# ============================================================
# IMPORTANT ISSUES
# ============================================================

if ($important.Count -gt 0) {

    Write-Host "`n=== IMPORTANT R# ISSUES ===" -ForegroundColor Yellow

    foreach ($issue in $important) {
        Write-Host "  ! $issue" -ForegroundColor Yellow
    }
}
else {
    Write-Host "`n✓ No obviously important R# issues detected." -ForegroundColor Green
}

# ============================================================
# OTHER INSPECTIONS
# ============================================================

if ($other.Count -gt 0) {

    Write-Host "`n=== OTHER R# INSPECTIONS ===" -ForegroundColor DarkYellow

    foreach ($issue in $other) {
        Write-Host "  ? $issue" -ForegroundColor DarkYellow
    }
}

# ============================================================
# STYLE SUMMARY
# ============================================================

Write-Host "`n=== STYLE / CLEANUP ===" -ForegroundColor Gray
Write-Host "  $($style.Count) style/maintainability suggestions were found."
Write-Host "  These normally do NOT indicate bugs."

# ============================================================
# FINAL VERDICT
# ============================================================

Write-Host "`n========================================" -ForegroundColor Cyan

if ($important.Count -gt 0) {

    Write-Host "RESULT: REVIEW IMPORTANT ISSUES" -ForegroundColor Yellow
    Write-Host "Build is clean, but R# found issues worth investigating."

}
else {

    Write-Host "RESULT: CLEAN" -ForegroundColor Green
    Write-Host "Build has no errors/warnings and no obvious serious R# issues."
}

Write-Host "========================================`n" -ForegroundColor Cyan

exit 0