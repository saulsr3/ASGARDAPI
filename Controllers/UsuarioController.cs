using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ASGARDAPI.Clases;
using ASGARDAPI.Models;

namespace ASGARDAPI.Controllers
{
    public class UsuarioController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("api/Usuario/listarUsuario")]

        public IEnumerable<UsuarioAF> listarUsuario()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                List<UsuarioAF> listaUsuario = (from usuario in bd.Usuario
                                                join empleado in bd.Empleado
                                                on usuario.IdEmpleado equals empleado.Dui
                                                join tipoUsuario in bd.TipoUsuario
                                                on usuario.IdTipoUsuario equals tipoUsuario.IdTipoUsuario
                                                where usuario.Dhabilitado == 1
                                                select new UsuarioAF
                                                {
                                                    iidusuario = usuario.IdUsuario,
                                                    nombreusuario = usuario.NombreUsuario,
                                                    nombreEmpleado = empleado.Nombres + "" + empleado.Apellidos,
                                                    nombreTipoUsuario = tipoUsuario.TipoUsuario1

                                                }).ToList();
                return listaUsuario;
            }
        }

        [HttpPost]
        [Route("api/Usuario/guardarUsuario")]
        public int guardarUsuario([FromBody]UsuarioAF oUsuarioAF)
        {
            int rpta = 0;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {

                    Usuario oUsuario = new Usuario();
                    oUsuario.IdUsuario = oUsuarioAF.iidusuario;
                    oUsuario.NombreUsuario = oUsuarioAF.nombreusuario;
                    oUsuario.Contra = oUsuarioAF.contra;
                    oUsuario.IdEmpleado = oUsuarioAF.nombreEmpleado;
                    oUsuario.IdTipoUsuario = oUsuarioAF.iidTipousuario;

                    oUsuario.Dhabilitado = 1;

                    bd.Usuario.Add(oUsuario);
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