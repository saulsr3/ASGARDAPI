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
        //para mostrar las fechas en string
        public string fechacadena { get; set; }
        public string Desripcion { get; set; }
        public string Clasificacion { get; set; }
        public string Marca { get; set; }
        public string AreaDeNegocio { get; set; }
        public string Resposnsable { get; set; }

        ///////////////////////////////////////////////
      
        public string codigobarra { get; set; }
        public string modelo { get; set; }
        public int tipoadquisicion { get; set; }
        public string color { get; set; }
        public string numserie { get; set; }
        public int idmarca { get; set; }
        public int idclasificacion { get; set; }
        public int idproveedor { get; set; }
        public int iddonante { get; set; }
        public int vidautil { get; set; }
        public int idresponsable { get; set; }
        public string estadoingreso { get; set; }
        public float valoradquisicion { get; set; }
        public string plazopago { get; set; }
        public float prima { get; set; }
        public float cuotaasignada { get; set; }
        public float intereses { get; set; }
        public float valorresidual { get; set; }
        public string foto { get; set; }
        public int estadoasignado { get; set; }
        public string destinoinicial { get; set; }
        public int estadoactual { get; set; }
      

    }
}
