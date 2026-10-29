using ByteBridgeERP.Models;
using Microsoft.EntityFrameworkCore;

namespace ByteBridgeERP.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<SalesOrder> SalesOrders => Set<SalesOrder>();
    public DbSet<SalesOrderItem> SalesOrderItems => Set<SalesOrderItem>();
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<Account> Accounts => Set<Account>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>()
            .HasIndex(p => p.SKU).IsUnique();

        modelBuilder.Entity<SalesOrder>()
            .HasMany(o => o.Items)
            .WithOne(i => i.SalesOrder)
            .HasForeignKey(i => i.SalesOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SalesOrderItem>()
            .HasOne(i => i.Product)
            .WithMany()
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SalesOrder>()
            .Property(o => o.Total)
            .HasPrecision(18, 2);
        modelBuilder.Entity<SalesOrderItem>()
            .Property(i => i.UnitPrice)
            .HasPrecision(18, 2);
        modelBuilder.Entity<Product>()
            .Property(p => p.UnitPrice)
            .HasPrecision(18, 2);
    }
}
