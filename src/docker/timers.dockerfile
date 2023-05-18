# BUILD
FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS build
WORKDIR /src
COPY ../ ./
RUN dotnet restore ./ModCore.Services.Timers
RUN dotnet publish ./ModCore.Services.Timers -c Release -o out

# RUNNER IMAGE
FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine
WORKDIR /app
COPY --from=build /src/out .
ENTRYPOINT ["dotnet", "/app/ModCore.Services.Timers.dll"]