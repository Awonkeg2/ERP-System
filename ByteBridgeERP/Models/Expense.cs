using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ByteBridgeERP.Models;

public class Expense
{
    public int Id { get; set; }

    [Required, StringLength(40)]
    [Display(Name = "Expense reference")]
    public string Reference { get; set; } = $"EXP-{DateTime.UtcNow:yyyyMMddHHmmss}";

    [Required, StringLength(120)]
    [Display(Name = "Expense category")]
    public string Category { get; set; } = string.Empty;

    [Required, StringLength(200)]
    public string Description { get; set; } = string.Empty;

    [Range(0.01, 999999999)]
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Amount (R)")]
    public decimal Amount { get; set; }

    [Required, DataType(DataType.Date)]
    [Display(Name = "Expense date")]
    public DateTime ExpenseDate { get; set; } = DateTime.Today;

    [StringLength(120)]
    [Display(Name = "Paid to")]
    public string? PaidTo { get; set; }

    [Required, StringLength(30)]
    [Display(Name = "Payment method")]
    public string PaymentMethod { get; set; } = "Bank transfer";

    [Required, StringLength(30)]
    public string Status { get; set; } = "Paid";

    [StringLength(500)]
    public string? Notes { get; set; }
}
