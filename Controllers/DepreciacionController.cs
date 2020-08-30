using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ASGARDAPI.Clases;
using ASGARDAPI.Models;

namespace ASGARDAPI.Controllers
{
    public class DepreciacionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        [Route("api/Depreciacion/listarActivosDepreciacion")]
        public IEnumerable<DepreciacionAF> listarActivosDepreciacion()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                Periodo anioActual = bd.Periodo.Where(p => p.Estado == 1).FirstOrDefault();
                IEnumerable<DepreciacionAF> listaActivos = (from activo in bd.ActivoFijo
                                                            join empleado in bd.Empleado
                                                            on activo.IdResponsable equals empleado.IdEmpleado
                                                            join area in bd.AreaDeNegocio
                                                            on empleado.IdAreaDeNegocio equals area.IdAreaNegocio
                                                            join sucursal in bd.Sucursal
                                                            on area.IdSucursal equals sucursal.IdSucursal
                                                         
                                                            where (activo.EstadoActual == 1 || activo.EstadoActual == 2) &&( activo.EstaAsignado==0 || activo.EstaAsignado==1)&& (activo.UltimoAnioDepreciacion==null ||(activo.UltimoAnioDepreciacion<(anioActual.Anio)))
                                                            
                                                            select new DepreciacionAF
                                                            {
                                                                idBien=activo.IdBien,
                                                                codigo = activo.CorrelativoBien,
                                                                descripcion = activo.Desripcion,
                                                                areanegocio = area.Nombre,
                                                            
                                                                responsable = empleado.Nombres + " " + empleado.Apellidos
                                                                
                                                              
                                                            }).ToList();
                return listaActivos;
            }
        }
        //Lista de la tarjeta a diferencia de la depreciacion no valida la ultima deoreciacion realizada
        [HttpGet]
        [Route("api/Depreciacion/listarActivosTarjeta")]
        public IEnumerable<DepreciacionAF> listarActivosTarjeta()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {

                IEnumerable<DepreciacionAF> listaActivos = (from activo in bd.ActivoFijo
                                                            join empleado in bd.Empleado
                                                            on activo.IdResponsable equals empleado.IdEmpleado
                                                            join area in bd.AreaDeNegocio
                                                            on empleado.IdAreaDeNegocio equals area.IdAreaNegocio
                                                            join sucursal in bd.Sucursal
                                                            on area.IdSucursal equals sucursal.IdSucursal

                                                            where (activo.EstadoActual == 1 || activo.EstadoActual == 2) && (activo.EstaAsignado == 0 || activo.EstaAsignado == 1)

                                                            select new DepreciacionAF
                                                            {
                                                                idBien = activo.IdBien,
                                                                codigo = activo.CorrelativoBien,
                                                                descripcion = activo.Desripcion,
                                                                areanegocio = area.Nombre,
                                                                sucursal=sucursal.Nombre,
                                                                responsable = empleado.Nombres + " " + empleado.Apellidos


                                                            }).ToList();
                return listaActivos;
            }
        }
        [HttpGet]
        [Route("api/Depreciacion/listarActivosDepreciacionFiltro/{id}")]
        public IEnumerable<DepreciacionAF> listarActivosDepreciacionFiltro(int id)
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                Periodo anioActual = bd.Periodo.Where(p => p.Estado == 1).FirstOrDefault();
                IEnumerable<DepreciacionAF> listaActivos = (from activo in bd.ActivoFijo
                                                            join empleado in bd.Empleado
                                                            on activo.IdResponsable equals empleado.IdEmpleado
                                                            join area in bd.AreaDeNegocio
                                                            on empleado.IdAreaDeNegocio equals area.IdAreaNegocio
                                                            join sucursal in bd.Sucursal
                                                           on area.IdSucursal equals sucursal.IdSucursal
                                                            where (activo.EstadoActual == 1 || activo.EstadoActual == 2) && (activo.UltimoAnioDepreciacion == null || (activo.UltimoAnioDepreciacion < (anioActual.Anio))) && area.IdAreaNegocio==id
                                                            select new DepreciacionAF
                                                            {
                                                                codigo = activo.CorrelativoBien,
                                                                descripcion = activo.Desripcion,
                                                                areanegocio = area.Nombre,
                                                                sucursal=sucursal.Nombre,
                                                                responsable = empleado.Nombres + " " + empleado.Apellidos
                                                            }).ToList();
                return listaActivos;
            }
        }
        [HttpGet]
        [Route("api/Depreciacion/listarActivosTarjetaFiltro/{id}")]
        public IEnumerable<DepreciacionAF> listarActivosTarjetaFiltro(int id)
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<DepreciacionAF> listaActivos = (from activo in bd.ActivoFijo
                                                            join empleado in bd.Empleado
                                                            on activo.IdResponsable equals empleado.IdEmpleado
                                                            join area in bd.AreaDeNegocio
                                                            on empleado.IdAreaDeNegocio equals area.IdAreaNegocio
                                                            join sucursal in bd.Sucursal
                                                           on area.IdSucursal equals sucursal.IdSucursal
                                                            where (activo.EstadoActual == 1 || activo.EstadoActual == 2) && area.IdAreaNegocio == id
                                                            select new DepreciacionAF
                                                            {
                                                                codigo = activo.CorrelativoBien,
                                                                descripcion = activo.Desripcion,
                                                                areanegocio = area.Nombre,
                                                                sucursal = sucursal.Nombre,
                                                                responsable = empleado.Nombres + " " + empleado.Apellidos
                                                            }).ToList();
                return listaActivos;
            }
        }
        [HttpGet]
        [Route("api/Depreciacion/DatosDepreciacion/{idBien}")]
        public BienesDepreciacionAF DatosDepreciacion(int idBien)
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                BienesDepreciacionAF odatos = new BienesDepreciacionAF();
                ActivoFijo oactivo = bd.ActivoFijo.Where(p => p.IdBien == idBien).First();
                TarjetaDepreciacion oTarjeta = bd.TarjetaDepreciacion.Where(p => p.IdBien == idBien).Last();
                Cooperativa oCooperativa = bd.Cooperativa.Where(p => p.Dhabilitado == 1).First();
                Periodo oPeriodo =bd.Periodo.Where(p => p.Estado == 1).First();
                odatos.cooperativa = oCooperativa.Nombre;
                odatos.anio = oPeriodo.Anio.ToString();
                odatos.idBien = oactivo.IdBien;
                //DateTime _date = DateTime.Now;
                //var _dateString = _date.ToString("yyyy");
                //odatos.fecha= _dateString;
                odatos.codigo = oactivo.CorrelativoBien;
                odatos.descipcion = oactivo.Desripcion;
                odatos.valorAdquicicon = oactivo.ValorAdquicicion.ToString();
                odatos.valorActual = oTarjeta.ValorActual.ToString();
                double valor=0.00;
                valor =(double) (oTarjeta.Valor / oactivo.VidaUtil);
                odatos.valorDepreciacion = valor;
                odatos.vidaUtil =(int) oactivo.VidaUtil;
                return odatos;
            }
        }
        [HttpGet]
        [Route("api/Depreciacion/TarjetaDatos/{idBien}")]
        public TarjetaDatosAF TarjetaDatos(int idBien)
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                TarjetaDatosAF odatos = new TarjetaDatosAF();
                ActivoFijo oactivo = bd.ActivoFijo.Where(p => p.IdBien == idBien).First();
                FormularioIngreso oformulario = bd.FormularioIngreso.Where(p => p.NoFormulario == oactivo.NoFormulario).First();
                odatos.fechaAdquicicion = oformulario.FechaIngreso == null ? " " : ((DateTime)oformulario.FechaIngreso).ToString("dd-MM-yyyy");
                odatos.Observaciones = oformulario.Observaciones;
                odatos.codigo = oactivo.CorrelativoBien;
                odatos.descripcion = oactivo.Desripcion;
                odatos.valor = oactivo.ValorAdquicicion.ToString();
                odatos.prima =oactivo.Prima.ToString();
                odatos.plazo = oactivo.PlazoPago;
                odatos.cuota =oactivo.CuotaAsignanda.ToString();
                odatos.interes =oactivo.Intereses.ToString();
                odatos.color = oactivo.Color;
                odatos.modelo = oactivo.Modelo;
                odatos.noSerie = oactivo.NoSerie;
                odatos.vidaUtil = oactivo.VidaUtil.ToString();
                odatos.valorResidual = oactivo.ValorResidual.ToString();
                Proveedor oProveedor = bd.Proveedor.Where(p => p.IdProveedor == oactivo.IdProveedor).First();
                odatos.proveedor = oProveedor.Nombre;
                odatos.direccion = oProveedor.Direccion;
                odatos.telefono = oProveedor.Telefono;
                Marcas oMarcas = bd.Marcas.Where(p => p.IdMarca == oactivo.IdMarca).First();
                odatos.marca = oMarcas.Marca;
                return odatos;

            }
        }
        [HttpGet]
        [Route("api/Depreciacion/TarjetaListaTrasacciones/{id}")]
        public IEnumerable<TarjetaTransaccionesAF> TarjetaListaTrasacciones(int id)
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<TarjetaTransaccionesAF> ListaTransacciones = (from tarjeta in bd.TarjetaDepreciacion
                                                                         where tarjeta.IdBien == id
                                                                         orderby tarjeta.IdTarjeta
                                                                         select new TarjetaTransaccionesAF
                                                                         {
                                                                id = tarjeta.IdTarjeta,
                                                                idBien = (int)tarjeta.IdBien,
                                                                fecha = tarjeta.Fecha == null ? " " : ((DateTime)tarjeta.Fecha).ToString("dd-MM-yyyy"),
                                                                concepto = tarjeta.Concepto,
                                                                 montoTransaccion = tarjeta.Valor.ToString(),
                                                               depreciacionAnual=tarjeta.DepreciacionAnual.ToString(),
                                                                depreciacionAcumulada=tarjeta.DepreciacionAcumulada.ToString(),
                                                                 valorActual=tarjeta.ValorActual.ToString(),
                                                                valorMejora=tarjeta.ValorMejora.ToString()
                                                                         }).ToList();
                return ListaTransacciones;
            }
        }
        [HttpPost]
        [Route("api/Depreciacion/transaccionDepreciacion")]
        public int transaccionDepreciacion([FromBody] DatosDepreciacionAF oActivoAF)
        {
            int rpta = 0;

            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    //Transaccion a tarjeta
                      
                        TarjetaDepreciacion transaccion = new TarjetaDepreciacion();
                        TarjetaDepreciacion oUltimaTransaccion = bd.TarjetaDepreciacion.Where(p => p.IdBien == oActivoAF.idBien).Last();
                        
                        //ActivoFijo oActivoFijoTransaccion = bd.ActivoFijo.Last();
                        transaccion.IdBien = oActivoAF.idBien;
                        transaccion.Fecha =  oActivoAF.fecha;
                        transaccion.Concepto = "Depreciación";
                        transaccion.Valor = oUltimaTransaccion.Valor;
                        transaccion.DepreciacionAnual = oActivoAF.valorDepreciacion;
                        transaccion.DepreciacionAcumulada = oUltimaTransaccion.DepreciacionAcumulada+oActivoAF.valorDepreciacion;
                        transaccion.ValorActual = oUltimaTransaccion.ValorActual-oActivoAF.valorDepreciacion;
                        transaccion.ValorMejora = 0.00;
                        bd.TarjetaDepreciacion.Add(transaccion);
                        bd.SaveChanges();
                    //cambia ultimo anio de depreciación

                    ActivoFijo activo = bd.ActivoFijo.Where(p => p.IdBien == oActivoAF.idBien).First();
                    Periodo anioActual = bd.Periodo.Where(p => p.Estado == 1).FirstOrDefault();
                    activo.UltimoAnioDepreciacion = anioActual.Anio;
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