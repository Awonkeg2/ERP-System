using System.ComponentModel.DataAnnotations;

namespace ByteBridgeERP.Models;

public class Supplier
{
    public int Id { get; set; }

    [Required, StringLength(120)]
    [Display(Name = "Supplier / business name")]
    public string Name { get; set; } = string.Empty;

    [StringLength(120), EmailAddress]
    public string? Email { get; set; }

    [StringLength(40)]
    public string? Phone { get; set; }

    [StringLength(120)]
    public string? Company { get; set; }

    [StringLength(300)]
    public string? Address { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }
}
