using System;
using System.Collections.Generic;

namespace ASGARDAPI.Models
{
    public partial class Token
    {
        public int IdToken { get; set; }
        public int? Codigo { get; set; }
        public int? IdUsuario { get; set; }
        public int? Estado { get; set; }

        public Usuario IdUsuarioNavigation { get; set; }
    }
}
