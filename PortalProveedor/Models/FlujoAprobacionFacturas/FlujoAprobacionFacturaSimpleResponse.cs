using PortalProveedor.Entities;

namespace PortalProveedor.Models.FlujoAprobacionFacturas
{
    public class FlujoAprobacionFacturaSimpleResponse
    {
        public int flujoAprobacionFacturaId { get; set; }
        public string flujoAprobacionFactura { get; set; }
        public bool flujoAprobacionFacturaPredeterminado { get; set; }
        public bool flujoAprobacionFacturaProforma { get; set; }
        public byte idOrigen { get; set; }
        public string estadoOrigenInterno { get; set; }
        public string estadoOrigenProveedor { get; set; }
        public bool estadoOrigenInicial { get; set; }
        public bool estadoOrigenFinal { get; set; }
        public byte idDestino { get; set; }
        public string estadoDestinoInterno { get; set; }
        public string estadoDestinoProveedor { get; set; }
        public bool estadoDestinoInicial { get; set; }
        public bool estadoDestinoFinal { get; set; }
        public int aprobadorId { get; set; }
        public string aprobador { get; set; }
        public int? aprobadorSecundarioId { get; set; }
        public string aprobadorSecundario { get; set; }
        public int? aprobadorDelegadoId { get; set; }
        public string aprobadorDelegado { get; set; }
        public bool permiteDelegacion { get; set; }
        public bool permiteComentario { get; set; }
        public bool? comentarioObligatorio { get; set; }
        public string? etiquetaBotonEstadoDestino { get; set; }
        public string? etiquetaBotonDelegar { get; set; }
        public string? etiquetaCampoComentario { get; set; }
    }
}
