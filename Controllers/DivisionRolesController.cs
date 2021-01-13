using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASGARDAPI.Clases;
using ASGARDAPI.Models;

namespace ASGARDAPI.Controllers
{
    public class DivisionRolesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        //Activos asignados de bienes muebles
        [HttpGet]
        [Route("api/Division/listarActivosAsignadosJefe/{idJefe}")]
        public List<RegistroAF> listarActivosAsignadosJefe(int idJefe)
        {
            
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                Empleado oempleado = bd.Empleado.Where(p => p.IdEmpleado == idJefe).FirstOrDefault();
                AreaDeNegocio oarea = bd.AreaDeNegocio.Where(p => p.IdAreaNegocio == oempleado.IdAreaDeNegocio).FirstOrDefault();
                List<RegistroAF> lista = (from activo in bd.ActivoFijo
                                          join noFormulario in bd.FormularioIngreso
                                          on activo.NoFormulario equals noFormulario.NoFormulario
                                          join clasif in bd.Clasificacion
                                          on activo.IdClasificacion equals clasif.IdClasificacion
                                          join resposable in bd.Empleado
                                          on activo.IdResponsable equals resposable.IdEmpleado
                                          join area in bd.AreaDeNegocio
                                          on resposable.IdAreaDeNegocio equals area.IdAreaNegocio
                                          where (activo.EstadoActual != 0) && activo.EstaAsignado == 1 && area.IdAreaNegocio==oarea.IdAreaNegocio

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

        [HttpGet]
        [Route("api/ActivoFijo/buscarActivoAsigJefe/{idJefe}/{buscador?}")]
        public IEnumerable<RegistroAF> buscarActivoAsigJefe(int idJefe,string buscador = "")
        {
            List<RegistroAF> listaActivo;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                Empleado oempleado = bd.Empleado.Where(p => p.IdEmpleado == idJefe).FirstOrDefault();
                AreaDeNegocio oarea = bd.AreaDeNegocio.Where(p => p.IdAreaNegocio == oempleado.IdAreaDeNegocio).FirstOrDefault();
                if (buscador == "")
                {
                    listaActivo = (from activo in bd.ActivoFijo
                                   join noFormulario in bd.FormularioIngreso
                                   on activo.NoFormulario equals noFormulario.NoFormulario
                                   join clasif in bd.Clasificacion
                                   on activo.IdClasificacion equals clasif.IdClasificacion
                                   join resposable in bd.Empleado
                                   on activo.IdResponsable equals resposable.IdEmpleado
                                   join area in bd.AreaDeNegocio
                                   on resposable.IdAreaDeNegocio equals area.IdAreaNegocio
                                   where (activo.EstadoActual != 0) && activo.EstaAsignado == 1 && area.IdAreaNegocio == oarea.IdAreaNegocio

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

                    return listaActivo;
                }
                else
                {
                    listaActivo = (from activo in bd.ActivoFijo
                                   join noFormulario in bd.FormularioIngreso
                                   on activo.NoFormulario equals noFormulario.NoFormulario
                                   join clasif in bd.Clasificacion
                                   on activo.IdClasificacion equals clasif.IdClasificacion
                                   join resposable in bd.Empleado
                                   on activo.IdResponsable equals resposable.IdEmpleado
                                   join area in bd.AreaDeNegocio
                                   on resposable.IdAreaDeNegocio equals area.IdAreaNegocio
                                   where (activo.EstadoActual != 0) && activo.EstaAsignado == 1 && area.IdAreaNegocio == oarea.IdAreaNegocio


                                       && ((activo.IdBien).ToString().Contains(buscador)
                                      || (activo.CorrelativoBien).ToLower().Contains(buscador.ToLower())
                                      || (noFormulario.FechaIngreso).ToString().ToLower().Contains(buscador.ToLower())
                                      || (activo.Desripcion).ToLower().Contains(buscador.ToLower())
                                      || (clasif.Clasificacion1).ToLower().Contains(buscador.ToLower())
                                      || (area.Nombre).ToLower().Contains(buscador.ToLower())
                                      || (resposable.Nombres).ToLower().Contains(buscador.ToLower())
                                      || (resposable.Apellidos).ToLower().Contains(buscador.ToLower()))
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

                    return listaActivo;
                }
            }
        }
        [HttpGet]
        [Route("api/Depreciacion/listarActivosTarjetaJefe/{idJefe}")]
        public IEnumerable<DepreciacionAF> listarActivosTarjetaJefe(int idJefe)
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                Empleado oempleado = bd.Empleado.Where(p => p.IdEmpleado == idJefe).FirstOrDefault();
                AreaDeNegocio oarea = bd.AreaDeNegocio.Where(p => p.IdAreaNegocio == oempleado.IdAreaDeNegocio).FirstOrDefault();
                IEnumerable<DepreciacionAF> listaActivos = (from activo in bd.ActivoFijo
                                                            join empleado in bd.Empleado
                                                            on activo.IdResponsable equals empleado.IdEmpleado
                                                            join area in bd.AreaDeNegocio
                                                            on empleado.IdAreaDeNegocio equals area.IdAreaNegocio
                                                            join sucursal in bd.Sucursal
                                                            on area.IdSucursal equals sucursal.IdSucursal

                                                            where (activo.EstadoActual != 0) && (activo.EstaAsignado == 0 || activo.EstaAsignado == 1) && area.IdAreaNegocio == oarea.IdAreaNegocio

                                                            select new DepreciacionAF
                                                            {
                                                                idBien = activo.IdBien,
                                                                codigo = activo.CorrelativoBien,
                                                                descripcion = activo.Desripcion,
                                                                areanegocio = area.Nombre,
                                                                sucursal = sucursal.Nombre,
                                                                responsable = empleado.Nombres + " " + empleado.Apellidos


                                                            }).ToList();
                return listaActivos;
            }
        }
        [HttpGet]
        [Route("api/Division/buscarActivosJefe/{idJefe}/{buscador?}")]
        public IEnumerable<DepreciacionAF> buscarActivosJefe(int idJefe,string buscador = "")
        {
            List<DepreciacionAF> listaActivo;

            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                Empleado oempleado = bd.Empleado.Where(p => p.IdEmpleado == idJefe).FirstOrDefault();
                AreaDeNegocio oarea = bd.AreaDeNegocio.Where(p => p.IdAreaNegocio == oempleado.IdAreaDeNegocio).FirstOrDefault();
                if (buscador == "")
                {
                    listaActivo = (from activo in bd.ActivoFijo
                                   join empleado in bd.Empleado
                                   on activo.IdResponsable equals empleado.IdEmpleado
                                   join area in bd.AreaDeNegocio
                                   on empleado.IdAreaDeNegocio equals area.IdAreaNegocio
                                   join sucursal in bd.Sucursal
                                   on area.IdSucursal equals sucursal.IdSucursal

                                   where (activo.EstadoActual != 0) && (activo.EstaAsignado == 0 || activo.EstaAsignado == 1) && area.IdAreaNegocio == oarea.IdAreaNegocio

                                   select new DepreciacionAF
                                   {
                                       idBien = activo.IdBien,
                                       codigo = activo.CorrelativoBien,
                                       descripcion = activo.Desripcion,
                                       areanegocio = area.Nombre,
                                       sucursal = sucursal.Nombre,
                                       responsable = empleado.Nombres + " " + empleado.Apellidos
                                   }).ToList();

                    return listaActivo;
                }
                else
                {
                    listaActivo = (from activo in bd.ActivoFijo
                                   join empleado in bd.Empleado
                                   on activo.IdResponsable equals empleado.IdEmpleado
                                   join area in bd.AreaDeNegocio
                                   on empleado.IdAreaDeNegocio equals area.IdAreaNegocio
                                   join sucursal in bd.Sucursal
                                   on area.IdSucursal equals sucursal.IdSucursal
                                   where (activo.EstadoActual != 0) && (activo.EstaAsignado == 0 || activo.EstaAsignado == 1) &&
                                      ((activo.CorrelativoBien).ToString().ToLower().Contains(buscador.ToLower())
                                      || (activo.Desripcion).ToString().ToLower().Contains(buscador.ToLower())
                                      || (area.Nombre).ToLower().Contains(buscador.ToLower())
                                      || (sucursal.Nombre).ToLower().Contains(buscador.ToLower())
                                      || (empleado.Nombres).ToLower().Contains(buscador.ToLower())
                                      || (empleado.Apellidos).ToLower().Contains(buscador.ToLower())
                                      ) && area.IdAreaNegocio == oarea.IdAreaNegocio
                                   select new DepreciacionAF
                                   {
                                       idBien = activo.IdBien,
                                       codigo = activo.CorrelativoBien,
                                       descripcion = activo.Desripcion,
                                       areanegocio = area.Nombre,
                                       sucursal = sucursal.Nombre,
                                       responsable = empleado.Nombres + " " + empleado.Apellidos
                                   }).ToList();

                    return listaActivo;
                }
            
        }
        }
        //Metodo listar cuadro de control
        [HttpGet]
        [Route("api/CuadroControl/listarCuadroControlJefe/{idJefe}")]
        public IEnumerable<CuadroControlAF> listarCuadroControlJefe(int idJefe)
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                Empleado oempleado = bd.Empleado.Where(p => p.IdEmpleado == idJefe).FirstOrDefault();
                AreaDeNegocio oarea = bd.AreaDeNegocio.Where(p => p.IdAreaNegocio == oempleado.IdAreaDeNegocio).FirstOrDefault();
                IEnumerable<CuadroControlAF> listacuadro = (
                                                              from tarjeta in bd.TarjetaDepreciacion
                                                              group tarjeta by tarjeta.IdBien into bar
                                                              join cuadro in bd.ActivoFijo
                                                              on bar.FirstOrDefault().IdBien equals cuadro.IdBien
                                                              join noFormulario in bd.FormularioIngreso
                                                              on cuadro.NoFormulario equals noFormulario.NoFormulario
                                                              join clasif in bd.Clasificacion
                                                              on cuadro.IdClasificacion equals clasif.IdClasificacion
                                                              join resposable in bd.Empleado
                                                              on cuadro.IdResponsable equals resposable.IdEmpleado
                                                              join area in bd.AreaDeNegocio
                                                              on resposable.IdAreaDeNegocio equals area.IdAreaNegocio
                                                              where area.IdAreaNegocio == oarea.IdAreaNegocio
                                                              //where cuadro.EstadoActual == 1 && cuadro.EstaAsignado == 1
                                                              select new CuadroControlAF()
                                                              {
                                                                  idbien = cuadro.IdBien,
                                                                  codigo = cuadro.CorrelativoBien,
                                                                  descripcion = cuadro.Desripcion,
                                                                  valoradquisicion = (double)cuadro.ValorAdquicicion,
                                                                  fechaadquisicion = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                                                  valoractual = Math.Round(((double)bar.OrderByDescending(x => x.IdTarjeta).First().ValorActual), 2),
                                                                  depreciacion = Math.Round(((double)bar.OrderByDescending(x => x.IdTarjeta).First().DepreciacionAnual), 2),
                                                                  depreciacionacumulada = Math.Round(((double)bar.Sum(x => x.DepreciacionAnual)), 2),
                                                                  ubicacion = area.Nombre,
                                                                  responsable = resposable.Nombres + " " + resposable.Apellidos

                                                              }).ToList();
                return listacuadro;

            }
        }
        //Método buscar
        [HttpGet]
        [Route("api/CuadroControl/buscarCuadroJefe/{idJefe}/{buscador?}")]
        public IEnumerable<CuadroControlAF> buscarCuadroJefe(int idJefe,string buscador = "")
        {
            List<CuadroControlAF> listaCuadro;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                Empleado oempleado = bd.Empleado.Where(p => p.IdEmpleado == idJefe).FirstOrDefault();
                AreaDeNegocio oarea = bd.AreaDeNegocio.Where(p => p.IdAreaNegocio == oempleado.IdAreaDeNegocio).FirstOrDefault();
                if (buscador == "")
                {
                    listaCuadro = (from tarjeta in bd.TarjetaDepreciacion
                                   group tarjeta by tarjeta.IdBien into bar
                                   join cuadro in bd.ActivoFijo
                                   on bar.FirstOrDefault().IdBien equals cuadro.IdBien
                                   join noFormulario in bd.FormularioIngreso
                                   on cuadro.NoFormulario equals noFormulario.NoFormulario
                                   join clasif in bd.Clasificacion
                                   on cuadro.IdClasificacion equals clasif.IdClasificacion
                                   join resposable in bd.Empleado
                                   on cuadro.IdResponsable equals resposable.IdEmpleado
                                   join area in bd.AreaDeNegocio
                                   on resposable.IdAreaDeNegocio equals area.IdAreaNegocio
                                   where area.IdAreaNegocio == oarea.IdAreaNegocio
                                   select new CuadroControlAF
                                   {

                                       idbien = cuadro.IdBien,
                                       codigo = cuadro.CorrelativoBien,
                                       descripcion = cuadro.Desripcion,
                                       valoradquisicion = (double)cuadro.ValorAdquicicion,
                                       fechaadquisicion = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                       valoractual = (double)bar.OrderByDescending(x => x.IdTarjeta).First().ValorActual,
                                       depreciacion = (double)bar.OrderByDescending(x => x.IdTarjeta).First().DepreciacionAnual,
                                       depreciacionacumulada = (double)bar.Sum(x => x.DepreciacionAnual),
                                       ubicacion = area.Nombre,
                                       responsable = resposable.Nombres + " " + resposable.Apellidos

                                   }).ToList();

                    return listaCuadro;
                }
                else
                {
                    listaCuadro = (from tarjeta in bd.TarjetaDepreciacion
                                   group tarjeta by tarjeta.IdBien into bar
                                   join cuadro in bd.ActivoFijo
                                   on bar.FirstOrDefault().IdBien equals cuadro.IdBien
                                   join noFormulario in bd.FormularioIngreso
                                   on cuadro.NoFormulario equals noFormulario.NoFormulario
                                   join clasif in bd.Clasificacion
                                   on cuadro.IdClasificacion equals clasif.IdClasificacion
                                   join resposable in bd.Empleado
                                   on cuadro.IdResponsable equals resposable.IdEmpleado
                                   join area in bd.AreaDeNegocio
                                   on resposable.IdAreaDeNegocio equals area.IdAreaNegocio

                                   where cuadro.IdBien.ToString().Contains(buscador) || (cuadro.CorrelativoBien).ToLower().Contains(buscador.ToLower()) ||
                                    cuadro.Desripcion.ToLower().Contains(buscador.ToLower())
                                   && area.IdAreaNegocio == oarea.IdAreaNegocio

                                   select new CuadroControlAF
                                   {
                                       idbien = cuadro.IdBien,
                                       codigo = cuadro.CorrelativoBien,
                                       descripcion = cuadro.Desripcion,
                                       valoradquisicion = (double)cuadro.ValorAdquicicion,
                                       fechaadquisicion = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                       valoractual = (double)bar.OrderByDescending(x => x.IdTarjeta).First().ValorActual,
                                       depreciacion = (double)bar.OrderByDescending(x => x.IdTarjeta).First().DepreciacionAnual,
                                       depreciacionacumulada = (double)bar.Sum(x => x.DepreciacionAnual),
                                       ubicacion = area.Nombre,
                                       responsable = resposable.Nombres + " " + resposable.Apellidos

                                   }).ToList();
                    return listaCuadro;
                }
            }
        }
        //Modulo de mantenimieto metodos para dividir roles

        [HttpGet]
        [Route("api/Division/listarBienesMttoJefe/{idJefe}")]
        public IEnumerable<SolicitudMantenimientoAF> listarBienesMttoJefe(int idJefe)
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                Empleado oempleado = bd.Empleado.Where(p => p.IdEmpleado == idJefe).FirstOrDefault();
                AreaDeNegocio oarea = bd.AreaDeNegocio.Where(p => p.IdAreaNegocio == oempleado.IdAreaDeNegocio).FirstOrDefault();
                IEnumerable<SolicitudMantenimientoAF> lista = (from activo in bd.ActivoFijo
                                                               join empleado in bd.Empleado
                                                               on activo.IdResponsable equals empleado.IdEmpleado
                                                               join area in bd.AreaDeNegocio
                                                               on empleado.IdAreaDeNegocio equals area.IdAreaNegocio
                                                               where activo.EstaAsignado == 1 && activo.TipoActivo == 2 && activo.EstadoActual == 1 && activo.EstadoActual != 2 && activo.EstadoActual != 6 && activo.EnSolicitud == 0 && area.IdAreaNegocio == oarea.IdAreaNegocio
                                                               select new SolicitudMantenimientoAF
                                                               {
                                                                   idbien = activo.IdBien,
                                                                   codigobien = activo.CorrelativoBien,
                                                                   descripcionbien = activo.Desripcion
                                                               }).ToList();
                return lista;
            }
        }
        [HttpGet]
        [Route("api/Division/buscarBienesSoliMtto/{idJefe}/{buscador?}")]
        public IEnumerable<SolicitudMantenimientoAF> buscarBienesSoliMtto(int idJefe, string buscador = "")
        {
            List<SolicitudMantenimientoAF> listaBienesCodigo;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                Empleado oempleado = bd.Empleado.Where(p => p.IdEmpleado == idJefe).FirstOrDefault();
                AreaDeNegocio oarea = bd.AreaDeNegocio.Where(p => p.IdAreaNegocio == oempleado.IdAreaDeNegocio).FirstOrDefault();
                if (buscador == "")
                {
                    listaBienesCodigo = (from activo in bd.ActivoFijo
                                         join empleado in bd.Empleado
                                         on activo.IdResponsable equals empleado.IdEmpleado
                                         join area in bd.AreaDeNegocio
                                         on empleado.IdAreaDeNegocio equals area.IdAreaNegocio
                                         where activo.EstaAsignado == 1 && activo.EstadoActual == 1 && activo.EstadoActual != 2 && area.IdAreaNegocio == oarea.IdAreaNegocio
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
                                         join empleado in bd.Empleado
                                         on activo.IdResponsable equals empleado.IdEmpleado
                                         join area in bd.AreaDeNegocio
                                         on empleado.IdAreaDeNegocio equals area.IdAreaNegocio
                                         where activo.EstaAsignado == 1 && activo.EstadoActual == 1 && activo.EstadoActual != 2 && area.IdAreaNegocio == oarea.IdAreaNegocio
                                              && ((activo.CorrelativoBien).ToLower().Contains(buscador.ToLower())
                                                || (activo.Desripcion).ToLower().Contains(buscador.ToLower()))

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
        //listar bienes en manteniento
        [HttpGet]
        [Route("api/InformeMantenimiento/listarBienesMttInformejefe/{idJefe}")]
        public IEnumerable<BienesSolicitadosMttoAF> listarBienesMttInformejefe(int idJefe)
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                Empleado oempleado = bd.Empleado.Where(p => p.IdEmpleado == idJefe).FirstOrDefault();
                AreaDeNegocio oarea = bd.AreaDeNegocio.Where(p => p.IdAreaNegocio == oempleado.IdAreaDeNegocio).FirstOrDefault();
                IEnumerable<BienesSolicitadosMttoAF> lista = (from bienMtto in bd.BienMantenimiento
                                                              join activo in bd.ActivoFijo
                                                              on bienMtto.IdBien equals activo.IdBien
                                                              join solicitud in bd.SolicitudMantenimiento
                                                              on bienMtto.IdSolicitud equals solicitud.IdSolicitud
                                                              join empleado in bd.Empleado
                                                              on activo.IdResponsable equals empleado.IdEmpleado
                                                              join area in bd.AreaDeNegocio
                                                              on empleado.IdAreaDeNegocio equals area.IdAreaNegocio
                                                              where activo.EstadoActual == 3 && bienMtto.Estado == 1 && area.IdAreaNegocio == oarea.IdAreaNegocio //ELEMENTO 2 LISTA
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


                return lista;
            }

        }
        [HttpGet]
        [Route("api/Division/buscarBienesEnManteJefe/{idJefe}/{buscador?}")]
        public IEnumerable<BienesSolicitadosMttoAF> buscarBienesEnManteJefe(int idJefe,string buscador = "")
        {
            List<BienesSolicitadosMttoAF> listaBienesMantenimiento;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                Empleado oempleado = bd.Empleado.Where(p => p.IdEmpleado == idJefe).FirstOrDefault();
                AreaDeNegocio oarea = bd.AreaDeNegocio.Where(p => p.IdAreaNegocio == oempleado.IdAreaDeNegocio).FirstOrDefault();
                if (buscador == "")
                {
                    listaBienesMantenimiento = (from bienMtto in bd.BienMantenimiento
                                                join activo in bd.ActivoFijo
                                                on bienMtto.IdBien equals activo.IdBien
                                                join solicitud in bd.SolicitudMantenimiento
                                                on bienMtto.IdSolicitud equals solicitud.IdSolicitud
                                                join empleado in bd.Empleado
                                                on activo.IdResponsable equals empleado.IdEmpleado
                                                join area in bd.AreaDeNegocio
                                                on empleado.IdAreaDeNegocio equals area.IdAreaNegocio
                                                where activo.EstadoActual == 3 && bienMtto.Estado == 1 && area.IdAreaNegocio == oarea.IdAreaNegocio //ELEMENTO 2 LISTA
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
                                                join empleado in bd.Empleado
                                                on activo.IdResponsable equals empleado.IdEmpleado
                                                join area in bd.AreaDeNegocio
                                                on empleado.IdAreaDeNegocio equals area.IdAreaNegocio
                                                where activo.EstadoActual == 3 && bienMtto.Estado == 1 && area.IdAreaNegocio == oarea.IdAreaNegocio //ELEMENTO 2 LISTA
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
        //Metodo que lista los activos en el historial de mantenimiento
        [HttpGet]
        [Route("api/Division/listarActivosHistorialJefe/{idJefe}")]
        public IEnumerable<DepreciacionAF> listarActivosHistorialJefe(int idJefe)
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                Empleado oempleado = bd.Empleado.Where(p => p.IdEmpleado == idJefe).FirstOrDefault();
                AreaDeNegocio oarea = bd.AreaDeNegocio.Where(p => p.IdAreaNegocio == oempleado.IdAreaDeNegocio).FirstOrDefault();
                //Periodo anioActual = bd.Periodo.Where(p => p.Estado == 1).FirstOrDefault();
                IEnumerable<DepreciacionAF> listaActivos = (from activo in bd.ActivoFijo
                                                            join empleado in bd.Empleado
                                                            on activo.IdResponsable equals empleado.IdEmpleado
                                                            join area in bd.AreaDeNegocio
                                                            on empleado.IdAreaDeNegocio equals area.IdAreaNegocio
                                                            join sucursal in bd.Sucursal
                                                            on area.IdSucursal equals sucursal.IdSucursal
                                                            where activo.EstaAsignado == 1 && area.IdAreaNegocio == oarea.IdAreaNegocio
                                                            select new DepreciacionAF
                                                            {
                                                                idBien = activo.IdBien,
                                                                codigo = activo.CorrelativoBien,
                                                                descripcion = activo.Desripcion,
                                                                areanegocio = area.Nombre,
                                                                sucursal = sucursal.Nombre,
                                                                responsable = empleado.Nombres + " " + empleado.Apellidos
                                                            }).ToList();
                return listaActivos;
            }
        }
        //Para buscar los activos en el historial.
        [HttpGet]
        [Route("api/Division/buscarActivoHistorialJefe/{idJefe}/{buscador?}")]
        public IEnumerable<DepreciacionAF> buscarActivoHistorialJefe(int idJefe,string buscador = "")
        {
            List<DepreciacionAF> listaActivo;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                Empleado oempleado = bd.Empleado.Where(p => p.IdEmpleado == idJefe).FirstOrDefault();
                AreaDeNegocio oarea = bd.AreaDeNegocio.Where(p => p.IdAreaNegocio == oempleado.IdAreaDeNegocio).FirstOrDefault();
                if (buscador == "")
                {
                    listaActivo = (from tarjeta in bd.TarjetaDepreciacion
                                   group tarjeta by tarjeta.IdBien into bar
                                   join activo in bd.ActivoFijo
                                  on bar.FirstOrDefault().IdBien equals activo.IdBien
                                   join empleado in bd.Empleado
                                   on activo.IdResponsable equals empleado.IdEmpleado
                                   join area in bd.AreaDeNegocio
                                   on empleado.IdAreaDeNegocio equals area.IdAreaNegocio
                                   join sucursal in bd.Sucursal
                                   on area.IdSucursal equals sucursal.IdSucursal
                                   where activo.EstaAsignado == 1 && activo.EstadoActual != 0 && area.IdAreaNegocio == oarea.IdAreaNegocio

                                   select new DepreciacionAF
                                   {

                                       idBien = activo.IdBien,
                                       codigo = activo.CorrelativoBien,
                                       descripcion = activo.Desripcion,
                                       areanegocio = area.Nombre + " - " + sucursal.Nombre + " - " + sucursal.Ubicacion,
                                       sucursal = sucursal.Nombre,
                                       responsable = empleado.Nombres + " " + empleado.Apellidos

                                   }).ToList();

                    return listaActivo;
                }
                else
                {
                    listaActivo = (from tarjeta in bd.TarjetaDepreciacion
                                   group tarjeta by tarjeta.IdBien into bar
                                   join activo in bd.ActivoFijo
                                  on bar.FirstOrDefault().IdBien equals activo.IdBien
                                   join empleado in bd.Empleado
                                   on activo.IdResponsable equals empleado.IdEmpleado
                                   join area in bd.AreaDeNegocio
                                   on empleado.IdAreaDeNegocio equals area.IdAreaNegocio
                                   join sucursal in bd.Sucursal
                                   on area.IdSucursal equals sucursal.IdSucursal

                                   where activo.EstaAsignado == 1 && activo.EstadoActual != 0 && area.IdAreaNegocio == oarea.IdAreaNegocio

                                   && ((activo.CorrelativoBien).ToLower().Contains(buscador.ToLower())
                                    || (activo.Desripcion).ToLower().Contains(buscador.ToLower())
                                    || (area.Nombre).ToLower().Contains(buscador.ToLower())
                                    || (empleado.Nombres).ToLower().Contains(buscador.ToLower())
                                    || (empleado.Apellidos).ToLower().Contains(buscador.ToLower()))

                                   select new DepreciacionAF
                                   {

                                       idBien = activo.IdBien,
                                       codigo = activo.CorrelativoBien,
                                       descripcion = activo.Desripcion,
                                       areanegocio = area.Nombre + " - " + sucursal.Nombre + " - " + sucursal.Ubicacion,
                                       sucursal = sucursal.Nombre,
                                       responsable = empleado.Nombres + " " + empleado.Apellidos

                                   }).ToList();

                    return listaActivo;
                }
            }
        }
        [HttpGet]
        [Route("api/Division/listarActivosTraspasosJefe/{idJefe}")]
        public List<ActivoFijoAF> listarActivosTraspasosJefe(int idJefe)
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                Empleado oempleado = bd.Empleado.Where(p => p.IdEmpleado == idJefe).FirstOrDefault();
                AreaDeNegocio oarea = bd.AreaDeNegocio.Where(p => p.IdAreaNegocio == oempleado.IdAreaDeNegocio).FirstOrDefault();
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
                                            where activo.EstadoActual == 1 && activo.EstaAsignado == 1 && area.IdAreaNegocio == oarea.IdAreaNegocio
                                            orderby activo.CorrelativoBien
                                            select new ActivoFijoAF
                                            {
                                                IdBien = activo.IdBien,
                                                idarea = area.IdAreaNegocio,
                                                Codigo = activo.CorrelativoBien,
                                                fechacadena = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                                Desripcion = activo.Desripcion,
                                                AreaDeNegocio = area.Nombre + " - " + sucursal.Nombre + " - " + sucursal.Ubicacion,
                                                Resposnsable = resposable.Nombres + " " + resposable.Apellidos,
                                                idresponsable = resposable.IdEmpleado,
                                                cargo = cargo.Cargo,

                                            }).ToList();
                return lista;

            }
        }
        [HttpGet]
        [Route("api/Division/buscarBienesTraspasojefe/{idJefe}/{buscador?}")]
        public IEnumerable<ActivoFijoAF> buscarBienesTraspasojefe(int idJefe,string buscador = "")
        {
            List<ActivoFijoAF> lista;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                Empleado oempleado = bd.Empleado.Where(p => p.IdEmpleado == idJefe).FirstOrDefault();
                AreaDeNegocio oarea = bd.AreaDeNegocio.Where(p => p.IdAreaNegocio == oempleado.IdAreaDeNegocio).FirstOrDefault();
                if (buscador == "")
                {
                    lista = (from activo in bd.ActivoFijo
                             join noFormulario in bd.FormularioIngreso
                             on activo.NoFormulario equals noFormulario.NoFormulario
                             join resposable in bd.Empleado
                             on activo.IdResponsable equals resposable.IdEmpleado
                             join area in bd.AreaDeNegocio
                             on resposable.IdAreaDeNegocio equals area.IdAreaNegocio
                             join sucursal in bd.Sucursal
                            on area.IdSucursal equals sucursal.IdSucursal
                             where activo.EstadoActual == 1 && activo.EstaAsignado == 1 && area.IdAreaNegocio == oarea.IdAreaNegocio
                             orderby activo.CorrelativoBien
                             select new ActivoFijoAF
                             {
                                 IdBien = activo.IdBien,
                                 Codigo = activo.CorrelativoBien,
                                 idresponsable = (int)activo.IdResponsable,
                                 idarea = area.IdAreaNegocio,
                                 fechacadena = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                 AreaDeNegocio = area.Nombre + " - " + sucursal.Nombre + " - " + sucursal.Ubicacion,
                                 Desripcion = activo.Desripcion,
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
                             join sucursal in bd.Sucursal
                            on area.IdSucursal equals sucursal.IdSucursal
                             where activo.EstadoActual == 1 && activo.EstaAsignado == 1 && area.IdAreaNegocio == oarea.IdAreaNegocio

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
                                 idresponsable = (int)activo.IdResponsable,
                                 idarea = area.IdAreaNegocio,
                                 Desripcion = activo.Desripcion,
                                 fechacadena = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                 AreaDeNegocio = area.Nombre + " - " + sucursal.Nombre + " - " + sucursal.Ubicacion,
                                 Resposnsable = resposable.Nombres + " " + resposable.Apellidos,
                                 cargo = cargo.Cargo,

                             }).ToList();
                    return lista;
                }
            }
        }
        [HttpGet]
        [Route("api/Division/listarBienesBajaJefe/{idJefe}")]
        public List<ActivoFijoAF> listarBienesBajaJefe(int idJefe)
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                Empleado oempleado = bd.Empleado.Where(p => p.IdEmpleado == idJefe).FirstOrDefault();
                AreaDeNegocio oarea = bd.AreaDeNegocio.Where(p => p.IdAreaNegocio == oempleado.IdAreaDeNegocio).FirstOrDefault();
                List<ActivoFijoAF> lista = (from activo in bd.ActivoFijo
                                            join noFormulario in bd.FormularioIngreso
                                             on activo.NoFormulario equals noFormulario.NoFormulario
                                            join resposable in bd.Empleado
                                            on activo.IdResponsable equals resposable.IdEmpleado
                                            join area in bd.AreaDeNegocio
                                            on resposable.IdAreaDeNegocio equals area.IdAreaNegocio
                                            join cargo in bd.Cargos
                                            on resposable.IdCargo equals cargo.IdCargo
                                            where activo.EstadoActual == 1 && activo.EstaAsignado == 1 && area.IdAreaNegocio == oarea.IdAreaNegocio
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
        [HttpGet]
        [Route("api/Division/buscarActivosBajaJefe/{idJefe}/{buscador?}")]
        public IEnumerable<ActivoFijoAF> buscarActivosBajaJefe(int idJefe,string buscador = "")
        {
            List<ActivoFijoAF> lista;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                Empleado oempleado = bd.Empleado.Where(p => p.IdEmpleado == idJefe).FirstOrDefault();
                AreaDeNegocio oarea = bd.AreaDeNegocio.Where(p => p.IdAreaNegocio == oempleado.IdAreaDeNegocio).FirstOrDefault();
                if (buscador == "")
                {
                    lista = (from activo in bd.ActivoFijo
                             join noFormulario in bd.FormularioIngreso
                             on activo.NoFormulario equals noFormulario.NoFormulario
                             join resposable in bd.Empleado
                             on activo.IdResponsable equals resposable.IdEmpleado
                             join area in bd.AreaDeNegocio
                             on resposable.IdAreaDeNegocio equals area.IdAreaNegocio
                             where activo.EstadoActual == 1 && activo.EstaAsignado == 1 && area.IdAreaNegocio == oarea.IdAreaNegocio
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
                             where activo.EstadoActual == 1 && activo.EstaAsignado == 1 && area.IdAreaNegocio == oarea.IdAreaNegocio

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
        //LISTA LOS ACTIVOS ASIGNADOS QUE HAN SIDO DADOS DE BAJA
        [HttpGet]
        [Route("api/Division/listarhistorialBajas/{idJefe}")]
        public List<ActivoFijoAF> listarhistorialBajas(int idJefe)
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                Empleado oempleado = bd.Empleado.Where(p => p.IdEmpleado == idJefe).FirstOrDefault();
                AreaDeNegocio oarea = bd.AreaDeNegocio.Where(p => p.IdAreaNegocio == oempleado.IdAreaDeNegocio).FirstOrDefault();
                List<ActivoFijoAF> lista = (from activo in bd.ActivoFijo
                                            join noFormulario in bd.FormularioIngreso
                                             on activo.NoFormulario equals noFormulario.NoFormulario
                                            join resposable in bd.Empleado
                                            on activo.IdResponsable equals resposable.IdEmpleado
                                            join area in bd.AreaDeNegocio
                                            on resposable.IdAreaDeNegocio equals area.IdAreaNegocio
                                            join cargo in bd.Cargos
                                            on resposable.IdCargo equals cargo.IdCargo
                                            where activo.EstadoActual == 0 && activo.EstaAsignado == 1 && area.IdAreaNegocio == oarea.IdAreaNegocio
                                            orderby activo.CorrelativoBien
                                            select new ActivoFijoAF
                                            {
                                                IdBien = activo.IdBien,
                                                Codigo = activo.CorrelativoBien,
                                                fechacadena = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                                Desripcion = activo.Desripcion,
                                                AreaDeNegocio = area.Nombre,
                                                Resposnsable = resposable.Nombres + " " + resposable.Apellidos,
                                                //cargo = cargo.Cargo,

                                            }).ToList();
                return lista;

            }

        }

        [HttpGet]
        [Route("api/Division/buscarHistorialBajasJefe/{idJefe}/{buscador?}")]
        public IEnumerable<ActivoFijoAF> buscarBienesBajaAsigBajas(int idJefe,string buscador = "")
        {
            List<ActivoFijoAF> lista;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                Empleado oempleado = bd.Empleado.Where(p => p.IdEmpleado == idJefe).FirstOrDefault();
                AreaDeNegocio oarea = bd.AreaDeNegocio.Where(p => p.IdAreaNegocio == oempleado.IdAreaDeNegocio).FirstOrDefault();
                if (buscador == "")
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
                             where activo.EstadoActual == 0 && activo.EstaAsignado == 1 && area.IdAreaNegocio == oarea.IdAreaNegocio
                             orderby activo.CorrelativoBien
                             select new ActivoFijoAF
                             {
                                 IdBien = activo.IdBien,
                                 Codigo = activo.CorrelativoBien,
                                 fechacadena = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                 Desripcion = activo.Desripcion,
                                 AreaDeNegocio = area.Nombre,
                                 Resposnsable = resposable.Nombres + " " + resposable.Apellidos,
                                 //cargo = cargo.Cargo,

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
                             where activo.EstadoActual == 0 && activo.EstaAsignado == 1 && area.IdAreaNegocio == oarea.IdAreaNegocio

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
                                 fechacadena = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                 Desripcion = activo.Desripcion,
                                 AreaDeNegocio = area.Nombre,
                                 Resposnsable = resposable.Nombres + " " + resposable.Apellidos,
                                 //cargo = cargo.Cargo,

                             }).ToList();
                    return lista;
                }
            }
        }
    }
}
