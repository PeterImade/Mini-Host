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


### Why add DTOs, validation, and mapping to the API layer?
I added DTOs validation and mapping to the API layer because it is the responsibility of the API layer to map external DTOs and internal domain models, validate external requests before reaching the application layer, and also define how data is being received from or returned to the clients. This separation prevents API-specific details (like JSON shape or HTTP request models) from leaking into business logic.

💡 Architect insight:
API = your translation layer.
It’s where “outside” (HTTP world) meets “inside” (domain world).
You translate, validate, and normalize everything at the boundary, so your core stays clean.