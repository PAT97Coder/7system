param([switch]$SkipBuild)
$ErrorActionPreference = 'Stop'
$workspace = Split-Path -Parent $PSScriptRoot
$buildTools = Join-Path ${env:ProgramFiles} 'Microsoft Visual Studio/2022/Enterprise/MSBuild/Current/Bin'
$bin = Join-Path $workspace 'KnowledgeSystem/bin/Debug'
$output = Join-Path $workspace 'tmp/offboarding-smoke'
New-Item -ItemType Directory -Path $output -Force | Out-Null
if (!$SkipBuild) {
    & (Join-Path $buildTools 'MSBuild.exe') (Join-Path $workspace 'KnowledgeSystem/KnowledgeSystem.csproj') /t:Build /p:Configuration=Debug /p:Platform=AnyCPU /v:quiet /nologo
    if ($LASTEXITCODE -ne 0) { throw 'Application build failed.' }
}
$references = @('KnowledgeSystem.exe','DataAccessLayer.dll','BusinessLayer.dll','EntityFramework.dll',
    'DevExpress.Data.v22.1.dll','DevExpress.Utils.v22.1.dll','DevExpress.XtraEditors.v22.1.dll',
    'DevExpress.XtraGrid.v22.1.dll','DevExpress.XtraLayout.v22.1.dll','DevExpress.XtraBars.v22.1.dll')
$arguments = @('/nologo','/target:exe','/platform:anycpu',('/out:' + (Join-Path $bin 'EmployeeOffboardingSmoke.exe')),
    '/r:System.Windows.Forms.dll','/r:System.Drawing.dll','/r:System.Data.dll')
$arguments += $references | ForEach-Object { '/r:' + (Join-Path $bin $_) }
$arguments += Join-Path $workspace 'Tests/EmployeeOffboardingSmoke.cs'
& (Join-Path $buildTools 'Roslyn/csc.exe') @arguments
if ($LASTEXITCODE -ne 0) { throw 'Smoke test compilation failed.' }
& (Join-Path $bin 'EmployeeOffboardingSmoke.exe') $output
if ($LASTEXITCODE -ne 0) { throw 'Smoke tests failed.' }
