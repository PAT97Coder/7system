param([string]$ConnectionString = $env:LAB208_TEST_SERVER_CONNECTION)
$ErrorActionPreference = 'Stop'
$workspacePath = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$binPath = Join-Path $workspacePath 'KnowledgeSystem/bin/Debug'
if (!$ConnectionString) {
    [System.Reflection.Assembly]::LoadFrom((Join-Path $binPath 'EntityFramework.dll')) | Out-Null
    [System.Reflection.Assembly]::LoadFrom((Join-Path $binPath 'DataAccessLayer.dll')) | Out-Null
    $ConnectionString = (New-Object System.Data.Entity.Core.EntityClient.EntityConnectionStringBuilder([DataAccessLayer.SingleConnection]::ConString)).ProviderConnectionString
}
$builder = New-Object System.Data.SqlClient.SqlConnectionStringBuilder($ConnectionString)
$builder['Initial Catalog'] = 'master'; $builder['Connect Timeout'] = 5
$serverConnection = New-Object System.Data.SqlClient.SqlConnection($builder.ConnectionString)
$testDatabase = '7system_Lab208_Test_' + [Guid]::NewGuid().ToString('N')
if ($testDatabase -notmatch '^7system_Lab208_Test_[0-9a-f]{32}$') { throw 'Unsafe test database name' }
$created = $false
$exePath = Join-Path $binPath 'Lab208IntegrationTests.exe'
try {
    $serverConnection.Open()
    $command = $serverConnection.CreateCommand(); $command.CommandText = 'CREATE DATABASE [' + $testDatabase + ']'; $command.ExecuteNonQuery() | Out-Null
    $created = $true
    $builder['Initial Catalog'] = $testDatabase
    $testConnection = New-Object System.Data.SqlClient.SqlConnection($builder.ConnectionString)
    try {
        $testConnection.Open()
        $fixtures = @'
CREATE TABLE dbo.dm_User(Id NVARCHAR(50) PRIMARY KEY,DisplayName NVARCHAR(200));
CREATE TABLE dbo.dm_Departments(Id NVARCHAR(50) PRIMARY KEY,DisplayName NVARCHAR(200),IsActive BIT);
CREATE TABLE dbo.dm_Group(Id INT IDENTITY PRIMARY KEY,DisplayName NVARCHAR(100),Describe NVARCHAR(500),Prioritize INT,IdDept NVARCHAR(50));
CREATE TABLE dbo.dm_GroupUser(Id INT IDENTITY PRIMARY KEY,IdGroup INT,IdUser NVARCHAR(50));
CREATE TABLE dbo.dm_Function(Id INT PRIMARY KEY,IdParent INT,DisplayName NVARCHAR(200),ControlName NVARCHAR(100),Prioritize INT,Status BIT,Images NVARCHAR(100));
CREATE TABLE dbo.dm_FunctionRole(Id INT IDENTITY PRIMARY KEY,IdFunction INT,IdRole INT);
CREATE TABLE dbo.sys_StaticValue(KeyT NVARCHAR(100),ValueT NVARCHAR(100));
INSERT dbo.dm_User VALUES('manager','Manager');
INSERT dbo.dm_Departments VALUES('D1','Department 1',1),('D2','Department 2',1);
INSERT dbo.dm_Function VALUES(100,2,'ISO','mnu201_ISO',1,1,NULL);
INSERT dbo.dm_FunctionRole(IdFunction,IdRole) VALUES(100,1);
INSERT dbo.sys_StaticValue VALUES('RoleISOAuditDocsMain','100');
INSERT dbo.dm_Group(DisplayName,IdDept) VALUES(N'溫濕度組','D1'),(N'溫濕度組','D2'),(N'溫濕度【編輯】','D1'),(N'溫濕度【編輯】','D2'),(N'溫濕度【處理】','D1'),(N'溫濕度【管理】','*');
INSERT dbo.dm_GroupUser(IdGroup,IdUser) VALUES(1,'viewer1'),(2,'viewer2'),(3,'editor1'),(3,'editorBoth'),(4,'editorBoth'),(5,'handler1'),(6,'admin');
'@
        $fixtureCommand = $testConnection.CreateCommand(); $fixtureCommand.CommandText = $fixtures; $fixtureCommand.ExecuteNonQuery() | Out-Null
        $script = [System.IO.File]::ReadAllText((Join-Path $workspacePath 'DBScripts/20261013_add_208_lab_environment.sql'))
        # Run twice to verify idempotent installation.
        foreach ($iteration in 1..2) {
            foreach ($batch in [regex]::Split($script, '(?im)^\s*GO\s*$')) {
                if ([string]::IsNullOrWhiteSpace($batch)) { continue }
                $batchCommand = $testConnection.CreateCommand(); $batchCommand.CommandTimeout = 30
                $batchCommand.CommandText = $batch; $batchCommand.ExecuteNonQuery() | Out-Null
            }
        }
        $menuCommand = $testConnection.CreateCommand(); $menuCommand.CommandText = "SELECT COUNT(*) FROM dbo.dm_Function root JOIN dbo.dm_Function child ON child.IdParent=root.Id WHERE root.ControlName='mnu208_LabEnvironment' AND root.IdParent=100 AND child.ControlName='uc208_LabEnvironment'"
        if ($menuCommand.ExecuteScalar() -ne 1) { throw 'Module menu is not correctly registered inside ISO' }
    } finally { $testConnection.Dispose() }
    $vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio/Installer/vswhere.exe'
    $installation = & $vswhere -latest -products * -requires Microsoft.Component.MSBuild -property installationPath
    $compiler = Join-Path $installation 'MSBuild/Current/Bin/Roslyn/csc.exe'
    $refs = @('BusinessLayer.dll','DataAccessLayer.dll','EntityFramework.dll') | ForEach-Object { '/r:' + (Join-Path $binPath $_) }
    & $compiler /nologo /target:exe "/out:$exePath" /r:System.Data.dll $refs (Join-Path $PSScriptRoot '208_IntegrationTests.cs')
    if ($LASTEXITCODE -ne 0) { throw 'Test compilation failed' }
    $env:LAB208_TEST_CONNECTION = $builder.ConnectionString
    & $exePath
    if ($LASTEXITCODE -ne 0) { throw 'Integration tests failed' }
} finally {
    Remove-Item Env:/LAB208_TEST_CONNECTION -ErrorAction SilentlyContinue
    Remove-Item -LiteralPath $exePath -ErrorAction SilentlyContinue
    if ($created) {
        [System.Data.SqlClient.SqlConnection]::ClearAllPools()
        $cleanup = $serverConnection.CreateCommand()
        $cleanup.CommandText = 'ALTER DATABASE [' + $testDatabase + '] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [' + $testDatabase + ']'
        $cleanup.ExecuteNonQuery() | Out-Null
    }
    $serverConnection.Dispose()
}
