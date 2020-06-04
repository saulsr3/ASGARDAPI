using ASGARDAPI.Clases;
using ASGARDAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

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


        [HttpGet]
        [Route("api/TipoUsuario/ListarTipoUsuarios")]
        public IEnumerable<TipoUsuarioAF> ListarTipoUsuarios()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {

                IEnumerable<TipoUsuarioAF> listaTipoUsuario = (from tipoUsuario in bd.TipoUsuario
                                                               where tipoUsuario.Dhabilitado == 1
                                                               select new TipoUsuarioAF
                                                               {
                                                                   iidtipousuario = tipoUsuario.IdTipoUsuario,
                                                                   tipo = tipoUsuario.TipoUsuario1,
                                                                   descripcion = tipoUsuario.Descripcion
                                                               }).ToList();

                return listaTipoUsuario;
            }


        }



        [HttpGet]
        //api/nombreControlador/nombreMetodo
        [Route("api/TipoUsuario/eliminarTipoUsuario/{iidTipoUsuario}")]
        public int eliminarTipoUsuario(int iidTipoUsuario)
        {
            //para eliminar solo se cambia de 1 a 0
            int rpta = 0;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    //definimos un objetotipo usuario y sacamos todo el registro

                    TipoUsuario oTipoUsuario = bd.TipoUsuario.Where(p => p.IdTipoUsuario == iidTipoUsuario).First();
                    oTipoUsuario.Dhabilitado = 0;
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
        [Route("api/TipoUsuario/RecuperarTipoUsuario/{id}")]
        public TipoUsuarioAF RecuperarTipoUsuario(int id)
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                TipoUsuarioAF oTipoUsuarioAF = new TipoUsuarioAF();
                TipoUsuario oTipoUsuario = bd.TipoUsuario.Where(p => p.IdTipoUsuario == id).First();
                oTipoUsuarioAF.iidtipousuario = oTipoUsuario.IdTipoUsuario;
                oTipoUsuarioAF.tipo = oTipoUsuario.TipoUsuario1;
                oTipoUsuarioAF.descripcion = oTipoUsuario.Descripcion;

                return oTipoUsuarioAF;
            }
        }

        [HttpPost]
        [Route("api/TipoUsuario/modificarTipoUsuario")]
        public int modificarTipoUsuario([FromBody]TipoUsuarioAF oTipoUsuarioAF)
        {
            int rpta = 0;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    //para editar tenemos que sacar la informacion
                    TipoUsuario oTipoUsuario = bd.TipoUsuario.Where(p => p.IdTipoUsuario == oTipoUsuarioAF.iidtipousuario).First();
                    oTipoUsuario.IdTipoUsuario = oTipoUsuarioAF.iidtipousuario;
                    oTipoUsuario.TipoUsuario1 = oTipoUsuarioAF.tipo;
                    oTipoUsuario.Descripcion = oTipoUsuarioAF.descripcion;

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
        [Route("api/TipoUsuario/buscarTipoUsuario/{buscador?}")]
        public IEnumerable<TipoUsuarioAF> buscarTipoUsuario(string buscador = "")
        {
            List<TipoUsuarioAF> listaTipoUsuario;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                if (buscador == "")
                {
                    listaTipoUsuario = (from tipousuario in bd.TipoUsuario
                                        where tipousuario.Dhabilitado == 1
                                        select new TipoUsuarioAF
                                        {
                                            iidtipousuario = tipousuario.IdTipoUsuario,
                                            tipo = tipousuario.TipoUsuario1,
                                            descripcion = tipousuario.Descripcion
                                        }).ToList();

                    return listaTipoUsuario;
                }
                else
                {
                    listaTipoUsuario = (from tipousuario in bd.TipoUsuario
                                        where tipousuario.Dhabilitado == 1

                                        && ((tipousuario.IdTipoUsuario).ToString().Contains(buscador)
                                        || (tipousuario.TipoUsuario1).ToLower().Contains(buscador.ToLower())
                                        || (tipousuario.Descripcion).ToLower().Contains(buscador.ToLower()))
                                        select new TipoUsuarioAF
                                        {
                                            iidtipousuario = tipousuario.IdTipoUsuario,
                                            tipo = tipousuario.TipoUsuario1,
                                            descripcion = tipousuario.Descripcion
                                        }).ToList();

                    return listaTipoUsuario;
                }
            }
        }


        [HttpGet]
        [Route("api/TipoUsuario/validarTipoUsuario/{iidtipousuario}/{tipo}")]
        public int validarTipoUsuario(int iidtipousuario, string tipo)
        {
            int rpta = 0;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    if (iidtipousuario == 0)
                    {
                        rpta = bd.TipoUsuario.Where(p => p.TipoUsuario1.ToLower() == tipo.ToLower()
                        && p.Dhabilitado == 1).Count();
                    }
                    else
                    {
                        rpta = bd.TipoUsuario.Where(p => p.TipoUsuario1.ToLower() == tipo.ToLower() && p.IdTipoUsuario != iidtipousuario
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

    }
}