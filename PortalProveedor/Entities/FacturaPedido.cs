using System;
using System.Collections.Generic;

namespace PortalProveedor.Entities;

public partial class FacturaPedido
{
    public int Id { get; set; }

    public int Factura { get; set; }

    public int Pedido { get; set; }

    public DateTime FechaAlta { get; set; }

    public virtual Factura FacturaNavigation { get; set; } = null!;

    public virtual Pedido PedidoNavigation { get; set; } = null!;
}
