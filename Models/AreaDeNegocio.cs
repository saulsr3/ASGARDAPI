using System;
using System.Collections.Generic;

namespace ASGARDAPI.Models
{
    public partial class AreaDeNegocio
    {
        public AreaDeNegocio()
        {
            Empleado = new HashSet<Empleado>();
        }

        public int IdAreaNegocio { get; set; }
        public string Nombre { get; set; }
        public int? IdSucursal { get; set; }
        public string Correlativo { get; set; }
        public int? Dhabilitado { get; set; }

        public Sucursal IdSucursalNavigation { get; set; }
        public ICollection<Empleado> Empleado { get; set; }
    }
}
