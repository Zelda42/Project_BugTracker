# Bug Tracker - ASP.NET Core MVC

A full-featured Bug Tracker built with ASP.NET Core MVC, featuring role-based authentication, automatic bug assignment, and status tracking.

## 🚀 Features:
- Role-based authentication (Admin, Developer, Reporter)
- Bug lifecycle management (Open, In Progress, Resolved, Closed)
- Automatic Developer assignment for unassigned bugs
- Admin panel for managing users and roles
- Bug tracking with creation timestamps
- Responsive UI with Bootstrap

## 🛠️ Tech Stack:
- **Framework:** ASP.NET Core MVC (.NET 7+)
- **Database:** SQL Server with Entity Framework Core
- **Authentication:** ASP.NET Identity with Role Management
- **Frontend:** Razor Views + Bootstrap
- **Version Control:** Git/GitHub

- # 🛠️ Bug Tracker - Setup Guide

## 🚀 Prerequisites
Before running the Bug Tracker, ensure you have the following installed:

### 🔹 Required Software
| Tool                  | Version  | Download Link |
|-----------------------|---------|--------------|
| **.NET SDK**         | 8.0.4+  | [Download](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) |
| **SQL Server**       | 2019+   | [Download](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) |
| **Visual Studio**    | 2022+   | [Download](https://visualstudio.microsoft.com/downloads/) |
| **Git**              | Latest  | [Download](https://git-scm.com/) |
| **Docker (Optional)** | Latest  | [Download](https://www.docker.com/get-started/) |

---
## 🔹 Step 1: Clone the Repository
## 🔹 Step 2: Configure the Database

### **Option 1: Using Local SQL Server**
1. Open **SQL Server Management Studio (SSMS)** and create a new database called `BugTrackerDB`.
2. Update **`appsettings.json`** to match your **SQL Server connection**:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=BugTrackerDB;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

3. Run database migrations:

```sh
dotnet ef database update
```

---

### **Option 2: Using Docker for SQL Server (Optional)**
Run this **Docker command** to start a SQL Server instance:

```sh
docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=YourPassword123!' -p 1433:1433 -d mcr.microsoft.com/mssql/server:2019-latest
```

Then update `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost,1433;Database=BugTrackerDB;User Id=sa;Password=YourPassword123!;MultipleActiveResultSets=true"
}
```

---

## 🔹 Step 3: Run the Application
1. Open the project in **Visual Studio**.
2. Set `BugTracker1._2025` as the **Startup Project**.

4. Open a browser and navigate to:

```
https://localhost:5001
```

---

## 🔹 Step 4: Create an Admin User
To manually create an Admin user:
1. **Register a new user** on the **Login/Register page**.
2. **Use SSMS or SQL Query** to assign the Admin role:

```sql
INSERT INTO AspNetUserRoles (UserId, RoleId)
SELECT u.Id, r.Id FROM AspNetUsers u, AspNetRoles r
WHERE u.Email = 'your-email@example.com' AND r.Name = 'Admin';
```

3. **Restart the application** and log in as Admin.

---

## 🔹 Step 5: Create a User via Admin Panel
1. **Login as Admin**.
2. **Go to the Admin Panel** (Manage Users).
3. **Click "Create User"** to add a new Developer or Reporter.

## 🛠️ Features Overview
| Feature                | Description |
|------------------------|------------|
| **Role Management**   | Admins can assign **Developers, Reporters**. |
| **Bug Tracking**      | Create, assign, and update bug statuses. |
| **Auto Bug Assignment** | Bugs are **auto-assigned to Developers**. |
| **Role-Based Access**  | Reporters can create bugs, Developers fix them, Admins manage users. |
| **Bug Timeline**      | Track **creation date & time** of each bug. |
