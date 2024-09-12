using Microsoft.EntityFrameworkCore;
using PortalProveedor.Database;
using PortalProveedor.Entities;
using PortalProveedor.Helpers;
using PortalProveedor.Models.Imputacion;
using PortalProveedor.Models.Proyectos;
using PortalProveedor.Models.Usuarios;

namespace PortalProveedor.Services
{
    public interface IImputacionService
    {
        public Task<IEnumerable<ListaImputacionResponse>> GetImputaciones();
        public Task<ImputacionResponse> GetById(int id);
        public Task AltaImputacion(AltaImputacionRequest dto, int usuario);
        public Task ActualizarImputacion(int id, EditImputacionRequest dto, int usuario);
    }
    public class ImputacionService : IImputacionService
    {
        private PortalProveedorContext _context;
        private IUsuarioTarifaService _usuarioTarifaService;
        public ImputacionService(PortalProveedorContext context, IUsuarioTarifaService usuarioTarifaService)
        {
            _context = context;
            _usuarioTarifaService = usuarioTarifaService;
        }

        private static decimal CalcularTotalHorasPorProyecto(List<decimal> horas) { 
            return horas.Sum(x => x);
        }
        private static decimal CalcularCostoTotalPorProyecto(decimal totalHoras, decimal precio) {
            return totalHoras * precio;
        }
        private static decimal CalcularCostosTotalImputacion(ICollection<ImputacionDetalle> lImputacionDetalle)
        {
            return lImputacionDetalle.Sum(i => i.TotalCosto);
        }
        private static decimal CalcularTotalHorasImputacion(ICollection<ImputacionDetalle> lImputacionDetalle) {
            return lImputacionDetalle.Sum(i => i.TotalHoras);
        }

        public async Task AltaImputacion(AltaImputacionRequest dto, int usuario)
        {
            var lImputacionDetalle = new List<ImputacionDetalle>();
            DateTime FechaImputacion = _usuarioTarifaService.ObtenerFormatoFecha(dto.Mes);
            decimal totalHoras;

            if(_context.Imputacion.Any( w => w.Usuario == usuario && w.Mes == dto.Mes )) 
                throw new Exception("Ya existe una imputacion para el mes especificado");

            UsuarioTarifa tarifa = (_context.UsuarioTarifa.Where(w => w.Usuario == usuario
                           && w.FechaInicia <= FechaImputacion && w.FechaVence >= FechaImputacion
                           && w.Borrado == false).OrderBy(x => x.Id).LastOrDefault()
                           ?? throw new AppException("No se encontró tarifa para su usuario en el mes especificado"));

            if (dto.ImputacionDetalle is not null)
            {
                foreach(AltaImputacionDetalleRequest detalle in dto.ImputacionDetalle)
                {
                    // Validar existencia del proyecto
                    if (!_context.Proyectos.Any(x => x.Id == detalle.Proyecto)) throw new AppException("El proyecto " + detalle.Proyecto +" no existe");
                    // Validar que el proyecto no se agregue mas de dos veces en la imputación en curso
                    if (lImputacionDetalle.Any(x => x.Proyecto == detalle.Proyecto)) throw new AppException("Un proyecto no puede agregarse mas de dos veces en una imputación");
                    
                    totalHoras = CalcularTotalHorasPorProyecto(detalle.horas);
                    lImputacionDetalle.Add(new ImputacionDetalle { 
                        Proyecto = detalle.Proyecto,
                        TotalHoras = totalHoras,
                        TotalCosto = CalcularCostoTotalPorProyecto(totalHoras, tarifa.PrecioHora),
                    });
                }
            }

            Imputacion model = new()
            {
                Mes = dto.Mes,
                Usuario = usuario,
                TotalHoras = CalcularTotalHorasImputacion(lImputacionDetalle),
                TotalCosto = CalcularCostosTotalImputacion(lImputacionDetalle),
                ImputacionHoras = dto.ImputacionHoras,
                ImputacionDetalle = lImputacionDetalle,
            };

            _context.Imputacion.Add(model);
            await _context.SaveChangesAsync();
        }
        public async Task ActualizarImputacion(int id, EditImputacionRequest dto, int usuario)
        {
            //remover imputación detalle cuales fueron excluidas en la actualización
            var proyectos = dto.ImputacionDetalle.Select(x => x.Proyecto).ToArray();
            List<ImputacionDetalle> proyectosExcluidos = _context.ImputacionDetalle.Where(w => !proyectos.Contains(w.Proyecto) && w.Imputacion == id).ToList();
            if(proyectosExcluidos != null)
            {
                _context.ImputacionDetalle.RemoveRange(proyectosExcluidos);
            }

            var imputacion = _context.Imputacion.Include(i => i.ImputacionDetalle)
                .FirstOrDefault( x => x.Id == id) ?? throw new AppException("La imputación no existe");
            
            DateTime FechaImputacion = _usuarioTarifaService.ObtenerFormatoFecha(imputacion.Mes);
            UsuarioTarifa tarifa = (_context.UsuarioTarifa.Where(w => w.Usuario == usuario
                           && w.FechaInicia <= FechaImputacion && w.FechaVence >= FechaImputacion
                           && w.Borrado == false).OrderBy(x => x.Id).LastOrDefault() 
                           ?? throw new AppException("No se encontró tarifa para su usuario en el mes especificado"));

            var lImputacionDetalle = new List<ImputacionDetalle>();
            decimal totalHoras;
            if (dto.ImputacionDetalle is not null)
            {
                foreach (EditImputacionDetalleRequest detalleRequest in dto.ImputacionDetalle)
                {
                    var detalle = imputacion.ImputacionDetalle.FirstOrDefault( x => x.Proyecto == detalleRequest.Proyecto);
                    totalHoras = CalcularTotalHorasPorProyecto(detalleRequest.horas);
                    if(detalle is not null) // Update
                    { 
                        detalle.TotalHoras = totalHoras;
                        detalle.TotalCosto = CalcularCostoTotalPorProyecto(totalHoras, tarifa.PrecioHora);
                    }
                    else // Create
                    {
                        // Validar existencia del proyecto
                        if (!_context.Proyectos.Any(x => x.Id == detalleRequest.Proyecto)) 
                            throw new AppException("El proyecto " + detalleRequest.Proyecto + " no existe");

                        totalHoras = CalcularTotalHorasPorProyecto(detalleRequest.horas);
                        imputacion.ImputacionDetalle.Add(new ImputacionDetalle
                        {
                            Proyecto = detalleRequest.Proyecto,
                            TotalHoras = totalHoras,
                            TotalCosto = CalcularCostoTotalPorProyecto(totalHoras, tarifa.PrecioHora),
                        });
                    }                 
                }

            }

            imputacion.Usuario = usuario;
            imputacion.TotalHoras = CalcularTotalHorasImputacion(imputacion.ImputacionDetalle);
            imputacion.TotalCosto = CalcularCostosTotalImputacion(imputacion.ImputacionDetalle);
            imputacion.ImputacionHoras = dto.ImputacionHoras;
            _context.Imputacion.Update(imputacion);
            await _context.SaveChangesAsync();      
        }

