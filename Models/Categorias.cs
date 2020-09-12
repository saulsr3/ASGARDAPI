using System;
using System.Collections.Generic;

namespace ASGARDAPI.Models
{
    public partial class Categorias
    {
        public Categorias()
        {
            Clasificacion = new HashSet<Clasificacion>();
        }

        public int IdCategoria { get; set; }
        public string Categoria { get; set; }
        public int? VidaUtil { get; set; }
        public string Descripcion { get; set; }
        public int? Dhabilitado { get; set; }

        public ICollection<Clasificacion> Clasificacion { get; set; }
    }
}
