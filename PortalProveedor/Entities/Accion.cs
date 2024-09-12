using System;
using System.Collections.Generic;

namespace PortalProveedor.Entities;

public partial class Accion
{
    public byte Id { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<PermisoRolAccion> PermisoRolAccions { get; set; } = new List<PermisoRolAccion>();
}
