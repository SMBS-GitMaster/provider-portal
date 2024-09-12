using System;
using System.Collections.Generic;

namespace PortalProveedor.Entities;

public partial class FlujoAprobacionFactura
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public int Sociedad { get; set; }

    public int? Proyecto { get; set; }

    public bool Predeterminado { get; set; }

    public bool Proforma { get; set; }

    public virtual ICollection<FlujoEstadoFactura> FlujoEstadoFacturas { get; set; } = new List<FlujoEstadoFactura>();

    public virtual Proyecto? ProyectoNavigation { get; set; }

    public virtual ICollection<Proyecto> Proyectos { get; set; } = new List<Proyecto>();

    public virtual ICollection<Sociedad> SociedadFlujoAprobacionFacturaNavigations { get; set; } = new List<Sociedad>();

    public virtual ICollection<Sociedad> SociedadFlujoAprobacionFacturaProformaNavigations { get; set; } = new List<Sociedad>();

    public virtual Sociedad SociedadNavigation { get; set; } = null!;
}
