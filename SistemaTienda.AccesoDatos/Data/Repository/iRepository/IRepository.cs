using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTienda.AccesoDatos.Data.Repository.iRepository
{
    public interface iRepository<T> where T : class
    {
        T Get(int id);
        IEnumerable<T> GetAll(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            string? includeProperties = null);

        T? GetFirstOrDefault( // ← ESTA ES LA CLAVE
            Expression<Func<T, bool>>? filter = null,
            string? includeProperties = null);

        void Add(T entity);
        void Remove(int id);
        void Remove(T entity);
    }
}
