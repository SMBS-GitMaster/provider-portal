namespace PortalProveedor.Services;

using Microsoft.Build.Framework;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using PortalProveedor.Database;
using PortalProveedor.Entities;
using PortalProveedor.Helpers;
using PortalProveedor.Models.Sociedades;
using PortalProveedor.Models.Usuarios;
using static System.Runtime.InteropServices.JavaScript.JSType;

public interface IUsuarioService
{
    UsuarioResponse GetUsuario(int id);
    public IEnumerable<ListaUsuarioResponse> GetResponsablesActuales(int usr);
    UsuarioResponse GetUsuarioByNombre(int usr, string nombre);
    UsuarioResponse GetUsuarioByEmail(int usr, string email);
    IEnumerable<Usuario> GetUsuarios();
    IEnumerable<ListaUsuarioResponse> GetUsuariosBySociedad(int usr, IEnumerable<int> sociedadusr, int sociedad);
    IEnumerable<ListaUsuarioResponse> GetUsuariosByCliente(int usr, IEnumerable<int> sociedadusr, int cliente);
    IEnumerable<ListaEstadoUsuarioResponse> GetUsuariosEstados(int usr, IEnumerable<int> sociedadusr);
    IEnumerable<ListaRolResponse> GetRoles(int usr, IEnumerable<int> sociedadusr);
    IEnumerable<ListaUsuarioResponse> GetUsuariosByUsuario(int usr, IEnumerable<int> sociedadusr);
    Task AltaUsuario(int usr, int cliente, AltaUsuarioRequest dto);
    Task ActualizarUsuario(int id, int usr, int cliente, ModifUsuarioRequest dto);
    IEnumerable<EstadoUsuarioResponse> GetEstados();
}

public class UsuarioService : IUsuarioService
{
    private PortalProveedorContext _context;

    public UsuarioService(
        PortalProveedorContext context)
    {
        _context = context;
    }

    public IEnumerable<Usuario> GetUsuarios()
    {
        var Usuarios = _context.Usuarios.ToList();
        return Usuarios;
    }

    public UsuarioResponse GetUsuarioByNombre(int usr, string nombre)
    {
        var usuario = _context.Usuarios
            .Include(i => i.EstadoNavigation)
            .Include(i => i.ClienteNavigation)
            .Include(i => i.Aprobadors)
            .Include(i => i.RolSociedadUsuarios).ThenInclude(ii => ii.SociedadNavigation)
            .Include(i => i.RolSociedadUsuarios).ThenInclude(ii => ii.RolNavigation).ThenInclude(iii => iii.PermisoRolAccions).ThenInclude(iii => iii.AccionNavigation)
            .FirstOrDefault(w => w.Nombre.Contains(nombre) && w.Estado == 1);
        if (usuario == null) throw new KeyNotFoundException("User not found");

        UsuarioResponse usuarioResponse = new()
        {
            id = usuario.Id,
            nombre = usuario.Nombre,
            email = usuario.Email,
            cliente = usuario.Cliente,
            estado = usuario.Estado,
            ausente = usuario.Ausente,
            fechaAlta = usuario.FechaAlta,
            clienteNavigation = new ClienteNavigationResponse()
            {
                id = usuario.ClienteNavigation.Id,
                nombre = usuario.ClienteNavigation.Nombre,
                email = usuario.ClienteNavigation.Email,
                identificador = usuario.ClienteNavigation.Identificador,
                tipoIdentificador = usuario.ClienteNavigation.TipoIdentificador,
                fechaAlta = usuario.ClienteNavigation.FechaAlta
            },
            estadoNavigation = new EstadoNavigationResponse()
            {
                id = usuario.EstadoNavigation.Id,
                nombre = usuario.EstadoNavigation.Nombre,
            },
            rolSociedadUsuarios = new List<RolSociedadUsuarioResponse>()
        };

        foreach (var rolSociedadUsuario in usuario.RolSociedadUsuarios)
        {

            var permisoRolAccionResponse = new List<PermisoRolAccionResponse>();
            foreach (var permisoRolAccions in rolSociedadUsuario.RolNavigation.PermisoRolAccions)
            {
                permisoRolAccionResponse.Add(new PermisoRolAccionResponse()
                {
                    Id = permisoRolAccions.Id,
                    Rol = permisoRolAccions.Rol,
                    Accion = permisoRolAccions.Accion,
                    Concedido = permisoRolAccions.Concedido,
                    accionNavigation = new AccionResponse() { Id = permisoRolAccions.AccionNavigation.Id, Nombre = permisoRolAccions.AccionNavigation.Nombre }
                });
            }

            usuarioResponse.rolSociedadUsuarios.Add(new RolSociedadUsuarioResponse()
            {
                RolId = rolSociedadUsuario.Rol,
                Rol = rolSociedadUsuario.RolNavigation.Nombre,
                SociedadId = rolSociedadUsuario.Sociedad,
                Sociedad = rolSociedadUsuario.SociedadNavigation.Nombre,
                permisoRolAccions = permisoRolAccionResponse
            });
        }

        return usuarioResponse;
    }

