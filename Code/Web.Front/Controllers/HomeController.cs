using Microsoft.AspNetCore.Mvc;

namespace Web.Front.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}