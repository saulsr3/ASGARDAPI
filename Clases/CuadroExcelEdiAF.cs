using System;
namespace ASGARDAPI.Clases
{
    public class CuadroExcelEdiAF
    {
        //Variables
        public string codigo { get; set; }
        public string descripcion { get; set; }
        public string fechaAdquisicion { get; set; }
        public double valorAdquisicion { get; set; }
        public double depreciacion { get; set; }
        public double depreciacionAcumulada { get; set; }
        public double valorActual { get; set; }
    }
}