    public IEnumerable<ListaUsuarioResponse> GetResponsablesActuales (int usr)
    {
        var usuarios = _context.Usuarios
            .Include(i => i.ClienteNavigation)
            .Include(i => i.RolSociedadUsuarios).ThenInclude(ii => ii.RolNavigation)
            .Include(i => i.RolSociedadUsuarios).ThenInclude(ii => ii.SociedadNavigation)
            .GroupJoin(_context.Facturas, u => u.Id, f => f.ResponsableAprobar, (u, facturas) => new { Usuario = u, Facturas = facturas })
            .SelectMany(x => x.Facturas.DefaultIfEmpty(), (x, f) => new { Usuario = x.Usuario, Factura = f })
            .GroupJoin(_context.RolSociedadUsuarios, x => x.Factura.Sociedad, rsu => rsu.Sociedad, (x, rsus) => new { Usuario = x.Usuario, RolSociedadUsuarios = rsus })
            .SelectMany(x => x.RolSociedadUsuarios.DefaultIfEmpty(), (x, rsu) => new { x.Usuario, RolSociedadUsuarios = rsu })
            .Where(w => w.RolSociedadUsuarios.Usuario == usr)
            .Distinct()
            .Select(x => x.Usuario);
    
        if (usuarios == null) throw new KeyNotFoundException("User not found");

        List<ListaUsuarioResponse> listaUsuariosResponse = new();
        foreach (Usuario item in usuarios)
        {
            List<RolSociedadUsuarioResponse> rolSociedadUsuarioResponses = new();
            foreach (RolSociedadUsuario rolSociedadUsuario in item.RolSociedadUsuarios)
            {
                rolSociedadUsuarioResponses.Add(new RolSociedadUsuarioResponse()
                {
                    RolId = rolSociedadUsuario.Rol,
                    Rol = rolSociedadUsuario.RolNavigation.Nombre,
                    SociedadId = rolSociedadUsuario.Sociedad,
                    Sociedad = rolSociedadUsuario.SociedadNavigation.Nombre
                });
            }
            listaUsuariosResponse.Add(new ListaUsuarioResponse
            {
                Id = item.Id,
                Nombre = item.Nombre,
                Email = item.Email,
                ClienteId = item.Cliente,
                Cliente = item.ClienteNavigation.Nombre,
                RolSociedadUsuario = rolSociedadUsuarioResponses
            });

        }
        return listaUsuariosResponse;
    }

