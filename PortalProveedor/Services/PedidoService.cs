namespace PortalProveedor.Services;

using Microsoft.EntityFrameworkCore;
using PortalProveedor.Database;
using BCrypt.Net;
using PortalProveedor.Entities;
using PortalProveedor.Models.Pedidos;
using PortalProveedor.Models.Facturas;
using static PortalProveedor.Models.Dtos.Dtos;
using PortalProveedor.Helpers;
using PortalProveedor.Models.Proyectos;
using Azure.Storage.Blobs;
using Azure.Storage;
using System.Runtime.InteropServices.JavaScript;
using Microsoft.Extensions.Options;
using PortalProveedor.Models.Dtos;

public interface IPedidoService
{
    public Pedido GetById(int id);
    public Task<PedidoResponse> GetPedido(int usr, string usrtype, int id);
    public Task<IEnumerable<PedidoResponse>> GetPedidosBySociedadAndProveedor(int usr, string usrtype, int? sociedad, int? proveedor, int? numero);
    public Task AltaPedido(int usr, string usrtype, AltaPedidoRequest dto);
    public Task ActualizarPedido(int id, int usr, string usrtype, EditPedidoRequest dto);
    public Task<IEnumerable<PedidoResponse>> GetPedidos(int usr, IEnumerable<int> sociedadusr);
}

public class PedidoService : IPedidoService
{
    private PortalProveedorContext _context;
    private ISociedadService _sociedadService;
    private readonly AppSettings _appSettings;

    public PedidoService(
        PortalProveedorContext context, ISociedadService sociedadService, IOptions<AppSettings> appSettings)
    {
        _context = context;
        _appSettings = appSettings.Value;
        _sociedadService =sociedadService;
    }

    public async Task AltaPedido(int usr, string usrtype, AltaPedidoRequest dto)
    {
        if (dto.FicheroPedido is null) throw new AppException("El fichero no existe");
        if (usrtype == "usuario" && !_context.Usuarios.Include(i => i.RolSociedadUsuarios).Any(w => w.Id == usr && w.RolSociedadUsuarios.Any(s => s.Sociedad == dto.Sociedad))) throw new AppException("La sociedad no existe");
        if (usrtype == "proveedor" && usr != dto.Proveedor) throw new AppException("El proveedor no existe");
        if (!_context.Proveedors.Any(x => x.Id == dto.Proveedor)) throw new AppException("El proveedor no existe");
        if (_context.Pedidos.Any(x => x.NumeroPedido == dto.NumeroPedido && x.Proveedor == dto.Proveedor && x.Sociedad == dto.Sociedad)) throw new AppException("Ya existe un pedido con el número '" + dto.NumeroPedido + "' para este Proveedor y Sociedad");

        var sociedad = _context.Sociedads.FirstOrDefault(x => x.Id == dto.Sociedad);
        if (sociedad is null) throw new AppException("La Sociedad no existe");

        var ficheroPedidoNombre = $"Pedido_{DateTime.Now:yyyyMMdd_hhmmss}_{Guid.NewGuid().ToString("N").Substring(0, 10)}.pdf";
        var ficheroPedidoRuta = $"{sociedad.Cliente}/{dto.Sociedad}/{dto.Proveedor}/{dto.NumeroPedido}/{ficheroPedidoNombre}";
        
        /*
        var lFacturaPedido = new List<FacturaPedido>();
        foreach (var factura in dto.Facturas)
        {
            if (!_context.Facturas.Any(x => x.Id == factura && x.Proveedor == dto.Proveedor && x.Sociedad == dto.Sociedad)) throw new AppException("La Factura no existe");

            lFacturaPedido.Add(new FacturaPedido()
            {
                Factura = factura,
                FechaAlta = DateTime.Now
            });
        }*/

        BlobServiceClient blobServiceClient = new BlobServiceClient(new Uri($"https://{_appSettings.Azure.BlobStorage.AccountName}.blob.core.windows.net"),
            new StorageSharedKeyCredential(_appSettings.Azure.BlobStorage.AccountName, _appSettings.Azure.BlobStorage.AccountKey));
        var storageContainer = blobServiceClient.GetBlobContainerClient("portalproveedor");
        if (!await storageContainer.ExistsAsync()) await blobServiceClient.CreateBlobContainerAsync("portalproveedor");

        if (dto.FicheroPedido is not null)
        {
            await using var memoryStream = new MemoryStream();
            await dto.FicheroPedido.CopyToAsync(memoryStream);

            memoryStream.Position = 0;
            BlobClient blobClient = storageContainer.GetBlobClient(ficheroPedidoRuta);
            await blobClient.UploadAsync(memoryStream, true);
        }

        Pedido model = new()
        {
            NumeroPedido = dto.NumeroPedido,
            Sociedad = dto.Sociedad,
            Proveedor = dto.Proveedor,
            FicheroPedidoNavigation = new Fichero() { Nombre = ficheroPedidoNombre, Ruta = ficheroPedidoRuta, FechaAlta = DateTime.Now },
            //FacturaPedidos = lFacturaPedido,
            FechaAlta = DateTime.Now
        };

        _context.Pedidos.Add(model);
        _context.SaveChanges();

        var idPedido = model.Id;
    }

