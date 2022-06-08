FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
ARG TARGETPLATFORM
WORKDIR /src

COPY *.sln .
COPY LXGaming.CursedAnalytics/*.csproj ./LXGaming.CursedAnalytics/
RUN case "$TARGETPLATFORM" in \
        linux/amd64) RUNTIME=linux-musl-x64 ;; \
        linux/arm64) RUNTIME=linux-musl-arm64 ;; \
        *) echo "Unsupported Platform: $TARGETPLATFORM"; exit 1 ;; \
    esac && \
    dotnet restore --runtime $RUNTIME

COPY LXGaming.CursedAnalytics/. ./LXGaming.CursedAnalytics/
WORKDIR /src/LXGaming.CursedAnalytics
RUN case "$TARGETPLATFORM" in \
        linux/amd64) RUNTIME=linux-musl-x64 ;; \
        linux/arm64) RUNTIME=linux-musl-arm64 ;; \
        *) echo "Unsupported Platform: $TARGETPLATFORM"; exit 1 ;; \
    esac && \
    dotnet publish --configuration Release --no-restore --output /app --runtime $RUNTIME --self-contained true /p:PublishSingleFile=true /p:PublishTrimmed=true

FROM mcr.microsoft.com/dotnet/runtime-deps:6.0-alpine
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["./LXGaming.CursedAnalytics"]