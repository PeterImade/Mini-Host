
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

yaml
Copy code
