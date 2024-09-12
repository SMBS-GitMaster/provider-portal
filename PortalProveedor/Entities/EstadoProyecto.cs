using System;
using System.Collections.Generic;

namespace PortalProveedor.Entities;

public partial class EstadoProyecto
{
    public byte Id { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Proyecto> Proyectos { get; set; } = new List<Proyecto>();
}
