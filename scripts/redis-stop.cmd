reg delete "HKCU\Environment" /F /V DB_RUS
reg delete "HKCU\Environment" /F /V DB_EU
reg delete "HKCU\Environment" /F /V DB_OTHER

taskkill /f /im redis-server.exe