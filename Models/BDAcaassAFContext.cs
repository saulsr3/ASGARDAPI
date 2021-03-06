﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ASGARDAPI.Models
{
    public partial class BDAcaassAFContext : DbContext
    {
        public BDAcaassAFContext()
        {
        }

        public BDAcaassAFContext(DbContextOptions<BDAcaassAFContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ActivoFijo> ActivoFijo { get; set; }
        public virtual DbSet<AreaDeNegocio> AreaDeNegocio { get; set; }
        public virtual DbSet<BienMantenimiento> BienMantenimiento { get; set; }
        public virtual DbSet<Bitacora> Bitacora { get; set; }
        public virtual DbSet<Cargos> Cargos { get; set; }
        public virtual DbSet<Categorias> Categorias { get; set; }
        public virtual DbSet<Clasificacion> Clasificacion { get; set; }
        public virtual DbSet<Cooperativa> Cooperativa { get; set; }
        public virtual DbSet<Donantes> Donantes { get; set; }
        public virtual DbSet<Empleado> Empleado { get; set; }
        public virtual DbSet<FormularioIngreso> FormularioIngreso { get; set; }
        public virtual DbSet<InformeMantenimiento> InformeMantenimiento { get; set; }
        public virtual DbSet<Marcas> Marcas { get; set; }
        public virtual DbSet<Periodo> Periodo { get; set; }
        public virtual DbSet<Proveedor> Proveedor { get; set; }
        public virtual DbSet<SolicitudBaja> SolicitudBaja { get; set; }
        public virtual DbSet<SolicitudMantenimiento> SolicitudMantenimiento { get; set; }
        public virtual DbSet<SolicitudTraspaso> SolicitudTraspaso { get; set; }
        public virtual DbSet<Sucursal> Sucursal { get; set; }
        public virtual DbSet<TarjetaDepreciacion> TarjetaDepreciacion { get; set; }
        public virtual DbSet<Tecnicos> Tecnicos { get; set; }
        public virtual DbSet<TipoDescargo> TipoDescargo { get; set; }
        public virtual DbSet<TipoTraspaso> TipoTraspaso { get; set; }
        public virtual DbSet<TipoUsuario> TipoUsuario { get; set; }
        public virtual DbSet<Token> Token { get; set; }
        public virtual DbSet<Usuario> Usuario { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=DBAcaassAF;database=BDAcaassAF;Integrated Security=true");
               //optionsBuilder.UseSqlServer("Data Source=tcp:server-db-asgard.database.windows.net,1433;Initial Catalog=asgardDB;User Id=adminAsgard@server-db-asgard;Password=Root14003$");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ActivoFijo>(entity =>
            {
                entity.HasKey(e => e.IdBien);

                entity.Property(e => e.Color)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.CorrelativoBien)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Desripcion)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.DestinoInicial)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.EstadoIngreso)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.FechaAsignacion).HasColumnType("date");

                entity.Property(e => e.Foto).IsUnicode(false);

                entity.Property(e => e.Modelo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.NoSerie)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PlazoPago)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdClasificacionNavigation)
                    .WithMany(p => p.ActivoFijo)
                    .HasForeignKey(d => d.IdClasificacion)
                    .HasConstraintName("FK__ActivoFij__IdCla__5812160E");

                entity.HasOne(d => d.IdDonanteNavigation)
                    .WithMany(p => p.ActivoFijo)
                    .HasForeignKey(d => d.IdDonante)
                    .HasConstraintName("FK__ActivoFij__IdDon__5AEE82B9");

                entity.HasOne(d => d.IdMarcaNavigation)
                    .WithMany(p => p.ActivoFijo)
                    .HasForeignKey(d => d.IdMarca)
                    .HasConstraintName("FK__ActivoFij__IdMar__59FA5E80");

                entity.HasOne(d => d.IdProveedorNavigation)
                    .WithMany(p => p.ActivoFijo)
                    .HasForeignKey(d => d.IdProveedor)
                    .HasConstraintName("FK__ActivoFij__IdPro__5BE2A6F2");

                entity.HasOne(d => d.IdResponsableNavigation)
                    .WithMany(p => p.ActivoFijo)
                    .HasForeignKey(d => d.IdResponsable)
                    .HasConstraintName("FK__ActivoFij__IdRes__5CD6CB2B");

                entity.HasOne(d => d.NoFormularioNavigation)
                    .WithMany(p => p.ActivoFijo)
                    .HasForeignKey(d => d.NoFormulario)
                    .HasConstraintName("FK__ActivoFij__NoFor__59063A47");
            });

            modelBuilder.Entity<AreaDeNegocio>(entity =>
            {
                entity.HasKey(e => e.IdAreaNegocio);

                entity.Property(e => e.Correlativo)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Nombre)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdSucursalNavigation)
                    .WithMany(p => p.AreaDeNegocio)
                    .HasForeignKey(d => d.IdSucursal)
                    .HasConstraintName("FK__AreaDeNeg__IdSuc__5535A963");
            });

            modelBuilder.Entity<BienMantenimiento>(entity =>
            {
                entity.HasKey(e => e.IdMantenimiento);

                entity.ToTable("Bien_Mantenimiento");

                entity.Property(e => e.PeriodoMantenimiento)
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.RazonMantenimiento)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdBienNavigation)
                    .WithMany(p => p.BienMantenimiento)
                    .HasForeignKey(d => d.IdBien)
                    .HasConstraintName("FK__Bien_Mant__IdBie__6383C8BA");

                entity.HasOne(d => d.IdSolicitudNavigation)
                    .WithMany(p => p.BienMantenimiento)
                    .HasForeignKey(d => d.IdSolicitud)
                    .HasConstraintName("FK__Bien_Mant__IdSol__628FA481");
            });

            modelBuilder.Entity<Bitacora>(entity =>
            {
                entity.HasKey(e => e.IdBitacora);

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Fecha).HasColumnType("datetime");

                entity.HasOne(d => d.IdUsuarioNavigation)
                    .WithMany(p => p.Bitacora)
                    .HasForeignKey(d => d.IdUsuario)
                    .HasConstraintName("FK__Bitacora__IdUsua__60A75C0F");
            });

            modelBuilder.Entity<Cargos>(entity =>
            {
                entity.HasKey(e => e.IdCargo);

                entity.Property(e => e.Cargo)
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Categorias>(entity =>
            {
                entity.HasKey(e => e.IdCategoria);

                entity.Property(e => e.Categoria)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Clasificacion>(entity =>
            {
                entity.HasKey(e => e.IdClasificacion);

                entity.Property(e => e.Clasificacion1)
                    .HasColumnName("Clasificacion")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Correlativo)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdCategoriaNavigation)
                    .WithMany(p => p.Clasificacion)
                    .HasForeignKey(d => d.IdCategoria)
                    .HasConstraintName("FK__Clasifica__IdCat__5629CD9C");
            });

            modelBuilder.Entity<Cooperativa>(entity =>
            {
                entity.HasKey(e => e.IdCooperativa);

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.Logo).IsUnicode(false);

                entity.Property(e => e.Nombre)
                    .HasMaxLength(75)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Donantes>(entity =>
            {
                entity.HasKey(e => e.IdDonante);

                entity.Property(e => e.Direccion)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Nombre)
                    .HasMaxLength(75)
                    .IsUnicode(false);

                entity.Property(e => e.Telefono)
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Empleado>(entity =>
            {
                entity.HasKey(e => e.IdEmpleado);

                entity.Property(e => e.Apellidos)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.BtieneUsuario).HasColumnName("BTieneUsuario");

                entity.Property(e => e.Direccion)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Dui)
                    .HasColumnName("DUI")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Nombres)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Telefono)
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.TelefonoPersonal)
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdAreaDeNegocioNavigation)
                    .WithMany(p => p.Empleado)
                    .HasForeignKey(d => d.IdAreaDeNegocio)
                    .HasConstraintName("FK__Empleado__IdArea__6754599E");

                entity.HasOne(d => d.IdCargoNavigation)
                    .WithMany(p => p.Empleado)
                    .HasForeignKey(d => d.IdCargo)
                    .HasConstraintName("FK__Empleado__IdCarg__571DF1D5");
            });

            modelBuilder.Entity<FormularioIngreso>(entity =>
            {
                entity.HasKey(e => e.NoFormulario);

                entity.Property(e => e.FechaIngreso).HasColumnType("date");

                entity.Property(e => e.NoFactura)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Observaciones)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.PersonaEntrega)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.PersonaRecibe)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<InformeMantenimiento>(entity =>
            {
                entity.HasKey(e => e.IdInformeMantenimiento);

                entity.Property(e => e.CostoMo).HasColumnName("CostoMO");

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Fecha).HasColumnType("date");

                entity.HasOne(d => d.IdMantenimientoNavigation)
                    .WithMany(p => p.InformeMantenimiento)
                    .HasForeignKey(d => d.IdMantenimiento)
                    .HasConstraintName("FK__InformeMa__IdMan__68487DD7");

                entity.HasOne(d => d.IdTecnicoNavigation)
                    .WithMany(p => p.InformeMantenimiento)
                    .HasForeignKey(d => d.IdTecnico)
                    .HasConstraintName("FK__InformeMa__IdTec__693CA210");
            });

            modelBuilder.Entity<Marcas>(entity =>
            {
                entity.HasKey(e => e.IdMarca);

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Marca)
                    .HasMaxLength(25)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Periodo>(entity =>
            {
                entity.HasKey(e => e.IdPeriodo);

                entity.HasOne(d => d.IdCooperativaNavigation)
                    .WithMany(p => p.Periodo)
                    .HasForeignKey(d => d.IdCooperativa)
                    .HasConstraintName("FK__Periodo__IdCoope__534D60F1");
            });

            modelBuilder.Entity<Proveedor>(entity =>
            {
                entity.HasKey(e => e.IdProveedor);

                entity.Property(e => e.Direccion)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Encargado)
                    .HasMaxLength(75)
                    .IsUnicode(false);

                entity.Property(e => e.Nombre)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Rubro)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Telefono)
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.TelefonoEncargado)
                    .HasMaxLength(25)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<SolicitudBaja>(entity =>
            {
                entity.HasKey(e => e.IdSolicitud);

                entity.Property(e => e.Acuerdo).IsUnicode(false);

                entity.Property(e => e.Contacto)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Domicilio)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.EntidadBeneficiaria)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Fecha).HasColumnType("date");

                entity.Property(e => e.Fechabaja).HasColumnType("date");

                entity.Property(e => e.Folio)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Observaciones)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Telefono)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdBienNavigation)
                    .WithMany(p => p.SolicitudBaja)
                    .HasForeignKey(d => d.IdBien)
                    .HasConstraintName("FK__Solicitud__IdBie__6477ECF3");

                entity.HasOne(d => d.IdTipoDescargoNavigation)
                    .WithMany(p => p.SolicitudBaja)
                    .HasForeignKey(d => d.IdTipoDescargo)
                    .HasConstraintName("FK__Solicitud__IdTip__66603565");
            });

            modelBuilder.Entity<SolicitudMantenimiento>(entity =>
            {
                entity.HasKey(e => e.IdSolicitud);

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Fecha).HasColumnType("date");

                entity.Property(e => e.Folio)
                    .HasMaxLength(10)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<SolicitudTraspaso>(entity =>
            {
                entity.HasKey(e => e.IdSolicitud);

                entity.Property(e => e.Acuerdo).IsUnicode(false);

                entity.Property(e => e.AreadenegocioAnterior)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Fecha).HasColumnType("date");

                entity.Property(e => e.Fechatraspaso).HasColumnType("date");

                entity.Property(e => e.Folio)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.NuevaAreadenegocio)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.NuevoResponsable)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ResponsableAnterior)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdBienNavigation)
                    .WithMany(p => p.SolicitudTraspaso)
                    .HasForeignKey(d => d.IdBien)
                    .HasConstraintName("FK__Solicitud__IdBie__656C112C");
            });

            modelBuilder.Entity<Sucursal>(entity =>
            {
                entity.HasKey(e => e.IdSucursal);

                entity.Property(e => e.Correlativo)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Nombre)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Ubicacion)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdCooperativaNavigation)
                    .WithMany(p => p.Sucursal)
                    .HasForeignKey(d => d.IdCooperativa)
                    .HasConstraintName("FK__Sucursal__IdCoop__5441852A");
            });

            modelBuilder.Entity<TarjetaDepreciacion>(entity =>
            {
                entity.HasKey(e => e.IdTarjeta);

                entity.Property(e => e.Concepto)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Fecha).HasColumnType("date");

                entity.HasOne(d => d.IdBienNavigation)
                    .WithMany(p => p.TarjetaDepreciacion)
                    .HasForeignKey(d => d.IdBien)
                    .HasConstraintName("FK__TarjetaDe__IdBie__5DCAEF64");
            });

            modelBuilder.Entity<Tecnicos>(entity =>
            {
                entity.HasKey(e => e.IdTecnico);

                entity.Property(e => e.Empresa)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Nombre)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TipoDescargo>(entity =>
            {
                entity.HasKey(e => e.IdTipo);

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Nombre)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TipoTraspaso>(entity =>
            {
                entity.HasKey(e => e.IdTipo);

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Nombre)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TipoUsuario>(entity =>
            {
                entity.HasKey(e => e.IdTipoUsuario);

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.TipoUsuario1)
                    .HasColumnName("TipoUsuario")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Token>(entity =>
            {
                entity.HasKey(e => e.IdToken);

                entity.Property(e => e.Codigo).HasColumnName("codigo");

                entity.HasOne(d => d.IdUsuarioNavigation)
                    .WithMany(p => p.Token)
                    .HasForeignKey(d => d.IdUsuario)
                    .HasConstraintName("FK__Token__IdUsuario__619B8048");
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.IdUsuario);

                entity.Property(e => e.Contra)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.NombreUsuario)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdEmpleadoNavigation)
                    .WithMany(p => p.Usuario)
                    .HasForeignKey(d => d.IdEmpleado)
                    .HasConstraintName("FK__Usuario__IdEmple__5EBF139D");

                entity.HasOne(d => d.IdTipoUsuarioNavigation)
                    .WithMany(p => p.Usuario)
                    .HasForeignKey(d => d.IdTipoUsuario)
                    .HasConstraintName("FK__Usuario__IdTipoU__5FB337D6");
            });
        }
    }
}
