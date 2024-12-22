using System.ComponentModel.DataAnnotations;

namespace YourExpo.Models;

public class TargetCountry
{
    public int Id { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "Country name can't exceed 100 characters.")]
    public string Name { get; set; } = "";

    [StringLength(500, ErrorMessage = "Description can't exceed 500 characters.")]
    public string Description { get; set; } = "";

    public ICollection<ProductTargetCountry> ProductTargetCountries { get; set; } = new List<ProductTargetCountry>();


   
}

