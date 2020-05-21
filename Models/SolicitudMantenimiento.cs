using System;
using System.Collections.Generic;

namespace ASGARDAPI.Models
{
    public partial class SolicitudMantenimiento
    {
        public int IdSolicitud { get; set; }
        public DateTime? Fecha { get; set; }
        public int? Folio { get; set; }
    }
}
