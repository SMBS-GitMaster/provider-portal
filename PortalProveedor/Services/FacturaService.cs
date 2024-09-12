namespace PortalProveedor.Services;

using AutoMapper;
using Azure.Core;
using Azure.Storage;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PortalProveedor.Database;
using PortalProveedor.Entities;
using PortalProveedor.Helpers;
using PortalProveedor.Models.Dtos;
using PortalProveedor.Models.Facturas;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using static PortalProveedor.Models.Dtos.Dtos;

public interface IFacturaService
{
    public Task<FacturaResponse?> GetFactura(int usr, string usrtype, int id);
    public Task<IEnumerable<FacturaResponse>> GetFacturasSearch(int usr, string usrtype, List<int> proveedores, List<int> sociedades, List<int> proyectos, List<int> estados, List<int> responsablesActuales, List<int> years);
    public Task<IEnumerable<FacturaResponse>> GetFacturasByEstadoDestino(int usr, string usrtype, int estado);
    public Task<IEnumerable<FacturaResponse>> GetFacturasByEstado(int usr, string usrtype, int estado);
    public Task<IEnumerable<FacturaResponse>> GetFacturasByPedido(int usr, string usrtype, int pedido);
    public Task AltaFactura(int usr, string usrtype, AltaFacturaRequest dto);
    public Task CancelFactura(int usr, string usrtype, int id);
    public Task ActualizarEstadoFactura(int usr, string usrtype, int id, byte estado, string comentario);
    public Task ActualizarFactura(int usr, string usrtype, int id, int proyecto, int aprobador);
    public Task<IEnumerable<EstadoFactura>> GetEstados(int usr, string usrtype);
    public Task<IEnumerable<int>> GetYears(int usr, string usrtype);
}

public class FacturaService : IFacturaService
{
    private PortalProveedorContext _context;
    private ISociedadService _sociedadService;
    private IProveedorService _proveedorService;
    private IProyectoService _proyectoService;
    private IUsuarioService _usuarioService;
    private readonly AppSettings _appSettings;

    public FacturaService(
        PortalProveedorContext context,
        ISociedadService sociedadService,
        IProveedorService proveedorService,
        IProyectoService proyectoService,
        IUsuarioService usuarioService,
        IOptions<AppSettings> appSettings,
        IMapper mapper)
    {
        _context = context;
        _appSettings = appSettings.Value;
        _sociedadService = sociedadService;
        _proveedorService = proveedorService;
        _proyectoService = proyectoService;
        _usuarioService = usuarioService;
    }


