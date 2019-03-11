sc query |find /i "SocketServer" >nul 2>nul 
if not errorlevel 1 (goto exist)

:exist
sc stop SocketServer
sc delete SocketServer