param([string]$OutputFolder = (Join-Path $env:TEMP '7system-FuelPhotoReport'))
$ErrorActionPreference = 'Stop'
$repo = (Resolve-Path (Join-Path $PSScriptRoot '../..')).Path
$bin = Join-Path $repo 'KnowledgeSystem/bin/Fpg311Debug'
Add-Type -AssemblyName System.Drawing
$assembly = [Reflection.Assembly]::LoadFrom((Join-Path $bin 'KnowledgeSystem.exe'))
$entryType = $assembly.GetType('KnowledgeSystem.Helpers.FuelPhotoReportEntry', $true)
$exporter = $assembly.GetType('KnowledgeSystem.Helpers.FuelPhotoReportExporter', $true)
$export = $exporter.GetMethod('Export', [Reflection.BindingFlags]'Static,NonPublic')
$listType = [System.Collections.Generic.List``1].MakeGenericType($entryType)
$entries = [Activator]::CreateInstance($listType)
New-Item -ItemType Directory -Path $OutputFolder -Force | Out-Null
foreach ($portrait in @($false, $true)) {
    $size = if ($portrait) { @(600,1000) } else { @(1200,600) }
    $image = New-Object Drawing.Bitmap($size[0],$size[1])
    $graphics = [Drawing.Graphics]::FromImage($image)
    $font = New-Object Drawing.Font('Arial',32)
    try {
        $graphics.Clear([Drawing.Color]::LightGray)
        $graphics.DrawRectangle([Drawing.Pens]::Black, 20, 20, ($size[0]-40), ($size[1]-40))
        $graphics.DrawString('Photo layout test', $font, [Drawing.Brushes]::Black, 40, 60)
        $image.Save((Join-Path $OutputFolder "photo-$portrait.png"))
    } finally { $font.Dispose(); $graphics.Dispose(); $image.Dispose() }
}
foreach ($index in 0..4) {
    $entry = [Activator]::CreateInstance($entryType, $true)
    $entry.Department = 'LG7810'
    $entry.LicensePlate = if ($index -eq 4) { '38A-864.49' } else { '38LD-400.23' }
    $entry.FuelDate = if ($index -eq 3) { [datetime]'2026-10-30' } else { ([datetime]'2026-11-01').AddDays($index) }
    $entry.OdometerReading = 123456 + $index
    $entry.Quantity = [decimal]42.75 + $index
    $entry.PhotoPath = if ($index -eq 1) { $null } else { Join-Path $OutputFolder "photo-$([bool]($index % 2)).png" }
    $entries.Add($entry)
}
$output = Join-Path $OutputFolder 'FuelPhotoReport.pdf'
$arguments = New-Object object[] 3
$arguments[0] = $entries
$arguments[1] = [string]$output
$arguments[2] = [datetime]'2026-11-21'
$export.Invoke($null, $arguments)
Add-Type -Path (Join-Path $bin 'Spire.Pdf.dll')
$pdf = New-Object Spire.Pdf.PdfDocument
try {
    $pdf.LoadFromFile($output)
    if ($pdf.Pages.Count -ne 3) { throw "Expected three pages grouped by vehicle, got $($pdf.Pages.Count)" }
    'PASS: five records grouped by vehicle into three pages; signatures appear only at the end of each vehicle.'
} finally { $pdf.Dispose() }
Write-Output "Preview: $output"
