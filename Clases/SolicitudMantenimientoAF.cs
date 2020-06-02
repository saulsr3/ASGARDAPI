using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ASGARDAPI.Clases
{
    public class SolicitudMantenimientoAF
    {
        public int idsolicitud { get; set; }
        public string folio { get; set; }
        public DateTime? fechasolicitud { get; set; }
        public string areasolicitante  { get; set; }
        public string sucursal { get; set; } 
        public string personasolicitante { get; set; }

        public string fechacadena { get; set; }

        //datos del modal

        public string codigobien { get; set; }

        public string descripcionbien { get; set; }
        public string razonesmantenimiento { get; set; }
        public string periodomantenimiento { get; set; }

        //pk relacioandas
        
        public int idmantenimiento { get; set; }
        public int idbien { get; set; } //tabla activo fijo
        public int idresponsable { get; set; } //foranea en activo fijo, pk de empelado

        public string nombrecompleto { get; set; }
        public string nombres { get; set; }
        public string apellidos { get; set; }

        public string areadenegocio { get; set; } 
        public int idareadenegocio { get; set; } //fk de empelado, pk de area de negocio
        public int idsucursal { get; set; } //fk de area de negocio, pk de sucursal








    }
}
