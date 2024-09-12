using System;
using System.Collections.Generic;

namespace PortalProveedor.Entities;

/// <summary>
/// Tabla auxiliar para los posibles estados de un usuario, en principio se establecen los siguientes: Activo e inactivo
/// </summary>
public partial class EstadoUsuario
{
    public byte Id { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
