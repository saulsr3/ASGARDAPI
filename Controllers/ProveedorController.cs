﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ASGARDAPI.Clases;
using ASGARDAPI.Models;
namespace ASGARDAPI.Controllers
{
    public class ProveedorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        [Route("api/Proveedor/listarProveedores")]
        public IEnumerable<ProveedoresAF> listarProveedores()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<ProveedoresAF> listaProveedores = (from proveedor in bd.Proveedor
                                                               where proveedor.Dhabilitado == 1
                                                               select new ProveedoresAF
                                                               {
                                                                   idProveedor = proveedor.IdProveedor,
                                                                   nombre = proveedor.Nombre,
                                                                   direccion = proveedor.Direccion,
                                                                   telefono = proveedor.Telefono,
                                                                   rubro = proveedor.Rubro,
                                                                   encargado = proveedor.Encargado,
                                                                   telefonoencargado = proveedor.TelefonoEncargado
                                                               }).ToList();
                return listaProveedores;
            }
        }
        [HttpGet]
        [Route("api/Proveedor/validarlistarProveedores")]
        public int validarlistarProveedores()
        {
            int respuesta = 0;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<ProveedoresAF> lista = (from proveedor in bd.Proveedor
                                                               where proveedor.Dhabilitado == 1
                                                               select new ProveedoresAF
                                                               {
                                                                   idProveedor = proveedor.IdProveedor,
                                                                   nombre = proveedor.Nombre,
                                                                   direccion = proveedor.Direccion,
                                                                   telefono = proveedor.Telefono,
                                                                   rubro = proveedor.Rubro,
                                                                   encargado = proveedor.Encargado,
                                                                   telefonoencargado = proveedor.TelefonoEncargado
                                                               }).ToList();
                if (lista.Count() > 0)
                {
                    respuesta = 1;
                }
                return respuesta;
            }
        }


        //Método guardar Proveedores
        [HttpPost]
        [Route("api/Proveedor/guardarProveedor")]
        public int guardarProveedor([FromBody]ProveedoresAF oProveedoresAF)
        {
            int rpta = 0;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {

                    Proveedor oProveedor = new Proveedor();
                    oProveedor.IdProveedor = oProveedoresAF.idProveedor;
                    oProveedor.Nombre = oProveedoresAF.nombre;
                    oProveedor.Direccion = oProveedoresAF.direccion;
                    oProveedor.Telefono = oProveedoresAF.telefono;
                    oProveedor.Rubro = oProveedoresAF.rubro;
                    oProveedor.Encargado = oProveedoresAF.encargado;
                    oProveedor.TelefonoEncargado = oProveedoresAF.telefonoencargado;
                    oProveedor.Dhabilitado = 1;

                    bd.Proveedor.Add(oProveedor);
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



        [HttpPost]
        [Route("api/Proveedor/modificarProveedor")]
        public int modificarProveedor([FromBody]ProveedoresAF oProveedoresAF)
        {
            int rpta = 0;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    //para editar tenemos que sacar la informacion
                    Proveedor oProveedor = bd.Proveedor.Where(p => p.IdProveedor == oProveedoresAF.idProveedor).First();
                    oProveedor.Nombre = oProveedoresAF.nombre;
                    oProveedor.Direccion = oProveedoresAF.direccion;
                    oProveedor.Telefono = oProveedoresAF.telefono;
                    oProveedor.Rubro = oProveedoresAF.rubro;
                    oProveedor.Encargado = oProveedoresAF.encargado;
                    oProveedor.TelefonoEncargado = oProveedoresAF.telefonoencargado;
                    //para guardar cambios
                    bd.SaveChanges();
                    //si todo esta bien
                    rpta = 1;
                }
            }
            catch (Exception ex)
            {
                rpta = 0;
            }
            return rpta;
        }


        //Metodo eliminar Proveedor
        [HttpGet]
        [Route("api/Proveedor/eliminarProveedor/{idProveedor}")]
        public int eliminarProveedor(int idProveedor)
        {
            int respuesta = 0;

            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    Proveedor oProveedor = bd.Proveedor.Where(p => p.IdProveedor == idProveedor).First();
                    oProveedor.Dhabilitado = 0;
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
        [Route("api/Proveedor/RecuperarProveedor/{id}")]
        public ProveedoresAF RecuperarProveedor(int id)
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                ProveedoresAF oProveedoresAF = new ProveedoresAF();
                Proveedor oProveedor = bd.Proveedor.Where(p => p.IdProveedor == id).First();
                oProveedoresAF.idProveedor = oProveedor.IdProveedor;
                oProveedoresAF.nombre = oProveedor.Nombre;
                oProveedoresAF.direccion = oProveedor.Direccion;
                oProveedoresAF.telefono = oProveedor.Telefono;
                oProveedoresAF.rubro = oProveedor.Rubro;
                oProveedoresAF.encargado = oProveedor.Encargado;
                oProveedoresAF.telefonoencargado = oProveedor.TelefonoEncargado;
                return oProveedoresAF;
            }
        }


        [HttpGet]
        [Route("api/Proveedor/buscarProveedor/{buscador?}")]
        public IEnumerable<ProveedoresAF> buscarProveedor(string buscador = "")
        {
            List<ProveedoresAF> listaProveedor;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                if (buscador == "")
                {
                    listaProveedor = (from proveedor in bd.Proveedor
                                      where proveedor.Dhabilitado == 1
                                      select new ProveedoresAF
                                      {
                                          idProveedor = proveedor.IdProveedor,
                                          nombre = proveedor.Nombre,
                                          direccion = proveedor.Direccion,
                                          telefono = proveedor.Telefono,
                                          rubro = proveedor.Rubro,
                                          encargado = proveedor.Encargado,
                                          telefonoencargado = proveedor.TelefonoEncargado
                                      }).ToList();

                    return listaProveedor;
                }
                else
                {
                    listaProveedor = (from proveedor in bd.Proveedor
                                      where proveedor.Dhabilitado == 1

                                      && ((proveedor.IdProveedor).ToString().Contains(buscador)
                                      || (proveedor.Nombre).ToLower().Contains(buscador.ToLower())
                                      || (proveedor.Direccion).ToLower().Contains(buscador.ToLower())
                                      || (proveedor.Telefono).ToLower().Contains(buscador.ToLower())
                                      || (proveedor.Rubro).ToLower().Contains(buscador.ToLower())
                                      || (proveedor.Encargado).ToLower().Contains(buscador.ToLower())
                                      || (proveedor.TelefonoEncargado).ToLower().Contains(buscador.ToLower()))
                                      select new ProveedoresAF
                                      {
                                          idProveedor = proveedor.IdProveedor,
                                          nombre = proveedor.Nombre,
                                          direccion = proveedor.Direccion,
                                          telefono = proveedor.Telefono,
                                          rubro = proveedor.Rubro,
                                          encargado = proveedor.Encargado,
                                          telefonoencargado = proveedor.TelefonoEncargado
                                      }).ToList();

                    return listaProveedor;
                }
            }
        }

        //Metodo para ver si el proveedor se esta utilizando en un activo
        [HttpGet]
        [Route("api/Proveedor/validarActivo/{idProveedor}")]
        public int validarActivo(int idProveedor)
        {
            int res = 0;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    ActivoFijo oArea = bd.ActivoFijo.Where(p => p.IdProveedor == idProveedor && p.EstadoActual == 1).First();
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
        [Route("api/Proveedor/validarProveedor/{idproveedor}/{nombre}")]
        public int validarProveedor(int idproveedor, string nombre)
        {
            int rpta = 0;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    if (idproveedor == 0)
                    {
                        rpta = bd.Proveedor.Where(p => p.Nombre.ToLower() == nombre.ToLower()
                        && p.Dhabilitado == 1).Count();
                    }
                    else
                    {
                        rpta = bd.Proveedor.Where(p => p.Nombre.ToLower() == nombre.ToLower() && p.IdProveedor != idproveedor
                        && p.Dhabilitado == 1).Count();
                    }
                }
            }
            catch (Exception ex)
            {
                //si se cae
                rpta = 0;

            }
            return rpta;
        }

        [HttpGet]
        [Route("api/Proveedor/validarEncargado/{idproveedor}/{nombre}")]
        public int validarEncargado(int idproveedor, string nombre)
        {
            int rpta = 0;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    if (idproveedor == 0)
                    {
                        rpta = bd.Proveedor.Where(p => p.Encargado.ToLower() == nombre.ToLower()
                        && p.Dhabilitado == 1).Count();
                    }
                    else
                    {
                        rpta = bd.Proveedor.Where(p => p.Encargado.ToLower() == nombre.ToLower() && p.IdProveedor != idproveedor
                        && p.Dhabilitado == 1).Count();
                    }
                }
            }
            catch (Exception ex)
            {
                //si se cae
                rpta = 0;

            }
            return rpta;
        }

        [HttpGet]
        [Route("api/Proveedor/validarTelProveedor/{idproveedor}/{telefono}")]
        public int validarTelProveedor(int idproveedor, string telefono)
        {
            int rpta = 0;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    if (idproveedor == 0)
                    {
                        rpta = bd.Proveedor.Where(p => p.Telefono == telefono 
                        && p.Dhabilitado == 1 || p.TelefonoEncargado == telefono).Count();
                       
                    }
                    else
                    {
                        rpta = bd.Proveedor.Where(p => p.Telefono == telefono && p.IdProveedor != idproveedor
                        && p.Dhabilitado == 1 ||p.TelefonoEncargado == telefono).Count();
                      
                    }
                }
            }
            catch (Exception ex)
            {
                //si se cae
                rpta = 0;

            }
            return rpta;
        }

        [HttpGet]
        [Route("api/Proveedor/validarTelEncargado/{idproveedorr}/{telefonoe}")]
        public int validarTelEncargado(int idproveedorr, string telefonoe)
        {
            int rpta = 0;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    if (idproveedorr == 0)
                    {
                        rpta = bd.Proveedor.Where(p => p.TelefonoEncargado == telefonoe
                        && p.Dhabilitado == 1 || p.Telefono == telefonoe).Count();
                    }
                    else
                    {
                        rpta = bd.Proveedor.Where(p => p.TelefonoEncargado == telefonoe && p.IdProveedor != idproveedorr
                        && p.Dhabilitado == 1 || p.Telefono == telefonoe).Count();
                    }
                }
            }
            catch (Exception ex)
            {
                //si se cae
                rpta = 0;

            }
            return rpta;
        }

    }
}