    public async Task<FacturaResponse?> GetFactura(int usr, string usrtype, int id)
    {
        var sociedadesUsuario = _sociedadService.GetSociedadesByUsuario(usr);
        var factura = _context.Facturas
            .Include(i => i.ProveedorNavigation)
            .Include(i => i.SociedadNavigation).ThenInclude(ii => ii.FlujoAprobacionFacturaNavigation).ThenInclude(ii => ii.FlujoEstadoFacturas).ThenInclude(ii => ii.AprobadorNavigation)
            .Include(i => i.SociedadNavigation).ThenInclude(ii => ii.FlujoAprobacionFacturaProformaNavigation).ThenInclude(ii => ii.FlujoEstadoFacturas).ThenInclude(ii => ii.AprobadorNavigation)
            .Include(i => i.ProyectoNavigation).ThenInclude(ii => ii.FlujoAprobacionFacturaNavigation).ThenInclude(ii => ii.FlujoEstadoFacturas).ThenInclude(ii => ii.AprobadorNavigation)
            .Include(i => i.ResponsableAprobarNavigation)
            .Include(i => i.FicheroFacturaNavigation)
            .Include(i => i.FicheroFacturaProformaNavigation)
            .Include(i => i.Albarans).ThenInclude(t => t.FicheroAlbaranNavigation)
            .Include(i => i.FacturaEstadoFacturas).ThenInclude(i => i.EstadoFacturaNavigation)
            .FirstOrDefault(w => w.Id == id && sociedadesUsuario.Contains(w.Sociedad));
        if (factura is null) return null;

        int flujoAprobacionFactura = factura.SociedadNavigation.FlujoAprobacionFactura;
        int flujoAprobacionFacturaProforma = factura.SociedadNavigation.FlujoAprobacionFacturaProforma;
        if (factura.ProyectoNavigation.FlujoAprobacionFactura is not null) flujoAprobacionFactura = factura.ProyectoNavigation.FlujoAprobacionFactura.Value;

        bool isEditable = false;
        byte estadoId = factura.FacturaEstadoFacturas.LastOrDefault().EstadoFacturaNavigation.Id;
        var flujoEstadoFactura = factura.SociedadNavigation.FlujoAprobacionFacturaNavigation.FlujoEstadoFacturas.Where(w => 
            w.EstadoOrigen == estadoId && 
            (w.Aprobador == usr && !w.AprobadorNavigation.Ausente && w.AprobadorDelegado is null) ||
            (w.AprobadorDelegado == usr && !w.AprobadorDelegadoNavigation.Ausente));
        if (flujoEstadoFactura.Any())
        {
            isEditable = true;
        }
        else
        {
            var flujoEstadoFacturaSecundario = factura.SociedadNavigation.FlujoAprobacionFacturaNavigation.FlujoEstadoFacturas.Where(w =>
                w.EstadoOrigen == estadoId &&
                w.AprobadorSecundario == usr && !w.AprobadorSecundarioNavigation.Ausente);
            if (flujoEstadoFacturaSecundario.Any()) isEditable = true;
        }

        var lFicherosAlbaran = new List<FicheroResponse>();
        if (factura.Albarans is not null)
        {
            foreach (var albaran in factura.Albarans)
            {
                lFicherosAlbaran.Add(new FicheroResponse
                {
                    Id = albaran.FicheroAlbaranNavigation.Id,
                    Nombre = albaran.FicheroAlbaranNavigation.Nombre,
                    Ruta = albaran.FicheroAlbaranNavigation.Ruta,
                    FechaAlta = albaran.FicheroAlbaranNavigation.FechaAlta
                });
            }
        }

        FacturaResponse facturaResponse = new()
        {
            Id = factura.Id,
            Numero = factura.Numero,
            Importe = factura.Importe,
            Proveedor = factura.ProveedorNavigation.Nombre,
            Responsable = factura.ResponsableAprobarNavigation.Nombre,
            ResponsableId = factura.ResponsableAprobarNavigation.Id,
            PermisoResponsable = factura.ResponsableAprobarNavigation.Id == usr,
            ProyectoId = factura.ProyectoNavigation.Id,
            CodigoProyecto = factura.ProyectoNavigation.Codigo,
            Proyecto = factura.ProyectoNavigation.Nombre,
            SociedadId = factura.Sociedad,
            Sociedad = factura.SociedadNavigation.Nombre,
            DescuentoProntoPago = factura.DescuentoProntoPago,
            FicherosAlbaran = lFicherosAlbaran,
            FechaContable = factura.FechaFactura.HasValue ? factura.FechaFactura.Value.ToString("yyyy-MM-dd") : factura.FechaFacturaProforma.Value.ToString("yyyy-MM-dd"),
            FechaVencimiento = factura.FechaVencimiento?.ToString("yyyy-MM-dd"),
            FechaAlta = factura.FechaAlta.ToString("yyyy-MM-dd"),
            EstadoId = estadoId,
            EstadoInterno = factura.FacturaEstadoFacturas.LastOrDefault().EstadoFacturaNavigation.EstadoInterno,
            EstadoProveedor = factura.FacturaEstadoFacturas.LastOrDefault().EstadoFacturaNavigation.EstadoProveedor,
            EstadoInicial = factura.FacturaEstadoFacturas.LastOrDefault().EstadoFacturaNavigation.EstadoInicial,
            EstadoFinal = factura.FacturaEstadoFacturas.LastOrDefault().EstadoFacturaNavigation.EstadoFinal,
            FlujoAprobacionFacturaId = flujoAprobacionFactura,
            FlujoAprobacionFacturaProformaId = flujoAprobacionFacturaProforma,
            IsEditable = isEditable
        };

        if (factura.FicheroFactura is not null && factura.FicheroFacturaNavigation is not null)
        {
            facturaResponse.FicheroFactura = new FicheroResponse() { Id = factura.FicheroFacturaNavigation.Id, Nombre = factura.FicheroFacturaNavigation.Nombre, Ruta = factura.FicheroFacturaNavigation.Ruta, FechaAlta = factura.FicheroFacturaNavigation.FechaAlta };
        }

        if (factura.FicheroFacturaProforma is not null && factura.FicheroFacturaProformaNavigation is not null)
        {
            facturaResponse.FicheroFacturaProforma = new FicheroResponse() { Id = factura.FicheroFacturaProformaNavigation.Id, Nombre = factura.FicheroFacturaProformaNavigation.Nombre, Ruta = factura.FicheroFacturaProformaNavigation.Ruta, FechaAlta = factura.FicheroFacturaProformaNavigation.FechaAlta };
        }

        return facturaResponse;
    }

    public async Task<IEnumerable<int>> GetYears(int usr, string usrtype)
    {
        var sociedadesUsuario = _sociedadService.GetSociedadesByUsuario(usr);
        var years = _context.Facturas
            .Include(i => i.SociedadNavigation)
            .Where(w => sociedadesUsuario.Contains(w.Sociedad))
            .Select(s => s.FechaFactura.HasValue ? s.FechaFactura.Value.Year : s.FechaFacturaProforma.Value.Year).Distinct();

        return years;
    } 

