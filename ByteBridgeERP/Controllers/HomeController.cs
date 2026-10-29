using System.Diagnostics;
using ByteBridgeERP.Data;
using ByteBridgeERP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ByteBridgeERP.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _db;
    public HomeController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var vm = new DashboardViewModel
        {
            ProductCount = await _db.Products.CountAsync(),
            CustomerCount = await _db.Customers.CountAsync(),
            SupplierCount = await _db.Suppliers.CountAsync(),
            PendingOrders = await _db.SalesOrders.CountAsync(o => o.Status == "Pending"),
            InventoryValue = (decimal)(await _db.Products.SumAsync(p => (double?)(p.UnitPrice * p.StockQuantity)) ?? 0d),
            LowStockProducts = await _db.Products.Where(p => p.StockQuantity <= p.ReorderLevel).OrderBy(p => p.StockQuantity).Take(6).ToListAsync(),
            RecentOrders = await _db.SalesOrders.Include(o => o.Customer).OrderByDescending(o => o.OrderDate).ThenByDescending(o => o.Id).Take(5).ToListAsync()
        };
        return View(vm);
    }

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}
