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
                                                               where activo.EstaAsignado == 1 && activo.TipoActivo==2 && activo.EstadoActual == 1 && activo.EstadoActual != 2 && activo.EstadoActual != 6 && activo.EnSolicitud==0

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
        [Route("api/SolicitudMantenimiento/validarActivosParaMantenimiento")]
        public int validarActivosParaMantenimiento()
        {
            int rpta = 0;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())

            {
               
                IEnumerable<SolicitudMantenimientoAF> lista = (from activo in bd.ActivoFijo
                                                               where activo.EstaAsignado == 1 && activo.TipoActivo == 2 && activo.EstadoActual == 1 && activo.EstadoActual != 2 && activo.EnSolicitud == 0
                                                               select new SolicitudMantenimientoAF
                                                 {
                                                     idbien = activo.IdBien,
                                                     codigobien = activo.CorrelativoBien,
                                                     descripcionbien = activo.Desripcion
                                                 }).ToList();
                if (lista.Count() > 0)
                {
                    rpta = 1;
                }
                return rpta;
            }
        }

        [HttpGet]
        [Route("api/SolicitudMantenimiento/listarSolicitudMante")]
        public IEnumerable<SolicitudMantenimientoPAF> listarSolicitudMante()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())

            {

                IEnumerable<SolicitudMantenimientoPAF> lista = (from solicitud in bd.SolicitudMantenimiento
                                                                where solicitud.Estado == 1

                                                                select new SolicitudMantenimientoPAF
                                                                {
                                                                    idsolicitud = solicitud.IdSolicitud,
                                                                    folio = solicitud.Folio,
                                                                    descripcion = solicitud.Descripcion,
                                                                    fechacadena = solicitud.Fecha == null ? " " : ((DateTime)solicitud.Fecha).ToString("dd-MM-yyyy")
                                                                }).ToList();


                return lista;

            }

        }
        [HttpGet]
        [Route("api/SolicitudMantenimiento/validarSolicitudesParaMantenimiento")]
        public int validarSolicitudesParaMantenimiento()
        {
            int rpta = 0;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())

            {

                IEnumerable<SolicitudMantenimientoPAF> lista = (from solicitud in bd.SolicitudMantenimiento
                                                               where solicitud.Estado == 1

                                                               select new SolicitudMantenimientoPAF
                                                               {
                                                                   idsolicitud = solicitud.IdSolicitud,
                                                                   folio = solicitud.Folio,
                                                                   descripcion = solicitud.Descripcion,
                                                                   fechacadena = solicitud.Fecha == null ? " " : ((DateTime)solicitud.Fecha).ToString("dd-MM-yyyy")

                                                               }).ToList();
                if (lista.Count() > 0)
                {
                    rpta = 1;
                }
                return rpta;
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


        //guardar los bienes y sus caracteristicas de solicitud de mantenimiento.
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
                    bienMtto.Estado = 1; //estado uno para que se liste en bienes en mantenimiento.  ELEMENTO 1.
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

        



        //metodos para aceptar solicitud y cambiar estado actual
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
                    BienMantenimiento obienmantenimiento = bd.BienMantenimiento.Where(p => p.IdBien == idBien).First();
                    oActivo.EstadoActual = 3;
                   // obienmantenimiento.Estado = 1; //cambiamos el estado a 1 para que lo liste en bienes en mantenimiento.
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

        //cambia el estado una vez que el bien se denego 
        [HttpGet]
        [Route("api/SolicitudMantenimiento/cambiarEstadoDenegado/{idBien}")]
        public int cambiarEstadoDenegado(int idBien)
        {
            int respuesta = 0;

            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    ActivoFijo oActivo = bd.ActivoFijo.Where(p => p.IdBien == idBien).Last();
                    BienMantenimiento obienmantenimiento = bd.BienMantenimiento.Where(p => p.IdBien == idBien).Last();
                    obienmantenimiento.Estado = 5; //cambiamos el estado a 2 para que ya no liste en bienes en mantenimeitno// ELEMENTO 3 SIRVE
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
        [Route("api/SolicitudMantenimiento/validarFolio/{idsolicitud}/{folio}")]
        public int validarFolio(int idsolicitud, string folio)
        {
            int respuesta = 0;
            try
            {


                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    if (idsolicitud == 0)
                    {
                        respuesta = bd.SolicitudMantenimiento.Where(p => p.Folio.ToLower() == folio.ToLower()).Count();
                    }
                    else
                    {
                        respuesta = bd.SolicitudMantenimiento.Where(p => p.Folio.ToLower() == folio.ToLower() ).Count();
                    }


                }

            }
            catch (Exception ex)
            {
                respuesta = 0;

            }
            return respuesta;

        }

        //metodo para buscar solicitudes de mantenimiento.
        [HttpGet]
        [Route("api/SolicitudMantenimiento/buscarSolicitudMante/{buscador?}")]
        public IEnumerable<SolicitudMantenimientoPAF> buscarSolicitudMante(string buscador = "")
        {
            List<SolicitudMantenimientoPAF> listaSolicitudMantenimiento;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                if (buscador == "")
                {
                    listaSolicitudMantenimiento = (from solicitudmante in bd.SolicitudMantenimiento  
                                   where solicitudmante.Estado == 1
                                     select new SolicitudMantenimientoPAF
                                     {
                                         idsolicitud= solicitudmante.IdSolicitud,
                                         folio= solicitudmante.Folio,
                                         fechacadena= solicitudmante.Fecha.ToString(),
                                         descripcion=solicitudmante.Descripcion
                                        

                                     }).ToList();
                    return listaSolicitudMantenimiento;
                }
                else
                {
                    listaSolicitudMantenimiento = (from solicitudmante in bd.SolicitudMantenimiento
                                                   where solicitudmante.Estado == 1

                                                     && ((solicitudmante.IdSolicitud).ToString().Contains(buscador.ToLower()) ||
                                     (solicitudmante.Folio).ToLower().Contains(buscador.ToLower()) ||
                                     (solicitudmante.Fecha).ToString().ToLower().Contains(buscador.ToLower()) ||
                                     (solicitudmante.Descripcion).ToLower().Contains(buscador.ToLower()))

                                     select new SolicitudMantenimientoPAF
                                     {
                                         idsolicitud = solicitudmante.IdSolicitud,
                                         folio = solicitudmante.Folio,
                                         fechacadena = solicitudmante.Fecha.ToString(),
                                         descripcion = solicitudmante.Descripcion
                                     }).ToList();
                    return listaSolicitudMantenimiento;
                }
            }
        }

       


        //metodo para buscar bienes en mantenimiento.
        [HttpGet]
        [Route("api/SolicitudMantenimiento/buscarBienesMante/{buscador?}")]
        public IEnumerable<BienesSolicitadosMttoAF> buscarBienesMante(string buscador = "")
        {
            List<BienesSolicitadosMttoAF> listaBienesMantenimiento;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                if (buscador == "")
                {
                    listaBienesMantenimiento = (from bienMtto in bd.BienMantenimiento
                                                join activo in bd.ActivoFijo
                                                on bienMtto.IdBien equals activo.IdBien
                                                join solicitud in bd.SolicitudMantenimiento
                                                on bienMtto.IdSolicitud equals solicitud.IdSolicitud
                                                //  join informe in bd.InformeMantenimiento
                                                // on bienMtto.IdMantenimiento equals informe.IdMantenimiento
                                                where activo.EstadoActual == 3 && bienMtto.Estado == 1 //ELEMENTO 2 LISTA
                                                select new BienesSolicitadosMttoAF
                                                {
                                                    idBien = activo.IdBien,
                                                    idmantenimiento = bienMtto.IdMantenimiento,
                                                    estadoActual = (int)activo.EstadoActual,
                                                    Codigo = activo.CorrelativoBien,
                                                    Descripcion = activo.Desripcion,
                                                    Periodo = bienMtto.PeriodoMantenimiento,
                                                    Razon = bienMtto.RazonMantenimiento,
                                                    fechacadena = solicitud.Fecha == null ? " " : ((DateTime)solicitud.Fecha).ToString("dd-MM-yyyy"),


                                                }).ToList();
                    return listaBienesMantenimiento;
                }
                else
                {
                    listaBienesMantenimiento = (from bienMtto in bd.BienMantenimiento
                                                join activo in bd.ActivoFijo
                                                on bienMtto.IdBien equals activo.IdBien
                                                join solicitud in bd.SolicitudMantenimiento
                                                on bienMtto.IdSolicitud equals solicitud.IdSolicitud
                                                //  join informe in bd.InformeMantenimiento
                                                // on bienMtto.IdMantenimiento equals informe.IdMantenimiento
                                                where activo.EstadoActual == 3 && bienMtto.Estado == 1 //ELEMENTO 2 LISTA

                                                     && ((activo.CorrelativoBien).ToLower().Contains(buscador.ToLower()) ||
                                     (activo.Desripcion).ToLower().Contains(buscador.ToLower()) ||
                                     (bienMtto.PeriodoMantenimiento).ToString().ToLower().Contains(buscador.ToLower()) ||
                                     (bienMtto.RazonMantenimiento).ToLower().Contains(buscador.ToLower()))
                                                select new BienesSolicitadosMttoAF
                                                {
                                                    idBien = activo.IdBien,
                                                    idmantenimiento = bienMtto.IdMantenimiento,
                                                    estadoActual = (int)activo.EstadoActual,
                                                    Codigo = activo.CorrelativoBien,
                                                    Descripcion = activo.Desripcion,
                                                    Periodo = bienMtto.PeriodoMantenimiento,
                                                    Razon = bienMtto.RazonMantenimiento,
                                                    fechacadena = solicitud.Fecha == null ? " " : ((DateTime)solicitud.Fecha).ToString("dd-MM-yyyy"),


                                                }).ToList();
                    return listaBienesMantenimiento;
                }
            }
        }


        //buscar activos por codigo (en la parte para enviarlos a mantenimiento)
        [HttpGet]
        [Route("api/SolicitudMantenimiento/buscarBienescodigo/{buscador?}")]
        public IEnumerable<SolicitudMantenimientoAF> buscarBienescodigo(string buscador = "")
        {
            List<SolicitudMantenimientoAF> listaBienesCodigo;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                if (buscador == "")
                {
                    listaBienesCodigo = (from activo in bd.ActivoFijo
                                         where activo.EstaAsignado == 1 && activo.EstadoActual == 1 && activo.EstadoActual != 2
                                         select new SolicitudMantenimientoAF
                                                {

                                                    idbien = activo.IdBien,
                                                    codigobien = activo.CorrelativoBien,
                                                    descripcionbien = activo.Desripcion
                                                }).ToList();
                    return listaBienesCodigo;
                }
                else
                {
                    listaBienesCodigo = (from activo in bd.ActivoFijo
                                         where activo.EstaAsignado == 1 && activo.EstadoActual == 1 && activo.EstadoActual != 2


                                              && ((activo.CorrelativoBien).ToLower().Contains(buscador.ToLower())
                                                ||  (activo.Desripcion).ToLower().Contains(buscador.ToLower()))

                                                select new SolicitudMantenimientoAF
                                                {
                                                    idbien = activo.IdBien,
                                                    codigobien = activo.CorrelativoBien,
                                                    descripcionbien = activo.Desripcion
                                                }).ToList();
                    return listaBienesCodigo;
                }
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
        [HttpGet]
        [Route("api/SolicitudMantenimiento/CambiarEstadoSolicitud/{idActivo}")]
        public int CambiarEstadoSolicitud(int idActivo)
        {
            int rpta = 0;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    ActivoFijo oActivo = bd.ActivoFijo.Where(p => p.IdBien == idActivo).First();
                    oActivo.EnSolicitud = 1;
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

        //Prueba cambio de estado
        [HttpGet]
        [Route("api/SolicitudMantenimiento/CambiarEstadoActivosSolicitud")]
        public int CambiarEstadoActivosSolicitud()
        {
            int respuesta = 0;
            try
            {


                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    IEnumerable<NoAsignadosAF> lista = (from activo in bd.ActivoFijo
                                                        where activo.EstadoActual == 1 && activo.EnSolicitud == 1
                                                        select new NoAsignadosAF
                                                        {
                                                            IdBien = activo.IdBien
                                                        }).ToList();
                    foreach (var item in lista)
                    {
                        ActivoFijo oActivo = bd.ActivoFijo.Where(p => p.IdBien == item.IdBien).First();
                        oActivo.EnSolicitud = 0;
                        bd.SaveChanges();
                        respuesta = 1;
                    }


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