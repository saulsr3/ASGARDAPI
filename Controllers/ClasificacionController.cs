using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ASGARDAPI.Clases;
using ASGARDAPI.Models;

namespace ASGARDAPI.Controllers
{
    public class ClasificacionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [Route("api/Clasificacion/guardarClasificacion")]
        public int guardarClasificacion([FromBody]ClasificacionAF oClasificacionAF)
        {
            int respuesta = 0;

            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    Clasificacion oClasificacion = new Clasificacion();

                    oClasificacion.IdClasificacion = oClasificacionAF.idclasificacion;
                    oClasificacion.Correlativo = oClasificacionAF.correlativo;
                    oClasificacion.Clasificacion1 = oClasificacionAF.clasificacion;
                    oClasificacion.Descripcion = oClasificacionAF.descripcion;
                    oClasificacion.IdCategoria = oClasificacionAF.idcategoria;
                    oClasificacion.Dhabilitado = 1;
                    bd.Clasificacion.Add(oClasificacion);
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

        [HttpPost]
        [Route("api/Clasificacion/modificarclasificacion")]
        public int modificarclasificacion([FromBody]ClasificacionAF oClasificacionAF)
        {
            int respuesta = 0;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    Clasificacion oClasificacion = bd.Clasificacion.Where(p => p.IdClasificacion == oClasificacionAF.idclasificacion).First();
                    oClasificacion.IdClasificacion = oClasificacionAF.idclasificacion;
                    oClasificacion.Clasificacion1 = oClasificacionAF.clasificacion;
                    oClasificacion.IdCategoria = oClasificacionAF.idcategoria;
                    oClasificacion.Correlativo = oClasificacionAF.correlativo;
                    oClasificacion.Descripcion = oClasificacionAF.descripcion;
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
        // metodo para listar las clasificaciones de los activos
        [HttpGet]
        [Route("api/Clasificacion/listarClasificacion")]
        public IEnumerable<ClasificacionAF> listarClasificacion()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<ClasificacionAF> listaClasificacion = (from clasificacion in bd.Clasificacion
                                                                   join categorias in bd.Categorias
                                                                   on clasificacion.IdCategoria equals categorias.IdCategoria
                                                                   where clasificacion.Dhabilitado == 1
                                                                   select new ClasificacionAF
                                                                   {
                                                                       idclasificacion = clasificacion.IdClasificacion,
                                                                       correlativo = clasificacion.Correlativo,
                                                                       categoria = categorias.Categoria,
                                                                       clasificacion = clasificacion.Clasificacion1,
                                                                       descripcion = clasificacion.Descripcion

                                                                   }).ToList();
                return listaClasificacion;
            }
        }


