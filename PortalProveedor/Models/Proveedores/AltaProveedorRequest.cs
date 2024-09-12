using System.ComponentModel.DataAnnotations;

namespace PortalProveedor.Models.Proveedores
{
    public class AltaProveedorRequest
    {
        [Required(ErrorMessage = "El Nombre es requerido")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El Identificador es requerido")]
        public string Identificador { get; set; }

        [Required(ErrorMessage = "El Tipo de Identificador es requerido")]
        public byte TipoIdentificador { get; set; }

        [Required(ErrorMessage = "El Estado es requerido")]
        public byte Estado { get; set; }

        [Required(ErrorMessage = "El Login del Proveedor es requerido")]
        public ICollection<LoginProveedorRequest> LoginProveedor { get; set; }

        [Required(ErrorMessage = "La Sociedad es requerida")]
        public int[] Sociedades { get; set; }
    }

    public class LoginProveedorRequest
    {
        [Required(ErrorMessage = "El Email es requerido")]
        [EmailAddress(ErrorMessage = "El Email no es una dirección de correo electrónico válida.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "El Password es requerido")]
        [StringLength(12, ErrorMessage = "El {0} debe tener al menos {2} caracteres.", MinimumLength = 6)]
        [RegularExpression(@"^((?=.*[a-z])(?=.*[A-Z])(?=.*\d)).+$", ErrorMessage = "El password debe incluir al menos una letra mayúscula, una letra minúscula y un dígito numérico.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
