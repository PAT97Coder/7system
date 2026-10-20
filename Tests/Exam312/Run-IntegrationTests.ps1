$ErrorActionPreference = 'Stop'
$workspacePath = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$binPath = Join-Path $workspacePath 'KnowledgeSystem/bin/Debug'
[void][Reflection.Assembly]::LoadFrom((Join-Path $binPath 'DataAccessLayer.dll'))
[void][Reflection.Assembly]::LoadFrom((Join-Path $binPath 'EntityFramework.dll'))
$entity = [System.Data.Entity.Core.EntityClient.EntityConnectionStringBuilder]::new([DataAccessLayer.SingleConnection]::ConString)
$serverConnection = [System.Data.SqlClient.SqlConnectionStringBuilder]::new($entity.ProviderConnectionString)
if ($env:EXAM312_TEST_SERVER_CONNECTION) { $serverConnection = [System.Data.SqlClient.SqlConnectionStringBuilder]::new($env:EXAM312_TEST_SERVER_CONNECTION) }
$serverConnection['Initial Catalog'] = 'master'
$serverConnection['Connect Timeout'] = 10
$testDatabase = '7system_Exam312_Test_' + [Guid]::NewGuid().ToString('N')
$connection = [System.Data.SqlClient.SqlConnection]::new($serverConnection.ConnectionString)
$created = $false
try {
    $connection.Open()
    $command = $connection.CreateCommand()
    $command.CommandText = 'CREATE DATABASE [' + $testDatabase + ']'
    [void]$command.ExecuteNonQuery(); $created = $true
    $connection.ChangeDatabase($testDatabase)
    $command.CommandText = [IO.File]::ReadAllText((Join-Path $workspacePath 'Database/Tests/312_fixture.sql'))
    [void]$command.ExecuteNonQuery()
    $command.CommandText = [IO.File]::ReadAllText((Join-Path $workspacePath 'Database/Tests/312_seed_before_migration.sql'))
    [void]$command.ExecuteNonQuery()
    $migration = [IO.File]::ReadAllText((Join-Path $workspacePath 'Database/20261020_add_312_essay_questions.sql'))
    $command.CommandText = $migration; [void]$command.ExecuteNonQuery()
    $command.CommandText = [IO.File]::ReadAllText((Join-Path $workspacePath 'Database/Tests/312_seed_after_migration.sql'))
    [void]$command.ExecuteNonQuery()
    $command.CommandText = $migration; [void]$command.ExecuteNonQuery()
    $testConnection = [System.Data.SqlClient.SqlConnectionStringBuilder]::new($serverConnection.ConnectionString)
    $testConnection['Initial Catalog'] = $testDatabase
    $entity['provider connection string'] = $testConnection.ConnectionString
    $env:EXAM312_TEST_CONNECTION = $entity.ConnectionString
    Write-Output ('Disposable test database: ' + $testDatabase)
    & (Join-Path $PSScriptRoot 'Run-Tests.ps1')
    if ($LASTEXITCODE -ne 0) { throw 'Integration/UI tests failed' }
} finally {
    Remove-Item Env:/EXAM312_TEST_CONNECTION -ErrorAction SilentlyContinue
    [System.Data.SqlClient.SqlConnection]::ClearAllPools()
    if ($created) {
        if ($testDatabase -notmatch '^7system_Exam312_Test_[0-9a-f]{32}$') { throw 'Refusing to drop unexpected database name' }
        $connection.ChangeDatabase('master')
        $command = $connection.CreateCommand()
        $command.CommandText = 'ALTER DATABASE [' + $testDatabase + '] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [' + $testDatabase + ']'
        [void]$command.ExecuteNonQuery()
        Write-Output 'Disposable test database removed.'
    }
    $connection.Dispose()
}
