param([Parameter(Mandatory=$true)][string]$TemplatePath,
      [string]$OutputFolder = (Join-Path $env:TEMP '7system-FuelStatistics'))
$ErrorActionPreference = 'Stop'
$repo = (Resolve-Path (Join-Path $PSScriptRoot '../..')).Path
$bin = Join-Path $repo 'KnowledgeSystem/bin/Fpg311Debug'
$csc = (Get-Item "${env:ProgramFiles}/Microsoft Visual Studio/*/*/MSBuild/Current/Bin/Roslyn/csc.exe" | Select-Object -First 1).FullName
if (!$csc) { throw 'Visual Studio C# compiler not found' }
& $csc /nologo /codepage:65001 /target:exe "/out:$bin/FuelUsageStatisticsTests.exe" /r:System.Drawing.dll /r:System.Core.dll /r:System.Xml.dll "/r:$bin/EPPlus.dll" (Join-Path $repo 'KnowledgeSystem/Helpers/FuelUsageStatisticsExporter.cs') (Join-Path $PSScriptRoot 'FuelUsageStatisticsTests.cs')
if ($LASTEXITCODE -ne 0) { throw 'Test compilation failed' }
& (Join-Path $bin 'FuelUsageStatisticsTests.exe') $TemplatePath $OutputFolder
if ($LASTEXITCODE -ne 0) { throw 'Fuel usage statistics tests failed' }
