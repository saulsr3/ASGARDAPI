using System;
using System.Collections.Generic;

namespace ASGARDAPI.Models
{
    public partial class Tecnicos
    {
        public Tecnicos()
        {
            InformeMantenimiento = new HashSet<InformeMantenimiento>();
        }

        public int IdTecnico { get; set; }
        public string Nombre { get; set; }
        public string Empresa { get; set; }
        public int? Dhabilitado { get; set; }

        public ICollection<InformeMantenimiento> InformeMantenimiento { get; set; }
    }
}
