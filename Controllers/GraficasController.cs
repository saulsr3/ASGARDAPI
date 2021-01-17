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
                                                        orderby periodo.Anio ascending
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
                                                        orderby periodo.Anio ascending
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
        [HttpGet]
        [Route("api/Graficas/GastosMttoPorAnio")]
        public List<ActivosXAnioAF> GastosMttoPorAnio()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                List<ActivosXAnioAF> activos = new List<ActivosXAnioAF>();
                IEnumerable<PeriodoAF> listaPeriodos = (from periodo in bd.Periodo
                                                        orderby periodo.Anio ascending
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
                    IEnumerable<MontoXAnioAF> lista = (from matto in bd.InformeMantenimiento
                                                           //join activo in bd.ActivoFijo
                                                           //on formulario.NoFormulario equals activo.NoFormulario

                                                       where (matto.Fecha >= DateTime.Parse(fechaMin) && matto.Fecha <= uDate)
                                                       select new MontoXAnioAF
                                                       {
                                                           idBien = matto.IdInformeMantenimiento,
                                                           monto = (double)matto.CostoTotal


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
        [HttpGet]
        [Route("api/Graficas/CargosDescargosRegistradosXAnio")]
        public List<ActivosXAnioAF> CargosDescargosRegistradosXAnio(string anio)
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                List<ActivosXAnioAF> activos = new List<ActivosXAnioAF>();
                IEnumerable<PeriodoAF> listaPeriodos = (from periodo in bd.Periodo
                                                        orderby periodo.Anio ascending
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
                    IEnumerable<ComboAnidadoAF> lista2 = (from solicitud in bd.SolicitudBaja
                                                         //join  in bd.ActivoFijo
                                                         //on formulario.NoFormulario equals activo.NoFormulario

                                                         where (solicitud.Fechabaja >= DateTime.Parse(fechaMin) && solicitud.Fechabaja <= uDate)
                                                         select new ComboAnidadoAF
                                                         {
                                                             id = solicitud.IdSolicitud

                                                         }).ToList();

                    int contador = 0;
                    int contadorbaja = 0;

                    ActivosXAnioAF activoAgregar = new ActivosXAnioAF();
                    foreach (var res1 in lista)
                    {
                        contador++;
                    }
                    foreach (var res2 in lista2)
                    {
                        contadorbaja++;
                    }
                    activoAgregar.anio = res.anio.ToString();
                    activoAgregar.numero = contador;
                    activoAgregar.descargos = contadorbaja;
                    activos.Add(activoAgregar);
                }
                return activos;
            }
        }
        [HttpGet]
        [Route("api/Graficas/ProvisionAnual")]
        public List<ActivosXAnioAF> ProvisionAnual(string anio)
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                List<ActivosXAnioAF> activos = new List<ActivosXAnioAF>();
                IEnumerable<PeriodoAF> listaPeriodos = (from periodo in bd.Periodo
                                                        orderby periodo.Anio ascending
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
                    List<ActivoRevalorizadoAF> lista = (from activo in bd.ActivoFijo
                                                        join noFormulario in bd.FormularioIngreso
                                                        on activo.NoFormulario equals noFormulario.NoFormulario
                                                        join tarjeta in bd.TarjetaDepreciacion
                                                        on activo.IdBien equals tarjeta.IdBien
                                                        where (tarjeta.Fecha >= DateTime.Parse(fechaMin) && tarjeta.Fecha <= uDate)
                                                        && tarjeta.Concepto == "Depreciación"
                                                        orderby activo.IdBien
                                                        select new ActivoRevalorizadoAF
                                                        {
                                                            idBien = activo.IdBien,

                                                            codigo = activo.CorrelativoBien,

                                                            fecha = tarjeta.Fecha == null ? " " : ((DateTime)tarjeta.Fecha).ToString("dd-MM-yyyy"),

                                                            concepto = tarjeta.Concepto,

                                                            valorAdquirido = activo.ValorAdquicicion.ToString(),

                                                            montoTransaccion = Math.Round((double)tarjeta.Valor, 2),

                                                            depreAnual = Math.Round((double)tarjeta.DepreciacionAnual, 2),

                                                            valorActual = Math.Round((double)tarjeta.ValorActual, 2)

                                                        }).ToList();

                    double valorTotal = 0;
                 
                    ActivosXAnioAF activoAgregar = new ActivosXAnioAF();
                    foreach (var res1 in lista)
                    {
                        valorTotal = valorTotal + res1.depreAnual;
                    }
                    activoAgregar.anio = res.anio.ToString();
                    activoAgregar.monto = valorTotal;
                    activos.Add(activoAgregar);
                }
                return activos;
            }
        }
    }
}
