using System;
using System.Collections.Generic;

namespace PortalProveedor.Entities;

/// <summary>
/// Tabla para relacionar una sociedad con sus Cecos. 
/// </summary>
public partial class CecoSociedad
{
    public int Id { get; set; }

    public int Ceco { get; set; }

    public int Sociedad { get; set; }

    public virtual Ceco CecoNavigation { get; set; } = null!;

    public virtual Sociedad SociedadNavigation { get; set; } = null!;
}
