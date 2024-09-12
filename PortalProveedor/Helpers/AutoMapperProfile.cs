namespace PortalProveedor.Helpers;

using AutoMapper;
using PortalProveedor.Entities;
using PortalProveedor.Models.Facturas;
using PortalProveedor.Models.Usuarios;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Usuario, AuthenticateResponse>();

        CreateMap<LoginProveedor, AuthenticateResponse>();

        CreateMap<RegisterRequest, Usuario>()
            .ForMember(m => m.Nombre, opt => opt.MapFrom(s => s.NombreCompleto))
            .ForMember(m => m.Cliente, opt => opt.MapFrom(s => s.Empresa))
            .ForMember(m => m.Clave, opt => opt.MapFrom(s => s.Password));

        CreateMap<Factura, ListaFacturaResponse>();
        CreateMap<ListaFacturaResponse, Factura>();
    }
}