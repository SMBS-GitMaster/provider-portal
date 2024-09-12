using PortalProveedor.Models.Facturas;

namespace PortalProveedor.Models.Pedidos
{
    public class PedidoResponse
    {
        public int Id { get; set; }

        public int NumeroPedido { get; set; }

        public int Sociedad { get; set; }

        public string SociedadName { get; set; }

        public int Proveedor { get; set; }

        public string ProveedorName { get; set; }

        public ICollection<FacturaPedidoResponse> FacturaPedidos { get; set; }

        public FicheroResponse FicheroPedido { get; set; }

        public string FechaAlta { get; set; }
    }
}
