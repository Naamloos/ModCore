# BUILD
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src
COPY ./ModCore ./
RUN dotnet restore
RUN dotnet publish -c Release -o out

# RUNNER IMAGE
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine
WORKDIR /app
COPY --from=build /src/out .
WORKDIR /config
# ADD FFMPEG FOR VOICE SUPPORT
RUN apk add ffmpeg
ENTRYPOINT ["dotnet", "/app/ModCore.dll"]
