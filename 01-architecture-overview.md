# 🏗️ Architecture Overview

This document describes the high-level design of the MiniHost system.

## 🎯 Goal
To build a **Mini Hosting Platform** (like Render, Vercel, or Heroku) using a **Modular Monolith** architecture in .NET, with clear boundaries between modules.

## 🧩 Core Principles
- Simplicity first, extensibility later
- Each module is self-contained
- Infrastructure and business logic are separated
- Code must express business intent clearly

## High-Level Layers

API → Application → Domain ← Infrastructure

| Layer | Responsibility |
|--------|----------------|
| **API** | Exposes HTTP endpoints, DTOs, and Mappings |
| **Application** | Orchestrates business use cases |
| **Domain** | Holds business truth and rules |
| **Infrastructure** | Handles technical details (DB, Docker, Nginx, Git) |

## Diagram
```mermaid
graph TD
    API --> Application
    Application --> Domain
    Application --> Infrastructure
    Infrastructure --> ExternalSystems[(Docker / Git / MSSQL)]
