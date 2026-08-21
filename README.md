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

## Completed Features

### Backend

The backend provides RESTful APIs for authentication, employee management, departments, attendance, and leave management.

#### Authentication

| Method | Endpoint          | Description                                          |
| ------ | ----------------- | ---------------------------------------------------- |
| `POST` | `/api/v1/login`   | Authenticate a user and obtain access/refresh tokens |
| `POST` | `/api/v1/refresh` | Refresh an expired access token                      |

#### Attendance

| Method | Endpoint                       | Description                         |
| ------ | ------------------------------ | ----------------------------------- |
| `POST` | `/api/v1/attendance/check-in`  | Record employee check-in            |
| `POST` | `/api/v1/attendance/check-out` | Record employee check-out           |
| `GET`  | `/api/v1/attendance`           | Retrieve attendance records         |
| `GET`  | `/api/v1/attendance/{id}`      | Retrieve an attendance record by ID |
| `PUT`  | `/api/v1/attendance/{id}`      | Update an attendance record         |

#### Department Management

| Method | Endpoint                  | Description                 |
| ------ | ------------------------- | --------------------------- |
| `POST` | `/api/v1/department`      | Create a department         |
| `GET`  | `/api/v1/department`      | Retrieve departments        |
| `GET`  | `/api/v1/department/{id}` | Retrieve a department by ID |
| `PUT`  | `/api/v1/department/{id}` | Update a department         |

#### Employee Management

| Method   | Endpoint                | Description                |
| -------- | ----------------------- | -------------------------- |
| `POST`   | `/api/v1/employee`      | Create an employee         |
| `GET`    | `/api/v1/employee`      | Retrieve employees         |
| `GET`    | `/api/v1/employee/{id}` | Retrieve an employee by ID |
| `PUT`    | `/api/v1/employee/{id}` | Update an employee         |
| `DELETE` | `/api/v1/employee/{id}` | Delete an employee         |

#### Leave Management

| Method  | Endpoint                     | Description                   |
| ------- | ---------------------------- | ----------------------------- |
| `POST`  | `/api/v1/leave`              | Apply for leave               |
| `GET`   | `/api/v1/leave`              | Retrieve leave records        |
| `GET`   | `/api/v1/leave/{id}`         | Retrieve a leave record by ID |
| `PUT`   | `/api/v1/leave/{id}`         | Update a leave request        |
| `PATCH` | `/api/v1/leave/{id}/cancel`  | Cancel a leave request        |
| `PATCH` | `/api/v1/leave/{id}/approve` | Approve a leave request       |
| `PATCH` | `/api/v1/leave/{id}/reject`  | Reject a leave request        |

### API Response Format

The API uses a common response structure for consistent communication between the backend and client applications.

A typical paginated response follows this structure:

```json
{
  "success": true,
  "message": "string",
  "data": {
    "data": [],
    "pagination": {
      "pageNumber": 0,
      "pageSize": 0,
      "totalRecords": 0,
      "totalPages": 0,
      "hasPrevious": true,
      "hasNext": true
    }
  }
}
```

The response structure provides:

* `success` — Indicates whether the request was successful.
* `message` — Provides a response or error message.
* `data` — Contains the response payload.
* `pagination` — Provides pagination metadata for list endpoints.

  * `pageNumber` — Current page number.
  * `pageSize` — Number of records per page.
  * `totalRecords` — Total number of available records.
  * `totalPages` — Total number of pages.
  * `hasPrevious` — Indicates whether a previous page exists.
  * `hasNext` — Indicates whether a next page exists.



### Frontend

The React frontend provides authentication, role-based route access, employee and department management, and protected navigation.

| Route           | Status          | Description                                                                                      | Accessible Roles         |
| --------------- | --------------- | ------------------------------------------------------------------------------------------------ | ------------------------ |
| `/login`        | Completed       | User login using email and password                                                              | Public                   |
| `/home`         | Completed       | Home page with navigation to available features based on the user's role                         | Authenticated users      |
| `/employees`    | Completed       | View and manage employees                                                                        | Admin, Manager           |
| `/departments`  | Completed       | View and manage departments                                                                      | Admin                    |
| `/attendance`   | Not Implemented | Attendance management                                                                            | Employee, Manager, Admin |
| `/leaves`       | Not Implemented | Leave management                                                                                 | Employee, Manager, Admin |
| `/unauthorized` | Completed       | Displays when an authenticated user attempts to access a route they are not authorized to access | Authenticated users      |
| `*`             | Completed       | Unknown routes are redirected to the Not Found page                                              | All users                |

### Frontend Access Control

The frontend implements role-based route access to restrict features based on the authenticated user's role.

* **Admin** — Access to employee and department management.
* **Manager** — Access to employee management.
* **Employee** — Access to employee-specific features.
* Unauthorized access attempts are redirected to the `/unauthorized` page.
* Unrecognized routes are redirected to a **Not Found** page.


### Mobile

The Flutter mobile application provides user authentication and attendance actions, with navigation to leave and attendance features.

| Screen            | Status          | Description                                                                |
| ----------------- | --------------- | -------------------------------------------------------------------------- |
| Login             | Completed       | Users can log in using their email address and password.                   |
| Home              | Completed       | Displays the main application actions available to the authenticated user. |
| Home → Check In   | Completed       | Allows the user to record their attendance check-in.                       |
| Home → Check Out  | Completed       | Allows the user to record their attendance check-out.                      |
| Home → Leaves     | Not Implemented | Navigates to the leaves screen.                                            |
| Home → Attendance | Not Implemented | Navigates to the attendance screen.                                        |
| Home → Sign Out   | Completed       | Signs the user out of the application.                                     |
| Leaves            | Not Implemented | Leave management functionality is planned but not yet implemented.         |
| Attendance        | Not Implemented | Attendance history/view functionality is planned but not yet implemented.  |


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
