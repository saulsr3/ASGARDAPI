using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ASGARDAPI.Clases;
using ASGARDAPI.Models;

namespace ASGARDAPI.Controllers
{
    public class DonantesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        //Método guardar Donantes
        [HttpPost]
        [Route("api/Donantes/guardarDonante")]
        public int guardarDonante([FromBody]DonantesAF oDonantesAF)
        {
            int rpta = 0;

            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    Donantes oDonantes = new Donantes();
                    oDonantes.IdDonante = oDonantesAF.IidDonante;
                    oDonantes.Nombre = oDonantesAF.nombre;
                    oDonantes.Telefono = oDonantesAF.telefono;
                    oDonantes.Direccion = oDonantesAF.direccion;
                    oDonantes.Dhabilitado = 1;

                    bd.Donantes.Add(oDonantes);
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

        //Método listar donantes
        [HttpGet]
        [Route("api/Donantes/listarDonantes")]
        public IEnumerable<DonantesAF> listarDonantes()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<DonantesAF> listaDonantes = (from donante in bd.Donantes
                                                         where donante.Dhabilitado == 1
                                                         select new DonantesAF
                                                         {
                                                             IidDonante = donante.IdDonante,
                                                             nombre = donante.Nombre,
                                                             telefono = donante.Telefono,
                                                             direccion = donante.Direccion
                                                         }).ToList();
                return listaDonantes;
            }
        }
        [HttpGet]
        [Route("api/Donantes/validarlistarDonantes")]
        public int validarlistarDonantes()
        {
            int respuesta = 0;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<DonantesAF> lista = (from donante in bd.Donantes
                                                         where donante.Dhabilitado == 1
                                                         select new DonantesAF
                                                         {
                                                             IidDonante = donante.IdDonante,
                                                             nombre = donante.Nombre,
                                                             telefono = donante.Telefono,
                                                             direccion = donante.Direccion
                                                         }).ToList();
                if (lista.Count() > 0)
                {
                    respuesta = 1;
                }
                return respuesta;
            }
        }

        //Método recuperar donante
        [HttpGet]
        [Route("api/Donantes/RecuperarDonante/{idDonante}")]
        public DonantesAF RecuperarDonante(int idDonante)
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                DonantesAF oDonantesAF = new DonantesAF();

                Donantes oDonantes = bd.Donantes.Where(p => p.IdDonante == idDonante).First();

                oDonantesAF.IidDonante = oDonantes.IdDonante;
                oDonantesAF.nombre = oDonantes.Nombre;
                oDonantesAF.telefono = oDonantes.Telefono;
                oDonantesAF.direccion = oDonantes.Direccion;

                return oDonantesAF;
            }
        }



        //Método modificar donante
        [HttpPost]
        [Route("api/Donantes/modificarDonante")]
        public int modificarDonante([FromBody]DonantesAF oDonanteAF)
        {
            int rpta = 0;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    Donantes oDonante = bd.Donantes.Where(p => p.IdDonante == oDonanteAF.IidDonante).First();
                    oDonante.IdDonante = oDonanteAF.IidDonante;
                    oDonante.Nombre = oDonanteAF.nombre;
                    oDonante.Telefono = oDonanteAF.telefono;
                    oDonante.Direccion = oDonanteAF.direccion;
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


        //Método eliminar donante
        [HttpGet]
        [Route("api/Donantes/eliminarDonante/{idDonante}")]
        public int eliminarDonante(int idDonante)
        {
            int rpta = 0;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    Donantes oDonantes = bd.Donantes.Where(p => p.IdDonante == idDonante).First();
                    oDonantes.Dhabilitado = 0;
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

        //Método buscar donante
        [HttpGet]
        [Route("api/Donantes/buscarDonantes/{buscador?}")]
        public IEnumerable<DonantesAF> buscarDonantes(string buscador = "")
        {
            List<DonantesAF> listaDonante;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                if (buscador == "")
                {
                    listaDonante = (from donante in bd.Donantes
                                    where donante.Dhabilitado == 1
                                    select new DonantesAF
                                    {
                                        IidDonante = donante.IdDonante,
                                        nombre = donante.Nombre,
                                        telefono = donante.Telefono,
                                        direccion = donante.Direccion
                                    }).ToList();
                    return listaDonante;
                }
                else
                {
                    listaDonante = (from donante in bd.Donantes
                                    where donante.Dhabilitado == 1
                                    && ((donante.IdDonante).ToString().Contains(buscador) ||
                                    (donante.Nombre).ToLower().Contains(buscador.ToLower()) ||
                                    (donante.Telefono).ToLower().Contains(buscador.ToLower()) ||
                                    (donante.Direccion).ToLower().Contains(buscador.ToLower()))
                                    select new DonantesAF
                                    {
                                        IidDonante = donante.IdDonante,
                                        nombre = donante.Nombre,
                                        telefono = donante.Telefono,
                                        direccion = donante.Direccion
                                    }).ToList();
                    return listaDonante;

                }
            }
        }

        //Método validar donante
        [HttpGet]
        [Route("api/Cargo/validarDonante/{idDonante}/{donante}")]
        public int validarDonante(int idDonante, string donante)
        {
            int rpta = 0;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    if (idDonante == 0)
                    {
                        rpta = bd.Donantes.Where(p => p.Nombre.ToLower() == donante.ToLower() && p.Dhabilitado == 1).Count();
                    }
                    else
                    {
                        rpta = bd.Donantes.Where(p => p.Nombre.ToLower() == donante.ToLower() && p.IdDonante != idDonante && p.Dhabilitado == 1).Count();
                    }
                }

            }
            catch (Exception ex)
            {
                return rpta = 0;
            }

            return rpta;
        }
    }
}