# Cinema Reservation System

An ASP.NET 9.0 console application for cinema movie tickets reservation and management.

# Development environment
- Copy `nuget.config.FIXME` to `nuget.config`
- Add `Username` and access token from github Developer Settings

# Visual Studio

- Open the solution file <code>CinemaReservations.sln</code> and build/run.

# Visual Studio Code

- `Ctrl`+`Shift`+`B` to build
- `F5` to start debug session

## Unit Testing

- Install .Net Core Test Explorer

# Logs
- logs are available at `/var/log/cinema/logYYYYMMDD_*`

## Windows 11
- Enter powershell: `powershell`
- `Get-Content -Path "c:\var\log\cinema\logYYYYMMDD_<foo>" -Wait`