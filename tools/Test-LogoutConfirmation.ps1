param([switch]$SkipBuild, [string]$BinDirectory)
$ErrorActionPreference = 'Stop'
$workspace = Split-Path -Parent $PSScriptRoot
$buildTools = Join-Path ${env:ProgramFiles} 'Microsoft Visual Studio/2022/Enterprise/MSBuild/Current/Bin'
$bin = if ([string]::IsNullOrWhiteSpace($BinDirectory)) { Join-Path $workspace 'KnowledgeSystem/bin/Debug' }
    else { [IO.Path]::GetFullPath($BinDirectory) }
$output = Join-Path $workspace 'tmp/offboarding-smoke'
New-Item -ItemType Directory -Path $output -Force | Out-Null
if (!$SkipBuild) {
    & (Join-Path $buildTools 'MSBuild.exe') (Join-Path $workspace 'KnowledgeSystem/KnowledgeSystem.csproj') /t:Build /p:Configuration=Debug /p:Platform=AnyCPU /v:quiet /nologo ('/p:OutDir=' + $bin + '/')
    if ($LASTEXITCODE -ne 0) { throw 'Application build failed.' }
}
$references = @('KnowledgeSystem.exe','DataAccessLayer.dll','DevExpress.Data.v22.1.dll','DevExpress.Utils.v22.1.dll',
    'DevExpress.XtraEditors.v22.1.dll','DevExpress.XtraLayout.v22.1.dll')
$arguments = @('/nologo',('/out:' + (Join-Path $bin 'LogoutConfirmationSmoke.exe')),
    '/r:System.Windows.Forms.dll','/r:System.Drawing.dll','/r:System.Data.dll')
$arguments += $references | ForEach-Object { '/r:' + (Join-Path $bin $_) }
$arguments += Join-Path $workspace 'Tests/LogoutConfirmationSmoke.cs'
& (Join-Path $buildTools 'Roslyn/csc.exe') @arguments
if ($LASTEXITCODE -ne 0) { throw 'Logout check compilation failed.' }
& (Join-Path $bin 'LogoutConfirmationSmoke.exe') $output
if ($LASTEXITCODE -ne 0) { throw 'Logout checks failed.' }
