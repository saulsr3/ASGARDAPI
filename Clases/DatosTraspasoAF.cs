using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASGARDAPI.Clases
{
    public class DatosTraspasoAF
    {
        public string fechacadena { get; set; }
        public int idbien { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public string areanegocioactual { get; set; }
        public string responsableactual { get; set; }
        public string areanegocioanterior{ get; set; }
        public string responsableanterior { get; set; }

        public int NoSolicitud { get; set; }
        public string folio { get; set; }
        //public DateTime? fechasolicitud { get; set; }
      
        public int estado { get; set; }
        public string acuerdo { get; set; }
        public int idresponsable { get; set; }
        public string fechacadena2 { get; set; }

    }
}
