namespace PortalProveedor.Entities;

public partial class Pedido
{
    public int Id { get; set; }

    public int NumeroPedido { get; set; }

    public int Sociedad { get; set; }

    public int Proveedor { get; set; }

    public int FicheroPedido { get; set; }

    public DateTime? FechaAlta { get; set; }

    public virtual ICollection<FacturaPedido> FacturaPedidos { get; set; } = new List<FacturaPedido>();

    public virtual Fichero FicheroPedidoNavigation { get; set; } = null!;
}
