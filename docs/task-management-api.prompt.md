# Task Management API Prompt

Create a RESTful API for a simple Task Management system using Clean Architecture.

Tech stack and style:
- Preferred language/framework: .NET 8 ASP.NET Core Web API
- Layers: Api, Application, Domain, Infrastructure
- Persistence: EF Core + SQLite
- Authentication: assume basic User model already exists; tasks belong to a user
- Keep endpoints RESTful, code concise, and production-ready for MVP

Required features:
- CRUD for tasks
- Task fields: title, description, status, due_date
- User relationship: each task is associated with one user

Expected API endpoints:
- POST /api/tasks
- GET /api/tasks
- GET /api/tasks/{id}
- PUT /api/tasks/{id}
- DELETE /api/tasks/{id}

Implementation requirements:
- Validate required fields and return proper HTTP status codes
- Only allow authenticated users to manage their own tasks
- Add DTOs, mappings, and use-case services in Application layer
- Add repositories/persistence in Infrastructure layer
- Add controller endpoints in Api layer
- Include minimal unit/integration tests for task CRUD