using static PortalProveedor.Models.Dtos.Dtos;

namespace PortalProveedor.Models.Proyectos
{
    public class AltaProyectoFormularioResponse
    {
        public List<SociedadDto> Sociedades { get; set; }
        public List<EstadoDto> Estados { get; set; }
        public List<UsuarioDto> Aprobadores { get; set; }
    }
}
