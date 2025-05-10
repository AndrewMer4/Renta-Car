using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaTienda.Models;
using System.Diagnostics;

namespace SistemaTienda.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")] // Solo usuarios con rol "Admin" pueden acceder
    [Area("Admin")]
    
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // Página principal del administrador
        public IActionResult Index()
        {
            return View(); // Asegúrate de tener Views/Admin/Home/Index.cshtml con LayoutAdmin
        }

        public IActionResult Privacy()
        {
            return View(); // Opcional
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
