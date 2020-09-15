﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ASGARDAPI.Clases;
using ASGARDAPI.Models;

namespace ASGARDAPI.Controllers
{
    public class CategoriasController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("api/Categorias/guardarCategorias")]
        public int guardarCategorias([FromBody]CategoriasAF oCategoriasAF)
        {
            int respuesta = 0;

            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    Categorias oCategorias = new Categorias();

                    oCategorias.IdCategoria = oCategoriasAF.IdCategoria;
                    oCategorias.VidaUtil = oCategoriasAF.VidaUtil;
                    oCategorias.Categoria = oCategoriasAF.Categoria;
                    oCategorias.Descripcion = oCategoriasAF.Descripcion;
                    oCategorias.Dhabilitado = 1;
                    bd.Categorias.Add(oCategorias);
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
        [Route("api/Categorias/modificarCategorias")]
        public int modificarCategorias([FromBody]CategoriasAF oCategoriasAF)
        {
            int respuesta = 0;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    Categorias oCategorias = bd.Categorias.Where(p => p.IdCategoria == oCategoriasAF.IdCategoria).First();
                    oCategorias.IdCategoria = oCategoriasAF.IdCategoria;
                    oCategorias.VidaUtil = oCategoriasAF.VidaUtil;
                    oCategorias.Categoria = oCategoriasAF.Categoria;
                    oCategorias.Descripcion = oCategoriasAF.Descripcion;
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

        // metodo para listar las categorias de los activos
        [HttpGet]
        [Route("api/Categorias/listarCategorias")]
        public IEnumerable<CategoriasAF> listarCategorias()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<CategoriasAF> listaCategorias = (from categorias in bd.Categorias
                                                                   where categorias.Dhabilitado == 1
                                                                   select new CategoriasAF

                                                                   { 
                                                                      IdCategoria = categorias.IdCategoria,
                                                                       VidaUtil =  categorias.VidaUtil,
                                                                       Categoria = categorias.Categoria,
                                                                       Descripcion = categorias.Descripcion

                                                                   }).ToList();
                return listaCategorias;
            }
        }

        [HttpGet]
        [Route("api/Categorias/eliminarCategorias/{idCategorias}")]
        public int eliminarCategorias(int idCategorias)
        {
            int respuesta = 0;

            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    Categorias oCategorias = bd.Categorias.Where(p => p.IdCategoria == idCategorias).First();
                    oCategorias.Dhabilitado = 0;
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
        [Route("api/Categorias/RecuperarCategorias/{id}")]
        public CategoriasAF RecuperarCategorias(int id)
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                CategoriasAF oCategoriasAF = new CategoriasAF();
                Categorias oCategorias = bd.Categorias.Where(p => p.IdCategoria == id).First();
                oCategoriasAF.IdCategoria = oCategorias.IdCategoria;
                oCategoriasAF.VidaUtil = oCategorias.VidaUtil;
                oCategoriasAF.Categoria = oCategorias.Categoria;
                oCategoriasAF.Descripcion = oCategorias.Descripcion;
        

                return oCategoriasAF;
            }
        }


        //esta validacion la dejaré para despues
        [HttpGet]
        [Route("api/Categorias/validarCategoria/{idcategoria}/{categoria}")]
        public int validarCategoria(int idcategoria, string categoria)
        {
            int respuesta = 0;
            try
            {


                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    if (idcategoria == 0)
                    {
                        respuesta = bd.Categorias.Where(p => p.Categoria.ToLower() == categoria.ToLower() && p.Dhabilitado == 1).Count();
                    }
                    else
                    {
                        respuesta = bd.Categorias.Where(p => p.Categoria.ToLower() == categoria.ToLower() && p.IdCategoria != idcategoria && p.Dhabilitado == 1).Count();
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
        [Route("api/Categorias/validarActivoc/{idcategorias}")]
        public int validarActivoc(int idcategorias)
        {
            int res = 0;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    Clasificacion oCategoria = bd.Clasificacion.Where(p => p.IdCategoria == idcategorias && p.Dhabilitado==1).First();
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
        [Route("api/Categorias/buscarCategorias/{buscador?}")]
        public IEnumerable<CategoriasAF> buscarCategorias(string buscador = "")
        {
            List<CategoriasAF> listaCategorias;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                if (buscador == "")
                {
                    listaCategorias = (from categorias in bd.Categorias
                                          where categorias.Dhabilitado == 1
                                          select new CategoriasAF
                                          {

                                              IdCategoria = categorias.IdCategoria,
                                              VidaUtil = categorias.VidaUtil,
                                              Categoria = categorias.Categoria,
                                              Descripcion = categorias.Descripcion
                                          }).ToList();
                    return listaCategorias;
                }
                else
                {
                    listaCategorias = (from categorias in bd.Categorias
                                          where categorias.Dhabilitado == 1

                                          && ((categorias.VidaUtil).ToString().Contains(buscador.ToLower()) ||
                                          (categorias.Categoria).ToLower().Contains(buscador.ToLower()) ||
                                          (categorias.Descripcion).ToLower().Contains(buscador.ToLower()))
                                          select new CategoriasAF
                                          {
                                              IdCategoria = categorias.IdCategoria,
                                              VidaUtil = categorias.VidaUtil,
                                              Categoria = categorias.Categoria,
                                              Descripcion = categorias.Descripcion
                                          }).ToList();
                    return listaCategorias;
                }
            }
        }

    }
}