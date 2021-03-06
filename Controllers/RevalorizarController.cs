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
    public class RevalorizarController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        

      
        [HttpGet]
        [Route("api/Revalorizar/listarActivosRevalorizar")]
        public IEnumerable<RegistroAF> listarActivosRevalorizar()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                Periodo anioActual = bd.Periodo.Where(p => p.Estado == 1).FirstOrDefault();
                //IEnumerable<DepreciacionAF> listaActivos = (Periodo anioActual = bd.Periodo.Where(p => p.Estado == 1).FirstOrDefault();
                IEnumerable<RegistroAF> listaActivos = (from tarjeta in bd.TarjetaDepreciacion
                                                        group tarjeta by tarjeta.IdBien into bar
                                                        join activo in bd.ActivoFijo
                                                       on bar.FirstOrDefault().IdBien equals activo.IdBien
                                                        join noFormulario in bd.FormularioIngreso
                                                        on activo.NoFormulario equals noFormulario.NoFormulario
                                                        join clasif in bd.Clasificacion
                                                        on activo.IdClasificacion equals clasif.IdClasificacion
                                                    //  where (activo.EstadoActual != 0) && activo.TipoActivo == 2 && (activo.UltimoAnioDepreciacion == null || (activo.UltimoAnioDepreciacion < (anioActual.Anio)))
                                                        where (activo.EstadoActual != 0 && activo.EstaAsignado == 1) && activo.TipoActivo == 2 && (activo.UltimoAnioDepreciacion == null || (activo.UltimoAnioDepreciacion < (anioActual.Anio)))
                                                        orderby activo.CorrelativoBien
                                                        select new RegistroAF
                                                        {
                                                            IdBien = activo.IdBien,
                                                            Codigo = activo.CorrelativoBien,
                                                            fechacadena = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                                            Descripcion = activo.Desripcion,
                                                            vidautil = activo.VidaUtil,
                                                            Clasificacion = clasif.Clasificacion1,


                                                        }).ToList();
                return listaActivos;
               

            }
        }

        [HttpGet]
        [Route("api/Revalorizar/VidaUtilRevalorizar/{idbien}")]
        public AsignacionAF VidaUtilRevalorizar(int idbien)
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                AsignacionAF oVidaUtil = new AsignacionAF();
                ActivoFijo oactivo = bd.ActivoFijo.Where(p => p.IdBien == idbien).First();
                Clasificacion oClas = bd.Clasificacion.Where(p => p.IdClasificacion == oactivo.IdClasificacion).First();
                Categorias oCategoria = bd.Categorias.Where(p => p.IdCategoria == oClas.IdCategoria).First();
                oVidaUtil.vidaUtil = (int)oCategoria.VidaUtil;
                oVidaUtil.realvidautil=(int) oactivo.VidaUtil;
                return oVidaUtil;
            }
        }


        //VALIDACION DE TABLA VACIÍA
        [HttpGet]
        [Route("api/Revalorizar/validarActivosRevalorizar")]
        public int validarActivosRevalorizar()
        {
            int rpta = 0;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
               
            {
                Periodo anioActual = bd.Periodo.Where(p => p.Estado == 1).FirstOrDefault();
                IEnumerable<RegistroAF> lista = (from activo in bd.ActivoFijo
                                                 where (activo.EstadoActual != 0) && (activo.UltimoAnioDepreciacion == null || (activo.UltimoAnioDepreciacion < (anioActual.Anio))) && activo.EstaAsignado == 1

                                                 // where (activo.EstadoActual != 0 && activo.EstaAsignado == 1) && activo.TipoActivo == 1 && activo.TipoActivo == 2 &&  activo.TipoActivo == 3 && (activo.UltimoAnioDepreciacion == null || (activo.UltimoAnioDepreciacion < (anioActual.Anio)))
                                                 select new RegistroAF
                                                    {
                                                        IdBien = activo.IdBien,
                                                        Codigo = activo.CorrelativoBien,                                            
                                                        Descripcion = activo.Desripcion,
                                                        vidautil = activo.VidaUtil                                                     
                                                    }).ToList();
                if (lista.Count() > 0)
                {
                    rpta = 1;
                }
                return rpta;
            }
        }

        [HttpGet]
        [Route("api/Revalorizar/listarActivosEdificiosRevalorizar")]
        public IEnumerable<RegistroAF> listarActivosEdificiosRevalorizar()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {

                Periodo anioActual = bd.Periodo.Where(p => p.Estado == 1).FirstOrDefault();
                IEnumerable<RegistroAF> listaActivos = (from tarjeta in bd.TarjetaDepreciacion
                                                        group tarjeta by tarjeta.IdBien into bar
                                                        join activo in bd.ActivoFijo
                                                       on bar.FirstOrDefault().IdBien equals activo.IdBien
                                                        join noFormulario in bd.FormularioIngreso
                                                        on activo.NoFormulario equals noFormulario.NoFormulario
                                                        join clasif in bd.Clasificacion
                                                        on activo.IdClasificacion equals clasif.IdClasificacion
                                                        where (activo.EstadoActual != 0) && activo.TipoActivo == 1 && (activo.UltimoAnioDepreciacion == null || (activo.UltimoAnioDepreciacion < (anioActual.Anio)))
                                                        orderby activo.CorrelativoBien
                                                        select new RegistroAF
                                                        {
                                                            IdBien = activo.IdBien,
                                                            Codigo = activo.CorrelativoBien,
                                                            fechacadena = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                                            Descripcion = activo.Desripcion,
                                                            vidautil = activo.VidaUtil,
                                                            Clasificacion = clasif.Clasificacion1,
                                                        }).ToList();
                return listaActivos;
            }
        }
        [HttpGet]
        [Route("api/Revalorizar/listarActivosIntangiblesRevalorizar")]
        public IEnumerable<RegistroAF> listarActivosIntangiblesRevalorizar()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {

                Periodo anioActual = bd.Periodo.Where(p => p.Estado == 1).FirstOrDefault();
                IEnumerable<RegistroAF> listaActivos = (from tarjeta in bd.TarjetaDepreciacion
                                                        group tarjeta by tarjeta.IdBien into bar
                                                        join activo in bd.ActivoFijo
                                                       on bar.FirstOrDefault().IdBien equals activo.IdBien
                                                        join noFormulario in bd.FormularioIngreso
                                                        on activo.NoFormulario equals noFormulario.NoFormulario
                                                        join clasif in bd.Clasificacion
                                                        on activo.IdClasificacion equals clasif.IdClasificacion
                                                        where (activo.EstadoActual != 0) && activo.TipoActivo == 3 && (activo.UltimoAnioDepreciacion == null || (activo.UltimoAnioDepreciacion < (anioActual.Anio)))
                                                        orderby activo.CorrelativoBien
                                                        select new RegistroAF
                                                        {
                                                            IdBien = activo.IdBien,
                                                            Codigo = activo.CorrelativoBien,
                                                            fechacadena = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                                            Descripcion = activo.Desripcion,
                                                            vidautil = activo.VidaUtil,
                                                            Clasificacion = clasif.Clasificacion1,
                                                        }).ToList();
                return listaActivos;
            }
        }

        //LISTAR ACTIVOS PARA FILTRO
        [HttpGet]
        [Route("api/Revalorizar/listarActivosFiltroRev/{id}")]
        public IEnumerable<RegistroAF> listarActivosFiltroRev(int id)
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
                                                     vidautil=activo.VidaUtil,
                                                     Responsable = resposable.Nombres + " " + resposable.Apellidos
                                                 }).ToList();
                return lista;

            }
        }



        //BUSCAR ACTIVOS ASIGNADOS

        [HttpGet]
        [Route("api/Revalorizar/buscarActivoRevalorizar/{buscador?}")]
        public IEnumerable<RegistroAF> buscarActivoRevalorizar(string buscador = "")
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
                                   join resposable in bd.Empleado
                                   on activo.IdResponsable equals resposable.IdEmpleado
                                   join area in bd.AreaDeNegocio
                                   on resposable.IdAreaDeNegocio equals area.IdAreaNegocio
                                   where (activo.EstadoActual == 1 || activo.EstadoActual == 2 || activo.EstadoActual == 3) && activo.EstaAsignado == 1

                                   orderby activo.CorrelativoBien
                                   select new RegistroAF
                                   {
                                       IdBien = activo.IdBien,
                                       Codigo = activo.CorrelativoBien,
                                       fechacadena = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                       Descripcion = activo.Desripcion,
                                       Clasificacion = clasif.Clasificacion1,
                                       AreaDeNegocio = area.Nombre,
                                       vidautil = activo.VidaUtil,
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
                                   where (activo.EstadoActual == 1 || activo.EstadoActual == 2 || activo.EstadoActual == 3) && activo.EstaAsignado == 1


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
                                       vidautil = activo.VidaUtil,
                                       Responsable = resposable.Nombres + " " + resposable.Apellidos
                                   }).ToList();

                    return listaActivo;
                }
            }
        }

        //BUSCAR EDIFICIOS
        [HttpGet]
        [Route("api/Revalorizar/buscarEdificiosRevalorizar/{buscador?}")]
        public IEnumerable<RegistroAF> buscarEdificiosRevalorizar(string buscador = "")
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
                                   where (activo.EstadoActual == 1 || activo.EstadoActual == 2 || activo.EstadoActual == 3) && activo.TipoActivo == 1
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
                                   where (activo.EstadoActual == 1 || activo.EstadoActual == 2 || activo.EstadoActual == 3) && activo.TipoActivo == 1


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
        //PARA EL COMBO DE FILTRO EN AREA 
        [HttpGet]
        [Route("api/Revalorizar/comboAreaDeSucursal/{id}")]
        public IEnumerable<AreasDeNegocioAF> comboAreaDeSucursal(int id)
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<AreasDeNegocioAF> lista = (from area in bd.AreaDeNegocio
                                                       where area.Dhabilitado == 1 && area.IdSucursal == id
                                                       orderby area.Nombre
                                                       select new AreasDeNegocioAF
                                                       {
                                                           IdAreaNegocio = area.IdAreaNegocio,
                                                           Nombre = area.Nombre
                                                       }).ToList();
                return lista;
            }
        }

        //BUSCAR ACTIVOS INTANGIBLES
        [HttpGet]
        [Route("api/Revalorizar/buscarActivoIntangibleRevalorizar/{buscador?}")]
        public IEnumerable<RegistroAF> buscarActivoIntangibleRevalorizar(string buscador = "")
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

        //PARA VER SI HAY ACTIVOS QUE REVALORIZAR
        [HttpGet]
        [Route("api/Revalorizar/ValidarActivosARevalorizar")]
        public int ValidarActivosADepreciar()
        {
            int rpta = 0;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    Periodo anioActual = bd.Periodo.Where(p => p.Estado == 1).FirstOrDefault();
                    List<ComboAnidadoAF> lista = (from tarjeta in bd.TarjetaDepreciacion
                                                  group tarjeta by tarjeta.IdBien into bar
                                                  join activo in bd.ActivoFijo
                                                 on bar.FirstOrDefault().IdBien equals activo.IdBien
                                                 // where (activo.EstadoActual != 0) && activo.TipoActivo == 2 && (activo.UltimoAnioDepreciacion == null || (activo.UltimoAnioDepreciacion < (anioActual.Anio))) && activo.EstaAsignado == 1
                                                  where (activo.EstadoActual != 0) && (activo.UltimoAnioDepreciacion == null || (activo.UltimoAnioDepreciacion < (anioActual.Anio))) && (bar.OrderByDescending(x => x.IdTarjeta).First().ValorActual > activo.ValorResidual) && activo.EstaAsignado == 1
                                                 // where (activo.EstadoActual != 0) && (activo.UltimoAnioDepreciacion == null || (activo.UltimoAnioDepreciacion < (anioActual.Anio))) && (bar.OrderByDescending(x => x.IdTarjeta).First().ValorActual > activo.ValorResidual) && activo.EstaAsignado == 1
                                                  select new ComboAnidadoAF
                                                  {
                                                      id = activo.IdBien,
                                                      nombre = activo.CorrelativoBien

                                                  }).ToList();
                    if (lista.Count() > 0)
                    {
                        rpta = 1;
                    }
                }
            }
            catch (Exception ex)
            {

                rpta = 0;
                // Console.WriteLine("prueba");
            }
            return rpta;

        }

    }
}