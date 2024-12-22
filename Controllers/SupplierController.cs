using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
//using Stripe.Climate;
using System.Security.Claims;
using YourExpo.Models;
using YourExpo.Persistence;
using YourExpo.ViewModels;

namespace YourExpo.Controllers;

[Authorize(Roles = "Supplier")]
public class SupplierController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public SupplierController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
    {
        _userManager = userManager;
        _context = context;
        _webHostEnvironment = webHostEnvironment;
    }
    //dashboard
    public async Task< IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.Users
            .Include(u => u.Supplier)
            .FirstOrDefaultAsync(u => u.Id == userId);

        // Fetch orders for the current supplier
        var orders = await _context.Orders
            .Where(o => o.OrderItems.Any(oi => oi.Product.SupplierId == user.SupplierId))
            .ToListAsync();

        // Count orders by status
        var pendingOrdersCount = orders.Count(o => o.Status == OrderStatus.Pending);
        var shippedOrdersCount = orders.Count(o => o.Status == OrderStatus.Shipped);
        var deliveredOrdersCount = orders.Count(o => o.Status == OrderStatus.Delivered);

        // Fetch products for the current supplier
        var products = await _context.Products
            .Include(p => p.Category)
            .Where(p => p.SupplierId == user.SupplierId)
            .ToListAsync();

        // Group products by category and count them
        var productCategoryDistribution = products
            .GroupBy(p => p.Category.Name)
            .ToDictionary(g => g.Key, g => g.Count());

        // Create ViewModel with statistics
        var viewModel = new SupplierDashboardViewModel
        {
            PendingOrdersCount = pendingOrdersCount,
            ShippedOrdersCount = shippedOrdersCount,
            DeliveredOrdersCount = deliveredOrdersCount,
            ProductCategoryDistribution = productCategoryDistribution
        };

        return View(viewModel);

    }



    [HttpGet]
    public async Task<IActionResult> GetSupplierProduct(string categoryFilter, decimal? price)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.Users
            .Include(u => u.Supplier)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user?.SupplierId == null)
        {
            return Unauthorized();
        }

        IQueryable<Product> productsQuery = _context.Products
            .Where(p => p.SupplierId == user.SupplierId)
            .Include(p => p.Category)
            .Include(p => p.ProductTargetCountries)
                .ThenInclude(ptc => ptc.TargetCountry);

        // Filter by category if provided
        if (!string.IsNullOrEmpty(categoryFilter))
        {
            productsQuery = productsQuery.Where(p => EF.Functions.Like(p.Category.Name, $"%{categoryFilter}%"));
        }

        // Filter by exact price if provided
        if (price.HasValue)
        {
            productsQuery = productsQuery.Where(p => p.Price == price.Value);
        }

        List<Product> products = await productsQuery.ToListAsync();

        // Map products to view model
        List<filterViewModel> viewModel = products.Select(p => new filterViewModel
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            Quantity = p.Quantity,
            Description = p.Description,
            CategoryName = p.Category?.Name,
            TargetCountries = p.ProductTargetCountries.Select(ptc => ptc.TargetCountry?.Name).ToList()
        }).ToList();

        // Pass filter values back to the view
        ViewData["CategoryFilter"] = categoryFilter;
        ViewData["Price"] = price;

        return View(viewModel);
    }







    //public async Task<IActionResult> GetSupplierProduct()
    //{
    //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Fetches the logged-in user's unique ID
    //    var user = await _userManager.Users
    //        .Include(u => u.Supplier)
    //        .FirstOrDefaultAsync(u => u.Id == userId);

    //    if (user?.SupplierId == null)
    //    {
    //        return Unauthorized(); // Ensure only suppliers can access their products
    //    }

    //    // Fetching products for the authenticated supplier
    //    var products = await _context.Products
    //        .Where(p => p.SupplierId == user.SupplierId) // Filter products by SupplierId
    //        .Include(p => p.Category)
    //        .Include(p => p.ProductTargetCountries)
    //            .ThenInclude(ptc => ptc.TargetCountry)  // Include TargetCountries related to the Product
    //        .ToListAsync();

    //    return View(products);
    //}

    [HttpGet]
    public async Task<IActionResult> CreateProduct()
    {

        ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");
        ViewBag.TargetCountries = new MultiSelectList(await _context.TargetCountries.ToListAsync(), "Id", "Name");


        return View();
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateProduct(Product model, IFormFile ImageFile, int[] TargetCountryIds)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the logged-in user's ID
        var user = await _userManager.Users
            .Include(u => u.Supplier)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user?.SupplierId == null)
        {
            ModelState.AddModelError(string.Empty, "You are not associated with any supplier.");
            return View(model);
        }

        var product = new Product
        {
            Name = model.Name,
            Description = model.Description,
            Quantity = model.Quantity,
            Price = model.Price,
            Specifications = model.Specifications,
            CategoryId = model.CategoryId, 
            SupplierId = user.SupplierId.Value,
            ImageUrl = model.ImageUrl
        };

        if (ImageFile != null && ImageFile.Length > 0)
        {
            string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products");

            // Ensure the directory exists
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            // Generate a unique file name
            string fileName = Guid.NewGuid().ToString();
            string extension = Path.GetExtension(ImageFile.FileName);
            string filePath = Path.Combine(uploadPath, fileName + extension);

            // Save the file
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await ImageFile.CopyToAsync(fileStream);
            }

            // Set the ImageUrl property
            product.ImageUrl = Path.Combine("images", "products", fileName + extension).Replace("\\", "/");
        }
        else
        {
            // Handle the case where no image is selected (optional)
            ModelState.AddModelError("ImageFile", "No image file selected.");
            return View(model);
        }


        if (TargetCountryIds != null && TargetCountryIds.Length > 0)
        {
            product.ProductTargetCountries = TargetCountryIds.Select(tcId => new ProductTargetCountry
            {
                TargetCountryId = tcId
            }).ToList();
        }



        // Add the product to the database
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        // Redirect to the product list or another page
        return RedirectToAction(nameof(GetSupplierProduct)); // Adjust this if you want to go somewhere else


        // If validation failed, pass the categories again to the view
        var categories = _context.Categories.ToList();
        ViewBag.Categories = new SelectList(categories, "Id", "Name");


        ViewBag.TargetCountries = new MultiSelectList(await _context.TargetCountries.ToListAsync(), "Id", "Name", TargetCountryIds);


        return View(model); // Return the view with validation errors
    }





    public async Task<IActionResult> SupplierOrders()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.Users
            .Include(u => u.Supplier)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user?.SupplierId == null)
        {
            return Unauthorized(); // Ensure the user is a supplier
        }

        // Fetch orders with their items and products belonging to the supplier
        var orders = await _context.Orders
            .Include(o => o.ApplicationUser) // Include the user who placed the order
            .Include(o => o.OrderItems) // Include OrderItems
            .ThenInclude(oi => oi.Product) // Include the associated Product
            .Where(o => o.OrderItems.Any(oi => oi.Product.SupplierId == user.SupplierId)) // Filter by SupplierId
            .ToListAsync();

        return View(orders);
    }



    public async Task<IActionResult> Profile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.Users
            .Include(u => u.Supplier)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user?.Supplier == null)
        {
            return NotFound("Supplier not found");
        }

        var supplier = user.Supplier;

        // Create a ViewModel or pass data to the view
        var profileViewModel = new SupplierProfileViewModel
        {
            SupplierName = supplier.SupplierName,
            ContactInfo = supplier.ContactInfo,
            IsApproved = supplier.IsApproved
        };

        return View(profileViewModel); // Return the Profile View with the data
    }





    [HttpGet]
    public async Task<IActionResult> EditProduct(int id)
    {
        // Fetch the product from the database
        var product = await _context.Products
            .Include(p => p.ProductTargetCountries)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
        {
            return NotFound(); // Return a 404 error if the product is not found
        }

        // Fetch categories and target countries for the dropdowns
        ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
        ViewBag.TargetCountries = new MultiSelectList(await _context.TargetCountries.ToListAsync(), "Id", "Name", product.ProductTargetCountries.Select(ptc => ptc.TargetCountryId).ToArray());

        // Pass the product data to the view
        return View(product);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProduct(int id, Product model, IFormFile ImageFile, int[] TargetCountryIds)
    {
        // Fetch the product from the database
        var product = await _context.Products
            .Include(p => p.ProductTargetCountries)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
        {
            return NotFound(); // Return a 404 error if the product is not found
        }

        // Update the product properties
        product.Name = model.Name;
        product.Description = model.Description;
        product.Quantity = model.Quantity;
        product.Price = model.Price;
        product.Specifications = model.Specifications;
        product.CategoryId = model.CategoryId;

        // Handle image upload
        if (ImageFile != null && ImageFile.Length > 0)
        {
            string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products");

            // Ensure the directory exists
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            // Generate a unique file name
            string fileName = Guid.NewGuid().ToString();
            string extension = Path.GetExtension(ImageFile.FileName);
            string filePath = Path.Combine(uploadPath, fileName + extension);

            // Save the file
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await ImageFile.CopyToAsync(fileStream);
            }

            // Update the ImageUrl property
            product.ImageUrl = Path.Combine("images", "products", fileName + extension).Replace("\\", "/");
        }

        // Update target countries
        if (TargetCountryIds != null && TargetCountryIds.Length > 0)
        {
            // Remove existing target countries
            _context.ProductTargetCountries.RemoveRange(product.ProductTargetCountries);

            // Add new target countries
            product.ProductTargetCountries = TargetCountryIds.Select(tcId => new ProductTargetCountry
            {
                TargetCountryId = tcId
            }).ToList();
        }

        // Save changes to the database
        _context.Products.Update(product);
        await _context.SaveChangesAsync();

        // Redirect to the product list or another page
        return RedirectToAction(nameof(GetSupplierProduct)); // Adjust this if you want to go somewhere else
    }




    [HttpGet]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        // Fetch the product from the database
        var product = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.ProductTargetCountries)
            .ThenInclude(ptc => ptc.TargetCountry)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
        {
            return NotFound(); // Return a 404 error if the product is not found
        }

        // Pass the product data to the view
        return View(product);
    }



    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteProduct(int id, IFormCollection collection)
    {
        // Fetch the product from the database
        var product = await _context.Products
            .Include(p => p.ProductTargetCountries)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
        {
            return NotFound(); // Return a 404 error if the product is not found
        }

        // Remove the product target countries
        _context.ProductTargetCountries.RemoveRange(product.ProductTargetCountries);

        // Remove the product
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        // Redirect to the product list or another page
        return RedirectToAction(nameof(GetSupplierProduct)); // Adjust this if you want to go somewhere else
    }



    [HttpGet]
    public IActionResult ApprovedSupplier()
    {
        return View(); 
    }
}
