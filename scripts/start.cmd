start "web-app 1" /d ..\Valuator\ dotnet run --no-build --urls "http://localhost:5001"
start "web-app 2" /d ..\Valuator\ dotnet run --no-build --urls "http://localhost:5002"

start /d ..\nginx\ nginx.exe

start "Rank Calculator 1" /d ..\RankCalculator\ dotnet run --no-build
start "Rank Calculator 2" /d ..\RankCalculator\ dotnet run --no-build

start "Events Logger 1" /d ..\EventsLogger\ dotnet run --no-build
start "Events Logger 2" /d ..\EventsLogger\ dotnet run --no-build