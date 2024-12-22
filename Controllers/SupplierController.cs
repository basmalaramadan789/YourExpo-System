using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace YourExpo.Controllers;

[Authorize(Roles = "Supplier")]
public class SupplierController : Controller
{
    public IActionResult Index()
    {
        return View();
    }





}
