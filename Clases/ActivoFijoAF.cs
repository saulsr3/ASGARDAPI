using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASGARDAPI.Clases
{
    public class ActivoFijoAF
    {
        //vaya vieja esta clase la compartimos los atributos que ya esten se pueden reutilizar para sus funciones
        public int IdBien { get; set; }
        public int NoFormulario { get; set; }
        public string Codigo { get; set; }
        public DateTime? FechaIngreso { get; set; }
        public string fechacadena { get; set; }
        public string Desripcion { get; set; }
        public string Clasificacion { get; set; }
        public string Marca { get; set; }
        public string AreaDeNegocio { get; set; }
        public string Resposnsable { get; set; }
        public string color { get; set; }
        public string modelo { get; set; }
        public int idclasificacion { get; set; }
    }
}
