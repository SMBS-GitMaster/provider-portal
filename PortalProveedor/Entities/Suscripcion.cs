using System;
using System.Collections.Generic;

namespace PortalProveedor.Entities;

public partial class Suscripcion
{
    public int Id { get; set; }

    public int Cliente { get; set; }

    public byte TipoSuscripcion { get; set; }

    public byte MedioPago { get; set; }

    public byte EstadoSuscripcion { get; set; }

    public DateTime ComienzoPeriodoActual { get; set; }

    public DateTime FinPeriodoActual { get; set; }

    public bool CancelarFinPeriodoActual { get; set; }

    public DateTime FechaPrevistaCancelacion { get; set; }

    public DateTime FechaCancelacion { get; set; }

    public DateTime FechaFin { get; set; }

    public int IdSuscripcionStripe { get; set; }

    public int IdClienteStripe { get; set; }

    public virtual EstadoSuscripcion EstadoSuscripcionNavigation { get; set; } = null!;

    public virtual MedioPago MedioPagoNavigation { get; set; } = null!;

    public virtual TipoSuscripcion TipoSuscripcionNavigation { get; set; } = null!;
}
