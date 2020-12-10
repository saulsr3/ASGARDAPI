using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ASGARDAPI.Clases;
using ASGARDAPI.Models;


namespace ASGARDAPI.Controllers
{
    public class TecnicoController : Controller
    {
        
        public IActionResult Index()
        {
            return View();
        }

        //Método guardar Técnico
        [HttpPost]
        [Route("api/Tecnico/guardarTecnico")]
        public int guardarTecnico([FromBody] TecnicoAF oTecnicoAF)
        {
            int rpta = 0;

            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {                 
                    Tecnicos oTecnico = new Tecnicos();

                    oTecnico.IdTecnico = oTecnicoAF.idtecnico;
                    oTecnico.Nombre = oTecnicoAF.nombre;
                    oTecnico.Empresa = oTecnicoAF.empresa;
                    oTecnico.Dhabilitado = 1;
                    bd.Tecnicos.Add(oTecnico);
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

        //Método listar técnico
        [HttpGet]
        [Route("api/Tecnico/listarTenico")]
        public IEnumerable<TecnicoAF> listarTecnico()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<TecnicoAF> listaTenico = (from tecnico in bd.Tecnicos
                                                   where tecnico.Dhabilitado == 1
                                                   select new TecnicoAF
                                                   {
                                                      idtecnico=tecnico.IdTecnico,
                                                      nombre=tecnico.Nombre,
                                                      empresa=tecnico.Empresa
                                                   }).ToList();
                return listaTenico;
            }
        }

        [HttpGet]
        [Route("api/Tecnico/validarlistarTenico")]
        public int validarlistarTenico()
        {
            int respuesta = 0;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<TecnicoAF> lista = (from tecnico in bd.Tecnicos
                                                      where tecnico.Dhabilitado == 1
                                                      select new TecnicoAF
                                                      {
                                                          idtecnico = tecnico.IdTecnico,
                                                          nombre = tecnico.Nombre,
                                                          empresa = tecnico.Empresa
                                                      }).ToList();
                if (lista.Count() > 0)
                {
                    respuesta = 1;
                }
                return respuesta;
            }
        }
        //Método recuperar técnico
        [HttpGet]
        [Route("api/Tecnico/recuperarTecnico/{id}")]
        public TecnicoAF recuperarTecnico(int id)
        {

            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                TecnicoAF oTecnicoAF = new TecnicoAF();
                Tecnicos oTecnico = bd.Tecnicos.Where(p => p.IdTecnico == id).First();

                oTecnicoAF.idtecnico = oTecnico.IdTecnico;
                oTecnicoAF.nombre = oTecnico.Nombre;
                oTecnicoAF.empresa = oTecnico.Empresa;

                return oTecnicoAF;
            }

        }

        //Método modificar técnico
        [HttpPost]
        [Route("api/Tecnico/modificarTecnico")]
        public int modificarTecnico([FromBody] TecnicoAF oTecnicoAF)
        {
            int rpta = 0;

            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    Tecnicos oTecnico = bd.Tecnicos.Where(p => p.IdTecnico == oTecnicoAF.idtecnico).First();

                    oTecnico.IdTecnico = oTecnicoAF.idtecnico;
                    oTecnico.Nombre = oTecnicoAF.nombre;
                    oTecnico.Empresa = oTecnicoAF.empresa;

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

        //Método eliminar técnico
        [HttpGet]
        [Route("api/Tecnico/eliminarTecnico/{idTecnico}")]
        public int eliminarTecnico(int idTecnico)
        {
            int rpta = 0;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    Tecnicos oTecnico = bd.Tecnicos.Where(p => p.IdTecnico == idTecnico).First();
                    oTecnico.Dhabilitado = 0;
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

        //Método buscar técnico
        [HttpGet]
        [Route("api/Tecnico/buscarTecnico/{buscador?}")]
        public IEnumerable<TecnicoAF> buscarTecnico(string buscador = "")
        {
            List<TecnicoAF> listaTecnico;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                if (buscador == "")
                {
                    listaTecnico = (from tecnico in bd.Tecnicos
                                  where tecnico.Dhabilitado == 1
                                  select new TecnicoAF
                                  {
                                      idtecnico=tecnico.IdTecnico,
                                      nombre=tecnico.Nombre,
                                      empresa=tecnico.Empresa

                                  }).ToList();

                    return listaTecnico;
                }
                else
                {
                    listaTecnico = (from tecnico in bd.Tecnicos
                                  where tecnico.Dhabilitado == 1

                                  && ((tecnico.IdTecnico).ToString().Contains(buscador) || (tecnico.Nombre).ToLower().Contains(buscador.ToLower()) || (tecnico.Empresa).ToLower().Contains(buscador.ToLower()))
                                  select new TecnicoAF
                                  {
                                      idtecnico = tecnico.IdTecnico,
                                      nombre = tecnico.Nombre,
                                      empresa = tecnico.Empresa
                                  }).ToList();

                    return listaTecnico;
                }
            }
        }

        //Método validar técnico
        [HttpGet]
        [Route("api/Tecnico/validarTecnico/{idTecnico}/{nombre}")]
        public int validarTecnico(int idTecnico, string nombre)
        {
            int rpta = 0;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    if (idTecnico == 0)
                    {
                        rpta = bd.Tecnicos.Where(p => p.Nombre.ToLower() == nombre.ToLower() && p.Dhabilitado == 1).Count();
                    }
                    else
                    {
                        rpta = bd.Tecnicos.Where(p => p.Nombre.ToLower() == nombre.ToLower() && p.IdTecnico != idTecnico && p.Dhabilitado == 1).Count();
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
