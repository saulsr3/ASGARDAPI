using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ASGARDAPI.Models;
using ASGARDAPI.Clases;

namespace ASGARDAPI.Controllers
{
    public class ActivoFijoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        [Route("api/ActivoFIjo/listarActivosNoAsignados")]
        public IEnumerable<ActivoFijoAF> listarActivosNoAsignados()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<ActivoFijoAF> lista = (from activo in bd.ActivoFijo
                                                   join noFormulario in bd.FormularioIngreso
                                                   on activo.NoFormulario equals noFormulario.NoFormulario
                                                   join clasif in bd.Clasificacion
                                                   on activo.IdClasificacion equals clasif.IdClasificacion
                                                   join marca in bd.Marcas
                                                   on activo.IdMarca equals  marca.IdMarca
                                                   //where activo.EstadoActual == 1 && activo.EstaAsignado==0
                                                   //orderby noFormulario.NoFormulario
                                                           select new ActivoFijoAF
                                                           {
                                                               IdBien=activo.IdBien,
                                                               NoFormulario=noFormulario.NoFormulario,
                                                               FechaIngreso=noFormulario.FechaIngreso,
                                                               Desripcion=activo.Desripcion,
                                                               Clasificacion=clasif.Clasificacion1,
                                                               Marca=marca.Marca
                                                           }).ToList();
                return lista;
            }
        }
    }
}