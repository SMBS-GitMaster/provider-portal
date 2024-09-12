using System;
using System.Collections.Generic;

namespace PortalProveedor.Entities;

/// <summary>
/// Tabla para relacionar los proveedores con sociedades
/// </summary>
public partial class LoginProveedorSociedad
{
    public int Id { get; set; }

    public int Proveedor { get; set; }

    public int Sociedad { get; set; }

    public int LoginProveedor { get; set; }

    public virtual LoginProveedor LoginProveedorNavigation { get; set; } = null!;

    public virtual Proveedor ProveedorNavigation { get; set; } = null!;

    public virtual Sociedad SociedadNavigation { get; set; } = null!;
}
