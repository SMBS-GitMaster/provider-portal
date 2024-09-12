namespace PortalProveedor.Models.LoginProveedores
{
    public class LoginProveedorResponse
    {
        public int Id { get; set; }

        public string Email { get; set; } = null!;

        public DateTime FechaAlta { get; set; }
    }
}
