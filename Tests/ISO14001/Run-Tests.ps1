param([string]$BuildFolder = 'KnowledgeSystem/bin/Iso209Debug')
$ErrorActionPreference = 'Stop'
$repo = (Resolve-Path (Join-Path $PSScriptRoot '../..')).Path
$bin = (Resolve-Path (Join-Path $repo $BuildFolder)).Path
Add-Type -AssemblyName System.Data

# Reuse Debug credentials without printing or storing them in a tracked file.
$builder = New-Object System.Data.SqlClient.SqlConnectionStringBuilder
if ($env:ISO209_TEST_SERVER_CONNECTION) {
    $builder.ConnectionString = $env:ISO209_TEST_SERVER_CONNECTION
} else {
    $source = Get-Content (Join-Path $repo 'DataAccessLayer/SingleConnection.cs') -Raw
    foreach ($mapping in @(@('DataSource','Data Source'),@('InitialCatalog','Initial Catalog'),@('UserID','User ID'),@('Password','Password'))) {
        $builder[$mapping[1]] = [regex]::Match($source, ('(?m)^\s*'+$mapping[0]+'\s*=\s*"([^"]+)"')).Groups[1].Value
    }
}
$builder['Connect Timeout'] = 10
$builder['Initial Catalog'] = 'master'
$admin = New-Object System.Data.SqlClient.SqlConnection($builder.ConnectionString)
$dbName = '7system_Iso209_Test_' + [Guid]::NewGuid().ToString('N')
$connection = $null
$created = $false
try {
    $admin.Open()
    $cmd = $admin.CreateCommand()
    $cmd.CommandText = "CREATE DATABASE [$dbName]"
    [void]$cmd.ExecuteNonQuery()
    $created = $true
    $builder['Initial Catalog'] = $dbName
    $connection = New-Object System.Data.SqlClient.SqlConnection($builder.ConnectionString)
    $connection.Open()
    function Run-Script([string]$path) {
        $sql = Get-Content $path -Raw -Encoding UTF8
        foreach ($batch in [regex]::Split($sql, '(?im)^\s*GO\s*\r?$')) {
            if ([string]::IsNullOrWhiteSpace($batch)) { continue }
            $command = $connection.CreateCommand()
            $command.CommandTimeout = 60
            $command.CommandText = $batch
            [void]$command.ExecuteNonQuery()
            $command.Dispose()
        }
    }
    Run-Script (Join-Path $PSScriptRoot 'fixture.sql')
    $scripts = @('20261028_add_209_iso14001_documents.sql','20261028_add_209_iso14001_triggers.sql','20261028_seed_209_iso14001_navigation.sql','20261028_remove_209_personnel.sql')
    foreach ($pass in 1..2) {
        foreach ($script in $scripts) { Run-Script (Join-Path $repo "Database/$script") }
    }
    Write-Output 'PASS: schema, triggers and navigation installed twice on isolated DB.'
    $env:ISO209_TEST_CONNECTION = $builder.ConnectionString
    $env:ISO209_TEST_ARTIFACTS = Join-Path $env:TEMP '7system-Iso209-TestArtifacts'
    New-Item -ItemType Directory -Path $env:ISO209_TEST_ARTIFACTS -Force | Out-Null
    $csc = Join-Path $env:WINDIR 'Microsoft.NET/Framework64/v4.0.30319/csc.exe'
    $references = @('DataAccessLayer.dll','BusinessLayer.dll','EntityFramework.dll','KnowledgeSystem.exe','DevExpress.Utils.v22.1.dll','DevExpress.XtraEditors.v22.1.dll','DevExpress.XtraGrid.v22.1.dll','DevExpress.Data.v22.1.dll','DevExpress.Drawing.v22.1.dll','DevExpress.Data.Desktop.v22.1.dll','DevExpress.Printing.v22.1.Core.dll') | ForEach-Object { '/r:'+(Join-Path $bin $_) }
    & $csc /nologo /codepage:65001 /debug /target:exe /platform:anycpu "/out:$bin/Iso209Tests.exe" /r:System.Data.dll /r:System.Data.Entity.dll /r:System.Drawing.dll /r:System.Windows.Forms.dll @references (Join-Path $PSScriptRoot 'Iso209Tests.cs')
    if ($LASTEXITCODE -ne 0) { throw 'Test compilation failed.' }
    Copy-Item -LiteralPath (Join-Path $bin 'KnowledgeSystem.exe.config') -Destination (Join-Path $bin 'Iso209Tests.exe.config')
    Push-Location $bin
    try {
        & (Join-Path $bin 'Iso209Tests.exe')
        if ($LASTEXITCODE -ne 0) { throw 'Integration/UI tests failed.' }
    } finally { Pop-Location }
    foreach ($pass in 1..2) { Run-Script (Join-Path $repo 'Database/20261028_optional_209_copy_reference_catalogs.sql') }
    Write-Output 'PASS: optional reference catalogue copy ran twice without duplicates.'
    Write-Output ('Artifacts: '+$env:ISO209_TEST_ARTIFACTS)
} finally {
    $env:ISO209_TEST_CONNECTION = $null
    if ($connection) { $connection.Dispose() }
    if ($created) {
        [System.Data.SqlClient.SqlConnection]::ClearAllPools()
        $cmd = $admin.CreateCommand()
        $cmd.CommandText = "ALTER DATABASE [$dbName] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [$dbName];"
        [void]$cmd.ExecuteNonQuery()
        Write-Output 'Isolated test database removed.'
    }
    $admin.Dispose()
}
