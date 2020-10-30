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

                    //If para clasificar el tipo de activo
                    if (oActivoAF.tipoactivo == 1)
                    {
                        ActivoFijo oActivoFijo = new ActivoFijo();
                        //Datos para la tabla activo fijo
                        oActivoFijo.IdBien = oActivoAF.idbien;
                        oActivoFijo.TipoActivo = oActivoAF.tipoactivo;
                        FormularioIngreso oFormulario = bd.FormularioIngreso.Last();
                        oActivoFijo.NoFormulario = oFormulario.NoFormulario;
                        oActivoFijo.Desripcion = oActivoAF.descripcion;
                        oActivoFijo.TipoAdquicicion = oActivoAF.tipoadquicicion;
                        
                        oActivoFijo.IdClasificacion = oActivoAF.idclasificacion;
                        oActivoFijo.VidaUtil = oActivoAF.vidautil;
                        if (oActivoAF.tipoadquicicion == 3)
                        {
                            oActivoFijo.IdDonante = oActivoAF.idproveedor;
                        }
                        else
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

                        oActivoFijo.EstadoIngreso = oActivoAF.estadoingreso;
                        oActivoFijo.ValorAdquicicion = oActivoAF.valoradquicicion;
                        oActivoFijo.Foto = oActivoAF.foto;
                        oActivoFijo.ValorResidual = oActivoAF.valorresidual;
                        oActivoFijo.EnSolicitud = 0;
                        oActivoFijo.EstadoActual = 1;
                        oActivoFijo.EstaAsignado = 0;
                        bd.ActivoFijo.Add(oActivoFijo);
                        bd.SaveChanges();
                        //Generar codigo
                        //objeto de la clase codigo que contiene los elementos
                        CodigoAF oCodigo = new CodigoAF();
                        //Extraer los datos padres de la base
                        ActivoFijo oActivo = bd.ActivoFijo.Last();
                        Sucursal osucursal = bd.Sucursal.Where(p => p.IdSucursal == oActivoAF.idsucursal).First();
                        Clasificacion oclasificacion = bd.Clasificacion.Where(p => p.IdClasificacion == oActivo.IdClasificacion).First();
                        //LLenado de objeto
                        oCodigo.CorrelativoSucursal = osucursal.Correlativo;
                        oCodigo.CorrelativoClasificacion = oclasificacion.Correlativo;
                        //selccionar cuantos hay de esa clasificacion
                        int oActivoC = bd.ActivoFijo.Where(p => p.EstaAsignado == 1 && p.IdClasificacion == oclasificacion.IdClasificacion).Count();

                        //comparar para la concatenacion correspondiente 
                        if (oActivoC >= 0 && oActivoC <= 9)
                        {
                            oActivoC = oActivoC + 1;
                            oCodigo.Correlativo = "00" + oActivoC.ToString();
                        }
                        else if (oActivoC >= 10 && oActivoC <= 99)
                        {
                            oActivoC = oActivoC + 1;
                            oCodigo.Correlativo = "0" + oActivoC.ToString();
                        }
                        else
                        {
                            oActivoC = oActivoC + 1;
                            oCodigo.Correlativo = oActivoC.ToString();
                        }
                        oActivo.CorrelativoBien = oCodigo.CorrelativoSucursal + "-" + oCodigo.CorrelativoClasificacion + "-" + oCodigo.Correlativo;
                        //Guardamos en la tabla activo fijo
                        oActivo.DestinoInicial = osucursal.Nombre;
                        oActivo.EstaAsignado = 1;
                        bd.SaveChanges();


                        //Transaccion a tarjeta
                        TarjetaDepreciacion transaccion = new TarjetaDepreciacion();
                        ActivoFijo oActivoFijoTransaccion = bd.ActivoFijo.Last();
                        transaccion.IdBien = oActivoFijoTransaccion.IdBien;
                        transaccion.Fecha = oFormulario.FechaIngreso;
                        transaccion.Concepto = "Compra";
                        transaccion.Valor = oActivoFijoTransaccion.ValorAdquicicion;
                        transaccion.DepreciacionAnual = 0.00;
                        transaccion.DepreciacionAcumulada = 0.00;
                        transaccion.ValorActual = oActivoFijoTransaccion.ValorAdquicicion;
                        transaccion.ValorMejora = 0.00;
                        bd.TarjetaDepreciacion.Add(transaccion);
                        bd.SaveChanges();
                        //Generar codigo

                    }
                    if(oActivoAF.tipoactivo == 2)
                    {

                        for (int i = 0; i < oActivoAF.cantidad; i++)
                        {
                            ActivoFijo oActivoFijo = new ActivoFijo();
                            //Datos para la tabla activo fijo
                            oActivoFijo.IdBien = oActivoAF.idbien;
                            oActivoFijo.TipoActivo = oActivoAF.tipoactivo;
                            FormularioIngreso oFormulario = bd.FormularioIngreso.Last();
                            oActivoFijo.NoFormulario = oFormulario.NoFormulario;
                            oActivoFijo.Desripcion = oActivoAF.descripcion;
                            oActivoFijo.Modelo = oActivoAF.modelo;
                            oActivoFijo.TipoAdquicicion = oActivoAF.tipoadquicicion;
                            oActivoFijo.Color = oActivoAF.color;
                            oActivoFijo.IdClasificacion = oActivoAF.idclasificacion;
                            if (oActivoAF.idmarca != 0)
                            {
                                oActivoFijo.IdMarca = oActivoAF.idmarca;
                            }
                            else
                            {
                                oActivoFijo.IdMarca = null;
                            }
                            if (oActivoAF.tipoadquicicion == 3)
                            {
                                oActivoFijo.IdDonante = oActivoAF.idproveedor;
                            }
                            else
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
                            oActivoFijo.EstadoIngreso = oActivoAF.estadoingreso;
                            oActivoFijo.ValorAdquicicion = oActivoAF.valoradquicicion;
                            oActivoFijo.Foto = oActivoAF.foto;
                            oActivoFijo.ValorResidual = oActivoAF.valorresidual;
                            //Linea de prueba en rama solicitud
                            oActivoFijo.EnSolicitud = 0;
                            oActivoFijo.EstaAsignado = 0;
                            oActivoFijo.EstadoActual = 1;


                            bd.ActivoFijo.Add(oActivoFijo);
                            bd.SaveChanges();
                            //Transaccion a tarjeta
                            TarjetaDepreciacion transaccion = new TarjetaDepreciacion();
                            ActivoFijo oActivoFijoTransaccion = bd.ActivoFijo.Last();
                            transaccion.IdBien = oActivoFijoTransaccion.IdBien;
                            transaccion.Fecha = oFormulario.FechaIngreso;
                            transaccion.Concepto = "Compra";
                            transaccion.Valor = oActivoFijoTransaccion.ValorAdquicicion;
                            transaccion.DepreciacionAnual = 0.00;
                            transaccion.DepreciacionAcumulada = 0.00;
                            transaccion.ValorActual = oActivoFijoTransaccion.ValorAdquicicion;
                            transaccion.ValorMejora = 0.00;
                            bd.TarjetaDepreciacion.Add(transaccion);
                            bd.SaveChanges();
                        }



                    }
                    if(oActivoAF.tipoactivo == 3)
                    {
                        ActivoFijo oActivoFijo = new ActivoFijo();
                        //Datos para la tabla activo fijo
                        oActivoFijo.IdBien = oActivoAF.idbien;
                        oActivoFijo.TipoActivo = oActivoAF.tipoactivo;
                        FormularioIngreso oFormulario = bd.FormularioIngreso.Last();
                        oActivoFijo.NoFormulario = oFormulario.NoFormulario;
                        oActivoFijo.Desripcion = oActivoAF.descripcion;
                        oActivoFijo.TipoAdquicicion = oActivoAF.tipoadquicicion;

                        oActivoFijo.IdClasificacion = oActivoAF.idclasificacion;
                        oActivoFijo.VidaUtil = oActivoAF.vidautil;
                        if (oActivoAF.tipoadquicicion == 3)
                        {
                            oActivoFijo.IdDonante = oActivoAF.idproveedor;
                        }
                        else
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

                        oActivoFijo.ValorAdquicicion = oActivoAF.valoradquicicion;
                        oActivoFijo.Foto = oActivoAF.foto;
                        oActivoFijo.ValorResidual = oActivoAF.valorresidual;
                        oActivoFijo.EnSolicitud = 0;
                        oActivoFijo.EstadoActual = 1;
                        oActivoFijo.EstaAsignado = 0;
                        bd.ActivoFijo.Add(oActivoFijo);
                        bd.SaveChanges();
                        //Generar codigo
                        //objeto de la clase codigo que contiene los elementos
                        CodigoAF oCodigo = new CodigoAF();
                        //Extraer los datos padres de la base
                        ActivoFijo oActivo = bd.ActivoFijo.Last();
                        AreaDeNegocio oarea = bd.AreaDeNegocio.Where(p => p.IdAreaNegocio == oActivoAF.idarea).First();
                        Sucursal osucursal = bd.Sucursal.Where(p => p.IdSucursal == oarea.IdSucursal).First();
                        Clasificacion oclasificacion = bd.Clasificacion.Where(p => p.IdClasificacion == oActivo.IdClasificacion).First();
                        

                        //LLenado de objeto
                        oCodigo.CorrelativoSucursal = osucursal.Correlativo;
                        oCodigo.CorrelativoClasificacion = oclasificacion.Correlativo;
                        oCodigo.CorrelativoArea = oarea.Correlativo;
                        //selccionar cuantos hay de esa clasificacion
                        int oActivoC = bd.ActivoFijo.Where(p => p.EstaAsignado == 1 && p.IdClasificacion == oclasificacion.IdClasificacion).Count();

                        //comparar para la concatenacion correspondiente 
                        if (oActivoC >= 0 && oActivoC <= 9)
                        {
                            oActivoC = oActivoC + 1;
                            oCodigo.Correlativo = "00" + oActivoC.ToString();
                        }
                        else if (oActivoC >= 10 && oActivoC <= 99)
                        {
                            oActivoC = oActivoC + 1;
                            oCodigo.Correlativo = "0" + oActivoC.ToString();
                        }
                        else
                        {
                            oActivoC = oActivoC + 1;
                            oCodigo.Correlativo = oActivoC.ToString();
                        }
                        oActivo.CorrelativoBien = oCodigo.CorrelativoSucursal + "-"  + oCodigo.CorrelativoArea + "-"  + oCodigo.CorrelativoClasificacion + "-" + oCodigo.Correlativo;
                        //Guardamos en la tabla activo fijo
                        oActivo.DestinoInicial = osucursal.Nombre;
                        oActivo.EstaAsignado = 1;
                        bd.SaveChanges();

                        //Transaccion a tarjeta
                        TarjetaDepreciacion transaccion = new TarjetaDepreciacion();
                        ActivoFijo oActivoFijoTransaccion = bd.ActivoFijo.Last();
                        transaccion.IdBien = oActivoFijoTransaccion.IdBien;
                        transaccion.Fecha = oFormulario.FechaIngreso;
                        transaccion.Concepto = "Compra";
                        transaccion.Valor = oActivoFijoTransaccion.ValorAdquicicion;
                        transaccion.DepreciacionAnual = 0.00;
                        transaccion.DepreciacionAcumulada = 0.00;
                        transaccion.ValorActual = oActivoFijoTransaccion.ValorAdquicicion;
                        transaccion.ValorMejora = 0.00;
                        bd.TarjetaDepreciacion.Add(transaccion);
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


        [HttpPost]
        [Route("api/FormularioIngreso/modificarFormularioIngreso")]
        public int modificarFormulario([FromBody]FormularioIngresoAF oformulario)
        {
            int rpta = 0;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    Console.WriteLine("formulario" + oformulario.noformulario);
                    FormularioIngreso oFormularioIngreso = bd.FormularioIngreso.Where(p => p.NoFormulario == oformulario.noformulario).First();
                    oFormularioIngreso.NoFormulario = oformulario.noformulario;
                    oFormularioIngreso.NoFactura = oformulario.nofactura;
                    oFormularioIngreso.FechaIngreso = oformulario.fechaingreso;
                    oFormularioIngreso.PersonaEntrega = oformulario.personaentrega;
                    oFormularioIngreso.PersonaRecibe = oformulario.personarecibe;
                    oFormularioIngreso.Observaciones = oformulario.observaciones;
                    bd.SaveChanges();

                    rpta = 1;

                }
            }
            catch (Exception ex)
            {
                rpta = 0;
                //Console.WriteLine(rpta);
            }
            return rpta;
        }

        [HttpPost]
        [Route("api/FormularioIngreso/modificarActivoFijo")]
        public int modificarActivoFijo([FromBody]ActivoModificarAF oActivo)
        {
            int rpta = 0;
            
            try
            {
               // Console.WriteLine("BIEN" + oActivoAF2.idbien);
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    ActivoFijo oActivoFijoo = bd.ActivoFijo.Where(p => p.IdBien == oActivo.idbien).First();

                    //Datos para la tabla activo fijo
                    //oActivoFijoo.IdBien = oActivoAF2.idbien;
                    //FormularioIngreso oFormulario = bd.FormularioIngreso.First();
                    List<ActivoFijo> laf = (from activo in bd.ActivoFijo
                                            join noFormulario in bd.FormularioIngreso
                                            on activo.NoFormulario equals noFormulario.NoFormulario
                                            where activo.NoFormulario == oActivoFijoo.NoFormulario
                                            && activo.EstadoActual ==1 && activo.EstaAsignado == 0 || activo.EstaAsignado==1
                                            select activo).ToList();

                    //if (oActivoAF2.tipoadquicicion == 1 || oActivoAF2.tipoadquicicion == 3)
                    //{
                    //    oActivoAF2.plazopago = "";
                    //    oActivoAF2.prima = 0;
                    //    oActivoAF2.interes = 0;
                    //    oActivoAF2.cuotaasignada = 0;

                    //}

                    foreach (var res in laf)
                    {

                            ActivoFijo oActivoFijo = bd.ActivoFijo.Where(p => p.IdBien == res.IdBien).First();

                            oActivoFijo.Desripcion = oActivo.descripcion;
                            oActivoFijo.Modelo = oActivo.modelo;
                            oActivoFijo.TipoAdquicicion = oActivo.tipoadquicicion;
                            oActivoFijo.Color = oActivo.color;
                            if(oActivo.idmarca != 0) { 
                                oActivoFijo.IdMarca = oActivo.idmarca;
                            }
                            else
                            {
                                oActivoFijo.IdMarca = null;
                            }

                          oActivoFijo.IdClasificacion = oActivo.idclasificacion;

                            if (oActivo.tipoadquicicion == 3)
                            {

                                    oActivoFijo.IdDonante = oActivo.idproveedor;
                                    oActivoFijo.IdProveedor = null;


                            }
                            else
                            {

                                    oActivoFijo.IdProveedor = oActivo.idproveedor;
                                    oActivoFijo.IdDonante = null;
                            }

                            oActivoFijo.PlazoPago = oActivo.plazopago;
                            oActivoFijo.Prima = oActivo.prima;
                            oActivoFijo.CuotaAsignanda = oActivo.cuotaasignada;
                            oActivoFijo.Intereses = oActivo.interes;
                            oActivoFijo.EstadoIngreso = oActivo.estadoingreso;
                            oActivoFijo.ValorAdquicicion = oActivo.valoradquicicion;
                            oActivoFijo.Foto = oActivo.foto;
                            oActivoFijo.ValorResidual = oActivo.valorresidual;

                        bd.SaveChanges();
                    }

                    rpta = 1;
                }
            }
            catch (Exception ex)
            {
                rpta = 0;
               // Console.WriteLine("prueba");
            }
            return rpta;
        }

        [HttpPost]
        [Route("api/FormularioIngreso/modificarActivoAsignado")]
        public int modificarActivoAsignado([FromBody]ActivoModificarAF oActivo)
        {
            int rpta = 0;

            try
            {
                // Console.WriteLine("BIEN" + oActivoAF2.idbien);
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    ActivoFijo oActivoFijoo = bd.ActivoFijo.Where(p => p.IdBien == oActivo.idbien).First();

                    //Datos para la tabla activo fijo
                    //oActivoFijoo.IdBien = oActivoAF2.idbien;
                    //FormularioIngreso oFormulario = bd.FormularioIngreso.First();
                    List<ActivoFijo> laf = (from activo in bd.ActivoFijo
                                            join noFormulario in bd.FormularioIngreso
                                            on activo.NoFormulario equals noFormulario.NoFormulario
                                            where activo.NoFormulario == oActivoFijoo.NoFormulario
                                            && activo.EstadoActual == 1 && activo.EstaAsignado == 1
                                            select activo).ToList();

                    //if (oActivoAF2.tipoadquicicion == 1 || oActivoAF2.tipoadquicicion == 3)
                    //{
                    //    oActivoAF2.plazopago = "";
                    //    oActivoAF2.prima = 0;
                    //    oActivoAF2.interes = 0;
                    //    oActivoAF2.cuotaasignada = 0;

                    //}

                   // foreach (var res in laf)
                    //{

                        ActivoFijo oActivoFijo = bd.ActivoFijo.Where(p => p.IdBien == oActivo.idbien).First();

                        oActivoFijo.Desripcion = oActivo.descripcion;
                        oActivoFijo.Modelo = oActivo.modelo;
                        oActivoFijo.TipoAdquicicion = oActivo.tipoadquicicion;
                        oActivoFijo.Color = oActivo.color;
                        if (oActivo.idmarca != 0)
                        {
                            oActivoFijo.IdMarca = oActivo.idmarca;
                        }
                        else
                        {
                            oActivoFijo.IdMarca = null;
                        }

                        oActivoFijo.IdClasificacion = oActivo.idclasificacion;

                        if (oActivo.tipoadquicicion == 3)
                        {

                            oActivoFijo.IdDonante = oActivo.idproveedor;
                            oActivoFijo.IdProveedor = null;


                        }
                        else
                        {

                            oActivoFijo.IdProveedor = oActivo.idproveedor;
                            oActivoFijo.IdDonante = null;
                        }

                        oActivoFijo.PlazoPago = oActivo.plazopago;
                        oActivoFijo.Prima = oActivo.prima;
                        oActivoFijo.CuotaAsignanda = oActivo.cuotaasignada;
                        oActivoFijo.Intereses = oActivo.interes;
                        oActivoFijo.EstadoIngreso = oActivo.estadoingreso;
                        oActivoFijo.ValorAdquicicion = oActivo.valoradquicicion;
                        oActivoFijo.Foto = oActivo.foto;
                        oActivoFijo.ValorResidual = oActivo.valorresidual;

                        bd.SaveChanges();
                    //}

                    rpta = 1;
                }
            }
            catch (Exception ex)
            {
                rpta = 0;
                // Console.WriteLine("prueba");
            }
            return rpta;
        }

        //Modificar edificios e instalaciones
        [HttpPost]
        [Route("api/FormularioIngreso/modificarEdificiosInstalaciones")]
        public int modificarEdificiosInstalaciones([FromBody] ActivoEdificiosIntangiblesAF oActivoEdi)
        {
            int rpta = 0;

            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                  
                        ActivoFijo oActivoFijo = bd.ActivoFijo.Where(p => p.IdBien == oActivoEdi.idbien).First();

                        oActivoFijo.Desripcion = oActivoEdi.descripcion;
                        oActivoFijo.TipoAdquicicion = oActivoEdi.tipoadquicicion;
                        oActivoFijo.IdClasificacion = oActivoEdi.idclasificacion;

                        if (oActivoEdi.tipoadquicicion == 3)
                        {

                            oActivoFijo.IdDonante = oActivoEdi.idproveedor;
                            oActivoFijo.IdProveedor = null;

                        }
                        else
                        {

                            oActivoFijo.IdProveedor = oActivoEdi.idproveedor;
                            oActivoFijo.IdDonante = null;
                        }

                        oActivoFijo.PlazoPago = oActivoEdi.plazopago;
                        oActivoFijo.Prima = oActivoEdi.prima;
                        oActivoFijo.CuotaAsignanda = oActivoEdi.cuotaasignada;
                        oActivoFijo.Intereses = oActivoEdi.interes;
                        oActivoFijo.ValorAdquicicion = oActivoEdi.valoradquicicion;
                        oActivoFijo.Foto = oActivoEdi.foto;
                        oActivoFijo.ValorResidual = oActivoEdi.valorresidual;
                        oActivoFijo.VidaUtil = oActivoEdi.vidautil;

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
    

