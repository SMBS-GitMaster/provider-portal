using PortalProveedor.Entities;
using static PortalProveedor.Models.Dtos.Dtos;

namespace PortalProveedor.Models.Facturas
{
    public class AltaProveedorFormularioResponse
    {
        public List<SociedadDto> Sociedades { get; set; }
        public List<EstadoDto> Estados { get; set; }
    }
}