    public async Task<PedidoResponse> GetPedido(int usr, string usrtype, int id)
    {

        var sociedadesUsuario = _context.RolSociedadUsuarios
            .Include(i => i.SociedadNavigation)
            .Include(i => i.UsuarioNavigation)
            .Where(w => w.Usuario == usr).ToDictionary(dKey => dKey.Sociedad, dVal => dVal.SociedadNavigation.Nombre);

        var proveedores = _context.Proveedors.ToDictionary(dKey => dKey.Id, dVal => dVal.Nombre);

        var pedido = _context.Pedidos
            .Include(i => i.FacturaPedidos)
            .Include(i => i.FicheroPedidoNavigation)
            .Include(i => i.FacturaPedidos)
            .FirstOrDefault(w => w.Id == id);
        
        var lFacturaPedido = new List<FacturaPedidoResponse>();
        foreach (var factura in pedido.FacturaPedidos)
        {
            lFacturaPedido.Add(new FacturaPedidoResponse()
            {
                FacturaId = factura.Factura,
                FechaAlta = DateTime.Now
            });
        }

        var pedidoResponse = new PedidoResponse
        {
            Id = pedido.Id,
            NumeroPedido = pedido.NumeroPedido,
            Sociedad = pedido.Sociedad,
            SociedadName = sociedadesUsuario[pedido.Sociedad],
            Proveedor = pedido.Proveedor,
            ProveedorName = proveedores[pedido.Proveedor],
            FicheroPedido = new FicheroResponse() { Id = pedido.FicheroPedidoNavigation.Id, Nombre = pedido.FicheroPedidoNavigation.Nombre, Ruta = pedido.FicheroPedidoNavigation.Ruta },
            FacturaPedidos = lFacturaPedido,
            FechaAlta = pedido.FechaAlta?.ToString()
        };

        return pedidoResponse;
    }

    public async Task<IEnumerable<PedidoResponse>> GetPedidos(int usr, IEnumerable<int> sociedadusr)
    {
        var sociedadesUsuario = _context.RolSociedadUsuarios
            .Include(i => i.SociedadNavigation)
            .Include(i => i.UsuarioNavigation)
            .Where(w => w.Usuario == usr).ToDictionary(dKey => dKey.Sociedad, dVal => dVal.SociedadNavigation.Nombre);

        var proveedores = _context.Proveedors.ToDictionary(dKey => dKey.Id, dVal => dVal.Nombre);

        var pedidos = _context.Pedidos
            .Include(i => i.FacturaPedidos)
            .Include(i => i.FicheroPedidoNavigation)
            .Include(i => i.FacturaPedidos)
            .Where(w => sociedadusr.Contains(w.Sociedad));
        if (pedidos is null) return Enumerable.Empty<PedidoResponse>();

        List<PedidoResponse> listaPedidoResponse = new();
        foreach (Pedido pedido in pedidos)
        {
            var lFacturaPedido = new List<FacturaPedidoResponse>();
            foreach (var factura in pedido.FacturaPedidos)
            {
                lFacturaPedido.Add(new FacturaPedidoResponse()
                {
                    FacturaId = factura.Factura,
                    FechaAlta = DateTime.Now
                });
            }

            listaPedidoResponse.Add(new PedidoResponse
            {
                Id = pedido.Id,
                NumeroPedido = pedido.NumeroPedido,
                Sociedad = pedido.Sociedad,
                SociedadName = sociedadesUsuario[pedido.Sociedad],
                Proveedor = pedido.Proveedor,
                ProveedorName = proveedores[pedido.Proveedor],
                FicheroPedido = new FicheroResponse() { Id = pedido.FicheroPedidoNavigation.Id, Nombre = pedido.FicheroPedidoNavigation.Nombre, Ruta = pedido.FicheroPedidoNavigation.Ruta },
                FacturaPedidos = lFacturaPedido,
                FechaAlta = pedido.FechaAlta?.ToString()
            });
        }

        return listaPedidoResponse;
    }
    public async Task<IEnumerable<PedidoResponse>> GetPedidosBySociedadAndProveedor(int usr, string usrtype, int? sociedad, int? proveedor, int? numero)
    {
        var sociedadesUsuario = _context.RolSociedadUsuarios
            .Include(i => i.SociedadNavigation)
            .Include(i => i.UsuarioNavigation)
            .Where(w => w.Usuario == usr).ToDictionary(dKey => dKey.Sociedad, dVal => dVal.SociedadNavigation.Nombre);

        var proveedores = _context.Proveedors.ToDictionary(dKey => dKey.Id, dVal => dVal.Nombre);

        var pedidos = _context.Pedidos
            .Include(i => i.FacturaPedidos)
            .Include(i => i.FicheroPedidoNavigation)
            .Include(i => i.FacturaPedidos)
            .Where(w => (!sociedad.HasValue || w.Sociedad == sociedad) && (!proveedor.HasValue || w.Proveedor == proveedor) && (!numero.HasValue || w.NumeroPedido == numero));
        if (pedidos is null) return Enumerable.Empty<PedidoResponse>();

        List<PedidoResponse> listaPedidoResponse = new();
        foreach (Pedido pedido in pedidos)
        {
            var lFacturaPedido = new List<FacturaPedidoResponse>();
            foreach (var factura in pedido.FacturaPedidos)
            {
                lFacturaPedido.Add(new FacturaPedidoResponse()
                {
                    FacturaId = factura.Factura,
                    FechaAlta = DateTime.Now
                });
            }

            listaPedidoResponse.Add(new PedidoResponse
            {
                Id = pedido.Id,
                NumeroPedido = pedido.NumeroPedido,
                Sociedad = pedido.Sociedad,
                SociedadName = sociedadesUsuario[pedido.Sociedad],
                Proveedor = pedido.Proveedor,
                ProveedorName = proveedores[pedido.Proveedor],
                FicheroPedido = new FicheroResponse() { Id = pedido.FicheroPedidoNavigation.Id, Nombre = pedido.FicheroPedidoNavigation.Nombre, Ruta = pedido.FicheroPedidoNavigation.Ruta },
                FacturaPedidos = lFacturaPedido,
                FechaAlta = pedido.FechaAlta?.ToString()
            });
        }

        return listaPedidoResponse;
    }

