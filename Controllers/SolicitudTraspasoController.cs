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
    public class SolicitudTraspasoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        //LISTAR EL AREA DE NEGOCIO CON LA SUCURSAL Y SU UBICACIÓN EN UN COMBO.
        [HttpGet]
        [Route("api/SolicitudTraspaso/listarAreaCombo")]
        public IEnumerable<AreasDeNegocioAF> listarAreaCombo()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<AreasDeNegocioAF> listarAreas = (from sucursal in bd.Sucursal
                                                             join area in bd.AreaDeNegocio
                                                             on sucursal.IdSucursal equals area.IdSucursal
                                                             where area.Dhabilitado == 1
                                                             select new AreasDeNegocioAF
                                                             {
                                                                 IdAreaNegocio = area.IdAreaNegocio,
                                                                 Nombre = area.Nombre,
                                                                 IdSucursal = sucursal.IdSucursal,
                                                                 nombreSucursal = sucursal.Nombre,
                                                                 ubicacion = sucursal.Ubicacion


                                                             }).ToList();


                return listarAreas;

            }
        }

        //LISTAR LOS EMPLEADOS EN UN COMBO.
        [HttpGet]
        [Route("api/SolicitudTraspaso/listarEmpleadosCombo")]
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

        //LISTAR

    }
}