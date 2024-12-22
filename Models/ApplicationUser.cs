using Microsoft.AspNetCore.Identity;

namespace YourExpo.Models;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public int? SupplierId { get; set; }
    public Supplier Supplier { get; set; }
}
