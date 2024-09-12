namespace PortalProveedor.Entities
{
    public class UsuarioTarifa
    {
        public int Id { get; set; }

        public int Usuario { get; set; }

        public decimal PrecioHora { get; set; }

        public DateTime FechaInicia { get; set; }

        public DateTime FechaVence { get; set; }

        public DateTime FechaAlta { get; set; } = DateTime.Now;

        public bool Borrado { get; set; }

        public virtual Usuario UsuarioNavigation { get; set; } = null!;
    }
}
