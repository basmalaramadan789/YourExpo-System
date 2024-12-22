using Microsoft.AspNetCore.Mvc;

namespace YourExpo.Controllers;
public class AdminController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
