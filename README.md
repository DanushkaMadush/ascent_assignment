# Employee Management System


* **Backend** — ASP.NET Core Web API (.NET 8)
* **Frontend** — React + TypeScript + Vite
* **Mobile** — Flutter

## Project Structure

```text
ascent_assignment/
├── backend/
├── frontend/
├── mobile/
├── .gitignore
└── README.md
```

## Technology Stack

### Backend

* ASP.NET Core / .NET 8
* Entity Framework Core 8
* Microsoft SQL Server
* ASP.NET Core Identity
* JWT Bearer Authentication
* Swagger

### Frontend

* React 19
* TypeScript
* Vite
* React Router
* Axios
* Material UI (MUI)

### Mobile

* Flutter
* Dart 3.13+
* Secure Storage
* JWT Decoder
* Material Design

---

## Prerequisites

Before running the projects, make sure the required development environments are installed.

### Backend

* .NET 8 SDK
* Microsoft SQL Server

### Frontend

* Node.js
* npm

### Mobile

* Flutter SDK
* Dart SDK
* An available Flutter device/emulator

---

# Running the Projects

The three applications are located in separate directories and can be run independently.

## 1. Backend

Navigate to the backend directory:

```bash
cd backend
```

Restore the .NET dependencies:

```bash
dotnet restore
```

Create an `EMS_DB` database in SQL Server and configure the connection string in `appsettings.json`.

Then apply the Entity Framework Core migrations:

```bash
dotnet ef database update
```

Run the application:

```bash
dotnet run
```

### Seed Data

When the backend starts, the application seeds the database with initial users, roles, and departments.

**Admin user**
- `ADMIN@EMS.COM` — Admin role

**Departments**
- IT
- HR
- Administration

**Department Managers**
- `HR.MANAGER@EMS.COM`
- `IT.MANAGER@EMS.COM`
- `ADMIN.MANAGER@EMS.COM`

**Department Employees**
- `IT.EMPLOYEE1@EMS.COM`
- `IT.EMPLOYEE2@EMS.COM`
- `HR.EMPLOYEE3@EMS.COM`
- `HR.EMPLOYEE1@EMS.COM`
- `HR.EMPLOYEE2@EMS.COM`
- `ADM.EMPLOYEE3@EMS.COM`
- `ADM.EMPLOYEE1@EMS.COM`
- `ADM.EMPLOYEE2@EMS.COM`


### Swagger

The project includes **Swashbuckle.AspNetCore**, so Swagger/OpenAPI support is configured in the application.

The exact Swagger URL will be documented once the application's launch configuration is confirmed.

---

## 2. Frontend

Open a new terminal and navigate to the frontend directory:

```bash
cd frontend
```

Install the dependencies:

```bash
npm install
```

Copy `.env.example` to `.env` and update the API URL:

```env
VITE_API_BASE_URL=https://your-backend-url/api/v1
```

Start the Vite development server:

```bash
npm run dev
```

Starts the development server.

```bash
npm run build
```

Builds the TypeScript project and creates a production build with Vite.


> **Frontend environment variables/API URL configuration will be documented once confirmed.**

---

## 3. Mobile

Open another terminal and navigate to the mobile directory:

```bash
cd mobile
```

Install Flutter dependencies:

```bash
flutter pub get
```

Run the application:

```bash
flutter run
```

### Flutter Dependencies

The mobile application currently uses:

* `http` — HTTP communication with the backend API
* `flutter_secure_storage` — secure local storage
* `jwt_decoder` — JWT token decoding
* `cupertino_icons` — Cupertino-style icons


---

# Application Architecture

The repository is organized into three separate applications:

```text
                    ┌─────────────────┐
                    │     Backend     │
                    │   ASP.NET Core  │
                    │     .NET 8       │
                    └────────┬────────┘
                             │
                  ┌──────────┴──────────┐
                  │                     │
                  ▼                     ▼
          ┌───────────────┐     ┌───────────────┐
          │    React      │     │    Flutter    │
          │   Frontend    │     │    Mobile     │
          └───────────────┘     └───────────────┘
```

The backend is implemented using ASP.NET Core and Entity Framework Core, with JWT Bearer Authentication and ASP.NET Core Identity available for authentication and user management.

The React application communicates with the backend through HTTP requests using Axios.

The Flutter application communicates with the backend using the `http` package and includes secure storage and JWT decoding support.

---

# Completed Features

This section will be expanded as the implemented features are documented.

### Backend

* [ ] Features to be documented
* [ ] API endpoints to be documented
* [ ] Authentication flow to be documented
* [ ] Database/migrations to be documented

### Frontend

* [ ] Pages/features to be documented
* [ ] Authentication flow to be documented
* [ ] API integration to be documented

### Mobile

* [ ] Screens/features to be documented
* [ ] Authentication flow to be documented
* [ ] API integration to be documented

---

# Configuration

Configuration details will be added for:

* Backend database connection
* JWT configuration
* Frontend API base URL
* Mobile API base URL
* Development environment requirements

**No secrets or credentials should be committed to the repository.**

---

# Notes

This project was developed as an interview assignment using a shared ASP.NET Core backend with separate web and mobile clients.

Additional implementation details, completed functionality, setup requirements, and known limitations will be documented as the project is reviewed.
