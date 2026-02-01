
# AeroTrack Fullstack (Angular + ASP.NET Core + SQL Server)

- Frontend: Angular 19 (standalone components, single `app.routes.ts`)
- Backend: ASP.NET Core 8 Web API (Minimal APIs) + EF Core SQL Server
- Database: SQL Server (Local named instance)

## Configure DB Connection

In `backend/appsettings.json` we use the local named instance and DB name:

```
"ConnectionStrings": {
  "Default": "Server=LTIN656977\SQLEXPRESS;Database=AeroTrack;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

## Create DB (migrations)
```
cd backend
# Ensure dotnet-ef is installed: dotnet tool install --global dotnet-ef
# Add your first migration
# dotnet ef migrations add InitialCreate
# Apply migrations
# dotnet ef database update
```

> You can generate and apply migrations from this folder. The project already compiles without migrations; create schema when you're ready.

## Run Backend
```
cd backend
dotnet run
```
Swagger UI will be at `http://localhost:5000/swagger` by default.

## Run Frontend
```
npm install --legacy-peer-deps
npm start
```
Angular app: `http://localhost:4200`

## Notes
- All module routes are consolidated in `src/app/app.routes.ts`.
- Angular data service calls the backend API (no mock data). Completing a maintenance task inserts a service event and updates last service date for the aircraft.
