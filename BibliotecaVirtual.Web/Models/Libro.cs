using System.ComponentModel.DataAnnotations;

namespace BibliotecaVirtual.Web.Models
{
    public class Libro
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El título es obligatorio.")]
        [StringLength(150)]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El autor es obligatorio.")]
        [StringLength(100)]
        public string Autor { get; set; } = string.Empty;

        [Required(ErrorMessage = "La categoría es obligatoria.")]
        [StringLength(50)]
        public string Categoria { get; set; } = string.Empty;

        [StringLength(20)]
        public string? ISBN { get; set; }

        public bool Disponible { get; set; } = true;

        // Relación con préstamos
        public ICollection<Prestamo>? Prestamos { get; set; }
    }
}