using System;
using System.Collections.Generic;

namespace ASGARDAPI.Models
{
    public partial class TipoDescargo
    {
        public TipoDescargo()
        {
            SolicitudBaja = new HashSet<SolicitudBaja>();
        }

        public int IdTipo { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int? Dhabilitado { get; set; }

        public ICollection<SolicitudBaja> SolicitudBaja { get; set; }
    }
}
