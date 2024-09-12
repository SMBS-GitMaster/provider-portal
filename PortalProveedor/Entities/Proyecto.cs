using System;
using System.Collections.Generic;

namespace PortalProveedor.Entities;

/// <summary>
/// Proyectos definidos por sociedad
/// </summary>
public partial class Proyecto
{
    public int Id { get; set; }

    public string Codigo { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public DateTime FechaAlta { get; set; }

    public int Sociedad { get; set; }

    public byte EstadoProyecto { get; set; }

    public bool Borrado { get; set; }

    public int? FlujoAprobacionFactura { get; set; }

    public virtual ICollection<Aprobador> Aprobadors { get; set; } = new List<Aprobador>();

    public virtual EstadoProyecto EstadoProyectoNavigation { get; set; } = null!;

    public virtual ICollection<Factura> Facturas { get; set; } = new List<Factura>();

    public virtual FlujoAprobacionFactura? FlujoAprobacionFacturaNavigation { get; set; }
    public virtual Usuario? UsuarioNavigation { get; set; } = null!;

    public virtual ICollection<FlujoAprobacionFactura> FlujoAprobacionFacturas { get; set; } = new List<FlujoAprobacionFactura>();

    public virtual Sociedad SociedadNavigation { get; set; } = null!;

    public int? Responsable { get; set; }

    public virtual ICollection<ImputacionDetalle> ImputacionDetalle { get; set; } = new List<ImputacionDetalle>();
}
