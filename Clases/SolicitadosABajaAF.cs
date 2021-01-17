using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASGARDAPI.Clases
{
    public class SolicitadosABajaAF
    {
        public string fechacadena { get; set; }
        public int idbien { get; set; }
        public string acuerdo { get; set; }
        public int NoSolicitud { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        //public string areanegocio { get; set; }
        public string responsable { get; set; }
       
        public string folio { get; set; }
        //public DateTime? fechasolicitud { get; set; }
        public string observaciones { get; set; }
        public int idTipo { get; set; }
        public string nombredescargo { get; set; }
        public int estado { get; set; }
        public string entidadbeneficiaria { get; set; }
       
        //public int idresponsable { get; set; }
        public int idproveedor { get; set; }
        public int iddonante { get; set; }
        public string fechacadena2 { get; set; }
        public string marca { get; set; }
        public string color { get; set; }
        public string clasificacion { get; set; }
        //public string prove { get; set; }
        //public string dona { get; set; }
        public double valor { get; set; }
        public double valoractual { get; set; }
        //public double depreciacion { get; set; }


        //public string domicilio { get; set; }
        //public string contacto { get; set; }
        //public string telefono { get; set; }
    }
}