    public async Task<IEnumerable<FacturaResponse>> GetFacturasSearch(int usr, string usrtype, List<int> proveedores, List<int> sociedades, List<int> proyectos, List<int> estados, List<int> responsablesActuales, List<int> years)
    {
        var sociedadesUsuario = _sociedadService.GetSociedadesByUsuario(usr);

        var facturas = _context.Facturas
            .Include(i => i.ProveedorNavigation)
            .Include(i => i.SociedadNavigation)
            .Include(i => i.ProyectoNavigation)
            .Include(i => i.ResponsableAprobarNavigation)
            .Include(i => i.FacturaEstadoFacturas).ThenInclude(i => i.EstadoFacturaNavigation)
            .Where(w => 
                (estados[0] == -1 || estados.Contains(w.FacturaEstadoFacturas.OrderByDescending(o => o.Id).FirstOrDefault().EstadoFactura)) &&
                (proveedores[0] == -1 || proveedores.Contains(w.Proveedor)) &&
                (sociedades[0] == -1 || sociedades.Contains(w.Sociedad)) && sociedadesUsuario.Contains(w.Sociedad) &&
                (proyectos[0] == -1 || proyectos.Contains(w.Proyecto)) &&
                (responsablesActuales[0] == -1 || responsablesActuales.Contains(w.ResponsableAprobar)) &&
                (years[0] == -1 || years[0] == 0 || years.Contains(w.FechaFactura.Value.Year) || years.Contains(w.FechaFacturaProforma.Value.Year)));
        // var sql = facturas.ToQueryString();
        if (facturas is null) return Enumerable.Empty<FacturaResponse>();

        List<FacturaResponse> listaFacturaResponse = new();
        foreach (Factura factura in facturas)
        {
            listaFacturaResponse.Add(new FacturaResponse
            {
                Id = factura.Id,
                Numero = factura.Numero,
                Importe = factura.Importe,
                Proveedor = factura.ProveedorNavigation.Nombre,
                Responsable = factura.ResponsableAprobarNavigation.Nombre,
                ResponsableId = factura.ResponsableAprobarNavigation.Id,
                PermisoResponsable = factura.ResponsableAprobarNavigation.Id == usr,
                ProyectoId = factura.ProyectoNavigation.Id,
                CodigoProyecto = factura.ProyectoNavigation.Codigo,
                Proyecto = factura.ProyectoNavigation.Nombre,
                SociedadId = factura.Sociedad,
                Sociedad = factura.SociedadNavigation.Nombre,
                DescuentoProntoPago = factura.DescuentoProntoPago,
                FechaContable = factura.FechaFactura.HasValue ? factura.FechaFactura.Value.ToString("yyyy-MM-dd") : factura.FechaFacturaProforma.Value.ToString("yyyy-MM-dd"),
                FechaVencimiento = factura.FechaVencimiento?.ToString("yyyy-MM-dd"),
                FechaAlta = factura.FechaAlta.ToString("yyyy-MM-dd"),
                EstadoId = factura.FacturaEstadoFacturas.LastOrDefault().EstadoFacturaNavigation.Id,
                EstadoInterno = factura.FacturaEstadoFacturas.LastOrDefault().EstadoFacturaNavigation.EstadoInterno,
                EstadoProveedor = factura.FacturaEstadoFacturas.LastOrDefault().EstadoFacturaNavigation.EstadoProveedor,
                EstadoInicial = factura.FacturaEstadoFacturas.LastOrDefault().EstadoFacturaNavigation.EstadoInicial,
                EstadoFinal = factura.FacturaEstadoFacturas.LastOrDefault().EstadoFacturaNavigation.EstadoFinal
            }); ;
        }

        return listaFacturaResponse;
    }

    public async Task<IEnumerable<EstadoFactura>> GetEstados(int usr, string usrtype)
    {
        var sociedadesUsuario = _sociedadService.GetSociedadesByUsuario(usr);
        var estadosFactura = _context.Facturas
            .Include(i => i.ProveedorNavigation)
            .Include(i => i.SociedadNavigation)
            .Include(i => i.ProyectoNavigation)
            .Include(i => i.ResponsableAprobarNavigation)
            .Include(i => i.FacturaEstadoFacturas).ThenInclude(i => i.EstadoFacturaNavigation)
            .Where(w => sociedadesUsuario.Contains(w.Sociedad))
            .SelectMany(e => e.FacturaEstadoFacturas.Select(f => f.EstadoFactura)).Distinct();
        var estados = _context.EstadoFacturas
            .Include(i => i.FacturaEstadoFacturas)
            .Where(w => estadosFactura.Contains(w.Id) && w.FacturaEstadoFacturas.OrderByDescending(o => o.Id).FirstOrDefault().EstadoFactura == w.Id).ToList();
        return estados;
    }

