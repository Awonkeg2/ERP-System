using ByteBridgeERP.Models;

namespace ByteBridgeERP.Data;

public static class SeedData
{
    public static void Initialize(ApplicationDbContext db)
    {
        // Seed sample operational data only when the core lists are empty.
        if (!db.Products.Any() && !db.Customers.Any() && !db.Suppliers.Any())
        {
            db.Products.AddRange(
                new Product { SKU = "BB-001", Name = "Starter Product", Category = "General", UnitPrice = 99.00m, StockQuantity = 25, ReorderLevel = 5, Description = "Example inventory item" },
                new Product { SKU = "BB-002", Name = "Packaging Box", Category = "Packaging", UnitPrice = 12.50m, StockQuantity = 100, ReorderLevel = 20, Description = "Example packaging stock" }
            );
            db.Customers.Add(new Customer { Name = "Demo Customer", Email = "customer@example.com", Phone = "000 000 0000", Company = "Example Trading" });
            db.Suppliers.Add(new Supplier { Name = "Demo Supplier", Email = "supplier@example.com", Phone = "000 000 0000", Company = "Example Supplies" });
            db.SaveChanges();
        }

        // Seed the finance chart independently, including when upgrading an
        // existing database that already contains products and customers.
        if (!db.Accounts.Any())
        {
            db.Accounts.AddRange(
                new Account { Code = "1000", Name = "Cash and Bank", Type = "Asset", Description = "Cash on hand and bank balances" },
                new Account { Code = "1100", Name = "Accounts Receivable", Type = "Asset", Description = "Amounts customers owe" },
                new Account { Code = "2000", Name = "Accounts Payable", Type = "Liability", Description = "Amounts owed to suppliers" },
                new Account { Code = "3000", Name = "Owner's Equity", Type = "Equity", Description = "Owner capital and equity" },
                new Account { Code = "4000", Name = "Sales Revenue", Type = "Income", Description = "Income from sales" },
                new Account { Code = "5000", Name = "Operating Expenses", Type = "Expense", Description = "General business expenses" }
            );
            db.SaveChanges();
        }
    }
}
