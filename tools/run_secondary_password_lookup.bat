@echo off
setlocal
cd /d "%~dp0"

set "CSC=%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
if not exist "%CSC%" set "CSC=%WINDIR%\Microsoft.NET\Framework\v4.0.30319\csc.exe"

if not exist "%CSC%" (
    echo Khong tim thay trinh bien dich C# cua .NET Framework.
    pause
    exit /b 1
)

"%CSC%" /nologo /target:winexe /optimize+ /codepage:65001 ^
    /out:"SecondaryPasswordLookup.exe" ^
    /reference:System.dll ^
    /reference:System.Data.dll ^
    /reference:System.Drawing.dll ^
    /reference:System.Windows.Forms.dll ^
    "SecondaryPasswordLookup.cs"

if errorlevel 1 (
    echo Bien dich that bai.
    pause
    exit /b 1
)

start "" "%~dp0SecondaryPasswordLookup.exe"