    public UsuarioResponse GetUsuarioByEmail(int usr, string email)
    {
        var usuario = _context.Usuarios
            .Include(i => i.EstadoNavigation)
            .Include(i => i.ClienteNavigation)
            .Include(i => i.Aprobadors)
            .Include(i => i.RolSociedadUsuarios).ThenInclude(ii => ii.SociedadNavigation)
            .Include(i => i.RolSociedadUsuarios).ThenInclude(ii => ii.RolNavigation).ThenInclude(iii => iii.PermisoRolAccions).ThenInclude(iii => iii.AccionNavigation)
            .FirstOrDefault(w => w.Email.Contains(email) && w.Estado == 1);
        if (usuario == null) throw new KeyNotFoundException("User not found");

        UsuarioResponse usuarioResponse = new()
        {
            id = usuario.Id,
            nombre = usuario.Nombre,
            email = usuario.Email,
            cliente = usuario.Cliente,
            estado = usuario.Estado,
            ausente = usuario.Ausente,
            fechaAlta = usuario.FechaAlta,
            clienteNavigation = new ClienteNavigationResponse()
            {
                id = usuario.ClienteNavigation.Id,
                nombre = usuario.ClienteNavigation.Nombre,
                email = usuario.ClienteNavigation.Email,
                identificador = usuario.ClienteNavigation.Identificador,
                tipoIdentificador = usuario.ClienteNavigation.TipoIdentificador,
                fechaAlta = usuario.ClienteNavigation.FechaAlta
            },
            estadoNavigation = new EstadoNavigationResponse()
            {
                id = usuario.EstadoNavigation.Id,
                nombre = usuario.EstadoNavigation.Nombre,
            },
            rolSociedadUsuarios = new List<RolSociedadUsuarioResponse>()
        };

        foreach (var rolSociedadUsuario in usuario.RolSociedadUsuarios)
        {

            var permisoRolAccionResponse = new List<PermisoRolAccionResponse>();
            foreach (var permisoRolAccions in rolSociedadUsuario.RolNavigation.PermisoRolAccions)
            {
                permisoRolAccionResponse.Add(new PermisoRolAccionResponse()
                {
                    Id = permisoRolAccions.Id,
                    Rol = permisoRolAccions.Rol,
                    Accion = permisoRolAccions.Accion,
                    Concedido = permisoRolAccions.Concedido,
                    accionNavigation = new AccionResponse() { Id = permisoRolAccions.AccionNavigation.Id, Nombre = permisoRolAccions.AccionNavigation.Nombre }
                });
            }

            usuarioResponse.rolSociedadUsuarios.Add(new RolSociedadUsuarioResponse()
            {
                RolId = rolSociedadUsuario.Rol,
                Rol = rolSociedadUsuario.RolNavigation.Nombre,
                SociedadId = rolSociedadUsuario.Sociedad,
                Sociedad = rolSociedadUsuario.SociedadNavigation.Nombre,
                permisoRolAccions = permisoRolAccionResponse
            });
        }

        return usuarioResponse;
    }

    public IEnumerable<ListaUsuarioResponse> GetUsuariosByUsuario(int usr, IEnumerable<int> sociedadusr)
    {
        var usuarios = _context.Usuarios
            .Include(i => i.ClienteNavigation)
            .Include(i => i.RolSociedadUsuarios).ThenInclude(ii => ii.RolNavigation)
            .Include(i => i.RolSociedadUsuarios).ThenInclude(ii => ii.SociedadNavigation)
            .Where(w => w.RolSociedadUsuarios.Any(a => sociedadusr.Contains(a.Sociedad))).ToList();

        List<ListaUsuarioResponse> listaUsuariosResponse = new();
        foreach (Usuario item in usuarios)
        {
            List<RolSociedadUsuarioResponse> rolSociedadUsuarioResponses = new();
            foreach (RolSociedadUsuario rolSociedadUsuario in item.RolSociedadUsuarios)
            {
                rolSociedadUsuarioResponses.Add(new RolSociedadUsuarioResponse()
                {
                    RolId = rolSociedadUsuario.Rol,
                    Rol = rolSociedadUsuario.RolNavigation.Nombre,
                    SociedadId = rolSociedadUsuario.Sociedad,
                    Sociedad = rolSociedadUsuario.SociedadNavigation.Nombre
                });
            }
            listaUsuariosResponse.Add(new ListaUsuarioResponse
            {
                Id = item.Id,
                Nombre = item.Nombre,
                Email = item.Email,
                ClienteId = item.Cliente,
                Cliente = item.ClienteNavigation.Nombre,
                RolSociedadUsuario = rolSociedadUsuarioResponses
            });

        }
        return listaUsuariosResponse;
    }

