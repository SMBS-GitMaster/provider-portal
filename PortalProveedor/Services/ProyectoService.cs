namespace PortalProveedor.Services;

using Microsoft.EntityFrameworkCore;
using PortalProveedor.Database;
using PortalProveedor.Entities;
using PortalProveedor.Helpers;
using PortalProveedor.Models.Dtos;
using PortalProveedor.Models.Proyectos;
using PortalProveedor.Models.Sociedades;
using static PortalProveedor.Models.Dtos.Dtos;

public interface IProyectoService
{
    public Task AltaProyecto(int usr, AltaProyectoRequest request);
    public Task ActualizarProyecto(int id, int usr, AltaProyectoRequest dto);
    public Task EliminarProyecto(int id, int usr);
    public IEnumerable<ListaProyectoResponse> GetProyectos(int usr, IEnumerable<int> sociedad, BusquedaProyectoRequest request);
    public IEnumerable<ListaProyectoResponse> GetProyectosBySociedad(int sociedad);
    public IEnumerable<ListaProyectoResponse> GetProyectosByUsuario(int usuario);
    IEnumerable<ListaEstadoProyectoResponse> GetProyectosEstados(int usr, IEnumerable<int> sociedadusr);
}

public class ProyectoService : IProyectoService
{
    private PortalProveedorContext _context;
    private ISociedadService _sociedadService;
    private IUsuarioService _usuarioService;

    public ProyectoService(PortalProveedorContext context, ISociedadService sociedadService, IUsuarioService usuarioService)
    {
        _context = context;
        _sociedadService = sociedadService;
        _usuarioService = usuarioService;
    }
    public IEnumerable<ListaProyectoResponse> GetProyectos(int usr, IEnumerable<int> sociedad, BusquedaProyectoRequest request)
    {
        var proyectos = _context.Proyectos
            .Include(i => i.SociedadNavigation).ThenInclude(ii => ii.FlujoAprobacionFacturas).ThenInclude(iii => iii.FlujoEstadoFacturas)
            .Include(i => i.EstadoProyectoNavigation)
            .Include(i => i.FlujoAprobacionFacturas).ThenInclude(iii => iii.FlujoEstadoFacturas)
            .Where(w => sociedad.Contains(w.Sociedad)
            && (w.FlujoAprobacionFacturas.Any(a => a.FlujoEstadoFacturas.Any(aa => aa.Aprobador == request.Aprobador)) || 
            w.SociedadNavigation.FlujoAprobacionFacturas.Any(a => a.FlujoEstadoFacturas.Any(aa => aa.Aprobador == request.Aprobador)))
            && (string.IsNullOrEmpty(request.Codigo) || w.Codigo == request.Codigo)
            && (string.IsNullOrEmpty(request.Nombre) || w.Codigo == request.Nombre)
            && (!request.Estado.HasValue || w.EstadoProyecto == request.Estado)
            && (!w.Borrado)).ToList();
        
        List<ListaProyectoResponse> listaProyectosResponse = new();
        foreach (Proyecto proyecto in proyectos)
        {
            listaProyectosResponse.Add(new ListaProyectoResponse
            {
                Id = proyecto.Id,
                Codigo = proyecto.Codigo,
                Nombre = proyecto.Nombre,
                Sociedad = proyecto.SociedadNavigation.Nombre,
                //Aprobador = proyecto.FlujoAprobacionFacturas.FirstOrDefault(,
                Estado = proyecto.EstadoProyectoNavigation.Nombre,
                Responsable = proyecto.Responsable
            });

        }
        return listaProyectosResponse;
    }
    public IEnumerable<ListaProyectoResponse> GetProyectosBySociedad(int sociedad)
    {
        var proyectos = _context.Proyectos
            .Include(i => i.SociedadNavigation)
            .Include(i => i.EstadoProyectoNavigation)
            .Include(i => i.FlujoAprobacionFacturas)
            .Where(x => x.Sociedad == sociedad);
        
        List<ListaProyectoResponse> listaProyectosResponse = new();
        foreach (Proyecto proyecto in proyectos)
        {
            listaProyectosResponse.Add(new ListaProyectoResponse
            {
                Id = proyecto.Id,
                Codigo = proyecto.Codigo,
                Nombre = proyecto.Nombre,
                Sociedad = proyecto.SociedadNavigation.Nombre,
                //Aprobador = proyecto.AprobadorNavigation.Nombre,                
                Estado = proyecto.EstadoProyectoNavigation.Nombre,
                Responsable = proyecto.Responsable
            });

        }
        return listaProyectosResponse;

    }

