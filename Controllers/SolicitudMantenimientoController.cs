using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ASGARDAPI.Models;
using ASGARDAPI.Clases;

namespace ASGARDAPI.Controllers
{
    public class SolicitudMantenimientoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        [Route("api/SolicitudMantenimiento/listarEmpleadosCombo")]
        public IEnumerable<EmpleadoAF> listarEmpleadosCombo()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {

                IEnumerable<EmpleadoAF> lista = (from empleado in bd.Empleado
                                                 where empleado.Dhabilitado == 1
                                                 select new EmpleadoAF
                                                 {
                                                     idempleado = empleado.IdEmpleado,
                                                     nombres = empleado.Nombres,
                                                     apellidos = empleado.Apellidos
                                                 }).ToList();
                return lista;
            }
        }
        [HttpGet]
        [Route("api/SolicitudMantenimiento/listarAreaCombo")]
        public IEnumerable<AreasDeNegocioAF> listarAreaCombo()
        {
            using (BDAcaassAFContext bd= new BDAcaassAFContext())
            {
                IEnumerable<AreasDeNegocioAF> lista = (from area in bd.AreaDeNegocio
                                                       join sucursal in bd.Sucursal
                                                       on area.IdSucursal equals sucursal.IdSucursal
                                                       where area.Dhabilitado == 1
                                                       select new AreasDeNegocioAF
                                                       {
                                                           IdAreaNegocio = area.IdAreaNegocio,
                                                           Nombre = area.Nombre,
                                                           nombreSucursal = sucursal.Nombre
                                                       }).ToList();
                return lista;
            }

        }
    }
}