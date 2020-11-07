using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASGARDAPI.Clases
{
    public class BienesDepreciacionAF
    {
        public int idBien { get; set; }
        public string cooperativa { get; set; }
        public string anio { get; set; }
        public string fechaAdquisicion { get; set; }
        public string fechaAsignacion { get; set; }
        public string fecha { get; set; }
        public string codigo { get; set; }
        public string descipcion { get; set; }
        public string valorAdquicicon { get; set; }
        public double valorDepreciacion { get; set; }
        public double valorResidual { get; set; }
        public float mejoras { get; set; }
        public float valorActual { get; set; }
        public int vidaUtil { get; set; }
        public string concepto { get; set; }
        public int tipo { get; set; }
    }
}
