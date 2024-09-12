namespace PortalProveedor.Services;

using Microsoft.EntityFrameworkCore;
using PortalProveedor.Database;
using PortalProveedor.Entities;
using PortalProveedor.Helpers;
using PortalProveedor.Models.Sociedades;

public interface ISociedadService
{
    public List<int> GetSociedadesByUsuario(int usr);
    public Task AltaSociedad(int usr, AltaSociedadRequest dto);
    public Task ActualizarSociedad(int id, int usr, AltaSociedadRequest dto);
    public Task EliminarSociedad(int id, int usr);
    IEnumerable<ListaSociedadResponse> GetSociedades(int usr, IEnumerable<int> sociedad, BusquedaSociedadRequest request);
    IEnumerable<ListaSociedadResponse> GetSociedadesResponseByUsuario(int usr);
}

public class SociedadService : ISociedadService
{
    private PortalProveedorContext _context;

    public SociedadService(PortalProveedorContext context)
    {
        _context = context;
    }

    public IEnumerable<Sociedad> GetSociedades()
    {
        var sociedades = _context.Sociedads.ToList();
        return sociedades;
    }

    public List<int> GetSociedadesByUsuario(int usr)
    {
        var sociedadesUsuario = _context.RolSociedadUsuarios
            .Include(i => i.UsuarioNavigation)
            .Where(w => w.Usuario == usr).Select(x => x.Sociedad).ToList();
        return sociedadesUsuario;
    }

    public IEnumerable<ListaSociedadResponse> GetSociedadesResponseByUsuario(int usr)
    {
        var sociedades = _context.Sociedads
            .Include(i => i.RolSociedadUsuarios).ThenInclude(i => i.UsuarioNavigation)
            .Include(i => i.ClienteNavigation)
            .Where(w => w.RolSociedadUsuarios.Any(rsu => rsu.Usuario == usr));

        List<ListaSociedadResponse> listaSociedadesResponse = new();
        foreach (Sociedad item in sociedades)
        {
            listaSociedadesResponse.Add(new ListaSociedadResponse
            {
                Id = item.Id,
                Nombre = item.Nombre,
                Identificador = item.Identificador,
                TipoIdentificador = item.TipoIdentificador,
                EmailNotifcaciones = item.EmailNotifcaciones,
                EmailProcesoFacturas = item.EmailProcesoFacturas,
                ClienteId = item.Cliente,
                Cliente = item.ClienteNavigation.Nombre
            });

        }
        return listaSociedadesResponse;
    }

    public IEnumerable<ListaSociedadResponse> GetSociedades(int usr, IEnumerable<int> sociedad, BusquedaSociedadRequest request)
    {
        var sociedades = _context.Sociedads
            .Include(i => i.LoginProveedorSociedads).ThenInclude(ii => ii.ProveedorNavigation)
            .Include(i => i.ClienteNavigation)
            .Where(w => //w.LoginProveedorSociedads.Any(a => sociedad.Contains(a.Sociedad))
            (!request.Id.HasValue || w.Id == request.Id)
            && (string.IsNullOrEmpty(request.Nombre) || w.Nombre == request.Nombre)
            && (string.IsNullOrEmpty(request.Identificador) || w.Identificador == request.Identificador)
            && (!request.TipoIdentificador.HasValue || w.TipoIdentificador == request.TipoIdentificador)
            && (string.IsNullOrEmpty(request.EmailNotifcaciones) || w.EmailNotifcaciones == request.EmailNotifcaciones)
            && (string.IsNullOrEmpty(request.EmailProcesoFacturas) || w.EmailProcesoFacturas == request.EmailProcesoFacturas)
            && (!request.Cliente.HasValue || w.Cliente == request.Cliente)
            && (!request.Proveedor.HasValue || w.LoginProveedorSociedads.Any(a => a.Proveedor == request.Proveedor))
            //&& (!w.Borrado)
            ).ToList();

        List<ListaSociedadResponse> listaSociedadesResponse = new();
        foreach (Sociedad item in sociedades)
        {
            listaSociedadesResponse.Add(new ListaSociedadResponse
            {
                Id = item.Id,
                Nombre = item.Nombre,
                Identificador = item.Identificador,
                TipoIdentificador = item.TipoIdentificador,
                EmailNotifcaciones = item.EmailNotifcaciones,
                EmailProcesoFacturas = item.EmailProcesoFacturas,
                ClienteId = item.Cliente,
                Cliente = item.ClienteNavigation.Nombre
            });

        }
        return listaSociedadesResponse;
    }

    public async Task AltaSociedad(int usr, AltaSociedadRequest dto)
    {
        if (!_context.Clientes.Any(x => x.Id == dto.Cliente)) throw new AppException("El Cliente no existe");
        if (_context.Sociedads.Any(x => x.Cliente == dto.Cliente && x.Identificador == dto.Identificador && x.TipoIdentificador == dto.TipoIdentificador)) throw new AppException("Ya existe una sociedad con el identificador '" + dto.Identificador + " " + dto.TipoIdentificador + "'  para este Cliente");

        Sociedad model = new()
        {
            Nombre = dto.Nombre,
            Identificador = dto.Identificador,
            TipoIdentificador = dto.TipoIdentificador,
            EmailNotifcaciones = dto.EmailNotifcaciones,
            EmailProcesoFacturas = dto.EmailProcesoFacturas,
            Cliente = dto.Cliente,
            FechaAlta = DateTime.Now
        };

        _context.Sociedads.Add(model);
        _context.SaveChanges();
    }
    public async Task ActualizarSociedad(int id, int usr, AltaSociedadRequest dto)
    {
        Sociedad sociedad = _context.Sociedads.FirstOrDefault(w => w.Id == id);
        if (sociedad is null) throw new AppException("La Sociedad no existe");

        if (!_context.Clientes.Any(x => x.Id == dto.Cliente)) throw new AppException("El Cliente no existe");
        if (_context.Sociedads.Any(x => x.Id != id && x.Cliente == dto.Cliente && x.Identificador == dto.Identificador && x.TipoIdentificador == dto.TipoIdentificador)) throw new AppException("Ya existe una sociedad con el identificador '" + dto.Identificador + " " + dto.TipoIdentificador + "'  para este Cliente");

        sociedad.Nombre = dto.Nombre;
        sociedad.Identificador = dto.Identificador;
        sociedad.TipoIdentificador = dto.TipoIdentificador;
        sociedad.EmailNotifcaciones = dto.EmailNotifcaciones;
        sociedad.EmailProcesoFacturas = dto.EmailProcesoFacturas;
        sociedad.Cliente = dto.Cliente;

        _context.Sociedads.Update(sociedad);
        _context.SaveChanges();
    }
    public async Task EliminarSociedad(int id, int usr)
    {
        Sociedad sociedad = _context.Sociedads.FirstOrDefault(w => w.Id == id);
        if (sociedad is null) throw new AppException("La Sociedad no existe");

        //sociedad.Borrado = true;

        _context.Entry(sociedad).State = EntityState.Modified;
        _context.Sociedads.Update(sociedad);
        _context.SaveChanges();
    }
}