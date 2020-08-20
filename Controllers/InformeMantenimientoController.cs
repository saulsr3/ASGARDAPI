using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ASGARDAPI.Models;
using ASGARDAPI.Clases;
using System.Threading;

namespace ASGARDAPI.Controllers
{
    public class InformeMantenimientoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        //METODO PARA GUARDAR EL INFORME DE LOS BIENES EN MANTENIMIENTO.

        [HttpPost]
        [Route("api/InformeMantenimiento/guardarInformeMantenimiento")]
        public int guardarInformeMantenimiento([FromBody]InformeMatenimientoAF oInformeMantenimientoAF)
        {
            int respuesta = 0;

            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    InformeMantenimiento oInformeMantenimiento = new InformeMantenimiento();
                    oInformeMantenimiento.IdInformeMantenimiento = oInformeMantenimientoAF.idinformematenimiento;
                    oInformeMantenimiento.IdMantenimiento = oInformeMantenimientoAF.idmantenimiento;
                    oInformeMantenimiento.IdTecnico = oInformeMantenimientoAF.idtecnico;
                    oInformeMantenimiento.Fecha = oInformeMantenimientoAF.fechainforme;
                    oInformeMantenimiento.Descripcion = oInformeMantenimientoAF.descripcion;
                    oInformeMantenimiento.CostoMateriales = oInformeMantenimientoAF.costomateriales;
                    oInformeMantenimiento.CostoMo = oInformeMantenimientoAF.costomo;
                    oInformeMantenimiento.CostoTotal = oInformeMantenimientoAF.costototal;
                    bd.InformeMantenimiento.Add(oInformeMantenimiento);
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

    }
}