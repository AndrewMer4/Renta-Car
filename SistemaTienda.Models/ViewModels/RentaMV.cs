using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SistemaTienda.Models.ViewModels
{
    public class RentaVM
    {
        public Renta Renta { get; set; }

        public IEnumerable<SelectListItem> ListaClientes { get; set; }
        public IEnumerable<SelectListItem> ListaVehiculos { get; set; }
    }
}
