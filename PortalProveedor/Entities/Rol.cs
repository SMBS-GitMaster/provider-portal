using System;
using System.Collections.Generic;

namespace PortalProveedor.Entities;

/// <summary>
/// Roles disponibles de usuario por cada sociedad que tenga un cliente, en principio se establecen dos: Administrdor y usuario interno. 
/// </summary>
public partial class Rol
{
    public byte Id { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<PermisoRolAccion> PermisoRolAccions { get; set; } = new List<PermisoRolAccion>();

    public virtual ICollection<RolSociedadUsuario> RolSociedadUsuarios { get; set; } = new List<RolSociedadUsuario>();
}
