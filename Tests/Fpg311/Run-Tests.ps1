$ErrorActionPreference = 'Stop'
$repo = (Resolve-Path (Join-Path $PSScriptRoot '../..')).Path
$bin = (Resolve-Path (Join-Path $repo 'KnowledgeSystem/bin/Fpg311Debug')).Path
$csc = Join-Path $env:WINDIR 'Microsoft.NET/Framework64/v4.0.30319/csc.exe'
$refs = @('KnowledgeSystem.exe','BusinessLayer.dll','DataAccessLayer.dll','DevExpress.XtraBars.v22.1.dll','DevExpress.XtraEditors.v22.1.dll','DevExpress.XtraGrid.v22.1.dll','DevExpress.Utils.v22.1.dll','DevExpress.Data.v22.1.dll','DevExpress.Data.Desktop.v22.1.dll','DevExpress.Printing.v22.1.Core.dll','DevExpress.Drawing.v22.1.dll') | ForEach-Object { '/r:'+(Join-Path $bin $_) }
& $csc /nologo /codepage:65001 /target:exe "/out:$bin/Fpg311PreviewTests.exe" /r:System.Drawing.dll /r:System.Windows.Forms.dll @refs (Join-Path $PSScriptRoot 'PreviewTests.cs')
if ($LASTEXITCODE -ne 0) { throw 'Test compilation failed' }
Copy-Item -LiteralPath (Join-Path $bin 'KnowledgeSystem.exe.config') -Destination (Join-Path $bin 'Fpg311PreviewTests.exe.config')
Push-Location $bin
try {
    & (Join-Path $bin 'Fpg311PreviewTests.exe') (Join-Path $bin 'Fpg311Preview.png')
    if ($LASTEXITCODE -ne 0) { throw 'Preview tests failed' }
} finally { Pop-Location }
