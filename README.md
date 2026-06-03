# Ecommarce_BackEnd_Project
# E-Commerce API — Complete Architecture & Mentorship Guide

---

## Table of Contents

1. [Project Overview](#1-project-overview)
2. [Architecture Deep Dive](#2-architecture-deep-dive)
3. [Folder Structure](#3-folder-structure)
4. [Layer-by-Layer Explanation](#4-layer-by-layer-explanation)
5. [Design Patterns Used & WHY](#5-design-patterns-used--why)
6. [API Reference](#6-api-reference)
7. [Authentication & Authorization Flow](#7-authentication--authorization-flow)

---

## 1. Project Overview

A fully layered, production-grade E-Commerce REST API built with:

| Technology | Purpose |
|---|---|
| ASP.NET Core 10 | Web framework |
| Entity Framework Core 10 | ORM / database access |
| SQL Server | Relational database |
| ASP.NET Core Identity | User management |
| JWT Bearer | Stateless authentication |
| FluentValidation | Request validation |
| Salar / OpenAPI | API documentation |

---

## 2. Architecture Deep Dive

```
┌─────────────────────────────────────────────────────────┐
│                    HTTP Request                          │
└──────────────────────────┬──────────────────────────────┘
                           │
┌──────────────────────────▼──────────────────────────────┐
│               PRESENTATION LAYER (API)                   │
│  Controllers → validate input → call BLL service         │
│  Middleware: JWT Auth, Exception Handler, CORS           │
└──────────────────────────┬──────────────────────────────┘
                           │  DTOs only (no Entities)
┌──────────────────────────▼──────────────────────────────┐
│            BUSINESS LOGIC LAYER (BLL)                    │
│  Services → orchestrate domain logic → call DAL via UoW  │
│  FluentValidation runs here (auto via DI pipeline)       │
└──────────────────────────┬──────────────────────────────┘
                           │  Repositories + UoW
┌──────────────────────────▼──────────────────────────────┐
│             DATA ACCESS LAYER (DAL)                      │
│  Repositories → EF Core queries → SQL Server             │
│  Unit of Work coordinates transactions                   │
└──────────────────────────┬──────────────────────────────┘
                           │
┌──────────────────────────▼──────────────────────────────┐
│                    SQL SERVER DB                          │
└─────────────────────────────────────────────────────────┘

              ECommerce.Common (shared across all layers)
              DTOs · Wrappers · Enums · Constants · Helpers
```

### Dependency Rule (STRICT)
```
API  →  BLL  →  DAL  →  Common
API  →  Common
BLL  →  Common
DAL  →  Common

❌ DAL must NEVER reference BLL
❌ Common must NEVER reference any other layer
❌ Entities must NEVER leave the DAL/BLL boundary
```

---

## 3. Folder Structure

```
ECommerceAPI/
├── ECommerceAPI.sln
│
├── ECommerce.API/                          ← Presentation Layer
│   ├── Controllers/
│   │   ├── BaseApiController.cs           ← Abstract base (shared helpers)
│   │   ├── AuthController.cs              ← POST /api/auth/register|login
│   │   ├── CategoriesController.cs        ← /api/categories
│   │   ├── ProductsController.cs          ← /api/products
│   │   ├── CartController.cs              ← /api/cart
│   │   ├── OrdersController.cs            ← /api/orders
│   │   └── ImageController.cs             ← /api/image/upload
│   ├── Program.cs                         ← App entry point + pipeline
│   ├── appsettings.json
│   └── appsettings.Development.json
│
├── ECommerce.BLL/                          ← Business Logic Layer
│   ├── Managers/
│   │   ├── AuthService                    ← JWT generation, Identity
│   │   ├── CategoryService
│   │   ├── ProductService
│   │   ├── CartService
│   │   ├── OrderService
│   │   └── ImageService
│   ├── DTOs/
│   │   ├── Auth/      AuthDtos.cs
│   │   ├── Category/  CategoryDtos.cs
│   │   ├── Product/   ProductDtos.cs
│   │   ├── Cart/      CartDtos.cs
│   │   ├── Order/     OrderDtos.cs
│   │   └── Image/     ImageDtos.cs
│   ├── Validators/                        ← All FluentValidation rules
│   └── ServicesExtension/                 ← All Services inject
│
├── ECommerce.DAL/                          ← Data Access Layer
│   ├── Data/
│   │   └── Models/                        ← All Entities
│   ├── Context/
│   │   └── AppDbContext.cs                ← IdentityDbContext + audit stamps
│   ├── Configurations/
│   │   └── EntityConfigurations.cs        ← IEntityTypeConfiguration per entity
│   ├── Repositories/                      ← All Repositories
│   ├── ServicesExtension/
│   └── UnitOfWork/
│       └── UnitOfWork.cs                  ← IUnitOfWork + implementation
│
└── ECommerce.Common/                       ← Shared / Cross-cutting
    ├── Wrappers/
    │   ├── GeneralResult                  ← Standard response envelope
    │   └── PagedResult.cs                 ← Pagination metadata
    ├── Helpers/
    │   └── ClaimsHelper.cs                ← JWT claim extraction
    ├── Enums/
    │   └── Enums.cs                       ← OrderStatus, UserRole
    └── Constants/
        └── AppConstants.cs                ← Roles, Policies, ClaimTypes
```

---

## 4. Layer-by-Layer Explanation

### ECommerce.Common
The foundation. Every other project references it.
- **DTOs**: What travels across layer boundaries. Never exposes EF Entities.
- **GeneralResult\<T\>**: The single response contract — every endpoint returns this.
- **PagedResult\<T\>**: Pagination metadata wrapper.
- **ClaimsHelper**: Centralises JWT claim extraction — one place to update if claims change.
- **Constants**: Roles, Policies, ClaimType keys — never hardcode strings.

### ECommerce.DAL
Owns the database. Nothing outside this layer touches EF Core.
- **Entities**: Pure EF models. `BaseEntity` provides `CreatedAt`/`UpdatedAt` automatically.
- **AppDbContext**: `IdentityDbContext` (gives us all 7 Identity tables). Overrides `SaveChangesAsync` to auto-stamp audit fields.
- **EntityConfigurations**: Fluent API configs via `IEntityTypeConfiguration<T>`. Keeps `OnModelCreating` clean.
- **GenericRepository**: Reusable CRUD. Never write `_context.Set<T>()` in BLL.
- **SpecificRepositories**: Complex queries (pagination, eager loading) go here, not in BLL.
- **UnitOfWork**: Lazily initialises repositories. Wraps `SaveChangesAsync`. Exposes `ExecuteInTransactionAsync` for atomic multi-step operations.

### ECommerce.BLL
The brain of the system. Contains all business decisions.
- **Services**: Orchestrate repositories. Only work with DTOs and Entities internally. Return `ApiResponse<T>`.
- **Validators**: FluentValidation rules. Auto-triggered by `AddFluentValidationAutoValidation()`.
- **AuthService**: JWT token generation. Identity user management. Role assignment.

### ECommerce.API
The thin HTTP interface. Controllers should be dumb — they call services and return HTTP results.
- **ServiceExtensions**: DI registrations split into focused methods. Program.cs stays readable.

---

## 5. Design Patterns Used & WHY

### N-Tier Architecture
**Why**: Separation of concerns. You can swap SQL Server for PostgreSQL by only touching DAL. You can replace JWT with OAuth without touching BLL. Each layer is independently testable.

### Repository Pattern
**Why**: Hides EF Core behind an interface. BLL services don't know if data comes from SQL Server, an in-memory store, or a mock — critical for unit testing without a real database.

### Generic Repository
**Why**: Eliminates 80% of boilerplate. All entities get `GetByIdAsync`, `AddAsync`, `Update`, `Remove` for free. Specific repos only add domain-specific queries on top.

### Unit of Work
**Why**: Without it, calling `SaveChanges` in each repository separately means partial failure is possible. UoW ensures all changes in a request commit together or roll back together. The `ExecuteInTransactionAsync` is essential for the Place Order flow (create order + clear cart = one atomic operation).

### DTO Pattern
**Why**: Entities carry database concerns (navigation properties, EF annotations, lazy loading proxies). Exposing them in APIs leaks your schema, causes serialisation cycles, and makes it hard to version your API independently of your database.

### Result Pattern (GeneralResult\<T\>)
**Why**: API clients get a predictable contract every time. No guessing whether a 200 means success. No dealing with raw exceptions. Consistent error messages with error lists. Eliminates try/catch boilerplate in controllers.

### FluentValidation
**Why**: Keeps validation logic out of controllers AND services. Validation rules are testable in isolation. Supports complex cross-field rules (e.g., `ConfirmPassword == Password`) cleanly. `AddFluentValidationAutoValidation()` triggers it before controller action methods run.

---

## 6. API Reference

### Authentication
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/api/auth/register` | Public | Register new user |
| POST | `/api/auth/login` | Public | Login → returns JWT |

---

### Categories
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/categories` | Public | List all categories |
| GET | `/api/categories/{id}` | Public | Get category details |
| POST | `/api/categories` | Admin | Create category |
| PUT | `/api/categories/{id}` | Admin | Update category |
| DELETE | `/api/categories/{id}` | Admin | Soft delete category |
| POST | `/api/categories/{id}/image` | Admin | Upload category image |

---

### Products
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/products` | Public | List products (filter + page) |
| GET | `/api/products/{id}` | Public | Get product details |
| POST | `/api/products` | Admin | Create product |
| PUT | `/api/products/{id}` | Admin | Update product |
| DELETE | `/api/products/{id}` | Admin | Soft delete product |
| POST | `/api/products/{id}/image` | Admin | Upload product image |

**Product Query Parameters:**
```
GET /api/products?categoryId=1&name=laptop&minPrice=100&maxPrice=1000
                 &pageNumber=1&pageSize=10&sortBy=price&sortDescending=false
```

---

### Cart (Requires Auth)
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/cart` | Customer/Admin | View my cart |
| POST | `/api/cart` | Customer/Admin | Add item to cart |
| PUT | `/api/cart` | Customer/Admin | Update item quantity |
| DELETE | `/api/cart/{productId}` | Customer/Admin | Remove item |

---

### Orders (Requires Auth)
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/api/orders` | Customer/Admin | Place order from cart |
| GET | `/api/orders` | Customer/Admin | My order history |
| GET | `/api/orders/{id}` | Customer/Admin | Order details |

---

### Images (Admin only)
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/api/image/upload` | Admin | Upload standalone image |
| POST | `/api/products/{id}/image` | Admin | Set product image |
| POST | `/api/categories/{id}/image` | Admin | Set category image |

---

## 7. Authentication & Authorization Flow

```
Client                          API                         Database
  │                              │                               │
  │  POST /api/auth/login        │                               │
  │ ─────────────────────────►  │                               │
  │                              │  FindByEmailAsync(email)      │
  │                              │ ────────────────────────────► │
  │                              │ ◄──────────────────────────── │
  │                              │  CheckPasswordAsync(hash)     │
  │                              │  GetRolesAsync(user)          │
  │                              │  GenerateJwtToken()           │
  │  { token: "eyJ..." }        │                               │
  │ ◄─────────────────────────  │                               │
  │                              │                               │
  │  GET /api/cart               │                               │
  │  Authorization: Bearer eyJ.. │                               │
  │ ─────────────────────────►  │                               │
  │                              │  ValidateToken()              │
  │                              │  → Extract claims             │
  │                              │  → userId = "abc123"          │
  │                              │  → roles = ["Customer"]       │
  │                              │  → Policy "AuthenticatedUser" ✓│
  │                              │  GetCartAsync("abc123")       │
  │                              │ ────────────────────────────► │
  │  { success: true, data:{} } │                               │
  │ ◄─────────────────────────  │                               │
```

### Policy Matrix
| Policy | Roles Allowed | Used On |
|--------|--------------|---------|
| `AdminOnly` | Admin | Category/Product CRUD, Image upload |
| `CustomerOnly` | Customer | (available if needed) |
| `AuthenticatedUser` | Admin + Customer | Cart, Orders |
| Public (no policy) | Anyone | Auth, GET Products, GET Categories |


