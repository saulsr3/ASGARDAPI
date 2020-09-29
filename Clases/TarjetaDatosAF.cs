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
        public string valor { get; set; }
        public string prima { get; set; }
        public string cuota { get; set; }
        public string plazo { get; set; }
        public string interes { get; set; }
        public string proveedor { get; set; }
        public string direccion { get; set; }
        public string telefono { get; set; }
        public string color { get; set; }
        public string marca { get; set; }
        public string modelo { get; set; }
        public string noSerie { get; set; }
        public string vidaUtil { get; set; }
        public string sistemaDepreciacion { get; set; }
        public string tasaAnual { get; set; }
        public string valorResidual { get; set; }
        public string Observaciones { get; set; }
        public int isProvDon { get; set; }
    }
}
