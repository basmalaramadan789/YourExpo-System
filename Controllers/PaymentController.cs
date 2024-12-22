using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe.Checkout;
using YourExpo.Helper;
using YourExpo.Models;
using YourExpo.Persistence;
using Stripe;

namespace YourExpo.Controllers;
public class PaymentController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly StripeSettings _stripeSettings;

    public PaymentController(ApplicationDbContext context, IOptions<StripeSettings> stripeSettings)
    {
        _context = context;
        _stripeSettings = stripeSettings.Value;
    }

    [HttpPost]
    public IActionResult ProcessPayment(int orderId)
    {
       
        var order = _context.Orders.Include(o => o.OrderItems).FirstOrDefault(o => o.Id == orderId);

        if (order == null)
        {
            return NotFound("Order not found");
        }

        
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

        
        return Redirect(session.Url);
    }

    public IActionResult PaymentSuccess(int orderId)
    {
        var order = _context.Orders.FirstOrDefault(o => o.Id == orderId);

        if (order == null)
        {
            return NotFound("Order not found");
        }

     
        order.Status = OrderStatus.Pending;
        _context.SaveChanges();

        return View("PaymentSuccess");
    }

    public IActionResult PaymentFailed(int orderId)
    {
        var order = _context.Orders.FirstOrDefault(o => o.Id == orderId);

        if (order == null)
        {
            return NotFound("Order not found");
        }

        
        order.Status = OrderStatus.Pending; 
        _context.SaveChanges();

        return View("PaymentFailed");
    }
}
