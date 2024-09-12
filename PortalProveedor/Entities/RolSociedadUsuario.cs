using System;
using System.Collections.Generic;

namespace PortalProveedor.Entities;

/// <summary>
/// Tabla que establece el rol de un usuario en una sociedad de un cliente.
/// </summary>
public partial class RolSociedadUsuario
{
    public int Id { get; set; }

    public byte Rol { get; set; }

    public int Usuario { get; set; }

    public int Sociedad { get; set; }

    public virtual Rol RolNavigation { get; set; } = null!;

    public virtual Sociedad SociedadNavigation { get; set; } = null!;

    public virtual Usuario UsuarioNavigation { get; set; } = null!;
}
