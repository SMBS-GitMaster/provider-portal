using System.ComponentModel.DataAnnotations;

namespace PortalProveedor.Models.Imputacion
{
    public class AltaImputacionRequest
    {
        [Required(ErrorMessage = "El Mes es requerido")]
        public string Mes { get; set; }
        [Required(ErrorMessage = "ImputacionHoras es requerido")]
        public string ImputacionHoras { get; set; }
        [Required(ErrorMessage = "ImputacionDetalle es requerido")]
        public List<AltaImputacionDetalleRequest> ImputacionDetalle { get; set; }
    }
}
