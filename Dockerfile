# syntax=docker/dockerfile:1
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
ARG TARGETARCH
WORKDIR /src

COPY *.sln ./
COPY LXGaming.CursedAnalytics/*.csproj LXGaming.CursedAnalytics/
RUN dotnet restore LXGaming.CursedAnalytics --arch $TARGETARCH

COPY LXGaming.CursedAnalytics/ LXGaming.CursedAnalytics/
RUN dotnet publish LXGaming.CursedAnalytics --arch $TARGETARCH --configuration Release --no-restore --output /app

FROM mcr.microsoft.com/dotnet/runtime-deps:8.0-alpine
RUN apk add --no-cache --upgrade tzdata
WORKDIR /app
COPY --from=build /app ./
USER $APP_UID
ENTRYPOINT ["./LXGaming.CursedAnalytics"]