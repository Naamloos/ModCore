# BUILD
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src
COPY ../ ./
ARG SENTRY_AUTH_TOKEN
ENV SENTRY_AUTH_TOKEN=$SENTRY_AUTH_TOKEN
RUN dotnet restore ./ModCore.Services.Shard
RUN dotnet publish ./ModCore.Services.Shard -c Release -o out

# RUNNER IMAGE
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine
WORKDIR /app
COPY --from=build /src/out .
ENTRYPOINT ["dotnet", "/app/ModCore.Services.Shard.dll"]