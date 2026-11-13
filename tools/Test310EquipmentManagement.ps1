param([string]$Configuration = 'Debug')
$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path $PSScriptRoot -Parent
$buildDirectory = Join-Path $repoRoot 'tmp/310-equipment-check'
$null = New-Item -ItemType Directory -Path $buildDirectory -Force
$vswherePath = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio/Installer/vswhere.exe'
$msbuildPath = & $vswherePath -latest -requires Microsoft.Component.MSBuild -find 'MSBuild\**\Bin\MSBuild.exe' | Select-Object -First 1
if (!$msbuildPath) { throw 'Khong tim thay MSBuild trong Visual Studio.' }
$compilerPath = Join-Path (Split-Path $msbuildPath -Parent) 'Roslyn/csc.exe'
$buildLog = Join-Path $buildDirectory 'build.log'
& $msbuildPath (Join-Path $repoRoot 'KnowledgeSystem/KnowledgeSystem.csproj') /t:Build "/p:Configuration=$Configuration" /p:Platform=AnyCPU "/p:OutDir=$buildDirectory/" /v:quiet /nologo *> $buildLog
if ($LASTEXITCODE -ne 0) { throw "Build loi. Xem $buildLog" }
$referenceArguments = @(Get-ChildItem -LiteralPath $buildDirectory -Filter 'DevExpress*.dll' | ForEach-Object { '/r:' + $_.FullName })
foreach ($dependency in @('KnowledgeSystem.exe', 'BusinessLayer.dll', 'DataAccessLayer.dll', 'EntityFramework.dll', 'Newtonsoft.Json.dll', 'Scriban.dll')) {
    $referenceArguments += '/r:' + (Join-Path $buildDirectory $dependency)
}
$referenceArguments += '/r:' + (Join-Path ${env:ProgramFiles(x86)} 'Reference Assemblies/Microsoft/Framework/.NETFramework/v4.7.2/Facades/netstandard.dll')
$testExe = Join-Path $buildDirectory 'EquipmentSmoke.exe'
& $compilerPath /nologo /codepage:65001 "/out:$testExe" $referenceArguments (Join-Path $PSScriptRoot 'Test310EquipmentManagement.cs')
if ($LASTEXITCODE -ne 0) { throw 'Bien dich smoke test that bai.' }
Push-Location $buildDirectory
try {
    & $testExe
    if ($LASTEXITCODE -ne 0) { throw 'Smoke test that bai.' }
}
finally { Pop-Location }
