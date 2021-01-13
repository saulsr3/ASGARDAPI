﻿using Microsoft.AspNetCore.Mvc;
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
        [HttpGet]
        [Route("api/Graficas/ListarPeriodos")]
        public IEnumerable<PeriodoAF> ListarPeriodos()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {

                IEnumerable<PeriodoAF> lista = (from periodo in bd.Periodo
                                                    orderby periodo.IdPeriodo
                                                     select new PeriodoAF
                                                     {
                                                         idperiodo = periodo.IdPeriodo,
                                                         anio=(int)periodo.Anio
                                                     }).ToList();
                return lista;
            }
        }
        [HttpGet]
        [Route("api/Graficas/ActivosRegistradosXAnio/{anio}")]
        public ActivosXAnioAF listarMarcas(string anio)
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                string fechaMin = "1-1-" + anio;
                string fechaMax = "31-12-" + anio;
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
                ActivosXAnioAF activos = new ActivosXAnioAF();
                foreach (var res in lista)
                {
                   
                    contador++;

                }
                activos.anio = anio;
                activos.numero = contador;
                return activos;
            }
        }
        [HttpGet]
        [Route("api/Graficas/montoPorAnio/{anio}")]
        public ActivosXAnioAF montoPorAnio(string anio)
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                string fechaMin = "1-1-" + anio;
                string fechaMax = "31-12-" + anio;
                DateTime uDate = DateTime.ParseExact(fechaMax, "dd-MM-yyyy", null);
                IEnumerable<MontoXAnioAF> lista = (from formulario in bd.FormularioIngreso
                                                     join activo in bd.ActivoFijo
                                                     on formulario.NoFormulario equals activo.NoFormulario

                                                     where (formulario.FechaIngreso >= DateTime.Parse(fechaMin) && formulario.FechaIngreso <= uDate)
                                                     select new MontoXAnioAF
                                                     {
                                                         idBien = activo.IdBien,
                                                         monto=(double)activo.ValorAdquicicion
                                                         
                                                     }).ToList();
                double monto = 0;
                ActivosXAnioAF activos = new ActivosXAnioAF();
                foreach (var res in lista)
                {

                    monto+=res.monto;

                }
                activos.anio = anio;
                activos.monto = monto;
                return activos;
            }
        }
    }
}