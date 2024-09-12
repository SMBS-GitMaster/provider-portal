namespace PortalProveedor.Models.Sociedades
{
    public class ListaSociedadResponse
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Identificador { get; set; }
        public byte TipoIdentificador { get; set; }
        public string EmailNotifcaciones { get; set; }
        public string EmailProcesoFacturas { get; set; }
        public int ClienteId { get; set; }
        public string Cliente { get; set; }
    }
}
