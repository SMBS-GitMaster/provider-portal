namespace PortalProveedor.Models.Usuarios;

using System.ComponentModel.DataAnnotations;

public class RegisterRequest
{
    [Required(ErrorMessage = "El NombreCompleto es requerido")]
    public string NombreCompleto { get; set; }

    
    [Required(ErrorMessage = "El Email es requerido")]
    [EmailAddress(ErrorMessage = "El Email no es una direcci�n de correo electr�nico v�lida.")]
    public string Email { get; set; }


    [Required(ErrorMessage = "La Empresa es requerida")]
    public int Empresa { get; set; }


    [Required(ErrorMessage = "El Password es requerido")]
    [StringLength(12, ErrorMessage = "El {0} debe tener al menos {2} caracteres.", MinimumLength = 6)]
    [RegularExpression(@"^((?=.*[a-z])(?=.*[A-Z])(?=.*\d)).+$", ErrorMessage = "El password debe incluir al menos una letra may�scula, una letra min�scula y un d�gito num�rico.")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}