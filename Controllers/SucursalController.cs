﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ASGARDAPI.Models;
using ASGARDAPI.Clases;

namespace ASGARDAPI.Controllers
{
    public class SucursalController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        [Route("api/Sucursal/listarSucursales")]
        public IEnumerable<SucursalAF> listarSucursales()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<SucursalAF> listaSucursales = (from suscursal in bd.Sucursal
                                                           where suscursal.Dhabilitado == 1
                                                           select new SucursalAF
                                                           {
                                                               IdSucursal = suscursal.IdSucursal,
                                                               Nombre = suscursal.Nombre,
                                                               Ubicacion = suscursal.Ubicacion,
                                                               Correlativo = suscursal.Correlativo
                                                           }).ToList();
                return listaSucursales;
            }
        }
        [HttpPost]
        [Route("api/Sucursal/guardarSucursal")]
        public int guardarSucursal([FromBody]SucursalAF oSucursalAF)
        {
            int res = 0;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    Sucursal oSucursal = new Sucursal();
                    oSucursal.IdSucursal = oSucursalAF.IdSucursal;
                    oSucursal.Nombre = oSucursalAF.Nombre;
                    oSucursal.Ubicacion = oSucursalAF.Ubicacion;
                    oSucursal.Correlativo = oSucursalAF.Correlativo;
                    oSucursal.IdCooperativa = 1;
                    oSucursal.Dhabilitado = 1;
                    bd.Sucursal.Add(oSucursal);
                    bd.SaveChanges();
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
        [Route("api/Sucursal/eliminarSucursal/{idSucursal}")]
        public int eliminarSucursal(int idSucursal)
        {
            int res = 0;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    Sucursal oSucursal = bd.Sucursal.Where(p => p.IdSucursal == idSucursal).First();
                    oSucursal.Dhabilitado = 0;
                    bd.SaveChanges();
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
        [Route("api/Sucursal/buscarSucursal/{buscador?}")]
        public IEnumerable<SucursalAF> buscarSucursal(string buscador = "")
        {
            List<SucursalAF> listaSucursal;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                if (buscador == "")
                {
                    listaSucursal = (from sucursal in bd.Sucursal
                                     where sucursal.Dhabilitado == 1
                                     select new SucursalAF
                                     {
                                         IdSucursal = sucursal.IdSucursal,
                                         Nombre = sucursal.Nombre,
                                         Ubicacion = sucursal.Ubicacion,
                                         Correlativo = sucursal.Correlativo
                                     }).ToList();
                    return listaSucursal;
                }
                else
                {
                    listaSucursal = (from sucursal in bd.Sucursal
                                     where sucursal.Dhabilitado == 1

                                     && ((sucursal.IdSucursal).ToString().Contains(buscador) || (sucursal.Nombre).ToLower().Contains(buscador.ToLower()) || (sucursal.Ubicacion).ToLower().Contains(buscador.ToLower()) || (sucursal.Correlativo).ToLower().Contains(buscador.ToLower()))
                                     select new SucursalAF
                                     {
                                         IdSucursal = sucursal.IdSucursal,
                                         Nombre = sucursal.Nombre,
                                         Ubicacion = sucursal.Ubicacion,
                                         Correlativo = sucursal.Correlativo
                                     }).ToList();
                    return listaSucursal;
                }
            }
        }
        [HttpPost]
        [Route("api/Sucursal/modificarSucursal")]
        public int modificarSucursal([FromBody]SucursalAF oSucursalAF)
        {
            int rpta = 0;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    Sucursal oSucursal = bd.Sucursal.Where(p => p.IdSucursal == oSucursalAF.IdSucursal).First();
                    oSucursal.IdSucursal = oSucursalAF.IdSucursal;
                    oSucursal.Nombre = oSucursalAF.Nombre;
                    oSucursal.Ubicacion = oSucursalAF.Ubicacion;
                    oSucursal.Correlativo = oSucursalAF.Correlativo;
                    bd.SaveChanges();
                    rpta = 1;
                }
            }
            catch (Exception ex)
            {
                rpta = 0;
            }
            return rpta;
        }
        [HttpGet]
        [Route("api/Sucursal/RecuperarSucursal/{id}")]
        public SucursalAF RecuperarSucursal(int id)
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                SucursalAF oSucursalAF = new SucursalAF();
                Sucursal oSucursal = bd.Sucursal.Where(p => p.IdSucursal == id).First();
                oSucursalAF.IdSucursal = oSucursal.IdSucursal;
                oSucursalAF.Nombre = oSucursal.Nombre;
                oSucursalAF.Ubicacion = oSucursal.Ubicacion;
                oSucursalAF.Correlativo = oSucursal.Correlativo;
                return oSucursalAF;
            }
        }
        [HttpGet]
        [Route("api/Sucursal/validarCorrelativo/{idSucursal}/{correlativo}")]
        public int validarCorrelativo(int idSucursal, string correlativo)
        {
            int respuesta = 0;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    if (idSucursal == 0)
                    {
                        respuesta = bd.Sucursal.Where(p => p.Correlativo.ToLower() == correlativo.ToLower() && p.Dhabilitado == 1).Count();
                    }
                    else
                    {
                        respuesta = bd.Sucursal.Where(p => p.Correlativo.ToLower() == correlativo.ToLower() && p.IdSucursal != idSucursal && p.Dhabilitado == 1).Count();
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
        [Route("api/Sucursal/validarCorrelativo/{idSucursal}/{nombre}/{ubicacion}")]
        public int noRepetirSucursalUbicacion(int idSucursal, string nombre,string ubicacion)
        {
            int respuesta = 0;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    if (idSucursal == 0)
                    {
                        respuesta = bd.Sucursal.Where(p => p.Nombre.ToLower() == nombre.ToLower() && p.Ubicacion.ToLower()==ubicacion.ToLower() && p.Dhabilitado == 1).Count();
                    }
                    else
                    {
                        respuesta = bd.Sucursal.Where(p => p.Nombre.ToLower() == nombre.ToLower() && p.Ubicacion.ToLower() == ubicacion.ToLower() && p.IdSucursal != idSucursal && p.Dhabilitado == 1).Count();
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta = 0;
            }
            return respuesta;
        }
        //Metodo para ver si la sucursal se esta utilizando en un area de negocio
        [HttpGet]
        [Route("api/Sucursal/validarArea/{idSucursal}")]
        public int validarArea(int idSucursal)
        {
            int res = 0;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    AreaDeNegocio oArea = bd.AreaDeNegocio.Where(p => p.IdSucursal == idSucursal && p.Dhabilitado == 1).First();
                    res = 1;
                }
            }
            catch (Exception ex)
            {
                res = 0;
            }
            return res;
        }
        //Valida si ya hay activos haciendo refencia
        [HttpGet]
        [Route("api/Sucursal/validarRefereciaActivo/{idSucursal}")]
        public int validarRefereciaActivo(int idSucursal)
        {
            int res = 0;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    AreaDeNegocio oArea = bd.AreaDeNegocio.Where(p => p.IdSucursal == idSucursal && p.Dhabilitado == 1).First();
                    Empleado Oempleado = bd.Empleado.Where(p => p.IdAreaDeNegocio == oArea.IdAreaNegocio && p.Dhabilitado == 1).First();
                    ActivoFijo oActivo = bd.ActivoFijo.Where(p => p.IdResponsable == Oempleado.IdEmpleado && p.EstadoActual != 0).First();
                    res = 1;
                }
            }
            catch (Exception ex)
            {
                res = 0;
            }
            return res;
        }
    }
}