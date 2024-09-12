using System.ComponentModel.DataAnnotations;

namespace PortalProveedor.Models.Proveedores
{
    public class EditProveedorRequest
    {
        [Required(ErrorMessage = "El Nombre es requerido")]
        public string Nombre { get; set; }


        [Required(ErrorMessage = "El Identificador es requerido")]
        public string Identificador { get; set; }


        [Required(ErrorMessage = "El Tipo de Identificador es requerido")]
        public byte TipoIdentificador { get; set; }


        [Required(ErrorMessage = "El Estado es requerido")]
        public byte Estado { get; set; }


        [Required(ErrorMessage = "La Sociedad es requerida")]
        public int[] Sociedades { get; set; }

        public ICollection<LoginProveedorRequest>? LoginProveedor { get; set; }
    }
}
