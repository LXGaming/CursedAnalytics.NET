# syntax=docker/dockerfile:1
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
ARG TARGETARCH
WORKDIR /src

COPY --link *.sln ./
COPY --link LXGaming.CursedAnalytics/*.csproj LXGaming.CursedAnalytics/
RUN dotnet restore LXGaming.CursedAnalytics --arch $TARGETARCH

COPY --link LXGaming.CursedAnalytics/ LXGaming.CursedAnalytics/
RUN dotnet publish LXGaming.CursedAnalytics --arch $TARGETARCH --configuration Release --no-restore --output /app

FROM mcr.microsoft.com/dotnet/runtime-deps:8.0-alpine
RUN apk add --no-cache --upgrade tzdata
WORKDIR /app
COPY --from=build --link /app ./
USER $APP_UID
ENTRYPOINT ["./LXGaming.CursedAnalytics"]