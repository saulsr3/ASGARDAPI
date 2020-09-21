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
        [Route("api/SolicitudBaja/cambiarEstadoAceptado/{idbien}/{acuerdo}")] //
        public int cambiarEstado(int idbien, string acuerdo)//, string acuerdo
        {
            int rpta = 0;

            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    Console.WriteLine("IDESTADO" + idbien);
                    SolicitudBaja oSolic = bd.SolicitudBaja.Where(p => p.IdSolicitud == idbien).First();
                    ActivoFijo oActivo = bd.ActivoFijo.Where(p => p.IdBien == oSolic.IdBien).First();
                    oActivo.EstadoActual = 0;
                    oActivo.EstaAsignado = 0;
                   
                   oSolic.Acuerdo = acuerdo;
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
        [Route("api/SolicitudBaja/cambiarEstadoRechazado/{idbien}/{acuerdo}")]
        public int cambiarEstadoDenegado(int idbien , string acuerdo )
        {
            int rpta = 0;

            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    SolicitudBaja oSolic = bd.SolicitudBaja.Where(p => p.IdSolicitud == idbien).First();
                    ActivoFijo oActivo = bd.ActivoFijo.Where(p => p.IdBien == oSolic.IdBien).First();
                    oActivo.EstadoActual = 1;
                    
                    oSolic.Acuerdo = acuerdo;
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
                
                //Empleado oempleado = bd.Empleado.Where(p => p.IdEmpleado == obien.IdResponsable).First();
                //AreaDeNegocio oArea = bd.AreaDeNegocio.Where(p => p.IdAreaNegocio == oempleado.IdAreaDeNegocio).First();
                
                //odatos.areanegocio = oArea.Nombre;
                odatos.NoSolicitud =  osolicitud.IdSolicitud;
                odatos.fechacadena = osolicitud.Fecha == null ? " " : ((DateTime)osolicitud.Fecha).ToString("dd-MM-yyyy");
                odatos.Codigo = obien.CorrelativoBien;
                TipoDescargo odescargo = bd.TipoDescargo.Where(p => p.IdTipo == osolicitud.IdTipoDescargo).First();
                //odatos.motivo = osolicitud.;
                odatos.nombredescargo = odescargo.Nombre; 
                odatos.folio = osolicitud.Folio;
                odatos.idbien = (int) osolicitud.IdBien;
                odatos.Codigo = obien.CorrelativoBien;
                odatos.Descripcion = obien.Desripcion;
                odatos.observaciones = osolicitud.Observaciones;
               // odatos.Resposnsable = oempleado.Nombres + "" + oempleado.Apellidos;

                return odatos;


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
        [Route("api/SolicitudBaja/buscarBienesBajaNoA/{buscador?}")]
        public IEnumerable<ActivoFijoAF> buscarBienesBaja(string buscador = "")
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
                             where solicitud.Estado == 1
                             select new SolicitudBajaAF
                             {
                                 idbien = activo.IdBien,
                                 idsolicitud = solicitud.IdSolicitud,
                                 folio = solicitud.Folio,
                                 fechacadena = solicitud.Fecha == null ? " " : ((DateTime)solicitud.Fecha).ToString("dd-MM-yyyy"),
                                 observaciones = solicitud.Observaciones,
                                 idtipodescargo = (int)solicitud.IdTipoDescargo,

                             }).ToList();

                    return lista;
                }
                else
                {
                    lista = (from activo in bd.ActivoFijo
                             join solicitud in bd.SolicitudBaja
                             on activo.IdBien equals solicitud.IdBien
                             where solicitud.Estado == 1

                                 && ((solicitud.Folio).ToLower().Contains(buscador.ToLower()) ||
                                    (solicitud.Fecha).ToString().ToLower().Contains(buscador.ToLower()) ||
                                    (solicitud.IdTipoDescargo).ToString().ToLower().Contains(buscador.ToLower()) ||
                                    (solicitud.Observaciones).ToLower().Contains(buscador.ToLower())
                                    
                                    )

                             select new SolicitudBajaAF
                             {
                                 idbien = activo.IdBien,
                                 idsolicitud = solicitud.IdSolicitud,
                                 folio = solicitud.Folio,
                                 fechacadena = solicitud.Fecha == null ? " " : ((DateTime)solicitud.Fecha).ToString("dd-MM-yyyy"),
                                 observaciones = solicitud.Observaciones,
                                 idtipodescargo = (int)solicitud.IdTipoDescargo,

                             }).ToList();
                    return lista;
                }
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
                                                   join solicitud in bd.SolicitudBaja
                                                   on activo.IdBien equals solicitud.IdBien
                                                   where activo.EstadoActual == 0
                                                   orderby activo.CorrelativoBien
                                                   select new ActivoFijoAF
                                                   {
                                                       IdBien = activo.IdBien,
                                                       Desripcion = activo.Desripcion,
                                                       fechacadena = solicitud.Fecha == null ? " " : ((DateTime)solicitud.Fecha).ToString("dd-MM-yyyy"),
                                                       Clasificacion = clasi.Clasificacion1
                                                   }).ToList();
                return lista;

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

        //para filtro combos
        [HttpGet]
        [Route("api/SolicitudBaja/listarActivosFiltro/{id}")]
        public IEnumerable<ActivoFijoAF> listarActivosFiltro(int id)
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<ActivoFijoAF> lista = (from activo in bd.ActivoFijo
                                                   join noFormulario in bd.FormularioIngreso
                                                   on activo.NoFormulario equals noFormulario.NoFormulario
                                                   join clasif in bd.Clasificacion
                                                   on activo.IdClasificacion equals clasif.IdClasificacion
                                                   join resposable in bd.Empleado
                                                   on activo.IdResponsable equals resposable.IdEmpleado
                                                   join area in bd.AreaDeNegocio
                                                   on resposable.IdAreaDeNegocio equals area.IdAreaNegocio
                                                   where activo.EstadoActual == 1 && activo.EstaAsignado == 1 && area.IdAreaNegocio == id
                                                   orderby activo.CorrelativoBien
                                                   select new ActivoFijoAF
                                                   {
                                                       IdBien = activo.IdBien,
                                                       Codigo = activo.CorrelativoBien,
                                                       fechacadena = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                                       Desripcion = activo.Desripcion,
                                                       Clasificacion = clasif.Clasificacion1,
                                                       AreaDeNegocio = area.Nombre,
                                                       Resposnsable = resposable.Nombres + " " + resposable.Apellidos
                                                   }).ToList();
                return lista;

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

    }

}