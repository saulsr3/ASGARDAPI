using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ASGARDAPI.Models;
using ASGARDAPI.Clases;

namespace ASGARDAPI.Controllers
{
    public class AreasNegociosController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        [Route("api/AreasNegocios/listarAreas")]
        public IEnumerable<AreasDeNegocioAF> listarAreas() {
            using (BDAcaassAFContext bd = new BDAcaassAFContext()) {
                IEnumerable<AreasDeNegocioAF> lista = (from area in bd.AreaDeNegocio
                                                       join sucursal in bd.Sucursal
                                                        on area.IdSucursal equals sucursal.IdSucursal
                                                       where area.Dhabilitado == 1 orderby sucursal.Ubicacion
                                                       select new AreasDeNegocioAF
                                                       {
                                                           IdAreaNegocio=area.IdAreaNegocio,
                                                           Nombre=area.Nombre,
                                                           Correlativo=area.Correlativo,
                                                           nombreSucursal=sucursal.Nombre,
                                                           ubicacion=sucursal.Ubicacion
                                                       }).ToList();
                return lista;
            }
        }
        [HttpGet]
        [Route("api/AreasNegocios/comboSucursal")]
        public IEnumerable<SucursalAF> comboSucursal() {
            using (BDAcaassAFContext bd = new BDAcaassAFContext()) {
                IEnumerable<SucursalAF> lista = (from sucursal in bd.Sucursal
                                                 where sucursal.Dhabilitado == 1 orderby sucursal.Ubicacion
                                                 select new SucursalAF
                                                 {
                                                     IdSucursal=sucursal.IdSucursal,
                                                     Nombre=sucursal.Nombre,
                                                     Ubicacion=sucursal.Ubicacion
                                                 
                                                 }).ToList();
                return lista;
            }
        }
        [HttpPost]
        [Route("api/AreasNegocios/agregarSucursal")]
        public int aregarMarca([FromBody]AreasDeNegocioAF oAreaAF) {
            int res = 0;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext()) {
                    AreaDeNegocio oArea = new AreaDeNegocio();
                    oArea.IdAreaNegocio = oAreaAF.IdAreaNegocio;
                    oArea.Nombre = oAreaAF.Nombre;
                    oArea.IdSucursal = oAreaAF.IdSucursal;
                    oArea.Correlativo = oAreaAF.Correlativo;
                    oArea.Dhabilitado = 1;
                    bd.AreaDeNegocio.Add(oArea);
                    bd.SaveChanges();
                    res = 1;
                }

            }
            catch (Exception)
            {

                res = 0;
            }
            return res;
        }
        [HttpGet]
        [Route("api/AreasNegocios/eliminarArea/{idArea}")]
        public int eliminarArea(int idArea) {
            int res;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    AreaDeNegocio oArea = bd.AreaDeNegocio.Where(p => p.IdAreaNegocio == idArea).First();
                    oArea.Dhabilitado = 0;
                    bd.SaveChanges();
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
        [Route("api/AreasNegocio/buscarArea/{buscador?}")]
        public IEnumerable<AreasDeNegocioAF> buscarMarca(string buscador = "")
        {
            List<AreasDeNegocioAF> listaAreas;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                if (buscador == "")
                {
                    listaAreas = (from area in bd.AreaDeNegocio
                                  join sucursal in bd.Sucursal
                                   on area.IdSucursal equals sucursal.IdSucursal
                                  where area.Dhabilitado == 1
                                  orderby sucursal.Ubicacion
                                  select new AreasDeNegocioAF
                                  {
                                      IdAreaNegocio = area.IdAreaNegocio,
                                      Nombre = area.Nombre,
                                      Correlativo = area.Correlativo,
                                      nombreSucursal = sucursal.Nombre,
                                      ubicacion = sucursal.Ubicacion
                                  }).ToList();
                    return listaAreas;
                }
                else
                {
                    listaAreas = (from area in bd.AreaDeNegocio
                                  join sucursal in bd.Sucursal
                                   on area.IdSucursal equals sucursal.IdSucursal
                                  where area.Dhabilitado == 1
                                  && ((area.IdAreaNegocio).ToString().Contains(buscador) || (area.Nombre).ToLower().Contains(buscador.ToLower()) || (area.Correlativo).ToLower().Contains(buscador.ToLower())||(sucursal.Nombre).ToLower().Contains(buscador.ToLower())||(sucursal.Ubicacion).ToLower().Contains(buscador.ToLower()))
                                  orderby sucursal.Ubicacion
                                  select new AreasDeNegocioAF
                                  {
                                      IdAreaNegocio = area.IdAreaNegocio,
                                      Nombre = area.Nombre,
                                      Correlativo = area.Correlativo,
                                      nombreSucursal = sucursal.Nombre,
                                      ubicacion = sucursal.Ubicacion
                                  }).ToList();
                    return listaAreas;
                }
            }
        }
        [HttpGet]
        [Route("api/AreasNegocios/RecuperarAreaNegocio/{id}")]
        public AreasDeNegocioAF RecuperarAreaNegocio(int id)
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                AreasDeNegocioAF oAreaAF = new AreasDeNegocioAF();
                AreaDeNegocio oArea = bd.AreaDeNegocio.Where(p => p.IdAreaNegocio == id).First();
                oAreaAF.IdAreaNegocio = oArea.IdAreaNegocio;
                oAreaAF.Nombre = oArea.Nombre;
                oAreaAF.IdSucursal =(int) oArea.IdSucursal;
                oAreaAF.Correlativo = oArea.Correlativo;
                return oAreaAF;
            }
        }
        [HttpPost]
        [Route("api/AreasNegocios/modificarArea")]
        public int modificarArea([FromBody]AreasDeNegocioAF oAreaAF)
        {
            int rpta = 0;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    AreaDeNegocio oArea = bd.AreaDeNegocio.Where(p => p.IdAreaNegocio == oAreaAF.IdAreaNegocio).First();
                    oArea.IdAreaNegocio = oAreaAF.IdAreaNegocio;
                    oArea.Nombre = oAreaAF.Nombre;
                    oArea.IdSucursal = oAreaAF.IdSucursal;
                    oArea.Correlativo = oAreaAF.Correlativo;
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