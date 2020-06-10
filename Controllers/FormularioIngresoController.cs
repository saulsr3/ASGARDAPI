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
        public int guardarActivoFijo([FromBody] ActivoAF oActivoAF)
        {
            int rpta = 0;

            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    for (int i = 0; i < oActivoAF.cantidad; i++)
                    {
                        ActivoFijo oActivoFijo = new ActivoFijo();
                        //Datos para la tabla activo fijo
                        oActivoFijo.IdBien = oActivoAF.idbien;
                        FormularioIngreso oFormulario = bd.FormularioIngreso.Last();
                        oActivoFijo.NoFormulario = oFormulario.NoFormulario;
                        oActivoFijo.Desripcion = oActivoAF.descripcion;
                        oActivoFijo.Modelo = oActivoAF.modelo;
                        oActivoFijo.TipoAdquicicion = oActivoAF.tipoadquicicion;
                        oActivoFijo.Color = oActivoAF.color;
                        oActivoFijo.IdMarca = oActivoAF.idmarca;
                        oActivoFijo.IdClasificacion = oActivoAF.idclasificacion;
                        if (oActivoAF.tipoadquicicion == 1 || oActivoAF.tipoadquicicion == 2)
                        {
                            oActivoFijo.IdProveedor = oActivoAF.idproveedor;
                            if (oActivoAF.tipoadquicicion == 2)
                            {
                                oActivoFijo.PlazoPago = oActivoAF.plazopago;
                                oActivoFijo.Prima = oActivoAF.prima;
                                oActivoFijo.CuotaAsignanda = oActivoAF.cuotaasignada;
                                oActivoFijo.Intereses = oActivoAF.interes;
                            }
                        }
                        else
                        {
                            oActivoFijo.IdDonante = oActivoAF.idproveedor;
                        }
                        oActivoFijo.EstadoIngreso = oActivoAF.estadoingreso;
                        oActivoFijo.ValorAdquicicion = oActivoAF.valoradquicicion;
                        oActivoFijo.ValorResidual = 0;
                        oActivoFijo.EstaAsignado = 0;
                        oActivoFijo.EstadoActual = 1;
                        bd.ActivoFijo.Add(oActivoFijo);
                        bd.SaveChanges();
                    }
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
    

