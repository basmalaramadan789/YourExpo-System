using YourExpo.Models;

namespace YourExpo.ViewModels;

public class CheckoutViewModel
{
    public List<ShoppingCart> CartItems { get; set; } 
    public decimal TotalAmount { get; set; } 
    public string ShippingAddress { get; set; } 
    public string PhoneNumber { get; set; } 
}
