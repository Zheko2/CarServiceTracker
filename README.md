# 🚗 CarServiceTracker

CarServiceTracker is an ASP.NET Core MVC web application designed to help users manage their cars, service records, expenses, and garage assignments in an organized and secure way.

---

## 📌 Project Overview

The application allows users to:
- Manage their personal cars
- Track service history and maintenance
- Record additional expenses
- Assign cars to garages
- Monitor car status in real-time

The system also includes an Administrator role with full access and management capabilities.

---

## ⚙️ Technologies Used

- ASP.NET Core MVC (.NET 6+)
- Entity Framework Core
- Microsoft SQL Server
- ASP.NET Core Identity (Authentication & Authorization)
- Bootstrap (Responsive UI)
- Razor Views

---

## 🔐 User Roles

### 👤 User
- Can create and manage their own cars
- Can add service records and expenses for their cars
- Can view available garages
- Can assign cars to garages
- Can update car status

### 🛠 Administrator
- Has full access to all data
- Can create and delete garages
- Can view all users' cars, service records, and expenses
- Can manage the entire system

---

## 🚀 Features

- ✔️ User authentication (Register / Login / Logout)
- ✔️ Role-based authorization (User / Administrator)
- ✔️ Car management (CRUD)
- ✔️ Service records tracking
- ✔️ Expense tracking with categories (Fuel, Insurance, Tax, etc.)
- ✔️ Garage system (shared, admin-managed)
- ✔️ Car status system (Repairing, Changing, Repaired, Changed)
- ✔️ Search functionality (Cars)
- ✔️ Filtering (Service Records by Car)
- ✔️ Pagination (Service Records)
- ✔️ Custom error pages (404, Access Denied)
- ✔️ Data validation (client & server-side)
- ✔️ Secure data access (users see only their own data)

---

## 🗂 Architecture

The project follows a layered architecture:

- **Controllers** → Handle HTTP requests
- **Services** → Business logic and data operations
- **Models** → Application data structure
- **Views** → UI (Razor)

Dependency Injection is used throughout the application.

---

## 🧪 Data & Validation

- Data is stored in SQL Server using Entity Framework Core
- Proper validation is implemented using Data Annotations
- Protection against common vulnerabilities:
  - CSRF (AntiForgeryToken)
  - Authorization checks
  - Data filtering by owner

---

## 🛠 Setup Instructions

1. Clone the repository
2. Open the solution in Visual Studio 2022
3. Configure the connection string in `appsettings.json`
4. Run the following commands in Package Manager Console:

```powershell
Update-Database