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

        [HttpGet]
        [Route("api/Empleado/listarEmpleado")]
        public IEnumerable<EmpleadoAF> listarEmpleado()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<EmpleadoAF> listaEmpleado = (from sucursal in bd.Sucursal
                                                         join area in bd.AreaDeNegocio
                                                         on sucursal.IdSucursal equals area.IdSucursal
                                                         join empleado in bd.Empleado
                                                         on  area.IdAreaNegocio equals empleado.IdAreaDeNegocio
                                                         join cargos in bd.Cargos
                                                         on empleado.IdCargo equals cargos.IdCargo
                                                         where empleado.Dhabilitado == 1
                                                         select new EmpleadoAF
                                                                   {
                                                                       idempleado=empleado.IdEmpleado,
                                                                       dui= empleado.Dui,
                                                                       nombres=empleado.Nombres,
                                                                       apellidos=empleado.Apellidos,
                                                                       direccion=empleado.Direccion,
                                                                       telefono=empleado.Telefono,
                                                                       telefonopersonal=empleado.TelefonoPersonal,
                                                                       nombrearea = area.Nombre,
                                                                       nombresucursal= sucursal.Nombre,
                                                                       ubicacion = sucursal.Ubicacion,
                                                                       cargo= cargos.Cargo
                               
                                                                   }).ToList();
                return listaEmpleado;
            }
        }

        [HttpGet]
        [Route("api/Empleado/RecuperarEmpleado/{idempleado}")]
        public EmpleadoAF RecuperarEmpleado(int idempleado)
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                EmpleadoAF oEmpleadoAF = new EmpleadoAF();
                Empleado oEmpleado = bd.Empleado.Where(p => p.IdEmpleado == idempleado).First();
                oEmpleadoAF.idempleado = oEmpleado.IdEmpleado;
                oEmpleadoAF.dui = oEmpleado.Dui;
                oEmpleadoAF.nombres = oEmpleado.Nombres;
                oEmpleadoAF.apellidos = oEmpleado.Apellidos;
                oEmpleadoAF.direccion = oEmpleado.Direccion;
                oEmpleadoAF.telefono = oEmpleado.Telefono;
                oEmpleadoAF.telefonopersonal = oEmpleado.TelefonoPersonal;
                oEmpleadoAF.idcargo = (int)oEmpleado.IdCargo;
                oEmpleadoAF.idareadenegocio = (int)oEmpleado.IdAreaDeNegocio;


                return oEmpleadoAF;
            }
        }



        [HttpPost]
        [Route("api/Empleado/modificarEmpleado")]
        public int modificarEmpleado([FromBody]EmpleadoAF oEmpleadoAF)
        {
            int respuesta = 0;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    Empleado oEmpleado = bd.Empleado.Where(p => p.IdEmpleado == oEmpleadoAF.idempleado).First();
                    oEmpleado.IdEmpleado = oEmpleadoAF.idempleado;
                    oEmpleado.Dui = oEmpleadoAF.dui;
                    oEmpleado.Nombres = oEmpleadoAF.nombres;
                    oEmpleado.Apellidos = oEmpleadoAF.apellidos;
                    oEmpleado.Direccion = oEmpleadoAF.direccion;
                    oEmpleado.Telefono = oEmpleadoAF.telefono;
                    oEmpleado.TelefonoPersonal = oEmpleadoAF.telefonopersonal;
                    oEmpleado.IdAreaDeNegocio = oEmpleadoAF.idareadenegocio;
                    oEmpleado.IdCargo = oEmpleadoAF.idcargo;
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
                    oEmpleado.IdEmpleado = oEmpleadoAF.idempleado;
                    oEmpleado.Dui = oEmpleadoAF.dui;
                    oEmpleado.Nombres = oEmpleadoAF.nombres;
                    oEmpleado.Apellidos = oEmpleadoAF.apellidos;
                    oEmpleado.Direccion = oEmpleadoAF.direccion;
                    oEmpleado.Telefono = oEmpleadoAF.telefono;
                    oEmpleado.TelefonoPersonal = oEmpleadoAF.telefonopersonal;
                    oEmpleado.IdAreaDeNegocio = oEmpleadoAF.idareadenegocio;
                    oEmpleado.IdCargo = oEmpleadoAF.idcargo;
                    oEmpleado.Dhabilitado = 1;
                    oEmpleado.BtieneUsuario = 0;
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
        [Route("api/Empleado/buscarEmpleado/{buscador?}")]
        public IEnumerable<EmpleadoAF> buscarEmpleado(string buscador = "")
        {
            List<EmpleadoAF> listaEmpleado;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                if (buscador == "")
                {
                    listaEmpleado = (from sucursal in bd.Sucursal
                                     join area in bd.AreaDeNegocio
                                     on sucursal.IdSucursal equals area.IdSucursal
                                     join empleado in bd.Empleado
                                     on area.IdAreaNegocio equals empleado.IdAreaDeNegocio
                                     join cargos in bd.Cargos
                                     on empleado.IdCargo equals cargos.IdCargo
                                     where empleado.Dhabilitado == 1
                                     select new EmpleadoAF
                                     { 
                                         
                                         idempleado=empleado.IdEmpleado,
                                         dui = empleado.Dui,
                                         nombres = empleado.Nombres,
                                         apellidos = empleado.Apellidos,
                                         direccion = empleado.Direccion,
                                         telefono = empleado.Telefono,
                                         telefonopersonal = empleado.TelefonoPersonal,
                                         nombrearea = area.Nombre,
                                         nombresucursal= sucursal.Nombre,
                                         ubicacion= sucursal.Ubicacion,
                                         cargo = cargos.Cargo
                                         


                                     }).ToList();
                    return listaEmpleado;
                }
                else
                {
                    listaEmpleado = (from sucursal in bd.Sucursal
                                     join area in bd.AreaDeNegocio
                                     on sucursal.IdSucursal equals area.IdSucursal
                                     join empleado in bd.Empleado
                                     on area.IdAreaNegocio equals empleado.IdAreaDeNegocio
                                     join cargos in bd.Cargos
                                     on empleado.IdCargo equals cargos.IdCargo
                                     where empleado.Dhabilitado == 1
                                  

                                     &&((empleado.IdEmpleado).ToString().Contains(buscador.ToLower()) ||
                                     (empleado.Dui).ToLower().Contains(buscador.ToLower()) ||
                                     (empleado.Nombres).ToLower().Contains(buscador.ToLower()) ||
                                     (empleado.Apellidos).ToLower().Contains(buscador.ToLower()) ||
                                     (empleado.Direccion).ToLower().Contains(buscador.ToLower()) ||
                                     (empleado.Telefono).ToLower().Contains(buscador.ToLower()) ||
                                     (empleado.TelefonoPersonal).ToLower().Contains(buscador.ToLower()) ||
                                     (area.Nombre).ToLower().Contains(buscador.ToLower()) ||
                                     (cargos.Cargo).ToLower().Contains(buscador.ToLower()))

                                      select new EmpleadoAF
                                          {
                                          idempleado=empleado.IdEmpleado,
                                          dui = empleado.Dui,
                                          nombres = empleado.Nombres,
                                          apellidos = empleado.Apellidos,
                                          direccion = empleado.Direccion,
                                          telefono = empleado.Telefono,
                                          telefonopersonal = empleado.TelefonoPersonal,
                                          nombrearea = area.Nombre,
                                          nombresucursal = sucursal.Nombre,
                                          ubicacion = sucursal.Ubicacion,
                                          cargo = cargos.Cargo
                                      }).ToList();
                    return listaEmpleado;
                }
            }
        }


        //para no poder editar el carea de un empleado.
        [HttpGet]
        [Route("api/Empleado/noModificarArea/{idempleado}")]
        public int noModificarArea(int idempleado)
        {
            int res = 0;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {

                    Empleado oEmpleado = bd.Empleado.Where(p => p.IdEmpleado == idempleado && p.Dhabilitado == 1).First();
                    ActivoFijo oActivo = bd.ActivoFijo.Where(p => p.IdResponsable == oEmpleado.IdEmpleado && p.EstadoActual != 0).First();
                    res = 1;
                }
            }
            catch (Exception ex)
            {
                res = 0;
            }
            return res;
        }
        //Metodo para no permitir elimiar un empleado cuando ya hay activos referenciados al empleado
        [HttpGet]
        [Route("api/Empleado/noEliminarEmpleado/{idempleado}")]
        public int noEliminarEmpleado(int idempleado)
        {
            int res = 0;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    ActivoFijo oEmpleado = bd.ActivoFijo.Where(p => p.IdResponsable == idempleado && p.EstaAsignado==1).First();
                    res = 1;
                }
            }
            catch (Exception ex)
            {
                res = 0;
            }
            return res;
        }

        [HttpGet]
        [Route("api/Empleado/eliminarEmpleado/{idempleado}")]
        public int eliminarEmpleado(int idempleado)
        {
            int respuesta = 0;

            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    Empleado oEmpleado = bd.Empleado.Where(p => p.IdEmpleado == idempleado).First();
                    oEmpleado.Dhabilitado = 0;
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
        [Route("api/Empleado/validardui/{idempleado}/{dui}")]
        public int validardui(int idempleado, string dui)
        {
            int respuesta = 0;
            try
            {


                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    if (idempleado == 0)
                    {
                        respuesta = bd.Empleado.Where(p => p.Dui.ToLower() == dui.ToLower() && p.Dhabilitado == 1).Count();
                    }
                    else
                    {
                       
                        respuesta = bd.Empleado.Where(p => p.Dui.ToLower() == dui.ToLower() &&  p.IdEmpleado != idempleado && p.Dhabilitado == 1).Count();
                       
                    }


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

        [HttpGet]
        [Route("api/Empleado/listarAreaCombo")]
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
                                                          IdAreaNegocio= area.IdAreaNegocio,
                                                          Nombre= area.Nombre,
                                                          IdSucursal= sucursal.IdSucursal,
                                                          nombreSucursal= sucursal.Nombre,
                                                          ubicacion= sucursal.Ubicacion


                                                     }).ToList();


                return listarAreas;

            }
        }
    }
}