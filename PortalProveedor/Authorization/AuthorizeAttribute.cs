namespace PortalProveedor.Authorization;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using PortalProveedor.Entities;
using System.Data;
using System.Linq;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute, IAuthorizationFilter
{
    private readonly IList<string> _roles;

    public AuthorizeAttribute(params string[] roles)
    {
        _roles = roles ?? new string[] { };
    }
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // skip authorization if action is decorated with [AllowAnonymous] attribute
        var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
        if (allowAnonymous)
            return;

        // authorization
        var usuario = (Usuario)context.HttpContext.Items["usuario"];
        if (usuario is null)
        {
            var proveedor = (LoginProveedor)context.HttpContext.Items["proveedor"];
            if (proveedor is null || !_roles.Contains("Proveedor")) context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
        }
        else
        {
            if (!_roles.Any()) //|| _roles.Where(w => usuario.RolSociedadUsuarios.Any(x => w.Contains(x.RolNavigation.Nombre))).Count() == 0)
                context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
        }
    }
}