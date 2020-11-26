using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASGARDAPI.Clases
{
    public class SolicitudTraspasoAF
    {
        public int idsolicitud { get; set; }
        public int idbien { get; set; }
        public string folio { get; set; }
        public string codigo { get; set; }
        public DateTime? fechasolicitud { get; set; }
        public DateTime? fechatraspaso { get; set; }
        public string descripcion { get; set; }
        public string acuerdo { get; set; }
        public string nuevoresponsable { get; set; }
        public string nuevaarea { get; set; }
        public string responsableanterior { get; set; }
        public string areaanterior { get; set; }

        public int idresponsable { get; set;}
        
        public int estadosolicitud { get; set; }
        public string fechacadena { get; set; }
        public string fechacadenatraspaso { get; set; }     
        //para listar el area actual y posible antigua.
        
        public string sucursal { get; set; }


    }
}
