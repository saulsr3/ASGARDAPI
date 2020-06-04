using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASGARDAPI.Clases
{
    public class BienesAF
    {
        public string correlativobien { get; set; }
        public string codigobarra { get; set; }
        public string tipoactivo { get; set; }
        public string area { get; set; }
        public string responsable { get; set; }
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
        public int idbien { get; set; }
        public int numformulario { get; set; }
        public DateTime? fechaingreso { get; set; }
        ////para mostrar las fechas en string
        public string fechacadena { get; set; }
        public string descripcion { get; set; }
        //public string Clasificacion { get; set; }
        //public string Marca { get; set; }

    }
}
