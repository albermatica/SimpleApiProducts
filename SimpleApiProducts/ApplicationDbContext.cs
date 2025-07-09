using Microsoft.EntityFrameworkCore;
using SimpleApiProducts.Models;

namespace SimpleApiProducts 
{
    public class ApplicationDbContext : DbContext // Hereda de DbContext
    {
        // Constructor: Configura el contexto con las opciones proporcionadas
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // DbSet para tu entidad Producto. Representará la tabla 'Productos' en la DB.
        public DbSet<Producto> Productos { get; set; }
    }
}
