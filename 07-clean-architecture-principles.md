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

- Infrastructure layer depend on the Domain layer.
- Dependencies must all point inward.
- Following the Dependency Inversion Principle that states high level modules, in the case Application Layer, Domain Layer, should not depend directly on low level modules(Infrastructure); both should depend on abstractions.

- The Infrastructure layer is what speaks to the outside world i.e communicates with external services.