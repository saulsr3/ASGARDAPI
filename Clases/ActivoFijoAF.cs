using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASGARDAPI.Clases
{
    public class ActivoFijoAF
    {
        public int IdBien { get; set; }
        public int NoFormulario { get; set; }
        public DateTime? FechaIngreso { get; set; }
        public string Desripcion { get; set; }
        public string Clasificacion { get; set; }
        public string Marca { get; set; }
    }
}
