using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASGARDAPI.Clases
{
    public class TarjetaDatosAF
    {
        public string codigo { get; set; }
        public string descripcion { get; set; }
        public string fechaAdquicicion { get; set; }
        public float valor { get; set; }
        public float prima { get; set; }
        public float cuota { get; set; }
        public int interes { get; set; }
        public string proveedor { get; set; }
        public string direccion { get; set; }
        public string telefono { get; set; }
        public string color { get; set; }
        public string modelo { get; set; }
        public string noSerie { get; set; }
        public int vidaUtil { get; set; }
        public string sistemaDepreciacion { get; set; }
        public float tasaAnual { get; set; }
        public float valorResidual { get; set; }
        public string Observaciones { get; set; }
    }
}