    public async Task<IEnumerable<FacturaResponse>> GetFacturasByEstadoDestino(int usr, string usrtype, int estado)
    {
        var sociedadesUsuario = _sociedadService.GetSociedadesByUsuario(usr);

        var estadosOrigen = _context.FlujoEstadoFacturas
            .Include(i => i.FlujoAprobacionFacturaNavigation)
            .Where(w => sociedadesUsuario.Contains(w.FlujoAprobacionFacturaNavigation.Sociedad) && w.EstadoDestino == estado)
            .Select(s => new { Origen = s.EstadoOrigen, s.FlujoAprobacionFacturaNavigation.Sociedad , s.FlujoAprobacionFacturaNavigation.Proforma }).AsEnumerable().ToList();

        var facturas = _context.Facturas
            .Include(i => i.ProveedorNavigation)
            .Include(i => i.SociedadNavigation).ThenInclude(i => i.FlujoAprobacionFacturaNavigation).ThenInclude(i => i.FlujoEstadoFacturas)
            .Include(i => i.ProyectoNavigation)
            .Include(i => i.ResponsableAprobarNavigation)
            .Include(i => i.FacturaEstadoFacturas).ThenInclude(i => i.EstadoFacturaNavigation)
            .Where(w => sociedadesUsuario.Contains(w.Sociedad))
            .Where(w => w.ResponsableAprobar == usr || w.ProyectoNavigation.Responsable == usr || w.FacturaEstadoFacturas.Any(ww => ww.Usuario == usr && ww.EstadoFactura != 1))
            .ToList() // Retrieve all the invoices from the database
            .Where(w => estadosOrigen.Any(eo => eo.Origen == w.FacturaEstadoFacturas.OrderByDescending(o => o.Id).FirstOrDefault().EstadoFactura && eo.Sociedad == w.Sociedad 
                && (!w.FicheroFacturaProforma.HasValue && eo.Proforma == false || w.FicheroFacturaProforma.HasValue && eo.Proforma == true)))
            .ToList();
        //var sql = facturas.ToQueryString();
        if (facturas is null) return Enumerable.Empty<FacturaResponse>();

        List<FacturaResponse> listaFacturaResponse = new();
        foreach (Factura factura in facturas)
        {
            listaFacturaResponse.Add(new FacturaResponse
            {
                Id = factura.Id,
                Numero = factura.Numero,
                Importe = factura.Importe,
                Proveedor = factura.ProveedorNavigation.Nombre,
                Responsable = factura.ResponsableAprobarNavigation.Nombre,
                ResponsableId = factura.ResponsableAprobarNavigation.Id,
                PermisoResponsable = factura.ResponsableAprobarNavigation.Id == usr,
                Proyecto = factura.ProyectoNavigation.Nombre,
                SociedadId = factura.Sociedad,
                Sociedad = factura.SociedadNavigation.Nombre,
                DescuentoProntoPago = factura.DescuentoProntoPago,
                FechaContable = factura.FechaFactura.HasValue ? factura.FechaFactura.Value.ToString("yyyy-MM-dd") : factura.FechaFacturaProforma.Value.ToString("yyyy-MM-dd"),
                FechaVencimiento = factura.FechaVencimiento?.ToString("yyyy-MM-dd"),
                FechaAlta = factura.FechaAlta.ToString("yyyy-MM-dd"),
                EstadoId = factura.FacturaEstadoFacturas.LastOrDefault().EstadoFacturaNavigation.Id,
                EstadoInterno = factura.FacturaEstadoFacturas.LastOrDefault().EstadoFacturaNavigation.EstadoInterno,
                EstadoProveedor = factura.FacturaEstadoFacturas.LastOrDefault().EstadoFacturaNavigation.EstadoProveedor,
                EstadoInicial = factura.FacturaEstadoFacturas.LastOrDefault().EstadoFacturaNavigation.EstadoInicial,
                EstadoFinal = factura.FacturaEstadoFacturas.LastOrDefault().EstadoFacturaNavigation.EstadoFinal
            });
        }

        return listaFacturaResponse;
    }

    public async Task<IEnumerable<FacturaResponse>> GetFacturasByPedido(int usr, string usrtype, int pedido)
    {
        var facturas = _context.Facturas
            .Include(i => i.ProveedorNavigation)
            .Include(i => i.SociedadNavigation)
            .Include(i => i.ProyectoNavigation)
            .Include(i => i.ResponsableAprobarNavigation)
            .Include(i => i.FacturaEstadoFacturas).ThenInclude(i => i.EstadoFacturaNavigation)
            .Include(i => i.FacturaPedidos)
            .Where(w => w.FacturaPedidos.Any(a => a.Pedido == pedido));

        if (facturas is null) return Enumerable.Empty<FacturaResponse>();

        List<FacturaResponse> listaFacturaResponse = new();
        foreach (Factura factura in facturas)
        {
            listaFacturaResponse.Add(new FacturaResponse
            {
                Id = factura.Id,
                Numero = factura.Numero,
                Importe = factura.Importe,
                Proveedor = factura.ProveedorNavigation.Nombre,
                Responsable = factura.ResponsableAprobarNavigation.Nombre,
                ResponsableId = factura.ResponsableAprobarNavigation.Id,
                PermisoResponsable = factura.ResponsableAprobarNavigation.Id == usr,
                Proyecto = factura.ProyectoNavigation.Nombre,
                SociedadId = factura.Sociedad,
                Sociedad = factura.SociedadNavigation.Nombre,
                DescuentoProntoPago = factura.DescuentoProntoPago,
                FechaContable = factura.FechaFactura.HasValue ? factura.FechaFactura.Value.ToString("yyyy-MM-dd") : factura.FechaFacturaProforma.Value.ToString("yyyy-MM-dd"),
                FechaVencimiento = factura.FechaVencimiento?.ToString("yyyy-MM-dd"),
                FechaAlta = factura.FechaAlta.ToString("yyyy-MM-dd"),
                EstadoId = factura.FacturaEstadoFacturas.LastOrDefault().EstadoFacturaNavigation.Id,
                EstadoInterno = factura.FacturaEstadoFacturas.LastOrDefault().EstadoFacturaNavigation.EstadoInterno,
                EstadoProveedor = factura.FacturaEstadoFacturas.LastOrDefault().EstadoFacturaNavigation.EstadoProveedor,
                EstadoInicial = factura.FacturaEstadoFacturas.LastOrDefault().EstadoFacturaNavigation.EstadoInicial,
                EstadoFinal = factura.FacturaEstadoFacturas.LastOrDefault().EstadoFacturaNavigation.EstadoFinal
            });
        }

        return listaFacturaResponse;
    }

