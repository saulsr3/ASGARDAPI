using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ASGARDAPI.Models;
using ASGARDAPI.Clases;

namespace ASGARDAPI.Controllers
{
    public class SolicitudBajaController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("api/SolicitudBaja/listarBienes")]
        public IEnumerable<ActivoFijoAF> listarBienes()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<ActivoFijoAF> lista = (from activo in bd.ActivoFijo
                                                      join resposable in bd.Empleado
                                                      on activo.IdResponsable equals resposable.IdEmpleado
                                                      join area in bd.AreaDeNegocio
                                                      on resposable.IdAreaDeNegocio equals area.IdAreaNegocio
                                                      join cargo in bd.Cargos
                                                      on resposable.IdCargo equals cargo.IdCargo
                                                      where activo.EstadoActual == 1 && activo.EstaAsignado == 1
                                                      orderby activo.CorrelativoBien
                                                      select new ActivoFijoAF
                                                      {
                                                          IdBien = activo.IdBien,
                                                          Codigo = activo.CorrelativoBien,
                                                          Desripcion = activo.Desripcion,
                                                          AreaDeNegocio = area.Nombre,
                                                          Resposnsable = resposable.Nombres + " " + resposable.Apellidos,
                                                          cargo = cargo.Cargo,

                                                      }).ToList();
                return lista;

            }
        }

        [HttpPost]
        [Route("api/SolicitudBaja/guardarSolicitud")]
        public int guardarSolicitud([FromBody]SolicitudBajaAF oSolicitudAF)
        {
            int rpta = 0;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                  
                    SolicitudBaja oSolicitud = new SolicitudBaja();
                    oSolicitud.IdSolicitud = oSolicitudAF.idsolicitud;
                    oSolicitud.Fecha = oSolicitudAF.fechasolicitud;
                    oSolicitud.Folio = oSolicitudAF.folio;
                    oSolicitud.Observaciones = oSolicitudAF.observaciones;
                    oSolicitud.Motivo = oSolicitudAF.motivo;
                    if (oSolicitudAF.motivo == 4)
                    {
                        oSolicitud.EntidadBeneficiaria = oSolicitudAF.entidadbeneficiaria;
                        oSolicitud.Domicilio = oSolicitudAF.domicilio;
                        oSolicitud.Contacto = oSolicitudAF.contacto;
                        oSolicitud.Telefono = oSolicitudAF.telefono;
                    }
                    
                    oSolicitud.Estado = 1;
                    bd.SolicitudBaja.Add(oSolicitud);
                    bd.SaveChanges();
                    rpta = 1;
                }
            }
            catch (Exception ex)
            {
                rpta = 0;
                Console.WriteLine(rpta);
            }
            return rpta;
        }


        [HttpGet]
        [Route("api/SolicitudBaja/listarSolicitud")]
        public IEnumerable<SolicitudBajaAF> listarSolicitud()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<SolicitudBajaAF> lista = (from activo in bd.ActivoFijo
                                                   join solicitud in bd.SolicitudBaja
                                                   on activo.IdBien equals solicitud.IdBien
                                                   where solicitud.Estado == 1 
                                                   select new SolicitudBajaAF
                                                   {
                                                       idbien = activo.IdBien,
                                                       idsolicitud = solicitud.IdSolicitud,
                                                       folio = solicitud.Folio,
                                                       fechacadena = solicitud.Fecha == null ? " " : ((DateTime)solicitud.Fecha).ToString("dd-MM-yyyy"),
                                                       observaciones = solicitud.Observaciones,
                                                       motivo = (int) solicitud.Motivo,
                                                       entidadbeneficiaria = solicitud.EntidadBeneficiaria,
                                                       domicilio = solicitud.Domicilio,
                                                       contacto = solicitud.Contacto,
                                                       telefono = solicitud.Telefono,

                                                   }).ToList();
                return lista;

            }
        }

        //metodo para aceptar la solicitud de baja
        [HttpGet]
        [Route("api/SolicitudBaja/aceptarSolicitud/{idsolicitud}")]
        public int aceptarSolicitud(int idsolicitud)
        {
            int rpta = 0;

            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    SolicitudBaja oSolicitud = bd.SolicitudBaja.Where(p => p.IdSolicitud == idsolicitud).First();
                    oSolicitud.Estado = 2;
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

        //metodo para rechazar la solicitud
        [HttpGet]
        [Route("api/SolicitudBaja/denegarSolicitud/{idsolicitud}")]
        public int denegarSolicitud(int idsolicitud)
        {
            int rpta = 0;

            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    SolicitudBaja oSolicitud = bd.SolicitudBaja.Where(p => p.IdSolicitud == idsolicitud).First();
                    oSolicitud.Estado = 0;
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

        //si la solicitud es aceptada cambiamos el estado del bien a 0
        [HttpGet]
        [Route("api/SolicitudBaja/cambiarEstadoAceptado/{idbien}")]
        public int cambiarEstado(int idbien)
        {
            int rpta = 0;

            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    ActivoFijo oActivo = bd.ActivoFijo.Where(p => p.IdBien == idbien).First();
                    oActivo.EstadoActual = 0;
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

        //si la solicitud es rechazada vuelve al estado normal que es 1
        [HttpGet]
        [Route("api/SolicitudBaja/cambiarEstadoRechazado/{idbien}")]
        public int cambiarEstadoDenegado(int idbien)
        {
            int rpta = 0;

            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    ActivoFijo oActivo = bd.ActivoFijo.Where(p => p.IdBien == idbien).First();
                    oActivo.EstadoActual = 1;
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


        [HttpGet]
        [Route("api/SolicitudBaja/verSolicitud/{id}")]
        public JsonResult verSolicitud(int id)
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                dynamic oActivo = new Newtonsoft.Json.Linq.JObject();
                //Extraer los datos padres de la base
                SolicitudBaja osolicitud = bd.SolicitudBaja.Where(p => p.IdSolicitud == id).First();
                //Utilizar los datos padres para extraer los datos
                ActivoFijo oActivoFijo = bd.ActivoFijo.Where(p => p.IdBien == osolicitud.IdBien).First();
                Empleado oEmpleado = bd.Empleado.Where(p => p.IdEmpleado == oActivoFijo.IdResponsable).First();
                AreaDeNegocio oarea = bd.AreaDeNegocio.Where(p => p.IdAreaNegocio == oEmpleado.IdAreaDeNegocio).First();
                Sucursal osucursal = bd.Sucursal.Where(p => p.IdSucursal == oarea.IdSucursal).First();
                Cargos ocargos = bd.Cargos.Where(p => p.IdCargo == oEmpleado.IdCargo).First();
                Clasificacion oclasi = bd.Clasificacion.Where(p => p.IdClasificacion == oActivoFijo.IdClasificacion).First();
                Marcas omarca = bd.Marcas.Where(p => p.IdMarca == oActivoFijo.IdMarca).First();
                //llenado
                oActivo.IdBien =(int) osolicitud.IdBien;
                oActivo.fechacadena = osolicitud.Fecha == null ? " " : ((DateTime)osolicitud.Fecha).ToString("dd-MM-yyyy");
                oActivo.Resposnsable = oEmpleado.Nombres + "" + oEmpleado.Apellidos;
                oActivo.AreaDeNegocio = oarea.Nombre;
                oActivo.cargo = ocargos.Cargo;

                oActivo.Codigo = oActivoFijo.CorrelativoBien;
                oActivo.Desripcion = oActivoFijo.Desripcion;
                oActivo.Clasificacion = oclasi.Clasificacion1;
                oActivo.Marca = omarca.Marca;
                oActivo.Modelo = oActivoFijo.Modelo;
                oActivo.Color = oActivoFijo.Color;
                oActivo.destinoinicial = oActivoFijo.DestinoInicial;
                oActivo.ubicacion = osucursal.Ubicacion;

                oActivo.observaciones = osolicitud.Observaciones;
                oActivo.motivo = osolicitud.Motivo;
                oActivo.entidad = osolicitud.EntidadBeneficiaria;
                oActivo.domicilio = osolicitud.Domicilio;
                oActivo.contacto = osolicitud.Contacto;
                oActivo.telefono = osolicitud.Telefono;
                oActivo.folio = osolicitud.Folio;


                return Json(oActivo);
            }
        }

    }
}