    public IEnumerable<ListaUsuarioResponse> GetUsuariosBySociedad(int usr, IEnumerable<int> sociedadusr, int sociedad)
    {
        var usuarios = _context.Usuarios
            .Include(i => i.ClienteNavigation)
            .Include(i => i.RolSociedadUsuarios).ThenInclude(ii => ii.RolNavigation)
            .Include(i => i.RolSociedadUsuarios).ThenInclude(ii => ii.SociedadNavigation)
            .Where(w => w.RolSociedadUsuarios.Any(a => a.Sociedad == sociedad) && (w.Estado == 1)).ToList();

        List<ListaUsuarioResponse> listaUsuariosResponse = new();
        foreach (Usuario item in usuarios)
        {
            List<RolSociedadUsuarioResponse> rolSociedadUsuarioResponses = new();
            foreach (RolSociedadUsuario rolSociedadUsuario in item.RolSociedadUsuarios)
            {
                rolSociedadUsuarioResponses.Add(new RolSociedadUsuarioResponse()
                {
                    RolId = rolSociedadUsuario.Rol,
                    Rol = rolSociedadUsuario.RolNavigation.Nombre,
                    SociedadId = rolSociedadUsuario.Sociedad,
                    Sociedad = rolSociedadUsuario.SociedadNavigation.Nombre
                });
            }
            listaUsuariosResponse.Add(new ListaUsuarioResponse
            {
                Id = item.Id,
                Nombre = item.Nombre,
                Email = item.Email,
                ClienteId = item.Cliente,
                Cliente = item.ClienteNavigation.Nombre,
                RolSociedadUsuario = rolSociedadUsuarioResponses
            });

        }
        return listaUsuariosResponse;
    }

    public IEnumerable<ListaUsuarioResponse> GetUsuariosByCliente(int usr, IEnumerable<int> sociedadusr, int cliente)
    {
        var usuarios = _context.Usuarios
            .Include(i => i.ClienteNavigation)
            .Include(i => i.RolSociedadUsuarios).ThenInclude(ii => ii.RolNavigation)
            .Include(i => i.RolSociedadUsuarios).ThenInclude(ii => ii.SociedadNavigation)
            .Where(w => w.Cliente == cliente && w.Estado == 1).ToList();

        List<ListaUsuarioResponse> listaUsuariosResponse = new();
        foreach (Usuario item in usuarios)
        {
            List<RolSociedadUsuarioResponse> rolSociedadUsuarioResponses = new();
            foreach (RolSociedadUsuario rolSociedadUsuario in item.RolSociedadUsuarios)
            {
                rolSociedadUsuarioResponses.Add(new RolSociedadUsuarioResponse()
                {
                    RolId = rolSociedadUsuario.Rol,
                    Rol = rolSociedadUsuario.RolNavigation.Nombre,
                    SociedadId = rolSociedadUsuario.Sociedad,
                    Sociedad = rolSociedadUsuario.SociedadNavigation.Nombre
                });
            }
            listaUsuariosResponse.Add(new ListaUsuarioResponse
            {
                Id = item.Id,
                Nombre = item.Nombre,
                Email = item.Email,
                ClienteId = item.Cliente,
                Cliente = item.ClienteNavigation.Nombre,
                RolSociedadUsuario = rolSociedadUsuarioResponses
            });

        }
        return listaUsuariosResponse;
    }

    public IEnumerable<ListaEstadoUsuarioResponse> GetUsuariosEstados(int usr, IEnumerable<int> sociedadusr)
    {
        var estadoUsuarios = _context.EstadoUsuarios.ToList();

        List<ListaEstadoUsuarioResponse> listaEstadoUsuarioResponse = new();
        foreach (EstadoUsuario item in estadoUsuarios)
        {
            listaEstadoUsuarioResponse.Add(new ListaEstadoUsuarioResponse
            {
                Id = item.Id,
                Nombre = item.Nombre
            });

        }
        return listaEstadoUsuarioResponse;
    }

