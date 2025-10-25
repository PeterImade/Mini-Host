
---

### 🧱 `02-modular-monolith-structure.md`

```markdown
# 🧩 Modular Monolith Structure

MiniHost is a **modular monolith**, not a single monolithic codebase.

## 📁 Folder Layout
Each module is a self-contained class library:
src/
├── MiniHost.API/
├── MiniHost.Shared/
├── MiniHost.Modules.Deployments/
├── MiniHost.Modules.Apps/
└── MiniHost.Modules.Logs/


## 🧱 Module Internal Structure
Each module follows Clean Architecture internally:
/API
/Application
/Domain
/Infrastructure
/Persistence


## ⚙️ Module Registration
Modules implement the `IModule` interface, allowing automatic discovery and registration.

```csharp
public interface IModule
{
    void RegisterModule(IServiceCollection services, IConfiguration config);
}

Modules are loaded dynamically in Program.cs via:
builder.Services.AddModules(builder.Configuration);


My Takeaways

Each module behaves like a mini microservice.

No module directly depends on another — only through contracts.

Transitioning to microservices later will be frictionless.