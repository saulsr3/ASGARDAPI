using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASGARDAPI.Clases;
using ASGARDAPI.Models;
using Microsoft.AspNetCore.Mvc;


namespace ASGARDAPI.Controllers
{
    public class ConfiguracionController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

        //Listar
        [HttpGet]
        [Route("api/Cooperativa/listarCooperativa")]
        public IEnumerable<CooperativaAF> listarCooperativa()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<CooperativaAF> listarCooperativa = (from cooperativa in bd.Cooperativa
                                                                join operiodo in bd.Periodo
                                                                on cooperativa.IdCooperativa equals operiodo.IdCooperativa                                        
                                                   where cooperativa.Dhabilitado == 1
                                                   select new CooperativaAF
                                                   {
                                                       idcooperativa=cooperativa.IdCooperativa,
                                                       nombre=cooperativa.Nombre,
                                                       anio= (int)operiodo.Anio,
                                                       descripcion = cooperativa.Descripcion

                                                   }).ToList();
                return listarCooperativa;
            }
        }

        //Guardar
        [HttpPost]
        [Route("api/Cooperativa/guardarCooperativa")]
        public int guardarCooperativa([FromBody] CooperativaAF oCooperativaF, PeriodoAF oPeriodoAF)
        {
            int rpta = 0;

            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    Cooperativa oCooperativa = new Cooperativa();
                    oCooperativa.IdCooperativa = oCooperativaF.idcooperativa;
                    oCooperativa.Nombre = oCooperativaF.nombre;
                    oCooperativa.Logo = oCooperativaF.logo;
                    oCooperativa.Descripcion = oCooperativaF.descripcion;
                    oCooperativa.Dhabilitado = 1;
                    bd.Cooperativa.Add(oCooperativa);
                    bd.SaveChanges();

                    //Para guardar en la tabla periodo
                    Periodo oPeriodo = new Periodo();
                    Cooperativa oCooperativaPeriodo = bd.Cooperativa.Last();
                    oPeriodo.IdPeriodo = oPeriodoAF.idperiodo;

                    //Le mando el id de la cooperativa a la tabla periodo
                    oPeriodo.IdCooperativa = oCooperativaPeriodo.IdCooperativa;

                    oPeriodo.Anio = oCooperativaF.anio;
                    oPeriodo.Estado = 1;
                    bd.Periodo.Add(oPeriodo);
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

        //Recuperar
        [HttpGet]
        [Route("api/Cooperativa/recuperarCooperativa/{id}")]
        public CooperativaAF recuperarCooperativa(int id)
        {

            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                CooperativaAF oCooperativaAF = new CooperativaAF();
                Cooperativa oCooperativa = bd.Cooperativa.Where(p => p.IdCooperativa == id).First();
                oCooperativaAF.idcooperativa = oCooperativa.IdCooperativa;
                oCooperativaAF.nombre = oCooperativa.Nombre;
                oCooperativaAF.logo = oCooperativa.Logo;

                return oCooperativaAF;
            }

        }

        //Validar
        [HttpGet]
        [Route("api/Cooperativa/validarCooperativa/{idcooperativa}/{cooperativa}")]
        public int validarCooperativa(int idcooperativa, string cooperativa)
        {
            int rpta = 0;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    if (idcooperativa == 0)
                    {
                        rpta = bd.Cooperativa.Where(p => p.Nombre.ToLower() == cooperativa.ToLower() && p.Dhabilitado == 1).Count();
                    }
                    else
                    {
                        rpta = bd.Cooperativa.Where(p => p.Nombre.ToLower() == cooperativa.ToLower() && p.IdCooperativa != idcooperativa && p.Dhabilitado == 1).Count();
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
