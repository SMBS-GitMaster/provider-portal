using Azure.Storage.Blobs;
using Azure.Storage;
using Microsoft.AspNetCore.Mvc;
using PortalProveedor.Authorization;
using PortalProveedor.Database;
using PortalProveedor.Entities;
using PortalProveedor.Helpers;
using PortalProveedor.Models.Facturas;
using PortalProveedor.Services;
using System.IO;

namespace PortalProveedor.Controllers
{
    [Authorize(new string[] { "Proveedor", "Administrador/Contable" })]
    [Route("api/[controller]")]
    [ApiController]
    public class FicheroController : ControllerBase
    {
        private readonly PortalProveedorContext _context;
        private readonly ILogger<FicheroController> _logger;
        private readonly IFicheroService _ficheroService;

        public FicheroController(PortalProveedorContext context, ILogger<FicheroController> logger, IFicheroService ficheroService)
        {
            _context = context;
            _logger = logger;
            _ficheroService = ficheroService;
        }

        [HttpGet("GetUrl/{id}")]
        public async Task<IActionResult> GetFileUrl(int id)
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.Errors());

            string usrtype = (string)HttpContext.Items["usrtype"];
            if (string.IsNullOrEmpty(usrtype)) return BadRequest();

            switch (usrtype)
            {
                case "proveedor":
                    var proveedor = (Proveedor)HttpContext.Items["proveedor"];
                    if (proveedor is null) return BadRequest();

                    var fichero = await _ficheroService.GetFileUrl(id, proveedor.Id, usrtype);
                    
                    if (fichero is null) return NotFound(); else return Ok(new { fileUrl = fichero });
                default:
                    var usr = (Usuario)HttpContext.Items["usuario"];
                    if (usr is null) return BadRequest();

                    fichero = await _ficheroService.GetFileUrl(id, usr.Id, usrtype);
                    
                    if (fichero is null) return NotFound(); else return Ok(new { fileUrl = fichero });
            }
        }

        // GET: api/Fichero/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FicheroResponse?>> GetFichero(int id)
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.Errors());

            string usrtype = (string)HttpContext.Items["usrtype"];
            if (string.IsNullOrEmpty(usrtype)) return BadRequest();

            switch (usrtype)
            {
                case "proveedor":
                    var proveedor = (Proveedor)HttpContext.Items["proveedor"];
                    if (proveedor is null) return BadRequest();

                    var fichero = await _ficheroService.GetFichero(id, proveedor.Id, usrtype);
                    if (fichero is null || fichero.Content is null) return NotFound(); else return File(fichero.Content, fichero.ContentType, fichero.Name);
                default:
                    var usr = (Usuario)HttpContext.Items["usuario"];
                    if (usr is null) return BadRequest();

                    fichero = await _ficheroService.GetFichero(id, usr.Id, usrtype);
                    if (fichero is null || fichero.Content is null) return NotFound(); else return File(fichero.Content, fichero.ContentType, fichero.Name);
            }
        }
    }
}
