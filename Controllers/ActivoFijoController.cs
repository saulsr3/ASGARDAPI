using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ASGARDAPI.Models;
using ASGARDAPI.Clases;
using BarcodeLib;
using Microsoft.AspNetCore.Cors;

namespace ASGARDAPI.Controllers
{
    //[EnableCors("TCAPolicy")]
    public class ActivoFijoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        //Metodos para modulo de asignacion
        [HttpGet]
        [Route("api/ActivoFIjo/listarActivosNoAsignados")]
        public IEnumerable<NoAsignadosAF> listarActivosNoAsignados()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<NoAsignadosAF> lista = (from activo in bd.ActivoFijo
                                                   join noFormulario in bd.FormularioIngreso
                                                   on activo.NoFormulario equals noFormulario.NoFormulario
                                                   join clasif in bd.Clasificacion
                                                   on activo.IdClasificacion equals clasif.IdClasificacion
                                                   //join marca in bd.Marcas
                                                   //on activo.IdMarca equals marca.IdMarca
                                                    where activo.EstadoActual == 1 && activo.EstaAsignado == 0
                                                    //orderby noFormulario.NoFormulario
                                                    select new NoAsignadosAF
                                                   {
                                                       IdBien = activo.IdBien,
                                                       NoFormulario = noFormulario.NoFormulario,
                                                       fechacadena = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                                       Desripcion = activo.Desripcion,
                                                       Clasificacion = clasif.Clasificacion1
                                                     
                                                   }).ToList();
                return lista;
            }
        }
        
        [HttpGet]
        [Route("api/ActivoFijo/listarEmpleadosCombo")]
        public IEnumerable<EmpleadoAF> listarEmpleadosCombo()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {

                IEnumerable<EmpleadoAF> lista = (from empleado in bd.Empleado
                                                 where empleado.Dhabilitado == 1
                                                 select new EmpleadoAF
                                                 {
                                                     idempleado = empleado.IdEmpleado,
                                                     nombres = empleado.Nombres,
                                                     apellidos = empleado.Apellidos
                                                 }).ToList();
                return lista;
            }
        }
        [HttpGet]
        [Route("api/Empleado/GenerarCodigo/{idempleado}/{idbien}")]
        public CodigoAF GenerarCodigo(int idempleado, int idbien)
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                //objeto de la clase codigo que contiene los elementos
                CodigoAF oCodigo = new CodigoAF();
                //Extraer los datos padres de la base
                ActivoFijo oActivo = bd.ActivoFijo.Where(p => p.IdBien == idbien).First();
                Empleado oEmpleado = bd.Empleado.Where(p => p.IdEmpleado == idempleado).First();
                //Utilizar los datos padres para extraer loc correlativos
                AreaDeNegocio oarea = bd.AreaDeNegocio.Where(p => p.IdAreaNegocio == oEmpleado.IdAreaDeNegocio).First();
                Sucursal osucursal = bd.Sucursal.Where(p => p.IdSucursal == oarea.IdSucursal).First();
                Clasificacion oclasificacion = bd.Clasificacion.Where(p => p.IdClasificacion == oActivo.IdClasificacion).First();

                //LLenado de objeto
                oCodigo.CorrelativoSucursal = osucursal.Correlativo;
                oCodigo.CorrelativoArea = oarea.Correlativo;
                oCodigo.CorrelativoClasificacion = oclasificacion.Correlativo;
                //selccionar cuantos hay de esa clasificacion
                int oActivoC = bd.ActivoFijo.Where(p => p.EstaAsignado==1 &&p.IdClasificacion== oclasificacion.IdClasificacion).Count();
             
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


                return oCodigo;
            }
        }
        [HttpPost]
        [Route("api/ActivoFIjo/asignarBien")]
        public int asignarBien([FromBody]AsignacionAF oAsignacionAF)
        {
            int rpta = 0;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                   // BarcodeLib.Barcode CodigoBarras = new BarcodeLib.Barcode();
                    //CodigoBarras.IncludeLabel = true;
                    ActivoFijo oActivo = bd.ActivoFijo.Where(p => p.IdBien == oAsignacionAF.idBien).First();
                    oActivo.IdBien = oAsignacionAF.idBien;
                    oActivo.NoSerie = oAsignacionAF.noSerie;
                    oActivo.VidaUtil = oAsignacionAF.vidaUtil;
                    oActivo.IdResponsable = oAsignacionAF.idEmpleado;
                    Empleado oEmpleado= bd.Empleado.Where(p => p.IdEmpleado == oActivo.IdResponsable).First();
                    AreaDeNegocio oArea=bd.AreaDeNegocio.Where(p => p.IdAreaNegocio == oEmpleado.IdAreaDeNegocio).First();
                    Sucursal oSucursal = bd.Sucursal.Where(p => p.IdSucursal == oArea.IdSucursal).First();
                    oActivo.CorrelativoBien = oAsignacionAF.Codigo;
                   // oActivo.CodigoBarra= CodigoBarras.Encode(BarcodeLib.TYPE.CODE128, oActivo.CorrelativoBien,BackColor:ConsoleColor.White,ForeColor:ConsoleColor.Black).;
                  
                
                   //= CodigoBarras;
                    oActivo.EstaAsignado = 1;
                    oActivo.DestinoInicial = oArea.Nombre + " " + oSucursal.Nombre;
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
        
        //Método guardar nuevo bien
        [HttpPost]
        [Route("api/ActivoFijo/guardarnuevoBien")]
        public int guardarnuevoBien([FromBody]ActivoFijoAF oActivoFijoAF)
        {
            int rpta = 0;

            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    ActivoFijo oActivoFijo = new ActivoFijo();
                    FormularioIngreso oFormularioIngreso = new FormularioIngreso();
                    //oFormularioIngreso.NoFormulario = oActivoFijoAF.noformulario;
                    //oFormularioIngreso.FechaIngreso = oActivoFijoAF.fechaingreso;
                    //oFormularioIngreso.NoFactura = oActivoFijoAF.nofactura;
                    //oFormularioIngreso.PersonaEntrega = oActivoFijoAF.personaentrega;
                    //oFormularioIngreso.PersonaRecibe = oActivoFijoAF.personarecibe;
                    //oFormularioIngreso.Observaciones = oActivoFijoAF.observaciones;
                    //bd.FormularioIngreso.Add(oFormularioIngreso);
                    //bd.SaveChanges();


                    //oActivoFijo.IdBien = oActivoFijoAF.IdBien;
                    //oActivoFijo.NoFormulario = oActivoFijoAF.noformulario;
                    //oActivoFijo.Desripcion = oActivoFijoAF.descripcion;
                    //oActivoFijo.Modelo = oActivoFijoAF.modelo;
                    //oActivoFijo.TipoAdquicicion = oActivoFijoAF.tipoadquisicion;
                    //oActivoFijo.Color = oActivoFijoAF.color;
                    //oActivoFijo.IdMarca = oActivoFijoAF.idmarca;
                    //oActivoFijo.IdClasificacion = oActivoFijoAF.idclasificacion;
                    //oActivoFijo.IdProveedor = oActivoFijoAF.idproveedor;
                    //oActivoFijo.IdDonante = oActivoFijoAF.iddonante;
                    //oActivoFijo.EstadoIngreso = oActivoFijoAF.estadoingreso;
                    //oActivoFijo.ValorAdquicicion = oActivoFijoAF.costo;
                    //oActivoFijo.PlazoPago = oActivoFijoAF.plazopago;
                    //oActivoFijo.Prima = oActivoFijoAF.prima;
                    //oActivoFijo.CuotaAsignanda = oActivoFijoAF.cuotaasignada;
                    //oActivoFijo.Intereses = oActivoFijoAF.interes;
                    //oActivoFijo.Foto = oActivoFijoAF.foto;

                    //Variables para formularioIngreso
                    

                    //oActivoFijo.EstadoActual = 1;
                    //bd.ActivoFijo.Add(oActivoFijo);

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
        [Route("api/ActivoFijo/guardarFormulario")]
        public int guardarFormulario([FromBody]FormularioIngresoAF oformulario)
        {
            int rpta = 0;

            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    //ActivoFijo oActivoFijo = new ActivoFijo();
                    FormularioIngreso oFormularioIngreso = new FormularioIngreso();
                    oFormularioIngreso.NoFormulario = oformulario.noformulario;
                    oFormularioIngreso.FechaIngreso = oformulario.fechaingreso;
                    oFormularioIngreso.NoFactura = oformulario.nofactura;
                    oFormularioIngreso.PersonaEntrega = oformulario.personaentrega;
                    oFormularioIngreso.PersonaRecibe = oformulario.personarecibe;
                    oFormularioIngreso.Observaciones = oformulario.observaciones;
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

        //Métodos para llenar combos del form
        [HttpGet]
        [Route("api/ActivoFijo/listarClasificacionCombo")]
        public IEnumerable<ClasificacionAF> listarClasificacionCombo()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {

                IEnumerable<ClasificacionAF> lista = (from clasificacion in bd.Clasificacion
                                                      where clasificacion.Dhabilitado == 1
                                                      select new ClasificacionAF
                                                      {
                                                          idclasificacion = clasificacion.IdClasificacion,
                                                          clasificacion = clasificacion.Clasificacion1,
                                                          correlativo = clasificacion.Correlativo

                                                      }).ToList();
                return lista;
            }
        }

        [HttpGet]
        [Route("api/ActivoFijo/listarProveedoresCombo")]
        public IEnumerable<ComboAnidadoAF> listarProveedoresCombo()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {

                IEnumerable<ComboAnidadoAF> lista = (from proveedor in bd.Proveedor
                                                     where proveedor.Dhabilitado == 1
                                                     select new ComboAnidadoAF
                                                     {
                                                         id = proveedor.IdProveedor,
                                                         nombre = proveedor.Nombre

                                                     }).ToList();
                return lista;
            }
        }

        [HttpGet]
        [Route("api/ActivoFijo/listarDonantesCombo")]
        public IEnumerable<ComboAnidadoAF> listarDonantesCombo()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {

                IEnumerable<ComboAnidadoAF> lista = (from donante in bd.Donantes
                                                     where donante.Dhabilitado == 1
                                                     select new ComboAnidadoAF
                                                     {
                                                         id = donante.IdDonante,
                                                         nombre = donante.Nombre

                                                     }).ToList();
                return lista;
            }
        }

        [HttpGet]
        [Route("api/ActivoFijo/listarMarcasCombo")]
        public IEnumerable<MarcasAF> listarMarcasCombo()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {

                IEnumerable<MarcasAF> lista = (from marca in bd.Marcas
                                                     where marca.Dhabilitado == 1
                                                     select new MarcasAF
                                                     {
                                                         IdMarca=marca.IdMarca,
                                                         Marca=marca.Marca

                                                     }).ToList();
                return lista;
            }
        }


        /// /////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        //Medotos para modulo de control mayra
        [HttpGet]
        [Route("api/ActivoFIjo/listarActivos")]
        public List<ActivoFijoAF> listarActivos()
        {

            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                List<ActivoFijoAF> lista = (from activo in bd.ActivoFijo
                                            join noFormulario in bd.FormularioIngreso
                                            on activo.NoFormulario equals noFormulario.NoFormulario
                                            join clasif in bd.Clasificacion
                                            on activo.IdClasificacion equals clasif.IdClasificacion
                                            where (activo.EstadoActual == 1 && activo.IdResponsable == null)
                                            select new ActivoFijoAF
                                            {
                                                IdBien = activo.IdBien,
                                                Codigo = activo.CorrelativoBien,
                                                fechacadena = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                                Desripcion = activo.Desripcion,
                                                Clasificacion = clasif.Clasificacion1,
                                                //AreaDeNegocio = area.Nombre,
                                                //Resposnsable = resposable.Nombres + " " + resposable.Apellidos
                                            }).ToList();
                foreach (var i in listarActivosAsignados())
                {
                    lista.Add(i);
                }


                return lista;

            }
        }


        public List<ActivoFijoAF> listarActivosAsignados()
        {

            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                List<ActivoFijoAF> lista = (from activo in bd.ActivoFijo
                                            join noFormulario in bd.FormularioIngreso
                                            on activo.NoFormulario equals noFormulario.NoFormulario
                                            join clasif in bd.Clasificacion
                                            on activo.IdClasificacion equals clasif.IdClasificacion
                                            join resposable in bd.Empleado
                                            on activo.IdResponsable equals resposable.IdEmpleado
                                            join area in bd.AreaDeNegocio
                                            on resposable.IdAreaDeNegocio equals area.IdAreaNegocio
                                            where activo.EstadoActual == 1

                                            orderby activo.CorrelativoBien
                                            select new ActivoFijoAF
                                            {
                                                IdBien = activo.IdBien,
                                                Codigo = activo.CorrelativoBien,
                                                fechacadena = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                                Desripcion = activo.Desripcion,
                                                Clasificacion = clasif.Clasificacion1,
                                                AreaDeNegocio = area.Nombre,
                                                Resposnsable = resposable.Nombres + " " + resposable.Apellidos
                                            }).ToList();
                return lista;

            }
        }


        [HttpGet]
        [Route("api/ActivoFijo/RecuperarFormCompleto/{id}")]
        public JsonResult RecuperarFormCompleto(int id)
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                //creamos un nuevo objeto dinamico bien
                dynamic odatos = new Newtonsoft.Json.Linq.JObject();
                //Extraer los datos padres de la base
                ActivoFijo oActivo = bd.ActivoFijo.Where(p => p.IdBien == id).First();
                //FormularioIngreso oformu = bd.FormularioIngreso.Where(p => p.NoFormulario == id).First();
                //Utilizar los datos padres para extraer los datos
                FormularioIngreso oformu = bd.FormularioIngreso.Where(p => p.NoFormulario == oActivo.NoFormulario).First();
                Clasificacion oclasi = bd.Clasificacion.Where(p => p.IdClasificacion == oActivo.IdClasificacion).First();
                Marcas omarca = bd.Marcas.Where(p => p.IdMarca == oActivo.IdMarca).First();
                Proveedor oprov = (oActivo.IdProveedor != null) ? bd.Proveedor.Where(p => p.IdProveedor == oActivo.IdProveedor).First() : null;
                Donantes odona = (oActivo.IdDonante != null) ? bd.Donantes.Where(p => p.IdDonante == oActivo.IdDonante).First() : null;

                //llenado
                odatos.idbien = oActivo.IdBien;
                odatos.bandera = 0;
                odatos.idclasificacion = oclasi.IdClasificacion;
                odatos.estadoingreso = oActivo.EstadoIngreso;
                odatos.fechaingreso = oformu.FechaIngreso == null ? " " : ((DateTime)oformu.FechaIngreso).ToString("yyyy-MM-dd");
                odatos.tipoadquicicion = (int)oActivo.TipoAdquicicion;
                odatos.idproveedor = (oprov != null) ? oprov.IdProveedor : odona.IdDonante;
                odatos.descripcion = oActivo.Desripcion;
                odatos.color = oActivo.Color;
                odatos.idmarca = omarca.IdMarca;
                odatos.modelo = oActivo.Modelo;
                odatos.nofactura = oformu.NoFactura;
                odatos.valoradquicicion = oActivo.ValorAdquicicion;
                odatos.prima = oActivo.Prima;
                odatos.plazopago = oActivo.PlazoPago;
                odatos.cuotaasignada = oActivo.CuotaAsignanda;
                odatos.personaentrega = oformu.PersonaEntrega;
                odatos.personarecibe = oformu.PersonaRecibe;
                odatos.observaciones = oformu.Observaciones;
                odatos.foto = oActivo.Foto;
                odatos.interes = oActivo.Intereses;
                odatos.noformulario = oformu.NoFormulario;
                odatos.cantidad = (from activo in bd.ActivoFijo
                                   join noFormulario in bd.FormularioIngreso
                                   on activo.NoFormulario equals noFormulario.NoFormulario
                                   where activo.NoFormulario == oActivo.NoFormulario
                                   select activo).ToList().Count();


                return Json(odatos);
            }
        }


        [HttpPost]
        [Route("api/ActivoFijo/modificarActivos")]
        public int modificarActivos([FromBody]ActivoFijoAF oActivoFijoAF)
        {
            int rpta = 0;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    Console.WriteLine("probando" + oActivoFijoAF.IdBien);
                    ActivoFijo oActivoFijo = bd.ActivoFijo.Where(p => p.IdBien == oActivoFijoAF.IdBien).First();
                    FormularioIngreso oFormularioIngreso = bd.FormularioIngreso.Where(p => p.NoFormulario == oActivoFijoAF.NoFormulario).First();
                    //oActivoFijo.NoFormulario = oFormularioIngreso.NoFormulario;
                    rpta = 1;

                    ////para editar tenemos que sacar la informacion
                    //ActivoFijo oActivoFijo = bd.ActivoFijo.Where(p => p.IdBien == oActivoFijoAF.IdBien).First();
                    //oActivoFijo.IdBien = oActivoFijoAF.IdBien;
                    //oActivoFijo.Desripcion = oActivoFijoAF.Desripcion;
                    //oActivoFijo.IdClasificacion = oActivoFijoAF.idclasificacion;
                    //oActivoFijo.IdMarca = oActivoFijoAF.idmarca;
                    //oActivoFijo.Modelo = oActivoFijoAF.Modelo;
                    //oActivoFijo.TipoAdquicicion = oActivoFijoAF.tipoadquicicion;
                    //oActivoFijo.Color = oActivoFijoAF.Color;
                    //oActivoFijo.VidaUtil = oActivoFijoAF.vidautil;
                    //oActivoFijo.EstadoIngreso = oActivoFijoAF.estadoingreso;
                    //oActivoFijo.ValorAdquicicion = oActivoFijoAF.valoradquicicion;
                    //oActivoFijo.PlazoPago = oActivoFijoAF.plazopago;
                    //oActivoFijo.Prima = oActivoFijoAF.prima;
                    //oActivoFijo.CuotaAsignanda = oActivoFijoAF.cuotaasignada;
                    //oActivoFijo.Intereses = oActivoFijoAF.interes;
                    //oActivoFijo.ValorResidual = oActivoFijoAF.valorresidual;
                    //oActivoFijo.Foto = oActivoFijoAF.foto;
                    //oActivoFijo.IdProveedor = oActivoFijoAF.idproveedor;
                    //oActivoFijo.IdDonante = oActivoFijoAF.iddonante;

                    ////para guardar cambios
                    //bd.SaveChanges();
                    ////si todo esta bien
                    //rpta = 1;

                }
            }
            catch (Exception ex)
            {
                rpta = 0;
                Console.WriteLine(rpta);
            }
            return rpta;
        }

        [HttpPost]
        [Route("api/ActivoFijo/modificarFormulario")]
        public int modificarFormulario([FromBody]FormularioIngresoAF oformulario)
        {
            int rpta = 0;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {

                    FormularioIngreso oFormularioIngreso = bd.FormularioIngreso.Where(p => p.NoFormulario == oformulario.noformulario).First();
                    oFormularioIngreso.NoFormulario = oformulario.noformulario;
                    oFormularioIngreso.NoFactura = oformulario.nofactura;
                    oFormularioIngreso.FechaIngreso = oformulario.fechaingreso;
                    oFormularioIngreso.PersonaEntrega = oformulario.personaentrega;
                    oFormularioIngreso.PersonaRecibe = oformulario.personarecibe;
                    oFormularioIngreso.Observaciones = oformulario.observaciones;

                    rpta = 1;
                }
            }
            catch (Exception ex)
            {
                rpta = 0;
                Console.WriteLine(rpta);
            }
            return rpta;
        }


        [HttpGet]
        [Route("api/ActivoFijo/buscarActivo/{buscador?}")]
        public IEnumerable<ActivoFijoAF> buscarActivo(string buscador = "")
        {
            List<ActivoFijoAF> listaActivo;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                if (buscador == "")
                {
                    listaActivo = (from activo in bd.ActivoFijo
                                   join noFormulario in bd.FormularioIngreso
                                   on activo.NoFormulario equals noFormulario.NoFormulario
                                   join clasif in bd.Clasificacion
                                   on activo.IdClasificacion equals clasif.IdClasificacion
                                   //join resposable in bd.Empleado
                                   //on activo.IdResponsable equals resposable.IdEmpleado
                                   //join area in bd.AreaDeNegocio
                                   //on resposable.IdAreaDeNegocio equals area.IdAreaNegocio
                                   where activo.EstadoActual == 1

                                   select new ActivoFijoAF
                                   {
                                       IdBien = activo.IdBien,
                                       Codigo = activo.CorrelativoBien,
                                       fechacadena = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                       Desripcion = activo.Desripcion,
                                       Clasificacion = clasif.Clasificacion1,
                                       //AreaDeNegocio = area.Nombre,
                                       //Resposnsable = resposable.Nombres + " " + resposable.Apellidos
                                   }).ToList();

                    return listaActivo;
                }
                else
                {
                    listaActivo = (from activo in bd.ActivoFijo
                                   join noFormulario in bd.FormularioIngreso
                                   on activo.NoFormulario equals noFormulario.NoFormulario
                                   join clasif in bd.Clasificacion
                                   on activo.IdClasificacion equals clasif.IdClasificacion
                                   join resposable in bd.Empleado
                                   on activo.IdResponsable equals resposable.IdEmpleado
                                   join area in bd.AreaDeNegocio
                                   on resposable.IdAreaDeNegocio equals area.IdAreaNegocio
                                   where activo.EstadoActual == 1


                                     && ((activo.IdBien).ToString().Contains(buscador)
                                      || (activo.CorrelativoBien).ToLower().Contains(buscador.ToLower())
                                      || (noFormulario.FechaIngreso).ToString().ToLower().Contains(buscador.ToLower())
                                      || (activo.Desripcion).ToLower().Contains(buscador.ToLower())
                                      || (clasif.Clasificacion1).ToLower().Contains(buscador.ToLower())
                                      || (area.Nombre).ToLower().Contains(buscador.ToLower())
                                      || (resposable.Nombres).ToLower().Contains(buscador.ToLower())
                                      || (resposable.Apellidos).ToLower().Contains(buscador.ToLower())
                                      )

                                   select new ActivoFijoAF
                                   {
                                       IdBien = activo.IdBien,
                                       Codigo = activo.CorrelativoBien,
                                       fechacadena = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                       Desripcion = activo.Desripcion,
                                       Clasificacion = clasif.Clasificacion1,
                                       AreaDeNegocio = area.Nombre,
                                       Resposnsable = resposable.Nombres + " " + resposable.Apellidos
                                   }).ToList();

                    return listaActivo;
                }
            }
        }


        [HttpGet]
        [Route("api/ActivoFijo/DatosVer/{id}")]
        public JsonResult DatosVer(int id)
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                //creamos un nuevo objeto dinamico bien
                dynamic bien = new Newtonsoft.Json.Linq.JObject();
                //Extraer los datos padres de la base
                ActivoFijo oActivo = bd.ActivoFijo.Where(p => p.IdBien == id).First();
                //Utilizar los datos padres para extraer los datos
                //Empleado oempleado = bd.Empleado.Where(p => p.IdEmpleado == oActivo.IdResponsable).First();
                //AreaDeNegocio oArea = bd.AreaDeNegocio.Where(p => p.IdAreaNegocio == oempleado.IdAreaDeNegocio).First();
                //Sucursal oSucursal = bd.Sucursal.Where(p => p.IdSucursal == oArea.IdSucursal).First();
                FormularioIngreso oformu = bd.FormularioIngreso.Where(p => p.NoFormulario == oActivo.NoFormulario).First();
                Clasificacion oclasi = bd.Clasificacion.Where(p => p.IdClasificacion == oActivo.IdClasificacion).First();
                Marcas omarca = bd.Marcas.Where(p => p.IdMarca == oActivo.IdMarca).First();
                // si los datos son nulos
                string oprov = (oActivo.IdProveedor != null) ? bd.Proveedor.Where(p => p.IdProveedor == oActivo.IdProveedor).First().Nombre : "--";
                string odona = (oActivo.IdDonante != null) ? bd.Donantes.Where(p => p.IdDonante == oActivo.IdDonante).First().Nombre : "--";
                string oemple = (oActivo.IdResponsable != null) ? bd.Empleado.Where(p => p.IdEmpleado == oActivo.IdResponsable).First().Nombres : "--";
                // bien.responsable = oemple + " " + oemple.Apellidos;
                //bien.area = oArea.Nombre;
                bien.marca = omarca.Marca;
                bien.clasificacion = oclasi.Clasificacion1;
                //bien.destino = oArea.Nombre + " " + oSucursal.Nombre;
                bien.proveedor = oprov;
                bien.responsable = oemple;
                bien.donante = odona;
                bien.fecha = oformu.FechaIngreso == null ? " " : ((DateTime)oformu.FechaIngreso).ToString("dd-MM-yyyy");

                bien.noformulario = oActivo.NoFormulario;
                bien.Codigo = oActivo.CorrelativoBien;
                bien.Desripcion = oActivo.Desripcion;
                bien.Modelo = oActivo.Modelo;
                if (oActivo.TipoAdquicicion == 1)
                {
                    bien.tipoadquicicion = "Contado";
                }
                else if (oActivo.TipoAdquicicion == 2)
                {
                    bien.tipoadquicicion = "Crédito";
                }
                else
                {
                    bien.tipoadquicicion = "Donado";
                }
                bien.Color = oActivo.Color;
                bien.numserie = oActivo.NoSerie;
                bien.vidautil = oActivo.VidaUtil;
                if (oActivo.EstadoIngreso == "1")
                {
                    bien.estadoingreso = "Nuevo";
                }
                else if (oActivo.EstadoIngreso == "1")
                {
                    bien.estadoingreso = "Usado";
                }
                else
                {
                    bien.estadoingreso = "Usado mal estado";
                }

                bien.valoradquicicion = oActivo.ValorAdquicicion;
                bien.plazopago = oActivo.PlazoPago;
                bien.prima = oActivo.Prima;
                bien.cuotaasignada = oActivo.CuotaAsignanda;
                bien.interes = oActivo.Intereses;
                bien.valorresidual = oActivo.ValorResidual;
                bien.foto = oActivo.Foto;
                bien.destinoinicial = oActivo.DestinoInicial;
                //para saber si es un donante o un proveedor
                bien.donaprov = false;
                return Json(bien);
            }
        }


        [HttpGet]
        [Route("api/ActivoFIjo/listarActivosFiltro/{id}")]
        public IEnumerable<ActivoFijoAF> listarActivosFiltro(int id)
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<ActivoFijoAF> lista = (from activo in bd.ActivoFijo
                                                   join noFormulario in bd.FormularioIngreso
                                                   on activo.NoFormulario equals noFormulario.NoFormulario
                                                   join clasif in bd.Clasificacion
                                                   on activo.IdClasificacion equals clasif.IdClasificacion
                                                   join resposable in bd.Empleado
                                                   on activo.IdResponsable equals resposable.IdEmpleado
                                                   join area in bd.AreaDeNegocio
                                                   on resposable.IdAreaDeNegocio equals area.IdAreaNegocio
                                                   where activo.EstadoActual == 1 && activo.EstaAsignado == 1 && area.IdAreaNegocio == id
                                                   orderby activo.CorrelativoBien
                                                   select new ActivoFijoAF
                                                   {
                                                       IdBien = activo.IdBien,
                                                       Codigo = activo.CorrelativoBien,
                                                       fechacadena = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                                       Desripcion = activo.Desripcion,
                                                       Clasificacion = clasif.Clasificacion1,
                                                       AreaDeNegocio = area.Nombre,
                                                       Resposnsable = resposable.Nombres + " " + resposable.Apellidos
                                                   }).ToList();
                return lista;

            }
        }






    }
}