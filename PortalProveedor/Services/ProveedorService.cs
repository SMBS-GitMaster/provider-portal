namespace PortalProveedor.Services;

using Microsoft.EntityFrameworkCore;
using PortalProveedor.Database;
using BCrypt.Net;
using PortalProveedor.Entities;
using PortalProveedor.Models.Proveedores;
using PortalProveedor.Models.Facturas;
using static PortalProveedor.Models.Dtos.Dtos;
using PortalProveedor.Helpers;
using PortalProveedor.Models.Proyectos;
using PortalProveedor.Models.LoginProveedores;
using PortalProveedor.Models.LoginProveedoresSociedades;

public interface IProveedorService
{
    public Proveedor GetById(int id);
    public Task<ProveedorResponse?> GetProveedor(int usr, string usrtype, int id);
    public IEnumerable<ProveedorResponse> GetProveedores(int usr, IEnumerable<int> sociedad, BusquedaProveedorRequest request);
    public Task AltaProveedor(int usr, AltaProveedorRequest dto);
    public Task ActualizarProveedor(int id, int usr, EditProveedorRequest dto);
    public IEnumerable<ListaEstadoProveedorResponse> GetProveedoresEstados(int usr, IEnumerable<int> sociedadusr);
    public IEnumerable<ProveedorResponse> GetProveedoresByUsr(int usr);
}

public class ProveedorService : IProveedorService
{
    private PortalProveedorContext _context;
    private ISociedadService _sociedadService;


    public ProveedorService(
        PortalProveedorContext context, ISociedadService sociedadService)
    {
        _context = context;
        _sociedadService=sociedadService;
    }

    public async Task<ProveedorResponse?> GetProveedor(int usr, string usrtype, int id)
    {
        var proveedor = _context.Proveedors
            .Include(i => i.TipoIdentificadorNavigation)
            .Include(i => i.EstadoProveedorNavigation)
            .Include(i => i.LoginProveedors)
            .Include(i => i.LoginProveedorSociedads)
            .ThenInclude(ii => ii.SociedadNavigation)
            .ThenInclude(iii => iii.ClienteNavigation)
            .ThenInclude(iiii => iiii.Usuarios)
            .FirstOrDefault(w => w.Id == id && w.LoginProveedorSociedads.Any(ww => ww.SociedadNavigation.ClienteNavigation.Usuarios.Any(www => www.Id == usr)));
        if (proveedor is null) return null;

        List<LoginProveedorResponse> lLoginProveedores = new List<LoginProveedorResponse>();
        foreach (var loginProveedor in proveedor.LoginProveedors)
        {
            lLoginProveedores.Add(new LoginProveedorResponse() { Id = loginProveedor.Id, Email = loginProveedor.Email, FechaAlta = loginProveedor.FechaAlta });
        };

        List<LoginProveedorSociedadResponse> lLoginProveedoresSociedades = new List<LoginProveedorSociedadResponse>();
        foreach (var loginProveedorSociedad in proveedor.LoginProveedorSociedads)
        {
            lLoginProveedoresSociedades.Add(new LoginProveedorSociedadResponse() { 
                Id = loginProveedorSociedad.Id, 
                SociedadId = loginProveedorSociedad.SociedadNavigation.Id,
                Sociedad = loginProveedorSociedad.SociedadNavigation.Nombre,
                LoginProveedor = loginProveedorSociedad.LoginProveedor
            });
        };

        return new ProveedorResponse
        {
            Id = proveedor.Id,
            Nombre = proveedor.Nombre,
            Identificador = proveedor.Identificador,
            TipoIdentificadorId = proveedor.TipoIdentificadorNavigation.Id,
            TipoIdentificador = proveedor.TipoIdentificadorNavigation.Nombre,
            EstadoId = proveedor.EstadoProveedorNavigation.Id,
            Estado = proveedor.EstadoProveedorNavigation.Nombre,
            LoginProveedores = lLoginProveedores,
            LoginProveedorSociedades = lLoginProveedoresSociedades
        };
    }

