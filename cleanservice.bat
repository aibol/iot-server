sc query |find /i "SocketServer" >nul 2>nul
if not errorlevel 1 (goto exist) else goto notexist

:exist
sc stop SocketServerService
sc delete SocketServerService

:notexist
echo pass delete SocketServer
