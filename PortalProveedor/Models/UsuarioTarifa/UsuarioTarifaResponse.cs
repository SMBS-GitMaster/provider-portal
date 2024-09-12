using PortalProveedor.Models.Usuarios;

namespace PortalProveedor.Models.UsuarioTarifa
{
    public class UsuarioTarifaResponse
    {
        public int Id { get; set; }
        public int Usuario { get; set; }
        public decimal PrecioHora { get; set; }
        public string FechaInicia { get; set; }
        public string FechaVence { get; set; }
        public UsuarioResponse UsuarioNavigation { get; set; }
    }
}
