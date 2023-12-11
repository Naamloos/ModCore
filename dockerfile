# BUILD
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
ARG BUILD_VER
WORKDIR /src
COPY ./ModCore ./
RUN dotnet restore
RUN dotnet publish -c Release -o out /p:VersionPrefix=${BUILD_VER}

# RUNNER IMAGE
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine
WORKDIR /app
COPY --from=build /src/out .
WORKDIR /config

# ADD FFMPEG FOR VOICE SUPPORT
RUN apk add ffmpeg
# FFMPEG is not actually used but fukit

# ADD TESSERACT FOR OCR
RUN apk add tesseract-ocr
RUN apk add leptonica-dev

RUN ln -s /usr/lib/libleptonica.so /app/x64/libleptonica-1.82.0.so
RUN ln -s /usr/lib/libtesseract.so.5 /app/x64/libtesseract50.so

ENTRYPOINT ["dotnet", "/app/ModCore.dll"]
