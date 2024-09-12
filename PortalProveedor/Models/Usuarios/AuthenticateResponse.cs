namespace PortalProveedor.Models.Usuarios;

public class AuthenticateResponse
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public DateTime FechaAlta { get; set; }
    public string Email { get; set; } = null!;
    public int Cliente { get; set; }
    public byte Estado { get; set; }
    public string Rol { get; set; }
    public string Token { get; set; }
}