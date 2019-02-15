---
code_name: TM-P_WCF-PROJECT_GAM_MON_CSH
create_date: 2015-06
obsolete_date: 2016-11
author: TM
author_site: timsmanter.net
editor: Visual Studio Community 2015+
language: C#
framework: Mono | MonoGame
locales: pl_PL
documentation:
    - Polish: /docs/WCFProject_Documentation_PL.docx
license: GPLv3
status: [Dev, Sample]
---

# WCFProject - Multiplayer Mono**Game**

## Overview

WCFProject is my provate lab project of creating full-featured game based on .NET WCF communication, MonoGame framework and physics engine. The result is server-client game application with ability to serve multiple client instances.

## Documentation

Full documentation in Polish language is available [here](docs/WCFProject_Documentation_PL.docx) in DOCX format.

## Frameworks and libraries

- Microsoft .NET Framework 4.5;
- Windows Presentation Foundation (WPF);
- Windows Communication Foundation (WCF);
- MonoGame;
- Farseer Physics Engine (Box2D);
- Json.NET;
- TiledSharp;
- MahApps.Metro;

## Screenshots

### Client

Client 1 | Client 2
:---: | :---:
![Client Launcher 1](docs/screenshots/client_launcher.png) | ![Client Launcher 2](docs/screenshots/client_launcher2.png)
![Client Window 1](docs/screenshots/client_window.png) | ![Client Window 2](docs/screenshots/client_window2.png)
![Client Window Debug 1](docs/screenshots/client_window_debug.png) | ![Client Window Debug 2](docs/screenshots/client_window_debug2.png)

![Client Window Debug Side](docs/screenshots/client_window_debug_side.png)

### Server

![Console Window](docs/screenshots/console_window2.png)

## Class Diagram

### WCFReference

![Class Diagram](docs/diagrams/WCFReference_Diagram.png)

### WCFClient

![Class Diagram](docs/diagrams/WCFClient_Diagram.png)

### WCFReference

![Class Diagram](docs/diagrams/WCFServer_Diagram.png)
