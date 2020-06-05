using System;
namespace ASGARDAPI.Clases
{
    public class FormularioIngresoAF
    {
        //Variables
        public int noformulario { get; set; }
        public DateTime? fechaingreso { get; set; }
        public string nofactura { get; set; }
        public string personaentrega { get; set; }
        public string personarecibe { get; set; }
        public string observaciones { get; set; }

    }
}
