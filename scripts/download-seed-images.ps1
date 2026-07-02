param(
    [int]$MinBytes = 10240,
    [int]$DelaySeconds = 5
)

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
$manifestPath = Join-Path $root "backend\MemeApp.Api\SeedImages\manifest.json"
$outputDir = Join-Path $root "backend\MemeApp.Api\SeedImages"
$userAgent = "MemeAppSeed/1.0"

Add-Type -AssemblyName System.Drawing

$entries = Get-Content $manifestPath -Raw | ConvertFrom-Json
$failed = @()

function Save-GeneratedImage {
    param(
        [string]$Path,
        [string]$Label,
        [System.Drawing.Color]$Color
    )

    $bitmap = New-Object System.Drawing.Bitmap 800, 600
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.Clear($Color)
    $font = New-Object System.Drawing.Font("Segoe UI", 36, [System.Drawing.FontStyle]::Bold)
    $brush = [System.Drawing.Brushes]::White
    $size = $graphics.MeasureString($Label, $font)
    $x = (800 - $size.Width) / 2
    $y = (600 - $size.Height) / 2
    $graphics.DrawString($Label, $font, $brush, $x, $y)
    $graphics.Dispose()
    $bitmap.Save($Path, [System.Drawing.Imaging.ImageFormat]::Jpeg)
    $bitmap.Dispose()
}

$fallbackColors = @{
    "cat.jpg" = [System.Drawing.Color]::FromArgb(180, 140, 220)
    "drake.jpg" = [System.Drawing.Color]::FromArgb(30, 60, 120)
    "success.jpg" = [System.Drawing.Color]::FromArgb(120, 180, 230)
    "harold.jpg" = [System.Drawing.Color]::FromArgb(240, 190, 120)
    "fine.png" = [System.Drawing.Color]::FromArgb(220, 80, 60)
    "brain.jpg" = [System.Drawing.Color]::FromArgb(90, 150, 220)
    "stonks.jpg" = [System.Drawing.Color]::FromArgb(70, 180, 100)
}

foreach ($entry in $entries) {
    $outPath = Join-Path $outputDir $entry.fileName
    $jpgPath = if ($entry.fileName.EndsWith(".png")) {
        Join-Path $outputDir ($entry.fileName -replace '\.png$', '.jpg')
    } else {
        $outPath
    }

    if ((Test-Path $jpgPath) -and ((Get-Item $jpgPath).Length -ge $MinBytes)) {
        Write-Host "SKIP $($entry.fileName) (already valid)"
        continue
    }

    Write-Host "Downloading $($entry.title) from $($entry.source)..."
    $downloadPath = if ($entry.fileName.EndsWith(".png")) { $outPath } else { $jpgPath }

    curl.exe -L -A $userAgent -o $downloadPath $entry.url | Out-Null
    Start-Sleep -Seconds $DelaySeconds

    if ($entry.fileName.EndsWith(".png") -and (Test-Path $downloadPath)) {
        $png = [System.Drawing.Image]::FromFile($downloadPath)
        $png.Save($jpgPath, [System.Drawing.Imaging.ImageFormat]::Jpeg)
        $png.Dispose()
        Remove-Item $downloadPath -ErrorAction SilentlyContinue
    }

    $finalPath = $jpgPath
    if (-not (Test-Path $finalPath)) {
        $finalPath = $outPath
    }

    $size = if (Test-Path $finalPath) { (Get-Item $finalPath).Length } else { 0 }

    if ($size -lt $MinBytes) {
        Write-Host "  Fallback generate for $($entry.title)"
        $color = $fallbackColors[$entry.fileName]
        if (-not $color) { $color = [System.Drawing.Color]::FromArgb(100, 100, 180) }
        $target = if ($entry.fileName -eq "fine.png") { Join-Path $outputDir "fine.jpg" } else { $finalPath }
        Save-GeneratedImage -Path $target -Label $entry.title -Color $color
        $size = (Get-Item $target).Length
    }

    if ($size -lt $MinBytes) {
        $failed += $entry.fileName
    } else {
        Write-Host "  OK: $finalPath ($size bytes)"
    }
}

if ($failed.Count -gt 0) {
    Write-Error "Failed: $($failed -join ', ')"
    exit 1
}

Write-Host "Seed images ready."
