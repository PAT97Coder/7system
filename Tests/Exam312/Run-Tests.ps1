$ErrorActionPreference = 'Stop'
$workspacePath = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$binPath = Join-Path $workspacePath 'KnowledgeSystem/bin/Debug'
$vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio/Installer/vswhere.exe'
$installation = & $vswhere -latest -products * -requires Microsoft.Component.MSBuild -property installationPath
$compiler = Join-Path $installation 'MSBuild/Current/Bin/Roslyn/csc.exe'
$refs = @('KnowledgeSystem.exe', 'BusinessLayer.dll', 'DataAccessLayer.dll', 'EntityFramework.dll', 'Newtonsoft.Json.dll') | ForEach-Object { '/r:' + (Join-Path $binPath $_) }
$refs += Get-ChildItem -LiteralPath $binPath -Filter 'DevExpress*.dll' | ForEach-Object { '/r:' + $_.FullName }
$exePath = Join-Path $binPath 'Exam312Tests.exe'
try {
    & $compiler /nologo /target:exe "/out:$exePath" /r:System.Data.dll /r:System.Drawing.dll /r:System.Windows.Forms.dll $refs (Join-Path $PSScriptRoot 'Exam312Tests.cs') (Join-Path $PSScriptRoot 'Exam312DbTests.cs')
    if ($LASTEXITCODE -ne 0) { throw 'Test compilation failed' }
    & $exePath
    if ($LASTEXITCODE -ne 0) { throw 'Tests failed' }
    $artifactPath = Join-Path ([IO.Path]::GetTempPath()) '7system-Exam312-TestArtifacts'
    $images = Get-ChildItem -LiteralPath $artifactPath -Filter '*.png' | Sort-Object Name
    $html = '<!doctype html><html lang="vi"><meta charset="utf-8"><title>Module 312 - Kiểm thử</title><style>body{font:16px Arial;max-width:1200px;margin:30px auto}img{max-width:100%;border:1px solid #ccc}section{margin:35px 0}pre{white-space:pre-wrap}</style><h1>Module 312 - Kết quả kiểm thử</h1>'
    foreach ($name in @('integration-results.txt','unit-results.txt')) {
        $resultPath = Join-Path $artifactPath $name
        if (Test-Path -LiteralPath $resultPath) { $html += '<pre>' + [System.Net.WebUtility]::HtmlEncode([IO.File]::ReadAllText($resultPath)) + '</pre>' }
    }
    foreach ($image in $images) { $html += '<section><h2>' + $image.BaseName + '</h2><img src="' + $image.Name + '"></section>' }
    [IO.File]::WriteAllText((Join-Path $artifactPath 'index.html'), $html + '</html>', [Text.Encoding]::UTF8)
} finally { Remove-Item -LiteralPath $exePath -ErrorAction SilentlyContinue }
