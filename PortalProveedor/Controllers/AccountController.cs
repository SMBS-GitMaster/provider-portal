using Microsoft.AspNetCore.Mvc;
using System.Linq;
using PortalProveedor.Authorization;
using PortalProveedor.Models.Usuarios;
using PortalProveedor.Services;
using PortalProveedor.Helpers;
using Newtonsoft.Json.Linq;

namespace PortalProveedor.Controllers
{
    /// <summary>
    /// Registro y Login de Usuarios.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private IAccountService _usuarioService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(ILogger<AccountController> logger, IAccountService usuarioService)
        {
            _logger = logger;
            _usuarioService = usuarioService;
        }

        /// <summary>
        /// Login de Usuario.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> AuthenticateAsync(AuthenticateRequest model)
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.Errors()); //return BadRequest(ModelState);

            var response = await _usuarioService.Authenticate(model);
            return Ok(response);
        }

        /// <summary>
        /// Registro de Usuario.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(RegisterRequest model)
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.Errors()); //return BadRequest(new { errors = ModelState.Errors() });

            await _usuarioService.Register(model);
            return Ok(new { message = "Registration successful" });
        }
    }
}