    public async Task<IEnumerable<FacturaResponse>> GetFacturasByEstado(int usr, string usrtype, int estado)
    {
        var sociedadesUsuario = _sociedadService.GetSociedadesByUsuario(usr);

        var facturas = _context.Facturas
            .Include(i => i.ProveedorNavigation)
            .Include(i => i.SociedadNavigation)
            .Include(i => i.ProyectoNavigation)
            .Include(i => i.ResponsableAprobarNavigation)
            .Include(i => i.FacturaEstadoFacturas).ThenInclude(i => i.EstadoFacturaNavigation)
            .Where(w => sociedadesUsuario.Contains(w.Sociedad) && w.FacturaEstadoFacturas.OrderByDescending(o => o.Id).FirstOrDefault().EstadoFactura == estado 
                && (w.ResponsableAprobar == usr || w.ProyectoNavigation.Responsable == usr || w.FacturaEstadoFacturas.Any(ww => ww.Usuario == usr && ww.EstadoFactura != 1)));
        //var sql = facturas.ToQueryString();
        if (facturas is null) return Enumerable.Empty<FacturaResponse>();

        List <FacturaResponse> listaFacturaResponse = new();
        foreach (Factura factura in facturas)
        {
            listaFacturaResponse.Add(new FacturaResponse
            {
                Id = factura.Id,
                Numero = factura.Numero,
                Importe = factura.Importe,
                Proveedor = factura.ProveedorNavigation.Nombre,
                Responsable = factura.ResponsableAprobarNavigation.Nombre,
                ResponsableId = factura.ResponsableAprobarNavigation.Id,
                PermisoResponsable = factura.ResponsableAprobarNavigation.Id == usr,
                Proyecto = factura.ProyectoNavigation.Nombre,
                SociedadId = factura.Sociedad,
                Sociedad = factura.SociedadNavigation.Nombre,
                DescuentoProntoPago = factura.DescuentoProntoPago,
                FechaContable = factura.FechaFactura.HasValue ? factura.FechaFactura.Value.ToString("yyyy-MM-dd") : factura.FechaFacturaProforma.Value.ToString("yyyy-MM-dd"),
                FechaVencimiento = factura.FechaVencimiento?.ToString("yyyy-MM-dd"),
                FechaAlta = factura.FechaAlta.ToString("yyyy-MM-dd"),
                EstadoId = factura.FacturaEstadoFacturas.LastOrDefault().EstadoFacturaNavigation.Id,
                EstadoInterno = factura.FacturaEstadoFacturas.LastOrDefault().EstadoFacturaNavigation.EstadoInterno,
                EstadoProveedor = factura.FacturaEstadoFacturas.LastOrDefault().EstadoFacturaNavigation.EstadoProveedor,
                EstadoInicial = factura.FacturaEstadoFacturas.LastOrDefault().EstadoFacturaNavigation.EstadoInicial,
                EstadoFinal = factura.FacturaEstadoFacturas.LastOrDefault().EstadoFacturaNavigation.EstadoFinal
            });
        }

        return listaFacturaResponse;
    }
    public async Task AltaFactura(int usr, string usrtype, AltaFacturaRequest dto)
    {
        if (dto.FicheroFactura is null && dto.FicheroFacturaProforma is null) throw new AppException("El fichero no existe");
        if (dto.FicheroFactura is not null && dto.FicheroFacturaProforma is not null) throw new AppException("Debe subir los ficheros de factura y proforma en tiempos diferentes");
        if (usrtype == "usuario" && !_context.Usuarios.Include(i => i.RolSociedadUsuarios).Any(w => w.Id == usr && w.RolSociedadUsuarios.Any(s => s.Sociedad == dto.Sociedad))) throw new AppException("La sociedad no existe");
        if (!_sociedadService.GetSociedadesByUsuario(usr).Contains(dto.Sociedad)) throw new AppException("No tiene permiso para dar de alta una factura en esta sociedad");
        if (usrtype == "proveedor" && usr != dto.Proveedor) throw new AppException("El proveedor no existe");
        if (!_context.Proveedors.Any(x => x.Id == dto.Proveedor)) throw new AppException("El proveedor no existe");
        if (_context.Facturas.Any(x => x.Numero == dto.Numero && x.Proveedor == dto.Proveedor && x.Sociedad == dto.Sociedad)) throw new AppException("Ya existe una factura con el número '" + dto.Numero + "' para este Proveedor y Sociedad");
        
        var proyecto = _context.Proyectos.Include(s => s.SociedadNavigation).ThenInclude(s => s.ClienteNavigation).FirstOrDefault(x => x.Id == dto.Proyecto && x.Sociedad == dto.Sociedad);
        if (proyecto is null) throw new AppException("El proyecto no existe");

        int? flujoAprobacionFactura = null;
        if (dto.FicheroFactura is null)
        {
            flujoAprobacionFactura = proyecto.SociedadNavigation.FlujoAprobacionFacturaProforma;
        }
        else
        {
            flujoAprobacionFactura = proyecto.SociedadNavigation.FlujoAprobacionFactura;
            if (proyecto.FlujoAprobacionFactura is not null) flujoAprobacionFactura = proyecto.FlujoAprobacionFactura.Value;
        }
        if (flujoAprobacionFactura is null) throw new AppException("La factura no se puede asignar a un flujo de aprobación");
        
        var flujoEstadoFacturaDestino = _context.FlujoEstadoFacturas.Include(i => i.EstadoOrigenNavigation).FirstOrDefault(w => w.FlujoAprobacionFactura == flujoAprobacionFactura.Value && w.EstadoOrigenNavigation.EstadoInicial);
        if (flujoEstadoFacturaDestino is null) throw new AppException("La factura no se puede asignar a un responsable aprobador");

        var ficheroFacturaNombre = "";
        var ficheroFacturaRuta = "";
        var ficheroFacturaProformaNombre = "";
        var ficheroFacturaProformaRuta = "";

        var lFacturaPedido = new List<FacturaPedido>();
        if (dto.Pedidos is not null)
        {            
            foreach (var pedido in dto.Pedidos)
            {
                if (!_context.Pedidos.Any(x => x.Id == pedido && x.Proveedor == dto.Proveedor && x.Sociedad == dto.Sociedad)) throw new AppException("El Pedido no existe");

                lFacturaPedido.Add(new FacturaPedido()
                {
                    Pedido = pedido,
                    FechaAlta = DateTime.Now
                });
            }
        }

        BlobServiceClient blobServiceClient = new BlobServiceClient(new Uri($"https://{_appSettings.Azure.BlobStorage.AccountName}.blob.core.windows.net"),
            new StorageSharedKeyCredential(_appSettings.Azure.BlobStorage.AccountName, _appSettings.Azure.BlobStorage.AccountKey));
        var storageContainer = blobServiceClient.GetBlobContainerClient("portalproveedor");
        if (!await storageContainer.ExistsAsync()) await blobServiceClient.CreateBlobContainerAsync("portalproveedor");

        if (dto.FicheroFactura is not null)
        {
            ficheroFacturaNombre = $"Factura_{DateTime.Now:yyyyMMdd_hhmmss}_{Guid.NewGuid().ToString("N").Substring(0, 10)}.pdf";
            ficheroFacturaRuta = $"{proyecto.SociedadNavigation.Cliente}/{proyecto.Sociedad}/{dto.Proveedor}/{dto.FechaFactura.Value.Year}/{dto.Numero}/{ficheroFacturaNombre}";

            await using var memoryStream = new MemoryStream();
            await dto.FicheroFactura.CopyToAsync(memoryStream);
            //var f = memoryStream.ToArray();

            memoryStream.Position = 0;
            BlobClient blobClient = storageContainer.GetBlobClient(ficheroFacturaRuta);
            await blobClient.UploadAsync(memoryStream, true);
        }

        if (dto.FicheroFacturaProforma is not null)
        {
            ficheroFacturaProformaNombre = $"Proforma_{DateTime.Now:yyyyMMdd_hhmmss}_{Guid.NewGuid().ToString("N").Substring(0, 10)}.pdf";
            ficheroFacturaProformaRuta = $"{proyecto.SociedadNavigation.Cliente}/{proyecto.Sociedad}/{dto.Proveedor}/{dto.FechaFacturaProforma.Value.Year}/{dto.Numero}/{ficheroFacturaProformaNombre}";

            await using var memoryStream = new MemoryStream();
            await dto.FicheroFacturaProforma.CopyToAsync(memoryStream);
            //var f = memoryStream.ToArray();

            memoryStream.Position = 0;
            BlobClient blobClient = storageContainer.GetBlobClient(ficheroFacturaProformaRuta);
            await blobClient.UploadAsync(memoryStream, true);
        }

        var lAlbaran = new List<Albaran>();
        if (dto.FicheroAlbaran is not null)
        {
            foreach (var albaran in dto.FicheroAlbaran)
            {
                var ficheroAlbaranNombre = $"Albarán_{DateTime.Now:yyyyMMdd_hhmmss}_{Guid.NewGuid().ToString("N").Substring(0, 10)}.pdf";
                var ficheroAlbaranRuta = $"{proyecto.SociedadNavigation.Cliente}/{proyecto.Sociedad}/{dto.Proveedor}/{dto.FechaFactura.Value.Year}/{dto.Numero}/{ficheroAlbaranNombre}";

                lAlbaran.Add(new Albaran
                {
                    FicheroAlbaranNavigation = new Fichero() { Nombre = ficheroAlbaranNombre, Ruta = ficheroAlbaranRuta, FechaAlta = DateTime.Now },
                    FechaAlta = DateTime.Now
                });

                await using var memoryStreamAlbaran = new MemoryStream();
                await albaran.CopyToAsync(memoryStreamAlbaran);
                memoryStreamAlbaran.Position = 0;
                BlobClient blobClientAlbaran = storageContainer.GetBlobClient(ficheroAlbaranRuta);
                await blobClientAlbaran.UploadAsync(memoryStreamAlbaran, true);
            }
        }

        var responsableId = 0;
        var proyectosData = _context.Proyectos
            .Include(i => i.UsuarioNavigation)
            .FirstOrDefault(w => w.Id == dto.Proyecto);

        responsableId = proyectosData.Responsable.GetValueOrDefault(0);

        Factura model = new()
        {
            Numero = dto.Numero,
            Importe = dto.Importe,
            FechaFactura = dto.FechaFactura,
            FechaFacturaProforma = dto.FechaFacturaProforma,
            Sociedad = dto.Sociedad,
            Proyecto = dto.Proyecto,
            Proveedor = dto.Proveedor,
            ResponsableAprobar = responsableId > 0 ? responsableId : flujoEstadoFacturaDestino.Aprobador,
            FacturaEstadoFacturas = new List<FacturaEstadoFactura> { new FacturaEstadoFactura() { Usuario = responsableId > 0 ? responsableId : flujoEstadoFacturaDestino.Aprobador, EstadoFactura = flujoEstadoFacturaDestino.EstadoOrigen, FechaAlta = DateTime.Now } },
            Albarans = lAlbaran,
            DescuentoProntoPago = dto.DescuentoProntoPago,
            FechaVencimiento = dto.FechaVencimiento,
            FacturaPedidos = lFacturaPedido,
            FechaAlta = DateTime.Now
        };

        if (dto.FicheroFactura is not null)
        {
            model.FicheroFacturaNavigation = new Fichero() { Nombre = ficheroFacturaNombre, Ruta = ficheroFacturaRuta, FechaAlta = DateTime.Now };
        }

        if (dto.FicheroFacturaProforma is not null)
        {
            model.FicheroFacturaProformaNavigation = new Fichero() { Nombre = ficheroFacturaProformaNombre, Ruta = ficheroFacturaProformaRuta, FechaAlta = DateTime.Now };
        }

        _context.Facturas.Add(model);
        _context.SaveChanges();

        var idFactura = model.Id;
    }
    public async Task CancelFactura(int usr, string usrtype, int id)
    {
        var sociedadesUsuario = _sociedadService.GetSociedadesByUsuario(usr);
        var factura = _context.Facturas
            .Include(i => i.FacturaEstadoFacturas)
            .Include(i => i.SociedadNavigation)
            .ThenInclude(t => t.LoginProveedorSociedads)
            .Include(ii => ii.SociedadNavigation)
            .ThenInclude(tt => tt.RolSociedadUsuarios)
            .FirstOrDefault(x => x.Id == id && sociedadesUsuario.Contains(x.Sociedad));
        if (factura is null) throw new AppException("La factura no existe");
        if (factura.FacturaEstadoFacturas.OrderByDescending(o => o.Id).FirstOrDefault().EstadoFactura != 1) throw new AppException("La factura no puede ser cancelada");

        switch (usrtype)
        {
            case "proveedor":
                if (factura.Proveedor != usr && !factura.SociedadNavigation.LoginProveedorSociedads.Any(a => a.Sociedad == factura.Sociedad)) throw new AppException("La factura no existe");
                break;
            case "usuario":
                if (!factura.SociedadNavigation.RolSociedadUsuarios.Any(a => a.Sociedad == factura.Sociedad)) throw new AppException("La factura no existe");
                break;
        }

        _context.FacturaEstadoFacturas.Add(new FacturaEstadoFactura() { Factura = id, Usuario = usr, EstadoFactura = 7, FechaAlta = DateTime.Now });
        _context.SaveChanges();
    }
    public async Task ActualizarEstadoFactura(int usr, string usrtype, int id, byte estado, string comentario)
    {
        var factura = _context.Facturas
            .Include(i => i.FacturaEstadoFacturas).ThenInclude(t => t.EstadoFacturaNavigation)
            .Include(ii => ii.SociedadNavigation).ThenInclude(tt => tt.LoginProveedorSociedads)
            .Include(ii => ii.SociedadNavigation).ThenInclude(tt => tt.FlujoAprobacionFacturas)
            .Include(ii => ii.SociedadNavigation).ThenInclude(tt => tt.RolSociedadUsuarios)
            .Include(ii => ii.ProyectoNavigation).ThenInclude(tt => tt.FlujoAprobacionFacturas)
            .FirstOrDefault(x => x.Id == id);
        if (factura is null) throw new AppException("La factura no existe");
        if (factura.FacturaEstadoFacturas.OrderByDescending(o => o.Id).FirstOrDefault().EstadoFacturaNavigation.EstadoFinal) throw new AppException("La factura no puede ser actualizada");

        switch (usrtype)
        {
            case "proveedor":
                if (factura.Proveedor != usr && !factura.SociedadNavigation.LoginProveedorSociedads.Any(a => a.Sociedad == factura.Sociedad)) throw new AppException("La factura no existe");
                break;
            case "usuario":
                if (!factura.SociedadNavigation.RolSociedadUsuarios.Any(a => a.Sociedad == factura.Sociedad)) throw new AppException("La factura no existe");
                break;
        }

        int? flujoAprobacionFactura = null;
        if (factura.FicheroFactura is null)
        {
            flujoAprobacionFactura = factura.SociedadNavigation.FlujoAprobacionFacturaProforma;
        }
        else
        {
            flujoAprobacionFactura = factura.SociedadNavigation.FlujoAprobacionFactura;
            if (factura.ProyectoNavigation.FlujoAprobacionFactura is not null) flujoAprobacionFactura = factura.ProyectoNavigation.FlujoAprobacionFactura.Value;
        }
        if (flujoAprobacionFactura is null) throw new AppException("La factura no se puede actualizar");


        bool isvalid = false;
        var estadoFactura =  factura.FacturaEstadoFacturas.OrderByDescending(o => o.Id).FirstOrDefault().EstadoFactura;
        var flujoEstadoFactura = _context.FlujoEstadoFacturas.Where(w => w.FlujoAprobacionFactura == flujoAprobacionFactura.Value && w.EstadoOrigen == estadoFactura && w.EstadoDestino == estado);
        if (flujoEstadoFactura is null || !flujoEstadoFactura.Any()) throw new AppException("La factura no se puede actualizar");
        foreach (var flujo in flujoEstadoFactura)
        {
            if (flujo.WebProveedor && usrtype == "proveedor") {isvalid = true; break; };
            if (flujo.WebGestion && usrtype == "usuario") {isvalid = true; break; };
        }

        if (!isvalid) throw new AppException("La factura no se puede actualizar");

        _context.FacturaEstadoFacturas.Add(new FacturaEstadoFactura() { Factura = id, Usuario = usr, EstadoFactura = estado, Comentario = comentario, FechaAlta = DateTime.Now });
        _context.SaveChanges();

        var flujoEstadoFacturaDestino = _context.FlujoEstadoFacturas.FirstOrDefault(w => w.FlujoAprobacionFactura == flujoAprobacionFactura.Value && w.EstadoOrigen == estado);
        if (flujoEstadoFacturaDestino is not null) {
            _context.Facturas.Where(w => w.Id == id).ExecuteUpdate(b => b.SetProperty(u => u.ResponsableAprobar, flujoEstadoFacturaDestino.Aprobador));
        }
    }

