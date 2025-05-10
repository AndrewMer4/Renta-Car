using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaTienda.AccesoDatos.Data.Repository.iRepository;
using SistemaTienda.Data;
using SistemaTienda.Models;
using SistemaTienda.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace SistemaTienda.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")] // Solo usuarios con rol "Admin" pueden acceder
    [Area("Admin")]
    public class RentasController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public RentasController(IContenedorTrabajo contenedorTrabajo,
                                ApplicationDbContext context,
                                IWebHostEnvironment hostingEnvironment)
        {
            _contenedorTrabajo = contenedorTrabajo;
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var rentas = _contenedorTrabajo.Renta.GetAll(includeProperties: "Cliente,Vehiculo");
            return Json(new { data = rentas });
        }

        [HttpGet]
        public IActionResult Create()
        {
            RentaVM rentaVM = new RentaVM
            {
                Renta = new Renta(),
                ListaClientes = _contenedorTrabajo.Cliente.GetAll().Select(c => new SelectListItem
                {
                    Text = c.Nombres + " " + c.Apellidos,
                    Value = c.Id.ToString()
                }),
                ListaVehiculos = _contenedorTrabajo.Vehiculo
                    .GetAll(v => v.Estado == "Disponible")
                    .Select(v => new SelectListItem
                    {
                        Text = $"{v.Marca} {v.Modelo} ({v.PrecioPorDia:C})",
                        Value = v.Id.ToString()
                    })
            };

            return View(rentaVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(RentaVM rentaVM)
        {
            var vehiculo = _contenedorTrabajo.Vehiculo.Get(rentaVM.Renta.VehiculoId);
            if (vehiculo == null)
            {
                ModelState.AddModelError("", "Vehículo no válido.");
                return View(rentaVM);
            }

            var dias = (rentaVM.Renta.FechaFin - rentaVM.Renta.FechaInicio).Days;
            rentaVM.Renta.Total = dias * vehiculo.PrecioPorDia;

            if (ModelState.IsValid)
            {
                _contenedorTrabajo.Renta.Add(rentaVM.Renta);
                vehiculo.Estado = "Rentado";
                _contenedorTrabajo.Vehiculo.Update(vehiculo);
                _contenedorTrabajo.Save();
                return RedirectToAction(nameof(Index));
            }

            rentaVM.ListaClientes = _contenedorTrabajo.Cliente.GetAll().Select(c => new SelectListItem
            {
                Text = c.Nombres + " " + c.Apellidos,
                Value = c.Id.ToString()
            });

            rentaVM.ListaVehiculos = _contenedorTrabajo.Vehiculo
                .GetAll(v => v.Estado == "Disponible")
                .Select(v => new SelectListItem
                {
                    Text = $"{v.Marca} {v.Modelo} ({v.PrecioPorDia:C})",
                    Value = v.Id.ToString()
                });

            return View(rentaVM);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var renta = _contenedorTrabajo.Renta.Get(id);
            if (renta == null)
            {
                return NotFound();
            }

            // Obtener el vehículo asociado a esta renta aunque no esté disponible
            var vehiculoRentado = _contenedorTrabajo.Vehiculo.Get(renta.VehiculoId);

            RentaVM rentaVM = new RentaVM
            {
                Renta = renta,
                ListaClientes = _contenedorTrabajo.Cliente.GetAll().Select(c => new SelectListItem
                {
                    Text = c.Nombres + " " + c.Apellidos,
                    Value = c.Id.ToString()
                }),
                ListaVehiculos = _contenedorTrabajo.Vehiculo
                    .GetAll() // Mostramos todos los vehículos, no solo los disponibles
                    .Select(v => new SelectListItem
                    {
                        Text = $"{v.Marca} {v.Modelo} ({v.PrecioPorDia:C}) - {v.Estado}",
                        Value = v.Id.ToString(),
                        Selected = v.Id == renta.VehiculoId
                    })
            };

            return View(rentaVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(RentaVM rentaVM)
        {
            // Validar que el vehículo existe
            var vehiculo = _contenedorTrabajo.Vehiculo.Get(rentaVM.Renta.VehiculoId);
            if (vehiculo == null)
            {
                ModelState.AddModelError("", "Vehículo no válido.");
                return View(rentaVM);
            }

            // Calcular el total basado en las fechas
            var dias = (rentaVM.Renta.FechaFin - rentaVM.Renta.FechaInicio).Days;
            rentaVM.Renta.Total = dias * vehiculo.PrecioPorDia;

            if (ModelState.IsValid)
            {
                // Obtener la renta original para verificar cambios en el vehículo
                var rentaOriginal = _contenedorTrabajo.Renta.Get(rentaVM.Renta.Id);

                // Si cambió el vehículo, actualizar estados
                if (rentaOriginal.VehiculoId != rentaVM.Renta.VehiculoId)
                {
                    // Liberar el vehículo anterior
                    var vehiculoAnterior = _contenedorTrabajo.Vehiculo.Get(rentaOriginal.VehiculoId);
                    if (vehiculoAnterior != null)
                    {
                        vehiculoAnterior.Estado = "Disponible";
                        _contenedorTrabajo.Vehiculo.Update(vehiculoAnterior);
                    }

                    // Marcar el nuevo vehículo como rentado
                    vehiculo.Estado = "Rentado";
                    _contenedorTrabajo.Vehiculo.Update(vehiculo);
                }

                // Actualizar la renta
                _contenedorTrabajo.Renta.Update(rentaVM.Renta);
                _contenedorTrabajo.Save();

                return RedirectToAction(nameof(Index));
            }

            // Si hay errores de validación, recargar las listas
            rentaVM.ListaClientes = _contenedorTrabajo.Cliente.GetAll().Select(c => new SelectListItem
            {
                Text = c.Nombres + " " + c.Apellidos,
                Value = c.Id.ToString()
            });

            rentaVM.ListaVehiculos = _contenedorTrabajo.Vehiculo
                .GetAll()
                .Select(v => new SelectListItem
                {
                    Text = $"{v.Marca} {v.Modelo} ({v.PrecioPorDia:C}) - {v.Estado}",
                    Value = v.Id.ToString()
                });

            return View(rentaVM);
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var renta = _contenedorTrabajo.Renta.Get(id);
            if (renta == null)
            {
                return Json(new { success = false, message = "Error eliminando renta" });
            }

            var vehiculo = _contenedorTrabajo.Vehiculo.Get(renta.VehiculoId);
            if (vehiculo != null)
            {
                vehiculo.Estado = "Disponible";
                _contenedorTrabajo.Vehiculo.Update(vehiculo);
            }

            _contenedorTrabajo.Renta.Remove(renta);
            _contenedorTrabajo.Save();
            return Json(new { success = true, message = "Renta eliminada correctamente" });
        }
    }
}