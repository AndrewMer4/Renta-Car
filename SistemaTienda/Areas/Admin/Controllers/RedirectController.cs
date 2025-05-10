using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Renta_Car.Areas.Admin.Controllers
{
    public class RedirectController : Controller
    {
        public IActionResult ToAdmin()
        {
            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }
    }
}