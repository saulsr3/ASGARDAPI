using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASGARDAPI.Clases
{
    public class TarjetaTransacciones
    {
        public string fecha { get; set; }
        public string concepto { get; set; }
        public float montoTransaccion { get; set; }
        public float depreciacionAnual { get; set; }
        public float depreciacionAcumulada { get; set; }
        public float valorActual { get; set; }
        public int MyProperty { get; set; }
    }
}
