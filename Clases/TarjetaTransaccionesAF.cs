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
        public double montoTransaccion { get; set; }
        public double depreciacionAnual { get; set; }
        public double depreciacionAcumulada { get; set; }
        public double valorActual { get; set; }
        public double valorTransaccion { get; set; }
    }
}
