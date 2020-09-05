using System;
using System.Collections.Generic;

namespace ASGARDAPI.Models
{
    public partial class InformeMantenimiento
    {
        public int IdInformeMantenimiento { get; set; }
        public int? IdMantenimiento { get; set; }
        public int? IdTecnico { get; set; }
        public DateTime? Fecha { get; set; }
        public string Descripcion { get; set; }
        public double? CostoMateriales { get; set; }
        public double? CostoMo { get; set; }
        public double? CostoTotal { get; set; }
        public int? Estado { get; set; }

        public BienMantenimiento IdMantenimientoNavigation { get; set; }
        public Tecnicos IdTecnicoNavigation { get; set; }
    }
}
