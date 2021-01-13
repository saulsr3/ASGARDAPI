using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASGARDAPI.Clases;
using ASGARDAPI.Models;

namespace ASGARDAPI.Controllers
{
    public class GraficasController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        //[HttpGet]
        //[Route("api/Graficas/ListarPeriodos")]
        //public IEnumerable<PeriodoAF> ListarPeriodos()
        //{
        //    using (BDAcaassAFContext bd = new BDAcaassAFContext())
        //    {

        //        IEnumerable<PeriodoAF> lista = (from periodo in bd.Periodo
        //                                            orderby periodo.IdPeriodo
        //                                             select new PeriodoAF
        //                                             {
        //                                                 idperiodo = periodo.IdPeriodo,
        //                                                 anio=(int)periodo.Anio
        //                                             }).ToList();
        //        return lista;
        //    }
        //}
        [HttpGet]
        [Route("api/Graficas/ActivosRegistradosXAnio")]
        public List<ActivosXAnioAF> ActivosRegistradosXAnio(string anio)
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                List<ActivosXAnioAF> activos = new List<ActivosXAnioAF>();
                IEnumerable<PeriodoAF> listaPeriodos = (from periodo in bd.Periodo
                                                orderby periodo.IdPeriodo
                                                select new PeriodoAF
                                                {
                                                    idperiodo = periodo.IdPeriodo,
                                                    anio = (int)periodo.Anio
                                                }).ToList();
                foreach (var res in listaPeriodos)
                {
                    string fechaMin = "1-1-" + res.anio;
                    string fechaMax = "31-12-" + res.anio;
                    DateTime uDate = DateTime.ParseExact(fechaMax, "dd-MM-yyyy", null);
                    IEnumerable<ComboAnidadoAF> lista = (from formulario in bd.FormularioIngreso
                                                         join activo in bd.ActivoFijo
                                                         on formulario.NoFormulario equals activo.NoFormulario

                                                         where (formulario.FechaIngreso >= DateTime.Parse(fechaMin) && formulario.FechaIngreso <= uDate)
                                                         select new ComboAnidadoAF
                                                         {
                                                             id = activo.IdBien
                                                             
                                                         }).ToList();
                    int contador = 0;
            
                    ActivosXAnioAF activoAgregar = new ActivosXAnioAF();
                    foreach (var res1 in lista)
                    {
                        contador++;
                    }
                    activoAgregar.anio = res.anio.ToString();
                    activoAgregar.numero = contador;
                    activos.Add(activoAgregar);
                }
                return activos;
            }
        }
        [HttpGet]
        [Route("api/Graficas/montoPorAnio")]
        public List<ActivosXAnioAF> montoPorAnio()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                List<ActivosXAnioAF> activos = new List<ActivosXAnioAF>();
                IEnumerable<PeriodoAF> listaPeriodos = (from periodo in bd.Periodo
                                                        orderby periodo.IdPeriodo
                                                        select new PeriodoAF
                                                        {
                                                            idperiodo = periodo.IdPeriodo,
                                                            anio = (int)periodo.Anio
                                                        }).ToList();
                foreach (var res in listaPeriodos)
                {
                    string fechaMin = "1-1-" + res.anio;
                    string fechaMax = "31-12-" + res.anio;
                    DateTime uDate = DateTime.ParseExact(fechaMax, "dd-MM-yyyy", null);
                    IEnumerable<MontoXAnioAF> lista = (from formulario in bd.FormularioIngreso
                                                       join activo in bd.ActivoFijo
                                                       on formulario.NoFormulario equals activo.NoFormulario

                                                       where (formulario.FechaIngreso >= DateTime.Parse(fechaMin) && formulario.FechaIngreso <= uDate)
                                                       select new MontoXAnioAF
                                                       {
                                                           idBien = activo.IdBien,
                                                           monto = (double)activo.ValorAdquicicion

                                                       }).ToList();
                    double monto = 0;
                    ActivosXAnioAF activoAgregar = new ActivosXAnioAF();
                    foreach (var res1 in lista)
                    {

                        monto += res1.monto;

                    }
                    activoAgregar.anio = res.anio.ToString();
                    activoAgregar.monto = monto;
                    activos.Add(activoAgregar);
                }
                
                return activos;
            }
        }
    }
}
