using System;
using System.Collections.Generic;

namespace PortalProveedor.Entities;

/// <summary>
/// Usuario interno que tiene un cliente y que tiene acceso a la plataforma para realizar las tareas que tenga encomendadas
/// 
/// Un cliente normalmente tendrá varios usuarios en la plataforma con distintos roles.
/// </summary>
public partial class Usuario
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public DateTime FechaAlta { get; set; }

    public string Email { get; set; } = null!;

    public string Clave { get; set; } = null!;

    public int Cliente { get; set; }

    public byte Estado { get; set; }

    public bool Ausente { get; set; }

    public virtual ICollection<Aprobador> Aprobadors { get; set; } = new List<Aprobador>();

    public virtual Cliente ClienteNavigation { get; set; } = null!;

    public virtual EstadoUsuario EstadoNavigation { get; set; } = null!;

    public virtual ICollection<Factura> Facturas { get; set; } = new List<Factura>();

    public virtual ICollection<FlujoEstadoFactura> FlujoEstadoFacturaAprobadorDelegadoNavigations { get; set; } = new List<FlujoEstadoFactura>();

    public virtual ICollection<FlujoEstadoFactura> FlujoEstadoFacturaAprobadorNavigations { get; set; } = new List<FlujoEstadoFactura>();

    public virtual ICollection<FlujoEstadoFactura> FlujoEstadoFacturaAprobadorSecundarioNavigations { get; set; } = new List<FlujoEstadoFactura>();

    public virtual ICollection<RolSociedadUsuario> RolSociedadUsuarios { get; set; } = new List<RolSociedadUsuario>();

    public virtual ICollection<Proyecto> Proyectos { get; set; } = new List<Proyecto>();

    public virtual ICollection<UsuarioTarifa> Tarifas { get; set; } = new List<UsuarioTarifa>();
    public virtual ICollection<Imputacion> Imputaciones { get; set; } = new List<Imputacion>();
}
