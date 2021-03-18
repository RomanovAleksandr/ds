start /d ..\Valuator\ dotnet run --no-build --urls "http://localhost:5001"
start /d ..\Valuator\ dotnet run --no-build --urls "http://localhost:5002"

start /d ..\nginx\ nginx.exe

start /d ..\RankCalculator\ dotnet run --no-build
start /d ..\RankCalculator\ dotnet run --no-build

start /d ..\EventsLogger\ dotnet run --no-build
start /d ..\EventsLogger\ dotnet run --no-build