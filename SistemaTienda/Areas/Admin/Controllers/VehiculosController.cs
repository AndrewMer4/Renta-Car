using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SistemaTienda.AccesoDatos.Data.Repository.iRepository;
using SistemaTienda.Models;
using System;
using System.IO;

namespace SistemaTienda.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")] 
    [Area("Admin")]
    public class VehiculosController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public VehiculosController(IContenedorTrabajo contenedorTrabajo, IWebHostEnvironment hostingEnvironment)
        {
            _contenedorTrabajo = contenedorTrabajo;
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
            var lista = _contenedorTrabajo.Vehiculo.GetAllVehiculos();
            return Json(new { data = lista });
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Vehiculo vehiculo)
        {
            if (ModelState.IsValid)
            {
                var archivos = HttpContext.Request.Form.Files;
                if (archivos.Count > 0)
                {
                    string rutaPrincipal = _hostingEnvironment.WebRootPath;
                    string nombreArchivo = Guid.NewGuid().ToString();
                    var extension = Path.GetExtension(archivos[0].FileName);
                    var carpeta = Path.Combine(rutaPrincipal, "imagenes/vehiculos");

                    if (!Directory.Exists(carpeta))
                        Directory.CreateDirectory(carpeta);

                    using (var stream = new FileStream(Path.Combine(carpeta, nombreArchivo + extension), FileMode.Create))
                    {
                        archivos[0].CopyTo(stream);
                    }

                    vehiculo.UrlImagen = "/imagenes/vehiculos/" + nombreArchivo + extension;
                }

                _contenedorTrabajo.Vehiculo.Add(vehiculo);
                _contenedorTrabajo.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(vehiculo);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var vehiculo = _contenedorTrabajo.Vehiculo.Get(id);
            if (vehiculo == null)
                return NotFound();
            return View(vehiculo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Vehiculo vehiculo)
        {
            if (ModelState.IsValid)
            {
                var archivos = HttpContext.Request.Form.Files;
                var vehiculoDesdeDb = _contenedorTrabajo.Vehiculo.Get(vehiculo.Id);
                if (vehiculoDesdeDb == null)
                    return NotFound();

                if (archivos.Count > 0)
                {
                    if (!string.IsNullOrEmpty(vehiculoDesdeDb.UrlImagen))
                    {
                        string rutaPrincipal = _hostingEnvironment.WebRootPath;
                        var rutaImagen = Path.Combine(rutaPrincipal, vehiculoDesdeDb.UrlImagen.TrimStart('/', '\\'));
                        if (System.IO.File.Exists(rutaImagen))
                        {
                            System.IO.File.Delete(rutaImagen);
                        }
                    }

                    string rutaPrincipalNueva = _hostingEnvironment.WebRootPath;
                    string nombreArchivo = Guid.NewGuid().ToString();
                    var extension = Path.GetExtension(archivos[0].FileName);
                    var carpeta = Path.Combine(rutaPrincipalNueva, "imagenes/vehiculos");

                    if (!Directory.Exists(carpeta))
                        Directory.CreateDirectory(carpeta);

                    using (var stream = new FileStream(Path.Combine(carpeta, nombreArchivo + extension), FileMode.Create))
                    {
                        archivos[0].CopyTo(stream);
                    }
                    vehiculoDesdeDb.UrlImagen = "/imagenes/vehiculos/" + nombreArchivo + extension;
                }

                vehiculoDesdeDb.Marca = vehiculo.Marca;
                vehiculoDesdeDb.Modelo = vehiculo.Modelo;
                vehiculoDesdeDb.Anio = vehiculo.Anio;
                vehiculoDesdeDb.Kilometraje = vehiculo.Kilometraje;
                vehiculoDesdeDb.Estado = vehiculo.Estado;
                vehiculoDesdeDb.PrecioPorDia = vehiculo.PrecioPorDia;

                _contenedorTrabajo.Vehiculo.Update(vehiculoDesdeDb);
                _contenedorTrabajo.Save();

                return RedirectToAction(nameof(Index));
            }
            return View(vehiculo);
        }

        [HttpDelete]
        // DELETE: /Admin/Vehiculos/Delete/{id}
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var vehiculoDesdeDb = _contenedorTrabajo.Vehiculo.Get(id);
            if (vehiculoDesdeDb == null)
            {
                return Json(new { success = false, message = "Vehículo no encontrado" });
            }

            // Validación de que el vehículo no tenga registros de renta
            if (_contenedorTrabajo.Renta.GetAll(r => r.VehiculoId == vehiculoDesdeDb.Id).Any())
            {
                return Json(new { success = false, message = "No se puede eliminar el vehículo porque tiene rentas asociadas." });
            }

            _contenedorTrabajo.Vehiculo.Remove(vehiculoDesdeDb);
            _contenedorTrabajo.Save();

            return Json(new { success = true, message = "Vehículo eliminado correctamente" });
        }
    }
}
