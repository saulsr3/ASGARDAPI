using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASGARDAPI.Models;
using Microsoft.AspNetCore.Mvc;
using ASGARDAPI.Clases;

namespace ASGARDAPI.Controllers
{
    public class EmpleadoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("api/Empleado/guardarEmpleado")]
        public int guardarEmpleado([FromBody]EmpleadoAF oEmpleadoAF)
        {
            int respuesta = 0;

            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    Empleado oEmpleado = new Empleado();

                    oEmpleado.Dui = oEmpleadoAF.dui;
                    oEmpleado.Nombres = oEmpleadoAF.nombres;
                    oEmpleado.Apellidos = oEmpleadoAF.apellidos;
                    oEmpleado.Direccion = oEmpleadoAF.direccion;
                    oEmpleado.Telefono = oEmpleadoAF.telefono;
                    oEmpleado.TelefonoPersonal = oEmpleadoAF.telefonopersonal;
                    oEmpleado.IdAreaDeNegocio = oEmpleadoAF.idareadenegocio;
                    oEmpleado.IdCargo = oEmpleadoAF.idcargo;
                    oEmpleado.Dhabilitado = 1;
                    bd.Empleado.Add(oEmpleado);
                    bd.SaveChanges();
                    respuesta = 1;

                }

            }
            catch (Exception ex)
            {
                respuesta = 0;
            }

            return respuesta;
        }

        [HttpGet]
        [Route("api/Empleado/listarCargoCombo")]
        public IEnumerable<CargoAF> listarCargoCombo()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<CargoAF> listarCargos = (from cargo in bd.Cargos
                                                         where cargo.Dhabilitado==1
                                                         select new CargoAF
                                                         {
                                                             idcargo=cargo.IdCargo,
                                                             cargo=cargo.Cargo
                                                         }).ToList();



                return listarCargos;

            }
        }
    }
}