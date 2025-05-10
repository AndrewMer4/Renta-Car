using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaTienda.Models;

namespace SistemaTienda.Models.ViewModels
{
    public class PagoVM
    {
        public Pago Pago { get; set; }
        public IEnumerable<SelectListItem> ListaRentas { get; set; }
    }
}
