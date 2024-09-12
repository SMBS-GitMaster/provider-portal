using System;
using System.Collections.Generic;

namespace PortalProveedor.Entities;

/// <summary>
/// Son las sociedades que un cliente tiene dadas de alta, es decir, un cliente puede tener distintas empresas que gestione el portal.
/// </summary>
public partial class Sociedad
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public DateTime FechaAlta { get; set; }

    public string Identificador { get; set; } = null!;

    public byte TipoIdentificador { get; set; }

    public int Cliente { get; set; }

    public string EmailNotifcaciones { get; set; } = null!;

    public string EmailProcesoFacturas { get; set; } = null!;

    public int FlujoAprobacionFactura { get; set; }

    public int FlujoAprobacionFacturaProforma { get; set; }

    public virtual ICollection<CecoSociedad> CecoSociedads { get; set; } = new List<CecoSociedad>();

    public virtual ICollection<Ceco> Cecos { get; set; } = new List<Ceco>();

    public virtual Cliente ClienteNavigation { get; set; } = null!;

    public virtual ICollection<Factura> Facturas { get; set; } = new List<Factura>();

    public virtual FlujoAprobacionFactura FlujoAprobacionFacturaNavigation { get; set; } = null!;

    public virtual FlujoAprobacionFactura FlujoAprobacionFacturaProformaNavigation { get; set; } = null!;

    public virtual ICollection<FlujoAprobacionFactura> FlujoAprobacionFacturas { get; set; } = new List<FlujoAprobacionFactura>();

    public virtual ICollection<LoginProveedorSociedad> LoginProveedorSociedads { get; set; } = new List<LoginProveedorSociedad>();

    public virtual ICollection<Proyecto> Proyectos { get; set; } = new List<Proyecto>();

    public virtual ICollection<RolSociedadUsuario> RolSociedadUsuarios { get; set; } = new List<RolSociedadUsuario>();
}
