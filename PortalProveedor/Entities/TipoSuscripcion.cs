using System;
using System.Collections.Generic;

namespace PortalProveedor.Entities;

public partial class TipoSuscripcion
{
    public byte Id { get; set; }

    public string Nombre { get; set; } = null!;

    public int IdProductoStripe { get; set; }

    public int IdPrecioStripe { get; set; }

    public virtual ICollection<Suscripcion> Suscripcions { get; set; } = new List<Suscripcion>();
}
