using System;
using System.Collections.Generic;

namespace ASGARDAPI.Models
{
    public partial class TipoTraspaso
    {
        public int IdTipo { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int? Dhabilitado { get; set; }
    }
}
