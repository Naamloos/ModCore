# BUILD
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src
COPY ../ ./
RUN dotnet restore ./ModCore.Services.Jobs
RUN dotnet publish ./ModCore.Services.Jobs -c Release -o out

# RUNNER IMAGE
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine
WORKDIR /app
COPY --from=build /src/out .
ENTRYPOINT ["dotnet", "/app/ModCore.Services.Jobs.dll"]