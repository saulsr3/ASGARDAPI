using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASGARDAPI.Clases
{
    public class DatosDepreciacionAF
    {
        public int idBien { get; set; }
        public DateTime fecha { get; set; }
        public double valorDepreciacion { get; set; }
        public double valorRevalorizacion { get; set; }

        public int vidaUtil { get; set; }
    }
}
