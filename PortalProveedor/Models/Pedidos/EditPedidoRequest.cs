using PortalProveedor.Helpers;
using System.ComponentModel.DataAnnotations;

namespace PortalProveedor.Models.Pedidos
{
    public class EditPedidoRequest
    {

        [Required(ErrorMessage = "El Número es requerido")]
        public int NumeroPedido { get; set; }

        [Required(ErrorMessage = "La Sociedad es requerida")]
        public int Sociedad { get; set; }

        [Required(ErrorMessage = "El Proveedor es requerido")]
        public int Proveedor { get; set; }
        public int? Factura { get; set; }

        [AllowedExtensions(new string[] { ".pdf" })]
        public IFormFile? FicheroPedido { get; set; }
    }
}
