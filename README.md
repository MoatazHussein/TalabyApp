# Talaby API

Talaby is a .NET 8 backend API for a service marketplace where clients can publish project requests, stores can submit proposals, users can discuss project details, and commissions can be collected through Tap Payments.

The solution follows a layered architecture with separate API, Application, Domain, and Infrastructure projects. It uses ASP.NET Core, Entity Framework Core, ASP.NET Core Identity, MediatR, FluentValidation, AutoMapper, Serilog, SQL Server, and Tap payment integrations.

## Features

- JWT-based authentication and role authorization.
- Client, store, and admin user roles.
- Client and store registration flows with email confirmation.
- Project request lifecycle management.
- Store proposal submission and proposal status updates.
- Project questions, question replies, proposal replies, and discussion flows.
- Dashboard endpoints for admin, client, and store users.
- Image and file uploads served from local storage.
- Store category management.
- User policy violation review workflow.
- Tap checkout, payment verification, and webhook processing for project commission payments.
- Automatic database migration and seed data on application startup.
- Swagger/OpenAPI documentation in development and production.
- Structured request logging with Serilog.

## Solution Structure

```text
Talaby.sln
src/
  Talaby.API/              ASP.NET Core Web API, controllers, middleware, Swagger, auth setup
  Talaby.Application/      CQRS commands/queries, validators, DTOs, application services
  Talaby.Domain/           Entities, enums, constants, repository contracts, domain exceptions
  Talaby.Infrastructure/   EF Core DbContext, migrations, repositories, Identity, payments, storage, email
```

## Tech Stack

- .NET 8
- ASP.NET Core Web API
- ASP.NET Core Identity
- Entity Framework Core with SQL Server
- MediatR
- FluentValidation
- AutoMapper
- Serilog
- Swashbuckle / Swagger
- Tap Payments API

## Prerequisites

- .NET SDK 8.0 or later
- SQL Server or SQL Server Express
- A configured SMTP account for email flows
- Tap payment credentials when testing payment features

## Configuration

Configuration is read from `src/Talaby.API/appsettings.json`, `src/Talaby.API/appsettings.Development.json`, environment variables, or user secrets.

For local development, prefer user secrets or environment variables for sensitive values:

```powershell
cd src/Talaby.API
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:TalabyDb" "Server=.\\SQLExpress;Database=TalabyDb;Trusted_Connection=True;TrustServerCertificate=True"
dotnet user-secrets set "Jwt:Key" "replace-with-a-long-secure-secret"
dotnet user-secrets set "Jwt:Issuer" "https://localhost:7072"
dotnet user-secrets set "Jwt:Audience" "https://localhost:5173"
dotnet user-secrets set "Smtp:Username" "your-email@example.com"
dotnet user-secrets set "Smtp:Password" "your-smtp-app-password"
dotnet user-secrets set "Tap:SecretKey" "your-tap-secret-key"
```

Important configuration keys:

| Section | Key | Purpose |
| --- | --- | --- |
| `ConnectionStrings` | `TalabyDb` | SQL Server connection string used by EF Core. |
| `Jwt` | `Key`, `Issuer`, `Audience` | JWT signing and validation settings. |
| `Smtp` | `Host`, `Port`, `Username`, `Password` | Email delivery settings. |
| `App` | `ConfirmEmailApiUrl`, `ResetPasswordUrl`, `FrontendBaseUrl`, `DefaultTimeZone` | Application URLs and default timezone. |
| `Tap` | `BaseUrl`, `SecretKey`, `MerchantId`, `SourceId`, `FrontendBaseUrl`, `ApiPublicBaseUrl`, `CommissionPercentage` | Tap API and checkout settings. |
| `Serilog` | `WriteTo`, `MinimumLevel` | Console and file logging configuration. |

Do not commit production secrets, SMTP passwords, payment keys, or live database credentials.

## Getting Started

Restore packages:

```powershell
dotnet restore Talaby.sln
```

Build the solution:

```powershell
dotnet build Talaby.sln
```

Run the API:

