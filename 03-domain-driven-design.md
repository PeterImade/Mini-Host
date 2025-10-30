
---

### 🧱 `03-domain-driven-design.md`

```markdown
# 🧠 Domain-Driven Design (DDD)

MiniHost applies **Tactical DDD** principles.

## 🎯 Purpose
To keep the **business domain** pure, independent, and expressive.

## 🧩 Domain Components
- **Entities/** → Core business objects (`AppInstance`)
- **ValueObjects/** → Immutable validated concepts (`RepoUrl`, `Port`)
- **Enums/** → Business states (`DeploymentStatus`)
- **Events/** → Business happenings (`AppDeployedEvent`)
- **Exceptions/** → Domain rule violations

## 📘 Example: AppInstance
```csharp
public class AppInstance
{
    public RepoUrl RepoUrl { get; private set; }
    public int Port { get; private set; }

    public void MarkAsRunning(string containerId) { ... }
}



💬 DDD Level
This project implements Tactical DDD, not full Strategic DDD (no bounded context mapping yet).

💡 My Takeaways
Domain is framework-agnostic.

The domain language should be business language, not technical jargon.

The system should enforce correctness by design, not by validation scripts.

Why is RepoUrl a ValueObject and not an Entity?
It's a value object because to define the business rule of that property and also validate the data that comes into it. For integrity and correctness. So, I don't have to be validating it in higher layers, causing code duplication. 

Why does your domain model not need EF Core attributes? Because the domain model should be independent of any dependencies; it shouldn't contain EFcore attributes or dependencies. The persistence rules are defined and enforced using EFcore configuration, to keep them decoupled and testable. So, it will be easier to switch to other databases like Postgres and MongoDB in the future.

What happens if you made all entity properties public set instead of private? I made it that way so that the properties will be manipulated, modified, changed from the outside, risking wrong data from getting into the system. It is to enforce strictness and security.

I added domain behaviours to the AppInstance entity to control the internal state(properties and fields) through methods(e.g MarkAsFailed(), etc)

Why validate entities in the Domain layer?
The reason is to ensure that the domain is the source of business truth and also to prevent data from entering into memory, not just the database. And by enforcing invariants inside the domain model, I guarantee that every entity in the system always respects business rules regardless of where it’s created (API, test, or event handler). This makes the domain self-protecting and consistent.


💡 Architectural insight:
You validate in the domain not to protect the database, but to protect the integrity of the business model itself.
If an invalid entity can’t exist in memory, it can never reach the database. That’s domain-driven design.