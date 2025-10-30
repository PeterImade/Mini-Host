## 🏗️ Infrastructure
Handles external systems:
- Docker integration
- Git operations
- Nginx configuration
- Email notifications

## 💽 Persistence
Handles data storage:
- MSSQL (via EF Core)
- Repositories
- Mappings
- Migrations

## ⚖️ Difference
| Aspect | Infrastructure | Persistence |
|--------|----------------|-------------|
| Focus | External systems | Data storage |
| Example | DockerManager | DeploymentRepository |
| Layer | Outermost | Part of Infrastructure layer |


### Why I separated Persistence from Infrastructure?
I separated the Persistence layer from the Infrastructure layer to keep database concerns independent from other external integrations.
The Persistence layer focuses only on data storage and retrieval (e.g., EF Core, repositories, DbContext).
The Infrastructure layer handles broader external systems such as email services, Docker, file storage, caching, etc.
Separating them ensures that persistence logic doesn’t get mixed with unrelated external dependencies and keeps the architecture modular and easier to test.


## 💡 Architectural insight:
Persistence is one kind of infrastructure, but it’s so central to business logic that we isolate it. That isolation gives you freedom to change databases or switch ORMs without touching your business or infrastructure code.

## 💡 My Takeaways
- Persistence is technically part of Infrastructure but isolated for clarity.
- Each module owns its own database (modular autonomy).
- No direct EF logic in Handlers — only through Repositories.