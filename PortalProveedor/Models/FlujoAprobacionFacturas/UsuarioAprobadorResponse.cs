namespace PortalProveedor.Models.FlujoAprobacionFacturas
{
    public class UsuarioAprobadorResponse
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public bool Ausente { get; set; }
        public int EstadoId { get; set; }
        public string Estado { get; set; }
    }
}
