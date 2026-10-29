# ByteBridge ERP — ASP.NET Core MVC starter

A small, readable ERP foundation built with **ASP.NET Core MVC**, **Entity Framework Core**, and **SQLite**. It is intended as a local prototype that you can extend into ByteBridge's business management platform.

## Included modules

- **Dashboard:** inventory value, item/customer counts, pending orders, recent orders and low-stock watchlist.
- **Inventory:** create, view, edit, delete and search products; SKU uniqueness; reorder levels.
- **Customers:** CRUD and search for customer/business contact records.
- **Suppliers:** CRUD and search for supplier records.
- **Sales orders:** select a customer, add one or more products, validate quantities against stock, calculate totals, and track Pending / Processing / Completed / Cancelled status.
- **Finance overview:** sales-order revenue estimate, paid expenses, estimated net result and open-order value.
- **Expense tracking:** record operating expenses, categories, dates, payment methods and Paid / Unpaid status.
- **Chart of accounts:** starter accounts for assets, liabilities, equity, income and expenses.
- **SQLite database:** created automatically at first launch; demo records are seeded on an empty database.
- **Basic safeguards:** server-side model validation, anti-forgery tokens on POST forms, parameterized EF Core queries, and checks before deleting products/customers referenced by orders.

## Requirements

- .NET 8 SDK
- Internet on first run to restore NuGet packages and load Bootstrap from its CDN (the application can be adapted to local assets if needed).

Check installation in a terminal:

```bash
dotnet --info
```

## Run the application

1. Extract the ZIP file.
2. Open a terminal in the folder containing `ByteBridgeERP.csproj`.
3. Restore packages:

   ```bash
   dotnet restore
   ```

4. Start the app:

   ```bash
   dotnet run
   ```

5. Open the local URL printed in the terminal (usually an `https://localhost:xxxx` or `http://localhost:xxxx` address).

The first run creates `bytebridge-erp.db` in the application's working directory and adds two sample products, one customer, one supplier and a starter chart of accounts. These are clearly labelled demo records and can be edited or deleted.

### Database note

This starter uses `Database.EnsureCreated()` for first launch and also runs idempotent SQLite `CREATE TABLE IF NOT EXISTS` statements for the Finance tables. This lets the finance module be added when reusing an earlier ByteBridge ERP SQLite database without deleting the old data. Back up your `.db` file before testing an upgrade. **This is a prototype convenience, not a replacement for EF Core migrations in production.** For a production schema-change strategy, move to EF Core migrations and use `dotnet ef database update`.

## Important prototype limitations

This is **not production-ready ERP software yet**:

- There is no login, user/role permissions, multi-company separation or audit log.
- The finance module is a starter tracker, not a complete accounting ledger. The chart of accounts is not yet linked to journal postings; there is no double-entry general ledger, VAT handling, tax invoice generation, payment integration, payroll, bank reconciliation or financial statement certification.
- Stock is reduced when an order is created, and restored when a non-cancelled order is cancelled. There is no reservation workflow, return processing or full stock movement ledger yet.
- Order status changes use POST with an anti-forgery token.
- No automated tests, database backup process, monitoring, or deployment configuration.
- SQLite is appropriate for local prototyping; evaluate a managed SQL database for concurrent production use.
- Before real customers use it, add authentication/authorization, proper audit logging, role-specific access, secure deployment, backups, input review, privacy compliance, and tests.

## Suggested next development phases

1. Add ASP.NET Core Identity (admin, manager, sales, inventory roles).
2. Add purchase orders and goods-received notes to increase stock.
3. Add a stock movement ledger and prevent negative inventory in concurrent requests.
4. Build a proper double-entry general ledger (journal entries and balanced debit/credit lines), then link sales, expenses and purchasing to it.
5. Add reporting/export, VAT configuration, invoices and payment tracking.
5. Add company settings, multi-tenant data separation, backups, tests and deployment automation.

## Project structure

```text
ByteBridgeERP/
├── Controllers/       # HTTP actions and workflow logic
├── Data/              # EF Core DbContext and demo seed data
├── Models/            # Entities and dashboard view model
├── Views/             # Razor pages grouped by controller
├── wwwroot/css/       # Application styling
├── Program.cs         # Dependency injection, database startup and routes
└── appsettings.json   # SQLite connection string and logging
```

## How MVC is used

- **Model:** `Product`, `Customer`, `Supplier`, `SalesOrder`, and `SalesOrderItem` represent data and validation rules.
- **View:** Razor `.cshtml` files display forms, tables and the dashboard.
- **Controller:** classes receive requests, validate input, call EF Core and return a view or redirect.
- **DbContext:** `ApplicationDbContext` maps the model classes to SQLite tables and configures relationships.
