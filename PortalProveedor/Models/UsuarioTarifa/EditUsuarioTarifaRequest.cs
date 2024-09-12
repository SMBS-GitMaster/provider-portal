namespace PortalProveedor.Models.UsuarioTarifa
{
    public class EditUsuarioTarifaRequest
    {
        public int Usuario { get; set; }
        public decimal PrecioHora { get; set; }
        public string FechaInicia { get; set; }
        public string FechaVence { get; set; }
    }
}
