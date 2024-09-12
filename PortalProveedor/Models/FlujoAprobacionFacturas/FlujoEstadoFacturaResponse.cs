using PortalProveedor.Entities;
using PortalProveedor.Models.Facturas;

namespace PortalProveedor.Models.FlujoAprobacionFacturas
{
    public class FlujoEstadoFacturaResponse
    {
        public byte Id { get; set; }

        public string? NombreEstadoInterno { get; set; }

        public string? NombreEstadoExterno { get; set; }

        public bool WebProveedor { get; set; }

        public bool WebGestion { get; set; }

        public bool PermiteDelegacion { get; set; }

        public bool PermiteComentario { get; set; }

        public bool? ComentarioObligatorio { get; set; }

        public virtual UsuarioAprobadorResponse? AprobadorDelegadoNavigation { get; set; }

        public virtual UsuarioAprobadorResponse AprobadorNavigation { get; set; } = null!;

        public virtual UsuarioAprobadorResponse? AprobadorSecundarioNavigation { get; set; }

        public virtual EstadoFacturaResponse EstadoDestinoNavigation { get; set; }

        public virtual EstadoFacturaResponse EstadoOrigenNavigation { get; set; }
    }
}
