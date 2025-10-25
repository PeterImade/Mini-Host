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


## 💡 My Takeaways
- Persistence is technically part of Infrastructure but isolated for clarity.
- Each module owns its own database (modular autonomy).
- No direct EF logic in Handlers — only through Repositories.