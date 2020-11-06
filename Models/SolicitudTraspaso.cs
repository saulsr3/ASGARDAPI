using System;
using System.Collections.Generic;

namespace ASGARDAPI.Models
{
    public partial class SolicitudTraspaso
    {
        public int IdSolicitud { get; set; }
        public DateTime? Fecha { get; set; }
        public DateTime? Fechatraspaso { get; set; }
        public string Folio { get; set; }
        public string Descripcion { get; set; }
        public int? IdBien { get; set; }
        public int? Estado { get; set; }
        public string ResponsableAnterior { get; set; }
        public string AreadenegocioAnterior { get; set; }
        public string Acuerdo { get; set; }

        public ActivoFijo IdBienNavigation { get; set; }
    }
}
