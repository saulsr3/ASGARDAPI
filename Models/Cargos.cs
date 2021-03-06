﻿using System;
using System.Collections.Generic;

namespace ASGARDAPI.Models
{
    public partial class Cargos
    {
        public Cargos()
        {
            Empleado = new HashSet<Empleado>();
        }

        public int IdCargo { get; set; }
        public string Cargo { get; set; }
        public string Descripcion { get; set; }
        public int? Dhabilitado { get; set; }

        public ICollection<Empleado> Empleado { get; set; }
    }
}
