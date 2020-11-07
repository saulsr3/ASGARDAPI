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
                List<RegistroAF> lista = (from activo in bd.ActivoFijo
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
                                              vidautil = activo.VidaUtil,
                                              AreaDeNegocio = area.Nombre,
                                              Responsable = resposable.Nombres + " " + resposable.Apellidos
                                          }).ToList();
                return lista;
            }
        }

        [HttpGet]
        [Route("api/Revalorizar/listarActivosEdificiosRevalorizar")]
        public IEnumerable<RegistroAF> listarActivosEdificiosRevalorizar()
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
                                              vidautil = activo.VidaUtil,
                                              Clasificacion = clasif.Clasificacion1,
                                          }).ToList();
                return lista;
            }
        }
        [HttpGet]
        [Route("api/Revalorizar/listarActivosIntangiblesRevalorizar")]
        public IEnumerable<RegistroAF> listarActivosIntangiblesRevalorizar()
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
                                              vidautil = activo.VidaUtil,
                                              Clasificacion = clasif.Clasificacion1,
                                          }).ToList();
                return lista;
            }
        }

    }
}