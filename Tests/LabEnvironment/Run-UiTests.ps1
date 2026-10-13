$ErrorActionPreference = 'Stop'
$workspacePath = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$binPath = Join-Path $workspacePath 'KnowledgeSystem/bin/Debug'
$vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio/Installer/vswhere.exe'
$installation = & $vswhere -latest -products * -requires Microsoft.Component.MSBuild -property installationPath
$compiler = Join-Path $installation 'MSBuild/Current/Bin/Roslyn/csc.exe'
$refs = @('KnowledgeSystem.exe','BusinessLayer.dll','DataAccessLayer.dll','EntityFramework.dll') | ForEach-Object { '/r:' + (Join-Path $binPath $_) }
$refs += Get-ChildItem -LiteralPath $binPath -Filter 'DevExpress*.dll' | ForEach-Object { '/r:' + $_.FullName }
$exePath = Join-Path $binPath 'Lab208UiTests.exe'
try {
 & $compiler /nologo /target:exe "/out:$exePath" /r:System.Data.dll /r:System.Drawing.dll /r:System.Windows.Forms.dll $refs (Join-Path $PSScriptRoot '208_UiTests.cs')
 if ($LASTEXITCODE -ne 0) { throw 'UI tests compilation failed' }
 & $exePath
 if ($LASTEXITCODE -ne 0) { throw 'UI tests failed' }
} finally { Remove-Item -LiteralPath $exePath -ErrorAction SilentlyContinue }
