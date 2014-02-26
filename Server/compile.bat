Set GOPATH=C:\Users\sunmock\Dropbox\Project Rollout\Server\OrbsServer
go build -a -o main.exe OrbsServer\src\main.go

:go build -a -o main.exe GoServer\src\main.go

:Set GOARCH=arm
:Set GOOS=linux
:Set GOARM=5