# Finance & Accounting module

This module is integrated into the same ByteBridge ERP ASP.NET Core MVC application and shares its EF Core SQLite database.

## Included now

- Finance dashboard with order-value estimates, paid expense totals, estimated net result, and pending/processing order value.
- Expense recording, listing, and deletion with date, category, amount, supplier/payee, payment method, paid/unpaid status, reference, and notes.
- Starter chart of accounts for assets, liabilities, equity, income, and expenses.
- Finance navigation from the shared layout.
- Server-side validation and anti-forgery protection on state-changing forms.
- Startup compatibility SQL to add `Expenses` and `Accounts` tables to an existing SQLite database without dropping the core ERP tables.

## Important accounting limitations

This is a finance tracking starter, **not a complete accounting system**. Revenue is calculated from non-cancelled sales orders, not confirmed cash receipts. Expenses are tracked independently and are not posted to accounts. Account opening balances are stored but are not included in balances or statements. There is no double-entry journal, debit/credit validation, VAT configuration, tax invoice, bank reconciliation, payroll, cash-flow statement, balance sheet, or income statement generated from a ledger.

Before using real business accounts, implement a balanced journal/ledger and have the accounting treatment reviewed by a qualified South African accounting professional. Back up the SQLite database before upgrading the app.
