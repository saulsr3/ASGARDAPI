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
    }
}
