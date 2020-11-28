using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ASGARDAPI.Clases;
using ASGARDAPI.Models;

namespace ASGARDAPI.Controllers
{
    public class BitacoraController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        [Route("api/Bitacora/guardarTransaccion/{idUsuario}/{descripcion}")]
        public int guardarTransaccion(int idUsuario,string descripcion)
        {
            int rpta = 0;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    Bitacora oBitacora = new Bitacora();
                    oBitacora.IdUsuario = idUsuario;
                    oBitacora.Descripcion = descripcion;
                    oBitacora.Fecha = DateTime.Now;
                    bd.Bitacora.Add(oBitacora);
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
