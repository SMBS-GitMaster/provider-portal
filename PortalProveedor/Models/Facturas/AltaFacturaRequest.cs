using PortalProveedor.Entities;
using PortalProveedor.Helpers;
using System.ComponentModel.DataAnnotations;

namespace PortalProveedor.Models.Facturas
{
    public class AltaFacturaRequest
    {
        [Required(ErrorMessage = "El Número es requerido")]
        public string Numero { get; set; }

        public decimal? Importe { get; set; }

        [Required(ErrorMessage = "La Sociedad es requerida")]
        public int Sociedad { get; set; }

        [Required(ErrorMessage = "El Proyecto es requerido")]
        public int Proyecto { get; set; }

        [Required(ErrorMessage = "El Proveedor es requerido")]
        public int Proveedor { get; set; }

        [AllowedExtensions(new string[] { ".pdf" })]
        public IFormFile? FicheroFactura { get; set; }

        [AllowedExtensions(new string[] { ".pdf" })]
        public IFormFile? FicheroFacturaProforma { get; set; }

        [AllowedExtensions(new string[] { ".pdf" })]
        public List<IFormFile>? FicheroAlbaran { get; set; }        

        public bool? DescuentoProntoPago { get; set; }

        public DateTime? FechaVencimiento { get; set; }

        public DateTime? FechaFactura { get; set; }
        public DateTime? FechaFacturaProforma { get; set; }

        public List<int>? Pedidos { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (FechaFactura == null && FechaFacturaProforma == null)
            {
                yield return new ValidationResult("La fecha de factura es requerida.", new[] { nameof(FechaFactura), nameof(FechaFacturaProforma) });
            }

            if (FechaFactura == null)
            {
                // Skip further validation if FechaFactura is null
                yield break;
            }

            if (Importe is null)
            {
                yield return new ValidationResult("El Importe es requerido", new[] { nameof(Importe) });
            }

            if (DescuentoProntoPago is null)
            {
                yield return new ValidationResult("El Descuento Pronto Pago es requerido", new[] { nameof(DescuentoProntoPago) });
            }

            if (FechaVencimiento is null)
            {
                yield return new ValidationResult("La Fecha de Vencimiento es requerida", new[] { nameof(FechaVencimiento) });
            }
        }
    }
}
