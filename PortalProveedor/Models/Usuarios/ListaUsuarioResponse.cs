using PortalProveedor.Entities;
using PortalProveedor.Models.Usuarios;

namespace PortalProveedor.Models.Usuarios
{
    public class ListaUsuarioResponse
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public int ClienteId { get; set; }
        public string Cliente { get; set; }
        public ICollection<RolSociedadUsuarioResponse> RolSociedadUsuario { get; set; }
    }
}
