using System;
using System.Collections.Generic;

namespace PortalProveedor.Entities;

public partial class Aprobador
{
    public int Id { get; set; }

    public int Proyecto { get; set; }

    public int Aprobador1 { get; set; }

    public int Orden { get; set; }

    public virtual Usuario Aprobador1Navigation { get; set; } = null!;

    public virtual Proyecto ProyectoNavigation { get; set; } = null!;
}
