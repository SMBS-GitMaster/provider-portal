namespace PortalProveedor.Models.Facturas
{
    public class EstadoFacturaResponse
    {
        public byte Id { get; set; }

        public string EstadoInterno { get; set; } = null!;

        public string EstadoProveedor { get; set; } = null!;

        public bool EstadoInicial { get; set; }

        public bool EstadoFinal { get; set; }
    }
}
