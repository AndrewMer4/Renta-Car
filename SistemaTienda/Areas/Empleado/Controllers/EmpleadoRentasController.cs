using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaTienda.AccesoDatos.Data.Repository.iRepository;
using SistemaTienda.Models;
using SistemaTienda.Models.ViewModels;
using SistemaTienda.Utilidades;
using System;
using System.Linq;

namespace SistemaTienda.Areas.Empleado.Controllers
{
    [Authorize(Roles = CNT.Empleado)]
    [Area("Empleado")]
    public class EmpleadoRentasController : Controller
    {
        private readonly IContenedorTrabajo _contenedor;

        public EmpleadoRentasController(IContenedorTrabajo contenedor)
        {
            _contenedor = contenedor;
        }

        // GET: /Empleado/EmpleadoRentas
        [HttpGet]
        public IActionResult Index() => View();

        // GET: /Empleado/EmpleadoRentas/GetAll
        [HttpGet]
        public IActionResult GetAll()
        {
            var data = _contenedor.Renta
                .GetAll(includeProperties: "Cliente,Vehiculo")
                .Select(r => new {
                    id = r.Id,
                    cliente = r.Cliente.Nombres + " " + r.Cliente.Apellidos,
                    vehiculo = r.Vehiculo.Marca + " " + r.Vehiculo.Modelo,
                    fechainicio = r.FechaInicio.ToString("yyyy-MM-dd"),
                    fechafin = r.FechaFin.ToString("yyyy-MM-dd"),
                    total = r.Total
                });
            return Json(new { data });
        }

        // GET: /Empleado/EmpleadoRentas/Create
        [HttpGet]
        public IActionResult Create()
        {
            var vm = new RentaVM
            {
                Renta = new Renta(),
                ListaClientes = _contenedor.Cliente.GetAll().Select(c => new SelectListItem
                {
                    Text = $"{c.Nombres} {c.Apellidos}",
                    Value = c.Id.ToString()
                }),
                ListaVehiculos = _contenedor.Vehiculo
                    .GetAll(v => v.Estado == "Disponible")
                    .Select(v => new SelectListItem
                    {
                        Text = $"{v.Marca} {v.Modelo} ({v.PrecioPorDia:C})",
                        Value = v.Id.ToString()
                    })
            };
            return View(vm);
        }

        // POST: /Empleado/EmpleadoRentas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(RentaVM vm)
        {
            var veh = _contenedor.Vehiculo.Get(vm.Renta.VehiculoId);
            if (veh == null)
            {
                ModelState.AddModelError("", "Vehículo no válido.");
            }
            else
            {
                var dias = (vm.Renta.FechaFin - vm.Renta.FechaInicio).Days;
                if (dias <= 0)
                    ModelState.AddModelError("", "La fecha fin debe ser posterior a la de inicio.");
                else
                    vm.Renta.Total = dias * veh.PrecioPorDia;
            }

            if (!ModelState.IsValid)
            {
                vm.ListaClientes = _contenedor.Cliente.GetAll().Select(c =>
                    new SelectListItem
                    {
                        Text = $"{c.Nombres} {c.Apellidos}",
                        Value = c.Id.ToString()
                    });
                vm.ListaVehiculos = _contenedor.Vehiculo
                    .GetAll(v => v.Estado == "Disponible")
                    .Select(v => new SelectListItem
                    {
                        Text = $"{v.Marca} {v.Modelo} ({v.PrecioPorDia:C})",
                        Value = v.Id.ToString()
                    });
                return View(vm);
            }

            _contenedor.Renta.Add(vm.Renta);
            veh.Estado = "Rentado";
            _contenedor.Vehiculo.Update(veh);
            _contenedor.Save();

            return RedirectToAction(nameof(Index));
        }

        // GET: /Empleado/EmpleadoRentas/Edit/5
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var renta = _contenedor.Renta.Get(id);
            if (renta == null) return NotFound();

            var vm = new RentaVM
            {
                Renta = renta,
                ListaClientes = _contenedor.Cliente.GetAll().Select(c =>
                    new SelectListItem
                    {
                        Text = $"{c.Nombres} {c.Apellidos}",
                        Value = c.Id.ToString(),
                        Selected = c.Id == renta.ClienteId
                    }),
                ListaVehiculos = _contenedor.Vehiculo.GetAll().Select(v =>
                    new SelectListItem
                    {
                        Text = $"{v.Marca} {v.Modelo} ({v.PrecioPorDia:C}) - {v.Estado}",
                        Value = v.Id.ToString(),
                        Selected = v.Id == renta.VehiculoId
                    })
            };
            return View(vm);
        }

        // POST: /Empleado/EmpleadoRentas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(RentaVM vm)
        {
            var veh = _contenedor.Vehiculo.Get(vm.Renta.VehiculoId);
            if (veh == null)
                ModelState.AddModelError("", "Vehículo no válido.");
            else
            {
                var dias = (vm.Renta.FechaFin - vm.Renta.FechaInicio).Days;
                if (dias <= 0)
                    ModelState.AddModelError("", "La fecha fin debe ser posterior a la de inicio.");
                else
                    vm.Renta.Total = dias * veh.PrecioPorDia;
            }

            if (!ModelState.IsValid)
            {
                vm.ListaClientes = _contenedor.Cliente.GetAll().Select(c =>
                    new SelectListItem
                    {
                        Text = $"{c.Nombres} {c.Apellidos}",
                        Value = c.Id.ToString()
                    });
                vm.ListaVehiculos = _contenedor.Vehiculo.GetAll().Select(v =>
                    new SelectListItem
                    {
                        Text = $"{v.Marca} {v.Modelo} ({v.PrecioPorDia:C}) - {v.Estado}",
                        Value = v.Id.ToString()
                    });
                return View(vm);
            }

            var original = _contenedor.Renta.Get(vm.Renta.Id);
            if (original.VehiculoId != vm.Renta.VehiculoId)
            {
                var oldVeh = _contenedor.Vehiculo.Get(original.VehiculoId);
                if (oldVeh != null)
                {
                    oldVeh.Estado = "Disponible";
                    _contenedor.Vehiculo.Update(oldVeh);
                }
                veh.Estado = "Rentado";
                _contenedor.Vehiculo.Update(veh);
            }

            _contenedor.Renta.Update(vm.Renta);
            _contenedor.Save();
            return RedirectToAction(nameof(Index));
        }

        // DELETE: /Empleado/EmpleadoRentas/Delete/5
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var renta = _contenedor.Renta.Get(id);
            if (renta == null)
                return Json(new { success = false, message = "Renta no encontrada." });

            var veh = _contenedor.Vehiculo.Get(renta.VehiculoId);
            if (veh != null)
            {
                veh.Estado = "Disponible";
                _contenedor.Vehiculo.Update(veh);
            }

            _contenedor.Renta.Remove(renta);
            _contenedor.Save();
            return Json(new { success = true, message = "Renta eliminada." });
        }
    }
}