```powershell
dotnet run --project src/Talaby.API
```

Default local URLs from `launchSettings.json`:

- HTTP: `http://localhost:5241`
- HTTPS: `https://localhost:7072`
- Swagger: `https://localhost:7072/swagger`

## Database

The application runs pending EF Core migrations automatically during startup through the `TalabySeeder`. It also seeds initial store categories and the default roles:

- `Admin`
- `Client`
- `Store`

To apply migrations manually, restore the local EF tool manifest and run the database update from the API project:

```powershell
cd src/Talaby.API
dotnet tool restore
dotnet ef database update --project ..\Talaby.Infrastructure --startup-project .
```

To add a migration:

```powershell
cd src/Talaby.API
dotnet ef migrations add MigrationName --project ..\Talaby.Infrastructure --startup-project . --output-dir Migrations
```

## API Overview

The API exposes endpoints under the following areas:

| Area | Base Route | Description |
| --- | --- | --- |
| Identity | `/api/identity` | Registration, login, current user, email confirmation, password reset, user updates. |
| Store Categories | `/api/storeCategories` | Store category CRUD operations. |
| Project Requests | `/api/project-requests` | Project request creation, listing, status changes, completion, proposals, questions, commission payments. |
| Project Proposals | `/api/project-proposals` | Store proposal creation, listing, status updates, replies. |
| Project Questions | `/api/project-questions` | Project question creation, updates, deletion, replies. |
| Question Replies | `/api/question-replies` | Replies to project questions. |
| Proposal Replies | `/api/proposal-replies` | Replies to project proposals. |
| Uploads | `/api/uploads` | Image and file upload endpoints. |
| Payments | `/api/payments` | Tap webhook handling. |
| Dashboard | `/api/dashboard` | Role-specific dashboard summaries. |
| Admin Policy Violations | `/api/admin/policy-violations` | Admin policy violation review endpoints. |

Use Swagger for full request and response schemas.

## Authentication and Roles

Most project, dashboard, and admin endpoints require a bearer token:

```http
Authorization: Bearer <access-token>
```

Role-protected endpoints use the seeded roles:

- `Admin` for administration and policy violation review.
- `Client` for client project request and commission payment flows.
- `Store` for store proposal and store dashboard flows.

## File Storage

Uploaded files are stored under `src/Talaby.API/Storage` at runtime and served from:

```text
/Storage
```

The startup task ensures required storage folders exist before the application handles requests.

## Payments

Commission payment checkout and verification are implemented with Tap Payments. The API supports:

- Creating checkout charges.
- Verifying payment state from Tap.
- Processing Tap webhook notifications.
- Reconciling commission payment attempts and final statuses.

For local webhook testing, expose the API with a tunneling tool and set `Tap:ApiPublicBaseUrl` to the public HTTPS URL.

## Logging

Serilog writes logs to:

```text
src/Talaby.API/Logs/
```

Console logging is also enabled. Adjust minimum levels in the `Serilog` section of the API configuration.

## Development Notes

- Keep business rules in `Talaby.Application` or `Talaby.Domain`.
- Keep persistence, external services, payments, storage, and email implementations in `Talaby.Infrastructure`.
- Add new API endpoints through controllers in `Talaby.API`.
- Add request validation with FluentValidation validators.
- Use MediatR commands and queries for application workflows.
- Prefer repository interfaces from the Domain/Application layer and implementations in Infrastructure.

## Useful Commands

```powershell
# Restore dependencies
dotnet restore Talaby.sln

# Build
dotnet build Talaby.sln

# Run API
dotnet run --project src/Talaby.API

# Restore EF Core local tool
cd src/Talaby.API
dotnet tool restore

# Apply migrations
dotnet ef database update --project ..\Talaby.Infrastructure --startup-project .
```

## Security Notes

- Rotate any credential that has ever been committed to source control.
- Store local secrets with user secrets or environment variables.
- Use different JWT keys, SMTP credentials, database credentials, and Tap keys per environment.
- Restrict CORS origins to trusted frontend domains.
- Use HTTPS for all deployed environments.

