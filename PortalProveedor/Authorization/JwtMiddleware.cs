namespace PortalProveedor.Authorization;

using PortalProveedor.Services;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;

    public JwtMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, IAccountService usuarioService, ILoginProveedorService loginProveedorService, IJwtUtils jwtUtils)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        var usuario = jwtUtils.ValidateToken(token);
        if (usuario.id != null)
        {
            // attach user to context on successful jwt validation
            switch (usuario.type)
            {
                case "proveedor":
                    context.Items["proveedor"] = loginProveedorService.GetById(usuario.id.Value);
                    context.Items["usrtype"] = "proveedor";
                    break;
                default:
                    context.Items["usuario"] = usuarioService.GetById(usuario.id.Value);
                    context.Items["usrtype"] = "usuario";
                    break;
            }
        }

        await _next(context);
    }
}