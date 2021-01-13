using ASGARDAPI.Clases;
using ASGARDAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASGARDAPI.Controllers
{
    public class RevertirController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        //lista usuarios
        [HttpGet]
        [Route("api/Revertir/listarTransaccionesActivo/{anio}")]

        public IEnumerable<TarjetaTransaccionesAF> listarTransaccionesActivo( int anio)
        {
   
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                string fechaMin = "1-1-" + anio;
                string fechaMax = "31-12-" + anio;
                DateTime uDate = DateTime.ParseExact(fechaMax, "dd-MM-yyyy", null);
                List<TarjetaTransaccionesAF> lstaTransacciones = (from transac in bd.TarjetaDepreciacion
                                                                  where transac.Fecha >= DateTime.Parse(fechaMin) && transac.Fecha <= uDate
                                                select new TarjetaTransaccionesAF
                                                {
                                                    id= transac.IdTarjeta,
                                                    idBien = (int)transac.IdBien,
                                                    concepto = transac.Concepto,
                                                    fecha = transac.Fecha == null ? " " : ((DateTime)transac.Fecha).ToString("yyyy"),
                                                }).ToList();
                return lstaTransacciones;
            }
        }
        [HttpGet]
        [Route("api/Revertir/EliminarTransaccionesActivo/{id}")]

        public int EliminarTransaccionesActivo(int id)
        {
            int rpta = 0;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                try
                {
                    TarjetaDepreciacion oTarjeta = bd.TarjetaDepreciacion.Where(p => p.IdTarjeta == id).FirstOrDefault();
                    bd.TarjetaDepreciacion.Remove(oTarjeta);
                    bd.SaveChanges();
                    rpta = 1;
                }
                catch (Exception)
                {
                    rpta = 0;
                }
                return rpta;
               
            
            }
        }
        [HttpGet]
        [Route("api/Revertir/EliminarActivos/{id}")]

        public int EliminarActivos(int id)
        {
            int rpta = 0;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                try
                {
                    ActivoFijo oActivo = bd.ActivoFijo.Where(p => p.IdBien == id).FirstOrDefault();
                    bd.ActivoFijo.Remove(oActivo);
                    bd.SaveChanges();
                    rpta = 1;
                }
                catch (Exception)
                {
                    rpta = 0;
                }
                return rpta;


            }
        }
        [HttpGet]
        [Route("api/Depreciacion/Revertir/{anio}")]
        public int Revertir(int anio)
        {
            int rpta = 0;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                try {
                    Periodo oPeriodo = bd.Periodo.Where(p => p.Anio == anio).FirstOrDefault();
                    Periodo oPeriodoNuevo = bd.Periodo.Where(p => p.Anio == (anio - 1)).FirstOrDefault();
                    oPeriodo.Estado = 0;
                    oPeriodoNuevo.Estado = 1;
                    bd.SaveChanges();
                    rpta = 1;
                }
                catch (Exception)
                {
                    rpta = 0;
                }
            }
            return rpta;
        }
        [HttpGet]
        [Route("api/Revertir/listarTransaccionesReversion/{anio}")]

        public IEnumerable<TarjetaTransaccionesAF> listarTransaccionesReversion(int anio)
        {

            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                string fechaMin = "1-1-" + anio;
                string fechaMax = "31-12-" + anio;
                DateTime uDate = DateTime.ParseExact(fechaMax, "dd-MM-yyyy", null);
                List<TarjetaTransaccionesAF> lstaTransacciones = (from transac in bd.TarjetaDepreciacion
                                                                  where ((transac.Fecha >= DateTime.Parse(fechaMin) && transac.Fecha <= uDate) && transac.Concepto== "Depreciación")
                                                                  select new TarjetaTransaccionesAF
                                                                  {
                                                                      id = transac.IdTarjeta,
                                                                      idBien = (int)transac.IdBien,
                                                                      concepto = transac.Concepto,
                                                                      fecha = transac.Fecha == null ? " " : ((DateTime)transac.Fecha).ToString("yyyy"),
                                                                  }).ToList();
                return lstaTransacciones;
            }
        }
        [HttpGet]
        [Route("api/Revertir/EliminarTransaccionesRevertir/{id}")]

        public int EliminarTransaccionesRevertir(int id)
        {
            int rpta = 0;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                try
                {
                    Periodo oPeriodo = bd.Periodo.Where(p => p.Estado==1).FirstOrDefault();
                    TarjetaDepreciacion oTarjeta = bd.TarjetaDepreciacion.Where(p => p.IdTarjeta == id).FirstOrDefault();
                    ActivoFijo oActivo = bd.ActivoFijo.Where(p => p.IdBien == oTarjeta.IdBien).FirstOrDefault();
                    oActivo.UltimoAnioDepreciacion = oPeriodo.Anio-1;
                    bd.TarjetaDepreciacion.Remove(oTarjeta);
                    bd.SaveChanges();
                    rpta = 1;
                }
                catch (Exception)
                {
                    rpta = 0;
                }
                return rpta;


            }
        }
    }
}
