using iTextSharp.text;
using System;
namespace ASGARDAPI.Clases
{
    public class ActivoRevalorizadoAF
    {
        //Variables
        public int idBien { get; set; }
        public string fecha { get; set; }
        public string codigo { get; set; }
        public string concepto { get; set; }
        public double valorTransaccion { get; set; }
        public string valorAdquirido { get; set; }
        public double valorActual { get; set; }
        public double depreAcum { get; set; }
        public double depreAnual { get; set; }
        public double montoTransaccion { get; set; }
        public string descripcion { get; set; }
        public double total { get; set; }

        internal Phrase Select()
        {
            throw new NotImplementedException();
        }
    }
}
