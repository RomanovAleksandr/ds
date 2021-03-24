taskkill /f /im valuator.exe
start /d "../nginx/"  nginx.exe -s quit
taskkill /f /im RankCalculator.exe
taskkill /f /im EventsLogger.exe