
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
Modules and API depends on the shared folder, meaning the Shared library is the glue that connects the API and Modules together.
Each module registers its own dependencies(repositories, DbContexts, services, etc.) by implementing a shared contract called IModule.
A Module discovery system that scans the assemblies that implements the IModule interface and automatically registers them in the API program.cs



```csharp
public interface IModule
{
    void RegisterModule(IServiceCollection services, IConfiguration config);
}

Modules are loaded dynamically in Program.cs via:
builder.Services.AddModules(builder.Configuration);


The flow at startup:

1️. Program.cs starts the web app
2️. builder.Services.AddModules() runs
3️. AddModules() scans all assemblies for IModule
4️. Each module (Deployments, Logs, etc.) self-registers
5️. API starts with all modules already loaded


What makes this system modular and decoupled is:

- Each module depends on the Shared Library and implements the IModule interface
- The modules don't talk directly to each other
- The API doesn't talk to the modules, but through the shared Library. It registers the modules through the shared library by automatically scanning all modules and registering them
- Each module contains its own dependencies, making it easier to convert into a microservice.
- Each module can evolve, adapt, and be tested independently.

My Takeaways
  
Modules should loosely coupled, independent and testable
Adding another module won't break any exisiting implementation.
No module directly depends on another — only through contracts.
Transitioning to microservices later will be frictionless.