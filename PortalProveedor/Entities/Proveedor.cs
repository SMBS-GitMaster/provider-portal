using System;
using System.Collections.Generic;

namespace PortalProveedor.Entities;

/// <summary>
/// Son los proveedores de una sociedad.
/// </summary>
public partial class Proveedor
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public DateTime FechaAlta { get; set; }

    public string Identificador { get; set; } = null!;

    public byte TipoIdentificador { get; set; }

    public byte EstadoProveedor { get; set; }

    public virtual EstadoProveedor EstadoProveedorNavigation { get; set; } = null!;

    public virtual ICollection<Factura> Facturas { get; set; } = new List<Factura>();

    public virtual ICollection<LoginProveedorSociedad> LoginProveedorSociedads { get; set; } = new List<LoginProveedorSociedad>();

    public virtual ICollection<LoginProveedor> LoginProveedors { get; set; } = new List<LoginProveedor>();

    public virtual TipoIdentificador TipoIdentificadorNavigation { get; set; } = null!;
}
