﻿using System;
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
        [Route("api/SolicitudBaja/listarBienes")]  //lista los bienes no asignados
        public List<ActivoFijoAF> listarBienes()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                List<ActivoFijoAF> lista = (from activo in bd.ActivoFijo
                                             join noFormulario in bd.FormularioIngreso
                                             on activo.NoFormulario equals noFormulario.NoFormulario
                                             join clasif in bd.Clasificacion
                                             on activo.IdClasificacion equals clasif.IdClasificacion
                                             where activo.EstadoActual == 1 && activo.EstaAsignado == 0
                                            
                                             select new ActivoFijoAF
                                             {
                                                 IdBien = activo.IdBien,
                                                 NoFormulario = noFormulario.NoFormulario,
                                                 fechacadena = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                                 Desripcion = activo.Desripcion,
                                                 Clasificacion = clasif.Clasificacion1

                                             }).ToList();

                return lista;

            }
        }

        [HttpGet]
        [Route("api/SolicitudBaja/listarBienesAsignados")]
        public List<ActivoFijoAF> listarBienesAsignados()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                List<ActivoFijoAF> lista = (from activo in bd.ActivoFijo
                                            join noFormulario in bd.FormularioIngreso
                                             on activo.NoFormulario equals noFormulario.NoFormulario
                                            join resposable in bd.Empleado
                                            on activo.IdResponsable equals resposable.IdEmpleado
                                            join area in bd.AreaDeNegocio
                                            on resposable.IdAreaDeNegocio equals area.IdAreaNegocio
                                            join cargo in bd.Cargos
                                            on resposable.IdCargo equals cargo.IdCargo
                                            where activo.EstadoActual == 1 && activo.EstaAsignado ==1
                                            orderby activo.CorrelativoBien
                                            select new ActivoFijoAF
                                            {
                                                IdBien = activo.IdBien,
                                                Codigo = activo.CorrelativoBien,
                                                fechacadena = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                                Desripcion = activo.Desripcion,
                                                AreaDeNegocio = area.Nombre,
                                                Resposnsable = resposable.Nombres + " " + resposable.Apellidos,
                                                cargo = cargo.Cargo,

                                            }).ToList();
                return lista;

            }
        }

        [HttpPost]
        [Route("api/SolicitudBaja/guardarSolicitudBaja")]
        public int guardarSolicitud([FromBody]SolicitudBajaAF oSolicitudAF)
        {
            int rpta = 0;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                  
                    SolicitudBaja oSolicitud = new SolicitudBaja();
                    oSolicitud.IdSolicitud = oSolicitudAF.idsolicitud;
                    oSolicitud.IdBien = oSolicitudAF.idbien;
                    oSolicitud.IdTipoDescargo = oSolicitudAF.idtipodescargo;
                    oSolicitud.Fecha = oSolicitudAF.fechasolicitud;
                    oSolicitud.Folio = oSolicitudAF.folio;
                    oSolicitud.Observaciones = oSolicitudAF.observaciones;
                    //oSolicitud.IdTipoDescargo = oSolicitudAF.motivo;
                    oSolicitud.EntidadBeneficiaria = oSolicitudAF.entidadbeneficiaria;
                    oSolicitud.Domicilio = oSolicitudAF.domicilio;
                    oSolicitud.Contacto = oSolicitudAF.contacto;
                    oSolicitud.Telefono = oSolicitudAF.telefono;
                    oSolicitud.Estado = 1;
                   
                    bd.SolicitudBaja.Add(oSolicitud);
                    bd.SaveChanges();
                    rpta = 1;
                }
            }
            catch (Exception ex)
            {
                rpta = 0;
               // Console.WriteLine(ex);
            }
            return rpta;
        }



        [HttpGet]
        [Route("api/SolicitudBaja/listarSolicitudBaja")]
        public IEnumerable<SolicitudBajaAF> listarSolicitud()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<SolicitudBajaAF> lista = (from activo in bd.ActivoFijo
                                                   join solicitud in bd.SolicitudBaja
                                                   on activo.IdBien equals solicitud.IdBien
                                                   join descargo in bd.TipoDescargo
                                                   on solicitud.IdTipoDescargo equals descargo.IdTipo
                                                   where solicitud.Estado == 1 
                                                   select new SolicitudBajaAF
                                                   {
                                                       idbien = activo.IdBien,
                                                       idsolicitud = solicitud.IdSolicitud,
                                                       folio = solicitud.Folio,
                                                       fechacadena = solicitud.Fecha == null ? " " : ((DateTime)solicitud.Fecha).ToString("dd-MM-yyyy"),
                                                       observaciones = solicitud.Observaciones,
                                                       nombredescargo =  descargo.Nombre,
                                                       entidadbeneficiaria = solicitud.EntidadBeneficiaria,
                                                       domicilio = solicitud.Domicilio,
                                                       contacto = solicitud.Contacto,
                                                       telefono = solicitud.Telefono,

                                                   }).ToList();
                return lista;

            }
        }

        [HttpPost]
        [Route("api/SolicitudBaja/guardarBienesBaja")]
        public int guardarBienes([FromBody]ActivoFijoAF oBaja)
        {
            int respuesta = 0;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    ActivoFijo bien = new ActivoFijo();
                    SolicitudBaja idSolicitud = bd.SolicitudBaja.Where(p => p.Estado == 1).First();
                    
                    Console.WriteLine("IDBIEN" + oBaja.IdBien);
                    ActivoFijo oActivo = bd.ActivoFijo.Where(p => p.IdBien == oBaja.IdBien).First();
                    oActivo.EstadoActual = 4;
                   // bd.ActivoFijo.Update(bien);
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

        //metodo para aceptar la solicitud de baja
        [HttpGet]
        [Route("api/SolicitudBaja/aceptarSolicitudBaja/{idsolicitud}")] ///{idbien}
        public int aceptarSolicitud(int idsolicitud)//, int idbien)
        {
            int rpta = 0;

            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    SolicitudBaja oSolicitud = bd.SolicitudBaja.Where(p => p.IdSolicitud == idsolicitud).First();
                    //ActivoFijo oActivo = bd.ActivoFijo.Where(p => p.IdBien == idbien).First();
                    //oActivo.EstadoActual = 0;
                    oSolicitud.Estado = 2;
                   // oSolicitud.Acuerdo = ""; 
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
        [Route("api/SolicitudBaja/denegarSolicitudBaja/{idsolicitud}")]
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
        [Route("api/SolicitudBaja/cambiarEstadoAceptado/{idbien}/{acuerdo}/{fecha2}")] // 
        public int cambiarEstado(int idbien, string acuerdo, string fecha2)// 
        {
            int rpta = 0;

            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                   // Console.WriteLine("IDESTADO" + idbien);
                    SolicitudBaja oSolic = bd.SolicitudBaja.Where(p => p.IdSolicitud == idbien).First();
                    ActivoFijo oActivo = bd.ActivoFijo.Where(p => p.IdBien == oSolic.IdBien).First();
                    oActivo.EstadoActual = 0;
                    oActivo.EstaAsignado = 0;
                   
                   oSolic.Acuerdo = acuerdo;
                   oSolic.Fechabaja = Convert.ToDateTime(fecha2);
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
        [Route("api/SolicitudBaja/cambiarEstadoDenegado/{idbien}")] //  
        public int cambiarEstadoDenegado(int idbien)//
        {
            int rpta = 0;

            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    SolicitudBaja oSolic = bd.SolicitudBaja.Where(p => p.IdSolicitud == idbien).First();
                    ActivoFijo oActivo = bd.ActivoFijo.Where(p => p.IdBien == oSolic.IdBien).First();
                    oActivo.EstadoActual = 1;
                    
                   // oSolic.Acuerdo = acuerdo;
                   // oSolic.Fechabaja = Convert.ToDateTime(fecha2);
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
        [Route("api/SolicitudBaja/verSolicitudBaja/{idSolicitud}")]
        public SolicitadosABajaAF verSolicitud(int idSolicitud)
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                SolicitadosABajaAF odatos = new SolicitadosABajaAF();
                SolicitudBaja osolicitud = bd.SolicitudBaja.Where(p => p.IdSolicitud == idSolicitud).First();

                ActivoFijo obien = bd.ActivoFijo.Where(p => p.IdBien == osolicitud.IdBien).First();
                
                odatos.NoSolicitud =  osolicitud.IdSolicitud;
                odatos.fechacadena = osolicitud.Fecha == null ? " " : ((DateTime)osolicitud.Fecha).ToString("dd-MM-yyyy");
                
                TipoDescargo odescargo = bd.TipoDescargo.Where(p => p.IdTipo == osolicitud.IdTipoDescargo).First();
                //odatos.motivo = osolicitud.;
                odatos.nombredescargo = odescargo.Nombre; 
                odatos.folio = osolicitud.Folio;
                odatos.idbien = (int) osolicitud.IdBien;
                odatos.Codigo = obien.CorrelativoBien;
                odatos.Descripcion = obien.Desripcion;
                odatos.observaciones = osolicitud.Observaciones;

                return odatos;
            }
        }

        [HttpGet]
        [Route("api/SolicitudBaja/verDescargos/{id}")]
        public SolicitadosABajaAF verDescargos(int id)
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                SolicitadosABajaAF odatos = new SolicitadosABajaAF();

                ActivoFijo obien = bd.ActivoFijo.Where(p => p.IdBien == id).First();
                Clasificacion oclasi = bd.Clasificacion.Where(p => p.IdClasificacion == obien.IdClasificacion).First();
                FormularioIngreso oformu = bd.FormularioIngreso.Where(p => p.NoFormulario == obien.NoFormulario).First();
                Empleado oemple = (obien.IdResponsable != null) ? bd.Empleado.Where(p => p.IdEmpleado == obien.IdResponsable).First() : null;
                SolicitudBaja osolicitud = bd.SolicitudBaja.Where(p => p.IdBien == obien.IdBien).Last();
                TipoDescargo odescargo = bd.TipoDescargo.Where(p => p.IdTipo == osolicitud.IdTipoDescargo).First();
                Marcas omarca = (obien.IdMarca != null) ? bd.Marcas.Where(p => p.IdMarca == obien.IdMarca).First() : null;
                Proveedor oprov = (obien.IdProveedor != null) ? bd.Proveedor.Where(p => p.IdProveedor == obien.IdProveedor).First() : null;
                Donantes odona = (obien.IdDonante != null) ? bd.Donantes.Where(p => p.IdDonante == obien.IdDonante).First() : null;
                if (omarca == null)
                {
                    odatos.marca = "";
                }
                else
                {
                    odatos.marca = omarca.Marca;
                }
                odatos.NoSolicitud = osolicitud.IdSolicitud;
                odatos.fechacadena = oformu.FechaIngreso == null ? " " : ((DateTime)oformu.FechaIngreso).ToString("dd-MM-yyyy");
                odatos.folio = osolicitud.Folio;
                odatos.idbien = (int)osolicitud.IdBien;
                odatos.Codigo = obien.CorrelativoBien;
                odatos.Descripcion = obien.Desripcion;
                odatos.observaciones = osolicitud.Observaciones;
                /////////////////////////////////////////////////
                odatos.acuerdo = osolicitud.Acuerdo;
                odatos.Codigo = obien.CorrelativoBien;
                odatos.responsable = (oemple == null) ? "" : oemple.Nombres + " " + oemple.Apellidos;
                odatos.idproveedor = (oprov != null) ? oprov.IdProveedor : odona.IdDonante;
                odatos.valor = (double)obien.ValorAdquicicion;
                odatos.nombredescargo = odescargo.Nombre;
                odatos.fechacadena2 = osolicitud.Fechabaja == null ? " " : ((DateTime)osolicitud.Fechabaja).ToString("dd-MM-yyyy");
                odatos.color = obien.Color;
                odatos.clasificacion = oclasi.Clasificacion1;
                return odatos;


            }
        }

        [HttpGet]
        [Route("api/SolicitudBaja/listarBajas")]
        public List<ActivoFijoAF> listarBajas()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                List<ActivoFijoAF> lista = (from activo in bd.ActivoFijo
                                            join formulario in bd.FormularioIngreso
                                            on activo.NoFormulario equals formulario.NoFormulario
                                            join clasi in bd.Clasificacion
                                            on activo.IdClasificacion equals clasi.IdClasificacion
                                            
                                            where activo.EstadoActual == 0
                                            //orderby activo.CorrelativoBien
                                            select new ActivoFijoAF
                                            {
                                                IdBien = activo.IdBien,
                                                Desripcion = activo.Desripcion,
                                                fechacadena = formulario.FechaIngreso == null ? " " : ((DateTime)formulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                                Clasificacion = clasi.Clasificacion1
                                            }).ToList();
                return lista;

            }
        }

        [HttpGet]
        [Route("api/SolicitudBaja/validarFolio/{idsolicitud}/{folio}")]
        public int validarFolio(int idsolicitud, string folio)
        {
            int respuesta = 0;
            try
            {


                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    if (idsolicitud == 0)
                    {
                        respuesta = bd.SolicitudBaja.Where(p => p.Folio.ToLower() == folio.ToLower()).Count();
                    }
                    else
                    {
                        respuesta = bd.SolicitudBaja.Where(p => p.Folio.ToLower() == folio.ToLower()).Count();
                    }


                }

            }
            catch (Exception ex)
            {
                respuesta = 0;

            }
            return respuesta;

        }

        [HttpGet]
        [Route("api/SolicitudBaja/validarAcuerdo/{idsolicitud}/{acuerdo}")]
        public int validarAcuerdo(int idsolicitud, string acuerdo)
        {
            int respuesta = 0;
            try
            {


                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    if (idsolicitud == 0)
                    {
                        respuesta = bd.SolicitudBaja.Where(p => p.Acuerdo.ToLower() == acuerdo.ToLower()).Count();
                    }
                    else
                    {
                        respuesta = bd.SolicitudBaja.Where(p => p.Acuerdo.ToLower() == acuerdo.ToLower()).Count();
                    }


                }

            }
            catch (Exception ex)
            {
                respuesta = 0;

            }
            return respuesta;

        }


        [HttpGet]
        [Route("api/SolicitudBaja/buscarBienesBajaAsig/{buscador?}")]
        public IEnumerable<ActivoFijoAF> buscarBienesBajaAsig(string buscador = "")
        {
            List<ActivoFijoAF> lista;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                if (buscador == "")
                {
                    lista = (from activo in bd.ActivoFijo
                             join noFormulario in bd.FormularioIngreso
                             on activo.NoFormulario equals noFormulario.NoFormulario
                             join resposable in bd.Empleado
                             on activo.IdResponsable equals resposable.IdEmpleado
                             join area in bd.AreaDeNegocio
                             on resposable.IdAreaDeNegocio equals area.IdAreaNegocio
                             where activo.EstadoActual == 1 && activo.EstaAsignado == 1
                             orderby activo.CorrelativoBien
                             select new ActivoFijoAF
                             {
                                 IdBien = activo.IdBien,
                                 Codigo = activo.CorrelativoBien,
                                 fechacadena = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),

                                 Desripcion = activo.Desripcion,
                                 AreaDeNegocio = area.Nombre,
                                 Resposnsable = resposable.Nombres + " " + resposable.Apellidos,
                                 

                             }).ToList();

                    return lista;
                }
                else
                {
                    lista = (from activo in bd.ActivoFijo
                             join noFormulario in bd.FormularioIngreso
                             on activo.NoFormulario equals noFormulario.NoFormulario
                             join resposable in bd.Empleado
                             on activo.IdResponsable equals resposable.IdEmpleado
                             join area in bd.AreaDeNegocio
                             on resposable.IdAreaDeNegocio equals area.IdAreaNegocio
                             join cargo in bd.Cargos
                             on resposable.IdCargo equals cargo.IdCargo
                             where activo.EstadoActual == 1 && activo.EstaAsignado == 1

                                 && ((activo.CorrelativoBien).ToLower().Contains(buscador.ToLower()) ||
                                    (activo.Desripcion).ToLower().Contains(buscador.ToLower()) ||
                                    (noFormulario.FechaIngreso).ToString().ToLower().Contains(buscador.ToLower()) ||
                                    (area.Nombre).ToString().ToLower().Contains(buscador.ToLower()) ||
                                    (resposable.Nombres).ToLower().Contains(buscador.ToLower()) ||
                                    (resposable.Apellidos).ToLower().Contains(buscador.ToLower()) 
                                    
                                    )

                             select new ActivoFijoAF
                             {
                                 IdBien = activo.IdBien,
                                 Codigo = activo.CorrelativoBien,
                                 Desripcion = activo.Desripcion,
                                 fechacadena = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),

                                 AreaDeNegocio = area.Nombre,
                                 Resposnsable = resposable.Nombres + " " + resposable.Apellidos,
                                 cargo = cargo.Cargo,

                             }).ToList();
                    return lista;
                }
            }
        }
                                                                                                                                                                                                                                                 

        [HttpGet]
        [Route("api/SolicitudBaja/buscarSolicitud/{buscador?}")]
        public IEnumerable<SolicitudBajaAF> buscarSolicitud(string buscador = "")
        {
            List<SolicitudBajaAF> lista;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                if (buscador == "")
                {
                    lista = (from activo in bd.ActivoFijo
                             join solicitud in bd.SolicitudBaja
                             on activo.IdBien equals solicitud.IdBien
                             join descargo in bd.TipoDescargo
                             on solicitud.IdTipoDescargo equals descargo.IdTipo
                             where solicitud.Estado == 1
                             select new SolicitudBajaAF
                             {
                                 idbien = activo.IdBien,
                                 idsolicitud = solicitud.IdSolicitud,
                                 folio = solicitud.Folio,
                                 fechacadena = solicitud.Fecha == null ? " " : ((DateTime)solicitud.Fecha).ToString("dd-MM-yyyy"),
                                 observaciones = solicitud.Observaciones,
                                 nombredescargo = descargo.Nombre ,
                                 

                             }).ToList();

                    return lista;
                }
                else
                {
                    lista = (from activo in bd.ActivoFijo
                             join solicitud in bd.SolicitudBaja
                             on activo.IdBien equals solicitud.IdBien
                             join descargo in bd.TipoDescargo
                             on solicitud.IdTipoDescargo equals descargo.IdTipo
                             where solicitud.Estado == 1

                                 && ((solicitud.Folio).ToLower().Contains(buscador.ToLower()) ||
                                    (solicitud.Fecha).ToString().ToLower().Contains(buscador.ToLower()) ||
                                    (descargo.Nombre).ToString().ToLower().Contains(buscador.ToLower()) ||
                                    (solicitud.Observaciones).ToLower().Contains(buscador.ToLower())
                                    
                                    )

                             select new SolicitudBajaAF
                             {
                                 idbien = activo.IdBien,
                                 idsolicitud = solicitud.IdSolicitud,
                                 folio = solicitud.Folio,
                                 fechacadena = solicitud.Fecha == null ? " " : ((DateTime)solicitud.Fecha).ToString("dd-MM-yyyy"),
                                 observaciones = solicitud.Observaciones,
                                 nombredescargo = descargo.Nombre,

                             }).ToList();
                    return lista;
                }
            }
        }

       
        [HttpGet]
        [Route("api/SolicitudBaja/buscarBaja/{buscador?}")]
        public IEnumerable<ActivoFijoAF> buscarBaja(string buscador = "")
        {
            List<ActivoFijoAF> lista;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                if (buscador == "")
                {
                    lista = (from activo in bd.ActivoFijo
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
                else
                {
                    lista = (from activo in bd.ActivoFijo
                             join resposable in bd.Empleado
                             on activo.IdResponsable equals resposable.IdEmpleado
                             join area in bd.AreaDeNegocio
                             on resposable.IdAreaDeNegocio equals area.IdAreaNegocio
                             join cargo in bd.Cargos
                             on resposable.IdCargo equals cargo.IdCargo
                             where activo.EstadoActual == 1 && activo.EstaAsignado == 1

                                 && ((activo.CorrelativoBien).ToLower().Contains(buscador.ToLower()) ||
                                    (activo.Desripcion).ToLower().Contains(buscador.ToLower()) ||
                                    (area.Nombre).ToString().ToLower().Contains(buscador.ToLower()) ||
                                    (resposable.Nombres).ToLower().Contains(buscador.ToLower()) ||
                                    (resposable.Apellidos).ToLower().Contains(buscador.ToLower()) ||
                                    (cargo.Cargo).ToLower().Contains(buscador.ToLower())
                                    )

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
        }

        

        [HttpGet]
        [Route("api/SolicitudBaja/buscarDescargos/{buscador?}")]
        public IEnumerable<ActivoFijoAF> buscarDescargos(string buscador = "")
        {
            List<ActivoFijoAF> lista;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                if (buscador == "")
                {
                    lista = (from activo in bd.ActivoFijo
                             join noFormulario in bd.FormularioIngreso
                             on activo.NoFormulario equals noFormulario.NoFormulario
                             join clasif in bd.Clasificacion
                             on activo.IdClasificacion equals clasif.IdClasificacion
                             where activo.EstadoActual == 0

                             select new ActivoFijoAF
                             {
                                 IdBien = activo.IdBien,
                                 NoFormulario = noFormulario.NoFormulario,
                                 fechacadena = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                 Desripcion = activo.Desripcion,
                                 Clasificacion = clasif.Clasificacion1

                             }).ToList();

                    return lista;
                }
                else
                {
                    lista = (from activo in bd.ActivoFijo
                             join noFormulario in bd.FormularioIngreso
                             on activo.NoFormulario equals noFormulario.NoFormulario
                             join clasif in bd.Clasificacion
                             on activo.IdClasificacion equals clasif.IdClasificacion
                             join soli in bd.SolicitudBaja
                             on activo.IdBien equals soli.IdBien
                             where activo.EstadoActual == 0

                                     && ((activo.IdBien).ToString().Contains(buscador)
                                      || (activo.CorrelativoBien).ToLower().Contains(buscador.ToLower())
                                      || (soli.Fecha).ToString().ToLower().Contains(buscador.ToLower())
                                      || (activo.Desripcion).ToLower().Contains(buscador.ToLower())
                                      || (clasif.Clasificacion1).ToLower().Contains(buscador.ToLower())

                                      )
                             select new ActivoFijoAF
                             {
                                 IdBien = activo.IdBien,
                                 NoFormulario = noFormulario.NoFormulario,
                                 fechacadena = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                 Desripcion = activo.Desripcion,
                                 Clasificacion = clasif.Clasificacion1

                             }).ToList();
                    return lista;
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //Listar combo sucursal
        [HttpGet]
        [Route("api/SolicitudBaja/comboSucursal")]
        public IEnumerable<SucursalAF> comboSucursal()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<SucursalAF> lista = (from sucursal in bd.Sucursal
                                                 where sucursal.Dhabilitado == 1
                                                 orderby sucursal.Ubicacion
                                                 select new SucursalAF
                                                 {
                                                     IdSucursal = sucursal.IdSucursal,
                                                     Nombre = sucursal.Nombre,
                                                     Ubicacion = sucursal.Ubicacion

                                                 }).ToList();
                return lista;
            }
        }

        //Listar área de negocio
        [HttpGet]
        [Route("api/SolicitudBaja/listarAreaCombo")]
        public IEnumerable<AreasDeNegocioAF> listarAreaCombo()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<AreasDeNegocioAF> listarAreas = (from sucursal in bd.Sucursal
                                                             join area in bd.AreaDeNegocio
                                                             on sucursal.IdSucursal equals area.IdSucursal
                                                             where area.Dhabilitado == 1
                                                             select new AreasDeNegocioAF
                                                             {
                                                                 IdAreaNegocio = area.IdAreaNegocio,
                                                                 Nombre = area.Nombre,
                                                                 IdSucursal = sucursal.IdSucursal,
                                                                 nombreSucursal = sucursal.Nombre,
                                                                 ubicacion = sucursal.Ubicacion


                                                             }).ToList();


                return listarAreas;

            }
        }

        //Activos para edificios e instalaciones
        [HttpGet]
        [Route("api/SolicitudBaja/listarActivosEdificios")]
        public List<RegistroAF> listarActivosEdificios()
        {

            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                List<RegistroAF> lista = (from activo in bd.ActivoFijo
                                          join noFormulario in bd.FormularioIngreso
                                          on activo.NoFormulario equals noFormulario.NoFormulario
                                          join clasif in bd.Clasificacion
                                          on activo.IdClasificacion equals clasif.IdClasificacion
                                          where (activo.EstadoActual == 1 || activo.EstadoActual == 2 || activo.EstadoActual == 3) && activo.TipoActivo == 1 && activo.EstadoActual == 1
                                          orderby activo.CorrelativoBien
                                          select new RegistroAF
                                          {
                                              IdBien = activo.IdBien,
                                              Codigo = activo.CorrelativoBien,
                                              fechacadena = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                              Descripcion = activo.Desripcion,
                                              Clasificacion = clasif.Clasificacion1,
                                          }).ToList();
                return lista;

            }
        }

        //Activos para intangibles
        [HttpGet]
        [Route("api/SolicitudBaja/listarActivosIntangibles")]
        public List<RegistroAF> listarActivosIntangibles()
        {

            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                List<RegistroAF> lista = (from activo in bd.ActivoFijo
                                          join noFormulario in bd.FormularioIngreso
                                          on activo.NoFormulario equals noFormulario.NoFormulario
                                          join clasif in bd.Clasificacion
                                          on activo.IdClasificacion equals clasif.IdClasificacion
                                          // Acá iría el área pero como está referenciada a empleado

                                          where (activo.EstadoActual == 1 || activo.EstadoActual == 2 || activo.EstadoActual == 3) && activo.TipoActivo == 3 && activo.EstadoActual == 1
                                          orderby activo.CorrelativoBien
                                          select new RegistroAF
                                          {
                                              IdBien = activo.IdBien,
                                              Codigo = activo.CorrelativoBien,
                                              fechacadena = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                              Descripcion = activo.Desripcion,
                                              Clasificacion = clasif.Clasificacion1,
                                          }).ToList();
                return lista;

            }
        }

        [HttpGet]
        [Route("api/SolicitudBaja/listarActivosFiltro/{id}")]
        public IEnumerable<RegistroAF> listarActivosFiltro(int id)
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<RegistroAF> lista = (from activo in bd.ActivoFijo
                                                 join noFormulario in bd.FormularioIngreso
                                                 on activo.NoFormulario equals noFormulario.NoFormulario
                                                 join clasif in bd.Clasificacion
                                                 on activo.IdClasificacion equals clasif.IdClasificacion
                                                 join resposable in bd.Empleado
                                                 on activo.IdResponsable equals resposable.IdEmpleado
                                                 join area in bd.AreaDeNegocio
                                                 on resposable.IdAreaDeNegocio equals area.IdAreaNegocio
                                                 where (activo.EstadoActual == 1 || activo.EstadoActual == 2 || activo.EstadoActual == 3) && activo.EstaAsignado == 1 && area.IdAreaNegocio == id
                                                 orderby activo.CorrelativoBien
                                                 select new RegistroAF
                                                 {
                                                     IdBien = activo.IdBien,
                                                     Codigo = activo.CorrelativoBien,
                                                     fechacadena = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                                     Descripcion = activo.Desripcion,
                                                     Clasificacion = clasif.Clasificacion1,
                                                     AreaDeNegocio = area.Nombre,
                                                     Responsable = resposable.Nombres + " " + resposable.Apellidos
                                                 }).ToList();
                return lista;

            }
        }

        //BUSCADORES DE ACTIVOS
        [HttpGet]
        [Route("api/SolicitudBaja/buscarActivoAsig/{buscador?}")]
        public IEnumerable<RegistroAF> buscarActivoAsig(string buscador = "")
        {
            List<RegistroAF> lista;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                if (buscador == "")
                {
                    lista = (from activo in bd.ActivoFijo
                             join noFormulario in bd.FormularioIngreso
                             on activo.NoFormulario equals noFormulario.NoFormulario
                             join resposable in bd.Empleado
                             on activo.IdResponsable equals resposable.IdEmpleado
                             join area in bd.AreaDeNegocio
                             on resposable.IdAreaDeNegocio equals area.IdAreaNegocio
                             where activo.EstadoActual == 1 && activo.EstaAsignado == 1
                             orderby activo.CorrelativoBien
                             select new RegistroAF
                             {
                                 IdBien = activo.IdBien,
                                 Codigo = activo.CorrelativoBien,
                                 fechacadena = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),

                                 Descripcion = activo.Desripcion,
                                 AreaDeNegocio = area.Nombre,
                                 Responsable = resposable.Nombres + " " + resposable.Apellidos,


                             }).ToList();

                    return lista;
                }
                else
                {
                    lista = (from activo in bd.ActivoFijo
                             join noFormulario in bd.FormularioIngreso
                             on activo.NoFormulario equals noFormulario.NoFormulario
                             join resposable in bd.Empleado
                             on activo.IdResponsable equals resposable.IdEmpleado
                             join area in bd.AreaDeNegocio
                             on resposable.IdAreaDeNegocio equals area.IdAreaNegocio
                             join cargo in bd.Cargos
                             on resposable.IdCargo equals cargo.IdCargo
                             where activo.EstadoActual == 1 && activo.EstaAsignado == 1

                                 && ((activo.CorrelativoBien).ToLower().Contains(buscador.ToLower()) ||
                                    (activo.Desripcion).ToLower().Contains(buscador.ToLower()) ||
                                    (noFormulario.FechaIngreso).ToString().ToLower().Contains(buscador.ToLower()) ||
                                    (area.Nombre).ToString().ToLower().Contains(buscador.ToLower()) ||
                                    (resposable.Nombres).ToLower().Contains(buscador.ToLower()) ||
                                    (resposable.Apellidos).ToLower().Contains(buscador.ToLower())

                                    )

                             select new RegistroAF
                             {
                                 IdBien = activo.IdBien,
                                 Codigo = activo.CorrelativoBien,
                                 Descripcion = activo.Desripcion,
                                 fechacadena = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),

                                 AreaDeNegocio = area.Nombre,
                                 Responsable = resposable.Nombres + " " + resposable.Apellidos,

                             }).ToList();
                    return lista;
                }
            }
        }
        [HttpGet]
        [Route("api/SolicitudBaja/buscarActivoEdificioAsig/{buscador?}")]
        public IEnumerable<RegistroAF> buscarActivoEdificioAsig(string buscador = "")
        {
            List<RegistroAF> listaActivo;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                if (buscador == "")
                {
                    listaActivo = (from activo in bd.ActivoFijo
                                   join noFormulario in bd.FormularioIngreso
                                   on activo.NoFormulario equals noFormulario.NoFormulario
                                   join clasif in bd.Clasificacion
                                   on activo.IdClasificacion equals clasif.IdClasificacion
                                   where (activo.EstadoActual == 1 || activo.EstadoActual == 2 || activo.EstadoActual == 3) && activo.TipoActivo == 1 && activo.EstadoActual == 1
                                   orderby activo.CorrelativoBien
                                   select new RegistroAF
                                   {
                                       IdBien = activo.IdBien,
                                       Codigo = activo.CorrelativoBien,
                                       fechacadena = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                       Descripcion = activo.Desripcion,
                                       vidautil = activo.VidaUtil,
                                       Clasificacion = clasif.Clasificacion1
                                   }).ToList();

                    return listaActivo;
                }
                else
                {
                    listaActivo = (from activo in bd.ActivoFijo
                                   join noFormulario in bd.FormularioIngreso
                                   on activo.NoFormulario equals noFormulario.NoFormulario
                                   join clasif in bd.Clasificacion
                                   on activo.IdClasificacion equals clasif.IdClasificacion
                                   where (activo.EstadoActual == 1 || activo.EstadoActual == 2 || activo.EstadoActual == 3) && activo.TipoActivo == 1 && activo.EstadoActual == 1


                                     && ((activo.IdBien).ToString().Contains(buscador)
                                      || (activo.CorrelativoBien).ToLower().Contains(buscador.ToLower())
                                      || (noFormulario.FechaIngreso).ToString().ToLower().Contains(buscador.ToLower())
                                      || (activo.Desripcion).ToLower().Contains(buscador.ToLower())
                                      || (clasif.Clasificacion1).ToLower().Contains(buscador.ToLower()))
                                   orderby activo.CorrelativoBien

                                   select new RegistroAF
                                   {
                                       IdBien = activo.IdBien,
                                       Codigo = activo.CorrelativoBien,
                                       fechacadena = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                       Descripcion = activo.Desripcion,
                                       vidautil = activo.VidaUtil,
                                       Clasificacion = clasif.Clasificacion1

                                   }).ToList();

                    return listaActivo;
                }
            }
        }
        [HttpGet]
        [Route("api/SolicitudBaja/buscarActivoIntengibleAsig/{buscador?}")]
        public IEnumerable<RegistroAF> buscarActivoIntengibleAsig(string buscador = "")
        {
            List<RegistroAF> listaActivo;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                if (buscador == "")
                {
                    listaActivo = (from activo in bd.ActivoFijo
                                   join noFormulario in bd.FormularioIngreso
                                   on activo.NoFormulario equals noFormulario.NoFormulario
                                   join clasif in bd.Clasificacion
                                   on activo.IdClasificacion equals clasif.IdClasificacion
                                   where (activo.EstadoActual == 1 || activo.EstadoActual == 2 || activo.EstadoActual == 3) && activo.TipoActivo == 3 && activo.EstadoActual == 1
                                   orderby activo.CorrelativoBien
                                   select new RegistroAF
                                   {
                                       IdBien = activo.IdBien,
                                       Codigo = activo.CorrelativoBien,
                                       fechacadena = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                       Descripcion = activo.Desripcion,
                                       vidautil = activo.VidaUtil,
                                       Clasificacion = clasif.Clasificacion1
                                   }).ToList();

                    return listaActivo;
                }
                else
                {
                    listaActivo = (from activo in bd.ActivoFijo
                                   join noFormulario in bd.FormularioIngreso
                                   on activo.NoFormulario equals noFormulario.NoFormulario
                                   join clasif in bd.Clasificacion
                                   on activo.IdClasificacion equals clasif.IdClasificacion
                                   where (activo.EstadoActual == 1 || activo.EstadoActual == 2 || activo.EstadoActual == 3) && activo.TipoActivo == 3 && activo.EstadoActual == 1

                                     && ((activo.IdBien).ToString().Contains(buscador)
                                      || (activo.CorrelativoBien).ToLower().Contains(buscador.ToLower())
                                      || (noFormulario.FechaIngreso).ToString().ToLower().Contains(buscador.ToLower())
                                      || (activo.Desripcion).ToLower().Contains(buscador.ToLower())
                                      || (clasif.Clasificacion1).ToLower().Contains(buscador.ToLower()))

                                   orderby activo.CorrelativoBien
                                   select new RegistroAF
                                   {
                                       IdBien = activo.IdBien,
                                       Codigo = activo.CorrelativoBien,
                                       fechacadena = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                       Descripcion = activo.Desripcion,
                                       vidautil = activo.VidaUtil,
                                       Clasificacion = clasif.Clasificacion1

                                   }).ToList();

                    return listaActivo;
                }
            }
        }
        //Buscadores no asignados
        [HttpGet]
        [Route("api/SolicitudBaja/buscarActivoNoAsig/{buscador?}")]
        public IEnumerable<NoAsignadosAF> buscarActivo(string buscador = "")
        {
            List<NoAsignadosAF> lista;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                if (buscador == "")
                {
                    lista = (from activo in bd.ActivoFijo
                             join noFormulario in bd.FormularioIngreso
                             on activo.NoFormulario equals noFormulario.NoFormulario
                             join clasif in bd.Clasificacion
                             on activo.IdClasificacion equals clasif.IdClasificacion
                             where activo.EstadoActual == 1 && activo.EstaAsignado == 0

                             select new NoAsignadosAF
                             {
                                 IdBien = activo.IdBien,
                                 NoFormulario = noFormulario.NoFormulario,
                                 fechacadena = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                 Desripcion = activo.Desripcion,
                                 Clasificacion = clasif.Clasificacion1

                             }).ToList();

                    return lista;
                }
                else
                {
                    lista = (from activo in bd.ActivoFijo
                             join noFormulario in bd.FormularioIngreso
                             on activo.NoFormulario equals noFormulario.NoFormulario
                             join clasif in bd.Clasificacion
                             on activo.IdClasificacion equals clasif.IdClasificacion
                             where activo.EstadoActual == 1 && activo.EstaAsignado == 0

                                     && ((activo.IdBien).ToString().Contains(buscador)
                                      || (activo.CorrelativoBien).ToLower().Contains(buscador.ToLower())
                                      || (noFormulario.FechaIngreso).ToString().ToLower().Contains(buscador.ToLower())
                                      || (activo.Desripcion).ToLower().Contains(buscador.ToLower())
                                      || (clasif.Clasificacion1).ToLower().Contains(buscador.ToLower())

                                      )
                             select new NoAsignadosAF
                             {
                                 IdBien = activo.IdBien,
                                 NoFormulario = noFormulario.NoFormulario,
                                 fechacadena = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                 Desripcion = activo.Desripcion,
                                 Clasificacion = clasif.Clasificacion1

                             }).ToList();
                    return lista;
                }
            }
        }

    }

}