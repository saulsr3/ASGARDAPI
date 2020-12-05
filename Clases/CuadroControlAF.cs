using System;
namespace ASGARDAPI.Clases
{
    public class CuadroControlAF
    {
        //Variables
        public int idbien { get; set; }
        public string codigo { get; set; }
        public string descripcion { get; set; }
        public string fechaadquisicion { get; set; }
        public double valoradquisicion { get; set; }
        public double depreciacion { get; set; }
        public double depreciacionacumulada { get; set; }
        public double valoractual { get; set; }
        public string ubicacion { get; set; }
        public string responsable { get; set; }
        public string concepto { get; set; }
    }
}
