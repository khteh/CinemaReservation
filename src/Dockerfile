FROM mcr.microsoft.com/dotnet/runtime:9.0
MAINTAINER Kok How, Teh <funcoolgeek@gmail.com>
RUN apt update -y
RUN DEBIAN_FRONTEND=noninteractive apt install -y tzdata gnupg2 gnupg gnupg1
WORKDIR /app
ADD CinemaConsole/bin/Release/net9.0/linux-x64/publish .
#ENTRYPOINT ["dotnet", "CinemaConsole.dll"]
ENTRYPOINT ["./CinemaConsole"]
