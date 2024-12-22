namespace YourExpo.Models;

public class ProductTargetCountry
{
    public int ProductId { get; set; }
    public Product? Product { get; set; }

    public int TargetCountryId { get; set; }
    public TargetCountry? TargetCountry { get; set; }
}
