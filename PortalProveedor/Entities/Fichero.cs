using System;
using System.Collections.Generic;

namespace PortalProveedor.Entities;

/// <summary>
/// Tabla que almacena los datos para localizar un fichero
/// </summary>
public partial class Fichero
{
    public int Id { get; set; }

    public DateTime FechaAlta { get; set; }

    public string Ruta { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Albaran> Albarans { get; set; } = new List<Albaran>();

    public virtual ICollection<Factura> FacturaFicheroFacturaNavigations { get; set; } = new List<Factura>();

    public virtual ICollection<Factura> FacturaFicheroFacturaProformaNavigations { get; set; } = new List<Factura>();

    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}
