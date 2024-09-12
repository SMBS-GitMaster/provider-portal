using Microsoft.EntityFrameworkCore;
using PortalProveedor.Database;
using PortalProveedor.Entities;
using PortalProveedor.Helpers;
using PortalProveedor.Models.Usuarios;
using PortalProveedor.Models.UsuarioTarifa;

namespace PortalProveedor.Services
{
    public interface IUsuarioTarifaService
    {
        public Task<IEnumerable<UsuarioTarifaResponse>> GetUsuarioTarifas();
        public Task<UsuarioTarifaResponse> GetById(int id);
        public Task AltaUsuarioTarifa(AltaUsuarioTarifaRequest request);
        public Task ActualizarUsuarioTarifa(int Id, EditUsuarioTarifaRequest request);
        public Task EliminarUsuarioTarifa(int Id);
        public DateTime ObtenerFormatoFecha(string fecha, bool esDiaInicial = true);
    }
    public class UsuarioTarifaService : IUsuarioTarifaService
    {
        private PortalProveedorContext _context;
        
        public UsuarioTarifaService(PortalProveedorContext context)
        {
            _context = context;
        }

        public async Task ActualizarUsuarioTarifa(int Id, EditUsuarioTarifaRequest dto)
        {
            UsuarioTarifa tarifa = _context.UsuarioTarifa.FirstOrDefault(w => w.Id == Id && w.Borrado == false) ?? throw new AppException("La tarifa del usuario no existe");
            DateTime FechaInicia = ObtenerFormatoFecha(dto.FechaInicia);
            DateTime FechaVence = ObtenerFormatoFecha(dto.FechaVence, false);

            if (!_context.Usuarios.Any(x => x.Id == dto.Usuario)) throw new AppException("El Usuario no existe");
            if (FechaInicia > FechaVence) throw new AppException("Fecha de inicio no puede ser mayor a la fecha de vencimiento");
            if (_context.UsuarioTarifa.Any(x => x.Usuario == dto.Usuario && x.Borrado == false && x.Id != Id
                && ((x.FechaInicia >= FechaInicia && x.FechaInicia <= FechaVence) ||
                      x.FechaInicia <= FechaInicia && x.FechaVence >= FechaInicia)))
                throw new AppException("Ya existe una tarifa dentro el rango de fecha seleccionada");

            tarifa.PrecioHora = dto.PrecioHora;
            tarifa.Usuario = dto.Usuario;
            tarifa.FechaInicia = FechaInicia;
            tarifa.FechaVence = FechaVence;
            _context.UsuarioTarifa.Update(tarifa);
            await _context.SaveChangesAsync();
        }

        public async Task AltaUsuarioTarifa(AltaUsuarioTarifaRequest dto)
        {
            DateTime FechaInicia = ObtenerFormatoFecha(dto.FechaInicia);
            DateTime FechaVence = ObtenerFormatoFecha(dto.FechaVence, false);

            if (!_context.Usuarios.Any(x => x.Id == dto.Usuario)) throw new AppException("El Usuario no existe");                  
            if (FechaInicia > FechaVence) throw new AppException("Fecha de inicio no puede ser mayor a la fecha de vencimiento");
            if (_context.UsuarioTarifa.Any(x => x.Usuario == dto.Usuario && x.Borrado == false
                && ((x.FechaInicia >= FechaInicia && x.FechaInicia <= FechaVence) ||
                      x.FechaInicia <= FechaInicia && x.FechaVence >= FechaInicia)))
                throw new AppException("Ya existe una tarifa dentro el rango de fecha seleccionada");

            UsuarioTarifa model = new()
            {
                Usuario = dto.Usuario,
                PrecioHora = dto.PrecioHora,
                FechaInicia = FechaInicia,
                FechaVence = FechaVence
            };
            _context.UsuarioTarifa.Add(model);
            await _context.SaveChangesAsync();
        }

        public async Task EliminarUsuarioTarifa(int Id)
        {
            UsuarioTarifa tarifa = _context.UsuarioTarifa.FirstOrDefault(w => w.Id == Id) ?? throw new AppException("La tarifa no existe");
            tarifa.Borrado = true;
            _context.Entry(tarifa).State = EntityState.Modified;
            _context.UsuarioTarifa.Update(tarifa);
           await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<UsuarioTarifaResponse>> GetUsuarioTarifas()
        {
            var tarifas = _context.UsuarioTarifa.Include( i => i.UsuarioNavigation)
                .Where( w => w.Borrado == false).ToList();
            if (tarifas is null) return Enumerable.Empty<UsuarioTarifaResponse>();
            List<UsuarioTarifaResponse> lUsuarioTarifa = new();
            foreach (UsuarioTarifa tarifa in tarifas)
            {
                UsuarioResponse usuario = new()
                {
                    id = tarifa.UsuarioNavigation.Id,
                    nombre = tarifa.UsuarioNavigation.Nombre,
                    email = tarifa.UsuarioNavigation.Email,
                    estado = tarifa.UsuarioNavigation.Estado,
                    ausente = tarifa.UsuarioNavigation.Ausente,
                };
                lUsuarioTarifa.Add(new UsuarioTarifaResponse
                {
                    Id = tarifa.Id,
                    Usuario = tarifa.Usuario,
                    UsuarioNavigation = usuario,
                    PrecioHora = tarifa.PrecioHora,
                    FechaInicia = tarifa.FechaInicia.ToString("yyyy-MM"),
                    FechaVence = tarifa.FechaVence.ToString("yyyy-MM"),
                });
            }
            return lUsuarioTarifa;
        }

        public async Task<UsuarioTarifaResponse> GetById(int id)
        {
            var tarifa = _context.UsuarioTarifa.Include(i => i.UsuarioNavigation)
                .Where(w => w.Id == id && w.Borrado == false).FirstOrDefault() ?? throw new AppException("La tarifa no existe");
            UsuarioResponse usuario = new()
            {
                id = tarifa.UsuarioNavigation.Id,
                nombre = tarifa.UsuarioNavigation.Nombre,
                email = tarifa.UsuarioNavigation.Email,
                estado = tarifa.UsuarioNavigation.Estado,
                ausente = tarifa.UsuarioNavigation.Ausente,
            };

            UsuarioTarifaResponse  lUsuarioTarifa = new(){
                Id = tarifa.Id,
                Usuario = tarifa.Usuario,
                UsuarioNavigation = usuario,
                PrecioHora = tarifa.PrecioHora,
                FechaInicia = tarifa.FechaInicia.ToString("yyyy-MM"),
                FechaVence = tarifa.FechaVence.ToString("yyyy-MM"),
            };

            return lUsuarioTarifa;
        }

        /// <summary>
        /// Esta funcion establece el dia de una fecha especificada a 1 o 31 para obtener meses cerrados.
        /// </summary>
        public DateTime ObtenerFormatoFecha(string fecha, bool esDiaInicial = true)
        {
            var FechaSplit = fecha.Split("-");
            int anio = Convert.ToInt32(FechaSplit[0]);
            int mes = Convert.ToInt32(FechaSplit[1]);
            int dia = esDiaInicial ? 1 : DateTime.DaysInMonth(anio, mes);
            DateTime FechaImputacion = new(anio, mes, dia);
            return FechaImputacion;
        }
    }
}
