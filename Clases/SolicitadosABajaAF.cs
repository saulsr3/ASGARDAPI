﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASGARDAPI.Clases
{
    public class SolicitadosABajaAF
    {
        public string fechacadena { get; set; }
        public int idbien { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public string areanegocio { get; set; }
        public string responsable { get; set; }
        public int NoSolicitud { get; set; }

        public string folio { get; set; }
        //public DateTime? fechasolicitud { get; set; }
        public string observaciones { get; set; }
        public int motivo { get; set; }
        public int estado { get; set; }
        public string entidadbeneficiaria { get; set; }
        //public string domicilio { get; set; }
        //public string contacto { get; set; }
        //public string telefono { get; set; }
    }
}