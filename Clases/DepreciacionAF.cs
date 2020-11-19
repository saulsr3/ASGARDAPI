using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASGARDAPI.Clases
{
    public class DepreciacionAF
    {
        public int idBien { get; set; }
        public string codigo { get; set; }
        public string descripcion { get; set; }
        public string sucursal { get; set; }
        public string areanegocio { get; set; }
        public string responsable { get; set; }
        public string clasificacion { get; set; }

        public string fechacadena { get; set; }
        //para cargar la vida util en revalorización.
         public int? vidautil { get; set; }
    }
}
