# University Course & Result Management System

[![ci](https://github.com/solaimanhosen/University-Course-and-Result-Management-System/actions/workflows/ci.yml/badge.svg?branch=migration/aspnet-core)](https://github.com/solaimanhosen/University-Course-and-Result-Management-System/actions/workflows/ci.yml)

A university's academic office manages departments, courses, teachers, course assignments,
student registration, classroom allocation, enrollment and results, and prints a student's
result as a PDF. The 14 user stories are in [docs/spec.pdf](docs/spec.pdf).

The original app (2017) was written in ASP.NET MVC 5. It is being rebuilt as an
**ASP.NET Core 10 Minimal API** with an **Angular 22** front end, deployed to AWS. The plan, with
every step and its progress, is in [docs/MIGRATION_PLAN.md](docs/MIGRATION_PLAN.md).

## Status

Work happens on the `migration/aspnet-core` branch. `main` keeps the 2017 app until the
migration is finished.

| Done | Next |
|---|---|
| Phase 0: repository, solution skeleton, Docker Compose | Phase 2: data model, migrations and test projects |
| Phase 1: API foundation (persistence, error handling, OpenAPI, OpenTelemetry, CI) | Phase 3: authentication and roles |

The API runs, but it has no feature endpoints yet.

## Stack

| Area | Choice |
|---|---|
| API | .NET 10, ASP.NET Core Minimal APIs, built-in validation, Problem Details |
| Data | EF Core 10 and SQL Server (a container locally, Amazon RDS in production) |
| Auth | ASP.NET Core Identity with a cookie; roles Admin, Staff and Teacher *(Phase 3)* |
| API docs | Built-in OpenAPI and the Scalar UI; the OpenAPI file is also written at build time |
| Observability | OpenTelemetry traces, metrics and logs, viewed in the Aspire Dashboard |
| Front end | Angular 22 and Angular Material *(Phase 5)* |
| Tests | xUnit v3, Testcontainers, Respawn, Shouldly, Vitest, Playwright *(Phase 2 onwards)* |
| CI/CD | GitHub Actions; deployment to AWS EC2 through OIDC and SSM *(Phase 10)* |

## Run it locally

You need the .NET 10 SDK, Docker with Compose, and (from Phase 5) Node 24.

```bash
# 1. Database and Aspire Dashboard
cp .env.example .env            # then set a strong MSSQL_SA_PASSWORD (rules are in the file)
docker compose up -d

# 2. API configuration, kept in user secrets (never in the repository)
cd src/University.Api
dotnet user-secrets set "ConnectionStrings:University" \
  'Server=localhost,1433;Database=University;User Id=sa;Password=<MSSQL_SA_PASSWORD>;TrustServerCertificate=True;Encrypt=True'
dotnet user-secrets set "Seed:AdminEmail" 'admin@university.local'
dotnet user-secrets set "Seed:AdminPassword" '<strong password>'

# 3. Start the API
dotnet watch
```

| URL | What |
|---|---|
| http://localhost:5080/scalar | API reference (Development only) |
| http://localhost:5080/openapi/v1.json | OpenAPI document (Development only) |
| http://localhost:5080/health/live | Liveness: the process is up |
| http://localhost:5080/health/ready | Readiness: the database is reachable (503 until the first migration exists) |
| http://localhost:18888 | Aspire Dashboard: traces, metrics and logs |

The app stops at startup if `ConnectionStrings:University` is missing.

## Repository layout

```
.github/workflows/ci.yml   build, OpenAPI check and tests on every push and pull request
docs/                      migration plan, spec, the 2017 SQL dumps
legacy/                    the 2017 ASP.NET MVC 5 app (removed when the migration ends)
openapi/                   University.Api.json, written by every build and committed
src/University.Api/        the ASP.NET Core 10 API
compose.yaml               SQL Server and the Aspire Dashboard for local development
Directory.Build.props      shared build settings (warnings are errors, analyzers on)
Directory.Packages.props   every NuGet version, in one place (Central Package Management)
```

## Contributing

Packages are added without a version (`dotnet add package <Name>`); the version lives in
`Directory.Packages.props`. Commit `openapi/University.Api.json` whenever the API changes, since CI
fails when the committed file is out of date.
