using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASGARDAPI.Clases
{
    public class RegistroAF
    {
        public int IdBien { get; set; }
        public string Codigo { get; set; }
        public string fechacadena { get; set; }
        public string Descripcion { get; set; }
        public string Clasificacion { get; set; }
        public string AreaDeNegocio { get; set; }
        public string Responsable { get; set; }

        //para actualizar la vida util despues de buscar

        public int? vidautil { get; set; }
    }
}
