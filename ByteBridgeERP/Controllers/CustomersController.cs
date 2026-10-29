using ByteBridgeERP.Data;
using ByteBridgeERP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ByteBridgeERP.Controllers;

public class CustomersController : Controller
{
    private readonly ApplicationDbContext _db;
    public CustomersController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index(string? search)
    {
        ViewData["Search"] = search;
        var query = _db.Customers.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(c => c.Name.Contains(search) || (c.Company != null && c.Company.Contains(search)) || (c.Email != null && c.Email.Contains(search)));
        return View(await query.OrderBy(c => c.Name).ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var item = await _db.Customers.FindAsync(id);
        return item == null ? NotFound() : View(item);
    }

    public IActionResult Create() => View(new Customer());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Email,Phone,Company,Address")] Customer customer)
    {
        if (!ModelState.IsValid) return View(customer);
        _db.Add(customer); await _db.SaveChangesAsync();
        TempData["Success"] = "Customer created.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var item = await _db.Customers.FindAsync(id);
        return item == null ? NotFound() : View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,Phone,Company,Address,CreatedAt")] Customer customer)
    {
        if (id != customer.Id) return NotFound();
        if (!ModelState.IsValid) return View(customer);
        _db.Update(customer); await _db.SaveChangesAsync();
        TempData["Success"] = "Customer updated.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var item = await _db.Customers.FindAsync(id);
        return item == null ? NotFound() : View(item);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var item = await _db.Customers.FindAsync(id);
        if (item == null) return NotFound();
        if (await _db.SalesOrders.AnyAsync(o => o.CustomerId == id))
        {
            TempData["Error"] = "This customer has orders and cannot be deleted.";
            return RedirectToAction(nameof(Index));
        }
        _db.Customers.Remove(item); await _db.SaveChangesAsync();
        TempData["Success"] = "Customer deleted.";
        return RedirectToAction(nameof(Index));
    }
}