    public IEnumerable<ListaRolResponse> GetRoles(int usr, IEnumerable<int> sociedadusr)
    {
        var rols = _context.Rols.ToList();

        List<ListaRolResponse> listaRolResponse = new();
        foreach (Rol item in rols)
        {
            listaRolResponse.Add(new ListaRolResponse
            {
                Id = item.Id,
                Nombre = item.Nombre
            });

        }
        return listaRolResponse;
    }

    public UsuarioResponse GetUsuario(int id)
    {
        var usuario = _context.Usuarios
            .Include(i => i.EstadoNavigation)
            .Include(i => i.ClienteNavigation)
            .Include(i => i.Aprobadors)
            .Include(i => i.RolSociedadUsuarios).ThenInclude(ii => ii.SociedadNavigation)
            .Include(i => i.RolSociedadUsuarios).ThenInclude(ii => ii.RolNavigation).ThenInclude(iii => iii.PermisoRolAccions).ThenInclude(iii => iii.AccionNavigation)
            .FirstOrDefault(w => w.Id == id && w.Estado == 1);
        if (usuario == null) throw new KeyNotFoundException("User not found");

        UsuarioResponse usuarioResponse = new()
        {
            id = usuario.Id,
            nombre = usuario.Nombre,
            email = usuario.Email,
            cliente = usuario.Cliente,
            estado = usuario.Estado,
            ausente = usuario.Ausente,
            fechaAlta = usuario.FechaAlta,
            clienteNavigation = new ClienteNavigationResponse()
            {
                id = usuario.ClienteNavigation.Id,
                nombre = usuario.ClienteNavigation.Nombre,
                email = usuario.ClienteNavigation.Email,
                identificador = usuario.ClienteNavigation.Identificador,
                tipoIdentificador = usuario.ClienteNavigation.TipoIdentificador,
                fechaAlta = usuario.ClienteNavigation.FechaAlta
            },
            estadoNavigation = new EstadoNavigationResponse()
            {
                id = usuario.EstadoNavigation.Id,
                nombre = usuario.EstadoNavigation.Nombre,
            },
            rolSociedadUsuarios = new List<RolSociedadUsuarioResponse>()
        };

        foreach (var rolSociedadUsuario in usuario.RolSociedadUsuarios) {
            
            var permisoRolAccionResponse = new List<PermisoRolAccionResponse>();
            foreach (var permisoRolAccions in rolSociedadUsuario.RolNavigation.PermisoRolAccions)
            {
                permisoRolAccionResponse.Add(new PermisoRolAccionResponse()
                {
                    Id = permisoRolAccions.Id,
                    Rol = permisoRolAccions.Rol,
                    Accion = permisoRolAccions.Accion,
                    Concedido = permisoRolAccions.Concedido,
                    accionNavigation = new AccionResponse() { Id = permisoRolAccions.AccionNavigation.Id, Nombre = permisoRolAccions.AccionNavigation.Nombre }
                });
            }

            usuarioResponse.rolSociedadUsuarios.Add(new RolSociedadUsuarioResponse()
            {
                RolId = rolSociedadUsuario.Rol,
                Rol = rolSociedadUsuario.RolNavigation.Nombre,
                SociedadId = rolSociedadUsuario.Sociedad,
                Sociedad = rolSociedadUsuario.SociedadNavigation.Nombre,
                permisoRolAccions = permisoRolAccionResponse
            });
        }

        return usuarioResponse;
    }

