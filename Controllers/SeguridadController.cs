using ASGARDAPI.Clases;
using ASGARDAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;


namespace ASGARDAPI.Controllers
{
    public class SeguridadController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [Route("api/Usuario/login")]
        public UsuarioAF login([FromBody] UsuarioAF oUsuarioAF) {
            int rpta = 0;
            UsuarioAF oUsuario = new UsuarioAF();
            using (BDAcaassAFContext bd = new BDAcaassAFContext()) {
               
                SHA256Managed sha = new SHA256Managed();
                byte[] dataNoCifrada = Encoding.Default.GetBytes(oUsuarioAF.contra);
                byte[] dataCifrada = sha.ComputeHash(dataNoCifrada);
                string claveCifrada = BitConverter.ToString(dataCifrada).Replace("-", "");
                rpta = bd.Usuario.Where(p => p.NombreUsuario.ToUpper() == oUsuarioAF.nombreusuario.ToUpper() && p.Contra == claveCifrada).Count();
                if (rpta == 1)
                {
                    Usuario oUsuarioRecuperar = bd.Usuario.Where(p => p.NombreUsuario.ToUpper() == oUsuarioAF.nombreusuario.ToUpper() && p.Contra == claveCifrada).First();
                    oUsuario.iidusuario = oUsuarioRecuperar.IdUsuario;
                    oUsuario.nombreusuario = oUsuarioRecuperar.NombreUsuario;
                    oUsuario.iidEmpleado = (int)oUsuarioRecuperar.IdEmpleado;
                    oUsuario.iidTipousuario = (int)oUsuarioRecuperar.IdTipoUsuario;
                }
                else {
                    oUsuario.iidusuario = 0;
                    oUsuario.nombreusuario = "";
                }
            }
            return oUsuario;
        }
        [HttpGet]
        [Route("api/Seguridad/listarBitacora")]
        public IEnumerable<TablaBitacoraAF> listarBitacora()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<TablaBitacoraAF> listaProveedores = (from bitacora in bd.Bitacora
                                                                 join usuario in bd.Usuario
                                                                  on bitacora.IdUsuario equals usuario.IdUsuario
                                                                 join empleado in bd.Empleado
                                                                 on usuario.IdEmpleado equals empleado.IdEmpleado
                                                                 orderby bitacora.Fecha descending
                                                                 select new TablaBitacoraAF
                                                               {
                                                                   idBitacora = bitacora.IdBitacora,
                                                                   nombreEmpleado = empleado.Nombres+ " " +empleado.Apellidos,
                                                                   nombreUsuario = usuario.NombreUsuario,
                                                                   fecha = bitacora. Fecha == null ? " " : ((DateTime)bitacora.Fecha).ToString("dd-MM-yyyy : HH:mm:ss"),
                                                                   descripcion = bitacora.Descripcion,
                                                               }).ToList();
                return listaProveedores;
            }
        }

        //[HttpGet]
        //[Route("api/Usuario/obtenervariableSession")]
        //public SeguridadAF obtenervariableSession()
        //{
        //    SeguridadAF oSeguridad = new SeguridadAF();
        //    //HttpContext.Session.SetString("usuario", "User1");
        //    var variableSession = HttpContext.Session.GetString("usuario");
        //    if (variableSession == null)
        //    {
        //        oSeguridad.valor = "";
        //    }
        //    else
        //    {
        //        oSeguridad.valor = variableSession;
        //    }
        //    //oSeguridad.valor = variableSession;
        //    return oSeguridad;
        //}
        //// metodos de prueba abre aqui
        //[HttpGet]
        //[Route("api/Usuario/crearSession")]
        //public int crearSession()
        //{
        //    int rpta = 0;
        //    UsuarioAF user = new UsuarioAF();
        //    user.nombreusuario = "kevin";
        //    HttpContext.Session.SetString("usuario", "Sessionvalue");
        //    string variableSession = HttpContext.Session.GetString("usuario");
        //    if (variableSession != null)
        //    {
        //        rpta = 1;
        //    }
        //    return rpta;
        //}
        //[HttpGet]
        //[Route("api/Usuario/recuperarSession")]
        //public SeguridadAF recuperarSession()
        //{
        //    SeguridadAF oSeguridad = new SeguridadAF();
        //    var variableSession = HttpContext.Session.GetString("usuario");
        //    oSeguridad.valor = variableSession;
        //    return oSeguridad;
        //}
        ////cierra aqui

        //[HttpGet]
        //[Route("api/Usuario/CerrarSesion")]
        //public SeguridadAF CerrarSesion()
        //{
        //    SeguridadAF oSeguridadClS = new SeguridadAF();
        //    try
        //    {
        //        HttpContext.Session.Remove("usuario");
        //        oSeguridadClS.valor = "Ok";

        //    }
        //    catch (Exception ex)
        //    {
        //        oSeguridadClS.valor = "";
        //        throw;
        //    }
        //    return oSeguridadClS;
        //}
    }
}
