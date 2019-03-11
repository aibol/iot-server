@echo off
sc query |find /i "SocketServer" >nul 2>nul
if not errorlevel 1 (goto exist) else goto notexist

:exist
sc stop SocketServer
sc delete SocketServer

goto :end

:notexist
echo not exist SocketServer

goto :end

:end
