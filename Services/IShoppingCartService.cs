using YourExpo.Models;

namespace YourExpo.Services;

public interface IShoppingCartService

{
    ShoppingCart GetCartByUser(string userId);
    void AddToCart(int productId, int count, string userId);
    void RemoveFromCart(int productId, string userId);
    void ClearCart(string userId);
}
