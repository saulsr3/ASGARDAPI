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
                    listarInformeMantenimiento = (from informemante in bd.InformeMantenimiento
                                                  join tecnico in bd.Tecnicos
                                           on informemante.IdInformeMantenimiento equals tecnico.IdTecnico
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
                    listarInformeMantenimiento = (from informemante in bd.InformeMantenimiento
                                                  join tecnico in bd.Tecnicos
                                           on informemante.IdInformeMantenimiento equals tecnico.IdTecnico
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

        //LISTAR INFORMES (HISTORIAL) vamos a cancelar este metodo por el momento
        [HttpGet]
        [Route("api/InformeMantenimiento/historialInformes/{idbien}")]
        public IEnumerable<InformeMatenimientoAF> historialInformes(int idbien)
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<InformeMatenimientoAF> listaInformeMante = (from informemante in bd.InformeMantenimiento
                                                                        join tecnico in bd.Tecnicos
                                                                        on informemante.IdInformeMantenimiento equals tecnico.IdTecnico
                                                                        join bienmante in bd.BienMantenimiento
                                                                        on informemante.IdMantenimiento equals bienmante.IdMantenimiento
                                                                        join bienes in bd.ActivoFijo
                                                                        on bienmante.IdBien equals bienes.IdBien
                                                                        where bienes.IdBien == idbien 
                                                                      //   where informemante.Estado == 1 || informemante.Estado == 0 || informemante.Estado == 2
                                                                        // && bienmante.IdBien == bienes.IdBien

                                                                        //where empleado.Dhabilitado == 1
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


        //METODOS PARA INTENTAS LISTAR EL MANTENIMIENTO.
        
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


        //LISTAR INFORMES DE MANTENIMIENTO (PARA DAR REVALORIZACIÓN)
        [HttpGet]
        [Route("api/InformeMantenimiento/ListarInformeMantenimiento")]
        public IEnumerable<InformeMatenimientoAF> listarInformeMantenimiento()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<InformeMatenimientoAF> listaInformeMante = (from informemante in bd.InformeMantenimiento
                                                                        join tecnico in bd.Tecnicos
                                                                 on informemante.IdInformeMantenimiento equals tecnico.IdTecnico
                                                                        join bienmante in bd.BienMantenimiento
                                                                        on informemante.IdMantenimiento equals bienmante.IdMantenimiento
                                                                        join bienes in bd.ActivoFijo
                                                                        on bienmante.IdBien equals bienes.IdBien
                                                                        where informemante.Estado == 1

                                                                        //where empleado.Dhabilitado == 1
                                                                        select new InformeMatenimientoAF
                                                                        {
                                                                            idinformematenimiento = informemante.IdInformeMantenimiento,
                                                                            idBien = bienes.IdBien,
                                                                            idmantenimiento = (int)informemante.IdMantenimiento,
                                                                            fechacadena = informemante.Fecha == null ? " " : ((DateTime)informemante.Fecha).ToString("dd-MM-yyyy"),
                                                                            nombretecnico = tecnico.Nombre,
                                                                            descripcion = informemante.Descripcion,
                                                                            costomateriales = (double)informemante.CostoMateriales,
                                                                            costomo = (double)informemante.CostoMo,
                                                                            costototal = (double)informemante.CostoTotal,
                                                                            bienes = bienes.Desripcion

                                                                        }).ToList();
                return listaInformeMante;
            }
        }

    }
}