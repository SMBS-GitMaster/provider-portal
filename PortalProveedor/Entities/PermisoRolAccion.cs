using System;
using System.Collections.Generic;

namespace PortalProveedor.Entities;

public partial class PermisoRolAccion
{
    public byte Id { get; set; }

    public byte Rol { get; set; }

    public byte Accion { get; set; }

    public bool Concedido { get; set; }

    public virtual Accion AccionNavigation { get; set; } = null!;

    public virtual Rol RolNavigation { get; set; } = null!;
}
