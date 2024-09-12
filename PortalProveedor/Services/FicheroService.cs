namespace PortalProveedor.Services;

using AutoMapper;
using Azure.Storage;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PortalProveedor.Database;
using PortalProveedor.Helpers;
using PortalProveedor.Models.Facturas;
using System.IO;

public interface IFicheroService
{
    public Task<BlobDto?> GetFichero(int id, int usr, string usrtype);
    public Task<string> GetFileUrl(int id, int usr, string usrtype);
}

public class FicheroService : IFicheroService
{
    private PortalProveedorContext _context;
    private ISociedadService _sociedadService;
    private IProveedorService _proveedorService;
    private IProyectoService _proyectoService;
    private IUsuarioService _usuarioService;
    private readonly AppSettings _appSettings;

    public FicheroService(
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

    public async Task<string> GetFileUrl(int id, int usr, string usrtype)
    {
        var fichero = await _context.Ficheroes
            .Include(i => i.FacturaFicheroFacturaNavigations)
            .Include(i => i.FacturaFicheroFacturaProformaNavigations)
            .Include(i => i.Albarans)
            .FirstOrDefaultAsync(w => w.Id == id);// && w.Factura.Any(f => f.ResponsableAprobar == usr));
        if (fichero is null) return null;

        BlobServiceClient blobServiceClient = new BlobServiceClient(
            new Uri($"https://{_appSettings.Azure.BlobStorage.AccountName}.blob.core.windows.net"),
            new StorageSharedKeyCredential(_appSettings.Azure.BlobStorage.AccountName, _appSettings.Azure.BlobStorage.AccountKey));
        var storageContainer = blobServiceClient.GetBlobContainerClient("portalproveedor");
        if (!await storageContainer.ExistsAsync()) await blobServiceClient.CreateBlobContainerAsync("portalproveedor");

        BlobClient blobClient = storageContainer.GetBlobClient(fichero.Ruta);
        if (await blobClient.ExistsAsync())
        {
            return blobClient.Uri.ToString();
        }

        return null;
    }
    public async Task<BlobDto?> GetFichero(int id, int usr, string usrtype)
    {
        var fichero = await _context.Ficheroes
            .Include(i => i.FacturaFicheroFacturaNavigations)
            .Include(i => i.FacturaFicheroFacturaProformaNavigations)
            .Include(i => i.Albarans)
            .FirstOrDefaultAsync(w => w.Id == id);// && w.Factura.Any(f => f.ResponsableAprobar == usr));
        if (fichero is null) return null;

        BlobServiceClient blobServiceClient = new BlobServiceClient(
            new Uri($"https://{_appSettings.Azure.BlobStorage.AccountName}.blob.core.windows.net"),
            new StorageSharedKeyCredential(_appSettings.Azure.BlobStorage.AccountName, _appSettings.Azure.BlobStorage.AccountKey));
        var storageContainer = blobServiceClient.GetBlobContainerClient("portalproveedor");
        if (!await storageContainer.ExistsAsync()) await blobServiceClient.CreateBlobContainerAsync("portalproveedor");

        BlobClient blobClient = storageContainer.GetBlobClient(fichero.Ruta);
        if (await blobClient.ExistsAsync())
        {
            string name = fichero.Nombre;
            var content = await blobClient.DownloadContentAsync();            
            string contentType = content.Value.Details.ContentType;

            return new BlobDto { Content = content.Value.Content.ToStream(), Name = name, ContentType = contentType };
        }

        return null;
    }
}