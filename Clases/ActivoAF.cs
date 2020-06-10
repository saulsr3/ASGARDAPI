using System;
namespace ASGARDAPI.Clases
{
    public class ActivoAF
    {
        //Variables de activo fijo
        public int idbien { get; set; }
        public int noformularioactivo { get; set; }
        public string descripcion { get; set; }
        public string modelo { get; set; }
        public int tipoadquicicion { get; set; }
        public string color { get; set; }
        public int idmarca { get; set; }
        public int idclasificacion { get; set; }
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
