using System;
using System.Collections.Generic;

namespace ASGARDAPI.Models
{
    public partial class Cooperativa
    {
        public Cooperativa()
        {
            Periodo = new HashSet<Periodo>();
            Sucursal = new HashSet<Sucursal>();
        }

        public int IdCooperativa { get; set; }
        public string Nombre { get; set; }
        public string Logo { get; set; }
        public string Descripcion { get; set; }
        public int? Dhabilitado { get; set; }

        public ICollection<Periodo> Periodo { get; set; }
        public ICollection<Sucursal> Sucursal { get; set; }
    }
}
