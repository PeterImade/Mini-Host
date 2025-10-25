# 🐛 Bugs and Fixes

## Bug #1 — EF Core not tracking entities
**Date:** Oct 25, 2025  
**Module:** Deployments  
**Symptom:** Updated entities not saving changes.

**Root Cause:** I used `.AsNoTracking()` in a query and then updated the entity. EF wasn’t tracking it.

**Fix:** Removed `.AsNoTracking()` or attached entity manually.

**Lesson:** Use `.AsNoTracking()` only for read-only queries.