using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;
using YourExpo.Helper;
using YourExpo.Models;
using YourExpo.Persistence;
using YourExpo.ViewModels;

namespace YourExpo.Controllers;
public class OrderController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly StripeSettings _stripeSettings;
    public OrderController(ApplicationDbContext context, IOptions<StripeSettings> stripeSettings)
    {
        _context = context;
        _stripeSettings = stripeSettings.Value;
    }

    [HttpGet]
    [Authorize]
    
    public IActionResult Checkout()
    {
        // Get the current user's ID
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        
        var cartItems = _context.ShoppingCarts
            .Where(c => c.ApplicationUserId == userId)
            .Include(c => c.Product) 
            .ToList();

        
        if (!cartItems.Any())
        {
            TempData["Error"] = "Your cart is empty. Please add items before proceeding to checkout.";
            return RedirectToAction("Index", "ShoppingCart");
        }

        
        var totalAmount = cartItems.Sum(c => c.Product.Price * c.Count);

        
        var checkoutViewModel = new CheckoutViewModel
        {
            CartItems = cartItems,
            TotalAmount = totalAmount
        };

        
        return View(checkoutViewModel);
    }


    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public IActionResult PlaceOrder(PlaceOrderViewModel model)
    //{
    //    // Get the current user's ID
    //    string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

    //    // Get the cart items for the user
    //    var cartItems = _context.ShoppingCarts
    //        .Where(c => c.ApplicationUserId == userId)
    //        .Include(c => c.Product)
    //        .ToList();

    //    if (!cartItems.Any())
    //    {
    //        return RedirectToAction("Index", "ShoppingCart"); // If no items in the cart, redirect back to shopping cart
    //    }

    //    // Calculate the total amount for the order
    //    var totalAmount = cartItems.Sum(c => c.Product.Price * c.Count);

    //    // Create the order
    //    var order = new Order
    //    {
    //        ApplicationUserId = userId,
    //        ShippingAddress = model.ShippingAddress,
    //        PhoneNumber = model.PhoneNumber,
    //        TotalAmount = totalAmount,
    //        PaymentMethod = "Pending", // You can change this when integrating payment systems
    //        OrderDate = DateTime.Now,
    //        Status = OrderStatus.Pending, // Default status
    //        OrderItems = cartItems.Select(c => new OrderItem
    //        {
    //            ProductId = c.ProductId,
    //            ProductName = c.Product.Name,
    //            Price = c.Product.Price,
    //            Quantity = c.Count
    //        }).ToList()
    //    };

    //    // Save the order to the database
    //    _context.Orders.Add(order);
    //    _context.SaveChanges();

    //    // Clear the shopping cart
    //    _context.ShoppingCarts.RemoveRange(cartItems);
    //    _context.SaveChanges();

    //    // Redirect to the order confirmation page or a similar page
    //    return RedirectToAction("OrderConfirmation", new { orderId = order.Id });
    //}


    public IActionResult OrderConfirmation(int orderId)
    {
        var order = _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefault(o => o.Id == orderId);

        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }




    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult PlaceOrder(PlaceOrderViewModel model)
    {
        
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        
        var cartItems = _context.ShoppingCarts
            .Where(c => c.ApplicationUserId == userId)
            .Include(c => c.Product)
            .ToList();

        if (!cartItems.Any())
        {
            return RedirectToAction("Index", "ShoppingCart"); // If no items in the cart, redirect back to shopping cart
        }

        
        var totalAmount = cartItems.Sum(c => c.Product.Price * c.Count);

        
        var order = new Order
        {
            ApplicationUserId = userId,
            ShippingAddress = model.ShippingAddress,
            PhoneNumber = model.PhoneNumber,
            TotalAmount = totalAmount,
            PaymentMethod = "Pending", 
            OrderDate = DateTime.Now,
            Status = OrderStatus.Pending, 
            OrderItems = cartItems.Select(c => new OrderItem
            {
                ProductId = c.ProductId,
                ProductName = c.Product.Name,
                Price = c.Product.Price,
                Quantity = c.Count
            }).ToList()
        };

        
        _context.Orders.Add(order);
        _context.SaveChanges();

        
        
        _context.SaveChanges();

        
        var paymentUrl = GetPaymentUrl(order); 
        return Redirect(paymentUrl); 
    }

    private string GetPaymentUrl(Order order)
    {
        
        StripeConfiguration.ApiKey = _stripeSettings.SecretKey; 

        var sessionOptions = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = order.OrderItems.Select(item => new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)(item.Price * 100), 
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = item.ProductName
                    }
                },
                Quantity = item.Quantity
            }).ToList(),
            Mode = "payment",
            SuccessUrl = Url.Action("PaymentSuccess", "Payment", new { orderId = order.Id }, protocol: Request.Scheme),
            CancelUrl = Url.Action("PaymentFailed", "Payment", new { orderId = order.Id }, protocol: Request.Scheme)
        };

        var service = new SessionService();
        Session session = service.Create(sessionOptions);

        return session.Url; 
    }

}
