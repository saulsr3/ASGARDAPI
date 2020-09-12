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
                
                
                IEnumerable<CuadroControlAF> listacuadro = (from cuadro in bd.ActivoFijo
                                                            join noFormulario in bd.FormularioIngreso
                                                            on cuadro.NoFormulario equals noFormulario.NoFormulario
                                                            join clasif in bd.Clasificacion
                                                            on cuadro.IdClasificacion equals clasif.IdClasificacion
                                                            join resposable in bd.Empleado
                                                            on cuadro.IdResponsable equals resposable.IdEmpleado
                                                            join area in bd.AreaDeNegocio
                                                            on resposable.IdAreaDeNegocio equals area.IdAreaNegocio
                                                            //Aca probando con el order by
                                                            join tarjeta in bd.TarjetaDepreciacion.OrderBy(valor=>valor.DepreciacionAnual)
                                                            on cuadro.IdBien equals tarjeta.IdBien
                                              
                                                            where cuadro.EstadoActual == 1 && cuadro.EstaAsignado == 1

                                                            //Así es como me da error...
                                                        //    group tarjeta by v
                                                            
                                                            select new CuadroControlAF()
                                                            {
                                                               

                                                                idbien = cuadro.IdBien,
                                                                codigo = cuadro.CorrelativoBien,
                                                                descripcion = cuadro.Desripcion,
                                                                valoradquisicion = (double)cuadro.ValorAdquicicion,
                                                                valoractual = (double)tarjeta.ValorActual,
                                                                depreciacion = (double)tarjeta.DepreciacionAnual,
                                                                ubicacion = area.Nombre,
                                                                depreciacionacumulada =(double)tarjeta.DepreciacionAcumulada,
                                                                fechaadquisicion = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
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
                    listaCuadro = (from cuadro in bd.ActivoFijo
                                   join noFormulario in bd.FormularioIngreso
                                   on cuadro.NoFormulario equals noFormulario.NoFormulario
                                   join tarjeta in bd.TarjetaDepreciacion
                                   on cuadro.IdBien equals tarjeta.IdBien
                                   join resposable in bd.Empleado
                                   on cuadro.IdResponsable equals resposable.IdEmpleado
                                   join area in bd.AreaDeNegocio
                                   on resposable.IdAreaDeNegocio equals area.IdAreaNegocio
                                   where cuadro.EstadoActual == 1 && cuadro.EstaAsignado == 1
                                   select new CuadroControlAF
                                   {
                                      
                                       codigo = cuadro.CorrelativoBien,
                                       descripcion = cuadro.Desripcion,
                                       fechaadquisicion = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                       valoradquisicion = (double)cuadro.ValorAdquicicion,
                                       depreciacion = (double)tarjeta.DepreciacionAnual,
                                       depreciacionacumulada = (double)tarjeta.DepreciacionAcumulada,
                                       valoractual = (double)tarjeta.ValorActual,
                                       ubicacion = area.Nombre,
                                       responsable = resposable.Nombres + " " + resposable.Apellidos

                                   }).ToList();

                    return listaCuadro;
                }
                else
                {
                    listaCuadro = (from cuadro in bd.ActivoFijo
                                   join noFormulario in bd.FormularioIngreso
                                   on cuadro.NoFormulario equals noFormulario.NoFormulario
                                   join tarjeta in bd.TarjetaDepreciacion
                                   on cuadro.IdBien equals tarjeta.IdBien
                                   join resposable in bd.Empleado
                                   on cuadro.IdResponsable equals resposable.IdEmpleado
                                   join area in bd.AreaDeNegocio
                                   on resposable.IdAreaDeNegocio equals area.IdAreaNegocio
                                   where cuadro.EstadoActual == 1 && cuadro.EstaAsignado == 1

                                 && ((cuadro.IdBien).ToString().Contains(buscador) || (cuadro.CorrelativoBien).ToLower().Contains(buscador.ToLower()) || (cuadro.Desripcion).ToLower().Contains(buscador.ToLower()))
                                   select new CuadroControlAF
                                   {
                                       codigo = cuadro.CorrelativoBien,
                                       descripcion = cuadro.Desripcion,
                                       fechaadquisicion = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                       valoradquisicion = (double)cuadro.ValorAdquicicion,
                                       depreciacion = (double)tarjeta.DepreciacionAnual,
                                       depreciacionacumulada = (double)tarjeta.DepreciacionAcumulada,
                                       valoractual = (double)tarjeta.ValorActual,
                                       ubicacion = area.Nombre,
                                       responsable = resposable.Nombres + " " + resposable.Apellidos

                                   }).ToList();
                    return listaCuadro;
                }
            }
        }

    }
}
