using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using YourExpo.Models;
using YourExpo.Persistence;

namespace YourExpo.Controllers;
[Authorize(Roles ="Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AdminController(ApplicationDbContext context, SignInManager<ApplicationUser> signInManager)
    {
        _context = context;
        _signInManager = signInManager;
    }

    public IActionResult Index()
    {
        var totalProducts = _context.Products.Count();

        
        var totalOrders = _context.Orders.Count();

      
        var topSupplier = _context.Suppliers
                                  .Include(s => s.Products) 
                                  .OrderByDescending(s => s.Products.Count)
                                  .FirstOrDefault();

        
        var topSupplierProductCount = topSupplier?.Products.Count ?? 0;

     
        ViewBag.TotalProducts = totalProducts;
        ViewBag.TotalOrders = totalOrders;
        ViewBag.TopSupplier = topSupplier?.SupplierName ?? "No Supplier";
        ViewBag.TopSupplierProductCount = topSupplierProductCount;


        var topSuppliers = _context.Suppliers
       .Select(s => new
       {
           SupplierName = s.SupplierName,
           ProductCount = s.Products.Count
       })
       .OrderByDescending(s => s.ProductCount)
       .Take(5) 
       .ToList();

       
        ViewBag.SupplierNames = topSuppliers.Select(s => s.SupplierName).ToList();
        ViewBag.ProductCounts = topSuppliers.Select(s => s.ProductCount).ToList();

        return View();
    }


    [HttpGet]
    public async Task<IActionResult> GetAllProducts()
    {
        var products = await _context.Products
            .Include(p => p.Supplier)
            .Select(p => new
            {
                ProductId = p.Id,
                ProductName = p.Name,
                Description = p.Description,
                Quantity = p.Quantity,
                Price = p.Price,
                Specifications = p.Specifications,
                ImageUrl = p.ImageUrl,
                CategoryName = p.Category != null ? p.Category.Name : "No Category",
                SupplierName = p.Supplier != null ? p.Supplier.SupplierName : "No Supplier"
            })
            .ToListAsync();

        return View(products);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllOrders()
    {
        var orders = await _context.Orders
            .Include(o => o.ApplicationUser) // Include the user who placed the order
            .Include(o => o.OrderItems)     // Include the order items
            .ThenInclude(oi => oi.Product)  // Include the product details for each order item
            .Select(o => new
            {
                OrderId = o.Id,
                UserName = o.ApplicationUser.FullName ?? o.ApplicationUser.Email,
                ShippingAddress = o.ShippingAddress,
                PhoneNumber = o.PhoneNumber,
                TotalAmount = o.TotalAmount,
                PaymentMethod = o.PaymentMethod,
                OrderDate = o.OrderDate,
                Status = o.Status.ToString(),
                Items = o.OrderItems.Select(oi => new
                {
                    ProductName = oi.ProductName,
                    Price = oi.Price,
                    Quantity = oi.Quantity
                }).ToList()
            })
            .ToListAsync();

        return View(orders);
    }


    public IActionResult GetAllSuppliers()
    {
        var suppliers = _context.Suppliers.ToList();  
        return View(suppliers);  
    }

    



    [HttpGet]
    public IActionResult Approve(int id)
    {
        var supplier = _context.Suppliers.FirstOrDefault(s => s.Id == id);
        if (supplier != null)
        {
            
            supplier.IsApproved = true;
            _context.SaveChanges();  
        }

        return RedirectToAction("GetAllSuppliers");  
    }

}
