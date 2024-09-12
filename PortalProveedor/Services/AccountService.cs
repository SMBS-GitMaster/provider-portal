namespace PortalProveedor.Services;

using AutoMapper;
using BCrypt.Net;
using PortalProveedor.Authorization;
using PortalProveedor.Entities;
using PortalProveedor.Helpers;
using PortalProveedor.Database;
using PortalProveedor.Models.Usuarios;
using Microsoft.EntityFrameworkCore;
using System.Security.Authentication;

public interface IAccountService
{
    public Task<AuthenticateResponse> Authenticate(AuthenticateRequest model);
    public Usuario GetById(int id);
    public Task Register(RegisterRequest model);
}

public class AccountService : IAccountService
{
    private PortalProveedorContext _context;
    private IJwtUtils _jwtUtils;
    private readonly IMapper _mapper;

    public AccountService(PortalProveedorContext context,IJwtUtils jwtUtils,IMapper mapper)
    {
        _context = context;
        _jwtUtils = jwtUtils;
        _mapper = mapper;
    }

    public async Task<AuthenticateResponse> Authenticate(AuthenticateRequest model)
    {
        var usuario = _context.Usuarios.SingleOrDefault(x => x.Email == model.Email);
        if (usuario == null || !BCrypt.Verify(model.Password, usuario.Clave)) throw new AuthenticationException ("El email o la contraseña son incorrectos");
        if (usuario.Estado != 1) throw new AuthenticationException("El usuario no se encuentra activo");
        
        var rolUsuario = _context.RolSociedadUsuarios?.Include(i => i.RolNavigation).FirstOrDefault(x => x.Usuario == usuario.Id)?.RolNavigation?.Nombre;
        if (string.IsNullOrEmpty(rolUsuario)) throw new AuthenticationException("El usuario no tiene un rol asignado");

        var response = _mapper.Map<AuthenticateResponse>(usuario);
        response.Rol = rolUsuario;
        response.Token = _jwtUtils.GenerateToken(usuario);
        return response;
    }

    public async Task Register(RegisterRequest model)
    {
        if (_context.Usuarios.Any(x => x.Email == model.Email)) throw new AppException("El email '" + model.Email + "' ya existe");
        if (!_context.Clientes.Any(x => x.Id == model.Empresa)) throw new AppException("La empresa '" + model.Empresa + "' no existe");

        var usuario = _mapper.Map<Usuario>(model);
        usuario.Clave = BCrypt.HashPassword(model.Password);
        usuario.Estado = 1;
        usuario.FechaAlta = DateTime.Today;

        _context.Usuarios.Add(usuario);
        _context.SaveChanges();
    }

    public Usuario GetById(int id)
    {
        return getUser(id);
    }    

    private Usuario getUser(int id)
    {
        var usuario = _context.Usuarios.Find(id);
        usuario.RolSociedadUsuarios = _context.RolSociedadUsuarios.Where(x => x.Usuario == usuario.Id).Include(i => i.RolNavigation).ToList();
        if (usuario == null) throw new KeyNotFoundException("User not found");
        return usuario;
    }
}