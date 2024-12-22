using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using YourExpo.Models;

namespace YourExpo.ViewModels;

public class ProductViewModel
{
    [Required]
    [StringLength(100, ErrorMessage = "Product name can't exceed 100 characters.")]
    public string Name { get; set; } = "";

    [StringLength(500, ErrorMessage = "Product description can't exceed 500 characters.")]
    public string Description { get; set; } = "";

    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than zero.")]
    public int Quantity { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
    public decimal Price { get; set; }

    [StringLength(500, ErrorMessage = "Specifications can't exceed 500 characters.")]
    public string Specifications { get; set; } = "";

    public IFormFile? ImageFile { get; set; }
   

    [Required]
    public int CategoryId { get; set; }

    [Required]
   // public int SupplierId { get; set; }

   // public List<int>? TargetCountryIds { get; set; }

   // public IEnumerable<SelectListItem>? Categories { get; set; }
    //public IEnumerable<SelectListItem>? Suppliers { get; set; }
    //public IEnumerable<SelectListItem>? TargetCountries { get; set; }
    public Product Product { get; set; }

    [ValidateNever]
    public IEnumerable<SelectListItem> CategoryList { get; set; }
}
