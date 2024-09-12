using System;
using System.Collections.Generic;

namespace PortalProveedor.Entities;

/// <summary>
/// Son los Cecos (centros de coste) definidos por cada uno de los proveedores por cada sociedad
/// </summary>
public partial class Ceco
{
    public int Id { get; set; }

    public string Codigo { get; set; } = null!;

    public DateTime FechaAlta { get; set; }

    public int Sociedad { get; set; }

    public virtual ICollection<CecoSociedad> CecoSociedads { get; set; } = new List<CecoSociedad>();

    public virtual Sociedad SociedadNavigation { get; set; } = null!;
}
