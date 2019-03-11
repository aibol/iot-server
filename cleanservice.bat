@echo off
sc query |find /i "SocketServer" >nul 2>nul
if not errorlevel 1 (goto exist) else goto notexist

:exist
sc stop SocketServer
sc delete SocketServer

:notexist
echo pass delete SocketServer
