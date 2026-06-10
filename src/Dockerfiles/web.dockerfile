# BUILD
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src
COPY ../ ./

WORKDIR /src/ModCore.Services.Web/ClientApp
# Install Node.js and npm
RUN apk add --no-cache nodejs npm

# Install dependencies
RUN npm install

# Build the assets
RUN npm run build

WORKDIR /src
RUN dotnet restore ./ModCore.Services.Web
RUN dotnet publish ./ModCore.Services.Web -c Release -o out

# RUNNER IMAGE
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine
WORKDIR /app
COPY --from=build /src/out .
ENTRYPOINT ["dotnet", "/app/ModCore.Services.Web.dll"]