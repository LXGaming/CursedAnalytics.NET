# CursedAnalytics.NET

[![License](https://img.shields.io/github/license/LXGaming/CursedAnalytics.NET?label=License&cacheSeconds=86400)](https://github.com/LXGaming/CursedAnalytics.NET/blob/main/LICENSE)
[![Docker Hub](https://img.shields.io/docker/v/lxgaming/cursedanalytics/latest?label=Docker%20Hub)](https://hub.docker.com/r/lxgaming/cursedanalytics)

**Cursed Analytics** is a cursed way of obtaining project analytic data from [CurseForge](https://www.curseforge.com/).

For [Reward Store](https://authors.curseforge.com/store) analytics check out [cursed-analytics](https://github.com/LXGaming/cursed-analytics)

## Prerequisites
- [Grafana](https://grafana.com/)
- [MariaDB](https://mariadb.org/) or [MySQL](https://www.mysql.com/)

## Usage
### docker-compose
```yaml
version: "3"
services:
  cursedanalytics:
    container_name: cursedanalytics
    environment:
      - TZ=Pacific/Auckland
    image: lxgaming/cursedanalytics:latest
    restart: unless-stopped
    volumes:
      - /path/to/cursedanalytics/logs:/app/logs
      - /path/to/cursedanalytics/config.json:/app/config.json
```

## License
CursedAnalytics.NET is licensed under the [Apache 2.0](https://github.com/LXGaming/CursedAnalytics.NET/blob/main/LICENSE) license.