$msbuild = "C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"
& $msbuild "SwitchKeyboardTray.sln" /t:Build /p:Configuration=Debug
if ($LASTEXITCODE -ne 0) {
    exit $LASTEXITCODE
}

& ".\tests\SwitchKeyboardTray.Tests\bin\Debug\SwitchKeyboardTray.Tests.exe"
exit $LASTEXITCODE
