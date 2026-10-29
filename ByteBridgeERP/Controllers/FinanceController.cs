using ByteBridgeERP.Data;
using ByteBridgeERP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ByteBridgeERP.Controllers;

public class FinanceController : Controller
{
    private readonly ApplicationDbContext _db;
    public FinanceController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var revenue = await _db.SalesOrders
            .Where(o => o.Status != "Cancelled")
            .SumAsync(o => (decimal?)o.Total) ?? 0m;
        var expenses = await _db.Expenses
            .Where(e => e.Status == "Paid")
            .SumAsync(e => (decimal?)e.Amount) ?? 0m;
        var vm = new FinanceDashboardViewModel
        {
            Revenue = revenue,
            Expenses = expenses,
            NetResult = revenue - expenses,
            OutstandingOrders = await _db.SalesOrders
                .Where(o => o.Status == "Pending" || o.Status == "Processing")
                .SumAsync(o => (decimal?)o.Total) ?? 0m,
            ExpenseCount = await _db.Expenses.CountAsync(),
            RecentExpenses = await _db.Expenses.OrderByDescending(e => e.ExpenseDate).ThenByDescending(e => e.Id).Take(6).ToListAsync(),
            Accounts = await _db.Accounts.Where(a => a.IsActive).OrderBy(a => a.Code).ToListAsync()
        };
        return View(vm);
    }

    public async Task<IActionResult> Expenses()
    {
        return View(await _db.Expenses.OrderByDescending(e => e.ExpenseDate).ThenByDescending(e => e.Id).ToListAsync());
    }

    public IActionResult CreateExpense() => View(new Expense());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateExpense([Bind("Category,Description,Amount,ExpenseDate,PaidTo,PaymentMethod,Status,Notes")] Expense expense)
    {
        if (!new[] { "Paid", "Unpaid" }.Contains(expense.Status))
            ModelState.AddModelError(nameof(expense.Status), "Choose Paid or Unpaid.");
        if (!new[] { "Cash", "Bank transfer", "Card", "Other" }.Contains(expense.PaymentMethod))
            ModelState.AddModelError(nameof(expense.PaymentMethod), "Choose a valid payment method.");
        if (!ModelState.IsValid) return View(expense);

        expense.Reference = $"EXP-{DateTime.UtcNow:yyyyMMddHHmmss}-{Random.Shared.Next(100,999)}";
        _db.Expenses.Add(expense);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Expense recorded.";
        return RedirectToAction(nameof(Expenses));
    }

    public async Task<IActionResult> DeleteExpense(int? id)
    {
        if (id == null) return NotFound();
        var expense = await _db.Expenses.FindAsync(id);
        return expense == null ? NotFound() : View(expense);
    }

    [HttpPost, ActionName("DeleteExpense"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteExpenseConfirmed(int id)
    {
        var expense = await _db.Expenses.FindAsync(id);
        if (expense == null) return NotFound();
        _db.Expenses.Remove(expense);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Expense removed.";
        return RedirectToAction(nameof(Expenses));
    }

    public async Task<IActionResult> Accounts()
    {
        return View(await _db.Accounts.OrderBy(a => a.Code).ToListAsync());
    }

    public IActionResult CreateAccount() => View(new Account());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAccount([Bind("Code,Name,Type,Description,OpeningBalance,IsActive")] Account account)
    {
        if (!new[] { "Asset", "Liability", "Equity", "Income", "Expense" }.Contains(account.Type))
            ModelState.AddModelError(nameof(account.Type), "Choose a valid account type.");
        if (await _db.Accounts.AnyAsync(a => a.Code == account.Code))
            ModelState.AddModelError(nameof(account.Code), "This account code already exists.");
        if (!ModelState.IsValid) return View(account);
        _db.Accounts.Add(account);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Account added to chart of accounts.";
        return RedirectToAction(nameof(Accounts));
    }
}
