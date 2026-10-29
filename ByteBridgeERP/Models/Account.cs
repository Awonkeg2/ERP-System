using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ByteBridgeERP.Models;

/// <summary>
/// Starter chart-of-accounts record. This does not itself implement double-entry posting.
/// </summary>
public class Account
{
    public int Id { get; set; }

    [Required, StringLength(20)]
    [Display(Name = "Account code")]
    public string Code { get; set; } = string.Empty;

    [Required, StringLength(120)]
    [Display(Name = "Account name")]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(30)]
    [Display(Name = "Account type")]
    public string Type { get; set; } = "Expense";

    [StringLength(300)]
    public string? Description { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Opening balance (R)")]
    public decimal OpeningBalance { get; set; }

    public bool IsActive { get; set; } = true;
}
