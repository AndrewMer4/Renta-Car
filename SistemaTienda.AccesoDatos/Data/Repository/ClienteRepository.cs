using SistemaTienda.AccesoDatos.Data.Repository.iRepository;
using SistemaTienda.Data;
using SistemaTienda.Models;

namespace SistemaTienda.AccesoDatos.Data.Repository
{
    public class ClienteRepository : Repository<Clientes>, IClienteRepository
    {
        private readonly ApplicationDbContext _db;

        public ClienteRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Clientes cliente)
        {
            var objDesdeDb = _db.Clientes.FirstOrDefault(c => c.Id == cliente.Id);
            if (objDesdeDb != null)
            {
                objDesdeDb.Nombres = cliente.Nombres;
                objDesdeDb.Apellidos = cliente.Apellidos;
                objDesdeDb.Edad = cliente.Edad;
                objDesdeDb.DUI = cliente.DUI;
                objDesdeDb.Telefono = cliente.Telefono;
                objDesdeDb.Direccion = cliente.Direccion;

                _db.SaveChanges();
            }
        }
    }
}