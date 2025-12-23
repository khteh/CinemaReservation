FROM mcr.microsoft.com/dotnet/runtime:10.0
LABEL org.opencontainers.image.authors="Kok How, Teh <funcoolgeeek@gmail.com>"
RUN apt update -y
RUN DEBIAN_FRONTEND=noninteractive apt install -y tzdata gnupg2 gnupg gnupg1
WORKDIR /app
ADD CinemaConsole/bin/Release/net10.0/linux-x64/publish .
#ENTRYPOINT ["dotnet", "CinemaConsole.dll"]
ENTRYPOINT ["./CinemaConsole"]
