using PortalProveedor.Entities;
using static PortalProveedor.Models.Dtos.Dtos;

namespace PortalProveedor.Models.Facturas
{
    public class FiltrosBusquedaResponse
    {
        public List<int> Sociedades { get; set; }
        public List<int> Proyectos { get; set; }
        public List<int> Proveedores { get; set; }
        public List<int> Estados { get; set; }
        public List<int> ResponsablesActuales { get; set; }
        public List<int> Years { get; set; }
    }
}
