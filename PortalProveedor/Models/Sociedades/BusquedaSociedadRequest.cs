namespace PortalProveedor.Models.Sociedades
{
    public class BusquedaSociedadRequest
    {
        public int? Id { get; set; }
        public string? Nombre { get; set; }
        public string? Identificador { get; set; }
        public byte? TipoIdentificador { get; set; }
        public string? EmailNotifcaciones { get; set; }
        public string? EmailProcesoFacturas { get; set; }
        public int? Cliente { get; set; }
        public int? Proveedor { get; set; }
    }
}
