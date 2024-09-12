using PortalProveedor.Models.Proyectos;

namespace PortalProveedor.Models.Imputacion
{
    public class ImputacionDetalleResponse
    {
        public int Id { get; set; }
        public int Imputacion { get; set; }
        public decimal TotalHoras { get; set; }
        public decimal TotalCosto { get; set; }
        public ListaProyectoResponse Proyecto { get; set; }

    }
}
