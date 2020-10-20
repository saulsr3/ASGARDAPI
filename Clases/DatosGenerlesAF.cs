using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASGARDAPI.Clases
{
    public class DatosGenerlesAF
    {
        public int idBien { get; set; }
        public string descripcion { get; set; }
        public string codigo { get; set; }
        public string fecha { get; set; }
        public string valorAquisicion { get; set; }
        public string Clasificacion { get; set; }
        public string Respondable { get; set; }
        public string Ubicacion { get; set; }
        public string valorActual { get; set; }
        public string ProvDon { get; set; }
        public int IsProvDon { get; set; }
        public string NoSerie { get; set; }
        public string VidaUtil { get; set; }
        public string Observaciones { get; set; }
        public string foto { get; set; }
    }
}
