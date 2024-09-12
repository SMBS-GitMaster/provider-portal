using System;
using System.Collections.Generic;

namespace PortalProveedor.Entities;

public partial class FlujoEstadoFactura
{
    public byte Id { get; set; }

    public int FlujoAprobacionFactura { get; set; }

    public string? NombreEstadoInterno { get; set; }

    public string? NombreEstadoExterno { get; set; }

    public byte EstadoOrigen { get; set; }

    public byte EstadoDestino { get; set; }

    public bool WebProveedor { get; set; }

    public bool WebGestion { get; set; }

    public int Aprobador { get; set; }

    public int? AprobadorSecundario { get; set; }

    public bool PermiteDelegacion { get; set; }

    public int? AprobadorDelegado { get; set; }

    public bool PermiteComentario { get; set; }

    public bool? ComentarioObligatorio { get; set; }
    public string? EtiquetaBotonEstadoDestino { get; set; }
    public string? EtiquetaBotonDelegar { get; set; }
    public string? EtiquetaCampoComentario { get; set; }

    public virtual Usuario? AprobadorDelegadoNavigation { get; set; }

    public virtual Usuario AprobadorNavigation { get; set; } = null!;

    public virtual Usuario? AprobadorSecundarioNavigation { get; set; }

    public virtual EstadoFactura EstadoDestinoNavigation { get; set; } = null!;

    public virtual EstadoFactura EstadoOrigenNavigation { get; set; } = null!;

    public virtual FlujoAprobacionFactura FlujoAprobacionFacturaNavigation { get; set; } = null!;
}
