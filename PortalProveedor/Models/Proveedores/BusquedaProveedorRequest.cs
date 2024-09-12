namespace PortalProveedor.Models.Proveedores
{
    public class BusquedaProveedorRequest
    {
        public string? Nombre { get; set; }
        public string? Identificador { get; set; }
        public byte? TipoIdentificador { get; set; }
        public byte? Estado { get; set; }
        public string? Email { get; set; }
    }
}
