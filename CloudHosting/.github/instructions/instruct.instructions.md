# Copilot Coding Agent Instructions for WebApplication3

## Project Overview
- This is an ASP.NET Core web application following the Clean Architecture pattern: Core, Infrastructure, Presentation.
- All Docker interactions are handled via the Docker.DotNet library (C# API). No shell scripts or external bash commands are used.
- The main goal is to allow users to build and run their own web projects (with Dockerfile) inside containers, and manage those containers (start/stop, view logs, resource info).

## Key Architectural Patterns
- **Core-Infrastructure-Presentation**: Keep business logic (entities, rules) in Core, external integrations (Docker, database) in Infrastructure, and web/API endpoints in Presentation.
- **Single-file/class preference**: Avoid splitting logic into multiple files unless necessary for clarity. Prefer comprehensive, clean classes.
- **Entity Relationships**: The `User` entity has one-to-many relations to uploaded files and "cloud plans" (resource allocation). Use simple placeholder entities if external dependencies are not present.
- **Cloud Plans**: Only show non-expired plans when presenting options to users.

## Developer Workflows
- **Build**: Use standard .NET build commands (`dotnet build WebApplication3.sln`).
- **Run/Debug**: Use `dotnet run` or Visual Studio launch profiles. Debugging is done via IDE or terminal.
- **Docker Operations**: All Docker image/container management is performed through C# code using Docker.DotNet. No manual CLI commands.
- **Resource Info**: Expose RAM, CPU, storage, latency, and container load status via C# APIs if possible.
- **Logs**: Communicate container logs and debug info to the user via terminal/cmd output.

## Conventions & Patterns
- **Interoperability**: Always check .cs files for redundant code and interoperability issues when making changes.
- **Consistent Design**: Maintain the Clean Architecture pattern throughout. Do not introduce ad-hoc structures.
- **External References**: See Docker.DotNet (https://github.com/dotnet/Docker.DotNet) for Docker API usage. See ASP.NET Core docs for framework conventions.

<!-- ignore this section ## Key Files & Directories
- `WebApplication3/Program.cs`: Application entry point.
- `WebApplication3/appsettings*.json`: Configuration files.
- `WebApplication3/WebApplication3.csproj`: Project file.
- `WebApplication3/Properties/launchSettings.json`: Debug/run profiles. end of ignore -->

## Example Patterns
- Docker management code should be encapsulated in Infrastructure layer services, e.g., `DockerService`.
- Presentation layer (Controllers) should only call into Core/Infrastructure via interfaces.
- When fetching user cloud plans, filter out expired plans before presenting.

## Additional Notes
- Optimize for clean, readable code. Avoid unnecessary complexity.
- Reference the provided markdown rules in `.continue/rules/ASP_Core.md` for further guidance.

---
If any section is unclear or missing, please provide feedback for further refinement.