    public async Task AltaUsuario(int usr, int cliente, AltaUsuarioRequest dto)
    {
        if (!_context.Clientes.Any(x => x.Id == cliente)) throw new AppException("El Cliente no existe");
        if (!_context.EstadoUsuarios.Any(x => x.Id == dto.Estado)) throw new AppException("El Estado no existe");

        List<RolSociedadUsuario> rolSocieddadUsuario = new List<RolSociedadUsuario>();
        foreach (var item in dto.RolSociedadUsuarios)
        {
            if (!_context.Sociedads.Any(x => x.Id == item.Sociedad)) throw new AppException("La Sociedad no existe");
            if (!_context.Rols.Any(x => x.Id == item.Rol)) throw new AppException("El Rol no existe");

            rolSocieddadUsuario.Add(new RolSociedadUsuario { Rol = item.Rol, Sociedad = item.Sociedad });
        }

        Usuario model = new()
        {
            Nombre = dto.Nombre,
            Email = dto.Email,
            Clave = BCrypt.HashPassword(dto.Clave),
            Cliente = cliente,
            RolSociedadUsuarios = rolSocieddadUsuario,
            Estado = dto.Estado,
            Ausente = dto.Ausente,
            FechaAlta = DateTime.Now
        };

        _context.Usuarios.Add(model);
        _context.SaveChanges();
    }

    public async Task ActualizarUsuario(int id, int usr, int cliente, ModifUsuarioRequest dto)
    {
        Usuario usuario = _context.Usuarios.Include(i => i.RolSociedadUsuarios).FirstOrDefault(w => w.Id == id);
        if (usuario is null) throw new AppException("El Usuario no existe");

        if (!_context.Clientes.Any(x => x.Id == cliente)) throw new AppException("El Cliente no existe");
        if (!_context.EstadoUsuarios.Any(x => x.Id == dto.Estado)) throw new AppException("El Estado no existe");

        foreach (var item in dto.RolSociedadUsuarios)
        {
            if (!_context.Sociedads.Any(x => x.Id == item.Sociedad)) throw new AppException("La Sociedad no existe");
            if (!_context.Rols.Any(x => x.Id == item.Rol)) throw new AppException("El Rol no existe");

            bool exist = false;
            foreach (var item2 in usuario.RolSociedadUsuarios)
            {
                if (id == item2.Usuario && item.Sociedad == item2.Sociedad)
                {
                    exist = true;
                    break;
                }
            }

            if (!exist) usuario.RolSociedadUsuarios.Add(new RolSociedadUsuario { Usuario = usuario.Id, Rol = item.Rol, Sociedad = item.Sociedad });
            else
            {
                var update = usuario.RolSociedadUsuarios.SingleOrDefault(s => s.Usuario == usuario.Id && s.Sociedad == item.Sociedad);
                if (update != null)
                    update.Rol = item.Rol;
            }
        }

        usuario.Nombre = dto.Nombre;
        usuario.Email = dto.Email;
        if (dto.Clave != null && dto.Clave.Trim() != "")
            usuario.Clave = BCrypt.HashPassword(dto.Clave);
        usuario.Cliente = cliente;
        usuario.Estado = dto.Estado;
        usuario.Ausente = dto.Ausente;

        _context.Usuarios.Update(usuario);
        _context.SaveChanges();

        foreach (var item in usuario.RolSociedadUsuarios.ToList())
        {
            bool exist = false;
            foreach (var item2 in dto.RolSociedadUsuarios)
            {
                if (item.Rol == item2.Rol && item.Sociedad == item2.Sociedad)
                {
                    exist = true;
                    break;
                }
            }

            if (!exist)
            {
                _context.RolSociedadUsuarios.Remove(item);
                _context.SaveChanges();
            }
        }
    }

    public IEnumerable<EstadoUsuarioResponse> GetEstados()
    {
        var estados = _context.EstadoUsuarios.ToList();

        List<EstadoUsuarioResponse> listaEstadosResponse = new();
        foreach (EstadoUsuario item in estados)
        {
            listaEstadosResponse.Add(new EstadoUsuarioResponse
            {
                Id = item.Id,
                Nombre = item.Nombre
            });

        }
        return listaEstadosResponse;
    }
}