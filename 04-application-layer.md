
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