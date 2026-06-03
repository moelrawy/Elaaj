#  Elaaj (عِلاج) - Healthcare & Pharmacy Integration Platform

![.NET Core](https://img.shields.io/badge/.NET%20Core-Purple?style=for-the-badge&logo=dotnet)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)
![Clean Architecture](https://img.shields.io/badge/Clean%20Architecture-Success?style=for-the-badge)

## 📌 Project Overview
**Elaaj** is a comprehensive backend system designed to bridge the gap between patients and local pharmacies. It provides a real-time bidding and notification ecosystem where patients can upload their prescriptions, and nearby pharmacies can instantly respond with offers, ensuring a seamless and efficient healthcare experience.

## ✨ Core Features
* **Real-Time Communication:** Powered by **SignalR** to deliver instant notifications and updates between patients and pharmacies.
* **Clean Architecture & CQRS:** Strictly structured to ensure modularity, scalability, and high maintainability using the **MediatR** pattern.
* **Robust Security:** Implemented **JWT Authentication** and strict Role-Based Access Control (RBAC) to isolate patient and pharmacy data.
* **High Performance:** Advanced server-side pagination, sorting, and dynamic filtering using **Entity Framework Core**.
* **System Stability:** A custom **Global Exception Handling Middleware** intercepts all unhandled errors, preventing crashes and returning consistent, mobile-friendly JSON responses.
* **API Documentation:** Interactive and comprehensive API endpoints documented via **Swagger UI**.

## 🛠️ Tech Stack
* **Backend Framework:** ASP.NET Core Web API
* **Language:** C#
* **Database & ORM:** MS SQL Server, Entity Framework Core (Code-First Approach)
* **Patterns & Principles:** Clean Architecture, CQRS, Repository Pattern, SOLID Principles
* **Real-Time Engine:** SignalR
* **Authentication:** ASP.NET Core Identity & JWT (JSON Web Tokens)

## 📂 Project Structure (Clean Architecture)
The solution is divided into four main layers:
1.  **Domain:** Contains Enterprise-wide logic and Types (Entities, Enums).
2.  **Application:** Contains Business Logic, CQRS Handlers (MediatR), DTOs, and Interfaces.
3.  **Infrastructure:** Contains Database Context, Repositories Implementation, and External Services (Auth, SignalR).
4.  **API (Presentation):** Contains Controllers, Middlewares, and Dependency Injection configurations.

## 🚀 Getting Started

### Prerequisites
* [.NET SDK](https://dotnet.microsoft.com/download) (Version 9.0 or higher)
* SQL Server (LocalDB or dedicated instance)
* Visual Studio 2022 or VS Code

### Installation & Setup
1.  **Clone the repository:**
    ```bash
    git clone <repository-url>
    cd Elaaj
    ```
2.  **Update Database Connection:**
    Navigate to `Elaaj.API/appsettings.json` and update the `DefaultConnection` string with your SQL Server credentials.
3.  **Apply Migrations:**
    Open the Package Manager Console (PMC) or terminal and run:
    ```bash
    Update-Database
    ```
    *Or using .NET CLI:*
    ```bash
    dotnet ef database update --project Elaaj.Infrastructure --startup-project Elaaj.API
    ```
4.  **Run the Application:**
    Press `F5` in Visual Studio or run the following command:
    ```bash
    dotnet run --project Elaaj.API
    ```
5.  **Explore the API:**
    Navigate to `https://localhost:<port>/swagger` in your browser to interact with the API endpoints.