    public IEnumerable<ListaProyectoResponse> GetProyectosByUsuario(int usuario)
    {
        var sociedadesUsuario = _sociedadService.GetSociedadesByUsuario(usuario);
        var proyectosFactura = _context.Facturas
            .Include(i => i.ProveedorNavigation)
            .Include(i => i.SociedadNavigation)
            .Include(i => i.ProyectoNavigation)
            .Include(i => i.ResponsableAprobarNavigation)
            .Where(w => (sociedadesUsuario.Contains(w.Sociedad)))
            .Select(e => e.Proyecto).Distinct();
        var proyectos = _context.Proyectos
            .Include(i => i.SociedadNavigation).ThenInclude(i => i.RolSociedadUsuarios).ThenInclude(i => i.UsuarioNavigation)
            .Include(i => i.EstadoProyectoNavigation)
            .Include(i => i.FlujoAprobacionFacturas)
            .Where(x => x.SociedadNavigation.RolSociedadUsuarios.Any(a => a.Usuario == usuario) && proyectosFactura.Contains(x.Id));

        List<ListaProyectoResponse> listaProyectosResponse = new();
        foreach (Proyecto proyecto in proyectos)
        {
            listaProyectosResponse.Add(new ListaProyectoResponse
            {
                Id = proyecto.Id,
                Codigo = proyecto.Codigo,
                Nombre = proyecto.Nombre,
                Sociedad = proyecto.SociedadNavigation.Nombre,
                //Aprobador = proyecto.AprobadorNavigation.Nombre,                
                Estado = proyecto.EstadoProyectoNavigation.Nombre,
                Responsable = proyecto.Responsable
            });

        }
        return listaProyectosResponse;

    }
    public IEnumerable<ListaEstadoProyectoResponse> GetProyectosEstados(int usr, IEnumerable<int> sociedadusr)
    {
        var estadoProyectos = _context.EstadoProyectos.ToList();

        List<ListaEstadoProyectoResponse> listaEstadoProyectoResponse = new();
        foreach (EstadoProyecto item in estadoProyectos)
        {
            listaEstadoProyectoResponse.Add(new ListaEstadoProyectoResponse
            {
                Id = item.Id,
                Nombre = item.Nombre
            });

        }
        return listaEstadoProyectoResponse;
    }
    public async Task AltaProyecto(int usr, AltaProyectoRequest dto)
    {
        //if (!_context.Usuarios.Any(x => x.Id == dto.Aprobador)) throw new AppException("El Aprobador no existe");
        if (!_context.Sociedads.Any(x => x.Id == dto.Sociedad)) throw new AppException("La Sociedad no existe");
        if (!_context.EstadoProyectos.Any(x => x.Id == dto.EstadoProyecto)) throw new AppException("El Estado no existe");
        if (_context.Proyectos.Any(x => x.Codigo == dto.Codigo && x.Sociedad == dto.Sociedad)) throw new AppException("Ya existe un proyecto con el código '" + dto.Codigo + "' para esta Sociedad");

        Proyecto model = new()
        {
            Codigo = dto.Codigo,
            Nombre = dto.Nombre,
            Sociedad = dto.Sociedad,
            //Aprobadors = dto.Aprobador,
            EstadoProyecto = dto.EstadoProyecto,
            Responsable = dto.Responsable,
            FechaAlta = DateTime.Now
        };

        _context.Proyectos.Add(model);
        _context.SaveChanges();        
    }
    public async Task ActualizarProyecto(int id, int usr, AltaProyectoRequest dto)
    {
        Proyecto proyecto = _context.Proyectos.FirstOrDefault(w => w.Id == id);
        if (proyecto is null) throw new AppException("El Proyecto no existe");

        //if (!_context.Usuarios.Any(x => x.Id == dto.Aprobador)) throw new AppException("El Aprobador no existe");
        if (!_context.Sociedads.Any(x => x.Id == dto.Sociedad)) throw new AppException("La Sociedad no existe");
        if (!_context.EstadoProyectos.Any(w => w.Id == dto.EstadoProyecto)) throw new AppException("El Estado no existe");
        if (_context.Proyectos.Any(w => w.Id != id && w.Codigo == dto.Codigo && w.Sociedad == dto.Sociedad)) throw new AppException("Ya existe un proyecto con el código '" + dto.Codigo + "' para esta Sociedad");

        proyecto.Codigo = dto.Codigo;
        proyecto.Nombre = dto.Nombre;
        proyecto.Sociedad = dto.Sociedad;
        //proyecto.Aprobador = dto.Aprobador;
        proyecto.EstadoProyecto = dto.EstadoProyecto;
        proyecto.Responsable = dto.Responsable;

        //_context.Entry(proyecto).State = EntityState.Modified;
        _context.Proyectos.Update(proyecto);
        _context.SaveChanges();
    }
    public async Task EliminarProyecto(int id, int usr)
    {
        Proyecto proyecto = _context.Proyectos.FirstOrDefault(w => w.Id == id);
        if (proyecto is null) throw new AppException("El Proyecto no existe");

        proyecto.Borrado = true;

        _context.Entry(proyecto).State = EntityState.Modified;
        _context.Proyectos.Update(proyecto);
        _context.SaveChanges();
    }
}