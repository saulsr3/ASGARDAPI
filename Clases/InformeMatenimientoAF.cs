using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASGARDAPI.Clases
{
    public class InformeMatenimientoAF
    {

        public int idinformematenimiento { get; set; }

        public int idBien { get; set; }
        public int idmantenimiento { get; set; }
        public string bienes { get; set; }
        public string fechacadena { get; set; }
        public DateTime? fechainforme { get; set; }
        public int idtecnico { get; set; }
        public string nombretecnico { get; set; } 
        public double costomateriales { get; set; }
        public double costomo { get; set; }
        public double costototal { get; set; }

        public int? vidautil { get; set; }


        //para los datos de arriba de la tabla:
        public string codigo { get; set; }
        public string descripcion { get; set; }
        public string encargado { get; set; }
        public string areadenegocio { get; set; }






    }
}
