namespace PortalProveedor.Models.Usuarios;

using System.ComponentModel.DataAnnotations;

public class AuthenticateRequest
{
    [Required(ErrorMessage = "El Email es requerido")]
    [EmailAddress(ErrorMessage = "El Email no es una direcci�n de correo electr�nico v�lida.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "El Password es requerido")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}