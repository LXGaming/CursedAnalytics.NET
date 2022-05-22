# CursedAnalytics.NET

[![License](https://lxgaming.github.io/badges/License-Apache%202.0-blue.svg)](https://www.apache.org/licenses/LICENSE-2.0)
[![Docker Pulls](https://img.shields.io/docker/pulls/lxgaming/cursedanalytics)](https://hub.docker.com/r/lxgaming/cursedanalytics)

**Cursed Analytics** is a cursed way of obtaining project analytic data from [CurseForge](https://www.curseforge.com/).

For [Reward Store](https://authors.curseforge.com/store) analytics check out [cursed-analytics](https://github.com/LXGaming/cursed-analytics)

## Prerequisites
- MySQL Database
- Grafana Instance

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
CursedAnalytics.NET is licensed under the [Apache 2.0](https://www.apache.org/licenses/LICENSE-2.0) license.