namespace PortalProveedor.Services;

using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PortalProveedor.Database;
using PortalProveedor.Entities;
using PortalProveedor.Helpers;
using PortalProveedor.Models.Dtos;
using PortalProveedor.Models.Facturas;
using PortalProveedor.Models.FlujoAprobacionFacturas;

public interface IFlujoAprobacionFacturaService
{
    IEnumerable<ListaFlujoAprobacionFacturaResponse> GetFlujoAprobacionFactura(int usr, IEnumerable<int> sociedad, BusquedaFlujoAprobacionFacturaRequest request);
    IEnumerable<FlujoAprobacionFacturaSimpleResponse> GetFlujoAprobacionFacturaSimple(int usr, IEnumerable<int> sociedad, BusquedaFlujoAprobacionFacturaRequest request);
}

public class FlujoAprobacionFacturaService : IFlujoAprobacionFacturaService
{
    private PortalProveedorContext _context;

    public FlujoAprobacionFacturaService(PortalProveedorContext context)
    {
        _context = context;
    }

    public IEnumerable<ListaFlujoAprobacionFacturaResponse> GetFlujoAprobacionFactura(int usr, IEnumerable<int> sociedad, BusquedaFlujoAprobacionFacturaRequest request)
    {
        var flujoAprobacionFacturas = _context.FlujoAprobacionFacturas
            .Include(i => i.SociedadNavigation)
            .Include(i => i.ProyectoNavigation)
            .Include(i => i.FlujoEstadoFacturas).ThenInclude(ii => ii.EstadoOrigenNavigation)
            .Include(i => i.FlujoEstadoFacturas).ThenInclude(ii => ii.EstadoDestinoNavigation)
            .Include(i => i.FlujoEstadoFacturas).ThenInclude(ii => ii.AprobadorNavigation).ThenInclude(iii => iii.EstadoNavigation)
            .Include(i => i.FlujoEstadoFacturas).ThenInclude(ii => ii.AprobadorSecundarioNavigation).ThenInclude(iii => iii.EstadoNavigation)
            .Include(i => i.FlujoEstadoFacturas).ThenInclude(ii => ii.AprobadorDelegadoNavigation).ThenInclude(iii => iii.EstadoNavigation)
            .Where(w => //w.LoginProveedorSociedads.Any(a => sociedad.Contains(a.Sociedad))
                w.Sociedad == request.Sociedad
            ).ToList();

        List<ListaFlujoAprobacionFacturaResponse> listaFlujoAprobacionFacturaResponse = new();
        foreach (FlujoAprobacionFactura item in flujoAprobacionFacturas)
        {
            List<FlujoEstadoFacturaResponse> listaFlujoEstadoFacturas = new List<FlujoEstadoFacturaResponse>();
            foreach (var flujoEstadoFactura in item.FlujoEstadoFacturas)
            {
                FlujoEstadoFacturaResponse flujoEstadoFacturaResponse = new FlujoEstadoFacturaResponse()
                {
                    Id = flujoEstadoFactura.Id,
                    NombreEstadoInterno = flujoEstadoFactura.NombreEstadoInterno,
                    NombreEstadoExterno = flujoEstadoFactura.NombreEstadoExterno,
                    EstadoOrigenNavigation = new EstadoFacturaResponse()
                    {
                        Id = flujoEstadoFactura.EstadoOrigenNavigation.Id,
                        EstadoInterno = flujoEstadoFactura.EstadoOrigenNavigation.EstadoInterno,
                        EstadoProveedor = flujoEstadoFactura.EstadoOrigenNavigation.EstadoProveedor,
                        EstadoInicial = flujoEstadoFactura.EstadoOrigenNavigation.EstadoInicial,
                        EstadoFinal = flujoEstadoFactura.EstadoOrigenNavigation.EstadoFinal
                    },
                    EstadoDestinoNavigation = new EstadoFacturaResponse()
                    {
                        Id = flujoEstadoFactura.EstadoDestinoNavigation.Id,
                        EstadoInterno = flujoEstadoFactura.EstadoDestinoNavigation.EstadoInterno,
                        EstadoProveedor = flujoEstadoFactura.EstadoDestinoNavigation.EstadoProveedor,
                        EstadoInicial = flujoEstadoFactura.EstadoDestinoNavigation.EstadoInicial,
                        EstadoFinal = flujoEstadoFactura.EstadoDestinoNavigation.EstadoFinal
                    },
                    WebProveedor = flujoEstadoFactura.WebProveedor,
                    WebGestion = flujoEstadoFactura.WebGestion,
                    AprobadorNavigation = new UsuarioAprobadorResponse()
                    {
                        Id = flujoEstadoFactura.AprobadorNavigation.Id,
                        Nombre = flujoEstadoFactura.AprobadorNavigation.Nombre,
                        Email = flujoEstadoFactura.AprobadorNavigation.Email,
                        Ausente = flujoEstadoFactura.AprobadorNavigation.Ausente,
                        EstadoId = flujoEstadoFactura.AprobadorNavigation.Estado,
                        Estado = flujoEstadoFactura.AprobadorNavigation.EstadoNavigation.Nombre
                    },
                    PermiteDelegacion = flujoEstadoFactura.PermiteDelegacion,
                    PermiteComentario = flujoEstadoFactura.PermiteComentario,
                    ComentarioObligatorio = flujoEstadoFactura.ComentarioObligatorio
                };

                if (flujoEstadoFactura.AprobadorSecundarioNavigation is not null)
                {
                    flujoEstadoFacturaResponse.AprobadorSecundarioNavigation = new UsuarioAprobadorResponse()
                    {
                        Id = flujoEstadoFactura.AprobadorSecundarioNavigation.Id,
                        Nombre = flujoEstadoFactura.AprobadorSecundarioNavigation.Nombre,
                        Email = flujoEstadoFactura.AprobadorSecundarioNavigation.Email,
                        Ausente = flujoEstadoFactura.AprobadorSecundarioNavigation.Ausente,
                        EstadoId = flujoEstadoFactura.AprobadorSecundarioNavigation.Estado,
                        Estado = flujoEstadoFactura.AprobadorSecundarioNavigation.EstadoNavigation.Nombre
                    };
                }

                if (flujoEstadoFactura.AprobadorDelegadoNavigation is not null)
                {
                    flujoEstadoFacturaResponse.AprobadorDelegadoNavigation = new UsuarioAprobadorResponse()
                    {
                        Id = flujoEstadoFactura.AprobadorDelegadoNavigation.Id,
                        Nombre = flujoEstadoFactura.AprobadorDelegadoNavigation.Nombre,
                        Email = flujoEstadoFactura.AprobadorDelegadoNavigation.Email,
                        Ausente = flujoEstadoFactura.AprobadorDelegadoNavigation.Ausente,
                        EstadoId = flujoEstadoFactura.AprobadorDelegadoNavigation.Estado,
                        Estado = flujoEstadoFactura.AprobadorDelegadoNavigation.EstadoNavigation.Nombre
                    };
                }

                listaFlujoEstadoFacturas.Add(flujoEstadoFacturaResponse);
            }

            listaFlujoAprobacionFacturaResponse.Add(new ListaFlujoAprobacionFacturaResponse
            {
                Id = item.Id,
                Nombre = item.Nombre,
                SociedadId = item.Sociedad,
                Sociedad = item.SociedadNavigation.Nombre,
                ProyectoId = item.Proyecto,
                Proyecto = item.ProyectoNavigation?.Nombre,
                Predeterminado = item.Predeterminado,
                Proforma = item.Proforma,
                FlujoEstadoFacturas = listaFlujoEstadoFacturas
            });

        }
        return listaFlujoAprobacionFacturaResponse;
    }

