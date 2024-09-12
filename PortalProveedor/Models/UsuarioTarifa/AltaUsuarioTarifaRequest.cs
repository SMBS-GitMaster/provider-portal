namespace PortalProveedor.Models.UsuarioTarifa
{
    public class AltaUsuarioTarifaRequest
    {
        public int Usuario { get; set; }
        public decimal PrecioHora { get; set; }
        public string FechaInicia { get; set; }
        public string FechaVence { get; set; }
    }
}
