using System;
using System.Collections.Generic;

namespace PortalProveedor.Entities;

/// <summary>
/// Tabla que almacena los datos para localizar un albarán
/// </summary>
public partial class Albaran
{
    public int Id { get; set; }

    public DateTime FechaAlta { get; set; }

    public int Factura { get; set; }

    public int FicheroAlbaran { get; set; }

    public virtual Factura FacturaNavigation { get; set; } = null!;

    public virtual Fichero FicheroAlbaranNavigation { get; set; } = null!;
}
