using ByteBridgeERP.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Create the initial database when it does not exist. EnsureCreated does not
// upgrade existing databases, so the idempotent SQL below adds the finance
// tables when an earlier ByteBridge ERP database is reused.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureCreated();
    db.Database.ExecuteSqlRaw("""
        CREATE TABLE IF NOT EXISTS "Expenses" (
            "Id" INTEGER NOT NULL CONSTRAINT "PK_Expenses" PRIMARY KEY AUTOINCREMENT,
            "Reference" TEXT NOT NULL,
            "Category" TEXT NOT NULL,
            "Description" TEXT NOT NULL,
            "Amount" TEXT NOT NULL,
            "ExpenseDate" TEXT NOT NULL,
            "PaidTo" TEXT NULL,
            "PaymentMethod" TEXT NOT NULL,
            "Status" TEXT NOT NULL,
            "Notes" TEXT NULL
        );
        """);
    db.Database.ExecuteSqlRaw("""
        CREATE TABLE IF NOT EXISTS "Accounts" (
            "Id" INTEGER NOT NULL CONSTRAINT "PK_Accounts" PRIMARY KEY AUTOINCREMENT,
            "Code" TEXT NOT NULL,
            "Name" TEXT NOT NULL,
            "Type" TEXT NOT NULL,
            "Description" TEXT NULL,
            "OpeningBalance" TEXT NOT NULL,
            "IsActive" INTEGER NOT NULL
        );
        CREATE UNIQUE INDEX IF NOT EXISTS "IX_Accounts_Code" ON "Accounts" ("Code");
        """);
    SeedData.Initialize(db);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();
