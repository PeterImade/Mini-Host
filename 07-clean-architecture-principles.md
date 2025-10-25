# 🧩 Clean Architecture Principles

## 🎯 Goal
Separate **concerns** and ensure **dependency direction** always points inward.

API → Application → Domain ← Infrastructure

## 💡 Rules
- Domain knows nothing about Application, Infrastructure, or API.
- Application depends on abstractions, not implementations.
- Infrastructure implements those abstractions.

---

## 🧱 Dependency Rule Diagram
```mermaid
graph LR
    API --> Application
    Application --> Domain
    Infrastructure --> Domain
    Infrastructure --> Application

    💡 My Takeaways

The Domain is the core — everything else serves it.

Each layer has a single purpose.

Keep dependency arrows pointing inward. Always.