namespace PortalProveedor.Models.Dtos
{
    public class Dtos
    {
        public class EstadoDto
        {
            public int Id { get; set; }
            public string Nombre { get; set; }
        }

        public class SociedadDto
        {
            public int Id { get; set; }
            public string Nombre { get; set; }
        }

        public class ResponsableDto
        {
            public int Id { get; set; }
            public string Nombre { get; set; }
        }
        
        public class ProveedorDto
        {
            public int Id { get; set; }
            public string Nombre { get; set; }
        }

        public class ProyectoDto
        {
            public int Id { get; set; }
            public string Nombre { get; set; }
        }

        public class UsuarioDto
        {
            public int Id { get; set; }
            public string Nombre { get; set; }
        }
    }
}
