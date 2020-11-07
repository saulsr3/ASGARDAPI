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

        //Metodo que lista los bienes en la tabla depreciación, validados si estan ya depreciados en el periodo actual.
        [HttpGet]
        [Route("api/Revalorizar/listarActivosRevalorizar")]
        public IEnumerable<DepreciacionAF> listarActivosRevalorizar()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                Periodo anioActual = bd.Periodo.Where(p => p.Estado == 1).FirstOrDefault();
                IEnumerable<DepreciacionAF> listaActivos = (from tarjeta in bd.TarjetaDepreciacion
                                                            group tarjeta by tarjeta.IdBien into bar
                                                            join activo in bd.ActivoFijo
                                                           on bar.FirstOrDefault().IdBien equals activo.IdBien
                                                            join empleado in bd.Empleado
                                                            on activo.IdResponsable equals empleado.IdEmpleado
                                                            join area in bd.AreaDeNegocio
                                                            on empleado.IdAreaDeNegocio equals area.IdAreaNegocio
                                                            join sucursal in bd.Sucursal
                                                            on area.IdSucursal equals sucursal.IdSucursal
                                                          //  where activo.EstaAsignado==1 && activo.TipoActivo==2 && activo.TipoActivo==3 && activo.TipoActivo == 1
                                                             where (activo.EstadoActual != 0) && (activo.UltimoAnioDepreciacion == null || (activo.UltimoAnioDepreciacion < (anioActual.Anio))) && (bar.OrderByDescending(x => x.IdTarjeta).First().ValorActual > 0)
                                                            select new DepreciacionAF
                                                            {
                                                                idBien = activo.IdBien,
                                                                codigo = activo.CorrelativoBien,
                                                                descripcion = activo.Desripcion,
                                                                areanegocio = area.Nombre,
                                                                sucursal = sucursal.Nombre,
                                                                vidautil = activo.VidaUtil,
                                                                responsable = empleado.Nombres + " " + empleado.Apellidos


                                                            }).ToList();
                return listaActivos;
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
                                                        //where activo.EstaAsignado == 1 && activo.TipoActivo == 1 
                                                        where (activo.EstadoActual != 0) && (activo.UltimoAnioDepreciacion == null || (activo.UltimoAnioDepreciacion < (anioActual.Anio))) && activo.TipoActivo == 1 && (bar.OrderByDescending(x => x.IdTarjeta).First().ValorActual > 0)
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
                                                        where (activo.EstadoActual != 0) && (activo.UltimoAnioDepreciacion == null || (activo.UltimoAnioDepreciacion < (anioActual.Anio))) && activo.TipoActivo == 3 && (bar.OrderByDescending(x => x.IdTarjeta).First().ValorActual > 0)
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

    }
}