        [HttpGet]
        [Route("api/Clasificacion/eliminarCasificacion/{idclasificacion}")]
        public int eliminarCasificacion(int idclasificacion)
        {
            int respuesta = 0;

            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    Clasificacion oClasificacion = bd.Clasificacion.Where(p => p.IdClasificacion == idclasificacion).First();
                    oClasificacion.Dhabilitado = 0;
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
        [Route("api/Clasificacion/RecuperarClasificacion/{id}")]
        public ClasificacionAF RecuperarClasificacion(int id)
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                ClasificacionAF oClasificacionAF = new ClasificacionAF();
                Clasificacion oClasificacion = bd.Clasificacion.Where(p => p.IdClasificacion == id).First();
                oClasificacionAF.idclasificacion = oClasificacion.IdClasificacion;
                oClasificacionAF.correlativo = oClasificacion.Correlativo;
                oClasificacionAF.idcategoria = (int) oClasificacion.IdCategoria;
                oClasificacionAF.clasificacion = oClasificacion.Clasificacion1;
                oClasificacionAF.descripcion = oClasificacion.Descripcion;

                return oClasificacionAF;
            }
        }

        [HttpGet]
        [Route("api/Clasificacion/listarCategoriaCombo")]
        public IEnumerable<CategoriasAF> listarCategoriaCombo()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<CategoriasAF> listarCategorias = (from categoria in bd.Categorias
                                                         where categoria.Dhabilitado == 1
                                                         select new CategoriasAF
                                                         {
                                                             IdCategoria = categoria.IdCategoria,
                                                             Categoria = categoria.Categoria
                                                         }).ToList();



                return listarCategorias;

            }
        }


        //para no poder editar el correlativo
        [HttpGet]
        [Route("api/Clasificacion/noEditCorrelativoClasificacion/{idClasificacion}")]
        public int noEditCorrelativoClasificacion(int idClasificacion)
        {
            int res = 0;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {

                    Clasificacion oClasificacion = bd.Clasificacion.Where(p => p.IdClasificacion == idClasificacion && p.Dhabilitado == 1).First();
                    ActivoFijo oActivo = bd.ActivoFijo.Where(p => p.IdClasificacion == oClasificacion.IdClasificacion && p.EstadoActual != 0).First();
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
        [Route("api/Clasificacion/validarCorrelativo/{idclasificacion}/{correlativo}")]
        public int validarCorrelativo(int idclasificacion, string correlativo)
        {
            int respuesta = 0;
            try
            {


                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    if (idclasificacion == 0)
                    {
                        respuesta = bd.Clasificacion.Where(p => p.Correlativo.ToLower() == correlativo.ToLower() && p.Dhabilitado == 1).Count();
                    }
                    else
                    {
                        respuesta = bd.Clasificacion.Where(p => p.Correlativo.ToLower() == correlativo.ToLower() && p.IdClasificacion != idclasificacion && p.Dhabilitado == 1).Count();
                    }


                }

            }
            catch (Exception ex)
            {
                respuesta = 0;

            }
            return respuesta;

        }


        [HttpGet]
        [Route("api/Clasificacion/validarClasificacion/{idclasificacion}/{clasificacion}")]
        public int validarClasificacion(int idclasificacion, string clasificacion)
        {
            int respuesta = 0;
            try
            {


                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    if (idclasificacion == 0)
                    {
                        respuesta = bd.Clasificacion.Where(p => p.Clasificacion1.ToLower() == clasificacion.ToLower() && p.Dhabilitado == 1).Count();
                    }
                    else
                    {
                        respuesta = bd.Clasificacion.Where(p => p.Clasificacion1.ToLower() == clasificacion.ToLower() && p.IdClasificacion != idclasificacion && p.Dhabilitado == 1).Count();
                    }


                }

            }
            catch (Exception ex)
            {
                respuesta = 0;

            }
            return respuesta;

        }

       //Metodo para no permitir elimiar una clasificacion de activo cuando ya hay activos con esa clasificación
        [HttpGet]
        [Route("api/Clasificacion/validarActivo/{idclasificacion}")]
        public int validarActivo(int idclasificacion)
        {
            int res = 0;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    ActivoFijo oClasificacion = bd.ActivoFijo.Where(p => p.IdClasificacion == idclasificacion && p.EstadoActual == 1
                     || p.EstadoActual == 2 || p.EstadoActual == 3 || p.EstadoActual == 4 || p.EstadoActual == 5).First();
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
        [Route("api/Clasificacion/buscarClasificacion/{buscador?}")]
        public IEnumerable<ClasificacionAF> buscarClasificacion(string buscador = "")
        {
            List<ClasificacionAF> listaClasificacion;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                if (buscador == "")
                {
                    listaClasificacion = (from clasificacion in bd.Clasificacion
                                          join categoria in bd.Categorias
                                          on clasificacion.IdCategoria equals categoria.IdCategoria
                                          where clasificacion.Dhabilitado == 1
                                          select new ClasificacionAF
                                          {
                                              idclasificacion = clasificacion.IdClasificacion,
                                              correlativo = clasificacion.Correlativo,
                                              categoria= categoria.Categoria,
                                              clasificacion = clasificacion.Clasificacion1,
                                              descripcion = clasificacion.Descripcion
                                          }).ToList();
                    return listaClasificacion;
                }
                else
                {
                    listaClasificacion = (from clasificacion in bd.Clasificacion
                                          join categoria in bd.Categorias
                                          on clasificacion.IdCategoria equals categoria.IdCategoria
                                          where clasificacion.Dhabilitado == 1

                                          && ((clasificacion.IdClasificacion).ToString().Contains(buscador) ||
                                          (clasificacion.Correlativo).ToLower().Contains(buscador.ToLower()) ||
                                          (categoria.Categoria).ToLower().Contains(buscador.ToLower()) ||
                                          (clasificacion.Descripcion).ToLower().Contains(buscador.ToLower()) ||
                                          (clasificacion.Clasificacion1).ToLower().Contains(buscador.ToLower()))
                                          select new ClasificacionAF
                                          {
                                              idclasificacion = clasificacion.IdClasificacion,
                                              correlativo = clasificacion.Correlativo,
                                              categoria = categoria.Categoria,
                                              clasificacion = clasificacion.Clasificacion1,
                                              descripcion = clasificacion.Descripcion
                                          }).ToList();
                    return listaClasificacion;
                }
            }
        }
    }
}