param(
    [double]$MinimumLineCoverage = 70.0,
    [switch]$NoRestore = $true
)

$ErrorActionPreference = "Stop"

$repoRoot = Resolve-Path (Join-Path $PSScriptRoot "..")
$testProject = Join-Path $repoRoot "tests\Rosheta.UnitTests\Rosheta.UnitTests.csproj"
$resultsDirectory = Join-Path $repoRoot "artifacts\coverage"

if (Test-Path $resultsDirectory) {
    Remove-Item -Recurse -Force $resultsDirectory
}

$dotnetArgs = @(
    "test",
    $testProject,
    "--collect", "XPlat Code Coverage",
    "--results-directory", $resultsDirectory,
    "-v", "minimal"
)

if ($NoRestore) {
    $dotnetArgs += "--no-restore"
}

& dotnet @dotnetArgs
if ($LASTEXITCODE -ne 0) {
    throw "dotnet test failed while collecting coverage."
}

$coverageFile = Get-ChildItem -Path $resultsDirectory -Recurse -Filter "coverage.cobertura.xml" |
    Sort-Object LastWriteTime -Descending |
    Select-Object -First 1

if ($null -eq $coverageFile) {
    throw "Coverage report not found in '$resultsDirectory'."
}

[xml]$coverageXml = Get-Content $coverageFile.FullName
$corePackage = $coverageXml.coverage.packages.package |
    Where-Object { $_.name -eq "Rosheta.Core" } |
    Select-Object -First 1

if ($null -eq $corePackage) {
    throw "Rosheta.Core package was not found in coverage report: $($coverageFile.FullName)"
}

$lineRate = [double]::Parse(
    $corePackage.'line-rate',
    [System.Globalization.CultureInfo]::InvariantCulture
)
$lineCoveragePercent = [math]::Round($lineRate * 100, 2)

Write-Host "Rosheta.Core line coverage: $lineCoveragePercent%"
Write-Host "Required threshold: $MinimumLineCoverage%"
Write-Host "Coverage file: $($coverageFile.FullName)"

if ($lineCoveragePercent -lt $MinimumLineCoverage) {
    Write-Error "Coverage gate failed. Rosheta.Core line coverage is below $MinimumLineCoverage%."
    exit 1
}

Write-Host "Coverage gate passed."
