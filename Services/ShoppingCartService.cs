using Microsoft.EntityFrameworkCore;
using YourExpo.Models;
using YourExpo.Persistence;

namespace YourExpo.Services;

public class ShoppingCartService:IShoppingCartService
{
    private readonly ApplicationDbContext _context;

    public ShoppingCartService(ApplicationDbContext context)
    {
        _context = context;
    }

    public ShoppingCart GetCartByUser(string userId)
    {
        return _context.ShoppingCarts
            .Include(sc => sc.Product)
            .FirstOrDefault(sc => sc.ApplicationUserId == userId);
    }

    public void AddToCart(int productId, int count, string userId)
    {
        var cart = _context.ShoppingCarts
            .FirstOrDefault(sc => sc.ProductId == productId && sc.ApplicationUserId == userId);

        if (cart != null)
        {
            cart.Count += count;
        }
        else
        {
            _context.ShoppingCarts.Add(new ShoppingCart
            {
                ProductId = productId,
                Count = count,
                ApplicationUserId = userId
            });
        }

        _context.SaveChanges();
    }

    public void RemoveFromCart(int productId, string userId)
    {
        var cart = _context.ShoppingCarts
            .FirstOrDefault(sc => sc.ProductId == productId && sc.ApplicationUserId == userId);

        if (cart != null)
        {
            _context.ShoppingCarts.Remove(cart);
            _context.SaveChanges();
        }
    }

    public void ClearCart(string userId)
    {
        var cartItems = _context.ShoppingCarts
            .Where(sc => sc.ApplicationUserId == userId)
            .ToList();

        _context.ShoppingCarts.RemoveRange(cartItems);
        _context.SaveChanges();
    }
}
