@echo off
setlocal enabledelayedexpansion

:: ==================== CONFIGURATION ====================
:: Path to your Hardhat project root (where hardhat.config.js lives)
set "HARDHAT_ROOT=./"
:: Output folder for stripped ABI files (relative to this script location)
set "TARGET_DIR=../Contracts"
:: =======================================================

set "CONTRACT_NAME=%1"
set "ARTIFACT_FILE=%HARDHAT_ROOT%artifacts\contracts\%CONTRACT_NAME%.sol\%CONTRACT_NAME%.json"
set "TARGET_FILE=%TARGET_DIR%\%CONTRACT_NAME%.json"


echo ==========================================
echo  DeployContract.bat
echo  Contract: %CONTRACT_NAME%
echo ==========================================

:: ------------------------------------------------------------------
:: Step 1: Compile with Hardhat
:: ------------------------------------------------------------------
echo [1/4] Compiling contracts with Hardhat...
cd /d "%~dp0%HARDHAT_ROOT%"
call npx hardhat compile
if errorlevel 1 (
    echo.
    echo ERROR: Hardhat compile failed.
    exit /b 1
)

:: ------------------------------------------------------------------
:: Step 2: Copy artifact to Contracts folder
:: ------------------------------------------------------------------
echo [2/4] Copying artifact to %TARGET_DIR%\
cd /d "%~dp0"
if not exist "%TARGET_DIR%" mkdir "%TARGET_DIR%"
if not exist "%ARTIFACT_FILE%" (
    echo ERROR: Artifact not found at %ARTIFACT_FILE%
    echo       Make sure the contract name is correct and compilation succeeded.
    exit /b 1
)
copy /y "%ARTIFACT_FILE%" "%TARGET_FILE%" >nul
echo       Copied to %TARGET_FILE%

:: ------------------------------------------------------------------
:: Step 3: Strip file to ABI only (line-by-line, from "abi:[" to closing bracket)
:: ------------------------------------------------------------------
echo [3/4] Stripping to ABI only...
powershell -NoProfile -Command ^
    $path = "%TARGET_FILE%"; ^
    $lines = Get-Content $path; ^
    $capturing = $false; ^
    $depth = 0; ^
    $result = New-Object System.Collections.Generic.List[string]; ^
    $done = $false; ^
    foreach ($line in $lines) { ^
        if (-not $done) { ^
            if (-not $capturing) { ^
                $idx = $line.IndexOf("abi:["); ^
                if ($idx -ge 0) { ^
                    $capturing = $true; ^
                    $startPos = $idx + 5; ^
                    $rest = $line.Substring($startPos); ^
                    $result.Add("[" + $rest); ^
                    $depth = 1; ^
                    for ($j = $startPos; $j -lt $line.Length; $j++) { ^
                        if ($line[$j] -eq '[') { $depth++ } ^
                        elseif ($line[$j] -eq ']') { $depth-- } ^
                        if ($depth -le 0) { $done = $true; break } ^
                    } ^
                } ^
            } ^
        } else { ^
            $result.Add($line); ^
            for ($j = 0; $j -lt $line.Length; $j++) { ^
                if ($line[$j] -eq '[') { $depth++ } ^
                elseif ($line[$j] -eq ']') { $depth-- } ^
            } ^
            if ($depth -le 0) { $done = $true } ^
        } ^
    } ^
    $out = ($result -join "`n"); ^
    Set-Content -Path $path -Value $out -Encoding UTF8; ^
    Write-Host "       ABI extracted."

:: ------------------------------------------------------------------
:: Step 4: Done
:: ------------------------------------------------------------------
echo [4/4] Done.
echo       ABI file: %TARGET_FILE%
endlocal