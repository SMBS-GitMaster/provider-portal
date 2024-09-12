namespace PortalProveedor.Entities
{
    /// <summary>
    /// Imputacion de horas para proyectos
    /// </summary>
    public partial class Imputacion
    {
        public int Id { get; set; }

        public int Usuario { get; set; }

        public string Mes { get; set; }
        
        public decimal TotalHoras { get; set; }

        public decimal TotalCosto { get; set; }

        public string ImputacionHoras { get; set; }

        public DateTime FechaAlta { get; set; } = DateTime.Now;

        public virtual ICollection<ImputacionDetalle> ImputacionDetalle { get; set; } = new List<ImputacionDetalle>();

        public virtual Usuario UsuarioNavigation { get; set; } = null!;

    }
}
