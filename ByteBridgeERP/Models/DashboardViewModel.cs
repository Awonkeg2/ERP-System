namespace ByteBridgeERP.Models;

public class DashboardViewModel
{
    public int ProductCount { get; set; }
    public int CustomerCount { get; set; }
    public int SupplierCount { get; set; }
    public int PendingOrders { get; set; }
    public decimal InventoryValue { get; set; }
    public List<Product> LowStockProducts { get; set; } = new();
    public List<SalesOrder> RecentOrders { get; set; } = new();
}

public class ErrorViewModel
{
    public string? RequestId { get; set; }
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
