using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ASGARDAPI.Clases;
using ASGARDAPI.Models;

namespace ASGARDAPI.Controllers
{
    public class TipoUsuarioController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("api/TipoUsuario/guardarTipoUsuario")]
        public int guardarTipoUsuario([FromBody] TipoUsuarioAF oTipoUsuarioAF)
        {
            int rpta = 0;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {

                    TipoUsuario oTipoUsuario = new TipoUsuario();
                    oTipoUsuario.IdTipoUsuario = oTipoUsuarioAF.iidtipousuario;
                    oTipoUsuario.TipoUsuario1 = oTipoUsuarioAF.tipo;
                    oTipoUsuario.Descripcion = oTipoUsuarioAF.descripcion;
                    oTipoUsuario.Dhabilitado = 1;

                    bd.TipoUsuario.Add(oTipoUsuario);
                    bd.SaveChanges();
                    rpta = 1;
                }
            }


            catch (Exception ex)
            {
                rpta = 0;
            }
            return rpta;
        }
    }
}