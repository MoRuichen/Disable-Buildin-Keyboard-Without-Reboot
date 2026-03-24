$releaseDir = Join-Path $PSScriptRoot "..\src\SwitchKeyboardTray\bin\Release"
$distRoot = Join-Path $PSScriptRoot "..\dist"
$distDir = Join-Path $distRoot "SwitchKeyboardTray"
$zipPath = Join-Path $distRoot "SwitchKeyboardTray.zip"
$installer = Join-Path $PSScriptRoot "..\vendor\Interception\Interception\command line installer\install-interception.exe"
$dll = Join-Path $releaseDir "interception.dll"
if (-not (Test-Path $dll)) {
    $dll = Join-Path $PSScriptRoot "..\vendor\Interception\Interception\library\x64\interception.dll"
}

if (Test-Path $distDir) {
    Remove-Item $distDir -Recurse -Force
}

if (Test-Path $zipPath) {
    Remove-Item $zipPath -Force
}

New-Item -ItemType Directory -Force $distDir | Out-Null
Copy-Item (Join-Path $releaseDir "SwitchKeyboardTray.exe") $distDir -Force
Copy-Item $dll $distDir -Force
Copy-Item $installer $distDir -Force
Copy-Item (Join-Path $PSScriptRoot "..\README.md") $distDir -Force

Compress-Archive -Path (Join-Path $distDir "*") -DestinationPath $zipPath -Force

Get-ChildItem $distDir
