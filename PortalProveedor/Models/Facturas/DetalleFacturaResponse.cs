using static PortalProveedor.Models.Dtos.Dtos;

namespace PortalProveedor.Models.Facturas
{
    public class DetalleFacturaResponse
    {
        public int Id { get; set; }
        public string Proveedor { get; set; }
        public SociedadDto Sociedad { get; set; }
        public string Proyecto { get; set; }
        public string Responsable { get; set; }
        public EstadoDto? Estado { get; set; }
        public string FechaAlta { get; set; }
        public string FechaFactura { get; set; }
        public string Numero { get; set; } = null!;
        public string Importe { get; set; }
        public string Comentario { get; set; }
    }

}
