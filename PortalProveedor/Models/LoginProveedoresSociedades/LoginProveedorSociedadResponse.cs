using PortalProveedor.Entities;

namespace PortalProveedor.Models.LoginProveedoresSociedades
{
    public class LoginProveedorSociedadResponse
    {
        public int Id { get; set; }
        public int SociedadId { get; set; }
        public string Sociedad { get; set; }
        public int LoginProveedor { get; set; }
    }
}
