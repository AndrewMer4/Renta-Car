using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaTienda.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SistemaTienda.AccesoDatos.Data.Repository.iRepository
{
    public interface IRentaRepository : iRepository<Renta>
    {
        void Update(Renta renta);
        
    }
}
