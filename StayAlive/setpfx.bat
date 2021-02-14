@echo off
:: vim:ts=2:sw=2:sts=2:autoindent:smartindent:expandtab:
:: setpfx.bat
:: Author: Douglas S. Elder
:: Date: 12/08/2020
:: Desc: Create a pfx file for a .NET app
::
setlocal
SET PFX="StayAlive.pfx"
SET CONTAINER="VS_KEY_EEE0E5F87CE119D"
SET SN="C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools\sn"

:loop
if "%1"=="-c" goto setContainer
if "%1"=="-d" goto setDebug
if "%1"=="-p" goto setPFX

@echo PFX=%PFX%
@echo CONTAINER=%CONTAINER%
pause
%SN% -i %PFX% %CONTAINER%
goto:eof

:setContainer
shift
if "%1"=="" usage
set CONTAINER=%1
shift
goto loop

:setDebug
shift
@echo on
goto loop

:setPFX
shift
if "%1"=="" usage
set PFX=%1
shift
goto loop

endlocal
