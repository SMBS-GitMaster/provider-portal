using System;
using System.Collections.Generic;

namespace PortalProveedor.Entities;

/// <summary>
/// Tabla auxiliar para los posibles estados de una factura, en principio se establecen los siguientes: Pendiente de cumplimentar, pendiente de aprobar, aprobada, pagada, retenida y rechazada
/// </summary>
public partial class EstadoFactura
{
    public byte Id { get; set; }

    public string EstadoInterno { get; set; } = null!;

    public string EstadoProveedor { get; set; } = null!;

    public bool EstadoInicial { get; set; }

    public bool EstadoFinal { get; set; }

    public virtual ICollection<FacturaEstadoFactura> FacturaEstadoFacturas { get; set; } = new List<FacturaEstadoFactura>();

    public virtual ICollection<FlujoEstadoFactura> FlujoEstadoFacturaEstadoDestinoNavigations { get; set; } = new List<FlujoEstadoFactura>();

    public virtual ICollection<FlujoEstadoFactura> FlujoEstadoFacturaEstadoOrigenNavigations { get; set; } = new List<FlujoEstadoFactura>();
}
