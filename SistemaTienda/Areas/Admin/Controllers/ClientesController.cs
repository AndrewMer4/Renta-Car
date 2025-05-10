using Microsoft.AspNetCore.Mvc;
using SistemaTienda.AccesoDatos.Data.Repository.iRepository;
using SistemaTienda.Data;
using SistemaTienda.Models;
using SistemaTienda.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;

namespace SistemaTienda.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]

    [Area("Admin")]
    public class ClientesController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public ClientesController(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }

        // GET: /Admin/Clientes
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // GET: /Admin/Clientes/GetAll
        [HttpGet]
        public IActionResult GetAll()
        {
            var clientes = _contenedorTrabajo.Cliente.GetAll();
            return Json(new { data = clientes });
        }

        // GET: /Admin/Clientes/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Admin/Clientes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Clientes cliente)
        {
            if (ModelState.IsValid)
            {
                _contenedorTrabajo.Cliente.Add(cliente);
                _contenedorTrabajo.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(cliente);
        }

        // GET: /Admin/Clientes/Edit/{id}
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var cliente = _contenedorTrabajo.Cliente.Get(id);
            if (cliente == null)
                return NotFound();

            return View(cliente);
        }

        // POST: /Admin/Clientes/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Clientes cliente)
        {
            if (ModelState.IsValid)
            {
                var clienteDesdeDb = _contenedorTrabajo.Cliente.Get(cliente.Id);
                if (clienteDesdeDb == null)
                    return NotFound();

                clienteDesdeDb.Nombres = cliente.Nombres;
                clienteDesdeDb.Apellidos = cliente.Apellidos;
                clienteDesdeDb.Edad = cliente.Edad;
                clienteDesdeDb.DUI = cliente.DUI;
                clienteDesdeDb.Telefono = cliente.Telefono;
                clienteDesdeDb.Direccion = cliente.Direccion;

                _contenedorTrabajo.Cliente.Update(clienteDesdeDb);
                _contenedorTrabajo.Save();

                return RedirectToAction(nameof(Index));
            }

            return View(cliente);
        }

        // DELETE: /Admin/Clientes/Delete/{id}
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var clienteDesdeDb = _contenedorTrabajo.Cliente.Get(id);
            if (clienteDesdeDb == null)
            {
                return Json(new { success = false, message = "Cliente no encontrado" });
            }

            // Validación de que el cliente no tenga registros asociados
            if (_contenedorTrabajo.Renta.GetAll(r => r.ClienteId == clienteDesdeDb.Id).Any())
            {
                return Json(new { success = false, message = "No se puede eliminar el cliente porque tiene rentas asociadas." });
            }

            _contenedorTrabajo.Cliente.Remove(clienteDesdeDb);
            _contenedorTrabajo.Save();

            return Json(new { success = true, message = "Cliente eliminado correctamente" });
        }
    }
}