using PortalProveedor.Models.Usuarios;

namespace PortalProveedor.Models.Imputacion
{
    public class ListaImputacionResponse
    {
        public int id { get; set; }
        public int Usuario { get; set; }
        public string Mes { get; set; }
        public decimal TotalHoras { get; set; }
        public decimal TotalCosto { get; set; }
        public DateTime FechaAlta { get; set; }
        public UsuarioResponse UsuarioNavigation { get; set; }
    }
}
