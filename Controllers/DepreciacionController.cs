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
        //Metodo que lista los bienes en la tabla depreciación, validados si estan ya depreciados en el periodo actual.
        [HttpGet]
        [Route("api/Depreciacion/listarActivosDepreciacion")]
        public IEnumerable<DepreciacionAF> listarActivosDepreciacion()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                Periodo anioActual = bd.Periodo.Where(p => p.Estado == 1).FirstOrDefault();
                IEnumerable<DepreciacionAF> listaActivos = (from tarjeta in bd.TarjetaDepreciacion
                                                            group tarjeta by tarjeta.IdBien into bar
                                                            join activo in bd.ActivoFijo
                                                           on bar.FirstOrDefault().IdBien equals activo.IdBien
                                                            join empleado in bd.Empleado
                                                            on activo.IdResponsable equals empleado.IdEmpleado
                                                            join area in bd.AreaDeNegocio
                                                            on empleado.IdAreaDeNegocio equals area.IdAreaNegocio
                                                            join sucursal in bd.Sucursal
                                                            on area.IdSucursal equals sucursal.IdSucursal
                                                          
                                                  

                                                            where (activo.EstadoActual != 0) && (activo.UltimoAnioDepreciacion == null || (activo.UltimoAnioDepreciacion < (anioActual.Anio)))&&(bar.OrderByDescending(x => x.IdTarjeta).First().ValorActual>0)
                                                            select new DepreciacionAF
                                                            {
                                                                idBien=activo.IdBien,
                                                                codigo = activo.CorrelativoBien,
                                                                descripcion = activo.Desripcion,
                                                                areanegocio = area.Nombre,
                                                                sucursal=sucursal.Nombre,
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

                                                            where (activo.EstadoActual !=0) && (activo.EstaAsignado == 0 || activo.EstaAsignado == 1)

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
        //Metodo que sirve para filtrar los datos en la tabla depreciacion, con respecto a lo seleccionado en el combo.
        [HttpGet]
        [Route("api/Depreciacion/listarActivosDepreciacionFiltro/{id}")]
        public IEnumerable<DepreciacionAF> listarActivosDepreciacionFiltro(int id)
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                Periodo anioActual = bd.Periodo.Where(p => p.Estado == 1).FirstOrDefault();
                IEnumerable<DepreciacionAF> listaActivos = (from tarjeta in bd.TarjetaDepreciacion
                                                            group tarjeta by tarjeta.IdBien into bar
                                                            join activo in bd.ActivoFijo
                                                           on bar.FirstOrDefault().IdBien equals activo.IdBien
                                                            join empleado in bd.Empleado
                                                            on activo.IdResponsable equals empleado.IdEmpleado
                                                            join area in bd.AreaDeNegocio
                                                            on empleado.IdAreaDeNegocio equals area.IdAreaNegocio
                                                            join sucursal in bd.Sucursal
                                                            on area.IdSucursal equals sucursal.IdSucursal



                                                            where (activo.EstadoActual != 0) && (activo.UltimoAnioDepreciacion == null || (activo.UltimoAnioDepreciacion < (anioActual.Anio))) && (bar.OrderByDescending(x => x.IdTarjeta).First().ValorActual > 0)&&area.IdAreaNegocio==id
                                                            select new DepreciacionAF
                                                            {
                                                                idBien = activo.IdBien,
                                                                codigo = activo.CorrelativoBien,
                                                                descripcion = activo.Desripcion,
                                                                areanegocio = area.Nombre,
                                                                sucursal = sucursal.Nombre,
                                                                responsable = empleado.Nombres + " " + empleado.Apellidos


                                                            }).ToList();
                return listaActivos;
            }
        }
        //Metodo de filtro para la tabla tarjeta
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
        //Metodo para bscar en la tabla tarjeta, resibe una cade de texto que compara con todos los datos de la tabla,
        [HttpGet]
        [Route("api/Depreciacion/buscarActivos/{buscador?}")]
        public IEnumerable<DepreciacionAF> buscarActivos(string buscador = "")
        {
            List<DepreciacionAF> listaActivo;
    
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                Periodo anioActual = bd.Periodo.Where(p => p.Estado == 1).FirstOrDefault();
                if (buscador == "")
                {
                    listaActivo = (from tarjeta in bd.TarjetaDepreciacion
                                   group tarjeta by tarjeta.IdBien into bar
                                   join activo in bd.ActivoFijo
                                  on bar.FirstOrDefault().IdBien equals activo.IdBien
                                   join empleado in bd.Empleado
                                   on activo.IdResponsable equals empleado.IdEmpleado
                                   join area in bd.AreaDeNegocio
                                   on empleado.IdAreaDeNegocio equals area.IdAreaNegocio
                                   join sucursal in bd.Sucursal
                                   on area.IdSucursal equals sucursal.IdSucursal
                                   where (activo.EstadoActual != 0) && (activo.UltimoAnioDepreciacion == null || (activo.UltimoAnioDepreciacion < (anioActual.Anio))) && (bar.OrderByDescending(x => x.IdTarjeta).First().ValorActual > 0)
                                   select new DepreciacionAF
                                   {
                                       idBien = activo.IdBien,
                                       codigo = activo.CorrelativoBien,
                                       descripcion = activo.Desripcion,
                                       areanegocio = area.Nombre,
                                       sucursal = sucursal.Nombre,
                                       responsable = empleado.Nombres + " " + empleado.Apellidos


                                   }).ToList();

                    return listaActivo;
                }
                else
                {
                    listaActivo = (from tarjeta in bd.TarjetaDepreciacion
                                   group tarjeta by tarjeta.IdBien into bar
                                   join activo in bd.ActivoFijo
                                   on bar.FirstOrDefault().IdBien equals activo.IdBien
                                   join empleado in bd.Empleado
                                   on activo.IdResponsable equals empleado.IdEmpleado
                                   join area in bd.AreaDeNegocio
                                   on empleado.IdAreaDeNegocio equals area.IdAreaNegocio
                                   join sucursal in bd.Sucursal
                                   on area.IdSucursal equals sucursal.IdSucursal
                                   where (activo.EstadoActual != 0) && (activo.UltimoAnioDepreciacion == null || (activo.UltimoAnioDepreciacion < (anioActual.Anio))) && (bar.OrderByDescending(x => x.IdTarjeta).First().ValorActual > 0)
                                   &&
                                      ((activo.CorrelativoBien).ToString().ToLower().Contains(buscador.ToLower())
                                      || (activo.Desripcion).ToString().ToLower().Contains(buscador.ToLower())
                                      || (area.Nombre).ToLower().Contains(buscador.ToLower())
                                      || (sucursal.Nombre).ToLower().Contains(buscador.ToLower())
                                      || (empleado.Nombres).ToLower().Contains(buscador.ToLower())
                                      ||(empleado.Apellidos).ToLower().Contains(buscador.ToLower())
                                      )
                                   select new DepreciacionAF
                                   {
                                       idBien = activo.IdBien,
                                       codigo = activo.CorrelativoBien,
                                       descripcion = activo.Desripcion,
                                       areanegocio = area.Nombre,
                                       sucursal = sucursal.Nombre,
                                       responsable = empleado.Nombres + " " + empleado.Apellidos
                                   }).ToList();

                    return listaActivo;
                }
            }
        }
        //Metodo para buscar en la tabla tarjet, muy similar al anterior pero este no valida si ya se le realizó depreciacion en el periodo actual.
        [HttpGet]
        [Route("api/Depreciacion/buscarActivosTarjeta/{buscador?}")]
        public IEnumerable<DepreciacionAF> buscarActivosTarjeta(string buscador = "")
        {
            List<DepreciacionAF> listaActivo;

            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                if (buscador == "")
                {
                    listaActivo = (from activo in bd.ActivoFijo
                                   join empleado in bd.Empleado
                                   on activo.IdResponsable equals empleado.IdEmpleado
                                   join area in bd.AreaDeNegocio
                                   on empleado.IdAreaDeNegocio equals area.IdAreaNegocio
                                   join sucursal in bd.Sucursal
                                   on area.IdSucursal equals sucursal.IdSucursal

                                   where (activo.EstadoActual != 0) && (activo.EstaAsignado == 0 || activo.EstaAsignado == 1)

                                   select new DepreciacionAF
                                   {
                                       idBien = activo.IdBien,
                                       codigo = activo.CorrelativoBien,
                                       descripcion = activo.Desripcion,
                                       areanegocio = area.Nombre,
                                       sucursal = sucursal.Nombre,
                                       responsable = empleado.Nombres + " " + empleado.Apellidos
                                   }).ToList();

                    return listaActivo;
                }
                else
                {
                    listaActivo = (from activo in bd.ActivoFijo
                                   join empleado in bd.Empleado
                                   on activo.IdResponsable equals empleado.IdEmpleado
                                   join area in bd.AreaDeNegocio
                                   on empleado.IdAreaDeNegocio equals area.IdAreaNegocio
                                   join sucursal in bd.Sucursal
                                   on area.IdSucursal equals sucursal.IdSucursal
                                   where (activo.EstadoActual != 0) && (activo.EstaAsignado == 0 || activo.EstaAsignado == 1) &&
                                      ((activo.CorrelativoBien).ToString().ToLower().Contains(buscador.ToLower())
                                      || (activo.Desripcion).ToString().ToLower().Contains(buscador.ToLower())
                                      || (area.Nombre).ToLower().Contains(buscador.ToLower())
                                      || (sucursal.Nombre).ToLower().Contains(buscador.ToLower())
                                      || (empleado.Nombres).ToLower().Contains(buscador.ToLower())
                                      || (empleado.Apellidos).ToLower().Contains(buscador.ToLower())
                                      )
                                   select new DepreciacionAF
                                   {
                                       idBien = activo.IdBien,
                                       codigo = activo.CorrelativoBien,
                                       descripcion = activo.Desripcion,
                                       areanegocio = area.Nombre,
                                       sucursal = sucursal.Nombre,
                                       responsable = empleado.Nombres + " " + empleado.Apellidos
                                   }).ToList();

                    return listaActivo;
                }
            }
        }
        //Metodo para mostrar un Object con los datos del activo en en el modal para aplicar depreciación
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
        //Metodo que devuelve un objeto con los datos generales en la tajeta de depecioacion.
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
                int tasa=(int)(100 / oactivo.VidaUtil);
                odatos.tasaAnual = tasa.ToString();
                odatos.vidaUtil = oactivo.VidaUtil.ToString();
                odatos.valorResidual = oactivo.ValorResidual.ToString();

                if (oactivo.IdProveedor != null)
                {
                    Proveedor oProveedor = bd.Proveedor.Where(p => p.IdProveedor == oactivo.IdProveedor).First();
                    odatos.proveedor = oProveedor.Nombre;
                    odatos.direccion = oProveedor.Direccion;
                    odatos.telefono = oProveedor.Telefono;
                    odatos.isProvDon = 1;
                }
                else {
                    Donantes oDonante = bd.Donantes.Where(p => p.IdDonante == oactivo.IdDonante).First();
                    odatos.proveedor = oDonante.Nombre;
                    odatos.direccion = oDonante.Direccion;
                    odatos.telefono = oDonante.Telefono;
                    odatos.isProvDon = 2;
                }
              
                if (oactivo.IdMarca != null)
                {
                    Marcas oMarcas = bd.Marcas.Where(p => p.IdMarca == oactivo.IdMarca).First();
                    odatos.marca = oMarcas.Marca;
                }
                else
                {
                    odatos.marca = " ";
                }
                return odatos;

            }
        }
        //Metodo que lista las transacciones por cada activo y lo muestra en la tarjeta.
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
                                                                depreciacionAcumulada = tarjeta.DepreciacionAcumulada.ToString(),
                                                                 valorActual=tarjeta.ValorActual.ToString(),
                                                                valorMejora=tarjeta.ValorMejora.ToString()
                                                                         }).ToList();
                return ListaTransacciones;
            }
        }
        //Metodo con el que se guarda la transaccion de la depreciacion que se realiza.
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
                        double valorAcumulado = (double)oUltimaTransaccion.DepreciacionAcumulada + oActivoAF.valorDepreciacion; 
                        transaccion.DepreciacionAcumulada = Math.Round(valorAcumulado,3);
                        //transaccion.DepreciacionAcumulada = valorAcumulado;
                        double valor= (double)oUltimaTransaccion.ValorActual - oActivoAF.valorDepreciacion;
                        double rounded = Math.Round(valor,3);
                        transaccion.ValorActual = rounded;
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
        //Metodo que recupera losdatos necesarios para el cierre de periodo.
        [HttpGet]
        [Route("api/Depreciacion/DatosCierre")]
        public CierreAF DatosCierre()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                CierreAF odatos = new CierreAF();
                Cooperativa oCooperativa = bd.Cooperativa.Where(p => p.Dhabilitado == 1).First();
                Periodo oPeriodo = bd.Periodo.Where(p => p.Estado == 1).First();
                odatos.anio =oPeriodo.Anio.ToString();
                odatos.cooperativa = oCooperativa.Nombre;
                odatos.idPeriodo = oPeriodo.IdPeriodo;
                return odatos;
            }
        }
        //Metodo que ejecuta el cierre del año actual.
        [HttpPost]
        [Route("api/Depreciacion/EjecutarCierre")]
        public int EjecutarCierre([FromBody]CierreAF oCierreAF)
        {
            int rpta = 0;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    Periodo oPeriodo = bd.Periodo.Where(p => p.IdPeriodo == oCierreAF.idPeriodo).First();
                    oPeriodo.Anio = oPeriodo.Anio+1;
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
        [HttpGet]
        [Route("api/Depreciacion/recuperarFoto/{id}")]
        public ActivoAF recuperarFoto(int id)
        {

            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                ActivoAF oActivoAF = new ActivoAF();
                ActivoFijo oActivo = bd.ActivoFijo.Where(p => p.IdBien == id).First();
                oActivoAF.foto = oActivo.Foto;
                return oActivoAF;
            }

        }
       

    }
}