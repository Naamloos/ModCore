FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish ModCore/ -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /config
COPY --from=build /out .
ENTRYPOINT ["dotnet", "ModCore.dll"]