        public async Task<ImputacionResponse> GetById(int id)
        {
            var imputacion = await _context.Imputacion.Include( i => i.ImputacionDetalle)
                .ThenInclude( ii => ii.ProyectoNavigation).ThenInclude( iii => iii.EstadoProyectoNavigation)
                .Include( i => i.ImputacionDetalle).ThenInclude( ii => ii.ProyectoNavigation).ThenInclude(iii=>iii.SociedadNavigation)
                .Include( i => i.UsuarioNavigation)
                .FirstOrDefaultAsync( w => w.Id == id ) ?? throw new AppException("No se econtró imputación con id " + id);

            if (imputacion is null) return null;
            // Detalle
            List<ImputacionDetalleResponse> lImputacionDetalle = new List<ImputacionDetalleResponse>();
            foreach (ImputacionDetalle detalle in imputacion.ImputacionDetalle)
            {
                lImputacionDetalle.Add(new ImputacionDetalleResponse()
                {
                    Id = detalle.Id,
                    Imputacion = detalle.Imputacion,
                    TotalHoras = detalle.TotalHoras,
                    TotalCosto = detalle.TotalCosto,
                    Proyecto = new ListaProyectoResponse()
                    {
                        Id = detalle.ProyectoNavigation.Id,
                        Codigo = detalle.ProyectoNavigation.Codigo,
                        Nombre = detalle.ProyectoNavigation.Nombre,
                        Estado = detalle.ProyectoNavigation.EstadoProyectoNavigation.Nombre,
                        Sociedad = detalle.ProyectoNavigation.SociedadNavigation.Nombre,
                    }
                });
            }
            //Usuario Response
            UsuarioResponse usuario = new()
            {
                id = imputacion.UsuarioNavigation.Id,
                nombre = imputacion.UsuarioNavigation.Nombre,
                email = imputacion.UsuarioNavigation.Email,
                estado = imputacion.UsuarioNavigation.Estado,
                ausente = imputacion.UsuarioNavigation.Ausente,
            };
            // Imputacion Response
            ImputacionResponse imputacionResponse = new ImputacionResponse { 
                Id = imputacion.Id,
                Usuario = imputacion.Usuario,
                Mes = imputacion.Mes,
                TotalHoras = imputacion.TotalHoras,
                TotalCosto = imputacion.TotalCosto,
                ImputacionHoras = imputacion.ImputacionHoras,
                FechaAlta = imputacion.FechaAlta,
                ImputacionDetalle = lImputacionDetalle,
                UsuarioNavigation = usuario
            };
            
            return imputacionResponse;
        }
        public async Task<IEnumerable<ListaImputacionResponse>> GetImputaciones()
        {
            var imputaciones = await _context.Imputacion.Include( i => i.UsuarioNavigation).ToListAsync();
            if(imputaciones is null) return Enumerable.Empty<ListaImputacionResponse>();
            List<ListaImputacionResponse> listImputacionResponse = new List<ListaImputacionResponse>();       

            foreach (Imputacion imputacion in imputaciones)
            {
                //Usuario Response
                UsuarioResponse usuario = new()
                {
                    id = imputacion.UsuarioNavigation.Id,
                    nombre = imputacion.UsuarioNavigation.Nombre,
                    email = imputacion.UsuarioNavigation.Email,
                    estado = imputacion.UsuarioNavigation.Estado,
                    ausente = imputacion.UsuarioNavigation.Ausente,
                };
                listImputacionResponse.Add(new ListaImputacionResponse
                {
                    id = imputacion.Id,
                    Usuario = imputacion.Usuario,
                    Mes = imputacion.Mes,
                    TotalHoras = imputacion.TotalHoras,
                    TotalCosto = imputacion.TotalCosto,
                    FechaAlta = imputacion.FechaAlta,
                    UsuarioNavigation = usuario
                });
            }
            return listImputacionResponse;
        }

    }
}