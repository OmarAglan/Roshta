# Repository Guidelines

## Project Structure & Module Organization
`Rosheta.sln` is organized by Clean Architecture layers:
- `Core/`: domain entities, DTOs, service logic, and interfaces (`Application/Contracts`).
- `Infrastructure/`: EF Core `ApplicationDbContext`, repository implementations, migrations, and storage adapters.
- `Presentation/`: ASP.NET Core Razor Pages app (`Program.cs`, `Pages/`, `wwwroot/`, middleware, filters).
- `tests/Rosheta.UnitTests/`: xUnit unit tests for Core services, mirroring Core paths.
- `docs/`: architecture notes, developer guide, and task tracking.

Keep dependencies flowing inward only: `Presentation -> Infrastructure -> Core`.

## Build, Test, and Development Commands
- `dotnet restore`: restore NuGet packages for all projects.
- `dotnet build Rosheta.sln`: compile the full solution.
- `dotnet run --project Presentation/Rosheta.Web.csproj`: run the web app locally.
- `dotnet test tests/Rosheta.UnitTests/Rosheta.UnitTests.csproj`: run unit tests.
- `dotnet ef migrations add <Name> -p Infrastructure/Rosheta.Infrastructure.csproj -s Presentation/Rosheta.Web.csproj`: create a migration.
- `dotnet ef database update -p Infrastructure/Rosheta.Infrastructure.csproj -s Presentation/Rosheta.Web.csproj`: apply migrations.

## Coding Style & Naming Conventions
Use C# defaults with 4-space indentation and nullable reference types enabled.
- `PascalCase`: classes, methods, properties, DTOs.
- `camelCase`: locals and parameters.
- Interfaces must be prefixed with `I` (for example, `IDoctorService`).
- Keep PageModels thin; business rules belong in `Core/Application/Services`.
- Inject abstractions, not concretes; register in `Core/DependencyInjection.cs` or `Infrastructure/DependencyInjection.cs`.

No dedicated lint config is committed; use IDE analyzers and run `dotnet format` before opening a PR.

## Testing Guidelines
Frameworks: xUnit + Moq + FluentAssertions (+ coverlet collector).
- Place tests under `tests/Rosheta.UnitTests/...` mirroring source folders.
- Name test files as `<ClassName>Tests.cs`.
- Name methods as `MethodName_ShouldExpectedBehavior_WhenCondition`.
- Add/adjust tests for every service behavior change and exception path.

## Commit & Pull Request Guidelines
Recent history uses imperative, scope-first subjects (for example, `Refactor DoctorRepository and DoctorService for generic CRUD`).
- Start commits with a verb: `Add`, `Refactor`, `Fix`, `Update`.
- Keep one logical change per commit.
- PRs should include: concise summary, linked issue/task, test results (`dotnet test`), and screenshots for UI changes.
- If schema changes are included, mention migration name and database impact.

## Security & Configuration Tips
- Do not commit secrets to `appsettings*.json`; prefer environment-specific overrides.
- SQLite data (`roshta.db`) is local runtime state; avoid manual edits in commits unless intentional.
