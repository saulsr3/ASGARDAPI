using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASGARDAPI.Clases
{
    public class TarjetaTransaccionesAF
    {
        public int id { get; set; }
        public int idBien { get; set; }
        public string fecha { get; set; }
        public string concepto { get; set; }
        public string montoTransaccion { get; set; }
        public string depreciacionAnual { get; set; }
        public string depreciacionAcumulada { get; set; }
        public string valorActual { get; set; }
        public string valorMejora { get; set; }
    }
}
