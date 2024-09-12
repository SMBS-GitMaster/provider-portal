using System;
using System.Collections.Generic;

namespace PortalProveedor.Entities;

public partial class FacturaEstadoFactura
{
    public int Id { get; set; }

    public int Factura { get; set; }

    public byte EstadoFactura { get; set; }

    public int Usuario { get; set; }

    public DateTime FechaAlta { get; set; }

    public bool FacturaDelegada { get; set; }

    public string? Comentario { get; set; }

    public virtual EstadoFactura EstadoFacturaNavigation { get; set; } = null!;

    public virtual Factura FacturaNavigation { get; set; } = null!;
}
