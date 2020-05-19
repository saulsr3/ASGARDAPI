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
                                                       where area.Dhabilitado == 1
                                                       select new AreasDeNegocioAF
                                                       {
                                                           IdAreaNegocio=area.IdAreaNegocio,
                                                           Nombre=area.Nombre,
                                                           Correlativo=area.Correlativo,
                                                           nombreSucursal=sucursal.Nombre
                                                       }).ToList();
                return lista;
            }
        }
        [HttpGet]
        [Route("api/AreasNegocios/comboSucursal")]
        public IEnumerable<SucursalAF> comboSucursal() {
            using (BDAcaassAFContext bd = new BDAcaassAFContext()) {
                IEnumerable<SucursalAF> lista = (from sucursal in bd.Sucursal
                                                 where sucursal.Dhabilitado == 1
                                                 select new SucursalAF
                                                 {
                                                     IdSucursal=sucursal.IdSucursal,
                                                     Nombre=sucursal.Nombre
                                                 
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
    }
}