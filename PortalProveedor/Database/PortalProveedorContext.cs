#nullable disable
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using PortalProveedor.Entities;

namespace PortalProveedor.Database
{
    public partial class PortalProveedorContext : DbContext
    {
        public PortalProveedorContext()
        {
        }

        public PortalProveedorContext(DbContextOptions<PortalProveedorContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Accion> Accions { get; set; }

        public virtual DbSet<Albaran> Albarans { get; set; }

        public virtual DbSet<Aprobador> Aprobadors { get; set; }

        public virtual DbSet<Ceco> Cecos { get; set; }

        public virtual DbSet<CecoSociedad> CecoSociedads { get; set; }

        public virtual DbSet<Cliente> Clientes { get; set; }

        public virtual DbSet<EstadoFactura> EstadoFacturas { get; set; }

        public virtual DbSet<EstadoProveedor> EstadoProveedors { get; set; }

        public virtual DbSet<EstadoProyecto> EstadoProyectos { get; set; }

        public virtual DbSet<EstadoSuscripcion> EstadoSuscripcions { get; set; }

        public virtual DbSet<EstadoUsuario> EstadoUsuarios { get; set; }

        public virtual DbSet<Factura> Facturas { get; set; }

        public virtual DbSet<FacturaEstadoFactura> FacturaEstadoFacturas { get; set; }

        public virtual DbSet<FacturaPedido> FacturaPedidos { get; set; }

        public virtual DbSet<Facturacion> Facturacions { get; set; }

        public virtual DbSet<Fichero> Ficheroes { get; set; }

        public virtual DbSet<FlujoAprobacionFactura> FlujoAprobacionFacturas { get; set; }

        public virtual DbSet<FlujoEstadoFactura> FlujoEstadoFacturas { get; set; }

        public virtual DbSet<Log> Logs { get; set; }

        public virtual DbSet<LoginProveedor> LoginProveedors { get; set; }

        public virtual DbSet<LoginProveedorSociedad> LoginProveedorSociedads { get; set; }

        public virtual DbSet<MedioPago> MedioPagos { get; set; }

        public virtual DbSet<Pedido> Pedidos { get; set; }

        public virtual DbSet<PermisoRolAccion> PermisoRolAccions { get; set; }

        public virtual DbSet<Proveedor> Proveedors { get; set; }

        public virtual DbSet<Proyecto> Proyectos { get; set; }

        public virtual DbSet<Rol> Rols { get; set; }

        public virtual DbSet<RolSociedadUsuario> RolSociedadUsuarios { get; set; }

        public virtual DbSet<Sociedad> Sociedads { get; set; }

        public virtual DbSet<Suscripcion> Suscripcions { get; set; }

        public virtual DbSet<TipoIdentificador> TipoIdentificadors { get; set; }

        public virtual DbSet<TipoSuscripcion> TipoSuscripcions { get; set; }

        public virtual DbSet<Usuario> Usuarios { get; set; }

        public virtual DbSet<Imputacion> Imputacion { get; set; }
        public virtual DbSet<ImputacionDetalle> ImputacionDetalle { get; set; }

        public virtual DbSet<UsuarioTarifa> UsuarioTarifa { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            var connectionString = configuration.GetConnectionString("portalProveedorDb");
            optionsBuilder.LogTo(Console.WriteLine)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors()
                .UseSqlServer(connectionString);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Accion>(entity =>
            {
                entity.ToTable("Accion");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Albaran>(entity =>
            {
                entity.ToTable("Albaran", tb => tb.HasComment("Tabla que almacena los datos para localizar un albarán"));

                entity.Property(e => e.FechaAlta).HasColumnType("datetime");

                entity.HasOne(d => d.FacturaNavigation).WithMany(p => p.Albarans)
                    .HasForeignKey(d => d.Factura)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Albaran_Factura");

                entity.HasOne(d => d.FicheroAlbaranNavigation).WithMany(p => p.Albarans)
                    .HasForeignKey(d => d.FicheroAlbaran)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Albaran_Fichero");
            });

            modelBuilder.Entity<Aprobador>(entity =>
            {
                entity.ToTable("Aprobador");

                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.Aprobador1).HasColumnName("Aprobador");

                entity.HasOne(d => d.Aprobador1Navigation).WithMany(p => p.Aprobadors)
                    .HasForeignKey(d => d.Aprobador1)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Aprobador_Usuario");

                entity.HasOne(d => d.ProyectoNavigation).WithMany(p => p.Aprobadors)
                    .HasForeignKey(d => d.Proyecto)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Aprobador_Proyecto");
            });

            modelBuilder.Entity<Ceco>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_CeCo");

                entity.ToTable("Ceco", tb => tb.HasComment("Son los Cecos (centros de coste) definidos por cada uno de los proveedores por cada sociedad"));

                entity.HasIndex(e => new { e.Codigo, e.Sociedad }, "IX_Ceco").IsUnique();

                entity.Property(e => e.Codigo)
                    .HasMaxLength(20)
                    .IsUnicode(false);
                entity.Property(e => e.FechaAlta).HasColumnType("datetime");

                entity.HasOne(d => d.SociedadNavigation).WithMany(p => p.Cecos)
                    .HasForeignKey(d => d.Sociedad)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Ceco_Sociedad");
            });

            modelBuilder.Entity<CecoSociedad>(entity =>
            {
                entity.ToTable("CecoSociedad", tb => tb.HasComment("Tabla para relacionar una sociedad con sus Cecos. "));

                entity.HasOne(d => d.CecoNavigation).WithMany(p => p.CecoSociedads)
                    .HasForeignKey(d => d.Ceco)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CecoSociedad_CeCo");

                entity.HasOne(d => d.SociedadNavigation).WithMany(p => p.CecoSociedads)
                    .HasForeignKey(d => d.Sociedad)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CecoSociedad_Sociedad");
            });

            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.ToTable("Cliente", tb => tb.HasComment("Cliente de la plataforma, suele ser una empresa o profesional. Es a quien le vendemos la plataforma."));

                entity.HasIndex(e => new { e.Identificador, e.TipoIdentificador }, "IX_Cliente").IsUnique();

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.FechaAlta).HasColumnType("datetime");
                entity.Property(e => e.Identificador)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.Nombre)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.TipoIdentificador)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<EstadoFactura>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_Estado");

                entity.ToTable("EstadoFactura", tb => tb.HasComment("Tabla auxiliar para los posibles estados de una factura, en principio se establecen los siguientes: Pendiente de cumplimentar, pendiente de aprobar, aprobada, pagada, retenida y rechazada"));

                entity.Property(e => e.EstadoInterno)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.EstadoProveedor)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<EstadoProveedor>(entity =>
            {
                entity.ToTable("EstadoProveedor");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<EstadoProyecto>(entity =>
            {
                entity.ToTable("EstadoProyecto");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<EstadoSuscripcion>(entity =>
            {
                entity.ToTable("EstadoSuscripcion", "Suscripcion");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<EstadoUsuario>(entity =>
            {
                entity.ToTable("EstadoUsuario", tb => tb.HasComment("Tabla auxiliar para los posibles estados de un usuario, en principio se establecen los siguientes: Activo e inactivo"));

                entity.Property(e => e.Nombre)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Factura>(entity =>
            {
                entity.ToTable("Factura", tb => tb.HasComment("Tabla de facturas"));

                entity.HasIndex(e => new { e.Numero, e.FechaFactura, e.Proveedor }, "IX_Factura").IsUnique();

                entity.Property(e => e.FechaAlta).HasColumnType("datetime");
                entity.Property(e => e.FechaFactura).HasColumnType("smalldatetime");
                entity.Property(e => e.FechaVencimiento).HasColumnType("date");
                entity.Property(e => e.Importe).HasColumnType("money");
                entity.Property(e => e.Numero)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.FicheroFacturaNavigation).WithMany(p => p.FacturaFicheroFacturaNavigations)
                    .HasForeignKey(d => d.FicheroFactura)
                    .HasConstraintName("FK_Factura_Fichero");

                entity.HasOne(d => d.FicheroFacturaProformaNavigation).WithMany(p => p.FacturaFicheroFacturaProformaNavigations)
                    .HasForeignKey(d => d.FicheroFacturaProforma)
                    .HasConstraintName("FK_Factura_Fichero2");

                entity.HasOne(d => d.ProveedorNavigation).WithMany(p => p.Facturas)
                    .HasForeignKey(d => d.Proveedor)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Factura_Proveedor");

                entity.HasOne(d => d.ProyectoNavigation).WithMany(p => p.Facturas)
                    .HasForeignKey(d => d.Proyecto)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Factura_Proyecto");

                entity.HasOne(d => d.ResponsableAprobarNavigation).WithMany(p => p.Facturas)
                    .HasForeignKey(d => d.ResponsableAprobar)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Factura_Usuario");

                entity.HasOne(d => d.SociedadNavigation).WithMany(p => p.Facturas)
                    .HasForeignKey(d => d.Sociedad)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Factura_Sociedad");
            });

            modelBuilder.Entity<FacturaEstadoFactura>(entity =>
            {
                entity.ToTable("FacturaEstadoFactura");

                entity.HasIndex(e => new { e.Factura, e.EstadoFactura, e.FechaAlta }, "IX_FacturaEstadoFactura").IsUnique();

                entity.Property(e => e.Comentario)
                    .HasMaxLength(512)
                    .IsUnicode(false);
                entity.Property(e => e.FechaAlta).HasColumnType("datetime");

                entity.HasOne(d => d.EstadoFacturaNavigation).WithMany(p => p.FacturaEstadoFacturas)
                    .HasForeignKey(d => d.EstadoFactura)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FacturaEstadoFactura_EstadoFactura");

                entity.HasOne(d => d.FacturaNavigation).WithMany(p => p.FacturaEstadoFacturas)
                    .HasForeignKey(d => d.Factura)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FacturaEstadoFactura_Factura");
            });

            modelBuilder.Entity<FacturaPedido>(entity =>
            {
                entity.ToTable("FacturaPedido");

                entity.Property(e => e.FechaAlta).HasColumnType("datetime");

                entity.HasOne(d => d.FacturaNavigation).WithMany(p => p.FacturaPedidos)
                    .HasForeignKey(d => d.Factura)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FacturaPedido_Factura");

                entity.HasOne(d => d.PedidoNavigation).WithMany(p => p.FacturaPedidos)
                    .HasForeignKey(d => d.Pedido)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FacturaPedido_Pedido");
            });

            modelBuilder.Entity<Facturacion>(entity =>
            {
                entity.ToTable("Facturacion", "Suscripcion");

                entity.HasIndex(e => e.Cliente, "IX_Facturacion").IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.CodigoPostal)
                    .HasMaxLength(5)
                    .IsUnicode(false);
                entity.Property(e => e.Dirección)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.Pais)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.Poblacion)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Fichero>(entity =>
            {
                entity.ToTable("Fichero", tb => tb.HasComment("Tabla que almacena los datos para localizar un fichero"));

                entity.Property(e => e.FechaAlta).HasColumnType("datetime");
                entity.Property(e => e.Nombre)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.Ruta)
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<FlujoAprobacionFactura>(entity =>
            {
                entity.ToTable("FlujoAprobacionFactura");

                entity.HasIndex(e => new { e.Sociedad, e.Predeterminado, e.Proforma, e.Proyecto }, "IX_FlujoAprobacionFactura").IsUnique();

                entity.Property(e => e.Nombre)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.ProyectoNavigation).WithMany(p => p.FlujoAprobacionFacturas)
                    .HasForeignKey(d => d.Proyecto)
                    .HasConstraintName("FK_FlujoAprobacionFactura_Proyecto");

                entity.HasOne(d => d.SociedadNavigation).WithMany(p => p.FlujoAprobacionFacturas)
                    .HasForeignKey(d => d.Sociedad)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FlujoAprobacionFactura_Sociedad");
            });

            modelBuilder.Entity<FlujoEstadoFactura>(entity =>
            {
                entity.ToTable("FlujoEstadoFactura");

                entity.HasIndex(e => new { e.EstadoOrigen, e.EstadoDestino, e.FlujoAprobacionFactura }, "IX_FlujoEstadoFactura").IsUnique();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.NombreEstadoExterno)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.NombreEstadoInterno)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.AprobadorNavigation).WithMany(p => p.FlujoEstadoFacturaAprobadorNavigations)
                    .HasForeignKey(d => d.Aprobador)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FlujoEstadoFactura_Usuario");

                entity.HasOne(d => d.AprobadorDelegadoNavigation).WithMany(p => p.FlujoEstadoFacturaAprobadorDelegadoNavigations)
                    .HasForeignKey(d => d.AprobadorDelegado)
                    .HasConstraintName("FK_FlujoEstadoFactura_Usuario2");

                entity.HasOne(d => d.AprobadorSecundarioNavigation).WithMany(p => p.FlujoEstadoFacturaAprobadorSecundarioNavigations)
                    .HasForeignKey(d => d.AprobadorSecundario)
                    .HasConstraintName("FK_FlujoEstadoFactura_Usuario1");

                entity.HasOne(d => d.EstadoDestinoNavigation).WithMany(p => p.FlujoEstadoFacturaEstadoDestinoNavigations)
                    .HasForeignKey(d => d.EstadoDestino)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FlujoEstadoFactura_EstadoFactura1");

                entity.HasOne(d => d.EstadoOrigenNavigation).WithMany(p => p.FlujoEstadoFacturaEstadoOrigenNavigations)
                    .HasForeignKey(d => d.EstadoOrigen)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FlujoEstadoFactura_EstadoFactura");

                entity.HasOne(d => d.FlujoAprobacionFacturaNavigation).WithMany(p => p.FlujoEstadoFacturas)
                    .HasForeignKey(d => d.FlujoAprobacionFactura)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FlujoEstadoFactura_FlujoAprobacionFactura");
            });

            modelBuilder.Entity<Log>(entity =>
            {
                entity.ToTable("Log");

                entity.Property(e => e.TimeStamp).HasColumnType("datetime");
            });

            modelBuilder.Entity<LoginProveedor>(entity =>
            {
                entity.ToTable("LoginProveedor");

                entity.Property(e => e.Clave)
                    .HasMaxLength(60)
                    .IsUnicode(false);
                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.FechaAlta).HasColumnType("datetime");

                entity.HasOne(d => d.ProveedorNavigation).WithMany(p => p.LoginProveedors)
                    .HasForeignKey(d => d.Proveedor)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LoginProveedor_Proveedor");
            });

            modelBuilder.Entity<LoginProveedorSociedad>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_ProveedorSociedad");

                entity.ToTable("LoginProveedorSociedad", tb => tb.HasComment("Tabla para relacionar los proveedores con sociedades"));

                entity.HasOne(d => d.LoginProveedorNavigation).WithMany(p => p.LoginProveedorSociedads)
                    .HasForeignKey(d => d.LoginProveedor)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LoginProveedorSociedad_LoginProveedor1");

                entity.HasOne(d => d.ProveedorNavigation).WithMany(p => p.LoginProveedorSociedads)
                    .HasForeignKey(d => d.Proveedor)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProveedorSociedad_Proveedor");

                entity.HasOne(d => d.SociedadNavigation).WithMany(p => p.LoginProveedorSociedads)
                    .HasForeignKey(d => d.Sociedad)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProveedorSociedad_Sociedad");
            });

            modelBuilder.Entity<MedioPago>(entity =>
            {
                entity.ToTable("MedioPago", "Suscripcion");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Pedido>(entity =>
            {
                entity.ToTable("Pedido");

                entity.HasIndex(e => new { e.NumeroPedido, e.Proveedor }, "IX_Pedido").IsUnique();

                entity.Property(e => e.FechaAlta).HasColumnType("datetime");

                entity.HasOne(d => d.FicheroPedidoNavigation).WithMany(p => p.Pedidos)
                    .HasForeignKey(d => d.FicheroPedido)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Pedido_Fichero1");
            });

            modelBuilder.Entity<PermisoRolAccion>(entity =>
            {
                entity.ToTable("PermisoRolAccion");

                entity.HasIndex(e => new { e.Accion, e.Rol }, "IX_PermisoRolAccion").IsUnique();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.HasOne(d => d.AccionNavigation).WithMany(p => p.PermisoRolAccions)
                    .HasForeignKey(d => d.Accion)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PermisoRolAccion_Accion");

                entity.HasOne(d => d.RolNavigation).WithMany(p => p.PermisoRolAccions)
                    .HasForeignKey(d => d.Rol)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PermisoRolAccion_Rol");
            });

            modelBuilder.Entity<Proveedor>(entity =>
            {
                entity.ToTable("Proveedor", tb => tb.HasComment("Son los proveedores de una sociedad."));

                entity.HasIndex(e => new { e.Identificador, e.TipoIdentificador }, "IX_Proveedor").IsUnique();

                entity.Property(e => e.FechaAlta).HasColumnType("datetime");
                entity.Property(e => e.Identificador)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.Nombre)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.EstadoProveedorNavigation).WithMany(p => p.Proveedors)
                    .HasForeignKey(d => d.EstadoProveedor)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Proveedor_EstadoProveedor");

                entity.HasOne(d => d.TipoIdentificadorNavigation).WithMany(p => p.Proveedors)
                    .HasForeignKey(d => d.TipoIdentificador)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Proveedor_TipoIdentificador");
            });

            modelBuilder.Entity<Proyecto>(entity =>
            {
                entity.ToTable("Proyecto", tb => tb.HasComment("Proyectos definidos por sociedad"));

                entity.HasIndex(e => new { e.Codigo, e.Sociedad }, "IX_Proyecto").IsUnique();

                entity.Property(e => e.Codigo)
                    .HasMaxLength(20)
                    .IsUnicode(false);
                entity.Property(e => e.FechaAlta).HasColumnType("datetime");
                entity.Property(e => e.Nombre)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.EstadoProyectoNavigation).WithMany(p => p.Proyectos)
                    .HasForeignKey(d => d.EstadoProyecto)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Proyecto_EstadoProyecto");

                entity.HasOne(d => d.FlujoAprobacionFacturaNavigation).WithMany(p => p.Proyectos)
                    .HasForeignKey(d => d.FlujoAprobacionFactura)
                    .HasConstraintName("FK_Proyecto_FlujoAprobacionFactura");

                entity.HasOne(d => d.SociedadNavigation).WithMany(p => p.Proyectos)
                    .HasForeignKey(d => d.Sociedad)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Proyecto_Sociedad");

                entity.HasOne(d => d.UsuarioNavigation).WithMany(p => p.Proyectos)
                    .HasForeignKey(d => d.Responsable)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RolSociedadUsuario_Usuario");
            });

            modelBuilder.Entity<Rol>(entity =>
            {
                entity.ToTable("Rol", tb => tb.HasComment("Roles disponibles de usuario por cada sociedad que tenga un cliente, en principio se establecen dos: Administrdor y usuario interno. "));

                entity.Property(e => e.Nombre)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<RolSociedadUsuario>(entity =>
            {
                entity.ToTable("RolSociedadUsuario", tb => tb.HasComment("Tabla que establece el rol de un usuario en una sociedad de un cliente."));

                entity.HasIndex(e => new { e.Sociedad, e.Usuario }, "IX_RolSociedadUsuario").IsUnique();

                entity.HasOne(d => d.RolNavigation).WithMany(p => p.RolSociedadUsuarios)
                    .HasForeignKey(d => d.Rol)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RolSociedadUsuario_RolSociedad");

                entity.HasOne(d => d.SociedadNavigation).WithMany(p => p.RolSociedadUsuarios)
                    .HasForeignKey(d => d.Sociedad)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RolSociedadUsuario_Sociedad");

                entity.HasOne(d => d.UsuarioNavigation).WithMany(p => p.RolSociedadUsuarios)
                    .HasForeignKey(d => d.Usuario)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RolSociedadUsuario_Usuario");
            });

            modelBuilder.Entity<Imputacion>(entity =>
            {
                entity.ToTable("Imputacion", "ParteHoras", tb => tb.HasComment("Tabla que contiene los datos generales de imputacion de horas en proyectos por Mes"));
                entity.Property( e => e.Mes ).HasMaxLength(10).IsUnicode(false);
                entity.Property( e => e.TotalCosto).HasColumnType("money");
                entity.Property( e => e.ImputacionHoras).HasMaxLength(int.MaxValue).IsUnicode(false);
                entity.Property( e => e.FechaAlta).HasColumnType("datetime");
                entity.HasOne(d => d.UsuarioNavigation).WithMany(p => p.Imputaciones).HasForeignKey(d => d.Usuario)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Imputacion_Usuario");
            });

            modelBuilder.Entity<ImputacionDetalle>(entity =>
            {
                entity.ToTable("ImputacionDetalle", "ParteHoras", tb => tb.HasComment("Tabla que contiene los datos detalles de imputacion de horas en proyectos por Mes"));
                entity.Property(e => e.TotalCosto).HasColumnType("money");
                entity.HasOne(d => d.ImputacionNavigation).WithMany(p => p.ImputacionDetalle)
                    .HasForeignKey(d => d.Imputacion)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ImputacionDetalle_Imputacion");

                entity.HasOne(d => d.ProyectoNavigation).WithMany(p => p.ImputacionDetalle)
                    .HasForeignKey(d => d.Proyecto)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ImputacionDetalle_Proyecto");

            });

            modelBuilder.Entity<UsuarioTarifa>(entity => {
                entity.ToTable("UsuarioTarifa", "ParteHoras", tb => tb.HasComment("Table que contiene los datos de tarifas de los usuarios"));
                entity.Property(e => e.PrecioHora).HasColumnType("money");
                entity.Property(e => e.FechaInicia).HasColumnType("datetime");
                entity.Property(e => e.FechaVence).HasColumnType("datetime");
                entity.Property(e => e.FechaAlta).HasColumnType("datetime");
                entity.HasOne(d => d.UsuarioNavigation).WithMany(p => p.Tarifas)
                    .HasForeignKey(d => d.Usuario)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UsuarioTarifa_Usuario");
            });

            modelBuilder.Entity<Sociedad>(entity =>
            {
                entity.ToTable("Sociedad", tb => tb.HasComment("Son las sociedades que un cliente tiene dadas de alta, es decir, un cliente puede tener distintas empresas que gestione el portal."));

                entity.HasIndex(e => new { e.Identificador, e.TipoIdentificador }, "IX_Sociedad").IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.EmailNotifcaciones)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.EmailProcesoFacturas)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.FechaAlta).HasColumnType("datetime");
                entity.Property(e => e.Identificador)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.Nombre)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.TipoIdentificador).ValueGeneratedOnAdd();

                entity.HasOne(d => d.ClienteNavigation).WithMany(p => p.Sociedads)
                    .HasForeignKey(d => d.Cliente)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Sociedad_Cliente");

                entity.HasOne(d => d.FlujoAprobacionFacturaNavigation).WithMany(p => p.SociedadFlujoAprobacionFacturaNavigations)
                    .HasForeignKey(d => d.FlujoAprobacionFactura)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Sociedad_FlujoAprobacionFactura");

                entity.HasOne(d => d.FlujoAprobacionFacturaProformaNavigation).WithMany(p => p.SociedadFlujoAprobacionFacturaProformaNavigations)
                    .HasForeignKey(d => d.FlujoAprobacionFacturaProforma)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Sociedad_FlujoAprobacionFactura1");
            });

            modelBuilder.Entity<Suscripcion>(entity =>
            {
                entity.ToTable("Suscripcion", "Suscripcion");

                entity.Property(e => e.ComienzoPeriodoActual).HasColumnType("datetime");
                entity.Property(e => e.FechaCancelacion).HasColumnType("datetime");
                entity.Property(e => e.FechaFin).HasColumnType("datetime");
                entity.Property(e => e.FechaPrevistaCancelacion).HasColumnType("datetime");
                entity.Property(e => e.FinPeriodoActual).HasColumnType("datetime");

                entity.HasOne(d => d.EstadoSuscripcionNavigation).WithMany(p => p.Suscripcions)
                    .HasForeignKey(d => d.EstadoSuscripcion)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Suscripcion_EstadoSuscripcion");

                entity.HasOne(d => d.MedioPagoNavigation).WithMany(p => p.Suscripcions)
                    .HasForeignKey(d => d.MedioPago)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Suscripcion_MedioPago");

                entity.HasOne(d => d.TipoSuscripcionNavigation).WithMany(p => p.Suscripcions)
                    .HasForeignKey(d => d.TipoSuscripcion)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Suscripcion_TipoSuscripcion1");
            });

            modelBuilder.Entity<TipoIdentificador>(entity =>
            {
                entity.ToTable("TipoIdentificador");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TipoSuscripcion>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_TipoSuscripcion_1");

                entity.ToTable("TipoSuscripcion", "Suscripcion");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("Usuario", tb => tb.HasComment("Usuario interno que tiene un cliente y que tiene acceso a la plataforma para realizar las tareas que tenga encomendadas\r\n\r\nUn cliente normalmente tendrá varios usuarios en la plataforma con distintos roles."));

                entity.Property(e => e.Clave)
                    .HasMaxLength(60)
                    .IsUnicode(false);
                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.FechaAlta).HasColumnType("datetime");
                entity.Property(e => e.Nombre)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.ClienteNavigation).WithMany(p => p.Usuarios)
                    .HasForeignKey(d => d.Cliente)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Usuario_Cliente");

                entity.HasOne(d => d.EstadoNavigation).WithMany(p => p.Usuarios)
                    .HasForeignKey(d => d.Estado)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Usuario_EstadoUsuario");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}