namespace PortalProveedor.Services;

using AutoMapper;
using BCrypt.Net;
using PortalProveedor.Authorization;
using PortalProveedor.Entities;
using PortalProveedor.Helpers;
using PortalProveedor.Database;
using PortalProveedor.Models.Usuarios;
using Microsoft.EntityFrameworkCore;

public interface ILoginProveedorService
{
    public AuthenticateResponse Authenticate(AuthenticateRequest model);
    public Task<IEnumerable<LoginProveedor>> GetLoginProveedor();
    public LoginProveedor GetById(int id);
}

public class LoginProveedorService : ILoginProveedorService
{
    private PortalProveedorContext _context;
    private IJwtUtils _jwtUtils;
    private readonly IMapper _mapper;

    public LoginProveedorService(PortalProveedorContext context,IJwtUtils jwtUtils,IMapper mapper)
    {
        _context = context;
        _jwtUtils = jwtUtils;
        _mapper = mapper;
    }

    public AuthenticateResponse Authenticate(AuthenticateRequest model)
    {
        var loginProveedor = _context.LoginProveedors.Include(i => i.ProveedorNavigation).SingleOrDefault(x => x.Email == model.Email);
        //if (loginProveedor == null || !BCrypt.Verify(model.Password, loginProveedor.Clave)) throw new AppException("El nombre de usuario o la contraseña son incorrectos");
        if (loginProveedor == null || model.Password != loginProveedor.Clave) throw new AppException("El email o la contraseña son incorrectos");
        if (loginProveedor.ProveedorNavigation.EstadoProveedor != 1) throw new AppException("El proveedor no se encuentra activo");

        var response = _mapper.Map<AuthenticateResponse>(loginProveedor);
        response.Rol = "Proveedor";
        response.Token = _jwtUtils.GenerateToken(loginProveedor);
        return response;
    }

    public async Task<IEnumerable<LoginProveedor>> GetLoginProveedor()
    {
        var loginProveedor = await _context.LoginProveedors.ToListAsync();
        return loginProveedor;
    }

    public LoginProveedor GetById(int id)
    {
        return getUser(id);
    }

    private LoginProveedor getUser(int id)
    {
        var usuario = _context.LoginProveedors.Find(id);
        if (usuario == null) throw new KeyNotFoundException("User not found");
        return usuario;
    }
}