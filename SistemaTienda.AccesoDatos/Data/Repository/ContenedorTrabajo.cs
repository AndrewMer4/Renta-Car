using SistemaTienda.AccesoDatos.Data.Repository.iRepository;
using SistemaTienda.Data;
using SistemaTienda.Models;

namespace SistemaTienda.AccesoDatos.Data.Repository
{
    public class ContenedorTrabajo : IContenedorTrabajo
    {
        private readonly ApplicationDbContext _db;
        public ContenedorTrabajo(ApplicationDbContext db)
        {
            _db = db;
           
            Vehiculo = new VehiculoRepository(_db);
            Cliente = new ClienteRepository(_db);
            Renta = new RentaRepository(_db);
            Pago = new PagoRepository(_db);

            //Renta = new RentaRepository(_db);
            //Pago = new PagoRepository(_db);
        }
       
        public IVehiculoRepository Vehiculo { get; private set; }
        public IClienteRepository Cliente { get; private set; }

        public IRentaRepository Renta { get; private set; }

        public IPagoRepository Pago { get; private set; }


        //public IRentaRepository Renta { get; private set; }
        //public IPagoRepository Pago { get; private set; }
        public void Save() { _db.SaveChanges(); }
        public void Dispose() { _db.Dispose(); }
    }

}
