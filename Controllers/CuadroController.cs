using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ASGARDAPI.Clases;
using ASGARDAPI.Models;

namespace ASGARDAPI.Controllers
{
    public class CuadroController : Controller
    {
     
        public IActionResult Index()
        {
            return View();
        }

        //Metodo listar cuadro de control
        [HttpGet]
        [Route("api/CuadroControl/listarCuadroControl")]
        public IEnumerable<CuadroControlAF> listarCuadroControl()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {


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

                                                              //where cuadro.EstadoActual == 1 && cuadro.EstaAsignado == 1
                                                              select new CuadroControlAF()
                                                            {
                                                                idbien = cuadro.IdBien,
                                                                codigo = cuadro.CorrelativoBien,
                                                                descripcion = cuadro.Desripcion,
                                                                valoradquisicion = (double)cuadro.ValorAdquicicion,
                                                                fechaadquisicion = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                                                valoractual =  (double)bar.OrderByDescending(x => x.IdTarjeta).First().ValorActual,
                                                                depreciacion = (double)bar.OrderByDescending(x=>x.IdTarjeta).First().DepreciacionAnual,
                                                                depreciacionacumulada = (double)bar.Sum(x=>x.DepreciacionAnual),
                                                                ubicacion = area.Nombre,
                                                                responsable = resposable.Nombres + " " + resposable.Apellidos

                                                            }).ToList();
                return listacuadro;

            }
        }

        //Método buscar
        [HttpGet]
        [Route("api/CuadroControl/buscarCuadro/{buscador?}")]
        public IEnumerable<CuadroControlAF> buscarCuadro(string buscador = "")
        {
            List<CuadroControlAF> listaCuadro;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
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

    }
}
