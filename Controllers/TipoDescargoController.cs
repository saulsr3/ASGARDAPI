using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ASGARDAPI.Clases;
using ASGARDAPI.Models;

namespace ASGARDAPI.Controllers
{
    public class TipoDescargoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("api/TipoDescargo/listarTipoDescargo")]
        public IEnumerable<TipoDescargoAF> listarTipoDescargo()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<TipoDescargoAF> listaDescargo = (from descargo in bd.TipoDescargo
                                                               where descargo.Dhabilitado == 1
                                                               select new TipoDescargoAF
                                                               {
                                                                   IdTipo = descargo.IdTipo,
                                                                   Nombre = descargo.Nombre,
                                                                   Descripcion = descargo.Descripcion,
                                                                   
                                                               }).ToList();
                return listaDescargo;
            }
        }
        [HttpGet]
        [Route("api/TipoDescargo/validarlistarTipoDescargo")]
        public int validarlistarTipoDescargo()
        {
            int respuesta = 0;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<TipoDescargoAF> lista = (from descargo in bd.TipoDescargo
                                                             where descargo.Dhabilitado == 1
                                                             select new TipoDescargoAF
                                                             {
                                                                 IdTipo = descargo.IdTipo,
                                                                 Nombre = descargo.Nombre,
                                                                 Descripcion = descargo.Descripcion,

                                                             }).ToList();
                if (lista.Count() > 0)
                {
                    respuesta = 1;
                }
                return respuesta;
            }
        }


        //Método guardar 
        [HttpPost]
        [Route("api/TipoDescargo/guardarTipoDescargo")]
        public int guardarTipoDescargo([FromBody]TipoDescargoAF oTipoDescargoAF)
        {
            int rpta = 0;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {

                    TipoDescargo oTipoDescargo = new TipoDescargo();
                    oTipoDescargo.IdTipo = oTipoDescargoAF.IdTipo;
                    oTipoDescargo.Nombre = oTipoDescargoAF.Nombre;
                    oTipoDescargo.Descripcion = oTipoDescargoAF.Descripcion;
                    oTipoDescargo.Dhabilitado = 1;

                    bd.TipoDescargo.Add(oTipoDescargo);
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


        [HttpPost]
        [Route("api/TipoDescargo/modificarTipoDescargo")]
        public int modificarTipoDescargo([FromBody]TipoDescargoAF oTipoDescargoAF)
        {
            int rpta = 0;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    //para editar tenemos que sacar la informacion
                    TipoDescargo oTipoDescargo = bd.TipoDescargo.Where(p => p.IdTipo == oTipoDescargoAF.IdTipo).First();
                    oTipoDescargo.Nombre = oTipoDescargoAF.Nombre;
                    oTipoDescargo.Descripcion = oTipoDescargoAF.Descripcion;
                   
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


        //Metodo eliminar Proveedor
        [HttpGet]
        [Route("api/TipoDescargo/eliminarTipoDescargo/{idTipoDescargo}")]
        public int eliminarTipoDescargo(int idTipoDescargo)
        {
            int respuesta = 0;

            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    TipoDescargo oTipoDescargo = bd.TipoDescargo.Where(p => p.IdTipo == idTipoDescargo).First();
                    oTipoDescargo.Dhabilitado = 0;
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
        [Route("api/TipoDescargo/RecuperarTipoDescargo/{id}")]
        public TipoDescargoAF RecuperarTipoDescargo(int id)
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                TipoDescargoAF oTipoDescargoAF = new TipoDescargoAF();
                TipoDescargo oTipoDescargo = bd.TipoDescargo.Where(p => p.IdTipo == id).First();
                oTipoDescargoAF.IdTipo = oTipoDescargo.IdTipo;
                oTipoDescargoAF.Nombre = oTipoDescargo.Nombre;
                oTipoDescargoAF.Descripcion = oTipoDescargo.Descripcion;
               
                return oTipoDescargoAF;
            }
        }

        [HttpGet]
        [Route("api/TipoDescargo/buscarTipoDescargo/{buscador?}")]
        public IEnumerable<TipoDescargoAF> buscarTipoDescargo(string buscador = "")
        {
            List<TipoDescargoAF> listaTipoDescargo;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                if (buscador == "")
                {
                    listaTipoDescargo = (from descargo in bd.TipoDescargo
                                      where descargo.Dhabilitado == 1
                                      select new TipoDescargoAF
                                      {
                                          IdTipo = descargo.IdTipo,
                                          Nombre = descargo.Nombre,
                                          Descripcion = descargo.Descripcion
                                          
                                      }).ToList();

                    return listaTipoDescargo;
                }
                else
                {
                    listaTipoDescargo = (from descargo in bd.TipoDescargo
                                         where descargo.Dhabilitado == 1

                                      && ((descargo.IdTipo).ToString().Contains(buscador)
                                      || (descargo.Nombre).ToLower().Contains(buscador.ToLower())
                                      || (descargo.Descripcion).ToLower().Contains(buscador.ToLower())
                                      )
                                         select new TipoDescargoAF
                                         {
                                             IdTipo = descargo.IdTipo,
                                             Nombre = descargo.Nombre,
                                             Descripcion = descargo.Descripcion

                                         }).ToList();

                    return listaTipoDescargo;
                }
            }
        }

        //Metodo para ver si el tipo descargo se esta utilizando en un activo
        [HttpGet]
        [Route("api/TipoDescargo/validarActivo/{idTipo}")]
        public int validarActivo(int idTipo)
        {
            int res = 0;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    TipoDescargo oTipo = bd.TipoDescargo.Where(p => p.IdTipo == idTipo).First();
                    SolicitudBaja oArea = bd.SolicitudBaja.Where(p => p.IdTipoDescargo == oTipo.IdTipo).First();
                    res = 1;
                }
            }
            catch (Exception ex)
            {
                res = 0;
            }
            return res;
        }

        [HttpGet]
        [Route("api/TipoDescargo/validarTipoDescargo/{idtipo}/{nombre}")]
        public int validarTipoDescargo(int idtipo, string nombre)
        {
            int rpta = 0;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    if (idtipo == 0)
                    {
                        rpta = bd.TipoDescargo.Where(p => p.Nombre.ToLower() == nombre.ToLower()
                        && p.Dhabilitado == 1).Count();
                    }
                    else
                    {
                        rpta = bd.TipoDescargo.Where(p => p.Nombre.ToLower() == nombre.ToLower() && p.IdTipo != idtipo
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