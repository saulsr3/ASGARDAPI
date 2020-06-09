using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASGARDAPI.Controllers
{
    public class SolicitudMantenimientoPAF
    {
        public int idsolicitud { get; set; }
        public string descripcion { get; set; }
        public string areadenegocio { get; set; }
        public string solicitante { get; set; }
        public DateTime? fechasolicitud { get; set; }
        public string fechacadena { get; set; }
    }
}
