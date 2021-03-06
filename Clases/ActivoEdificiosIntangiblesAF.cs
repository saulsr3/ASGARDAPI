﻿using System;
namespace ASGARDAPI.Clases
{
    public class ActivoEdificiosIntangiblesAF
    {
        //Variables
        public int idbien { get; set; }
        public string fechaingreso { get; set; }
        public string personaentrega { get; set; }
        public string personarecibe { get; set; }
        public string observaciones { get; set; }
        public int noformularioactivo { get; set; }
        public string descripcion { get; set; }
        public int tipoadquicicion { get; set; }
        public int idclasificacion { get; set; }
        public int idproveedor { get; set; }
        public int iddonante { get; set; }
        public string ProvDon { get; set; }
        public int IsProvDon { get; set; }
        public string estadoingreso { get; set; }
        public double valoradquicicion { get; set; }
        public string plazopago { get; set; }
        public double prima { get; set; }
        public double cuotaasignada { get; set; }
        public double interes { get; set; }
        public double valorresidual { get; set; }
        public string foto { get; set; }
        public int vidautil { get; set; }
        public int idsucursal { get; set; }
        public int idarea { get; set; }
    }
}
