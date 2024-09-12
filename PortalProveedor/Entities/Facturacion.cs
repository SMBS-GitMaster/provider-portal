using System;
using System.Collections.Generic;

namespace PortalProveedor.Entities;

public partial class Facturacion
{
    public int Id { get; set; }

    public int Cliente { get; set; }

    public string Dirección { get; set; } = null!;

    public string CodigoPostal { get; set; } = null!;

    public string Poblacion { get; set; } = null!;

    public string Pais { get; set; } = null!;
}
