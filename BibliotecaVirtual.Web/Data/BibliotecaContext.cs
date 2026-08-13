using BibliotecaVirtual.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaVirtual.Web.Data
{
    public class BibliotecaContext : DbContext
    {
        public BibliotecaContext(DbContextOptions<BibliotecaContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
    }
}