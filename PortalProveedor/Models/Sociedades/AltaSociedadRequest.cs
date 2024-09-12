namespace PortalProveedor.Models.Sociedades
{
    public class AltaSociedadRequest
    {
        public string Nombre { get; set; }
        public string Identificador { get; set; }
        public byte TipoIdentificador { get; set; }
        public int Cliente { get; set; }
        public string EmailNotifcaciones { get; set; }
        public string EmailProcesoFacturas { get; set; }
    }
}
