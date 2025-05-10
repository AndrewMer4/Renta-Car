using SistemaTienda.Utilidades;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace SistemaTienda.Models.ViewModels
{
    public class UserVM
    {
        public string Id { get; set; }

        [Required, Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Required, Display(Name = "Apellido")]
        public string Apellido { get; set; }

        [Required, EmailAddress, Display(Name = "Correo")]
        public string Email { get; set; }

        [RequiredIf(nameof(IsCreate), true, ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [Display(Name = "Rol")]
        [Required(ErrorMessage = "Debe seleccionar un rol.")]
        public string Role { get; set; }
        public bool IsCreate { get; set; }
        public SelectList Roles { get; set; }
    }
}
