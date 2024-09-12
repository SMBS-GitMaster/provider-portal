using System;
using System.Collections.Generic;

namespace PortalProveedor.Entities;

public partial class MedioPago
{
    public byte Id { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Suscripcion> Suscripcions { get; set; } = new List<Suscripcion>();
}
