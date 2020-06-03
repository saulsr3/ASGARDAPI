﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ASGARDAPI.Models;
using ASGARDAPI.Clases;

namespace ASGARDAPI.Controllers
{
    public class SolicitudMantenimientoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        [Route("api/SolicitudMantenimiento/listarSolicitudMante")]
        public IEnumerable<SolicitudMantenimientoAF> listarSolicitudMante()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<SolicitudMantenimientoAF> lista = (from solicitud in bd.SolicitudMantenimiento
                                                               join bienmante in bd.BienMantenimiento
                                                               on solicitud.IdSolicitud equals bienmante.IdSolicitud
                                                               join activo in bd.ActivoFijo
                                                               on bienmante.IdBien equals activo.IdBien
                                                               //join empleado in bd.Empleado
                                                               //on activo.IdResponsable equals empleado.IdEmpleado
                                                              // join areanegocio in bd.AreaDeNegocio
                                                              //on empleado.IdAreaDeNegocio equals areanegocio.IdAreaNegocio
                                                              // join sucursal in bd.Sucursal
                                                               //on areanegocio.IdSucursal equals sucursal.IdSucursal
                                                              where solicitud.Estado == 1 

                                                             //  orderby solicitud.Folio

                                                               select new SolicitudMantenimientoAF
                                                               {
                                                                   idsolicitud = solicitud.IdSolicitud,
                                                                   
                                                                   folio = solicitud.Folio,
                                                                   fechacadena = solicitud.Fecha == null ? " " : ((DateTime)solicitud.Fecha).ToString("dd-MM-yyyy"),
                                                                   idmantenimiento = bienmante.IdMantenimiento,                     
                                                                   razonesmantenimiento = bienmante.RazonMantenimiento,
                                                                   periodomantenimiento = bienmante.PeriodoMantenimiento,
                                                                   //idresponsable = (int)activo.IdResponsable
                                                                   idbien = (int)bienmante.IdBien,
                                                                   descripcionbien = activo.Desripcion,
                                                                   codigobien = activo.CorrelativoBien
                                                                 //  nombrecompleto = empleado.Nombres + " " + empleado.Apellidos
                                                                  // idareadenegocio = areanegocio.IdAreaNegocio,
                                                                   //areadenegocio = areanegocio.Nombre,
                                                                  // idsucursal = sucursal.IdSucursal,
                                                                   //sucursal = sucursal.Nombre



                                                               }).ToList();
                return lista;

            }

        }



        [HttpPost]
        [Route("api/SolicitudMantenimiento/guardarSolicitud")]
        public int guardarSolicitud ([FromBody] SolicitudMantenimientoAF oSolicitudAF)
        {
            int respuesta = 0;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    SolicitudMantenimiento oSolicitud = new SolicitudMantenimiento();
                    oSolicitud.IdSolicitud = oSolicitudAF.idsolicitud;
                    oSolicitud.Fecha = oSolicitudAF.fechasolicitud;
                    oSolicitud.Folio = oSolicitudAF.folio;
                  
                    AreaDeNegocio oArea = new AreaDeNegocio();
                    oArea.IdAreaNegocio = oSolicitudAF.idareadenegocio;
                    Sucursal oSucursal = new Sucursal();
                    oSucursal.IdSucursal = oSolicitudAF.idsucursal;
                    Empleado oEmpleado = new Empleado();
                    oEmpleado.IdEmpleado = oSolicitudAF.idresponsable;

                    oSolicitud.Estado = 1;
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
        [Route("api/SolicitudMantenimiento/listarEmpleadosCombo")]
        public IEnumerable<EmpleadoAF> listarEmpleadosCombo()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {

                IEnumerable<EmpleadoAF> lista = (from empleado in bd.Empleado
                                                 where empleado.Dhabilitado == 1
                                                 select new EmpleadoAF
                                                 {
                                                     idempleado = empleado.IdEmpleado,
                                                     nombres = empleado.Nombres,
                                                     apellidos = empleado.Apellidos
                                                 }).ToList();
                return lista;
            }
        }
        [HttpGet]
        [Route("api/SolicitudMantenimiento/listarAreaCombo")]
        public IEnumerable<AreasDeNegocioAF> listarAreaCombo()
        {
            using (BDAcaassAFContext bd= new BDAcaassAFContext())
            {
                IEnumerable<AreasDeNegocioAF> lista = (from area in bd.AreaDeNegocio
                                                       join sucursal in bd.Sucursal
                                                       on area.IdSucursal equals sucursal.IdSucursal
                                                       where area.Dhabilitado == 1
                                                       select new AreasDeNegocioAF
                                                       {
                                                           IdAreaNegocio = area.IdAreaNegocio,
                                                           Nombre = area.Nombre,
                                                           nombreSucursal = sucursal.Nombre
                                                       }).ToList();
                return lista;
            }

        }
    }
}