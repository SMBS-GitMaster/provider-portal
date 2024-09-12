using System;
using System.Collections.Generic;

namespace PortalProveedor.Entities;

/// <summary>
/// Tabla de facturas
/// </summary>
public partial class Factura
{
    public int Id { get; set; }

    public int Sociedad { get; set; }

    public DateTime FechaAlta { get; set; }

    public DateTime? FechaFactura { get; set; }

    public DateTime? FechaFacturaProforma { get; set; }

    public string? Numero { get; set; }

    public decimal? Importe { get; set; }

    public int Proyecto { get; set; }

    public int? FicheroFactura { get; set; }

    public int? FicheroFacturaProforma { get; set; }

    public int Proveedor { get; set; }

    public int ResponsableAprobar { get; set; }

    public DateTime? FechaVencimiento { get; set; }

    public bool? DescuentoProntoPago { get; set; }

    public virtual ICollection<Albaran> Albarans { get; set; } = new List<Albaran>();

    public virtual ICollection<FacturaEstadoFactura> FacturaEstadoFacturas { get; set; } = new List<FacturaEstadoFactura>();

    public virtual ICollection<FacturaPedido> FacturaPedidos { get; set; } = new List<FacturaPedido>();

    public virtual Fichero? FicheroFacturaNavigation { get; set; }

    public virtual Fichero? FicheroFacturaProformaNavigation { get; set; }

    public virtual Proveedor ProveedorNavigation { get; set; } = null!;

    public virtual Proyecto ProyectoNavigation { get; set; } = null!;

    public virtual Usuario ResponsableAprobarNavigation { get; set; } = null!;

    public virtual Sociedad SociedadNavigation { get; set; } = null!;
}
