FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY *.sln .
COPY LXGaming.CursedAnalytics/*.csproj ./LXGaming.CursedAnalytics/
RUN dotnet restore

COPY LXGaming.CursedAnalytics/. ./LXGaming.CursedAnalytics/
WORKDIR /src/LXGaming.CursedAnalytics
RUN dotnet publish -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/runtime:6.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "LXGaming.CursedAnalytics.dll"]