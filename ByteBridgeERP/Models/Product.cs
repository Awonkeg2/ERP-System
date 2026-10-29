using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ByteBridgeERP.Models;

public class Product
{
    public int Id { get; set; }

    [Required, StringLength(40)]
    [Display(Name = "SKU / Code")]
    public string SKU { get; set; } = string.Empty;

    [Required, StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [StringLength(80)]
    public string? Category { get; set; }

    [Range(0, 999999999)]
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Unit price (R)")]
    public decimal UnitPrice { get; set; }

    [Range(0, 100000000)]
    [Display(Name = "Stock quantity")]
    public int StockQuantity { get; set; }

    [Range(0, 100000000)]
    [Display(Name = "Reorder level")]
    public int ReorderLevel { get; set; } = 5;

    [StringLength(1000)]
    public string? Description { get; set; }

    [NotMapped]
    public bool IsLowStock => StockQuantity <= ReorderLevel;
}
