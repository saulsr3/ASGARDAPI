using System;
using System.Collections.Generic;

namespace ASGARDAPI.Models
{
    public partial class SolicitudBaja
    {
        public int IdSolicitud { get; set; }
        public DateTime? Fecha { get; set; }
        public string Folio { get; set; }
        public string Observaciones { get; set; }
        public int? Motivo { get; set; }
        public int? IdBien { get; set; }
        public int? Estado { get; set; }
        public string EntidadBeneficiaria { get; set; }
        public string Domicilio { get; set; }
        public string Contacto { get; set; }
        public string Telefono { get; set; }
        public string Acuerdo { get; set; }

        public ActivoFijo IdBienNavigation { get; set; }
    }
}
