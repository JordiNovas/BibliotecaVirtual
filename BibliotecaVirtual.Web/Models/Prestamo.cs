using System.ComponentModel.DataAnnotations;

namespace BibliotecaVirtual.Web.Models
{
    public class Prestamo
    {
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        [Required]
        public int LibroId { get; set; }
        public Libro? Libro { get; set; }

        public DateTime FechaPrestamo { get; set; } = DateTime.Now;
        public DateTime? FechaDevolucion { get; set; }

        [Required]
        public string Estado { get; set; } = "Activo"; // "Activo" o "Devuelto"
    }
}