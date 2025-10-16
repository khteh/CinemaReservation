# Cinema Reservation System

An .NET 9.0 console application for cinema movie tickets reservation and management.

## Class Diagram

![Class Diagram](./CinemaReservation_ClassDiagram.jpg?raw=true "Class Diagram")

## Sequence Diagram

![Sequence Diagram](./CinemaReservation_SequenceDiagram.jpg?raw=true "Sequence Diagram")

## Prerequisites

### Windows

- Create `/var/log/cinema` folder.

## Development environment

- Copy `nuget.config.FIXME` to `nuget.config`
- Add `Username` and access token from github Developer Settings

### Visual Studio Code

- `Ctrl`+`Shift`+`B` to build
- `F5` to start debug session

## Logs

- logs are available at `/var/log/cinema/logYYYYMMDD_*`

### Windows 11

- Enter powershell: `powershell`
- `Get-Content -Path "c:\var\log\cinema\logYYYYMMDD_<foo>" -Wait`
