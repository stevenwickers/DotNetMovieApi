# DotNetMovieApi

`DotNetMovieApi` is a local-development C# movie API built with ASP.NET Core Minimal APIs, Dapper, PostgreSQL, Swagger, and GraphQL.

The project exposes:

- REST endpoints for movies and genres
- A GraphQL endpoint for querying movies and patching movie data
- Swagger UI for exploring the REST API
- Request logging, correlation IDs, CORS, and centralized error handling

## Tech Stack

- .NET 10
- ASP.NET Core Minimal APIs
- Dapper
- PostgreSQL via `Npgsql`
- Hot Chocolate GraphQL
- Swashbuckle / Swagger

## Features

- Browse, create, update, patch, and delete movies
- Browse genres and fetch a genre by ID
- Filter and paginate movie results
- Sort movie results by supported columns
- Query movies through GraphQL
- Patch movie records through a GraphQL mutation-backed repository method

## Endpoints

### REST

- `GET /movies`
- `GET /movies/{id}`
- `POST /movies`
- `PUT /movies/{id}`
- `PATCH /movies/{id}`
- `DELETE /movies/{id}`
- `GET /genres`
- `GET /genres/{id}`
- `GET /health`
- `GET /` redirects to Swagger UI

Swagger UI is available at:

- `http://localhost:5000/swagger`

### GraphQL

GraphQL is enabled through Hot Chocolate and is mapped at:

- `POST /graphql`

Current GraphQL operations include:

- Query movies with optional filters
- Query a movie by ID
- Update a movie using a patch request

## Movie Query Support

The `GET /movies` endpoint supports:

- `search`
- `searchMode`
- `page`
- `pageSize`
- `sortBy`
- `sortDirection`
- `releaseDateFrom`
- `releaseDateTo`
- `worldwideGrossMin`
- `worldwideGrossMax`
- `productionBudgetMin`
- `productionBudgetMax`
- `domesticGrossMin`
- `domesticGrossMax`
- `genres`

Supported `searchMode` values:

- `general`
- `starts`
- `ends`
- `contains`

## Project Structure

```text
Configuration/   Application setup, Swagger, service registration, route mapping
Contracts/       Request, response, enum, and support models
Data/            Database connection and type handlers
Endpoints/       Minimal API endpoints
GraphQL/         GraphQL query and mutation types
Json/            Custom JSON converters
Middleware/      Correlation ID and request logging middleware
Repositories/    Data access logic and repository contracts
Services/        Request-scoped helpers
Validation/      Request validation helpers
```

## Running Locally

This project is intended as a local development proof of concept.

### Requirements

- .NET 10 SDK
- PostgreSQL running locally

### Configuration

The default configuration in `appsettings.json` includes:

- API URL: `http://localhost:5000`
- PostgreSQL connection string:
  `Host=localhost;Port=55432;Database=wickers_db;Username=user;Password=password`
- CORS origins for local frontends such as `http://localhost:5173` and `http://localhost:3000`

### Start the API

```bash
dotnet restore
dotnet build
dotnet run
```

## Notes

- The app uses a custom `DateOnly` Dapper type handler.
- Enum values are serialized as strings.
- PATCH handling supports partial updates.
- Unhandled exceptions are returned as RFC 7807 problem details with a correlation ID.