    public async Task ActualizarFactura(int usr, string usrtype, int id, int proyecto, int aprobador)
    {
        var aprobadorexist = _context.Usuarios.FirstOrDefault(x => x.Id == aprobador);
        if (aprobadorexist is null) throw new AppException("El aprobador no existe");

        var sociedadesUsuario = _sociedadService.GetSociedadesByUsuario(usr);
        var factura = _context.Facturas
            .Include(i => i.FacturaEstadoFacturas).ThenInclude(t => t.EstadoFacturaNavigation)
            .Include(ii => ii.SociedadNavigation).ThenInclude(tt => tt.LoginProveedorSociedads)
            .Include(ii => ii.SociedadNavigation).ThenInclude(tt => tt.RolSociedadUsuarios)
            .FirstOrDefault(x => x.Id == id && sociedadesUsuario.Contains(x.Sociedad));
        if (factura is null) throw new AppException("La factura no existe");
        if (factura.FacturaEstadoFacturas.OrderByDescending(o => o.Id).FirstOrDefault().EstadoFacturaNavigation.EstadoFinal) throw new AppException("La factura no puede ser actualizada");

        var proyectoexist = _context.Proyectos.FirstOrDefault(x => x.Id == proyecto && x.Sociedad == factura.Sociedad);
        if (proyectoexist is null) throw new AppException("El proyecto no existe");

        switch (usrtype)
        {
            case "proveedor":
                if (factura.Proveedor != usr && !factura.SociedadNavigation.LoginProveedorSociedads.Any(a => a.Sociedad == factura.Sociedad)) throw new AppException("La factura no existe");
                break;
            case "usuario":
                if (!factura.SociedadNavigation.RolSociedadUsuarios.Any(a => a.Sociedad == factura.Sociedad)) throw new AppException("La factura no existe");
                break;
        }

        _context.Facturas.Where(w => w.Id == id).ExecuteUpdate(b => b.SetProperty(u => u.Proyecto, proyecto).SetProperty(u => u.ResponsableAprobar, aprobador));
    }
}