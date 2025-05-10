using SistemaTienda.AccesoDatos.Data.Repository.iRepository;
using SistemaTienda.Data;
using SistemaTienda.Models;
using System.Collections.Generic;
using System.Linq;

namespace SistemaTienda.AccesoDatos.Data.Repository
{
    public class VehiculoRepository : Repository<Vehiculo>, IVehiculoRepository
    {
        private readonly ApplicationDbContext _db;
        public VehiculoRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public IEnumerable<Vehiculo> GetAllVehiculos()
        {
            return _db.Vehiculo.ToList();
        }

        public void Update(Vehiculo vehiculo)
        {
            var objDesdeDb = _db.Vehiculo.FirstOrDefault(v => v.Id == vehiculo.Id);
            if (objDesdeDb != null)
            {
                objDesdeDb.Marca = vehiculo.Marca;
                objDesdeDb.Modelo = vehiculo.Modelo;
                objDesdeDb.Anio = vehiculo.Anio;
                objDesdeDb.Kilometraje = vehiculo.Kilometraje;
                objDesdeDb.Estado = vehiculo.Estado;
                objDesdeDb.UrlImagen = vehiculo.UrlImagen; // Actualiza la imagen
                objDesdeDb.PrecioPorDia = vehiculo.PrecioPorDia;
                _db.SaveChanges();
            }
        }
    }
}
