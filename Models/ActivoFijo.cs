using System;
using System.Collections.Generic;

namespace ASGARDAPI.Models
{
    public partial class ActivoFijo
    {
        public ActivoFijo()
        {
            BienMantenimiento = new HashSet<BienMantenimiento>();
        }

        public int IdBien { get; set; }
        public string CorrelativoBien { get; set; }
        public int? NoFormulario { get; set; }
        public string Desripcion { get; set; }
        public int? TipoAdquicicion { get; set; }
        public string EstadoIngreso { get; set; }
        public double? ValorAdquicicion { get; set; }
        public string PlazoPago { get; set; }
        public double? Prima { get; set; }
        public double? CuotaAsignanda { get; set; }
        public double? Intereses { get; set; }
        public double? ValorResidual { get; set; }
        public int? IdClasificacion { get; set; }
        public int? IdProveedor { get; set; }
        public int? IdDonante { get; set; }
        public int? IdMarca { get; set; }
        public string Modelo { get; set; }
        public string Color { get; set; }
        public string NoSerie { get; set; }
        public int? VidaUtil { get; set; }
        public int? IdResponsable { get; set; }
        public byte[] Foto { get; set; }
        public int? EstaAsignado { get; set; }
        public string DestinoInicial { get; set; }
        public int? EstadoActual { get; set; }

        public Clasificacion IdClasificacionNavigation { get; set; }
        public Donantes IdDonanteNavigation { get; set; }
        public Marcas IdMarcaNavigation { get; set; }
        public Proveedor IdProveedorNavigation { get; set; }
        public Empleado IdResponsableNavigation { get; set; }
        public FormularioIngreso NoFormularioNavigation { get; set; }
        public ICollection<BienMantenimiento> BienMantenimiento { get; set; }
    }
}
