namespace PortalProveedor.Models.Facturas
{
    public class CambiarEstadoFacturaRequest
    {
        public int Id { get; set; }
        public byte Estado { get; set; }
        public int Proyecto { get; set; }
        public string Comentario { get; set; }
    }
}
