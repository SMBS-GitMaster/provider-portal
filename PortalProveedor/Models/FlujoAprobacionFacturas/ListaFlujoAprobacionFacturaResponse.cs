using PortalProveedor.Entities;

namespace PortalProveedor.Models.FlujoAprobacionFacturas
{
    public class ListaFlujoAprobacionFacturaResponse
    {
        public int Id { get; set; }
        public string Nombre { get; set; }

        public int SociedadId { get; set; }

        public string Sociedad { get; set; }

        public int? ProyectoId { get; set; }

        public string Proyecto { get; set; }

        public bool Predeterminado { get; set; }

        public bool Proforma { get; set; }

        public virtual ICollection<FlujoEstadoFacturaResponse> FlujoEstadoFacturas { get; set; }

        public virtual Proyecto? ProyectoNavigation { get; set; }

        public virtual ICollection<Proyecto> Proyectos { get; set; }

        public virtual ICollection<Sociedad> SociedadFlujoAprobacionFacturaNavigations { get; set; }

        public virtual ICollection<Sociedad> SociedadFlujoAprobacionFacturaProformaNavigations { get; set; }

        public virtual Sociedad SociedadNavigation { get; set; }
    }
}
