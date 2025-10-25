```markdown
# 📘 Key Architectural Decisions

| Decision | Why |
|-----------|-----|
| Use Modular Monolith | Simpler to start, easy to evolve to microservices |
| Use MSSQL + EF Core | Balance between speed and reliability |
| No CQRS/MediatR yet | Focus on simplicity during MVP |
| Handlers contain business logic | Fewer abstractions, faster iteration |
| Persistence separated per module | Each module owns its data |
| Application services only for orchestration | Keeps business logic centralized |
| Repositories stay thin | Encapsulate EF Core only |
| Use IModule for discovery | Enables modular registration at runtime |

---

## 💡 My Takeaways
Architecture is **decisions made consciously**, not folders created blindly.