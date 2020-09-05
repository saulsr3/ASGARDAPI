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
        public virtual DbSet<Sucursal> Sucursal { get; set; }
        public virtual DbSet<TarjetaDepreciacion> TarjetaDepreciacion { get; set; }
        public virtual DbSet<Tecnicos> Tecnicos { get; set; }
        public virtual DbSet<TipoUsuario> TipoUsuario { get; set; }
        public virtual DbSet<Usuario> Usuario { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=DBAcaassAF;database=BDAcaassAF;Integrated Security=true");
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
                    .HasConstraintName("FK__ActivoFij__IdCla__4D94879B");

                entity.HasOne(d => d.IdDonanteNavigation)
                    .WithMany(p => p.ActivoFijo)
                    .HasForeignKey(d => d.IdDonante)
                    .HasConstraintName("FK__ActivoFij__IdDon__5070F446");

                entity.HasOne(d => d.IdMarcaNavigation)
                    .WithMany(p => p.ActivoFijo)
                    .HasForeignKey(d => d.IdMarca)
                    .HasConstraintName("FK__ActivoFij__IdMar__4F7CD00D");

                entity.HasOne(d => d.IdProveedorNavigation)
                    .WithMany(p => p.ActivoFijo)
                    .HasForeignKey(d => d.IdProveedor)
                    .HasConstraintName("FK__ActivoFij__IdPro__5165187F");

                entity.HasOne(d => d.IdResponsableNavigation)
                    .WithMany(p => p.ActivoFijo)
                    .HasForeignKey(d => d.IdResponsable)
                    .HasConstraintName("FK__ActivoFij__IdRes__52593CB8");

                entity.HasOne(d => d.NoFormularioNavigation)
                    .WithMany(p => p.ActivoFijo)
                    .HasForeignKey(d => d.NoFormulario)
                    .HasConstraintName("FK__ActivoFij__NoFor__4E88ABD4");
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
                    .HasConstraintName("FK__AreaDeNeg__IdSuc__4BAC3F29");
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
                    .HasConstraintName("FK__Bien_Mant__IdBie__5812160E");

                entity.HasOne(d => d.IdSolicitudNavigation)
                    .WithMany(p => p.BienMantenimiento)
                    .HasForeignKey(d => d.IdSolicitud)
                    .HasConstraintName("FK__Bien_Mant__IdSol__571DF1D5");
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
                    .HasConstraintName("FK__Bitacora__IdUsua__5629CD9C");
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
                    .HasConstraintName("FK__Empleado__IdArea__59FA5E80");

                entity.HasOne(d => d.IdCargoNavigation)
                    .WithMany(p => p.Empleado)
                    .HasForeignKey(d => d.IdCargo)
                    .HasConstraintName("FK__Empleado__IdCarg__4CA06362");
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
                    .HasConstraintName("FK__InformeMa__IdMan__5AEE82B9");

                entity.HasOne(d => d.IdTecnicoNavigation)
                    .WithMany(p => p.InformeMantenimiento)
                    .HasForeignKey(d => d.IdTecnico)
                    .HasConstraintName("FK__InformeMa__IdTec__5BE2A6F2");
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
                    .HasConstraintName("FK__Periodo__IdCoope__49C3F6B7");
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

                entity.Property(e => e.Acuerdo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

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
                    .HasConstraintName("FK__Solicitud__IdBie__59063A47");
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
                    .HasConstraintName("FK__Sucursal__IdCoop__4AB81AF0");
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
                    .HasConstraintName("FK__TarjetaDe__IdBie__534D60F1");
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
                    .HasConstraintName("FK__Usuario__IdEmple__5441852A");

                entity.HasOne(d => d.IdTipoUsuarioNavigation)
                    .WithMany(p => p.Usuario)
                    .HasForeignKey(d => d.IdTipoUsuario)
                    .HasConstraintName("FK__Usuario__IdTipoU__5535A963");
            });
        }
    }
}
