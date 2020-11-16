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
    public class SolicitudTraspasoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        //LISTAR ACTIVOS ASIGNADOS PARA REALIZAR SOLICITUD DE TRASPASO.
        [HttpGet]
        [Route("api/SolicitudTraspaso/listarActivosAsignados")]
        public List<ActivoFijoAF> listarActivosAsignados()
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
                                            join sucursal in bd.Sucursal
                                            on area.IdSucursal equals sucursal.IdSucursal
                                            where activo.EstadoActual == 1 && activo.EstaAsignado == 1
                                            orderby activo.CorrelativoBien
                                            select new ActivoFijoAF
                                            {
                                                IdBien = activo.IdBien,
                                                Codigo = activo.CorrelativoBien,
                                                fechacadena = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                                Desripcion = activo.Desripcion,
                                                AreaDeNegocio = area.Nombre + " - " + sucursal.Nombre + " - " + sucursal.Ubicacion,
                                                Resposnsable = resposable.Nombres + " " + resposable.Apellidos,
                                                cargo = cargo.Cargo,

                                            }).ToList();
                return lista;

            }
        }

        //LISTAR EL AREA DE NEGOCIO CON LA SUCURSAL Y SU UBICACIÓN EN UN COMBO.
        [HttpGet]
        [Route("api/SolicitudTraspaso/listarAreaCombo")]
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

        //LISTAR LOS EMPLEADOS EN UN COMBO.
        [HttpGet]
        [Route("api/SolicitudTraspaso/listarEmpleadosCombo")]
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

        //GUARDAR SOLICITUD DE TRASPASO

        [HttpPost]
        [Route("api/SolicitudTraspaso/guardarSolicitudTraspaso")]
        public int guardarSolicitudTraspaso([FromBody]SolicitudTraspasoAF oSolicitudAF)
        {
            int respuesta = 0;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {

                    SolicitudTraspaso oSolicitud = new SolicitudTraspaso();
                    oSolicitud.IdSolicitud = oSolicitudAF.idsolicitud;
                    oSolicitud.IdBien = oSolicitudAF.idbien;                    
                    oSolicitud.Fecha = oSolicitudAF.fechasolicitud;
                    oSolicitud.Folio = oSolicitudAF.folio;
                    oSolicitud.Descripcion = oSolicitudAF.descripcion;       
                    //en la bd se llaman responsableanterior y area anterior pero se guardarán los nuevos porque los anteriores serian los actuales.
                    oSolicitud.ResponsableAnterior = oSolicitudAF.nuevoresponsable;
                    oSolicitud.AreadenegocioAnterior = oSolicitudAF.nuevaarea;                   
                    oSolicitud.Estado = 1;
                    bd.SolicitudTraspaso.Add(oSolicitud);
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

        //CAMBIAR ESTADO DEL ACTIVO QUE SE ENVIA A SOLICITUD
        [HttpPost]
        [Route("api/SolicitudTraspaso/cambiarEstadoSolicitud")]
        public int cambiarEstadoSolicitud([FromBody]ActivoFijoAF oTraspaso)
        {
            int respuesta = 0;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    ActivoFijo bien = new ActivoFijo();
                    SolicitudTraspaso idSolicitud = bd.SolicitudTraspaso.Where(p => p.Estado == 1).First();

                   // Console.WriteLine("IDBIEN" + oTraspaso.IdBien);
                    ActivoFijo oActivo = bd.ActivoFijo.Where(p => p.IdBien == oTraspaso.IdBien).First();
                    oActivo.EstadoActual = 6;// estado 6 del activo indica que está en solicitud de traspaso (7 será cuando lo apruebe)                   
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

        //LISTAR SOLICITUD DE TRASPASO
        [HttpGet]
        [Route("api/SolicitudTraspaso/listarSolicitudTraspaso")]
        public IEnumerable<SolicitudTraspasoAF> listarSolicitudTraspaso()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<SolicitudTraspasoAF> lista = (from activo in bd.ActivoFijo
                                                      join solicitud in bd.SolicitudTraspaso
                                                      on activo.IdBien equals solicitud.IdBien
                                                      join empleado in bd.Empleado
                                                      on activo.IdResponsable equals empleado.IdEmpleado
                                                      join area in bd.AreaDeNegocio
                                                      on empleado.IdAreaDeNegocio equals area.IdAreaNegocio
                                                      join sucursal in bd.Sucursal
                                                      on area.IdSucursal equals sucursal.IdSucursal
                                                      where solicitud.Estado == 1
                                                      select new SolicitudTraspasoAF
                                                      {
                                                          idbien = activo.IdBien,
                                                          idsolicitud = solicitud.IdSolicitud,
                                                          folio = solicitud.Folio,
                                                          codigo=activo.CorrelativoBien,
                                                          fechacadena = solicitud.Fecha == null ? " " : ((DateTime)solicitud.Fecha).ToString("dd-MM-yyyy"),
                                                          descripcion= solicitud.Descripcion,
                                                          responsableanterior= solicitud.ResponsableAnterior,
                                                          areaanterior=solicitud.AreadenegocioAnterior, 
                                                          nuevoresponsable= empleado.Nombres +" "+ empleado.Apellidos,
                                                          nuevaarea= area.Nombre +" "+ sucursal.Nombre +" "+sucursal.Ubicacion,
                                                      }).ToList();
                return lista;

            }
        }

        //VER LOS DATOS DE LA SOLICITUD

        [HttpGet]
        [Route("api/SolicitudTraspaso/verSolicitudTraspaso/{idSolicitud}")]
        public DatosTraspasoAF verSolicitudTraspaso(int idSolicitud)
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                DatosTraspasoAF odatos = new DatosTraspasoAF();
                SolicitudTraspaso oSolicitud = bd.SolicitudTraspaso.Where(p => p.IdSolicitud == idSolicitud).First();
                ActivoFijo oActivo = bd.ActivoFijo.Where(p => p.IdBien == oSolicitud.IdBien).First();
                Empleado oEmpleado = bd.Empleado.Where(p=> p.IdEmpleado == oActivo.IdResponsable).First();
                AreaDeNegocio oArea = bd.AreaDeNegocio.Where(p => p.IdAreaNegocio == oEmpleado.IdAreaDeNegocio).First();
                Sucursal oSucursal = bd.Sucursal.Where(p=>p.IdSucursal == oArea.IdSucursal).First();

                odatos.NoSolicitud = oSolicitud.IdSolicitud;
                odatos.fechacadena = oSolicitud.Fecha == null ? " " : ((DateTime)oSolicitud.Fecha).ToString("dd-MM-yyyy");
                odatos.folio = oSolicitud.Folio;
                odatos.idbien = (int)oSolicitud.IdBien;
                odatos.areanegocioactual = oArea.Nombre + " - " + oSucursal.Nombre + " - " + oSucursal.Ubicacion;
                odatos.areanegocioanterior = oSolicitud.AreadenegocioAnterior;
                odatos.responsableactual = oEmpleado.Nombres + " " + oEmpleado.Apellidos;
                odatos.responsableanterior = oSolicitud.ResponsableAnterior;
                odatos.Codigo = oActivo.CorrelativoBien;
                odatos.Descripcion = oActivo.Desripcion;

                return odatos;
            }
        }

        //ACEPTAR LA SOLICITUD DE TRASPASO DE UN ACTIVO.
        [HttpGet]
        [Route("api/SolicitudTraspaso/aceptarSolicitud/{idsolicitud}")]
        public int aceptarSolicitud(int idsolicitud)
        {
            int respuesta = 0;

            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    SolicitudTraspaso oSolicitud = bd.SolicitudTraspaso.Where(p => p.IdSolicitud == idsolicitud).First();
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

        //CAMBIAR ESTADO DEL BIEN A ACEPTADO
        //GUARDO EL ACUERDO Y LA FECHA AQUI POR QUE DEPENDE DE SI ACEPTO O NO LA SOLICITUD.
        [HttpGet]
        [Route("api/SolicitudTraspaso/cambiarEstadoAceptoTraspaso/{idactivo}/{acuerdo}/{fechatraspaso}")] // 
        public int cambiarEstadoAceptoTraspaso(int idactivo, string acuerdo, string fechasolicitud)// 
        {
            int respuesta = 0;

            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    //primero cambiamos el el estado del activo para hacer notar que ya no está en solicitud (UNA VEZ SEA APROBADA)
                    SolicitudTraspaso oSolicitudT = bd.SolicitudTraspaso.Where(p => p.IdSolicitud == idactivo).First();
                    ActivoFijo oActivo = bd.ActivoFijo.Where(p => p.IdBien == oSolicitudT.IdBien).First();
                    oActivo.EstadoActual = 1;
                    oActivo.EstaAsignado = 1;
                    //guardamos el acuerdo y la fecha
                    oSolicitudT.Acuerdo = acuerdo;
                    oSolicitudT.Fechatraspaso = Convert.ToDateTime(fechasolicitud);
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

        //DENEGAR SOLICITUD DE TRASPASO
        [HttpGet]
        [Route("api/SolicitudTraspaso/denegarSolicitud/{idsolicitud}")]
        public int denegarSolicitud(int idsolicitud)
        {
            int rpta = 0;

            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    SolicitudTraspaso oSolicitud = bd.SolicitudTraspaso.Where(p => p.IdSolicitud == idsolicitud).First();
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

        //CAMBIAR ESTADO DE SOLICITUD CUANDO SE DENEGA
        [HttpGet]
        [Route("api/SolicitudTraspaso/estadoSolicitudDenegada/{idsolicitud}")]
        public int estadoSolicitudDenegada(int idsolicitud)
        {
            int respuesta = 0;

            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                   // SolicitudTraspaso oSolicitud = bd.SolicitudTraspaso.Where(p => p.IdSolicitud == idsolicitud).First();

                    ActivoFijo oActivo = bd.ActivoFijo.Where(p => p.IdBien == idsolicitud).First();                   
                    oActivo.EstadoActual = 1;// a 1 para que vuelva a estar disponible para traspaso o para otros procesos
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

    }
}