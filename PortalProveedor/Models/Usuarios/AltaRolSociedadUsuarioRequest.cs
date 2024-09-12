using PortalProveedor.Entities;

namespace PortalProveedor.Models.Usuarios
{
    public class AltaRolSociedadUsuarioRequest
    {
        public byte Rol { get; set; }

        public int Sociedad { get; set; }
    }
}
