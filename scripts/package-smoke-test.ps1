param(
    [string] $Configuration = "Release",
    [string] $PackageOutput = "artifacts/packages",
    [switch] $SkipSolutionBuild,
    [switch] $SkipPack
)

$ErrorActionPreference = "Stop"

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$packageOutputPath = Join-Path $repositoryRoot $PackageOutput
$smokeTestOutputPath = Join-Path $repositoryRoot "artifacts/package-smoke-test"
$smokeTestPackagesPath = Join-Path $smokeTestOutputPath "packages"
$nugetConfigPath = Join-Path $smokeTestOutputPath "NuGet.config"
$solutionPath = Join-Path $repositoryRoot "PdfFormPublisher.slnx"
$projectPath = Join-Path $repositoryRoot "src/PdfFormPublisher/PdfFormPublisher.csproj"
$smokeTestProjectPath = Join-Path $repositoryRoot "tests/PackageSmokeTest/PackageSmokeTest.csproj"

function Invoke-DotNet {
    dotnet @args

    if ($LASTEXITCODE -ne 0) {
        throw "dotnet $args failed with exit code $LASTEXITCODE."
    }
}

New-Item -ItemType Directory -Path $packageOutputPath -Force | Out-Null
New-Item -ItemType Directory -Path $smokeTestOutputPath -Force | Out-Null

@"
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="local-packages" value="$packageOutputPath" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
</configuration>
"@ | Set-Content -Path $nugetConfigPath

if (-not $SkipSolutionBuild) {
    Invoke-DotNet restore $solutionPath
    Invoke-DotNet build $solutionPath --configuration $Configuration --no-restore /nr:false
}

if (-not $SkipPack) {
    Invoke-DotNet pack $projectPath --configuration $Configuration --no-build --output $packageOutputPath
}

if (-not (Get-ChildItem -Path $packageOutputPath -Filter "PdfFormPublisher.*.nupkg" -File)) {
    throw "No PdfFormPublisher package was found in '$packageOutputPath'."
}

Invoke-DotNet restore $smokeTestProjectPath --configfile $nugetConfigPath --packages $smokeTestPackagesPath
Invoke-DotNet run --project $smokeTestProjectPath --configuration $Configuration --no-restore
