namespace PortalProveedor.Entities
{
    /// <summary>
    /// Detalle de imputacion
    /// </summary>
    public partial class ImputacionDetalle
    {
        public int Id { get; set; }

        public int Imputacion { get; set; }

        public int Proyecto { get; set; }

        public decimal TotalHoras { get; set; }

        public decimal TotalCosto { get; set; }

        public virtual Imputacion ImputacionNavigation { get; set; } = null!;

        public virtual Proyecto ProyectoNavigation { get; set; } = null!;
    }
}
