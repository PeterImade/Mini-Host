
---

### ?? `04-application-layer.md`

```markdown
# ?? Application Layer

The Application layer coordinates **use cases** (commands and queries).

## ?? Structure
/Commands
/Queries
/Interfaces
/Validators

## 🧩 Handlers
Each handler contains its own logic — no service layer for now.

```csharp
public class DeployAppHandler
{
    private readonly IDeploymentRepository _repository;
    private readonly IDockerManager _docker;

    public async Task<AppInstance> HandleAsync(DeployAppCommand command) { ... }
}

💡 My Takeaways

Business logic lives directly in handlers for simplicity.

The layer depends on interfaces, not implementations.

CQRS can be introduced later without structural changes.

The Deployments module uses a local saga pattern to ensure atomic-like consistency across multiple external systems—Git, Docker, Nginx, and the database—within the modular monolith. Instead of relying on distributed events, the DeployAppHandler acts as an in-memory orchestrator that performs each deployment step sequentially and defines compensating actions to roll back previous steps if a failure occurs. Database writes are wrapped in an EF Core transaction, while side effects like cloned repos or running containers are explicitly undone on errors. This design provides reliability, prevents partial or “zombie” deployments, and lays the foundation for an eventual transition to a distributed, event-driven saga when the system evolves into microservices.