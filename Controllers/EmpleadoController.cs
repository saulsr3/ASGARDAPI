﻿using System;
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
                IEnumerable<EmpleadoAF> listaEmpleado = (from empleado in bd.Empleado
                                                         join area in bd.AreaDeNegocio
                                                         on empleado.IdAreaDeNegocio equals area.IdAreaNegocio
                                                         join cargos in bd.Cargos
                                                         on empleado.IdCargo equals cargos.IdCargo
                                                         where empleado.Dhabilitado == 1
                                                         select new EmpleadoAF
                                                                   {
                                                                       dui= empleado.Dui,
                                                                       nombres=empleado.Nombres,
                                                                       apellidos=empleado.Apellidos,
                                                                       direccion=empleado.Direccion,
                                                                       telefono=empleado.Telefono,
                                                                       telefonopersonal=empleado.TelefonoPersonal,
                                                                       nombrearea = area.Nombre,
                                                                       cargo= cargos.Cargo
                               
                                                                   }).ToList();
                return listaEmpleado;
            }
        }

        [HttpGet]
        [Route("api/Empleado/RecuperarEmpleado/{dui}")]
        public EmpleadoAF RecuperarEmpleado(string dui)
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                EmpleadoAF oEmpleadoAF = new EmpleadoAF();
                Empleado oEmpleado = bd.Empleado.Where(p => p.Dui == dui).First();
                oEmpleadoAF.dui = oEmpleado.Dui;
                oEmpleadoAF.nombres = oEmpleado.Nombres;
                oEmpleadoAF.apellidos = oEmpleado.Apellidos;
                oEmpleadoAF.direccion = oEmpleado.Direccion;
                oEmpleadoAF.telefono = oEmpleado.Telefono;
                oEmpleadoAF.telefonopersonal = oEmpleado.TelefonoPersonal;
                oEmpleadoAF.idareadenegocio = (int)oEmpleado.IdAreaDeNegocio;
                oEmpleadoAF.idcargo = (int)oEmpleado.IdCargo;



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
                    Empleado oEmpleado = bd.Empleado.Where(p => p.Dui == oEmpleadoAF.dui).First();
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
        [Route("api/Empleado/buscarEmpleado/{buscador?}")]
        public IEnumerable<EmpleadoAF> buscarEmpleado(string buscador = "")
        {
            List<EmpleadoAF> listaEmpleado;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                if (buscador == "")
                {
                    listaEmpleado = (from empleado in bd.Empleado
                                     join area in bd.AreaDeNegocio
                                     on empleado.IdAreaDeNegocio equals area.IdAreaNegocio
                                     join cargos in bd.Cargos
                                     on empleado.IdCargo equals cargos.IdCargo
                                     where empleado.Dhabilitado == 1
                                     select new EmpleadoAF
                                     {
                                         dui = empleado.Dui,
                                         nombres = empleado.Nombres,
                                         apellidos = empleado.Apellidos,
                                         direccion = empleado.Direccion,
                                         telefono = empleado.Telefono,
                                         telefonopersonal = empleado.TelefonoPersonal,
                                         nombrearea = area.Nombre,
                                         cargo = cargos.Cargo

                                     }).ToList();
                    return listaEmpleado;
                }
                else
                {
                    listaEmpleado = (from empleado in bd.Empleado
                                     join area in bd.AreaDeNegocio
                                     on empleado.IdAreaDeNegocio equals area.IdAreaNegocio
                                     join cargos in bd.Cargos
                                     on empleado.IdCargo equals cargos.IdCargo
                                     where empleado.Dhabilitado == 1

                                     &&((empleado.Dui).ToLower().Contains(buscador.ToLower()) ||
                                     (empleado.Nombres).ToLower().Contains(buscador.ToLower()) ||
                                     (empleado.Apellidos).ToLower().Contains(buscador.ToLower()) ||
                                     (empleado.Direccion).ToLower().Contains(buscador.ToLower()) ||
                                     (empleado.Telefono).ToLower().Contains(buscador.ToLower()) ||
                                     (empleado.TelefonoPersonal).ToLower().Contains(buscador.ToLower()) ||
                                     (area.Nombre).ToLower().Contains(buscador.ToLower()) ||
                                     (cargos.Cargo).ToLower().Contains(buscador.ToLower()))

                                      select new EmpleadoAF
                                          {
                                          dui = empleado.Dui,
                                          nombres = empleado.Nombres,
                                          apellidos = empleado.Apellidos,
                                          direccion = empleado.Direccion,
                                          telefono = empleado.Telefono,
                                          telefonopersonal = empleado.TelefonoPersonal,
                                          nombrearea = area.Nombre,
                                          cargo = cargos.Cargo
                                      }).ToList();
                    return listaEmpleado;
                }
            }
        }

        [HttpGet]
        [Route("api/Empleado/eliminarEmpleado/{dui}")]
        public int eliminarEmpleado(string dui)
        {
            int respuesta = 0;

            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    Empleado oEmpleado = bd.Empleado.Where(p => p.Dui == dui).First();
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
        [Route("api/Empleado/validardui/{nombres}/{dui}")]
        public string validardui(string nombres,string dui)
        {
            int respuesta = 0;
            try
            {


                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    if (nombres == " ")
                    {
                        respuesta = bd.Empleado.Where(p => p.Dui == dui && p.Dhabilitado == 1).Count();
                    }
                    else
                    {
                       
                        respuesta = bd.Empleado.Where(p => p.Dui == dui &&  p.Nombres.ToLower() != nombres.ToLower() && p.Dhabilitado == 1).Count();
                       
                    }


                }

            }
            catch (Exception ex)
            {
                respuesta = 0;

            }
            return respuesta.ToString();

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
        public IEnumerable<AreasDeNegocioAFdato> listarAreaCombo()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<AreasDeNegocioAFdato> listarAreas = (from area in bd.AreaDeNegocio
                                                     where area.Dhabilitado == 1
                                                     select new AreasDeNegocioAFdato
                                                     {
                                                         idareadenegocio= area.IdAreaNegocio,
                                                         nombre= area.Nombre

                                                     }).ToList();


                return listarAreas;

            }
        }
    }
}