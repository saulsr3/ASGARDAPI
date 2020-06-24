using System;
using System.Collections.Generic;

namespace ASGARDAPI.Models
{
    public partial class TarjetaDepreciacion
    {
        public int IdTarjeta { get; set; }
        public int? IdBien { get; set; }
        public DateTime? Fecha { get; set; }
        public string Concepto { get; set; }
        public double? Valor { get; set; }
        public double? DepreciacionAnual { get; set; }
        public double? DepreciacionAcumulada { get; set; }
        public double? ValorActual { get; set; }
        public double? ValorMejora { get; set; }

        public ActivoFijo IdBienNavigation { get; set; }
    }
}
