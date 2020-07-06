using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASGARDAPI.Clases
{
    public class BienesSolicitadosMttoAF
    {
        public int idBien { get; set; }
        public int idmantenimiento { get; set; }
        public int estadoActual { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public string Razon { get; set; }
        public string  Periodo { get; set; }
        public string NoSolicitud { get; set; }
        public string fechacadena { get; set; }
        public string areanegocio { get; set; }
        public string jefe { get; set; }
    }
}
