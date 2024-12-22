using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using YourExpo.Models;
using YourExpo.Persistence;
using YourExpo.ViewModels;

namespace YourExpo.Controllers;
public class ProductController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
   private readonly IWebHostEnvironment _webHostEnvironment;

    public ProductController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
    {
        _userManager = userManager;
        _context = context;
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<IActionResult> Index()
    {
        // Fetching products with their associated Category and ProductTargetCountries (via a join or navigation property)
        var products = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.ProductTargetCountries)
                .ThenInclude(ptc => ptc.TargetCountry)  // Include TargetCountries related to the Product
            .ToListAsync();

        return View(products);
    }


   




    [HttpGet]
    public async Task<IActionResult> Create()
    {
        
        ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");
        ViewBag.TargetCountries = new MultiSelectList(await _context.TargetCountries.ToListAsync(), "Id", "Name");


        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product model, IFormFile ImageFile, int[] TargetCountryIds)
    {
       
            var product = new Product
            {
                Name = model.Name,
                Description = model.Description,
                Quantity = model.Quantity,
                Price = model.Price,
                Specifications = model.Specifications,
                CategoryId = model.CategoryId, // Link to category
                SupplierId = 1,
                ImageUrl = model.ImageUrl// Assuming you have SupplierId from the logged-in user
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
            return RedirectToAction(nameof(Index)); // Adjust this if you want to go somewhere else

        
        // If validation failed, pass the categories again to the view
        var categories = _context.Categories.ToList();
        ViewBag.Categories = new SelectList(categories, "Id", "Name");


        ViewBag.TargetCountries = new MultiSelectList(await _context.TargetCountries.ToListAsync(), "Id", "Name", TargetCountryIds);


        return View(model); // Return the view with validation errors
    }







    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products
            .Include(p => p.ProductTargetCountries)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (product == null)
        {
            return NotFound();
        }

        return View(product); // This will show the delete confirmation view
    }

    // POST: Product/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var product = await _context.Products
            .Include(p => p.ProductTargetCountries)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (product == null)
        {
            return NotFound();
        }

        // Remove related ProductTargetCountries if necessary
        _context.ProductTargetCountries.RemoveRange(product.ProductTargetCountries);

        // Remove the product
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        // Redirect back to product index page after deletion
        return RedirectToAction(nameof(Index));
    }

}