    public IEnumerable<ProveedorResponse> GetProveedoresByUsr(int usr)
    {
        var sociedadesUsuario = _sociedadService.GetSociedadesByUsuario(usr);
        var proveedoresFactura = _context.Facturas
            .Include(i => i.ProveedorNavigation)
            .Include(i => i.SociedadNavigation)
            .Include(i => i.ProyectoNavigation)
            .Include(i => i.ResponsableAprobarNavigation)
            .Where(w => (sociedadesUsuario.Contains(w.Sociedad)))
            .Select(e => e.Proveedor).Distinct();
        var proveedores = _context.Proveedors
            .Include(i => i.TipoIdentificadorNavigation)
            .Include(i => i.EstadoProveedorNavigation)
            .Include(i => i.LoginProveedorSociedads)
            .ThenInclude(ii => ii.SociedadNavigation)
            .ThenInclude(iii => iii.ClienteNavigation)
            .ThenInclude(iiii => iiii.Usuarios)
            .Where(w => w.LoginProveedorSociedads.Any(ww => ww.SociedadNavigation.ClienteNavigation.Usuarios.Any(www => www.Id == usr)) && proveedoresFactura.Contains(w.Id)).ToList();

        List<ProveedorResponse> listaProveedoresResponse = new();
        foreach (Proveedor proveedor in proveedores)
        {
            listaProveedoresResponse.Add(new ProveedorResponse
            {
                Id = proveedor.Id,
                Nombre = proveedor.Nombre,
                Identificador = proveedor.Identificador,
                TipoIdentificador = proveedor.TipoIdentificadorNavigation.Nombre,
                Estado = proveedor.EstadoProveedorNavigation.Nombre
            });

        }
        return listaProveedoresResponse;
    }

    public IEnumerable<ProveedorResponse> GetProveedores(int usr, IEnumerable<int> sociedad, BusquedaProveedorRequest request)
    {
        var proveedores = _context.Proveedors
            .Include(i => i.TipoIdentificadorNavigation)
            .Include(i => i.EstadoProveedorNavigation)
            .Include(i => i.LoginProveedorSociedads)
            .ThenInclude(ii => ii.SociedadNavigation)
            .ThenInclude(iii => iii.ClienteNavigation)
            .ThenInclude(iiii => iiii.Usuarios)
            .Where(w => (w.LoginProveedorSociedads.Any(ww => ww.SociedadNavigation.ClienteNavigation.Usuarios.Any(www => www.Id == usr)))
            && (string.IsNullOrEmpty(request.Nombre) || w.Nombre == request.Nombre)
            && (string.IsNullOrEmpty(request.Identificador) || w.Identificador == request.Identificador)
            && (!request.TipoIdentificador.HasValue || w.TipoIdentificador == request.TipoIdentificador)
            && (!request.Estado.HasValue || w.EstadoProveedor == request.Estado)
            && (string.IsNullOrEmpty(request.Email) || w.LoginProveedors.Any(a => a.Email == request.Email))).ToList();

        List<ProveedorResponse> listaProveedoresResponse = new();
        foreach (Proveedor proveedor in proveedores)
        {
            listaProveedoresResponse.Add(new ProveedorResponse
            {
                Id = proveedor.Id,
                Nombre = proveedor.Nombre,
                Identificador = proveedor.Identificador,
                TipoIdentificador = proveedor.TipoIdentificadorNavigation.Nombre,
                Estado = proveedor.EstadoProveedorNavigation.Nombre
            });

        }
        return listaProveedoresResponse;
    }

    public IEnumerable<ListaEstadoProveedorResponse> GetProveedoresEstados(int usr, IEnumerable<int> sociedadusr)
    {
        var estadoProveedores = _context.EstadoProveedors.ToList();

        List<ListaEstadoProveedorResponse> listaEstadoProveedorResponse = new();
        foreach (EstadoProveedor item in estadoProveedores)
        {
            listaEstadoProveedorResponse.Add(new ListaEstadoProveedorResponse
            {
                Id = item.Id,
                Nombre = item.Nombre
            });

        }
        return listaEstadoProveedorResponse;
    }

    public async Task AltaProveedor(int usr, AltaProveedorRequest dto)
    {
        if (!_context.EstadoProveedors.Any(w => w.Id == dto.Estado)) throw new AppException("El Estado no existe");
        if (!_context.TipoIdentificadors.Any(w => w.Id == dto.TipoIdentificador)) throw new AppException("El Tipo de Identificador no existe");
        if (_context.Proveedors.Include(i => i.LoginProveedorSociedads).Any(w => w.Identificador == dto.Identificador && w.TipoIdentificador == dto.TipoIdentificador)) throw new AppException("Ya existe un proveedor con el mismo identificador");

        foreach (var sociedad in dto.Sociedades) {if (!_context.Sociedads.Any(w => w.Id == sociedad)) throw new AppException("La Sociedad no existe");}

        var lLoginProveedor = new List<LoginProveedor>();
        foreach (var loginProveedor in dto.LoginProveedor)
        {
            lLoginProveedor.Add(new LoginProveedor
            {
                Email = loginProveedor.Email,
                Clave = BCrypt.HashPassword(loginProveedor.Password),
                FechaAlta = DateTime.Now
            });
        }

        Proveedor model = new()
        {
            Nombre = dto.Nombre,
            Identificador = dto.Identificador,
            TipoIdentificador = dto.TipoIdentificador,
            EstadoProveedor = dto.Estado,
            LoginProveedors = lLoginProveedor,
            FechaAlta = DateTime.Now
        };

        _context.Proveedors.Add(model);
        _context.SaveChanges();

        var idProveedor = model.Id;

        var proveedor = _context.Proveedors.Include(i => i.LoginProveedors).FirstOrDefault(w => w.Id == idProveedor);
        if (proveedor is not null)
        {
            foreach (var loginProveedor in proveedor.LoginProveedors)
            {
                foreach (var sociedad in dto.Sociedades)
                {
                    _context.LoginProveedorSociedads.Add(new LoginProveedorSociedad()
                    {
                        Proveedor = idProveedor,
                        Sociedad = sociedad,
                        LoginProveedor = loginProveedor.Id
                    });
                }
            }
            _context.SaveChanges();
        }
    }

