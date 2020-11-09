using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ASGARDAPI.Models;
using ASGARDAPI.Clases;
using System.Threading;

namespace ASGARDAPI.Controllers
{
    public class InformeMantenimientoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        //METODO PARA GUARDAR EL INFORME DE LOS BIENES EN MANTENIMIENTO.

        [HttpPost]
        [Route("api/InformeMantenimiento/guardarInformeMantenimiento")]
        public int guardarInformeMantenimiento([FromBody]InformeMatenimientoAF oInformeMantenimientoAF)
        {
            int respuesta = 0;

            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    InformeMantenimiento oInformeMantenimiento = new InformeMantenimiento();
                    oInformeMantenimiento.IdInformeMantenimiento = oInformeMantenimientoAF.idinformematenimiento;
                    oInformeMantenimiento.IdMantenimiento = oInformeMantenimientoAF.idmantenimiento;
                    oInformeMantenimiento.IdTecnico = oInformeMantenimientoAF.idtecnico;
                    oInformeMantenimiento.Fecha = oInformeMantenimientoAF.fechainforme;
                    oInformeMantenimiento.Descripcion = oInformeMantenimientoAF.descripcion;
                    oInformeMantenimiento.CostoMateriales = oInformeMantenimientoAF.costomateriales;
                    oInformeMantenimiento.CostoMo = oInformeMantenimientoAF.costomo;
                    oInformeMantenimiento.CostoTotal = oInformeMantenimientoAF.costomateriales + oInformeMantenimientoAF.costomo;
                    bd.InformeMantenimiento.Add(oInformeMantenimiento);
                    oInformeMantenimiento.Estado = 1;
                    
                    bd.SaveChanges();
                    respuesta = 1;

                }


            }
            catch (Exception ex)
            {

                respuesta = 0;
            }
            return respuesta;
        }

        //guardar revalorización 

            [HttpPost]
           [Route("api/InformeMantenimiento/insertarRevalorizacion")]
            public int  insertarRevalorizacion([FromBody] DatosDepreciacionAF oActivoAF)
        {
            int respuesta = 0;
            try
            {
                using (BDAcaassAFContext bd= new BDAcaassAFContext())
                {
                    TarjetaDepreciacion transaccion = new TarjetaDepreciacion();
                    TarjetaDepreciacion oUltimaTransaccion = bd.TarjetaDepreciacion.Where(p => p.IdBien == oActivoAF.idBien).Last();

                    transaccion.IdBien = oActivoAF.idBien;
                    transaccion.Fecha = oActivoAF.fecha;
                    transaccion.ValorMejora =oActivoAF.valorRevalorizacion;
                    transaccion.Concepto = "Revalorización";
                    transaccion.Valor = oUltimaTransaccion.Valor + oActivoAF.valorRevalorizacion;
                    transaccion.DepreciacionAnual = 0.00;
                    transaccion.DepreciacionAcumulada = 0.00;

                

                    // al valor actual tambien se le va a sumar el valor de la revalorización.
                    transaccion.ValorActual = oUltimaTransaccion.ValorActual + oActivoAF.valorRevalorizacion;
                   
                    bd.TarjetaDepreciacion.Add(transaccion);
                    bd.SaveChanges();

                    //para modificar la vida util en caso que el administrador quiera hacerlo.

                    ActivoFijo activo = bd.ActivoFijo.Where(p => p.IdBien == oActivoAF.idBien).First();
                    activo.VidaUtil = oActivoAF.vidaUtil;
                    bd.SaveChanges();
                    respuesta = 1;
                }

            }
            catch (Exception ex)
            {

                respuesta = 0;
            }
            return respuesta;
        }

        //metodo para buscar informes.
        [HttpGet]
        [Route("api/InformeMantenimiento/buscarInformes/{buscador?}")]
        public IEnumerable<InformeMatenimientoAF> buscarInformes(string buscador = "")
        {
            List<InformeMatenimientoAF> listarInformeMantenimiento;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                if (buscador == "")
                {
                    listarInformeMantenimiento = (from tecnico in bd.Tecnicos
                                                  join informemante in bd.InformeMantenimiento
                                                  on tecnico.IdTecnico equals informemante.IdTecnico
                                                  join bienmante in bd.BienMantenimiento
                                                  on informemante.IdMantenimiento equals bienmante.IdMantenimiento
                                                  join bienes in bd.ActivoFijo
                                                  on bienmante.IdBien equals bienes.IdBien
                                                  where informemante.Estado == 1


                                                  select new InformeMatenimientoAF
                                                  {
                                                      fechacadena = informemante.Fecha.ToString(),

                                                      //fechacadena = informemante.Fecha == null ? " " : ((DateTime)informemante.Fecha).ToString("dd-MM-yyyy"),
                                                      nombretecnico = tecnico.Nombre,
                                                      descripcion = informemante.Descripcion,
                                                      costomateriales = (double)informemante.CostoMateriales,
                                                      costomo = (double)informemante.CostoMo,
                                                      costototal = (double)informemante.CostoTotal,
                                                      bienes = bienes.Desripcion

                                                  }).ToList();
                    return listarInformeMantenimiento;
                }
                else
                {
                    listarInformeMantenimiento = (from tecnico in bd.Tecnicos
                                                  join informemante in bd.InformeMantenimiento
                                                  on tecnico.IdTecnico equals informemante.IdTecnico
                                                  join bienmante in bd.BienMantenimiento
                                                  on informemante.IdMantenimiento equals bienmante.IdMantenimiento
                                                  join bienes in bd.ActivoFijo
                                                  on bienmante.IdBien equals bienes.IdBien
                                                  where informemante.Estado == 1

                                      && ((informemante.Fecha).ToString().Contains(buscador.ToString()) ||
                                     (tecnico.Nombre).ToLower().Contains(buscador.ToLower()) ||
                                     (bienes.Desripcion).ToString().ToLower().Contains(buscador.ToLower()) ||
                                     (informemante.CostoMateriales).ToString().Contains(buscador.ToString()) ||
                                     (informemante.CostoMo).ToString().Contains(buscador.ToString()) ||
                                     (informemante.CostoTotal).ToString().Contains(buscador.ToString()) ||
                                     (informemante.Descripcion).ToString().Contains(buscador.ToLower()))

                                                  select new InformeMatenimientoAF
                                                  {
                                                      
                                                      fechacadena = informemante.Fecha.ToString(),
                                                     // fechacadena = informemante.Fecha == null ? " " : ((DateTime)informemante.Fecha).ToString("dd-MM-yyyy"),
                                                      nombretecnico = tecnico.Nombre,
                                                      descripcion = informemante.Descripcion,
                                                      costomateriales = (double)informemante.CostoMateriales,
                                                      costomo = (double)informemante.CostoMo,
                                                      costototal = (double)informemante.CostoTotal,
                                                      bienes = bienes.Desripcion
                                                  }).ToList();
                    return listarInformeMantenimiento;
                }
            }
        }

        //Para buscar los activos en el historial.
        [HttpGet]
        [Route("api/InformeMantenimiento/buscarActivoHistorial/{buscador?}")]
        public IEnumerable<DepreciacionAF> buscarActivoHistorial(string buscador = "")
        {
            List<DepreciacionAF> listaActivo;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
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
                                   where activo.EstaAsignado == 1

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

                                   where activo.EstaAsignado == 1

                                   && ((activo.CorrelativoBien).ToLower().Contains(buscador.ToLower())
                                    || (activo.Desripcion).ToLower().Contains(buscador.ToLower())
                                    || (area.Nombre).ToLower().Contains(buscador.ToLower())
                                    || (empleado.Nombres).ToLower().Contains(buscador.ToLower())
                                    || (empleado.Apellidos).ToLower().Contains(buscador.ToLower()))

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
        //listar bienes en manteniento
        [HttpGet]
        [Route("api/InformeMantenimiento/listarBienesMantenimientoInforme")]
        public IEnumerable<BienesSolicitadosMttoAF> listarBienesMantenimientoInforme()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<BienesSolicitadosMttoAF> lista = (from bienMtto in bd.BienMantenimiento
                                                              join activo in bd.ActivoFijo
                                                              on bienMtto.IdBien equals activo.IdBien
                                                              //  join informe in bd.InformeMantenimiento
                                                              // on bienMtto.IdMantenimiento equals informe.IdMantenimiento
                                                              where activo.EstadoActual == 3 && bienMtto.Estado == 1 //ELEMENTO 2 LISTA
                                                              select new BienesSolicitadosMttoAF
                                                              {
                                                                  idBien = activo.IdBien,
                                                                  idmantenimiento = bienMtto.IdMantenimiento,
                                                                  estadoActual = (int)activo.EstadoActual,
                                                                  Codigo = activo.CorrelativoBien,
                                                                  Descripcion = activo.Desripcion,
                                                                  Periodo = bienMtto.PeriodoMantenimiento,
                                                                  Razon = bienMtto.RazonMantenimiento

                                                              }).ToList();


                return lista;
            }

        }

        //cambia el estado una vez que el bien se le realizó el informe
        [HttpGet]
        [Route("api/InformeMantenimiento/cambiarEstadoActivoMantenimiento/{idBien}")]
        public int cambiarEstadoActivoMantenimiento(int idBien)
        {
            int respuesta = 0;

            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    ActivoFijo oActivo = bd.ActivoFijo.Where(p => p.IdBien == idBien).Last();
                    BienMantenimiento obienmantenimiento = bd.BienMantenimiento.Where(p => p.IdBien == idBien).Last();                   
                    obienmantenimiento.Estado = 5; //cambiamos el estado a 2 para que ya no liste en bienes en mantenimeitno// ELEMENTO 3 SIRVE
                    oActivo.EstadoActual = 1;
                    bd.SaveChanges();
                    respuesta = 1;

                }


            }
            catch (Exception ex)
            {

                respuesta = 0;
            }
            return respuesta;
        }




        //cambiar el estado de del informe para que desaparezca depues de que se aplicar la revalorización
        [HttpGet]
        [Route("api/InformeMantenimiento/estadoInformeRevalorizado/{idinformeMantenimiento}")]
        public int estadoInformeRevalorizado(int idinformeMantenimiento)
        {
            int respuesta = 0;

            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    InformeMantenimiento oInformeMantenimiento = bd.InformeMantenimiento.Where(p => p.IdInformeMantenimiento == idinformeMantenimiento).First();
                    oInformeMantenimiento.Estado = 2;// significa que aplicó revalorización 
                    bd.SaveChanges();
                    respuesta = 1;

                }


            }
            catch (Exception ex)
            {

                respuesta = 0;
            }
            return respuesta;
        }

        //cambiar el estado de del informe para que desaparezca cuando no aplique revalorización
        [HttpGet]
        [Route("api/InformeMantenimiento/estadosinrevalorizar/{idinformeMantenimiento}")]
        public int estadosinrevalorizar(int idinformeMantenimiento)
        {
            int respuesta = 0;

            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    InformeMantenimiento oInformeMantenimiento = bd.InformeMantenimiento.Where(p => p.IdInformeMantenimiento == idinformeMantenimiento).First();
                    oInformeMantenimiento.Estado = 0; // significa que no aplicó revalorización

                    bd.SaveChanges();
                    respuesta = 1;

                }


            }
            catch (Exception ex)
            {

                respuesta = 0;
            }
            return respuesta;
        }


       

        //LISTAR INFORMES (HISTORIAL)
        [HttpGet]
        [Route("api/InformeMantenimiento/historialInformes/{idbien}")]
        public IEnumerable<InformeMatenimientoAF> historialInformes(int idbien)
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<InformeMatenimientoAF> listaInformeMante = (from tecnico in bd.Tecnicos
                                                                        join informemante in bd.InformeMantenimiento
                                                                        on tecnico.IdTecnico equals informemante.IdTecnico
                                                                        join bienmante in bd.BienMantenimiento
                                                                        on informemante.IdMantenimiento equals bienmante.IdMantenimiento
                                                                        join bienes in bd.ActivoFijo
                                                                        on bienmante.IdBien equals bienes.IdBien
                                                                        where bienes.IdBien == idbien 
                                                                 
                                                                        select new InformeMatenimientoAF
                                                                        {
                                                                            idinformematenimiento = informemante.IdInformeMantenimiento,
                                                                            idBien = bienes.IdBien,
                                                                            idmantenimiento = (int)informemante.IdMantenimiento,
                                                                            fechacadena = informemante.Fecha == null ? " " : ((DateTime)informemante.Fecha).ToString("dd-MM-yyyy"),
                                                                            nombretecnico = tecnico.Nombre,
                                                                            codigo = bienes.CorrelativoBien,
                                                                            descripcion = informemante.Descripcion,
                                                                            costomateriales = (double)informemante.CostoMateriales,
                                                                            costomo = (double)informemante.CostoMo,
                                                                            costototal = (double)informemante.CostoTotal,
                                                                            bienes = bienes.Desripcion

                                                                        }).ToList();
                return listaInformeMante;
            }
        }


     
        
        // METODO DOS PARA INTENTAR LISTAR EL MANTENIMIENTO.

        [HttpGet]
        [Route("api/InformeMantenimiento/datosHistorial/{idbien}")]
        public InformeMatenimientoAF datosHistorial(int idbien)
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                InformeMatenimientoAF odatos = new InformeMatenimientoAF();
                ActivoFijo oActivo = bd.ActivoFijo.Where(p => p.IdBien == idbien).First();

                odatos.descripcion = oActivo.Desripcion;
                odatos.codigo = oActivo.CorrelativoBien;
                Empleado oempleado = bd.Empleado.Where(p => p.IdEmpleado == oActivo.IdResponsable).First();
                AreaDeNegocio oArea = bd.AreaDeNegocio.Where(p => p.IdAreaNegocio == oempleado.IdAreaDeNegocio).First();
                odatos.encargado = oempleado.Nombres + " " + oempleado.Apellidos;
                odatos.areadenegocio = oArea.Nombre;
                return odatos;

            }
        }
        //Metodo que lista los activos en el historial de mantenimiento
        [HttpGet]
        [Route("api/InformeMantenimiento/listarActivosHistorial")]
        public IEnumerable<DepreciacionAF> listarActivosHistorial()
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

                                                            where activo.EstaAsignado==1
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


        //LISTAR INFORMES DE MANTENIMIENTO (PARA DAR REVALORIZACIÓN)
        [HttpGet]
        [Route("api/InformeMantenimiento/ListarInformeMantenimiento")]
        public IEnumerable<InformeMatenimientoAF> listarInformeMantenimiento()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                Periodo anioActual = bd.Periodo.Where(p => p.Estado == 1).FirstOrDefault();
                IEnumerable<InformeMatenimientoAF> listaInformeMante = (from tecnico in bd.Tecnicos
                                                                      join informemante in bd.InformeMantenimiento
                                                                        on tecnico.IdTecnico equals informemante.IdTecnico
                                                                      join bienmante in bd.BienMantenimiento
                                                                      on informemante.IdMantenimiento equals bienmante.IdMantenimiento
                                                                      join activo in bd.ActivoFijo
                                                                      on bienmante.IdBien equals activo.IdBien
                                                                       where informemante.Estado == 1 && (activo.EstadoActual != 0) && (activo.UltimoAnioDepreciacion == null || (activo.UltimoAnioDepreciacion < (anioActual.Anio)))


                                                                        select new InformeMatenimientoAF
                                                                        {
                                                                            idinformematenimiento = informemante.IdInformeMantenimiento,
                                                                            idBien = activo.IdBien,
                                                                            idmantenimiento = (int)informemante.IdMantenimiento,
                                                                            fechacadena = informemante.Fecha == null ? " " : ((DateTime)informemante.Fecha).ToString("dd-MM-yyyy"),
                                                                            nombretecnico = tecnico.Nombre,
                                                                            descripcion = informemante.Descripcion,
                                                                            costomateriales = (double)informemante.CostoMateriales,
                                                                            costomo = (double)informemante.CostoMo,
                                                                            costototal = (double)informemante.CostoTotal,
                                                                            vidautil= activo.VidaUtil,
                                                                            bienes = activo.Desripcion
                                                                            

                                                                        }).ToList();
                return listaInformeMante;
            }
        }

    }
}
 