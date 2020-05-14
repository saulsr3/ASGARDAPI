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
    }
}
