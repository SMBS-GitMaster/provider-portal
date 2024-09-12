using PortalProveedor.Models.Usuarios;

namespace PortalProveedor.Models.Imputacion
{
    public class ImputacionResponse
    {
        public int Id { get; set; }
        public int Usuario { get; set; }
        public string Mes { get; set; }
        public decimal TotalHoras { get; set; }
        public decimal TotalCosto { get; set; }
        public string ImputacionHoras { get; set; }
        public DateTime FechaAlta { get; set; }
        public ICollection<ImputacionDetalleResponse> ImputacionDetalle { get; set; }
        public UsuarioResponse UsuarioNavigation { get; set; }
    }
}
