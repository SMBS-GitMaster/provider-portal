using PortalProveedor.Entities;

namespace PortalProveedor.Models.Proyectos
{
    public class ListaProyectoResponse
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Sociedad { get; set; }
        public string Estado { get; set; }
        public int? Responsable { get; set; }
    }
}
