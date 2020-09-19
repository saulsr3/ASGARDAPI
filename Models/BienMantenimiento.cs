using System;
using System.Collections.Generic;

namespace ASGARDAPI.Models
{
    public partial class BienMantenimiento
    {
        public BienMantenimiento()
        {
            InformeMantenimiento = new HashSet<InformeMantenimiento>();
        }

        public int IdMantenimiento { get; set; }
        public int? IdSolicitud { get; set; }
        public int? IdBien { get; set; }
        public string RazonMantenimiento { get; set; }
        public string PeriodoMantenimiento { get; set; }
        public int? Estado { get; set; }

        public ActivoFijo IdBienNavigation { get; set; }
        public SolicitudMantenimiento IdSolicitudNavigation { get; set; }
        public ICollection<InformeMantenimiento> InformeMantenimiento { get; set; }
    }
}
