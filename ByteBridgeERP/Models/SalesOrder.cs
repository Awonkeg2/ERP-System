using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ByteBridgeERP.Models;

public class SalesOrder
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Order number")]
    public string OrderNumber { get; set; } = $"BB-{DateTime.UtcNow:yyyyMMddHHmmss}";

    [Required]
    [Display(Name = "Customer")]
    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }

    [Required, DataType(DataType.Date)]
    [Display(Name = "Order date")]
    public DateTime OrderDate { get; set; } = DateTime.Today;

    [Required, StringLength(30)]
    public string Status { get; set; } = "Pending";

    [Column(TypeName = "decimal(18,2)")]
    public decimal Total { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }

    public List<SalesOrderItem> Items { get; set; } = new();
}
