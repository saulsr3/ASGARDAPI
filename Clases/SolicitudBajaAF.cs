﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASGARDAPI.Clases
{
    public class SolicitudBajaAF
    {

        public int idsolicitud { get; set; }
        public string folio { get; set; }
        public DateTime? fechasolicitud { get; set; }
        public string observaciones { get; set; }
        public int motivo { get; set; }
        public int estado { get; set; }
        public string entidadbeneficiaria { get; set; }
        public string domicilio { get; set; }
        public string contacto { get; set; }
        public string telefono { get; set; }

        ///----------------------------------------------
        public string fechacadena { get; set; }
        public int idbien { get; set; } 
      

    }
}