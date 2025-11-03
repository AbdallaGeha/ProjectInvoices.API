# 🏗️ ProjectInvoices.API

A  **.NET 8 Web API** for managing construction project invoices — from project and supplier and items setup, to invoice creation and approvals, and transactional payment handling.

This personal project showcases my skills in **clean architecture**, **Entity Framework Core**, **transaction management**, and **secure role-based authorization**, designed to reflect real-world enterprise backend systems.

It is used alongside: 
- [Login Management API](https://github.com/AbdallaGeha)
- [Client Angular APP](https://github.com/AbdallaGeha)

---

## 🚀 What This Project Demonstrates

- ✅ **.NET 8 Web API** development
- ✅ **Clean Architecture** with separation of concerns
- ✅ **EF Core (code-first)** with SQL Server
- ✅ **Database transactions** for complex workflows
- ✅ **Role-based JWT authentication** (via external Login API)
- ✅ **AutoMapper** with custom value mappings
- ✅ **Unit testing** of controllers, services, and repositories
- ✅ **Search, filtering, and pagination**
- ✅ **Exception handling** 
- ✅ **Clean, well-documented code** using XML comments and inline explanations

---

## 💼 Real-World Inspiration

This project is inspired by a production system I worked on during my time as a .NET developer at a construction company in Syria. I simplified the logic for the purpose of showcasing my skills in a public, portfolio-friendly format, while keeping the core business concepts.


## 🧠 Problem Solved

In construction, managing project invoices and payments can involve complex workflows and financial approval chains. This API simulates that by:

1. Allowing authorized users to manage:
   - Projects
   - Suppliers
   - Items
2. Enabling invoice creation with multiple line items by authorized users
3. Supporting approval workflows
4. Generating suggested payments automatically
5. Processing payments by authorized users using:
   - Cash movements
   - Check movements tied to existing banks/accounts
6. Using **database transactions** to guarantee data consistency

---

## 🧱 Architecture Overview

I implemented a **Clean Architecture** pattern to promote testability, modularity, and maintainability:

Application → API Controllers - Services (business Logic orchestration) - DTOs 
Domain → Domain Models and Interfaces (including repository interfaces)
Infrastructure → EF Core, DB Context, Repositories


- **EF Core** is used with **code-first migrations**
- **Services** encapsulate complex logic
- **Repositories** for DB operations including pagination and search
- **Queries** for complex queries
- **DTOs + AutoMapper** abstract internal structures from external consumers

---

## 🔐 Authentication & Authorization

- Role-based authorization with **JWT tokens**
- Authentication handled via external `LoginManagement.API` [Login Management API](https://github.com/yourusername)
- User roles determine access to features like:
  - Setup of project and supplier and items
  - Project invoice creation and approval
  - Payment processing
  
---

## 💳 Payment Handling Logic

Once an invoice is approved, a **suggested payment** is generated.

Payments can then be processed:
- As **single payments**
- Or in **groups**, combining:
  - 💵 Cash Movements
  - 🧾 Check Movements (with selected banks and accounts)

All payment-related operations are executed using **DB transactions** to ensure atomicity.

---

## 🧪 Testing Strategy

I wrote **unit tests** for the following components:

- Controllers
- Services
- Repositories

Testing tools:
- `xUnit`
- `Moq`
- `In-memory DB contexts`

> Note: Common CRUD logic was omitted from testing to reduce redundancy and focus on distinct business logic.

---

## 🔍 Sample Use Case
1. Login as admin for simplicity
2. Create a project, supplier, and list of items, Bank, Bank account
3. Create a new invoice by selecting a project and supplier, then adding items (with quantity and price)
4. Approve invoice
5. Suggested payment is generated automatically
6. Process payment using cash/check movements

---

## 📦 Technologies Used

| Technology       | Purpose                            |
|------------------|-------------------------------------|
| .NET 8           | Web API framework                  |
| Entity Framework Core | ORM + code-first migrations     |
| SQL Server       | Relational database                |
| AutoMapper       | DTO ↔️ Domain mapping                |
| xUnit / Moq      | Unit testing                       |
| JWT Auth         | Secure authentication/authorization |
| Swagger          | API documentation/testing          |

---

## ⚙️ How to Run (As a part of the system)

1. Run Login Management API: [Login Management API](https://github.com/AbdallaGeha)  

2. Clone the repo  
   `git clone https://github.com/AbdallaGeha/ProjectInvoices.API.git` or via Visual Studio

3. Change DB connection string in appsettings.json to match your SQL Server
   
4. Run the API (The DB will be created automatically) 
   `dotnet run` or via Visual studio

5. Run Client Angular App [Client App](https://github.com/AbdallaGeha)

6. Login to the system as admin using:
   User name: admin@admin.com
   Password: P@ssw0rd

7. You can try the simplest scenario following steps mentioned in **Sample use case** section   
---

## 📈 What the project covers

- Designing a maintainable, layered architecture from scratch
- Using **transactions** to handle multi-entity financial flows
- Implementing **role-based security** and modular login via external auth APIs
- Building **DTOs** and integrating AutoMapper effectively
- Writing **unit tests** focused on core logic and edge cases

---

## 👋 About Me

I'm a .net developer focused on building scalable, testable applications using modern .NET technologies. My proficiency includes C#, ASP.NET Core, Web API, MVC, and Entity Framework Core. Also, I have experience Delivering end-to-end solutions. My skills include HTML, CSS, JavaScript, and experience with popular front-end frameworks like Angular. I have many years of experience in designing and optimizing relational databases, with a strong understanding of SQL Server and experience in database modelling, stored procedures, and data manipulation.

This project is part of my portfolio to demonstrate my ability to model real-world business logic and design clean, maintainable systems.

- 💼 [LinkedIn](https://www.linkedin.com/in/abdalla-geha-9664b62b7/)
- 📂 [More Projects](https://github.com/AbdallaGeha)
- 📧 Contact: abd_geha@hotmail.com

---

