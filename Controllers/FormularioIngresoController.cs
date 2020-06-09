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

        //Método guardar en activo fijo
        [HttpPost]
        [Route("api/FormularioIngreso/guardarActivoFijo")]
        public int guardarActivoFijo([FromBody] ActivoFijoAF oActivoAF)
        {
            int rpta = 0;

            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    ActivoFijo oActivoFijo = new ActivoFijo();
                    //Datos para la tabla activo fijo
                    oActivoFijo.IdBien = oActivoAF.IdBien;
                    oActivoFijo.CorrelativoBien = null;
                    FormularioIngreso oFormulario = bd.FormularioIngreso.Last();
                    oActivoFijo.NoFormulario = oFormulario.NoFormulario;
                    oActivoFijo.Desripcion = oActivoAF.Desripcion;
                    oActivoFijo.Modelo = oActivoAF.Modelo;
                    oActivoFijo.TipoAdquicicion = oActivoAF.tipoadquicicion;
                    oActivoFijo.Color = oActivoAF.Color;
                    oActivoFijo.NoSerie = null;
                    oActivoFijo.IdMarca = oActivoAF.idmarca;
                    oActivoFijo.IdClasificacion = oActivoAF.idclasificacion;
                    oActivoFijo.IdProveedor = oActivoAF.idproveedor;
                    oActivoFijo.IdDonante = oActivoAF.iddonante;
                    oActivoFijo.VidaUtil = null;
                    oActivoFijo.IdResponsable = null;
                    oActivoFijo.EstadoIngreso = oActivoAF.estadoingreso;
                    oActivoFijo.ValorAdquicicion = oActivoAF.valoradquicicion;
                    oActivoFijo.PlazoPago = oActivoAF.plazopago;
                    oActivoFijo.Prima = oActivoAF.prima;
                    oActivoFijo.CuotaAsignanda = oActivoAF.cuotaasignada;
                    oActivoFijo.Intereses = oActivoAF.interes;
                    oActivoFijo.ValorResidual = oActivoAF.valorresidual;
                    oActivoFijo.Foto = null;
                    oActivoFijo.EstaAsignado = 0;
                    oActivoFijo.DestinoInicial = null;
                    oActivoFijo.EstadoActual = 1;
                    bd.ActivoFijo.Add(oActivoFijo);
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
    

