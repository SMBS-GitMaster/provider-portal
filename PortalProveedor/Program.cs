using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PortalProveedor.Authorization;
using PortalProveedor.Database;
using PortalProveedor.Helpers;
using PortalProveedor.Services;
using Serilog;
using System.Reflection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var MyCors = "_MyCors";
var services = builder.Services;
var env = builder.Environment;
var connectionString = builder.Configuration.GetConnectionString("portalProveedorDb");
var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

services.AddControllers();

services.AddCors(options =>
{
    options.AddPolicy(name: MyCors,
        policy =>
        {
            policy.WithOrigins("http://localhost", "");
            policy.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost").AllowAnyHeader().AllowAnyMethod();
        });
});

services.AddAutoMapper(typeof(Program));

services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

services.AddDbContext<PortalProveedorContext>(x => x.UseSqlServer(connectionString));

services.AddEndpointsApiExplorer();

services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API Portal Proveedor",
        Version = "v1",
        Contact = new OpenApiContact
        {
            Name = "Portal del proveedor",
            Url = new Uri("https://localhost"),
            Email = ""
        },
        License = new OpenApiLicense {Name = "© Copyright Tadatic. All Rights Reserved"},
        Description = ""
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. <br><br> 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      <br><br>Example: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,              
            },
            new List<string>()
        }        
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
services.AddControllers().ConfigureApiBehaviorOptions(options => {options.SuppressModelStateInvalidFilter = true;});

// configure DI for application services
services.AddScoped<IJwtUtils, JwtUtils>();
services.AddScoped<IAccountService, AccountService>();
services.AddScoped<IFacturaService, FacturaService>();
services.AddScoped<IProyectoService, ProyectoService>();
services.AddScoped<ISociedadService, SociedadService>();
services.AddScoped<IProveedorService, ProveedorService>();
services.AddScoped<IFicheroService, FicheroService>();
services.AddScoped<IUsuarioService, UsuarioService>();
services.AddScoped<ILoginProveedorService, LoginProveedorService>();
services.AddScoped<IFlujoAprobacionFacturaService, FlujoAprobacionFacturaService>();
services.AddScoped<IPedidoService, PedidoService>();
services.AddScoped<IUsuarioTarifaService, UsuarioTarifaService>();
services.AddScoped<IImputacionService, ImputacionService>();


var app = builder.Build();

app.UseCors(MyCors);

// Configure the HTTP request pipeline.
app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Portal Proveedor v1");
    c.RoutePrefix = "docs";
});

app.UseHttpsRedirection();

// global error handler
app.UseMiddleware<ErrorHandlerMiddleware>();

// custom jwt auth middleware
app.UseMiddleware<JwtMiddleware>();

app.MapControllers();

app.Run();
