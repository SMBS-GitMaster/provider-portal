using System;

namespace PortalProveedor.Models.Facturas
{
    public class FicheroResponse
    {
        public int Id { get; set; }
        public string Ruta { get; set; }
        public string Nombre { get; set; }
        public DateTime FechaAlta { get; set; }
    }

    public class BlobDto
    {
        public string Name { get; set; }
        public string ContentType { get; set; }
        public Stream Content { get; set; }
    }
}
