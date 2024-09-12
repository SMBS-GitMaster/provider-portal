namespace PortalProveedor.Models.Proyectos
{
    public class AltaProyectoRequest
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        //public int Aprobador { get; set; }
        public int Sociedad { get; set; }
        //public string TipoIdentificador { get; set; }
        public byte EstadoProyecto { get; set; }
        public int Responsable { get; set; }
    }
}
