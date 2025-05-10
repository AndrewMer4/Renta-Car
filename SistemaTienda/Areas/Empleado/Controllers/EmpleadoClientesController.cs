using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaTienda.AccesoDatos.Data.Repository.iRepository;
using SistemaTienda.Models;
using SistemaTienda.Utilidades;
using System.Linq;

namespace SistemaTienda.Areas.Empleado.Controllers
{
    [Authorize(Roles = CNT.Empleado)]
    [Area("Empleado")]
    public class EmpleadoClientesController : Controller
    {
        private readonly IContenedorTrabajo _contenedor;

        public EmpleadoClientesController(IContenedorTrabajo contenedor)
        {
            _contenedor = contenedor;
        }

        // GET: /Empleado/EmpleadoClientes
        [HttpGet]
        public IActionResult Index() => View();

        // GET: /Empleado/EmpleadoClientes/GetAll
        [HttpGet]
        public IActionResult GetAll()
        {
            var data = _contenedor.Cliente
                .GetAll()
                .Select(c => new {
                    id = c.Id,
                    nombres = c.Nombres,
                    apellidos = c.Apellidos,
                    dui = c.DUI,
                    telefono = c.Telefono,
                    edad = c.Edad,
                    direccion = c.Direccion
                });
            return Json(new { data });
        }

        // GET: /Empleado/EmpleadoClientes/Create
        [HttpGet]
        public IActionResult Create() => View();

        // POST: /Empleado/EmpleadoClientes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Clientes cliente)
        {
            if (!ModelState.IsValid) return View(cliente);

            _contenedor.Cliente.Add(cliente);
            _contenedor.Save();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Empleado/EmpleadoClientes/Edit/5
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var cliente = _contenedor.Cliente.Get(id);
            if (cliente == null) return NotFound();
            return View(cliente);
        }

        // POST: /Empleado/EmpleadoClientes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Clientes cliente)
        {
            if (!ModelState.IsValid) return View(cliente);

            var db = _contenedor.Cliente.Get(cliente.Id);
            if (db == null) return NotFound();

            db.Nombres = cliente.Nombres;
            db.Apellidos = cliente.Apellidos;
            db.DUI = cliente.DUI;
            db.Telefono = cliente.Telefono;
            db.Edad = cliente.Edad;
            db.Direccion = cliente.Direccion;

            _contenedor.Cliente.Update(db);
            _contenedor.Save();
            return RedirectToAction(nameof(Index));
        }

        // DELETE: /Empleado/EmpleadoClientes/Delete/5
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var cliente = _contenedor.Cliente.Get(id);
            if (cliente == null)
                return Json(new { success = false, message = "Cliente no encontrado." });

            // Validación: Comprobar si el cliente tiene rentas asociadas
            var rentasAsociadas = _contenedor.Renta.GetAll(r => r.ClienteId == id).Any();
            if (rentasAsociadas)
            {
                return Json(new { success = false, message = "No se puede eliminar al cliente porque tiene rentas asociadas." });
            }

            // Eliminar el cliente si no tiene rentas asociadas
            _contenedor.Cliente.Remove(cliente);
            _contenedor.Save();
            return Json(new { success = true, message = "Cliente eliminado." });
        }
    }
}