    public IEnumerable<FlujoAprobacionFacturaSimpleResponse> GetFlujoAprobacionFacturaSimple(int usr, IEnumerable<int> sociedad, BusquedaFlujoAprobacionFacturaRequest request)
    {
        if (request.Sociedad is null && request.Proyecto is null && request.FlujoAprobacionFactura is null) throw new AppException("Debe ingresar al menos un filtro");
        int responsableId = 0;
        string responsableNombre = "";
        int sociedadProyecto = 0;

        if (request.Proyecto is not null)
        {
            var proyectosData = _context.Proyectos
                .Include(i => i.UsuarioNavigation)
                .FirstOrDefault(w => w.Id == request.Proyecto);

            responsableId = proyectosData.Responsable.GetValueOrDefault(0);
            responsableNombre = proyectosData.UsuarioNavigation.Nombre;
            sociedadProyecto = proyectosData.Sociedad;
        }


        var flujoAprobacionFacturas = _context.FlujoEstadoFacturas
            .Include(i => i.EstadoOrigenNavigation)
            .Include(i => i.EstadoDestinoNavigation)
            .Include(i => i.FlujoAprobacionFacturaNavigation).ThenInclude(ii => ii.SociedadNavigation)
            .Include(i => i.AprobadorNavigation)
            .Include(i => i.AprobadorSecundarioNavigation)
            .Include(i => i.AprobadorDelegadoNavigation)
            .Where(w =>
                (!request.Sociedad.HasValue || w.FlujoAprobacionFacturaNavigation.Sociedad == request.Sociedad) &&
                (!request.Proyecto.HasValue || w.FlujoAprobacionFacturaNavigation.Sociedad == sociedadProyecto && w.FlujoAprobacionFacturaNavigation.SociedadNavigation.FlujoAprobacionFactura == w.FlujoAprobacionFactura) &&
                (!request.FlujoAprobacionFactura.HasValue || w.FlujoAprobacionFactura == request.FlujoAprobacionFactura)).ToList();

        List<FlujoAprobacionFacturaSimpleResponse> flujoAprobacionFacturaResponse = new();
        foreach (FlujoEstadoFactura item in flujoAprobacionFacturas)
        {
            flujoAprobacionFacturaResponse.Add(new FlujoAprobacionFacturaSimpleResponse()
            {
                flujoAprobacionFacturaId = item.FlujoAprobacionFactura,
                flujoAprobacionFactura = item.FlujoAprobacionFacturaNavigation.Nombre,
                flujoAprobacionFacturaPredeterminado = item.FlujoAprobacionFacturaNavigation.Predeterminado,
                flujoAprobacionFacturaProforma = item.FlujoAprobacionFacturaNavigation.Proforma,
                idOrigen = item.EstadoOrigen,
                estadoOrigenInterno = item.EstadoOrigenNavigation.EstadoInterno,
                estadoOrigenProveedor = item.EstadoOrigenNavigation.EstadoProveedor,
                estadoOrigenInicial = item.EstadoOrigenNavigation.EstadoInicial,
                estadoOrigenFinal = item.EstadoOrigenNavigation.EstadoFinal,
                idDestino = item.EstadoDestino,
                estadoDestinoInterno = item.EstadoDestinoNavigation.EstadoInterno,
                estadoDestinoProveedor = item.EstadoDestinoNavigation.EstadoProveedor,
                estadoDestinoInicial = item.EstadoDestinoNavigation.EstadoInicial,
                estadoDestinoFinal = item.EstadoDestinoNavigation.EstadoFinal,
                aprobadorId = request.Proyecto != null && item.EstadoOrigenNavigation.EstadoInicial ? responsableId : item.Aprobador,
                aprobador = request.Proyecto != null && item.EstadoOrigenNavigation.EstadoInicial ? responsableNombre : item.AprobadorNavigation.Nombre,
                aprobadorSecundarioId = item.AprobadorSecundario,
                aprobadorSecundario = item.AprobadorSecundarioNavigation?.Nombre,
                aprobadorDelegadoId = item.AprobadorSecundario,
                aprobadorDelegado = item.AprobadorDelegadoNavigation?.Nombre,
                permiteDelegacion = item.PermiteDelegacion,
                permiteComentario = item.PermiteComentario,
                comentarioObligatorio = item.ComentarioObligatorio,
                etiquetaBotonEstadoDestino = item.EtiquetaBotonEstadoDestino,
                etiquetaBotonDelegar = item.EtiquetaBotonDelegar,
                etiquetaCampoComentario = item.EtiquetaCampoComentario
            });
        }

        return flujoAprobacionFacturaResponse;
    }
}