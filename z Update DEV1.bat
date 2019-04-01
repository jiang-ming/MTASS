@echo off
rem set CONFIRM=
rem set /P CONFIRM=Type 'deploy' to confirm (target folder will be overwritten): %=%
rem echo.
rem if not "%CONFIRM%"=="deploy" echo user cancelled
rem if not "%CONFIRM%"=="deploy" goto :END

set SOURCEDIR=C:\INETPUB\WWWROOT\MTASS
set TARGETDIR=\\www1\wwwroot\MTASS
cd /d %SOURCEDIR%
echo Current Directory is
cd
echo.
echo.
dir cvs*.* /a/s/b > c:\exclude.txt
xcopy %SOURCEDIR%\*.* %TARGETDIR%\*.* /e/s/v/d/y/f/i/c/h/r/Exclude:c:\exclude.txt
echo.
echo.
echo Removing CVS control files...
dir %TARGETDIR%\cvs* /a/d/s/b > temp.txt
for /f %%G in (temp.txt) do call :ProcessFolder "%%G"
del temp.txt
del c:\exclude.txt
goto :end

:ProcessFolder
   echo Processing %1
   rd /s/q %1
goto :EOF


:end
@ping -n 6 localhost > nul
