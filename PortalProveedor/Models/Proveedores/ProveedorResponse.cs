using PortalProveedor.Models.LoginProveedores;
using PortalProveedor.Models.LoginProveedoresSociedades;

namespace PortalProveedor.Models.Proveedores
{
    public class ProveedorResponse
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Identificador { get; set; }
        public byte TipoIdentificadorId { get; set; }
        public string TipoIdentificador { get; set; }
        public byte EstadoId { get; set; }
        public string Estado { get; set; }
        public ICollection<LoginProveedorResponse> LoginProveedores { get; set; }
        public ICollection<LoginProveedorSociedadResponse> LoginProveedorSociedades { get; set; }
    }
}
