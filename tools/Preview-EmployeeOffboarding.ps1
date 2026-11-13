param()
$ErrorActionPreference = 'Stop'
$workspace = Split-Path -Parent $PSScriptRoot
$bin = Join-Path $workspace 'KnowledgeSystem/bin/Debug'
$compiler = Join-Path ${env:ProgramFiles} 'Microsoft Visual Studio/2022/Enterprise/MSBuild/Current/Bin/Roslyn/csc.exe'
$output = Join-Path $workspace 'tmp/offboarding-smoke'
New-Item -ItemType Directory -Path $output -Force | Out-Null
$arguments = @('/nologo',('/out:' + (Join-Path $bin 'EmployeeOffboardingPreview.exe')),
    '/r:System.Windows.Forms.dll','/r:System.Drawing.dll','/r:System.Data.dll')
foreach ($file in @('KnowledgeSystem.exe','DataAccessLayer.dll','DevExpress.Data.v22.1.dll','DevExpress.Utils.v22.1.dll',
    'DevExpress.XtraEditors.v22.1.dll','DevExpress.XtraGrid.v22.1.dll','DevExpress.XtraLayout.v22.1.dll','DevExpress.XtraBars.v22.1.dll')) {
    $arguments += '/r:' + (Join-Path $bin $file)
}
$arguments += Join-Path $workspace 'Tests/EmployeeOffboardingPreview.cs'
& $compiler @arguments
if ($LASTEXITCODE -ne 0) { throw 'Preview compilation failed.' }
& (Join-Path $bin 'EmployeeOffboardingPreview.exe') $output
if ($LASTEXITCODE -ne 0) { throw 'Preview rendering failed.' }
