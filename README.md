# TripMate

> A production-ready .NET MAUI cross-platform application for managing group trips, splitting expenses, and settling balances among friends.

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![MAUI](https://img.shields.io/badge/MAUI-Cross--platform-0078D4?logo=xamarin)](https://learn.microsoft.com/dotnet/maui/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

---

## Table of Contents

- [Features](#features)
- [Tech Stack](#tech-stack)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [Project Structure](#project-structure)
- [Architecture](#architecture)
- [License](#license)

---

## Features

| Feature | Description |
|---------|-------------|
| **Authentication** | Register, login, logout (local storage) |
| **Trip Management** | Create, edit, delete trips with members |
| **Expense Management** | Add expenses, equal/custom split, per-person amounts |
| **Settlement** | Who owes whom – simplified debt resolution |
| **Dashboard** | Total cost, your paid, your balance |
| **Reports** | Expense list with member filter |
| **UI** | Pull-to-refresh, dark mode, iOS Cupertino style |

---

## Tech Stack

- **.NET 10** / **.NET MAUI**
- **C#** with MVVM pattern
- **CommunityToolkit.Mvvm**
- **SQL Server** with Entity Framework Core
- **Dependency Injection**
- **Shell Navigation**

---

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (17.10+) or [VS Code](https://code.visualstudio.com/) with .NET MAUI workload
- **iOS:** Xcode (Mac) or Mac build agent (Windows)
- **Android:** Android SDK
- **Database:** SQL Server or SQL Server LocalDB

---

## Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/your-username/trip-management.git
cd trip-management
```

### 2. Restore packages

```bash
dotnet restore
```

### 3. Configure database

Update `appsettings.json` with your SQL Server connection string (see [Configuration](#configuration)).

### 4. Run the app

**iOS (Mac):**
```bash
dotnet build -f net10.0-ios -t:Run
```

**Android:**
```bash
dotnet build -f net10.0-android -t:Run
```

**Windows:**
```bash
dotnet build -f net10.0-windows10.0.19041.0 -t:Run
```

---

## Configuration

### Database connection

Edit `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=TripMate;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
```

**Example connection strings:**

| Scenario | Connection String |
|----------|-------------------|
| Local SQL Server | `Server=.;Database=TripMate;Trusted_Connection=True;TrustServerCertificate=True` |
| SQL Server Express | `Server=.\SQLEXPRESS;Database=TripMate;Trusted_Connection=True;TrustServerCertificate=True` |
| SQL Authentication | `Server=YOUR_SERVER;Database=TripMate;User Id=user;Password=***;TrustServerCertificate=True` |

**Override via environment variable:**
```bash
TRIPMATE_CONNECTION_STRING="Server=.;Database=TripMate;..."
```

### Sample login credentials

After first run, sample data is seeded:

| Email | Password |
|-------|----------|
| alice@example.com | password123 |
| bob@example.com | password123 |
| carol@example.com | password123 |

---

## Project Structure

```
TripMate/
├── Models/              # User, Trip, TripMember, Expense, ExpenseSplit
├── ViewModels/          # MVVM ViewModels (CommunityToolkit.Mvvm)
├── Views/               # XAML pages
├── Services/            # Auth, Trip, Expense, Settlement, Dashboard
├── Data/                # AppDbContext, Repositories, DataSeeder
├── Helpers/             # PasswordHelper, ServiceHelper, DatabaseConfiguration
├── Resources/           # Styles, Colors, Fonts
└── appsettings.json     # SQL Server connection string
```

---

## Architecture

- **MVVM** – Views bind to ViewModels; no logic in code-behind
- **Repository pattern** – Data access abstraction
- **Async/await** – All I/O is asynchronous
- **Shell routing** – `GoToAsync` with query parameters (e.g. `tripId`)

### Navigation flow

```
Login → Trip List → Create Trip / Trip Detail
                         ↓
              Add Expense → Expense List
              Settlement
              Edit Trip / Members
```

---

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
# Trip-Management-App-
