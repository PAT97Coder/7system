[CmdletBinding()]
param(
    [ValidateRange(0, 99)]
    [int]$BuildIndex = 0,
    [switch]$OpenOutput,
    [switch]$Preview,
    [string]$OutputDirectory
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
$repositoryRoot = Split-Path -Parent $PSScriptRoot
$applicationRoot = Join-Path $repositoryRoot 'KnowledgeSystem'
$applicationProject = Join-Path $applicationRoot 'KnowledgeSystem.csproj'
$assemblyInfoPath = Join-Path $applicationRoot 'Properties\AssemblyInfo.cs'
$releaseOutput = Join-Path $PSScriptRoot 'Staging'
$installerProject = Join-Path $repositoryRoot 'SetupDocumentSystem\SetupDocumentSystem.aip'
$versionStatePath = Join-Path $PSScriptRoot 'version.json'
if (-not $OutputDirectory) {
    $systemRoot = Split-Path -Parent (Split-Path -Parent $repositoryRoot)
    $OutputDirectory = Join-Path $systemRoot '04. Setup files'
}
$OutputDirectory = [IO.Path]::GetFullPath($OutputDirectory)
$packagePath = Join-Path $OutputDirectory 'SetupDocumentSystem.exe'

function Invoke-NativeTool {
    param([string]$FilePath, [string[]]$ToolArguments)
    & $FilePath @ToolArguments
    if ($LASTEXITCODE -ne 0) { throw "Lệnh thất bại ($LASTEXITCODE): $FilePath" }
}

function Find-MSBuild {
    $vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
    if (Test-Path -LiteralPath $vswhere) {
        $installation = & $vswhere -latest -products * -requires Microsoft.Component.MSBuild -property installationPath
        if ($installation) {
            $candidate = Join-Path $installation 'MSBuild\Current\Bin\MSBuild.exe'
            if (Test-Path -LiteralPath $candidate) { return $candidate }
        }
    }
    $command = Get-Command MSBuild.exe -ErrorAction SilentlyContinue
    if ($command) { return $command.Source }
    throw 'Không tìm thấy MSBuild. Hãy cài Visual Studio hoặc Build Tools.'
}

function Find-AdvancedInstaller {
    if ($env:ADVANCED_INSTALLER_PATH) {
        if (Test-Path -LiteralPath $env:ADVANCED_INSTALLER_PATH -PathType Leaf) {
            return $env:ADVANCED_INSTALLER_PATH
        }
        throw 'ADVANCED_INSTALLER_PATH không trỏ tới AdvancedInstaller.com.'
    }
    $candidates = @(foreach ($root in @(${env:ProgramFiles(x86)}, $env:ProgramFiles)) {
        $caphyon = Join-Path $root 'Caphyon'
        if (Test-Path -LiteralPath $caphyon) {
            Get-ChildItem -LiteralPath $caphyon -Filter AdvancedInstaller.com -Recurse -File
        }
    })
    $candidate = $candidates | Sort-Object { [version]$_.VersionInfo.FileVersion } -Descending | Select-Object -First 1
    if ($candidate) { return $candidate.FullName }
    throw 'Không tìm thấy Advanced Installer. Hãy cài hoặc đặt ADVANCED_INSTALLER_PATH.'
}

if (-not (Test-Path -LiteralPath $installerProject)) {
    throw "Thiếu project Advanced Installer hiện có: $installerProject"
}
$msbuild = Find-MSBuild
$advancedInstaller = Find-AdvancedInstaller
# Dùng ngày Việt Nam, không phụ thuộc múi giờ của máy build.
$today = [TimeZoneInfo]::ConvertTimeBySystemTimeZoneId([DateTime]::UtcNow, 'SE Asia Standard Time')
$datePart = $today.ToString('yy.MM.dd')
$previousIndex = 0
if (Test-Path -LiteralPath $versionStatePath) {
    $state = Get-Content -LiteralPath $versionStatePath -Raw -Encoding UTF8 | ConvertFrom-Json
    if ($state.date -eq $datePart) { $previousIndex = [int]$state.index }
}
$assemblyOriginal = [IO.File]::ReadAllText($assemblyInfoPath)
$assemblyVersion = [regex]::Match($assemblyOriginal, '(?m)^\[assembly: AssemblyFileVersion\("([^"]+)"\)\]')
if ($assemblyVersion.Success -and $assemblyVersion.Groups[1].Value.StartsWith($datePart + '.')) {
    $previousIndex = [Math]::Max($previousIndex, [int]($assemblyVersion.Groups[1].Value.Split('.')[-1]))
}
$index = $BuildIndex
if ($index -eq 0) { $index = $previousIndex + 1 }
if ($index -le $previousIndex -or $index -gt 99) {
    throw "BuildIndex phải lớn hơn $previousIndex và không vượt quá 99."
}
$version = "$datePart.$index"
# MSI chỉ so sánh ba phần version; mã hóa ngày và index vào phần thứ ba.
$msiVersion = '{0}.{1}.{2}' -f ($today.Year % 100), $today.Month, ($today.Day * 100 + $index)
Write-Host "Phiên bản ứng dụng: $version; MSI: $msiVersion"
Write-Host "Bộ cài: $packagePath"
Write-Host "MSBuild: $msbuild"
Write-Host "Advanced Installer: $advancedInstaller"
if ($Preview) { return }

$utf8Bom = [Text.UTF8Encoding]::new($true)
$temporaryProject = Join-Path (Split-Path -Parent $installerProject) ('Build-' + [guid]::NewGuid().ToString('N') + '.aip')
$completed = $false
try {
    $assemblyContent = $assemblyOriginal
    foreach ($attribute in @('AssemblyVersion', 'AssemblyFileVersion')) {
        $pattern = '(?m)^\[assembly: ' + $attribute + '\("[^"]+"\)\]'
        if (-not [regex]::IsMatch($assemblyContent, $pattern)) { throw "Không tìm thấy $attribute trong AssemblyInfo.cs." }
        $assemblyContent = [regex]::Replace($assemblyContent, $pattern, "[assembly: $attribute(`"$version`")]")
    }
    [IO.File]::WriteAllText($assemblyInfoPath, $assemblyContent, $utf8Bom)
    # Build project ứng dụng cùng các thư viện; tránh build hai bộ cài trong solution.
    $stagingFullPath = [IO.Path]::GetFullPath($releaseOutput)
    $installerRoot = [IO.Path]::GetFullPath($PSScriptRoot).TrimEnd('\') + '\'
    if (-not $stagingFullPath.StartsWith($installerRoot, [StringComparison]::OrdinalIgnoreCase)) {
        throw 'Thư mục staging nằm ngoài thư mục Installer.'
    }
    if (Test-Path -LiteralPath $stagingFullPath) { Remove-Item -LiteralPath $stagingFullPath -Recurse -Force }
    New-Item -ItemType Directory -Path $stagingFullPath -Force | Out-Null
    Invoke-NativeTool $msbuild @($applicationProject, '/t:Rebuild', '/p:Configuration=Release', '/p:Platform=x64', '/p:BuildProjectReferences=true', "/p:OutDir=$stagingFullPath/", '/nologo', '/v:minimal')
    $applicationExe = Join-Path $releaseOutput 'KnowledgeSystem.exe'
    if (-not (Test-Path -LiteralPath $applicationExe)) { throw "Thiếu output: $applicationExe" }
    if ([Diagnostics.FileVersionInfo]::GetVersionInfo($applicationExe).FileVersion -ne $version) {
        throw 'Version ứng dụng sau build không khớp version bộ cài.'
    }

    # Giữ nguyên cấu hình cài đặt, shortcut và UpgradeCode của project sẵn có.
    # Bản làm việc nằm cạnh project gốc để các đường dẫn tương đối vẫn đúng.
    $aipContent = [IO.File]::ReadAllText($installerProject)
    $aipContent = $aipContent.Replace('..\KnowledgeSystem\bin\Debug', '..\Installer\Staging')
    $aipContent = $aipContent.Replace('..\KnowledgeSystem\bin\x64\Release', '..\Installer\Staging')
    [IO.File]::WriteAllText($temporaryProject, $aipContent, $utf8Bom)
    # Project cũ có cả Emgu x86; bản Release x64 chỉ sinh runtime x64.
    $aipDocument = [xml]$aipContent
    # Các thư viện project có thể trỏ thẳng tới BusinessLayer/DataAccessLayer/bin/Debug.
    # OutDir đã gom toàn bộ output Release vào staging, nên chuyển cả các đường dẫn này.
    foreach ($row in $aipDocument.SelectNodes('//ROW[starts-with(@Property,"AI_PROPPATH_DIR_PERBUILD_")]')) {
        $sourceDirectory = $row.GetAttribute('Value')
        if ($sourceDirectory -match '^\.\.\\[^\\]+\\bin\\(?:x64\\)?(?:Debug|Release)(?<suffix>\\.*)?$') {
            $row.SetAttribute('Value', '..\Installer\Staging' + $Matches['suffix'])
        }
    }
    $remainingDebugDirectories = @($aipDocument.SelectNodes('//ROW[starts-with(@Property,"AI_PROPPATH_DIR_PERBUILD_")]') |
        Where-Object { $_.GetAttribute('Value') -match '\\bin\\(?:x64\\)?Debug(?:\\|$)' })
    if ($remainingDebugDirectories.Count -gt 0) {
        throw 'Project installer vẫn tham chiếu output Debug; dừng để tránh đóng gói nhầm thư viện.'
    }
    $x86Files = @($aipDocument.SelectNodes('//ROW[starts-with(@Property,"AI_PROPPATH_DIR_PERBUILD_")]') |
        Where-Object { $_.GetAttribute('Value').EndsWith('\x86') } |
        ForEach-Object { $_.GetAttribute('Property').Substring('AI_PROPPATH_DIR_PERBUILD_'.Length) })
    foreach ($row in @($aipDocument.SelectNodes('//ROW[@File or @OutputFile]'))) {
        if ($x86Files -contains $row.GetAttribute('File') -or $x86Files -contains $row.GetAttribute('OutputFile')) {
            $row.ParentNode.RemoveChild($row) | Out-Null
        }
    }
    $aipDocument.Save($temporaryProject)
    Invoke-NativeTool $advancedInstaller @('/edit', $temporaryProject, '/SetVersion', $msiVersion)
    Invoke-NativeTool $advancedInstaller @('/edit', $temporaryProject, '/SetProperty', "FULLPRODUCTVERSION=$version")
    Invoke-NativeTool $advancedInstaller @('/edit', $temporaryProject, '/SetOutputType', 'ExeInside', '-buildname', 'DefaultBuild')
    Invoke-NativeTool $advancedInstaller @('/edit', $temporaryProject, '/SetPackageName', 'SetupDocumentSystem.msi', '-buildname', 'DefaultBuild')
    Invoke-NativeTool $advancedInstaller @('/edit', $temporaryProject, '/SetPackageName', 'SetupDocumentSystem.exe', '-buildname', 'DefaultBuild')
    Invoke-NativeTool $advancedInstaller @('/edit', $temporaryProject, '/SetOutputLocation', '-buildname', 'DefaultBuild', '-path', $OutputDirectory)
    New-Item -ItemType Directory -Path $OutputDirectory -Force | Out-Null
    $buildStarted = [DateTime]::UtcNow
    Invoke-NativeTool $advancedInstaller @('/rebuild', $temporaryProject, '-buildslist', 'DefaultBuild')
    if (-not (Test-Path -LiteralPath $packagePath) -or (Get-Item -LiteralPath $packagePath).LastWriteTimeUtc -lt $buildStarted) {
        throw 'Không tìm thấy bộ cài mới; không ghi nhận version thành công.'
    }
    Copy-Item -LiteralPath $temporaryProject -Destination $installerProject -Force
    [ordered]@{ date = $datePart; index = $index; version = $version } |
        ConvertTo-Json | Set-Content -LiteralPath $versionStatePath -Encoding UTF8
    $completed = $true
    Write-Host "Đã tạo bộ cài: $packagePath" -ForegroundColor Green
} finally {
    if (-not $completed) { [IO.File]::WriteAllText($assemblyInfoPath, $assemblyOriginal, $utf8Bom) }
    if (Test-Path -LiteralPath $temporaryProject) { Remove-Item -LiteralPath $temporaryProject -Force }
    $cachePath = [IO.Path]::ChangeExtension($temporaryProject, $null) + '-cache'
    $installerFolder = [IO.Path]::GetFullPath((Split-Path -Parent $installerProject)).TrimEnd('\') + '\'
    $cacheFullPath = [IO.Path]::GetFullPath($cachePath)
    if (-not $cacheFullPath.StartsWith($installerFolder, [StringComparison]::OrdinalIgnoreCase)) {
        throw 'Đường dẫn cache nằm ngoài thư mục installer.'
    }
    if (Test-Path -LiteralPath $cacheFullPath) { Remove-Item -LiteralPath $cacheFullPath -Recurse -Force }
}
if ($OpenOutput) { Start-Process explorer.exe -ArgumentList @($OutputDirectory) }
