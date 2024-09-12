using System.ComponentModel.DataAnnotations;

namespace PortalProveedor.Models.Imputacion
{
    public class AltaImputacionDetalleRequest
    {
        [Required(ErrorMessage = "El Proyecto es requerido")]
        public int Proyecto { get; set; }
        [Required(ErrorMessage = "Horas es requerido")]
        public List<decimal> horas { get; set; }
    }
}
