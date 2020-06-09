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
    public class SolicitudMantenimientoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("api/SolicitudMantenimiento/listarBienes")]
        public IEnumerable<SolicitudMantenimientoAF> listarBienes()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<SolicitudMantenimientoAF> lista = (from activo in bd.ActivoFijo
                                                               join bienmante in bd.BienMantenimiento
                                                             

                                                               select new SolicitudMantenimientoAF
                                                               {
                                                                  
                                                                   codigobien = activo.CorrelativoBien,
                                                                   descripcionbien = activo.Desripcion,
                                                                   idbien = (int)bienmante.IdBien,
                                                                   idmantenimiento = bienmante.IdMantenimiento,
                                                                   razonesmantenimiento = bienmante.RazonMantenimiento,
                                                                   periodomantenimiento = bienmante.PeriodoMantenimiento,

                                                               }).ToList();
                return lista;
            }
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
                    bd.SolicitudMantenimiento.Add(oSolicitud);

                    //AreaDeNegocio oArea = new AreaDeNegocio();
                    //oArea.IdAreaNegocio = oSolicitudAF.idareadenegocio;
                    ////  bd.AreaDeNegocio.Add(oArea);
                    //Sucursal oSucursal = new Sucursal();
                    //oSucursal.IdSucursal = oSolicitudAF.idsucursal;
                    //// bd.Sucursal.Add(oSucursal);
                    //Empleado oEmpleado = new Empleado();
                    //oEmpleado.IdEmpleado = oSolicitudAF.idresponsable;
                    //bd.Empleado.Add(oEmpleado);

                    //estos son los datos de la tabla

                   //ActivoFijo oActivo = new ActivoFijo();
                   // oActivo.IdBien = oSolicitudAF.idbien;
                   // oActivo.CorrelativoBien = oSolicitudAF.codigobien;
                   // oActivo.Desripcion = oSolicitudAF.descripcionbien;
                   // bd.ActivoFijo.Add(oActivo);
                    BienMantenimiento oBienMantenimiento = new BienMantenimiento();

                    oBienMantenimiento.IdMantenimiento = oSolicitudAF.idmantenimiento;
                  //  oBienMantenimiento.IdSolicitud = oSolicitudAF.idsolicitud;
                    oBienMantenimiento.RazonMantenimiento = oSolicitudAF.razonesmantenimiento;
                    oBienMantenimiento.PeriodoMantenimiento = oSolicitudAF.periodomantenimiento;
                    bd.BienMantenimiento.Add(oBienMantenimiento);

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
        [Route("api/SolicitudMantenimiento/listarCodigoCombo")]
        public IEnumerable<ActivoFijoAF> listarCodigoCombo()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {

                IEnumerable<ActivoFijoAF> lista = (from activofijo in bd.ActivoFijo
                                                 where activofijo.EstadoActual==1
                                                 select new ActivoFijoAF
                                                 {
                                                     IdBien=activofijo.IdBien,
                                                     Codigo=activofijo.CorrelativoBien,
                                                     Desripcion=activofijo.Desripcion
                                                    
                                                 }).ToList();
                return lista;
            }
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