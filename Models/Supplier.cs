namespace YourExpo.Models;

public class Supplier
{
    public int Id { get; set; }
    public string SupplierName { get; set; }= string.Empty;
    public string ContactInfo { get; set; } = string.Empty;


    public List<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();

   
    public List<Product> Products { get; set; } = new List<Product>();
    public bool IsApproved {  get; set; }


  
}
