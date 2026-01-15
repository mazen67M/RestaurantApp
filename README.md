# Restaurant App - Food Ordering Platform

Customizable, high-performance food ordering and restaurant management platform built with .NET 8/10 and Blazor.

## 🚀 Architecture

The solution follows **Clean Architecture** principles, ensuring separation of concerns and maintainability:

- **RestaurantApp.Domain**: Core entities, enums, and domain logic.
- **RestaurantApp.Application**: Service interfaces, DTOs, and application logic.
- **RestaurantApp.Infrastructure**: EF Core data access, identity, and third-party service implementations (Email, etc.).
- **RestaurantApp.API**: ASP.NET Core Web API providing RESTful endpoints for mobile and web clients.
- **RestaurantApp.Web**: Modern Blazor-based Admin Dashboard for restaurant management.
- **RestaurantApp.UnitTests**: XUnit test suite covering core service logic.

## 🛠️ Technology Stack

- **Backend**: ASP.NET Core Web API (.NET 10)
- **Database**: SQL Server with Entity Framework Core
- **Identity**: ASP.NET Core Identity with JWT Authentication
- **Frontend**: Blazor Web App (Server-side/InteractiveAuto)
- **Real-time**: SignalR for live order notifications
- **Documentation**: Swagger/OpenAPI

## 📦 Features

- **Menu Management**: Categories, menu items, and add-ons with image support.
- **Order Processing**: Real-time order tracking, status updates, and history.
- **Delivery System**: Branch-based delivery zones and driver assignment.
- **Loyalty & Rewards**: Points system and coupon/promo code management.
- **User Management**: Role-based access control (Admin, Cashier, Customer).
- **Reports**: Revenue summaries, popular items, and branch performance analytics.

## ⚙️ Setup Instructions

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/sql-server)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (recommended)

### Local Configuration
1. Clone the repository.
2. Update the connection string in `src/RestaurantApp.API/appsettings.json` and `src/RestaurantApp.Web/appsettings.json`.
3. Set the `JWT_SECRET_KEY` environment variable or update it in configuration (development only).

### Database Setup
Run the following command from the root directory to apply migrations:
```bash
dotnet ef database update --project src/RestaurantApp.Infrastructure --startup-project src/RestaurantApp.API
```

### Running the Application
To run both the API and the Admin Dashboard:
```bash
# Start the API
dotnet run --project src/RestaurantApp.API

```bash
# Start the Web Dashboard
dotnet run --project src/RestaurantApp.Web
```

### Running Tests
Execute the unit test suite:
```bash
dotnet test
```

## 🔒 Security Features

- **JWT Authentication**: Secure token-based access for mobile and web.
- **Rate Limiting**: Protection against DoS and brute-force attacks.
- **IDOR Protection**: Role-based authorization on sensitive data endpoints.
- **Structured Logging**: Production-ready diagnostics via `ILogger`.
- **Preflight Guards**: Debug-only endpoints restricted via preprocessor directives.
- **Advanced Diagnostics**: Structured JSON logging via Serilog for production monitoring.
- **Health Monitoring**: Integrated health checks for database and service availability.

## 📄 License
This project is licensed under the MIT License.
