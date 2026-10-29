using System.ComponentModel.DataAnnotations;

namespace ByteBridgeERP.Models;

public class Customer
{
    public int Id { get; set; }

    [Required, StringLength(120)]
    [Display(Name = "Customer / business name")]
    public string Name { get; set; } = string.Empty;

    [StringLength(120), EmailAddress]
    public string? Email { get; set; }

    [StringLength(40)]
    public string? Phone { get; set; }

    [StringLength(120)]
    public string? Company { get; set; }

    [StringLength(300)]
    public string? Address { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
