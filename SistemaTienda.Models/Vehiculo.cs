using System;
using System.ComponentModel.DataAnnotations;

namespace SistemaTienda.Models
{
    public class Vehiculo
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "La marca no puede tener más de 50 caracteres.")]
        public string Marca { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "El modelo no puede tener más de 50 caracteres.")]
        public string Modelo { get; set; }

        [Required]
        [Display(Name = "Año")]
        [Range(2000, 2026, ErrorMessage = "El año debe estar entre 2000 y 2026.")]
        public int Anio { get; set; }

        [Required(ErrorMessage = "El kilometraje es obligatorio.")]
        [Range(0, int.MaxValue, ErrorMessage = "El kilometraje debe ser un valor positivo.")]
        public int Kilometraje { get; set; }

        [Required]
        public string Estado { get; set; }

        [DataType(DataType.ImageUrl)]
        [Display(Name = "Imagen")]
        [Url(ErrorMessage = "La URL de la imagen no es válida.")]
        public string UrlImagen { get; set; }

        [Required(ErrorMessage = "El precio por día es obligatorio")]
        [Display(Name = "Precio por Día")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio por día debe ser mayor que 0.")]
        [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        public decimal PrecioPorDia { get; set; }
    }
}