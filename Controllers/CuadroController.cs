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
                                                            join tarjeta in bd.TarjetaDepreciacion
                                                            on cuadro.IdBien equals tarjeta.IdBien
                                                            where cuadro.EstadoActual == 1 && cuadro.EstaAsignado == 1

                                                            select new CuadroControlAF
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

    }
}
