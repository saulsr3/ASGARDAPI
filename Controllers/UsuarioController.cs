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
                                                on usuario.IdEmpleado equals empleado.IdEmpleado
                                                join tipoUsuario in bd.TipoUsuario
                                                on usuario.IdTipoUsuario equals tipoUsuario.IdTipoUsuario
                                                where usuario.Dhabilitado == 1
                                                select new UsuarioAF
                                                {
                                                    iidusuario = usuario.IdUsuario,
                                                    nombreusuario = usuario.NombreUsuario,
                                                    nombreEmpleado = empleado.Nombres + " " + empleado.Apellidos,
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
                    oUsuario.IdEmpleado = oUsuarioAF.iidEmpleado;
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

        [HttpGet]
        [Route("api/Usuario/listarEmpleadoCombo")]
        public IEnumerable<EmpleadoAF> listarEmpleadoCombo()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<EmpleadoAF> listarEmpleado = (from empleado in bd.Empleado
                                                                 where empleado.Dhabilitado == 1
                                                                 && empleado.BtieneUsuario==0
                                                                 select new EmpleadoAF
                                                                 {
                                                                    dui = empleado.Dui,
                                                                   nombres = empleado.Nombres + " " + empleado.Apellidos,

                                                                 }).ToList();
                return listarEmpleado;

            }
        }

        [HttpGet]
        [Route("api/Usuario/listarTipoCombo")]
        public IEnumerable<TipoUsuarioAF> listarTipoCombo()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<TipoUsuarioAF> listarTipo = (from tipoUsuario in bd.TipoUsuario
                                                          where tipoUsuario.Dhabilitado == 1
                                                          select new TipoUsuarioAF
                                                          {
                                                              iidtipousuario = tipoUsuario.IdTipoUsuario,
                                                              tipo = tipoUsuario.TipoUsuario1,

                                                          }).ToList();
                return listarTipo;

            }
        }


        [HttpGet]
        [Route("api/Usuario/RecuperarUsuario/{id}")]
        public UsuarioAF RecuperarUsuario(int id)
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                UsuarioAF oUsuarioAF = new UsuarioAF();
                Usuario oUsuario = bd.Usuario.Where(p => p.IdUsuario == id).First();
                oUsuarioAF.iidusuario = oUsuario.IdUsuario;
                oUsuarioAF.nombreusuario = oUsuario.NombreUsuario;
                //oUsuarioAF.nombreEmpleado =  oUsuario.IdEmpleado;
                oUsuarioAF.iidTipousuario = (int)oUsuario.IdTipoUsuario;  

                return oUsuarioAF;
            }
        }

        [HttpPost]
        [Route("api/Usuario/modificarUsuario")]
        public int modificarUsuario([FromBody]UsuarioAF oUsuarioAF)
        {
            int rpta = 0;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    //para editar tenemos que sacar la informacion
                    Usuario oUsuario = bd.Usuario.Where(p => p.IdUsuario == oUsuarioAF.iidusuario).First();
                    oUsuario.NombreUsuario = oUsuarioAF.nombreusuario;
                    oUsuario.Contra = oUsuarioAF.contra;
                    oUsuario.IdEmpleado = oUsuarioAF.iidEmpleado;
                    oUsuario.IdTipoUsuario = oUsuarioAF.iidTipousuario;
                    //para guardar cambios
                    bd.SaveChanges();
                    //si todo esta bien
                    rpta = 1;
                }
            }
            catch (Exception ex)
            {
                rpta = 0;
            }
            return rpta;
        }

        [HttpGet]
        [Route("api/Usuario/eliminarUsuario/{iidusuario}")]
        public int eliminarUsuario(int iidusuario)
        {
            int respuesta = 0;

            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    Usuario oUsuario = bd.Usuario.Where(p => p.IdUsuario == iidusuario).First();
                    oUsuario.Dhabilitado = 0;
                    bd.SaveChanges();
                    respuesta = 1;
                }


            }
            catch (Exception ex)
            {

                respuesta = 0;
            }
            return respuesta;
        }



        [HttpGet]
        [Route("api/Usuario/validarUsuario/{iidusuario}/{tipo}")]
        public int validarUsuario(int iidusuario, string tipo)
        {
            int rpta = 0;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    if (iidusuario == 0)
                    {
                        rpta = bd.Usuario.Where(p => p.NombreUsuario.ToLower() == tipo.ToLower() 
                        && p.Dhabilitado == 1).Count();
                    }
                    else
                    {
                        rpta = bd.Usuario.Where(p => p.NombreUsuario.ToLower() == tipo.ToLower() && p.IdUsuario != iidusuario 
                        && p.Dhabilitado == 1).Count();
                    }
                }
            }
            catch (Exception ex)
            {
                //si se cae
                rpta = 0;

            }
            return rpta;
        }

        [HttpGet]
        [Route("api/Usuario/buscarUsuario/{buscador?}")]
        public IEnumerable<UsuarioAF> buscarUsuario(string buscador = "")
        {
            List<UsuarioAF> listaUsuario;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                if (buscador == "")
                {
                    listaUsuario = (from usuario in bd.Usuario
                                    join empleado in bd.Empleado
                                    on usuario.IdEmpleado equals empleado.IdEmpleado
                                    join tipoUsuario in bd.TipoUsuario
                                    on usuario.IdTipoUsuario equals tipoUsuario.IdTipoUsuario
                                    where usuario.Dhabilitado == 1
                                    select new UsuarioAF
                                    {
                                        iidusuario = usuario.IdUsuario,
                                        nombreusuario = usuario.NombreUsuario,
                                        contra = usuario.Contra,
                                        iidEmpleado = empleado.IdEmpleado,
                                       iidTipousuario = tipoUsuario.IdTipoUsuario

                                    }).ToList();

                    return listaUsuario;
                }
                else
                {
                    listaUsuario = (from usuario in bd.Usuario
                                    join empleado in bd.Empleado
                                   on usuario.IdEmpleado equals empleado.IdEmpleado
                                    join tipoUsuario in bd.TipoUsuario
                                    on usuario.IdTipoUsuario equals tipoUsuario.IdTipoUsuario
                                    where usuario.Dhabilitado == 1

                                    && ((usuario.IdUsuario).ToString().Contains(buscador)
                                      || (usuario.NombreUsuario).ToLower().Contains(buscador.ToLower())
                                      || (usuario.Contra).ToLower().Contains(buscador.ToLower())
                                      //|| (usuario.IdEmpleado).Contains(buscador.ToLower())
                                      //|| (usuario.IdTipoUsuario).ToLower().Contains(buscador.ToLower())
                                      )
                                    select new UsuarioAF
                                    {
                                        iidusuario = usuario.IdUsuario,
                                        nombreusuario = usuario.NombreUsuario,
                                        contra = usuario.Contra,
                                        // iidEmpleado = empleado.Dui,
                                        //iidTipousuario = tipoUsuario.TipoUsuario1
                                    }).ToList();

                    return listaUsuario;
                }
            }
        }

        

    }
}