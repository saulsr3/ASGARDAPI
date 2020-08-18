using System;
using System.Collections.Generic;

namespace ASGARDAPI.Models
{
    public partial class Periodo
    {
        public int IdPeriodo { get; set; }
        public int? Anio { get; set; }
        public int? IdCooperativa { get; set; }
        public int? Estado { get; set; }

        public Cooperativa IdCooperativaNavigation { get; set; }
    }
}
