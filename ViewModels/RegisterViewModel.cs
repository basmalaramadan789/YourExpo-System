﻿using System.ComponentModel.DataAnnotations;

namespace YourExpo.ViewModels;

public class RegisterViewModel
{

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string FullName { get; set; } = string.Empty;

    public string Role { get; set; } = "Customer"; 

    // Supplier information
    public string SupplierName { get; set; } = string.Empty;
    public string ContactInfo { get; set; } = string.Empty;
}


