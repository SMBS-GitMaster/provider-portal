namespace PortalProveedor.Models.Facturas
{
    public class FacturaResponse
    {
        public int Id { get; set; }
        public string Numero { get; set; } = null!;
        public decimal? Importe { get; set; }
        public string Proveedor { get; set; }
        public string Responsable { get; set; }
        public int ResponsableId { get; set; }
        public bool PermisoResponsable { get; set; }
        public int ProyectoId { get; set; }
        public string CodigoProyecto { get; set;}
        public string Proyecto { get; set; }
        public int SociedadId { get; set; }
        public string Sociedad { get; set; }
        public FicheroResponse? FicheroFactura { get; set; }
        public FicheroResponse? FicheroFacturaProforma { get; set; }
        public ICollection<FicheroResponse> FicherosAlbaran { get; set; }
        public bool? DescuentoProntoPago { get; set; }
        public byte EstadoId { get; set; }
        public string EstadoInterno { get; set; }
        public string EstadoProveedor { get; set; }
        public bool EstadoInicial { get; set; }
        public bool EstadoFinal { get; set; }
        public int FlujoAprobacionFacturaId { get; set; }
        public int FlujoAprobacionFacturaProformaId { get; set; }
        public bool IsEditable { get; set; }
        public string FechaContable { get; set; }
        public string FechaVencimiento { get; set; }
        public string FechaAlta { get; set; }
    }
}
