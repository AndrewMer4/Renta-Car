using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaTienda.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTienda.AccesoDatos.Data.Repository.iRepository
{
    public interface IVehiculoRepository : iRepository<Vehiculo>
    {
        IEnumerable<Vehiculo> GetAllVehiculos();
        void Update(Vehiculo vehiculo);
    }
}
