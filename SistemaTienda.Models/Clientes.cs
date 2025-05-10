using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTienda.Models
{
    public class Clientes
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "El nombre solo puede contener letras y espacios.")]
        public string Nombres { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "El apellido solo puede contener letras y espacios.")]
        public string Apellidos { get; set; }

        [Range(18, 120, ErrorMessage = "La edad debe ser de al menos 18 años")]
        public int Edad { get; set; }

        [Required(ErrorMessage = "El DUI es obligatorio")]
        [RegularExpression(@"^\d{8}-\d{1}$", ErrorMessage = "Formato de DUI inválido")]
        public string DUI { get; set; }

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [RegularExpression(@"^[267]{1}[0-9]{7}$", ErrorMessage = "El teléfono debe tener 8 dígitos y comenzar con 2, 6 o 7")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "La dirección es obligatoria")]
        public string Direccion { get; set; }
    }
} 