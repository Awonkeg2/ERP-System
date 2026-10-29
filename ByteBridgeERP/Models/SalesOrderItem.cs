using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ByteBridgeERP.Models;

public class SalesOrderItem
{
    public int Id { get; set; }

    public int SalesOrderId { get; set; }
    public SalesOrder? SalesOrder { get; set; }

    [Required]
    [Display(Name = "Product")]
    public int ProductId { get; set; }
    public Product? Product { get; set; }

    [Range(1, 1000000)]
    public int Quantity { get; set; } = 1;

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Unit price (R)")]
    public decimal UnitPrice { get; set; }

    [NotMapped]
    public decimal LineTotal => Quantity * UnitPrice;
}
