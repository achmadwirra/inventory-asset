# IT Asset Inventory Management System

<img width="1273" height="779" alt="image" src="https://github.com/user-attachments/assets/0b3bab30-06cd-4163-98ab-0988ce9d78e8" />


Inventory Management System untuk pengelolaan aset IT perusahaan.

## 🔧 Tech Stack

### Backend
- ASP.NET Core 8 Web API
- Clean Architecture
- Entity Framework Core
- PostgreSQL
- JWT Authentication & Role-based Authorization

### Frontend
- React + Vite + TypeScript
- React Router
- TanStack Query
- Zustand
- React Hook Form + Zod

---

## ✨ Features

- Authentication & Authorization (Admin, ITStaff, Employee)
- Asset Category Management
- Asset Creation
- Asset Assignment & Return
- Asset Lifecycle Management
- Audit Trail (automatic logging)
- Role-based UI access

---

## 🧠 Architecture

Backend menggunakan Clean Architecture:
- Domain
- Application
- Infrastructure
- WebAPI

Frontend menggunakan feature-based architecture.

---

## 🚀 Getting Started

### Backend
```bash
dotnet restore
dotnet ef database update
dotnet run
