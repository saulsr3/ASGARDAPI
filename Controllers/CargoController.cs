using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ASGARDAPI.Clases;
using ASGARDAPI.Models;

namespace ASGARDAPI.Controllers
{
    public class CargoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        //Método guardar cargo
        [HttpPost]
        [Route("api/Cargo/guardarCargo")]
        public int guardarCargo([FromBody] CargoAF oCargoAF)
        {
            int rpta = 0;

            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    Cargos oCargo = new Cargos();
                    oCargo.IdCargo = oCargoAF.idcargo;
                    oCargo.Cargo = oCargoAF.cargo;
                    oCargo.Descripcion = oCargoAF.descripcion;
                    oCargo.Dhabilitado = 1;
                    bd.Cargos.Add(oCargo);
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

        //Método listar cargo
        [HttpGet]
        [Route("api/Cargo/listarCargo")]
        public IEnumerable<CargoAF> listarCargo()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<CargoAF> listaCargo = (from cargo in bd.Cargos
                                                   where cargo.Dhabilitado == 1
                                                   select new CargoAF
                                                   {
                                                       idcargo = cargo.IdCargo,
                                                       cargo = cargo.Cargo,
                                                       descripcion = cargo.Descripcion
                                                   }).ToList();
                return listaCargo;
            }
        }

        //Método recuperar cargo
        [HttpGet]
        [Route("api/Cargo/recuperarCargo/{id}")]
        public CargoAF recuperarCargo(int id)
        {

            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                CargoAF oCargoAF = new CargoAF();
                Cargos oCargo = bd.Cargos.Where(p => p.IdCargo == id).First();
                oCargoAF.idcargo = oCargo.IdCargo;
                oCargoAF.cargo = oCargo.Cargo;
                oCargoAF.descripcion = oCargo.Descripcion;

                return oCargoAF;
            }

        }

        //Método modificar cargo
        [HttpPost]
        [Route("api/Cargo/modificarCargo")]
        public int modificarCargo([FromBody] CargoAF oCargoAF)
        {
            int rpta = 0;

            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    Cargos oCargo = bd.Cargos.Where(p => p.IdCargo == oCargoAF.idcargo).First();
                    oCargo.IdCargo = oCargoAF.idcargo;
                    oCargo.Cargo = oCargoAF.cargo;
                    oCargo.Descripcion = oCargoAF.descripcion;
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

        //Método eliminar cargo
        [HttpGet]
        [Route("api/Cargo/eliminarCargo/{idCargo}")]
        public int eliminarCargo(int idCargo)
        {
            int rpta = 0;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    Cargos oCargo = bd.Cargos.Where(p => p.IdCargo == idCargo).First();
                    oCargo.Dhabilitado = 0;
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

        //Método buscar cargo
        [HttpGet]
        [Route("api/Cargo/buscarCargo/{buscador?}")]
        public IEnumerable<CargoAF> buscarCargo(string buscador = "")
        {
            List<CargoAF> listaCargo;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                if (buscador == "")
                {
                    listaCargo = (from cargo in bd.Cargos
                                  where cargo.Dhabilitado == 1
                                  select new CargoAF
                                  {
                                      idcargo = cargo.IdCargo,
                                      cargo = cargo.Cargo,
                                      descripcion = cargo.Descripcion
                                  }).ToList();

                    return listaCargo;
                }
                else
                {
                    listaCargo = (from cargo in bd.Cargos
                                  where cargo.Dhabilitado == 1

                                  && ((cargo.IdCargo).ToString().Contains(buscador) || (cargo.Cargo).ToLower().Contains(buscador.ToLower()) || (cargo.Descripcion).ToLower().Contains(buscador.ToLower()))
                                  select new CargoAF
                                  {
                                      idcargo = cargo.IdCargo,
                                      cargo = cargo.Cargo,
                                      descripcion = cargo.Descripcion
                                  }).ToList();
                    return listaCargo;
                }
            }
        }

        //Método validar cargo
        [HttpGet]
        [Route("api/Cargo/validarCargo/{idCargo}/{cargo}")]
        public int validarCargo(int idCargo, string cargo)
        {
            int rpta = 0;
            try
            {
                using(BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    if(idCargo==0)
                    {
                        rpta = bd.Cargos.Where(p => p.Cargo.ToLower() == cargo.ToLower() && p.Dhabilitado == 1).Count();
                    } else
                    {
                        rpta = bd.Cargos.Where(p => p.Cargo.ToLower() == cargo.ToLower() && p.IdCargo != idCargo && p.Dhabilitado == 1).Count();
                    }
                }

            } catch(Exception ex)
            {
                return rpta = 0;
            }

            return rpta;
        }

        [HttpGet]
        [Route("api/Cargo/validarCargos/{idCargo}")]
        public int validarCargos(int idCargo)
        {
            int res = 0;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    Empleado oCargo = bd.Empleado.Where(p => p.IdCargo == idCargo && p.Dhabilitado == 1).First();
                    Empleado oEmpleado = bd.Empleado.Where(p => p.IdEmpleado == oCargo.IdCargo && p.Dhabilitado == 1).First();
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