using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;

namespace SistemaTienda.Models
{
    public class Pago
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "La renta es obligatoria")]
        public int RentaId { get; set; }

        [ForeignKey("RentaId")]
        public Renta Renta { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Monto { get; set; } // Se llenará desde Renta.Total automáticamente

        [Required]
        [Display(Name = "Fecha de Pago")]
        [DataType(DataType.Date)]
        public DateTime FechaPago { get; set; } = DateTime.Now;

        [Display(Name = "Estado")]
        public string Estado { get; set; } = "Pendiente"; // Default: Pendiente

        [Display(Name = "Notas")]
        public string Notas { get; set; }
    }
}
