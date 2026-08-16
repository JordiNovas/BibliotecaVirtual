using System.ComponentModel.DataAnnotations;

namespace BibliotecaVirtual.Web.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        [Display(Name = "Nombre completo")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido.")]
        [StringLength(150)]
        public string Correo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        [StringLength(50, MinimumLength = 4,
            ErrorMessage = "El usuario debe tener entre 4 y 50 caracteres.")]
        [Display(Name = "Nombre de usuario")]
        public string NombreUsuario { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        [Display(Name = "Rol")]
        public string Rol { get; set; } = "Usuario";

        [Display(Name = "Usuario activo")]
        public bool Activo { get; set; } = true;

        [Display(Name = "Fecha de registro")]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}