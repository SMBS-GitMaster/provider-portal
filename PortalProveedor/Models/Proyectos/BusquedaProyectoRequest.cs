namespace PortalProveedor.Models.Proyectos
{
    public class BusquedaProyectoRequest
    {
        public string? Codigo { get; set; }
        public string? Nombre { get; set; }
        public int Aprobador { get; set; }
        public byte? Estado { get; set; }
    }
}
