setx DB_RUS "localhost:6000"
setx DB_EU "localhost:6001"
setx DB_OTHER "localhost:6002"

cd ..\..\Redis\
start "DB_KEYMAP" redis-server
start "DB_RUS" redis-server --port 6000
start "DB_EU" redis-server --port 6001
start "DB_OTHER" redis-server --port 6002