namespace PortalProveedor.Models.Usuarios
{
    public class UsuarioResponse
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string email { get; set; }
        public int cliente { get; set; }
        public int estado { get; set; }
        public bool ausente { get; set; }
        public ClienteNavigationResponse clienteNavigation { get; set; }
        public EstadoNavigationResponse estadoNavigation { get; set; }
        public List<RolSociedadUsuarioResponse> rolSociedadUsuarios { get; set; }
        public DateTime fechaAlta { get; set; }
    }
    public class ClienteNavigationResponse
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string email { get; set; }
        public string identificador { get; set; }
        public string tipoIdentificador { get; set; }
        public DateTime fechaAlta { get; set; }
    }

    public class EstadoNavigationResponse
    {
        public int id { get; set; }
        public string nombre { get; set; }
    }




    public class RolNavigationResponse
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public List<object> permisoRolAccions { get; set; }
        public List<object> rolSociedadUsuarios { get; set; }
    }

    public class SociedadNavigationResponse
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public DateTime fechaAlta { get; set; }
        public string identificador { get; set; }
        public int tipoIdentificador { get; set; }
        public int cliente { get; set; }
        public string emailNotifcaciones { get; set; }
        public string emailProcesoFacturas { get; set; }
        public int flujoAprobacionFactura { get; set; }
        public int flujoAprobacionFacturaProforma { get; set; }
        public List<object> cecoSociedads { get; set; }
        public List<object> cecos { get; set; }
        public object clienteNavigation { get; set; }
        public List<object> facturas { get; set; }
        public object flujoAprobacionFacturaNavigation { get; set; }
        public object flujoAprobacionFacturaProformaNavigation { get; set; }
        public List<object> flujoAprobacionFacturas { get; set; }
        public List<object> loginProveedorSociedads { get; set; }
        public List<object> proyectos { get; set; }
        public List<object> rolSociedadUsuarios { get; set; }
    }
}
