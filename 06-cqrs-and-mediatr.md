# 🔀 CQRS and MediatR

## 🎯 Principle
Separate **reads (queries)** and **writes (commands)** for clarity and scalability.

## 🧩 Current MVP
- No MediatR used yet.
- Each handler acts as a mini CQRS component.

## 🧠 When to Add MediatR
When:
- Many commands/queries emerge.
- You need pipeline behaviors (validation, logging).
- You move toward microservices.

---

## 💡 My Takeaways
- CQRS ≠ complexity. It’s a *concept*, not a framework.
- Don’t add MediatR until there’s an actual need.