using System;
using System.Collections.Generic;

namespace PortalProveedor.Entities;

public partial class TipoIdentificador
{
    public byte Id { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Proveedor> Proveedors { get; set; } = new List<Proveedor>();
}
