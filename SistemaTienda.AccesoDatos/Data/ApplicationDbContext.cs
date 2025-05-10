using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SistemaTienda.Models;

namespace SistemaTienda.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        public DbSet<Renta> Renta { get; set; }
        public DbSet<Pago> Pago { get; set; }
        public DbSet<Vehiculo> Vehiculo { get; set; }
        public DbSet<Clientes> Clientes { get; set; }
    }
}