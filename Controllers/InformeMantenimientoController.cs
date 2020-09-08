﻿using System;
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
                    oInformeMantenimiento.CostoTotal = oInformeMantenimientoAF.costomateriales + oInformeMantenimientoAF.costomo;
                    bd.InformeMantenimiento.Add(oInformeMantenimiento);
                    oInformeMantenimiento.Estado = 1;
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

        //guardar revalorización 

            [HttpPost]
           [Route("api/InformeMantenimiento/insertarRevalorizacion")]
            public int  insertarRevalorizacion([FromBody] DatosDepreciacionAF oActivoAF)
        {
            int respuesta = 0;
            try
            {
                using (BDAcaassAFContext bd= new BDAcaassAFContext())
                {
                    TarjetaDepreciacion transaccion = new TarjetaDepreciacion();
                    TarjetaDepreciacion oUltimaTransaccion = bd.TarjetaDepreciacion.Where(p => p.IdBien == oActivoAF.idBien).Last();

                    transaccion.IdBien = oActivoAF.idBien;
                    transaccion.Fecha = oActivoAF.fecha;
                    transaccion.ValorMejora =oActivoAF.valorRevalorizacion;
                    transaccion.Concepto = "Revalorización";
                    transaccion.Valor = oUltimaTransaccion.Valor + oActivoAF.valorRevalorizacion;
                    transaccion.DepreciacionAnual = 0.00;
                    transaccion.DepreciacionAcumulada = 0.00;
                

                    // al valor actual tambien se le va a sumar el valor de la revalorización.
                    transaccion.ValorActual = oUltimaTransaccion.ValorActual + oActivoAF.valorRevalorizacion;
                   
                    bd.TarjetaDepreciacion.Add(transaccion);
                    bd.SaveChanges();

                    //para modificar la vida util en caso que el administrador quiera hacerlo.

                    ActivoFijo activo = bd.ActivoFijo.Where(p => p.IdBien == oActivoAF.idBien).First();
                    activo.VidaUtil = oActivoAF.vidaUtil;
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

        //metodo para buscar informes.
        [HttpGet]
        [Route("api/InformeMantenimiento/buscarInformes/{buscador?}")]
        public IEnumerable<InformeMatenimientoAF> buscarInformes(string buscador = "")
        {
            List<InformeMatenimientoAF> listarInformeMantenimiento;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                if (buscador == "")
                {
                    listarInformeMantenimiento = (from informemante in bd.InformeMantenimiento
                                                  join tecnico in bd.Tecnicos
                                           on informemante.IdInformeMantenimiento equals tecnico.IdTecnico
                                                  join bienmante in bd.BienMantenimiento
                                                  on informemante.IdMantenimiento equals bienmante.IdMantenimiento
                                                  join bienes in bd.ActivoFijo
                                                  on bienmante.IdBien equals bienes.IdBien
                                                  where informemante.Estado == 1


                                                  select new InformeMatenimientoAF
                                                  {
                                                      fechacadena = informemante.Fecha.ToString(),

                                                      //fechacadena = informemante.Fecha == null ? " " : ((DateTime)informemante.Fecha).ToString("dd-MM-yyyy"),
                                                      nombretecnico = tecnico.Nombre,
                                                      descripcion = informemante.Descripcion,
                                                      costomateriales = (double)informemante.CostoMateriales,
                                                      costomo = (double)informemante.CostoMo,
                                                      costototal = (double)informemante.CostoTotal,
                                                      bienes = bienes.Desripcion

                                                  }).ToList();
                    return listarInformeMantenimiento;
                }
                else
                {
                    listarInformeMantenimiento = (from informemante in bd.InformeMantenimiento
                                                  join tecnico in bd.Tecnicos
                                           on informemante.IdInformeMantenimiento equals tecnico.IdTecnico
                                                  join bienmante in bd.BienMantenimiento
                                                  on informemante.IdMantenimiento equals bienmante.IdMantenimiento
                                                  join bienes in bd.ActivoFijo
                                                  on bienmante.IdBien equals bienes.IdBien
                                                  where informemante.Estado == 1

                                      && ((informemante.Fecha).ToString().Contains(buscador.ToLower()) ||
                                     (tecnico.Nombre).ToLower().Contains(buscador.ToLower()) ||
                                     (bienes.Desripcion).ToString().ToLower().Contains(buscador.ToLower()) ||
                                     (informemante.CostoMateriales).ToString().Contains(buscador.ToLower()) ||
                                     (informemante.CostoMo).ToString().Contains(buscador.ToLower()) ||
                                     (informemante.CostoTotal).ToString().Contains(buscador.ToLower()) ||
                                     (informemante.Descripcion).ToString().Contains(buscador.ToLower()))

                                                  select new InformeMatenimientoAF
                                                  {
                                                      
                                                      fechacadena = informemante.Fecha.ToString(),
                                                     // fechacadena = informemante.Fecha == null ? " " : ((DateTime)informemante.Fecha).ToString("dd-MM-yyyy"),
                                                      nombretecnico = tecnico.Nombre,
                                                      descripcion = informemante.Descripcion,
                                                      costomateriales = (double)informemante.CostoMateriales,
                                                      costomo = (double)informemante.CostoMo,
                                                      costototal = (double)informemante.CostoTotal,
                                                      bienes = bienes.Desripcion
                                                  }).ToList();
                    return listarInformeMantenimiento;
                }
            }
        }




        //cambiar el estado de del informe para que desaparezca depues de que se aplicar la revalorización
        //quedara pendiente
        [HttpGet]
        [Route("api/InformeMantenimiento/cambiarEstadoDenegado/{idBien}")]
        public int cambiarEstadobien(int idBien)
        {
            int respuesta = 0;

            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    ActivoFijo oActivo = bd.ActivoFijo.Where(p => p.IdBien == idBien).First();
                    oActivo.EstadoActual = 1;
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





        public int transaccionDepreciacion([FromBody] DatosDepreciacionAF oActivoAF)
        {
            int rpta = 0;

            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    //Transaccion a tarjeta

                    TarjetaDepreciacion transaccion = new TarjetaDepreciacion();
                    TarjetaDepreciacion oUltimaTransaccion = bd.TarjetaDepreciacion.Where(p => p.IdBien == oActivoAF.idBien).Last();

                    //ActivoFijo oActivoFijoTransaccion = bd.ActivoFijo.Last();
                    transaccion.IdBien = oActivoAF.idBien;
                    transaccion.Fecha = oActivoAF.fecha;
                    transaccion.Concepto = "Depreciación";
                    transaccion.Valor = oUltimaTransaccion.Valor;
                    transaccion.DepreciacionAnual = oActivoAF.valorDepreciacion;
                    double valorAcumulado = (double)oUltimaTransaccion.DepreciacionAcumulada + oActivoAF.valorDepreciacion; ;
                    transaccion.DepreciacionAcumulada = Math.Round(valorAcumulado, 2);
                    double valor = (double)oUltimaTransaccion.ValorActual - oActivoAF.valorDepreciacion;
                    double rounded = Math.Round(valor, 2);
                    transaccion.ValorActual = rounded;
                    transaccion.ValorMejora = 0.00;
                    bd.TarjetaDepreciacion.Add(transaccion);
                    bd.SaveChanges();
                    //cambia ultimo anio de depreciación

                    ActivoFijo activo = bd.ActivoFijo.Where(p => p.IdBien == oActivoAF.idBien).First();
                    Periodo anioActual = bd.Periodo.Where(p => p.Estado == 1).FirstOrDefault();
                    activo.UltimoAnioDepreciacion = anioActual.Anio;
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



        //LISTAR INFORMES DE MANTENIMIENTO (PARA DAR REVALORIZACIÓN)
        [HttpGet]
        [Route("api/InformeMantenimiento/ListarInformeMantenimiento")]
        public IEnumerable<InformeMatenimientoAF> listarInformeMantenimiento()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<InformeMatenimientoAF> listaInformeMante= (from informemante in bd.InformeMantenimiento
                                                                       join tecnico in bd.Tecnicos
                                                                on informemante.IdInformeMantenimiento equals tecnico.IdTecnico
                                                                join bienmante in bd.BienMantenimiento
                                                                on informemante.IdMantenimiento equals bienmante.IdMantenimiento
                                                                join bienes in bd.ActivoFijo 
                                                                on bienmante.IdBien equals bienes.IdBien

                                                                       //where empleado.Dhabilitado == 1
                                                                       select new InformeMatenimientoAF
                                                         {
                                                             idinformematenimiento = informemante.IdInformeMantenimiento,
                                                             idmantenimiento = (int)informemante.IdMantenimiento,
                                                             fechacadena = informemante.Fecha == null ? " " : ((DateTime)informemante.Fecha).ToString("dd-MM-yyyy"),
                                                             nombretecnico = tecnico.Nombre,
                                                             descripcion=informemante.Descripcion,
                                                             costomateriales= (double)informemante.CostoMateriales,
                                                             costomo= (double)informemante.CostoMo,
                                                             costototal= (double)informemante.CostoTotal,
                                                             bienes = bienes.Desripcion
                                                             
                                                             
                                                             

                                                         }).ToList();
                return listaInformeMante;
            }
        }

    }
}