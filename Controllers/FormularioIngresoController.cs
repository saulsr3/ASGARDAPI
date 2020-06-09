using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ASGARDAPI.Clases;
using ASGARDAPI.Models;

namespace ASGARDAPI.Controllers
{
    public class FormularioIngresoController : Controller
    {

        public IActionResult Index()
        {
            return View();

        }

        //Método guardar formularioIngreso
        [HttpPost]
        [Route("api/FormularioIngreso/guardarFormIngreso")]
        public int guardarFormIngreso([FromBody] FormularioIngresoAF oFormularioIngresoAF)
        {
            int rpta = 0;

            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    FormularioIngreso oFormularioIngreso = new FormularioIngreso();

                    oFormularioIngreso.NoFormulario = oFormularioIngresoAF.noformulario;
                    oFormularioIngreso.NoFactura = oFormularioIngresoAF.nofactura;
                    oFormularioIngreso.FechaIngreso = oFormularioIngresoAF.fechaingreso;
                    oFormularioIngreso.PersonaEntrega = oFormularioIngresoAF.personaentrega;
                    oFormularioIngreso.PersonaRecibe = oFormularioIngresoAF.personarecibe;
                    oFormularioIngreso.Observaciones = oFormularioIngresoAF.observaciones;

                    bd.FormularioIngreso.Add(oFormularioIngreso);
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
    

