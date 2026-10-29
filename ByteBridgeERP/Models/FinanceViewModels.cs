namespace ByteBridgeERP.Models;

public class FinanceDashboardViewModel
{
    public decimal Revenue { get; set; }
    public decimal Expenses { get; set; }
    public decimal NetResult { get; set; }
    public decimal OutstandingOrders { get; set; }
    public int ExpenseCount { get; set; }
    public List<Expense> RecentExpenses { get; set; } = new();
    public List<Account> Accounts { get; set; } = new();
}
