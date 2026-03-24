$msbuild = "C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"
& $msbuild "SwitchKeyboardTray.sln" /t:Build /p:Configuration=Release
if ($LASTEXITCODE -ne 0) {
    exit $LASTEXITCODE
}
