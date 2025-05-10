using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTienda.AccesoDatos.Data.Repository.iRepository
{
    public interface IContenedorTrabajo : IDisposable
    {

        IVehiculoRepository Vehiculo { get; }
        IClienteRepository Cliente { get; }
        IRentaRepository Renta { get; }

        IPagoRepository Pago { get; }



        //IRentaRepository Renta { get; }
        //IPagoRepository Pago { get; }
        void Save();
    }

}