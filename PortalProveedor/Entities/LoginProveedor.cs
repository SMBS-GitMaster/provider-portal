using System;
using System.Collections.Generic;

namespace PortalProveedor.Entities;

public partial class LoginProveedor
{
    public int Id { get; set; }

    public DateTime FechaAlta { get; set; }

    public string Email { get; set; } = null!;

    public string Clave { get; set; } = null!;

    public int Proveedor { get; set; }

    public virtual ICollection<LoginProveedorSociedad> LoginProveedorSociedads { get; set; } = new List<LoginProveedorSociedad>();

    public virtual Proveedor ProveedorNavigation { get; set; } = null!;
}
