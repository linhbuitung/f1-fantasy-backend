# F1 Fantasy Backend

> A comprehensive Formula 1 fantasy league platform built with .NET 9.0, featuring real-time data synchronization, AI-powered predictions, and competitive gameplay.

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-15+-336791?logo=postgresql)](https://www.postgresql.org/)
[![License](https://img.shields.io/badge/License-Academic-blue)](LICENSE)

**Live Demo**: [https://woof1.racing](https://woof1.racing)  

---

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Tech Stack](#tech-stack)
- [Getting Started](#getting-started)
- [Deployment](#deployment)
- [Testing](#testing)
- [Contributing](#contributing)
- [Database Schema](#database-schema)
- [License](#license)
- [Acknowledgments](#acknowledgments)

---

## Overview

F1 Fantasy Backend is a modular monolith application that powers a fantasy sports platform for Formula 1 enthusiasts. Users create fantasy teams by selecting real F1 drivers and constructors, earning points based on actual race results and competing in private leagues.

### Key Components

- **F1Fantasy** - REST API with SignalR real-time notifications
- **F1FantasyWorker** - Background service for data synchronization and scoring
- **Shared Database** - PostgreSQL with Entity Framework Core
- **ML Integration** - FastAPI service on Google Cloud Run for race predictions

---

## Features

### For Users
- Create fantasy teams with budget constraints
- Real-time scoring based on F1 race results
- Private leagues with friends
- Live leaderboards and statistics
- AI-powered race predictions (consumes credits)
- Real-time notifications via SignalR
- User profiles with favorite drivers/constructors

### For Administrators
- Manage active seasons
- Update images for drivers/constructors/circuits
- League oversight and moderation
- User management and moderation

### Technical Features
- Cookie-based authentication with refresh tokens
- Real-time notification updates via SignalR hubs
- Automated data sync from jolpica-f1 API
- Dynamic pricing algorithm based on performance
- Complex statistical aggregations

---

## Tech Stack

### Backend
- **.NET 9.0** - Core framework
- **ASP.NET Core** - Web API & SignalR
- **Entity Framework Core 9.0** - ORM
- **Npgsql** - PostgreSQL provider

### Database
- **PostgreSQL 15+** - Primary database
- **EF Core Migrations** - Schema management

### Authentication & Security
- **ASP.NET Core Identity** - User management & cookie authentication
- **PBKDF2** - Password hashing (via Identity)
- **Role-Based Authorization** - Granular access control
- **API Key Authentication** - Worker service security

### External Services
- **Google Cloud Storage** - File storage
- **Gmail SMTP** - Email notifications
- **Google Cloud Run** - ML API hosting
- **Jolpica-F1 F1 API** - Race data source

### DevOps & Testing
- **xUnit** - Testing framework
- **Docker** - Containerization
- **GitHub Actions** - CI/CD pipeline
- **Heroku** - Production hosting

---

## Getting Started

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [PostgreSQL 15+](https://www.postgresql.org/download/)
- [Docker](https://www.docker.com/) (optional)
- [Git](https://git-scm.com/)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/f1-fantasy-backend.git
   cd f1-fantasy-backend
   ```

2. **Setup environment variables**

   Create `.env` files in both `F1Fantasy/` and `F1FantasyWorker/`:

   ```bash
   # F1Fantasy/.env
   POSTGRES_USER=for-local-developement
   POSTGRES_PASSWORD=for-local-developement
   POSTGRES_DB=for-local-developement
   POSTGRES_PORT=for-local-developement
   DB_PASSWORD=for-local-developement
   ASPNETCORE_ENVIRONMENT=Development
   DATABASE_URL=your-postgresql-url
   POSTGRES_HOST=your-postgresql-host
   AUTH_KEY=your-super-secret-jwt-key-at-least-32-characters-long
   MAIL_PASSWORD=your-gmail-smpt-password
   WORKER_API_KEY=your-secure-worker-api-key
   GOOGLE_CREDENTIAL_BASE64=<base64-encoded-service-account-json>
   ML_API_KEY=your-ml-api-key
   ```

   ```bash
   # F1FantasyWorker/.env
   DELAY_BETWEEN_REQUESTS=1000 (customize as needed)
   SYNC_REQUEST_LIMIT=100 (customize as needed)
   DATABASE_URL=your-postgresql-url
   POSTGRES_USER=for-local-developement
   POSTGRES_PASSWORD=for-local-developement
   POSTGRES_DB=for-local-developement
   POSTGRES_PORT=for-local-developement
   DB_PASSWORD=for-local-developement
   POSTGRES_HOST=your-postgresql-host
   AUTH_KEY=your-super-secret-jwt-key-at-least-32-characters-long
   MAIL_PASSWORD=your-gmail-smpt-password
   WORKER_API_KEY=your-secure-worker-api-key
   ```

3. **Create PostgreSQL database**
   ```bash
   createdb woof1
   ```

4. **Apply database migrations**
   ```bash
   cd F1Fantasy
   dotnet ef database update
   cd ..
   ```

5. **Run the API**
   ```bash
   cd F1Fantasy
   dotnet run
   ```

   API will be available at `https://localhost:7093`

6. **Run the Worker** (in separate terminal)
   ```bash
   cd F1FantasyWorker
   dotnet run
   ```

7. **Access Swagger UI**

   Open browser: `https://localhost:7093/swagger`

### Quick Start with Docker

```bash
# Build images
docker build -t f1fantasy-api -f F1Fantasy/Dockerfile .
docker build -t f1fantasy-worker -f F1FantasyWorker/Dockerfile .

# Run containers
docker run -d -p 8080:8080 --name api f1fantasy-api
docker run -d --name worker f1fantasy-worker
```

## Deployment

### Production Environment

**Hosting Infrastructure**:
- **API**: Heroku (Eco dyno)
- **Worker**: Heroku (Eco dyno)
- **Database**: Heroku Postgres (Essential-0)
- **ML Service**: Google Cloud Run
- **Static Assets**: Google Cloud Storage
- **Frontend**: Netlify

---

## Testing

### Run All Tests

```bash
cd Tests
dotnet test
```

### Run Specific Test Class

```bash
dotnet test --filter "FullyQualifiedName~PointCalculationTests"
```

**Test Examples**:
- [`PointCalculationTests.cs`](Tests/WorkerTests/PointCalculationTests.cs) - Fantasy scoring logic
- [`StatisticRepositoryTests.cs`](Tests/ApiTests/StatisticRepositoryTests.cs) - LINQ aggregations
- [`DataSyncRepositoryTests.cs`](Tests/WorkerTests/DataSyncRepositoryTests.cs) - Data synchronization

### Continuous Integration

GitHub Actions automatically runs tests on every PR:
```yaml
# .github/workflows/test.yml
on: [pull_request, push]
jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '9.0.x'
      - run: dotnet test
```

---

## Contributing

We welcome contributions! Please follow these guidelines:

### Development Workflow

1. **Fork the repository**
2. **Create feature branch**: `git checkout -b feature/AmazingFeature`
3. **Commit changes**: `git commit -m 'Add AmazingFeature'`
4. **Push to branch**: `git push origin feature/AmazingFeature`
5. **Open Pull Request**

### Pull Request Requirements

- All tests must pass
- Include tests for new features
- Follow existing code structure
- Update documentation if needed
- No hardcoded secrets

### Code Style

- Use C# naming conventions (PascalCase for public members)
- Follow existing module structure
- Keep methods focused and small
- Add XML comments for public APIs

---

## Database Schema

### Migrations

```bash
# Create new migration
dotnet ef migrations add MigrationName --project F1Fantasy

# Apply to database
dotnet ef database update --project F1Fantasy

# Revert migration
dotnet ef database update PreviousMigrationName --project F1Fantasy
```

---

## License

This project is developed as part of a Bachelor's thesis for educational purposes.

**Author**: Bui Tung Linh
**Student ID**: S28786
**University**: PJATK
**Thesis**: Polish-Japanese Academy of Information Technology

---

## Acknowledgments

- F1 data provided by [Jolpica-F1 Developer API](https://github.com/jolpica/jolpica-f1)
- Built with [ASP.NET Core](https://docs.microsoft.com/aspnet/core/)
- Hosted on [Heroku](https://www.heroku.com/) + [Netlify](https://www.netlify.com/)
