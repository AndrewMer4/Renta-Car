using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaTienda.AccesoDatos.Data.Repository.iRepository;
using SistemaTienda.Data;
using SistemaTienda.Models;
using SistemaTienda.Models.ViewModels;


namespace SistemaTienda.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")] // Solo usuarios con rol "Admin" pueden acceder
    [Area("Admin")]
    public class PagosController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;
        private readonly ApplicationDbContext _context;

        public PagosController(IContenedorTrabajo contenedorTrabajo, ApplicationDbContext context)
        {
            _contenedorTrabajo = contenedorTrabajo;
            _context = context;
        }

        // Vista principal
        public IActionResult Index()
        {
            return View();
        }

        // API: Obtener todos los pagos
        [HttpGet]
        public IActionResult GetAll()
        {
            var pagos = _context.Pago
                .Include(p => p.Renta)
                    .ThenInclude(r => r.Cliente)
                .Include(p => p.Renta.Vehiculo)
                .ToList();

            return Json(new { data = pagos });
        }


        // GET: Crear nuevo pago
        public IActionResult Create()
        {
            var pagoVM = new PagoVM
            {
                Pago = new Pago(),
                ListaRentas = _context.Renta
                    .Include(r => r.Cliente)
                    .Include(r => r.Vehiculo)
                    .Select(r => new SelectListItem
                    {
                        Text = $"{r.Cliente.Nombres} {r.Cliente.Apellidos} - {r.Vehiculo.Marca} {r.Vehiculo.Modelo} ({r.FechaInicio:dd/MM/yyyy} a {r.FechaFin:dd/MM/yyyy})",
                        Value = r.Id.ToString()
                    })
            };

            return View(pagoVM);
        }

        // POST: Crear nuevo pago
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PagoVM pagoVM)
        {
            if (ModelState.IsValid)
            {
                var renta = _contenedorTrabajo.Renta.Get(pagoVM.Pago.RentaId);
                pagoVM.Pago.Monto = renta.Total;
                pagoVM.Pago.FechaPago = DateTime.Now;
                pagoVM.Pago.Estado = "Pendiente";

                _contenedorTrabajo.Pago.Add(pagoVM.Pago);
                _contenedorTrabajo.Save();

                return RedirectToAction(nameof(Index));
            }

            pagoVM.ListaRentas = _context.Renta
                .Include(r => r.Cliente)
                .Include(r => r.Vehiculo)
                .Select(r => new SelectListItem
                {
                    Text = $"{r.Cliente.Nombres} {r.Cliente.Apellidos} - {r.Vehiculo.Marca} {r.Vehiculo.Modelo}",
                    Value = r.Id.ToString()
                });

            return View(pagoVM);
        }

        // GET: Editar pago
        public IActionResult Edit(int id)
        {
            var pago = _contenedorTrabajo.Pago.Get(id);
            if (pago == null) return NotFound();

            var pagoVM = new PagoVM
            {
                Pago = pago,
                ListaRentas = _context.Renta
                    .Include(r => r.Cliente)
                    .Include(r => r.Vehiculo)
                    .Select(r => new SelectListItem
                    {
                        Text = $"{r.Cliente.Nombres} {r.Cliente.Apellidos} - {r.Vehiculo.Marca} {r.Vehiculo.Modelo}",
                        Value = r.Id.ToString()
                    })
            };

            return View(pagoVM);
        }

        // POST: Editar pago
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(PagoVM pagoVM)
        {
            if (ModelState.IsValid)
            {
                _contenedorTrabajo.Pago.Update(pagoVM.Pago);
                _contenedorTrabajo.Save();

                return RedirectToAction(nameof(Index));
            }

            pagoVM.ListaRentas = _context.Renta
                .Include(r => r.Cliente)
                .Include(r => r.Vehiculo)
                .Select(r => new SelectListItem
                {
                    Text = $"{r.Cliente.Nombres} {r.Cliente.Apellidos} - {r.Vehiculo.Marca} {r.Vehiculo.Modelo}",
                    Value = r.Id.ToString()
                });

            return View(pagoVM);
        }

        // DELETE: Eliminar pago
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var pago = _contenedorTrabajo.Pago.Get(id);
            if (pago == null)
            {
                return Json(new { success = false, message = "Error eliminando el pago" });
            }

            _contenedorTrabajo.Pago.Remove(pago);
            _contenedorTrabajo.Save();

            return Json(new { success = true, message = "Pago eliminado correctamente" });
        }

        // GET: Recibo para impresión
        public IActionResult Recibo(int id)
        {
            var pago = _context.Pago
                .Include(p => p.Renta)
                .ThenInclude(r => r.Cliente)
                .Include(p => p.Renta.Vehiculo)
                .FirstOrDefault(p => p.Id == id);

            if (pago == null)
            {
                return NotFound();
            }

            return View(pago); // Recibo.cshtml
        }

    }
}
