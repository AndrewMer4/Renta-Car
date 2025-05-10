using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaTienda.AccesoDatos.Data.Repository.iRepository;
using SistemaTienda.Data;
using SistemaTienda.Models;
using System.Collections.Generic;
using System.Linq;

namespace SistemaTienda.AccesoDatos.Data.Repository
{
    public class RentaRepository : Repository<Renta>, IRentaRepository
    {
        private readonly ApplicationDbContext _db;

        public RentaRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public IEnumerable<Renta> GetAll(string? includeProperties = null)
        {
            IQueryable<Renta> query = _db.Renta;

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties.Split(',', System.StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            return query.ToList();
        }

        public void Update(Renta renta)
        {
            var objBD = _db.Renta.FirstOrDefault(r => r.Id == renta.Id);
            if (objBD != null)
            {
                objBD.ClienteId = renta.ClienteId;
                objBD.VehiculoId = renta.VehiculoId;
                objBD.FechaInicio = renta.FechaInicio;
                objBD.FechaFin = renta.FechaFin;
                objBD.Total = renta.Total;

                _db.SaveChanges();
            }
        }
    }
}