    public async Task ActualizarProveedor(int id, int usr, EditProveedorRequest dto)
    {
        if (!_context.EstadoProveedors.Any(w => w.Id == dto.Estado)) throw new AppException("El Estado no existe");
        if (!_context.TipoIdentificadors.Any(w => w.Id == dto.TipoIdentificador)) throw new AppException("El Tipo de Identificador no existe");
        if (_context.Proveedors.Include(i => i.LoginProveedorSociedads).Any(w => w.Id != id && w.Identificador == dto.Identificador && w.TipoIdentificador == dto.TipoIdentificador)) throw new AppException("Ya existe un proveedor con el mismo identificador");

        foreach (var sociedad in dto.Sociedades) { if (!_context.Sociedads.Any(w => w.Id == sociedad)) throw new AppException("La Sociedad no existe"); }

        Proveedor proveedor = _context.Proveedors.Include(i => i.LoginProveedors).Include(i => i.LoginProveedorSociedads).FirstOrDefault(w => w.Id == id);
        if (proveedor is null) throw new AppException("El Proveedor no existe");

        foreach (var loginProveedor in dto.LoginProveedor)
        {
            if (proveedor.LoginProveedors.Any(w => w.Email == loginProveedor.Email))
            {
                proveedor.LoginProveedors.FirstOrDefault(w => w.Email == loginProveedor.Email).Clave = BCrypt.HashPassword(loginProveedor.Password);
            }
            else
            {
                proveedor.LoginProveedors.Add(new LoginProveedor
                {
                    Email = loginProveedor.Email,
                    Clave = BCrypt.HashPassword(loginProveedor.Password),
                    FechaAlta = DateTime.Now
                });
            }

            foreach (var sociedad in dto.Sociedades)
            {
                if (!_context.Sociedads.Any(x => x.Id == sociedad)) throw new AppException("La Sociedad no existe");

                bool exist = false;
                foreach (var sociedad2 in proveedor.LoginProveedorSociedads)
                {
                    if (sociedad == sociedad2.Sociedad)
                    {
                        exist = true;
                        break;
                    }
                }
                if (!exist)
                {
                    proveedor.LoginProveedorSociedads.Add(new LoginProveedorSociedad()
                    {
                        Proveedor = id,
                        Sociedad = sociedad,
                        LoginProveedor = proveedor.LoginProveedors.FirstOrDefault(w => w.Email == loginProveedor.Email).Id
                    });
                }
            }
        }

        proveedor.Nombre = dto.Nombre;
        proveedor.Identificador = dto.Identificador;
        proveedor.TipoIdentificador = dto.TipoIdentificador;
        proveedor.EstadoProveedor = dto.Estado;

        //_context.Entry(proveedor).State = EntityState.Modified;
        _context.Proveedors.Update(proveedor);
        _context.SaveChanges();

        foreach (var loginProveedorSociedad in proveedor.LoginProveedorSociedads.ToList())
        {
            bool exist = false;
            foreach (var sociedad in dto.Sociedades)
            {
                if (loginProveedorSociedad.Sociedad == sociedad)
                {
                    exist = true;
                    break;
                }
            }

            if (!exist)
            {
                _context.LoginProveedorSociedads.Remove(loginProveedorSociedad);
                _context.SaveChanges();
            }
        }
    }

    public Proveedor GetById(int id)
    {
        return GetProveedor(id);
    }

    // helper methods
    private Proveedor GetProveedor(int id)
    {
        var proveedor = _context.Proveedors.Find(id);
        if (proveedor == null) throw new KeyNotFoundException("Proveedor no encontrado");
        return proveedor;
    }
}