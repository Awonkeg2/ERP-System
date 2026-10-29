using ByteBridgeERP.Data;
using ByteBridgeERP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ByteBridgeERP.Controllers;

public class SuppliersController : Controller
{
    private readonly ApplicationDbContext _db;
    public SuppliersController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index(string? search)
    {
        ViewData["Search"] = search;
        var query = _db.Suppliers.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(s => s.Name.Contains(search) || (s.Company != null && s.Company.Contains(search)) || (s.Email != null && s.Email.Contains(search)));
        return View(await query.OrderBy(s => s.Name).ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var item = await _db.Suppliers.FindAsync(id);
        return item == null ? NotFound() : View(item);
    }

    public IActionResult Create() => View(new Supplier());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Email,Phone,Company,Address,Notes")] Supplier supplier)
    {
        if (!ModelState.IsValid) return View(supplier);
        _db.Add(supplier); await _db.SaveChangesAsync();
        TempData["Success"] = "Supplier created.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var item = await _db.Suppliers.FindAsync(id);
        return item == null ? NotFound() : View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,Phone,Company,Address,Notes")] Supplier supplier)
    {
        if (id != supplier.Id) return NotFound();
        if (!ModelState.IsValid) return View(supplier);
        _db.Update(supplier); await _db.SaveChangesAsync();
        TempData["Success"] = "Supplier updated.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var item = await _db.Suppliers.FindAsync(id);
        return item == null ? NotFound() : View(item);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var item = await _db.Suppliers.FindAsync(id);
        if (item == null) return NotFound();
        _db.Suppliers.Remove(item); await _db.SaveChangesAsync();
        TempData["Success"] = "Supplier deleted.";
        return RedirectToAction(nameof(Index));
    }
}
