using ByteBridgeERP.Data;
using ByteBridgeERP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ByteBridgeERP.Controllers;

public class ProductsController : Controller
{
    private readonly ApplicationDbContext _db;
    public ProductsController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index(string? search)
    {
        ViewData["Search"] = search;
        var query = _db.Products.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p => p.Name.Contains(search) || p.SKU.Contains(search) || (p.Category != null && p.Category.Contains(search)));
        return View(await query.OrderBy(p => p.Name).ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == id);
        return product == null ? NotFound() : View(product);
    }

    public IActionResult Create() => View(new Product());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("SKU,Name,Category,UnitPrice,StockQuantity,ReorderLevel,Description")] Product product)
    {
        if (await _db.Products.AnyAsync(p => p.SKU == product.SKU))
            ModelState.AddModelError(nameof(product.SKU), "This SKU already exists.");
        if (!ModelState.IsValid) return View(product);
        _db.Add(product);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Product created successfully.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var product = await _db.Products.FindAsync(id);
        return product == null ? NotFound() : View(product);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,SKU,Name,Category,UnitPrice,StockQuantity,ReorderLevel,Description")] Product product)
    {
        if (id != product.Id) return NotFound();
        if (await _db.Products.AnyAsync(p => p.SKU == product.SKU && p.Id != id))
            ModelState.AddModelError(nameof(product.SKU), "This SKU already exists.");
        if (!ModelState.IsValid) return View(product);
        try { _db.Update(product); await _db.SaveChangesAsync(); }
        catch (DbUpdateConcurrencyException) { if (!await _db.Products.AnyAsync(p => p.Id == id)) return NotFound(); throw; }
        TempData["Success"] = "Product updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == id);
        return product == null ? NotFound() : View(product);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null) return NotFound();
        if (await _db.SalesOrderItems.AnyAsync(i => i.ProductId == id))
        {
            TempData["Error"] = "This product is used in an order and cannot be deleted. Set its stock to zero instead.";
            return RedirectToAction(nameof(Index));
        }
        _db.Products.Remove(product);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Product deleted.";
        return RedirectToAction(nameof(Index));
    }
}
