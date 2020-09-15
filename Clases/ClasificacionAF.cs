using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASGARDAPI.Clases
{
    public class ClasificacionAF
    {
        public int idclasificacion { get; set; }
        public string clasificacion { get; set; }
        public string correlativo { get; set; }
        public string descripcion { get; set; }
        public int dhabilitado { get; set; }
        public int idcategoria { get; set; }
        public string categoria { get; set; }

    }
}
