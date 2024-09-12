using PortalProveedor.Entities;
using System.ComponentModel.DataAnnotations;

namespace PortalProveedor.Models.Usuarios
{
    public class AltaUsuarioRequest
    {
        [Required(ErrorMessage = "El Nombre es requerido")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El Email es requerido")]
        [EmailAddress(ErrorMessage = "El Email no es una dirección de correo electrónico válida.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La Clave es requerida")]
        [StringLength(12, ErrorMessage = "La {0} debe tener al menos {2} caracteres.", MinimumLength = 6)]
        [RegularExpression(@"^((?=.*[a-z])(?=.*[A-Z])(?=.*\d)).+$", ErrorMessage = "El password debe incluir al menos una letra mayúscula, una letra minúscula y un dígito numérico.")]
        [DataType(DataType.Password)]
        public string Clave { get; set; }

        [Required(ErrorMessage = "El Cliente es requerido")]
        public int Cliente { get; set; }

        [Required(ErrorMessage = "El Rol es requerido")]
        public virtual ICollection<AltaRolSociedadUsuarioRequest> RolSociedadUsuarios { get; set; } = new List<AltaRolSociedadUsuarioRequest>();

        [Required(ErrorMessage = "El Estado es requerido")]
        public byte Estado { get; set; }

        [Required(ErrorMessage = "El Ausente es requerido")]
        public bool Ausente { get; set; }
    }
}
