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
                                                                   //join bienmante in bd.BienMantenimiento
                                                                   //on activo.IdBien equals bienmante.IdBien
                                                               where activo.EstaAsignado == 1 && activo.EstadoActual == 1 && activo.EstadoActual != 2

                                                               select new SolicitudMantenimientoAF
                                                               {
                                                                   idbien = activo.IdBien,
                                                                   codigobien = activo.CorrelativoBien,
                                                                   descripcionbien = activo.Desripcion
                                                                   // idbien = (int)bienmante.IdBien,
                                                                   //idmantenimiento = bienmante.IdMantenimiento,
                                                                   //razonesmantenimiento = bienmante.RazonMantenimiento,
                                                                   //periodomantenimiento = bienmante.PeriodoMantenimiento,

                                                               }).ToList();
                return lista;
            }
        }

        [HttpGet]
        [Route("api/SolicitudMantenimiento/listarSolicitudMante")]
        public IEnumerable<SolicitudMantenimientoPAF> listarSolicitudMante()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())

            {

                IEnumerable<SolicitudMantenimientoPAF> lista = (from solicitud in bd.SolicitudMantenimiento
                                                                    //join bienMtoto in bd.BienMantenimiento
                                                                    //on solicitud.IdSolicitud equals bienMtoto.IdSolicitud
                                                                    //join bien in bd.ActivoFijo
                                                                    //on bienMtoto.IdBien equals bien.IdBien
                                                                    //join empleado in bd.Empleado
                                                                    //on bien.IdResponsable equals empleado.IdEmpleado
                                                                    //join areaNegocio in bd.AreaDeNegocio
                                                                    //on empleado.IdAreaDeNegocio equals areaNegocio.IdAreaNegocio
                                                                    //join sucursal in bd.Sucursal
                                                                    //on areaNegocio.IdSucursal equals sucursal.IdSucursal

                                                                where solicitud.Estado == 1

                                                                select new SolicitudMantenimientoPAF
                                                                {
                                                                    idsolicitud = solicitud.IdSolicitud,
                                                                    folio = solicitud.Folio,
                                                                    descripcion = solicitud.Descripcion,
                                                                    fechacadena = solicitud.Fecha == null ? " " : ((DateTime)solicitud.Fecha).ToString("dd-MM-yyyy")

                                                                    //areadenegocio = areaNegocio.Nombre,
                                                                    //sucursal=sucursal.Nombre,
                                                                    //solicitante=empleado.Nombres

                                                                }).ToList();


                return lista;

            }

        }




        [HttpPost]
        [Route("api/SolicitudMantenimiento/guardarSolicitud")]
        public int guardarSolicitud([FromBody] SolicitudMantenimientoAF oSolicitudAF)
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
                    oSolicitud.Descripcion = oSolicitudAF.descripcion;
                    oSolicitud.Estado = 1;
                    bd.SolicitudMantenimiento.Add(oSolicitud);
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


        //METODO PARA GUARDAR EL INFORME DE LOS BIENES EN MANTENIMIENTO.

        [HttpPost]
        [Route("api/SolicitudMantenimiento/guardarInformeMantenimiento")]
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



        [HttpPost]
        [Route("api/SolicitudMantenimiento/guardarBienes")]
        public int guardarBienes([FromBody]ArrayMantenimientoAF oArray)
        {
            int respuesta = 0;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {

                    BienMantenimiento bienMtto = new BienMantenimiento();
                    SolicitudMantenimiento idSolicitud = bd.SolicitudMantenimiento.Where(p => p.Estado == 1).Last();
                    bienMtto.IdMantenimiento = oArray.idMantenimiento;
                    bienMtto.IdSolicitud = idSolicitud.IdSolicitud;
                    bienMtto.IdBien = oArray.idBien;
                    ActivoFijo oActivo = bd.ActivoFijo.Where(p => p.IdBien == oArray.idBien).First();
                    oActivo.EstadoActual = 2;
                    bienMtto.RazonMantenimiento = oArray.razonesMantenimiento;
                    bienMtto.PeriodoMantenimiento = oArray.periodoMantenimiento;
                    bd.BienMantenimiento.Add(bienMtto);
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
                                                   where activofijo.EstadoActual == 1
                                                   select new ActivoFijoAF
                                                   {
                                                       IdBien = activofijo.IdBien,
                                                       Codigo = activofijo.CorrelativoBien,
                                                       Desripcion = activofijo.Desripcion

                                                   }).ToList();
                return lista;
            }
        }


  
    
        [HttpGet]
        [Route("api/SolicitudMantenimiento/listaBienesSolicitados/{idSolicitud}")]
        public IEnumerable<BienesSolicitadosMttoAF> listaBienesSolicitados(int idSolicitud)
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<BienesSolicitadosMttoAF> lista = (from bienMtto in bd.BienMantenimiento
                                                              join activo in bd.ActivoFijo
                                                              on bienMtto.IdBien equals activo.IdBien
                                                              where bienMtto.IdSolicitud == idSolicitud
                                                              select new BienesSolicitadosMttoAF
                                                              {
                                                                  idBien=activo.IdBien,
                                                                  estadoActual= (int) activo.EstadoActual,
                                                                  Codigo = activo.CorrelativoBien,
                                                                  Descripcion = activo.Desripcion,
                                                                  Periodo = bienMtto.PeriodoMantenimiento,
                                                                  Razon = bienMtto.RazonMantenimiento

                                                              }).ToList();


                return lista;
            }

        }

        //listar bienes en manteniento
        [HttpGet]
        [Route("api/SolicitudMantenimiento/listarBienesMantenimiento")]
        public IEnumerable<BienesSolicitadosMttoAF> listarBienesMantenimiento()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<BienesSolicitadosMttoAF> lista = (from bienMtto in bd.BienMantenimiento
                                                              join activo in bd.ActivoFijo
                                                              on bienMtto.IdBien equals activo.IdBien
                                                              where activo.EstadoActual == 3
                                                              select new BienesSolicitadosMttoAF
                                                              {
                                                                  idBien = activo.IdBien,
                                                                  idmantenimiento = bienMtto.IdMantenimiento,
                                                                  estadoActual = (int)activo.EstadoActual,
                                                                  Codigo = activo.CorrelativoBien,
                                                                  Descripcion = activo.Desripcion,
                                                                  Periodo = bienMtto.PeriodoMantenimiento,
                                                                  Razon = bienMtto.RazonMantenimiento

                                                              }).ToList();


                return lista;
            }

        }



        //metodos para acetptar solicitud y cambiar estado actual
        [HttpGet]
        [Route("api/SolicitudMantenimiento/aceptarSolicitud/{idSoli}")]
        public int aceptarSolicitud(int idSoli)
        {
            int respuesta = 0;

            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    SolicitudMantenimiento oSolicitud = bd.SolicitudMantenimiento.Where(p => p.IdSolicitud == idSoli).First();
                    oSolicitud.Estado = 2;
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
        [Route("api/SolicitudMantenimiento/denegarSolicitud/{idSoli}")]
        public int denegarSolicitud(int idSoli)
        {
            int respuesta = 0;

            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    SolicitudMantenimiento oSolicitud = bd.SolicitudMantenimiento.Where(p => p.IdSolicitud == idSoli).First();
                    oSolicitud.Estado = 0;
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
        [Route("api/SolicitudMantenimiento/cambiarEstado/{idBien}")]
        public int cambiarEstado(int idBien)
        {
            int respuesta = 0;

            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    ActivoFijo oActivo = bd.ActivoFijo.Where(p => p.IdBien == idBien).First();
                    oActivo.EstadoActual = 3;
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
        [Route("api/SolicitudMantenimiento/cambiarEstadoDenegado/{idBien}")]
        public int cambiarEstadoDenegado(int idBien)
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



        [HttpGet]
        [Route("api/SolicitudMantenimiento/DatosSolicitud/{idSolicitud}")]
        public BienesSolicitadosMttoAF DatosSolicitud(int idSolicitud)
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                BienesSolicitadosMttoAF odatos = new BienesSolicitadosMttoAF();
                SolicitudMantenimiento osolicitud = bd.SolicitudMantenimiento.Where(p => p.IdSolicitud == idSolicitud).First();

                odatos.NoSolicitud = "00" + osolicitud.IdSolicitud.ToString();
                odatos.fechacadena = osolicitud.Fecha == null ? " " : ((DateTime)osolicitud.Fecha).ToString("dd-MM-yyyy");
                BienMantenimiento obienMtto=bd.BienMantenimiento.Where(p => p.IdSolicitud == osolicitud.IdSolicitud).First();
                ActivoFijo obien= bd.ActivoFijo.Where(p => p.IdBien == obienMtto.IdBien).First();
                Empleado oempleado= bd.Empleado.Where(p => p.IdEmpleado == obien.IdResponsable).First();
                AreaDeNegocio oArea= bd.AreaDeNegocio.Where(p => p.IdAreaNegocio == oempleado.IdAreaDeNegocio).First();
                odatos.jefe = oempleado.Nombres+" "+oempleado.Apellidos;
                odatos.areanegocio = oArea.Nombre;
                return odatos;

            }
        }


        [HttpGet]
        [Route("api/SolicitudMantenimiento/listarTecnicoCombo")]
        public IEnumerable<TecnicoAF> listarTecnicosCombo()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<TecnicoAF> lista = (from tecnico in bd.Tecnicos                                                                                           
                                                     where tecnico.Dhabilitado == 1
                                                       select new TecnicoAF
                                                       {
                                                           idtecnico=tecnico.IdTecnico,
                                                           nombre=tecnico.Nombre                                                
                                                       }).ToList();
                return lista;
            }

        }
    }
}