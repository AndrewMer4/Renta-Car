using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaTienda.Models;
using SistemaTienda.Models.ViewModels;

namespace SistemaTienda.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RegistroController : Controller
    {
        private readonly UserManager<ApplicationUser> _userMgr;
        private readonly RoleManager<IdentityRole> _roleMgr;

        public RegistroController(
            UserManager<ApplicationUser> userMgr,
            RoleManager<IdentityRole> roleMgr)
        {
            _userMgr = userMgr;
            _roleMgr = roleMgr;
        }

        // GET /Admin/Registro
        [HttpGet]
        public IActionResult Index() => View();

        // GET /Admin/Registro/GetAll  → para DataTable
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var usuarios = await _userMgr.Users.ToListAsync();
            var data = new List<object>();

            foreach (var u in usuarios)
            {
                var roles = await _userMgr.GetRolesAsync(u);
                data.Add(new
                {
                    id = u.Id,
                    nombre = u.Nombre,
                    apellido = u.Apellido,
                    email = u.Email,
                    role = roles.FirstOrDefault() ?? ""
                });
            }

            return Json(new { data });
        }

        // GET /Admin/Registro/Register
        [HttpGet]
        public async Task<IActionResult> Register()
        {
            var roles = await _roleMgr.Roles
                .Select(r => new { Value = r.Name, Text = r.Name })
                .ToListAsync();

            var vm = new UserVM
            {
                IsCreate = true,
                Roles = new SelectList(roles, "Value", "Text")
            };
            return View("Register", vm);
        }

        // POST /Admin/Registro/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserVM vm)
        {
            if (!ModelState.IsValid)
            {
                var roles = await _roleMgr.Roles
                    .Select(r => new { Value = r.Name, Text = r.Name })
                    .ToListAsync();
                vm.Roles = new SelectList(roles, "Value", "Text");
                return View("Register", vm);
            }

            var user = new ApplicationUser
            {
                UserName = vm.Email,
                Email = vm.Email,
                Nombre = vm.Nombre,
                Apellido = vm.Apellido
            };

            var result = await _userMgr.CreateAsync(user, vm.Password);
            if (result.Succeeded)
            {
                await _userMgr.AddToRoleAsync(user, vm.Role);
                return RedirectToAction(nameof(Index));
            }

            foreach (var e in result.Errors)
                ModelState.AddModelError(string.Empty, e.Description);

            var allRoles = await _roleMgr.Roles
                .Select(r => new { Value = r.Name, Text = r.Name })
                .ToListAsync();
            vm.Roles = new SelectList(allRoles, "Value", "Text");
            return View("Register", vm);
        }

        // GET /Admin/Registro/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userMgr.FindByIdAsync(id);
            if (user == null) return NotFound();

            var userRole = (await _userMgr.GetRolesAsync(user)).FirstOrDefault() ?? "";
            var roles = await _roleMgr.Roles
                .Select(r => new { Value = r.Name, Text = r.Name })
                .ToListAsync();

            var vm = new UserVM
            {
                Id = user.Id,
                Nombre = user.Nombre,
                Apellido = user.Apellido,
                Email = user.Email,
                Role = userRole,
                IsCreate = false,
                Roles = new SelectList(roles, "Value", "Text", userRole)
            };
            return View(vm);
        }

        // POST /Admin/Registro/Edit
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserVM vm)
        {
            if (!ModelState.IsValid)
            {
                var roles = await _roleMgr.Roles
                    .Select(r => new { Value = r.Name, Text = r.Name })
                    .ToListAsync();
                vm.Roles = new SelectList(roles, "Value", "Text", vm.Role);
                return View(vm);
            }

            var user = await _userMgr.FindByIdAsync(vm.Id);
            if (user == null) return NotFound();

            user.Nombre = vm.Nombre;
            user.Apellido = vm.Apellido;
            user.Email = vm.Email;
            user.UserName = vm.Email;

            var updateResult = await _userMgr.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                foreach (var e in updateResult.Errors)
                    ModelState.AddModelError(string.Empty, e.Description);

                var rolesAgain = await _roleMgr.Roles
                    .Select(r => new { Value = r.Name, Text = r.Name })
                    .ToListAsync();
                vm.Roles = new SelectList(rolesAgain, "Value", "Text", vm.Role);
                return View(vm);
            }

            var currentRoles = await _userMgr.GetRolesAsync(user);
            if (currentRoles.FirstOrDefault() != vm.Role)
            {
                await _userMgr.RemoveFromRolesAsync(user, currentRoles);
                await _userMgr.AddToRoleAsync(user, vm.Role);
            }

            return RedirectToAction(nameof(Index));
        }

        // DELETE /Admin/Registro/Delete/{id}
        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userMgr.FindByIdAsync(id);
            if (user == null)
                return Json(new { success = false, message = "Usuario no encontrado." });

            var result = await _userMgr.DeleteAsync(user);
            return Json(new
            {
                success = result.Succeeded,
                message = result.Succeeded ? "Usuario eliminado." : "Error al eliminar."
            });
        }
    }
}
