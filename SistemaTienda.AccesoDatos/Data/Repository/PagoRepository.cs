using Microsoft.EntityFrameworkCore;
using SistemaTienda.AccesoDatos.Data.Repository.iRepository;
using SistemaTienda.Data;
using SistemaTienda.Models;
using System.Collections.Generic;
using System.Linq;

namespace SistemaTienda.AccesoDatos.Data.Repository
{
    public class PagoRepository : Repository<Pago>, IPagoRepository
    {
        private readonly ApplicationDbContext _db;

        public PagoRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public IEnumerable<Pago> GetAll(string? includeProperties = null)
        {
            IQueryable<Pago> query = _db.Pago;

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties.Split(',', System.StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            return query.ToList();
        }

        public void Update(Pago pago)
        {
            var objDesdeDb = _db.Pago.FirstOrDefault(s => s.Id == pago.Id);
            if (objDesdeDb != null)
            {
                objDesdeDb.Monto = pago.Monto;
                objDesdeDb.FechaPago = pago.FechaPago;
                objDesdeDb.Estado = pago.Estado;
                objDesdeDb.Notas = pago.Notas;
                objDesdeDb.RentaId = pago.RentaId;

                _db.SaveChanges();
            }
        }
    }
}
