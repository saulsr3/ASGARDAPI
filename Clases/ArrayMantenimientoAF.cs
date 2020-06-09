using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASGARDAPI.Clases
{
    public class ArrayMantenimientoAF
    {
        public int idMantenimiento { get; set; }
        public int idSolicitud { get; set; }
        public int idBien { get; set; }
        public string razonesMantenimiento { get; set; }
        public string periodoMantenimiento { get; set; }
    }
}
