using System;
namespace ASGARDAPI.Clases
{
    public class TarjetaExcelAF
    {
        //Variables
        public string codigo { get; set; }
        public string fecha { get; set; }
        public string concepto { get; set; }
        public double montoTransaccion { get; set; }
        public double depreciacionAnual { get; set; }
        public double depreciacionAcumulada { get; set; }
        public double valorActual { get; set; }
        public double valorTransaccion { get; set; }
        public string descripcion { get; set; }
    }
}
