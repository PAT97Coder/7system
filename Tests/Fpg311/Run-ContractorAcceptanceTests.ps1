param([Parameter(Mandatory = $true)][string]$TemplatePath, [string]$TestUserId, [string]$OtherDepartmentTestUserId)
$ErrorActionPreference = 'Stop'
$repo = (Resolve-Path (Join-Path $PSScriptRoot '../..')).Path
$template = (Resolve-Path -LiteralPath $TemplatePath).Path
$bin = Join-Path $repo 'KnowledgeSystem/bin/ContractorAcceptanceDebug'
$msbuild = (Get-Item "${env:ProgramFiles}/Microsoft Visual Studio/*/*/MSBuild/Current/Bin/MSBuild.exe" | Select-Object -First 1).FullName
& $msbuild (Join-Path $repo 'KnowledgeSystem/KnowledgeSystem.csproj') /t:Build /p:Configuration=Debug /p:Platform=x64 /p:OutputPath=bin\ContractorAcceptanceDebug /m /nologo /v:quiet
if ($LASTEXITCODE -ne 0) { throw 'Application build failed' }
$csc = Join-Path (Split-Path $msbuild) 'Roslyn/csc.exe'
$references = @('KnowledgeSystem.exe', 'BusinessLayer.dll', 'DataAccessLayer.dll', 'EntityFramework.dll', 'EPPlus.dll', 'DevExpress.XtraBars.v22.1.dll',
    'DevExpress.XtraEditors.v22.1.dll', 'DevExpress.XtraGrid.v22.1.dll', 'DevExpress.Utils.v22.1.dll',
    'DevExpress.Data.v22.1.dll', 'DevExpress.Data.Desktop.v22.1.dll', 'DevExpress.Printing.v22.1.Core.dll',
    'DevExpress.Drawing.v22.1.dll') | ForEach-Object { '/r:' + (Join-Path $bin $_) }
$exe = Join-Path $bin 'ContractorAcceptanceTests.exe'
& $csc /nologo /platform:x64 /codepage:65001 /target:exe "/out:$exe" /r:System.Drawing.dll /r:System.Windows.Forms.dll @references (Join-Path $PSScriptRoot 'ContractorAcceptanceTests.cs')
if ($LASTEXITCODE -ne 0) { throw 'Test compilation failed' }
Copy-Item -LiteralPath (Join-Path $bin 'KnowledgeSystem.exe.config') -Destination "$exe.config"
$work = Join-Path $env:TEMP ('7system-ContractorAcceptance-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $work | Out-Null
& $exe $template $work $TestUserId $OtherDepartmentTestUserId
if ($LASTEXITCODE -ne 0) { throw 'Contractor acceptance tests failed' }
Write-Output "Test artifacts: $work"
