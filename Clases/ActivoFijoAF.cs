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
        public string Color { get; set; }
        public string Modelo { get; set; }
        public int idclasificacion { get; set; }


        //Variables que faltan de activo fijo 
        public int tipoadquicicion { get; set; }
        public int idmarca { get; set; }
        public int idproveedor { get; set; }
        public int iddonante { get; set; }
        public int idresponsable { get; set; } // Empleado
        public string estadoingreso { get; set; }
        public double valoradquicicion { get; set; }
        public string plazopago { get; set; }
        public double prima { get; set; }
        public double cuotaasignada { get; set; }
        public double interes { get; set; }
        public double valorresidual { get; set; }
        public string foto { get; set; }
    }
}
