using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using YourExpo.Models;
using YourExpo.Persistence;
using YourExpo.Services;
using YourExpo.ViewModels;

namespace YourExpo.Controllers;
[Authorize]
public class ShoppingCartController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ShoppingCartController> _logger;
    private readonly IShoppingCartService _cartService;
    private readonly IOrderService _orderService;

    public ShoppingCartController(ApplicationDbContext context, ILogger<ShoppingCartController> logger, IShoppingCartService cartService, IOrderService orderService)
    {
        _context = context;
        _logger = logger;
        _cartService = cartService;
        _orderService = orderService;
    }

    public IActionResult Index()
    {
        
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var cartItems = _context.ShoppingCarts
            .Where(c => c.ApplicationUserId == userId)
            .Include(c => c.Product)
            .ToList();

        return View(cartItems);
    }

    [HttpGet]
    public IActionResult AddToCart(int productId)
    {
       
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
        {
            return RedirectToAction("Login", "Account"); // Redirect to login if the user isn't authenticated
        }

        
        var cartItem = _context.ShoppingCarts
            .FirstOrDefault(c => c.ProductId == productId && c.ApplicationUserId == userId);

        if (cartItem != null)
        {
            
            cartItem.Count++;
        }
        else
        {
            
            cartItem = new ShoppingCart
            {
                ProductId = productId,
                ApplicationUserId = userId,
                Count = 1
            };
            _context.ShoppingCarts.Add(cartItem);
        }

        _context.SaveChanges(); 
        return RedirectToAction("Index", "ShoppingCart"); 
    }




    [HttpPost]
    public IActionResult UpdateQuantity(int productId, int newQuantity)
    {
       
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        
        var cartItem = _context.ShoppingCarts
            .FirstOrDefault(c => c.ProductId == productId && c.ApplicationUserId == userId);

        if (cartItem == null)
        {
            return NotFound(); 
        }

       
        cartItem.Count = newQuantity;

        
        if (cartItem.Count < 1)
        {
            cartItem.Count = 1;
        }

        _context.SaveChanges(); 

        
        var cartItems = _context.ShoppingCarts
            .Where(c => c.ApplicationUserId == userId)
            .Include(c => c.Product)
            .ToList();

        var totalItems = cartItems.Sum(c => c.Count);
        var totalPrice = cartItems.Sum(c => c.Product.Price * c.Count);

        
        return Json(new
        {
            totalItems,
            totalPrice
        });
    }



   




    [HttpPost]
    public IActionResult RemoveFromCart(int productId)
    {
        
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        
        var cartItem = _context.ShoppingCarts
            .FirstOrDefault(c => c.ProductId == productId && c.ApplicationUserId == userId);

        if (cartItem == null)
        {
            return NotFound(); 
        }

        
        _context.ShoppingCarts.Remove(cartItem);
        _context.SaveChanges();

        
        var cartItems = _context.ShoppingCarts
            .Where(c => c.ApplicationUserId == userId)
            .Include(c => c.Product)
            .ToList();

        var totalItems = cartItems.Sum(c => c.Count);
        var totalPrice = cartItems.Sum(c => c.Product.Price * c.Count);

        

        return RedirectToAction("Index", "ShoppingCart");
    }




    
    

}
