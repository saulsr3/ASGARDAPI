using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASGARDAPI.Clases
{
    public class BajaAF
    {
        public int IdBien { get; set; }
        public int NoFormulario { get; set; }
        public string Codigo { get; set; }
        public DateTime? FechaIngreso { get; set; }
        public string fechacadena { get; set; }
        public string Desripcion { get; set; }
        public string Clasificacion { get; set; }
       
        public string AreaDeNegocio { get; set; }
        public string Resposnsable { get; set; }

        public string acuerdo { get; set; }
        public int idsolicitud { get; set; }
        public string fecha2 { get; set; }

    }
}
