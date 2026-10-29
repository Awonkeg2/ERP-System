using ByteBridgeERP.Data;
using ByteBridgeERP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ByteBridgeERP.Controllers;

public class SalesOrdersController : Controller
{
    private readonly ApplicationDbContext _db;
    public SalesOrdersController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index(string? status)
    {
        ViewData["Status"] = status;
        var query = _db.SalesOrders.Include(o => o.Customer).AsQueryable();
        if (!string.IsNullOrWhiteSpace(status)) query = query.Where(o => o.Status == status);
        return View(await query.OrderByDescending(o => o.OrderDate).ThenByDescending(o => o.Id).ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var order = await _db.SalesOrders.Include(o => o.Customer).Include(o => o.Items).ThenInclude(i => i.Product).FirstOrDefaultAsync(o => o.Id == id);
        return order == null ? NotFound() : View(order);
    }

    public async Task<IActionResult> Create()
    {
        await LoadLists();
        return View(new SalesOrder { OrderDate = DateTime.Today, Status = "Pending" });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int customerId, DateTime orderDate, string status, string? notes, int[] productIds, int[] quantities)
    {
        if (customerId <= 0 || !await _db.Customers.AnyAsync(c => c.Id == customerId))
            ModelState.AddModelError("CustomerId", "Select a valid customer.");
        if (productIds.Length == 0 || quantities.Length != productIds.Length)
            ModelState.AddModelError("", "Add at least one product and quantity.");
        if (!new[] { "Pending", "Processing", "Completed", "Cancelled" }.Contains(status))
            ModelState.AddModelError("Status", "Select a valid status.");

        var items = new List<SalesOrderItem>();
        if (productIds.Length == quantities.Length)
        {
            for (var i = 0; i < productIds.Length; i++)
            {
                var product = await _db.Products.FindAsync(productIds[i]);
                if (product == null || quantities[i] < 1)
                {
                    ModelState.AddModelError("", "Each order line needs a valid product and quantity.");
                    continue;
                }
                if (status != "Cancelled" && product.StockQuantity < quantities[i])
                    ModelState.AddModelError("", $"Not enough stock for {product.Name}. Available: {product.StockQuantity}.");
                items.Add(new SalesOrderItem { ProductId = product.Id, Quantity = quantities[i], UnitPrice = product.UnitPrice });
            }
        }

        if (!ModelState.IsValid)
        {
            await LoadLists();
            ViewBag.SelectedCustomerId = customerId;
            ViewBag.PostedStatus = status;
            ViewBag.PostedDate = orderDate.ToString("yyyy-MM-dd");
            ViewBag.PostedNotes = notes;
            ViewBag.PostedProductIds = productIds;
            ViewBag.PostedQuantities = quantities;
            return View(new SalesOrder { CustomerId = customerId, OrderDate = orderDate, Status = status, Notes = notes });
        }

        var order = new SalesOrder
        {
            CustomerId = customerId,
            OrderDate = orderDate,
            Status = status,
            Notes = notes,
            Items = items,
            Total = items.Sum(i => i.Quantity * i.UnitPrice)
        };

        // Stock is reduced when an order is created unless it is cancelled.
        if (status != "Cancelled")
        {
            foreach (var item in items)
            {
                var product = await _db.Products.FindAsync(item.ProductId);
                product!.StockQuantity -= item.Quantity;
            }
        }
        _db.SalesOrders.Add(order);
        await _db.SaveChangesAsync();
        TempData["Success"] = $"Order {order.OrderNumber} created.";
        return RedirectToAction(nameof(Details), new { id = order.Id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, string status)
    {
        var order = await _db.SalesOrders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id);
        if (order == null) return NotFound();
        if (!new[] { "Pending", "Processing", "Completed", "Cancelled" }.Contains(status))
        {
            TempData["Error"] = "Invalid order status.";
            return RedirectToAction(nameof(Details), new { id });
        }
        if (order.Status == "Cancelled" && status != "Cancelled")
        {
            TempData["Error"] = "Cancelled orders cannot be reactivated automatically; stock reconciliation is required.";
            return RedirectToAction(nameof(Details), new { id });
        }
        if (status == "Cancelled" && order.Status != "Cancelled")
        {
            foreach (var item in order.Items)
            {
                var product = await _db.Products.FindAsync(item.ProductId);
                if (product != null) product.StockQuantity += item.Quantity;
            }
        }
        order.Status = status;
        await _db.SaveChangesAsync();
        TempData["Success"] = "Order status updated.";
        return RedirectToAction(nameof(Details), new { id });
    }

    private async Task LoadLists()
    {
        ViewBag.Customers = new SelectList(await _db.Customers.OrderBy(c => c.Name).ToListAsync(), "Id", "Name");
        ViewBag.Products = await _db.Products.Where(p => p.StockQuantity > 0).OrderBy(p => p.Name).ToListAsync();
    }
}
