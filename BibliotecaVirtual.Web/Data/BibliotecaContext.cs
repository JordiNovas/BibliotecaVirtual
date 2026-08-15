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
        public DbSet<Libro> Libros { get; set; }
        public DbSet<Prestamo> Prestamos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cargar Libros de prueba iniciales
            modelBuilder.Entity<Libro>().HasData(
                new Libro { Id = 1, Titulo = "Cien años de soledad", Autor = "Gabriel García Márquez", Categoria = "Novela", ISBN = "978-0307474728", Disponible = true },
                new Libro { Id = 2, Titulo = "El principito", Autor = "Antoine de Saint-Exupéry", Categoria = "Infantil", ISBN = "978-0156013987", Disponible = true },
                new Libro { Id = 3, Titulo = "Clean Code", Autor = "Robert C. Martin", Categoria = "Tecnología", ISBN = "978-0132350884", Disponible = true },
                new Libro { Id = 4, Titulo = "Don Quijote de la Mancha", Autor = "Miguel de Cervantes", Categoria = "Clásico", ISBN = "978-8424115463", Disponible = true },
                new Libro { Id = 5, Titulo = "Aprende C# en 21 Días", Autor = "Bradley L. Jones", Categoria = "Tecnología", ISBN = "978-0672320712", Disponible = true }
            );
        }
    }
}