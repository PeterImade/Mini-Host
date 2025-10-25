## Clean Architecture
- API → Application → Domain ← Infrastructure
- Domain must not know about frameworks — it's pure business logic.

## DDD Insights
- Entities = things that have identity.
- Value Objects = things defined by value (immutable).
- Domain exceptions enforce business rules in code.