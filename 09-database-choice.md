# Database design choice
**Date:** 2025-10-25   
**Author:** Peter

# ADR 001: Database Paradigm — Relational vs NoSQL


## Decision

I picked a relational database for the following reasons: 

- Strong consistency and data integrity. I don't want a situation whereby a deployment state is "Running" instead of "Failed". I want that exact correctness.
- Structured data. The data model has many relationships, for example: Deployment has many histories. Deployment has many logs, etc.
- High read workloads. The read-to-write ratio is higher. Reads includes deployment logs, deployment histories, and deployment statues. These will be frequently accessed, and speed can be optimized using indexes and caching.
- Low write workload. There will be fewer writes in the system. Like updates, deployments, and restarts. This will be infrequent because they don't happen often.
- Transaction guarantee. There will be a set of operations that needs to atomic for data correctness, for example, during deployment there would be for multiple operations, such as creating an app instance, writing to DeploymentHistory table, and saving logs, and this needs to be atomic i.e they need to work together as one unit, so that when a write fails there won't be any inconsistent data in the system.


### Tradeoffs
As you know, every design choice in a system has a tradeoff(s) lurking around the corner. And awareness of them is critical:

- Lower throughput under heavy-write load. When the system scales and has a high-write workload, the database might be inefficient in handling it.
- And scaling writes horizontally and ensuring strong consistency requires thorough and detailed planning.

## Alternatives considered
I considered using a NOSQL database, but it doesn't fit well in my use case due to the following reasons: 
- I needed strong consistency because using eventual consistency is not suitable for deployment updates, risking stale data.
- The data is structured and well-defined with relationships, and schema flexibility is not needed here.

### Future considerations
- Introduce more replicas to handle increased reads
- Add a caching layer for faster reads and low latency.
- Revisit a NOSQL or hybrid models if the system evolves towards heavy write workloads.

### Summary
For this MiniHost platform, choosing a database that ensures strong consistency, heavy-read workloads, and structured data is critical for system reliability and data consistency.