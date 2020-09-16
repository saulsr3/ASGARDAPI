using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ASGARDAPI.Clases;
using ASGARDAPI.Models;

namespace ASGARDAPI.Controllers
{
    public class TipoTraspasoController : Controller
    {
     
        public IActionResult Index()
        {
            return View();
        }

        //Método guardar tipo de traspaso
        [HttpPost]
        [Route("api/TipoTraspaso/guardarTraspaso")]
        public int guardarTraspaso([FromBody] TipoTraspasoAF oTipoTraspasoAF)
        {
            int rpta = 0;

            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    TipoTraspaso oTipoTraspaso = new TipoTraspaso();
                    oTipoTraspaso.IdTipo = oTipoTraspasoAF.idtipo;
                    oTipoTraspaso.Nombre = oTipoTraspasoAF.nombre;
                    oTipoTraspaso.Descripcion = oTipoTraspasoAF.descripcion;
                    oTipoTraspaso.Dhabilitado = 1;
                    bd.TipoTraspaso.Add(oTipoTraspaso);
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

        //Método listar tipo de traspaso
        [HttpGet]
        [Route("api/TipoTraspaso/listarTipoTraspaso")]
        public IEnumerable<TipoTraspasoAF> listarTipoTraspaso()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<TipoTraspasoAF> listaTipoTraspaso = (from tipotraspaso in bd.TipoTraspaso
                                                                 where tipotraspaso.Dhabilitado==1
                                                                 select new TipoTraspasoAF
                                                                 {
                                                                     idtipo=tipotraspaso.IdTipo,
                                                                     nombre=tipotraspaso.Nombre,
                                                                     descripcion=tipotraspaso.Descripcion

                                                                 }).ToList();
                return listaTipoTraspaso;
            }
        }

        //Método recuperar tipo de traspaso
        [HttpGet]
        [Route("api/TipoTraspaso/recuperarTipoTraspaso{id}")]
        public TipoTraspasoAF recuperarTipoTraspaso(int id)
        {

            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                TipoTraspasoAF oTipoTraspasoAF = new TipoTraspasoAF();
                TipoTraspaso oTipoTraspaso = bd.TipoTraspaso.Where(p => p.IdTipo == id).First();
                oTipoTraspasoAF.idtipo = oTipoTraspaso.IdTipo;
                oTipoTraspasoAF.nombre = oTipoTraspaso.Nombre;
                oTipoTraspasoAF.descripcion = oTipoTraspaso.Descripcion;

                return oTipoTraspasoAF;
            }

        }

        //Método modificar tipo de traspaso
        [HttpPost]
        [Route("api/TipoTraspaso/modificarTipoTraspaso")]
        public int modificarTipoTraspaso([FromBody] TipoTraspasoAF oTipoTraspasoAF)
        {
            int rpta = 0;

            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    TipoTraspaso oTipoTraspaso= bd.TipoTraspaso.Where(p => p.IdTipo == oTipoTraspasoAF.idtipo).First();
                    oTipoTraspaso.IdTipo = oTipoTraspasoAF.idtipo;
                    oTipoTraspaso.Nombre = oTipoTraspasoAF.nombre;
                    oTipoTraspaso.Descripcion = oTipoTraspasoAF.descripcion;
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
