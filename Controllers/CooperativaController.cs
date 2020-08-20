using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASGARDAPI.Clases;
using ASGARDAPI.Models;
using Microsoft.AspNetCore.Mvc;


namespace ASGARDAPI.Controllers
{
    public class CooperativaController : Controller
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
                                                   where cooperativa.Dhabilitado == 1
                                                   select new CooperativaAF
                                                   {
                                                       idcooperativa=cooperativa.IdCooperativa,
                                                       nombre=cooperativa.Nombre,
                                                       descripcion = cooperativa.Descripcion

                                                   }).ToList();
                return listarCooperativa;
            }
        }

    }
}
