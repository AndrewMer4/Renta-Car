using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaTienda.AccesoDatos.Data.Repository.iRepository;
using SistemaTienda.Data;
using SistemaTienda.Models;
using SistemaTienda.Models.ViewModels;
using SistemaTienda.Utilidades;
using System;
using System.Linq;

namespace SistemaTienda.Areas.Empleado.Controllers
{
    [Authorize(Roles = CNT.Empleado)]
    [Area("Empleado")]
    public class EmpleadoPagosController : Controller
    {
        private readonly IContenedorTrabajo _contenedor;
        private readonly ApplicationDbContext _context;

        public EmpleadoPagosController(
            IContenedorTrabajo contenedor,
            ApplicationDbContext context)
        {
            _contenedor = contenedor;
            _context = context;
        }

        // GET: /Empleado/EmpleadoPagos
        [HttpGet]
        public IActionResult Index() => View();

        // GET: /Empleado/EmpleadoPagos/GetAll
        [HttpGet]
        public IActionResult GetAll()
        {
            var data = _context.Pago
                .Include(p => p.Renta).ThenInclude(r => r.Cliente)
                .Include(p => p.Renta).ThenInclude(r => r.Vehiculo)
                .Select(p => new {
                    id = p.Id,
                    cliente = p.Renta.Cliente.Nombres + " " + p.Renta.Cliente.Apellidos,
                    vehiculo = p.Renta.Vehiculo.Marca + " " + p.Renta.Vehiculo.Modelo,
                    monto = p.Monto,
                    fechapago = p.FechaPago.ToString("yyyy-MM-dd HH:mm"),
                    estado = p.Estado
                });
            return Json(new { data });
        }

        // GET: /Empleado/EmpleadoPagos/Create
        [HttpGet]
        public IActionResult Create()
        {
            var vm = new PagoVM
            {
                Pago = new Pago(),
                ListaRentas = _context.Renta
                    .Include(r => r.Cliente)
                    .Include(r => r.Vehiculo)
                    .Select(r => new SelectListItem
                    {
                        Text = $"{r.Cliente.Nombres} {r.Cliente.Apellidos} — {r.Vehiculo.Marca} {r.Vehiculo.Modelo}",
                        Value = r.Id.ToString()
                    })
            };
            return View(vm);
        }

        // POST: /Empleado/EmpleadoPagos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PagoVM pagoVM)
        {
            if (!ModelState.IsValid)
            {
                pagoVM.ListaRentas = _context.Renta
                    .Include(r => r.Cliente)
                    .Include(r => r.Vehiculo)
                    .Select(r => new SelectListItem
                    {
                        Text = $"{r.Cliente.Nombres} {r.Cliente.Apellidos} — {r.Vehiculo.Marca} {r.Vehiculo.Modelo}",
                        Value = r.Id.ToString()
                    });
                return View(pagoVM);
            }

            var renta = _contenedor.Renta.Get(pagoVM.Pago.RentaId);
            if (renta == null)
            {
                ModelState.AddModelError("", "Renta no válida.");
                return View(pagoVM);
            }

            pagoVM.Pago.Monto = renta.Total;
            pagoVM.Pago.FechaPago = DateTime.Now;
            pagoVM.Pago.Estado = "Pendiente";

            _contenedor.Pago.Add(pagoVM.Pago);
            _contenedor.Save();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Empleado/EmpleadoPagos/Edit/5
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var pago = _contenedor.Pago.Get(id);
            if (pago == null) return NotFound();

            var vm = new PagoVM
            {
                Pago = pago,
                ListaRentas = _context.Renta
                    .Include(r => r.Cliente)
                    .Include(r => r.Vehiculo)
                    .Select(r => new SelectListItem
                    {
                        Text = $"{r.Cliente.Nombres} {r.Cliente.Apellidos} — {r.Vehiculo.Marca} {r.Vehiculo.Modelo}",
                        Value = r.Id.ToString(),
                        Selected = r.Id == pago.RentaId
                    })
            };
            return View(vm);
        }

        // POST: /Empleado/EmpleadoPagos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(PagoVM pagoVM)
        {
            if (!ModelState.IsValid)
            {
                pagoVM.ListaRentas = _context.Renta
                    .Include(r => r.Cliente)
                    .Include(r => r.Vehiculo)
                    .Select(r => new SelectListItem
                    {
                        Text = $"{r.Cliente.Nombres} {r.Cliente.Apellidos} — {r.Vehiculo.Marca} {r.Vehiculo.Modelo}",
                        Value = r.Id.ToString(),
                        Selected = r.Id == pagoVM.Pago.RentaId
                    });
                return View(pagoVM);
            }

            _contenedor.Pago.Update(pagoVM.Pago);
            _contenedor.Save();
            return RedirectToAction(nameof(Index));
        }

        // DELETE: /Empleado/EmpleadoPagos/Delete/5
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var pago = _contenedor.Pago.Get(id);
            if (pago == null)
                return Json(new { success = false, message = "Pago no encontrado." });

            _contenedor.Pago.Remove(pago);
            _contenedor.Save();
            return Json(new { success = true, message = "Pago eliminado." });
        }

        // GET: /Empleado/EmpleadoPagos/Recibo/5
        [HttpGet]
        public IActionResult Recibo(int id)
        {
            var pago = _context.Pago
                .Include(p => p.Renta).ThenInclude(r => r.Cliente)
                .Include(p => p.Renta).ThenInclude(r => r.Vehiculo)
                .FirstOrDefault(p => p.Id == id);
            if (pago == null) return NotFound();
            return View(pago);
        }
    }
}
