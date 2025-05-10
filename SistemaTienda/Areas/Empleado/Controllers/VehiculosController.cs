using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using SistemaTienda.AccesoDatos.Data.Repository.iRepository;
using SistemaTienda.Data;
using SistemaTienda.Models;
using SistemaTienda.Models.ViewModels;
using System.Linq;
using Microsoft.AspNetCore.Authorization;


namespace SistemaTienda.Areas.Empleado.Controllers
{
    [Authorize(Roles = "Empleado")]
    [Area("Empleado")]
    public class VehiculosController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public VehiculosController(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }

        public IActionResult Index()
        {
            var vehiculos = _contenedorTrabajo.Vehiculo.GetAll();
            return View(vehiculos);
        }
    }
}
