using YourExpo.Models;

namespace YourExpo.ViewModels;

public class ShoppingCartViewModel
{
    public List<ShoppingCart> Items { get; set; } 
    public decimal TotalAmount { get; set; } 
}
