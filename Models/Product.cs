using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YourExpo.Models;

public class Product
{
        public int Id { get; set; }

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

    
        public string ImageUrl { get; set; } = "";

   
        public int CategoryId { get; set; }
    
        public Category? Category { get; set; } 

        public int SupplierId { get; set; } 
        public Supplier? Supplier { get; set; }
    
    public ICollection<ProductTargetCountry> ProductTargetCountries { get; set; }
    [NotMapped]
    public int[] TargetCountryIds { get; set; }

}