    public async Task ActualizarPedido(int id, int usr, string usrtype, EditPedidoRequest dto)
    {
        if (usrtype == "usuario" && !_context.Usuarios.Include(i => i.RolSociedadUsuarios).Any(w => w.Id == usr && w.RolSociedadUsuarios.Any(s => s.Sociedad == dto.Sociedad))) throw new AppException("La sociedad no existe");
        if (usrtype == "proveedor" && usr != dto.Proveedor) throw new AppException("El proveedor no existe");
        if (!_context.Proveedors.Any(x => x.Id == dto.Proveedor)) throw new AppException("El proveedor no existe");
        if (_context.Pedidos.Any(x => x.NumeroPedido == dto.NumeroPedido && x.Proveedor == dto.Proveedor && x.Sociedad == dto.Sociedad && x.Id != id)) throw new AppException("Ya existe un pedido con el número '" + dto.NumeroPedido + "' para este Proveedor y Sociedad");

        var sociedad = _context.Sociedads.FirstOrDefault(x => x.Id == dto.Sociedad);
        if (sociedad is null) throw new AppException("La Sociedad no existe");

        Pedido pedido = _context.Pedidos.FirstOrDefault(x => x.Id == id);
        
        if (dto.FicheroPedido is not null)
        {
            var ficheroPedidoNombre = $"Pedido_{DateTime.Now:yyyyMMdd_hhmmss}_{Guid.NewGuid().ToString("N").Substring(0, 10)}.pdf";
            var ficheroPedidoRuta = $"{sociedad.Cliente}/{dto.Sociedad}/{dto.Proveedor}/{dto.NumeroPedido}/{ficheroPedidoNombre}";
            /*
            var lFacturaPedido = new List<FacturaPedido>();
            foreach (var factura in dto.Facturas)
            {
                if (!_context.Facturas.Any(x => x.Id == factura && x.Proveedor == dto.Proveedor && x.Sociedad == dto.Sociedad)) throw new AppException("La Factura no existe");

                lFacturaPedido.Add(new FacturaPedido()
                {
                    Factura = factura,
                    FechaAlta = DateTime.Now
                });
            }*/

            BlobServiceClient blobServiceClient = new BlobServiceClient(new Uri($"https://{_appSettings.Azure.BlobStorage.AccountName}.blob.core.windows.net"),
                new StorageSharedKeyCredential(_appSettings.Azure.BlobStorage.AccountName, _appSettings.Azure.BlobStorage.AccountKey));
            var storageContainer = blobServiceClient.GetBlobContainerClient("portalproveedor");
            if (!await storageContainer.ExistsAsync()) await blobServiceClient.CreateBlobContainerAsync("portalproveedor");

            await using var memoryStream = new MemoryStream();
            await dto.FicheroPedido.CopyToAsync(memoryStream);

            memoryStream.Position = 0;
            BlobClient blobClient = storageContainer.GetBlobClient(ficheroPedidoRuta);
            await blobClient.UploadAsync(memoryStream, true);

            pedido.FicheroPedidoNavigation = new Fichero() { Nombre = ficheroPedidoNombre, Ruta = ficheroPedidoRuta, FechaAlta = DateTime.Now };
        }


        
        pedido.NumeroPedido = dto.NumeroPedido;
        pedido.Sociedad = dto.Sociedad;
        pedido.Proveedor = dto.Proveedor;

        _context.Pedidos.Update(pedido);
        _context.SaveChanges();
    }

    public Pedido GetById(int id)
    {
        return GetPedido(id);
    }

    // helper methods
    private Pedido GetPedido(int id)
    {
        var pedido = _context.Pedidos.Find(id);
        if (pedido == null) throw new KeyNotFoundException("Pedido no encontrado");
        return pedido;
    }
}