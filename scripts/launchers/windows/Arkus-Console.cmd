@echo off
setlocal
title Arkus Telegram Console
powershell.exe -NoLogo -NoProfile -ExecutionPolicy Bypass -File "%~dp0Arkus-Console.ps1"
set "ERR=%ERRORLEVEL%"
if not "%ERR%"=="0" (
  echo.
  echo Arkus Console termino con codigo %ERR%.
  echo Pulsa una tecla para cerrar...
  pause >nul
)
exit /b %ERR%
