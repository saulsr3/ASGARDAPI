using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ASGARDAPI.Clases;
using ASGARDAPI.Models;

namespace ASGARDAPI.Controllers
{
    public class BienesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("api/Bienes/listarBienes")]
        public IEnumerable<BienesAF> listarBienes()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<BienesAF> listaBienes = (from bienes in bd.ActivoFijo
                                                     join formu in bd.FormularioIngreso
                                                     on bienes.NoFormulario equals formu.NoFormulario
                                                     join clasi in bd.Clasificacion
                                                     on bienes.IdClasificacion equals clasi.IdClasificacion
                                                     //join emp in bd.Empleado
                                                     //on bienes.IdResponsable equals emp.IdEmpleado
                                                     where bienes.EstadoActual==1
                                                     select new BienesAF
                                                               {
                                                                  idbien= bienes.IdBien,
                                                                  descripcion= bienes.Desripcion,
                                                                  fechacadena = formu.FechaIngreso == null ? " " : ((DateTime)formu.FechaIngreso).ToString("dd-MM-yyyy"),
                                                                  tipoactivo =clasi.Clasificacion1,
                                                                  correlativobien= bienes.CorrelativoBien,
                                                                  modelo = bienes.Modelo,
                                                                  color = bienes.Color
                                                                  //responsable = emp.Nombres + " " + emp.Apellidos

                                                     }).ToList();
                return listaBienes;
            }
        }


        [HttpGet]
        [Route("api/Bienes/RecuperarBienes/{id}")]
        public BienesAF RecuperarBienes(int id)
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                BienesAF oBienesAF = new BienesAF();
                ActivoFijo oBienes = bd.ActivoFijo.Where(p => p.IdBien == id).First();
                oBienesAF.idbien = oBienes.IdBien;
                oBienesAF.descripcion = oBienes.Desripcion;
                oBienesAF.numformulario =(int) oBienes.NoFormulario;
                oBienesAF.idclasificacion= (int)oBienes.IdClasificacion;
                oBienesAF.color = oBienes.Color;
                oBienesAF.modelo = oBienes.Modelo;

                return oBienesAF;
            }
        }


    }
}