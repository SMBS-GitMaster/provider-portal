namespace PortalProveedor.Models.Facturas
{
    public class ListaFacturaResponse
    {
        public int Id { get; set; }
        public string Proveedor { get; set; }
        public string Responsable { get; set; }
        public string? Estado { get; set; }
        public string FechaAlta { get; set; }
        public string Proyecto { get; set; }
        public string Sociedad { get; set; }
        public string FechaContable { get; set; }
        public string Numero { get; set; } = null!;
    }
}
