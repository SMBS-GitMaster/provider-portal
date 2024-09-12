using PortalProveedor.Entities;

namespace PortalProveedor.Models.Usuarios
{
    public class RolSociedadUsuarioResponse
    {
        public byte RolId { get; set; }
        public string Rol { get; set; }
        public List<PermisoRolAccionResponse> permisoRolAccions { get; set; }

        public int SociedadId { get; set; }
        public string Sociedad { get; set; }
    }
}
