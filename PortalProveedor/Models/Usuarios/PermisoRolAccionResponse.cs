namespace PortalProveedor.Models.Usuarios
{
    public class PermisoRolAccionResponse
    {
        public byte Id { get; set; }

        public byte Rol { get; set; }

        public byte Accion { get; set; }

        public bool Concedido { get; set; }

        public virtual AccionResponse accionNavigation { get; set; }
    }
}
