using PortalProveedor.Entities;
using PortalProveedor.Helpers;
using System.ComponentModel.DataAnnotations;

namespace PortalProveedor.Models.Pedidos
{
    public class AltaPedidoRequest
    {
        [Required(ErrorMessage = "El Número es requerido")]
        public int NumeroPedido { get; set; }

        [Required(ErrorMessage = "La Sociedad es requerida")]
        public int Sociedad { get; set; }

        [Required(ErrorMessage = "El Proveedor es requerido")]
        public int Proveedor { get; set; }

        [Required(ErrorMessage = "El Fichero es requerido")]
        [AllowedExtensions(new string[] { ".pdf" })]
        public IFormFile FicheroPedido { get; set; }

        //public List<int> Facturas { get; set; }
    }

    public partial class FacturaPedidoRequest
    {
        public int Factura { get; set; }
    }
}
