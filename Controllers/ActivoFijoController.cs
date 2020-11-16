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
                                                    where (activo.EstadoActual == 1 || activo.EstadoActual == 2 || activo.EstadoActual == 3) && activo.EstaAsignado == 0
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
        [Route("api/ActivoFIjo/validarActivosAsignar")]
        public int validarActivosAsignar()
        {
            int rpta = 0;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<NoAsignadosAF> lista = (from activo in bd.ActivoFijo    
                                                    where (activo.EstadoActual == 1 || activo.EstadoActual == 2 || activo.EstadoActual == 3) && activo.EstaAsignado == 0
                                                    select new NoAsignadosAF
                                                    {
                                                        IdBien = activo.IdBien,
                                                        Desripcion = activo.Desripcion,
                                                    }).ToList();
                if (lista.Count() > 0) {
                    rpta = 1;
                }
                return rpta;
            }
        }
        [HttpGet]
        [Route("api/ActivoFIjo/RecuperarVidaUtil/{idbien}")]
        public AsignacionAF RecuperarVidaUtil(int idbien)
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                AsignacionAF oVidaUtil = new AsignacionAF();
                ActivoFijo oactivo = bd.ActivoFijo.Where(p => p.IdBien == idbien).First();
                Clasificacion oClas = bd.Clasificacion.Where(p => p.IdClasificacion == oactivo.IdClasificacion).First();
                Categorias oCategoria  = bd.Categorias.Where(p => p.IdCategoria == oClas.IdCategoria).First();
                oVidaUtil.vidaUtil =(int) oCategoria.VidaUtil;
                return oVidaUtil;
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

        //Metodo para no permitir editar la fecha de un activo asignado si ya se ha depreciado.
        [HttpGet]
        [Route("api/ActivoFijo/noEditarfecha/{idbien}")]
        public int noEditarfecha(int idbien)
        {
            int respuesta = 0;

            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    ActivoFijo oActivo = bd.ActivoFijo.Where(p => p.IdBien == idbien && p.UltimoAnioDepreciacion != null).First();
                    respuesta = 1;
                }
            }
            catch (Exception ex)
            {

                respuesta = 0;
            }
            return respuesta;
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
                    oActivo.FechaAsignacion = oAsignacionAF.fecha;
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

        //Listar clasificación sólo de categoría instalaciones
        [HttpGet]
        [Route("api/ActivoFijo/listarClasificacionComboEdi")]
        public IEnumerable<ClasificacionAF> listarClasificacionComboEdi()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {

                IEnumerable<ClasificacionAF> lista = (from clasificacion in bd.Clasificacion
                                                      where clasificacion.Dhabilitado == 1 & clasificacion.IdCategoria==1
                                                      select new ClasificacionAF
                                                      {
                                                          idclasificacion = clasificacion.IdClasificacion,
                                                          clasificacion = clasificacion.Clasificacion1,

                                                      }).ToList();
                return lista;
            }
        }

        //Listar clasificación sólo de categoría intangibles
        [HttpGet]
        [Route("api/ActivoFijo/listarClasificacionComboIntan")]
        public IEnumerable<ClasificacionAF> listarClasificacionComboIntan()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {

                IEnumerable<ClasificacionAF> lista = (from clasificacion in bd.Clasificacion
                                                      where clasificacion.Dhabilitado == 1 & clasificacion.IdCategoria == 4
                                                      select new ClasificacionAF
                                                      {
                                                          idclasificacion = clasificacion.IdClasificacion,
                                                          clasificacion = clasificacion.Clasificacion1,

                                                      }).ToList();
                return lista;
            }
        }

        //Listar combo sucursal
        [HttpGet]
        [Route("api/ActivoFijo/comboSucursal")]
        public IEnumerable<SucursalAF> comboSucursal()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<SucursalAF> lista = (from sucursal in bd.Sucursal
                                                 where sucursal.Dhabilitado == 1
                                                 orderby sucursal.Ubicacion
                                                 select new SucursalAF
                                                 {
                                                     IdSucursal = sucursal.IdSucursal,
                                                     Nombre = sucursal.Nombre,
                                                     Ubicacion = sucursal.Ubicacion

                                                 }).ToList();
                return lista;
            }
        }

        //Listar área de negocio
        [HttpGet]
        [Route("api/ActivoFijo/listarAreaCombo")]
        public IEnumerable<AreasDeNegocioAF> listarAreaCombo()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<AreasDeNegocioAF> listarAreas = (from sucursal in bd.Sucursal
                                                             join area in bd.AreaDeNegocio
                                                             on sucursal.IdSucursal equals area.IdSucursal
                                                             where area.Dhabilitado == 1
                                                             select new AreasDeNegocioAF
                                                             {
                                                                 IdAreaNegocio = area.IdAreaNegocio,
                                                                 Nombre = area.Nombre,
                                                                 IdSucursal = sucursal.IdSucursal,
                                                                 nombreSucursal = sucursal.Nombre,
                                                                 ubicacion = sucursal.Ubicacion


                                                             }).ToList();


                return listarAreas;

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
        
        //Activos asignados de bienes muebles
        [HttpGet]
        [Route("api/ActivoFIjo/listarActivosAsignados")]
        public List<RegistroAF> listarActivosAsignados()
        {

            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                List<RegistroAF> lista = (from activo in bd.ActivoFijo
                                            join noFormulario in bd.FormularioIngreso
                                            on activo.NoFormulario equals noFormulario.NoFormulario
                                            join clasif in bd.Clasificacion
                                            on activo.IdClasificacion equals clasif.IdClasificacion
                                            join resposable in bd.Empleado
                                            on activo.IdResponsable equals resposable.IdEmpleado
                                            join area in bd.AreaDeNegocio
                                            on resposable.IdAreaDeNegocio equals area.IdAreaNegocio
                                            where (activo.EstadoActual != 0) && activo.EstaAsignado == 1

                                            orderby activo.CorrelativoBien
                                            select new RegistroAF
                                            {
                                                IdBien = activo.IdBien,
                                                Codigo = activo.CorrelativoBien,
                                                fechacadena = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                                Descripcion = activo.Desripcion,
                                                Clasificacion = clasif.Clasificacion1,
                                                AreaDeNegocio = area.Nombre,
                                                Responsable = resposable.Nombres + " " + resposable.Apellidos
                                            }).ToList();
                return lista;

            }
        }

        //Activos para edificios e instalaciones
        [HttpGet]
        [Route("api/ActivoFIjo/listarActivosEdificios")]
        public List<RegistroAF> listarActivosEdificios()
        {

            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                List<RegistroAF> lista = (from activo in bd.ActivoFijo
                                            join noFormulario in bd.FormularioIngreso
                                            on activo.NoFormulario equals noFormulario.NoFormulario
                                            join clasif in bd.Clasificacion
                                            on activo.IdClasificacion equals clasif.IdClasificacion
                                            where (activo.EstadoActual == 1 || activo.EstadoActual == 2 || activo.EstadoActual == 3) && activo.TipoActivo==1 && activo.EstadoActual == 1
                                            orderby activo.CorrelativoBien
                                            select new RegistroAF
                                            {
                                                IdBien = activo.IdBien,
                                                Codigo = activo.CorrelativoBien,
                                                fechacadena = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                                Descripcion = activo.Desripcion,
                                                Clasificacion = clasif.Clasificacion1,
                                            }).ToList();
                return lista;

            }
        }

        //Activos para intangibles
        [HttpGet]
        [Route("api/ActivoFIjo/listarActivosIntangibles")]
        public List<RegistroAF> listarActivosIntangibles()
        {

            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                List<RegistroAF> lista = (from activo in bd.ActivoFijo
                                            join noFormulario in bd.FormularioIngreso
                                            on activo.NoFormulario equals noFormulario.NoFormulario
                                            join clasif in bd.Clasificacion
                                            on activo.IdClasificacion equals clasif.IdClasificacion
                                          // Acá iría el área pero como está referenciada a empleado
                                           
                                            where (activo.EstadoActual == 1 || activo.EstadoActual == 2 || activo.EstadoActual == 3) && activo.TipoActivo == 3 && activo.EstadoActual==1
                                            orderby activo.CorrelativoBien
                                            select new RegistroAF
                                            {
                                                IdBien = activo.IdBien,
                                                Codigo = activo.CorrelativoBien,
                                                fechacadena = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                                Descripcion = activo.Desripcion,
                                                Clasificacion = clasif.Clasificacion1,
                                            }).ToList();
                return lista;

            }
        }
        //RECUPERAMOS EL ACTIVO CUANDO ESTÁ ASIGNADO.
        //Recuperar bien mueble
        [HttpGet]
        [Route("api/ActivoFijo/recuperarActivoAsignado/{id}")]
        public ActivoAF recuperarActivoAsignado(int id)
        {

            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                ActivoAF oActivoAF = new ActivoAF();
                ActivoFijo oActivo = bd.ActivoFijo.Where(p => p.IdBien == id).First();
                FormularioIngreso oFormulario = bd.FormularioIngreso.Where(p => p.NoFormulario == oActivo.NoFormulario).First();
                Clasificacion oClasificacion = bd.Clasificacion.Where(p => p.IdClasificacion == oActivo.IdClasificacion).First();
                Marcas omarca = (oActivo.IdMarca != null) ? bd.Marcas.Where(p => p.IdMarca == oActivo.IdMarca).First() : null;
                //  Proveedor oprov = (oActivo.IdProveedor != null) ? bd.Proveedor.Where(p => p.IdProveedor == oActivo.IdProveedor).First() : null;
                //  Donantes odona = (oActivo.IdDonante != null) ? bd.Donantes.Where(p => p.IdDonante == oActivo.IdDonante).First() : null;
                Empleado oemple = (oActivo.IdResponsable != null) ? bd.Empleado.Where(p => p.IdEmpleado == oActivo.IdResponsable).First() : null;

                //Llenar campos
                oActivoAF.idbien = oActivo.IdBien;
                oActivoAF.idclasificacion = oClasificacion.IdClasificacion;
                oActivoAF.estadoingreso = oActivo.EstadoIngreso;
                oActivoAF.fechaingreso = oFormulario.FechaIngreso == null ? " " : ((DateTime)oFormulario.FechaIngreso).ToString("yyyy-MM-dd");
                oActivoAF.tipoadquicicion = (int)oActivo.TipoAdquicicion;

                //oActivoAF.idproveedor = oprov.IdProveedor : odona.IdDonante;

                if (oActivo.IdProveedor != null)
                {
                    Proveedor oProveedor = bd.Proveedor.Where(p => p.IdProveedor == oActivo.IdProveedor).First();
                    oActivoAF.idproveedor = oProveedor.IdProveedor;
                    oActivoAF.IsProvDon = 1;
                }
                else
                {
                    Donantes oDonante = bd.Donantes.Where(p => p.IdDonante == oActivo.IdDonante).First();
                    oActivoAF.iddonante = oDonante.IdDonante;
                    oActivoAF.IsProvDon = 2;
                }
                if (oActivo.IdProveedor == null && oActivo.IdDonante == null)
                {
                    oActivoAF.ProvDon = "";
                }

                oActivoAF.descripcion = oActivo.Desripcion;
                oActivoAF.color = oActivo.Color;
                oActivoAF.idmarca = (omarca == null) ? 0 : omarca.IdMarca;
                oActivoAF.modelo = oActivo.Modelo;
                oActivoAF.nofactura = oFormulario.NoFactura;
                oActivoAF.valoradquicicion = (double)oActivo.ValorAdquicicion;



                //Datos del crédito
                if (oActivo.Prima != null)
                {
                    oActivoAF.prima = (double)oActivo.Prima;
                }
                else
                {

                }
                if (oActivo.PlazoPago != null)
                {
                    oActivoAF.plazopago = oActivo.PlazoPago;
                }
                else
                {

                }
                if (oActivo.CuotaAsignanda != null)
                {
                    oActivoAF.cuotaasignada = (double)oActivo.CuotaAsignanda;
                }
                else
                {

                }
                if (oActivo.Intereses != null)
                {
                    oActivoAF.interes = (double)oActivo.Intereses;
                }
                else
                {

                }

                oActivoAF.personaentrega = oFormulario.PersonaEntrega;
                oActivoAF.personarecibe = oFormulario.PersonaRecibe;
                oActivoAF.observaciones = oFormulario.Observaciones;
                oActivoAF.valorresidual = (double)oActivo.ValorResidual;
                oActivoAF.noserie = oActivo.NoSerie; //para recuperar el numero de serie.
                oActivoAF.foto = oActivo.Foto;
                oActivoAF.noformularioactivo = oFormulario.NoFormulario;
                oActivoAF.cantidad = (from activo in bd.ActivoFijo
                                      join noFormulario in bd.FormularioIngreso
                                      on activo.NoFormulario equals noFormulario.NoFormulario
                                      where activo.NoFormulario == oActivo.NoFormulario && activo.EstadoActual == 1 && activo.EstaAsignado == 1 //SOLO ESTO CAMBIAMOS
                                      select activo).ToList().Count();

                return oActivoAF;
            }

        }

        //Recuperar bien mueble
        [HttpGet]
        [Route("api/ActivoFijo/recuperarBienMueble/{id}")]
        public ActivoAF recuperarBienMueble(int id)
        {

            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                ActivoAF oActivoAF = new ActivoAF();
                ActivoFijo oActivo = bd.ActivoFijo.Where(p => p.IdBien == id).First();
                FormularioIngreso oFormulario = bd.FormularioIngreso.Where(p => p.NoFormulario == oActivo.NoFormulario).First();
                Clasificacion oClasificacion = bd.Clasificacion.Where(p => p.IdClasificacion == oActivo.IdClasificacion).First();
                Marcas omarca = (oActivo.IdMarca != null) ? bd.Marcas.Where(p => p.IdMarca == oActivo.IdMarca).First() : null;
                //  Proveedor oprov = (oActivo.IdProveedor != null) ? bd.Proveedor.Where(p => p.IdProveedor == oActivo.IdProveedor).First() : null;
                //  Donantes odona = (oActivo.IdDonante != null) ? bd.Donantes.Where(p => p.IdDonante == oActivo.IdDonante).First() : null;
                Empleado oemple = (oActivo.IdResponsable != null) ? bd.Empleado.Where(p => p.IdEmpleado == oActivo.IdResponsable).First() : null;

                //Llenar campos
                oActivoAF.idbien = oActivo.IdBien;
                oActivoAF.idclasificacion = oClasificacion.IdClasificacion;
                oActivoAF.estadoingreso = oActivo.EstadoIngreso;
                oActivoAF.fechaingreso = oFormulario.FechaIngreso == null ? " " : ((DateTime)oFormulario.FechaIngreso).ToString("yyyy-MM-dd");
                oActivoAF.tipoadquicicion = (int)oActivo.TipoAdquicicion;

                //oActivoAF.idproveedor = oprov.IdProveedor : odona.IdDonante;

                if (oActivo.IdProveedor != null)
                {
                    Proveedor oProveedor = bd.Proveedor.Where(p => p.IdProveedor == oActivo.IdProveedor).First();
                    oActivoAF.idproveedor = oProveedor.IdProveedor;
                    oActivoAF.IsProvDon = 1;
                }
                else
                {
                    Donantes oDonante = bd.Donantes.Where(p => p.IdDonante == oActivo.IdDonante).First();
                    oActivoAF.iddonante = oDonante.IdDonante;
                    oActivoAF.IsProvDon = 2;
                }
                if (oActivo.IdProveedor == null && oActivo.IdDonante == null)
                {
                    oActivoAF.ProvDon = "";
                }

                oActivoAF.descripcion = oActivo.Desripcion;
                oActivoAF.color = oActivo.Color;
                oActivoAF.idmarca = (omarca == null) ? 0 : omarca.IdMarca;
                oActivoAF.modelo = oActivo.Modelo;
                oActivoAF.nofactura = oFormulario.NoFactura;
                oActivoAF.valoradquicicion = (double)oActivo.ValorAdquicicion;



                //Datos del crédito
                if (oActivo.Prima != null)
                {
                    oActivoAF.prima = (double)oActivo.Prima;
                }
                else
                {

                }
                if (oActivo.PlazoPago != null)
                {
                    oActivoAF.plazopago = oActivo.PlazoPago;
                }
                else
                {

                }
                if (oActivo.CuotaAsignanda != null)
                {
                    oActivoAF.cuotaasignada = (double)oActivo.CuotaAsignanda;
                }
                else
                {

                }
                if (oActivo.Intereses != null)
                {
                    oActivoAF.interes = (double)oActivo.Intereses;
                }
                else
                {

                }

                oActivoAF.personaentrega = oFormulario.PersonaEntrega;
                oActivoAF.personarecibe = oFormulario.PersonaRecibe;
                oActivoAF.observaciones = oFormulario.Observaciones;
                oActivoAF.valorresidual = (double)oActivo.ValorResidual;
                oActivoAF.foto = oActivo.Foto;
                oActivoAF.noformularioactivo = oFormulario.NoFormulario;
                oActivoAF.cantidad = (from activo in bd.ActivoFijo
                                      join noFormulario in bd.FormularioIngreso
                                      on activo.NoFormulario equals noFormulario.NoFormulario
                                      where activo.NoFormulario == oActivo.NoFormulario && activo.EstadoActual == 1 && activo.EstaAsignado == 0
                                      select activo).ToList().Count();

                return oActivoAF;
            }

        }

        //Recuperar edificios e instalaciones
        [HttpGet]
        [Route("api/ActivoFijo/recuperarEdificioInsta/{id}")]
        public ActivoEdificiosIntangiblesAF recuperarEdificioInsta(int id)
        {

            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                ActivoEdificiosIntangiblesAF oEdificiosAF = new ActivoEdificiosIntangiblesAF();
                ActivoFijo oActivo = bd.ActivoFijo.Where(p => p.IdBien == id).First();
                FormularioIngreso oFormulario = bd.FormularioIngreso.Where(p => p.NoFormulario == oActivo.NoFormulario).First();
                Clasificacion oClasificacion = bd.Clasificacion.Where(p => p.IdClasificacion == oActivo.IdClasificacion).First();

                //Llenar campos
                oEdificiosAF.idbien = oActivo.IdBien;
                oEdificiosAF.idclasificacion = oClasificacion.IdClasificacion;
                oEdificiosAF.estadoingreso = oActivo.EstadoIngreso;
                oEdificiosAF.fechaingreso = oFormulario.FechaIngreso == null ? " " : ((DateTime)oFormulario.FechaIngreso).ToString("yyyy-MM-dd");
                oEdificiosAF.tipoadquicicion = (int)oActivo.TipoAdquicicion;

                if (oActivo.IdProveedor != null)
                {
                    Proveedor oProveedor = bd.Proveedor.Where(p => p.IdProveedor == oActivo.IdProveedor).First();
                    oEdificiosAF.idproveedor = oProveedor.IdProveedor;
                    oEdificiosAF.IsProvDon = 1;
                }
                else
                {
                    Donantes oDonante = bd.Donantes.Where(p => p.IdDonante == oActivo.IdDonante).First();
                    oEdificiosAF.iddonante = oDonante.IdDonante;
                    oEdificiosAF.IsProvDon = 2;
                }
                if (oActivo.IdProveedor == null && oActivo.IdDonante == null)
                {
                    oEdificiosAF.ProvDon = "";
                }

                oEdificiosAF.descripcion = oActivo.Desripcion;
                oEdificiosAF.valoradquicicion = (double)oActivo.ValorAdquicicion;



                //Datos del crédito
                if (oActivo.Prima != null)
                {
                    oEdificiosAF.prima = (double)oActivo.Prima;
                }
                else
                {

                }
                if (oActivo.PlazoPago != null)
                {
                    oEdificiosAF.plazopago = oActivo.PlazoPago;
                }
                else
                {

                }
                if (oActivo.CuotaAsignanda != null)
                {
                    oEdificiosAF.cuotaasignada = (double)oActivo.CuotaAsignanda;
                }
                else
                {

                }
                if (oActivo.Intereses != null)
                {
                    oEdificiosAF.interes = (double)oActivo.Intereses;
                }
                else
                {

                }

                oEdificiosAF.personaentrega = oFormulario.PersonaEntrega;
                oEdificiosAF.personarecibe = oFormulario.PersonaRecibe;
                oEdificiosAF.observaciones = oFormulario.Observaciones;
                oEdificiosAF.valorresidual = (double)oActivo.ValorResidual;
                oEdificiosAF.foto = oActivo.Foto;
                oEdificiosAF.noformularioactivo = oFormulario.NoFormulario;
                oEdificiosAF.vidautil = (int)oActivo.VidaUtil;

                return oEdificiosAF;
            }

        }

        //Método para recuperar fecha 
        [HttpGet]
        [Route("api/ActivoFijo/listarAnio")]
        public IEnumerable<PeriodoAF> listarAnio()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<PeriodoAF> listaAnio = (from periodo in bd.Periodo
                                                   where periodo.Estado==1
                                                   select new PeriodoAF
                                                   {
                                                       anio= (int)periodo.Anio
                                                   }).ToList();
                return listaAnio;
            }
        }
        [HttpGet]
        [Route("api/Depreciacion/RecuperarAnio")]
        public CierreAF RecuperarAnio()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                CierreAF odatos = new CierreAF();
                Cooperativa oCooperativa = bd.Cooperativa.Where(p => p.Dhabilitado == 1).First();
                Periodo oPeriodo = bd.Periodo.Where(p => p.Estado == 1).First();
                odatos.anio = oPeriodo.Anio.ToString();
                //odatos.cooperativa = oCooperativa.Nombre;
                //odatos.idPeriodo = oPeriodo.IdPeriodo;
                return odatos;
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
                    //Console.WriteLine("probando" + oActivoFijoAF.IdBien);
                    ActivoFijo oActivoFijo = bd.ActivoFijo.Where(p => p.IdBien == oActivoFijoAF.IdBien).First();
                    FormularioIngreso oFormularioIngreso = bd.FormularioIngreso.Where(p => p.NoFormulario == oActivoFijoAF.NoFormulario).First();
                    //oActivoFijo.NoFormulario = oFormularioIngreso.NoFormulario;
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
        [Route("api/ActivoFijo/buscarActivoAsig/{buscador?}")]
        public IEnumerable<RegistroAF> buscarActivoAsig(string buscador = "")
        {
            List<RegistroAF> listaActivo;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                if (buscador == "")
                {
                    listaActivo =( from activo in bd.ActivoFijo
                                  join noFormulario in bd.FormularioIngreso
                                  on activo.NoFormulario equals noFormulario.NoFormulario
                                  join clasif in bd.Clasificacion
                                  on activo.IdClasificacion equals clasif.IdClasificacion
                                  join resposable in bd.Empleado
                                  on activo.IdResponsable equals resposable.IdEmpleado
                                  join area in bd.AreaDeNegocio
                                  on resposable.IdAreaDeNegocio equals area.IdAreaNegocio
                                  where (activo.EstadoActual == 1 || activo.EstadoActual == 2 || activo.EstadoActual == 3) && activo.EstaAsignado == 1

                                  orderby activo.CorrelativoBien
                                  select new RegistroAF
                                  {
                                      IdBien = activo.IdBien,
                                      Codigo = activo.CorrelativoBien,
                                      fechacadena = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                      Descripcion = activo.Desripcion,
                                      Clasificacion = clasif.Clasificacion1,
                                      AreaDeNegocio = area.Nombre,
                                      Responsable = resposable.Nombres + " " + resposable.Apellidos
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
                                   where (activo.EstadoActual == 1 || activo.EstadoActual == 2 || activo.EstadoActual == 3) && activo.EstaAsignado == 1

                               
                                      && ((activo.IdBien).ToString().Contains(buscador)
                                      || (activo.CorrelativoBien).ToLower().Contains(buscador.ToLower())
                                      || (noFormulario.FechaIngreso).ToString().ToLower().Contains(buscador.ToLower())
                                      || (activo.Desripcion).ToLower().Contains(buscador.ToLower())
                                      || (clasif.Clasificacion1).ToLower().Contains(buscador.ToLower())
                                      || (area.Nombre).ToLower().Contains(buscador.ToLower())
                                      || (resposable.Nombres).ToLower().Contains(buscador.ToLower())
                                      || (resposable.Apellidos).ToLower().Contains(buscador.ToLower()) )
                                   orderby activo.CorrelativoBien
                                   select new RegistroAF
                                   {
                                       IdBien = activo.IdBien,
                                       Codigo = activo.CorrelativoBien,
                                       fechacadena = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                       Descripcion = activo.Desripcion,
                                       Clasificacion = clasif.Clasificacion1,
                                       AreaDeNegocio = area.Nombre,
                                       Responsable = resposable.Nombres + " " + resposable.Apellidos
                                   }).ToList();

                    return listaActivo;
                }
            }
        }
        [HttpGet]
        [Route("api/ActivoFijo/buscarActivoEdificioAsig/{buscador?}")]
        public IEnumerable<RegistroAF> buscarActivoEdificioAsig(string buscador = "")
        {
            List<RegistroAF> listaActivo;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                if (buscador == "")
                {
                    listaActivo = (from activo in bd.ActivoFijo
                                   join noFormulario in bd.FormularioIngreso
                                   on activo.NoFormulario equals noFormulario.NoFormulario
                                   join clasif in bd.Clasificacion
                                   on activo.IdClasificacion equals clasif.IdClasificacion
                                   where (activo.EstadoActual == 1 || activo.EstadoActual == 2 || activo.EstadoActual == 3) && activo.TipoActivo == 1 && activo.EstadoActual == 1
                                   orderby activo.CorrelativoBien
                                   select new RegistroAF
                                   {
                                       IdBien = activo.IdBien,
                                       Codigo = activo.CorrelativoBien,
                                       fechacadena = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                       Descripcion = activo.Desripcion,
                                       vidautil = activo.VidaUtil,
                                       Clasificacion = clasif.Clasificacion1
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
                                   where (activo.EstadoActual == 1 || activo.EstadoActual == 2 || activo.EstadoActual == 3) && activo.TipoActivo == 1 && activo.EstadoActual == 1
                               

                                     && ((activo.IdBien).ToString().Contains(buscador)
                                      || (activo.CorrelativoBien).ToLower().Contains(buscador.ToLower())
                                      || (noFormulario.FechaIngreso).ToString().ToLower().Contains(buscador.ToLower())
                                      || (activo.Desripcion).ToLower().Contains(buscador.ToLower())
                                      || (clasif.Clasificacion1).ToLower().Contains(buscador.ToLower()) )
                                   orderby activo.CorrelativoBien

                                   select new RegistroAF
                                   {
                                       IdBien = activo.IdBien,
                                       Codigo = activo.CorrelativoBien,
                                       fechacadena = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                       Descripcion = activo.Desripcion,
                                       vidautil = activo.VidaUtil,
                                       Clasificacion = clasif.Clasificacion1

                                   }).ToList();

                    return listaActivo;
                }
            }
        }
        [HttpGet]
        [Route("api/ActivoFijo/buscarActivoIntengibleAsig/{buscador?}")]
        public IEnumerable<RegistroAF> buscarActivoIntengibleAsig(string buscador = "")
        {
            List<RegistroAF> listaActivo;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                if (buscador == "")
                {
                    listaActivo = (from activo in bd.ActivoFijo
                                   join noFormulario in bd.FormularioIngreso
                                   on activo.NoFormulario equals noFormulario.NoFormulario
                                   join clasif in bd.Clasificacion
                                   on activo.IdClasificacion equals clasif.IdClasificacion
                                   where (activo.EstadoActual == 1 || activo.EstadoActual == 2 || activo.EstadoActual == 3) && activo.TipoActivo == 3 && activo.EstadoActual == 1
                                   orderby activo.CorrelativoBien
                                   select new RegistroAF
                                   {
                                       IdBien = activo.IdBien,
                                       Codigo = activo.CorrelativoBien,
                                       fechacadena = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                       Descripcion = activo.Desripcion,
                                       vidautil = activo.VidaUtil,
                                       Clasificacion = clasif.Clasificacion1
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
                                   where (activo.EstadoActual == 1 || activo.EstadoActual == 2 || activo.EstadoActual == 3) && activo.TipoActivo == 3 && activo.EstadoActual == 1
                              
                                     && ((activo.IdBien).ToString().Contains(buscador)
                                      || (activo.CorrelativoBien).ToLower().Contains(buscador.ToLower())
                                      || (noFormulario.FechaIngreso).ToString().ToLower().Contains(buscador.ToLower())
                                      || (activo.Desripcion).ToLower().Contains(buscador.ToLower())
                                      || (clasif.Clasificacion1).ToLower().Contains(buscador.ToLower()))

                                   orderby activo.CorrelativoBien
                                   select new RegistroAF
                                   {
                                       IdBien = activo.IdBien,
                                       Codigo = activo.CorrelativoBien,
                                       fechacadena = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                       Descripcion = activo.Desripcion,
                                       vidautil = activo.VidaUtil,
                                       Clasificacion = clasif.Clasificacion1

                                   }).ToList();

                    return listaActivo;
                }
            }
        }
        //Buscadores no asignados
        [HttpGet]
        [Route("api/ActivoFijo/buscarActivoNoAsig/{buscador?}")]
        public IEnumerable<NoAsignadosAF> buscarActivo(string buscador = "")
        {
            List<NoAsignadosAF> listaActivo;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                if (buscador == "")
                {
                    listaActivo = (from activo in bd.ActivoFijo
                                   join noFormulario in bd.FormularioIngreso
                                   on activo.NoFormulario equals noFormulario.NoFormulario
                                   join clasif in bd.Clasificacion
                                   on activo.IdClasificacion equals clasif.IdClasificacion
                                   where (activo.EstadoActual == 1 || activo.EstadoActual == 2 || activo.EstadoActual == 3) && activo.EstaAsignado == 0

                                   select new NoAsignadosAF
                                   {
                                       IdBien = activo.IdBien,
                                       NoFormulario=noFormulario.NoFormulario,
                                       fechacadena = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                       Desripcion = activo.Desripcion,
                                       Clasificacion = clasif.Clasificacion1,
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
                                   where (activo.EstadoActual == 1 || activo.EstadoActual == 2 || activo.EstadoActual == 3) && activo.EstaAsignado == 0

                                     && ((activo.IdBien).ToString().Contains(buscador)
                                      || (noFormulario.FechaIngreso).ToString().ToLower().Contains(buscador.ToLower())
                                      || (activo.Desripcion).ToLower().Contains(buscador.ToLower())
                                       || (noFormulario.NoFormulario).ToString().Contains(buscador.ToLower())
                                      || (clasif.Clasificacion1).ToLower().Contains(buscador.ToLower())
                                      )

                                   select new NoAsignadosAF
                                   {
                                       IdBien = activo.IdBien,
                                       NoFormulario = noFormulario.NoFormulario,
                                       fechacadena = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                       Desripcion = activo.Desripcion,
                                       Clasificacion = clasif.Clasificacion1,
                                   }).ToList();

                    return listaActivo;
                }
            }
        }

        //Para modal de activos asignados
        [HttpGet]
        [Route("api/ActivoFijo/DatosGeneralesActivosAsignados/{id}")]
        public DatosGeneralesActivosAsignadosAF DatosGeneralesActivosAsignados(int id)
        {

            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                DatosGeneralesActivosAsignadosAF oDatosAF = new DatosGeneralesActivosAsignadosAF();
                ActivoFijo oActivo = bd.ActivoFijo.Where(p => p.IdBien == id).First();
                FormularioIngreso oFOrmulario = bd.FormularioIngreso.Where(p => p.NoFormulario == oActivo.NoFormulario).First();
                Clasificacion oclasi = bd.Clasificacion.Where(p => p.IdClasificacion == oActivo.IdClasificacion).First();
                
                Empleado oEmpleado = bd.Empleado.Where(p => p.IdEmpleado == oActivo.IdResponsable).First();
                AreaDeNegocio oArea = bd.AreaDeNegocio.Where(p => p.IdAreaNegocio == oEmpleado.IdAreaDeNegocio).First();
                Sucursal oSucursal = bd.Sucursal.Where(p => p.IdSucursal == oArea.IdSucursal).First();

                Marcas omarca = (oActivo.IdMarca != null) ? bd.Marcas.Where(p => p.IdMarca == oActivo.IdMarca).First() : null;
                if (omarca == null)
                {
                    oDatosAF.marca = "";
                }
                else
                {
                    oDatosAF.marca = omarca.Marca;
                }

                if (oActivo.IdProveedor != null)
                {
                    Proveedor oProveedor = bd.Proveedor.Where(p => p.IdProveedor == oActivo.IdProveedor).First();
                    oDatosAF.ProvDon = oProveedor.Nombre;
                    oDatosAF.IsProvDon = 1;
                }
                else
                {
                    Donantes oDonante = bd.Donantes.Where(p => p.IdDonante == oActivo.IdDonante).First();
                    oDatosAF.ProvDon = oDonante.Nombre;
                    oDatosAF.IsProvDon = 2;
                }
                if (oActivo.IdProveedor == null && oActivo.IdDonante == null)
                {
                    oDatosAF.ProvDon = "";
                }

                TarjetaDepreciacion oTarjeta = bd.TarjetaDepreciacion.Where(p => p.IdBien == oActivo.IdBien).Last();

                oDatosAF.idBien = (int)oActivo.IdBien;
                oDatosAF.fecha = oFOrmulario.FechaIngreso == null ? " " : ((DateTime)oFOrmulario.FechaIngreso).ToString("dd-MM-yyyy");
                oDatosAF.codigo = oActivo.CorrelativoBien;
                oDatosAF.descripcion = oActivo.Desripcion;
                oDatosAF.valorAquisicion = oActivo.ValorAdquicicion.ToString();
               
                oDatosAF.modelo = oActivo.Modelo;
                //Datos del estado del activo
                if (oActivo.EstadoIngreso == "1")
                {
                    oDatosAF.estadoingreso = "Nuevo";
                }
                else if (oActivo.EstadoIngreso == "2")
                {
                    oDatosAF.estadoingreso = "Usado";
                }
                else
                {
                    oDatosAF.estadoingreso = "Usado mal estado";
                }
                oDatosAF.color = oActivo.Color;
                oDatosAF.clasificacion = oclasi.Clasificacion1;
                oDatosAF.responsable = oEmpleado.Nombres + " " + oEmpleado.Apellidos;
                oDatosAF.Ubicacion = oArea.Nombre + " - " + oSucursal.Nombre;
                //Datos para el tipo de adquisición
                if (oActivo.TipoAdquicicion == 1)
                {
                    oDatosAF.tipoadquicicion = "Contado";
                }
                else if (oActivo.TipoAdquicicion == 2)
                {
                    oDatosAF.tipoadquicicion = "Crédito";
                }
                else
                {
                    oDatosAF.tipoadquicicion = "Donado";
                }
                //Datos del crédito
                if (oActivo.Prima != null)
                {
                    oDatosAF.prima = oActivo.Prima.ToString();
                }
                else
                {
                    oDatosAF.prima = "";
                }
                if (oActivo.PlazoPago != null)
                {
                    oDatosAF.plazo = oActivo.PlazoPago.ToString();
                }
                else
                {
                    oDatosAF.plazo = "";
                }
                if (oActivo.CuotaAsignanda != null)
                {
                    oDatosAF.cuota = oActivo.CuotaAsignanda.ToString();
                }
                else
                {
                    oDatosAF.cuota = "";
                }
                if (oActivo.Intereses != null)
                {
                    oDatosAF.interes = oActivo.Intereses.ToString();
                }
                else
                {
                    oDatosAF.interes = "";
                }
                oDatosAF.VidaUtil = oActivo.VidaUtil.ToString();
                oDatosAF.valorresidual = oActivo.ValorResidual.ToString();
                oDatosAF.Observaciones = oFOrmulario.Observaciones;
                oDatosAF.foto = oActivo.Foto;
                return oDatosAF;
            }

        }

        //Para modal de activos no asignados
        [HttpGet]
        [Route("api/ActivoFijo/DatosGeneralesActivosNoAsignados/{id}")]
        public DatosGeneralesActivosNoAsignadosAF DatosGeneralesActivosNoAsignados(int id)
        {

            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                DatosGeneralesActivosNoAsignadosAF oDatosAF = new DatosGeneralesActivosNoAsignadosAF();
                ActivoFijo oActivo = bd.ActivoFijo.Where(p => p.IdBien == id).First();
                FormularioIngreso oFOrmulario = bd.FormularioIngreso.Where(p => p.NoFormulario == oActivo.NoFormulario).First();
                Clasificacion oclasi = bd.Clasificacion.Where(p => p.IdClasificacion == oActivo.IdClasificacion).First();
                Marcas omarca = (oActivo.IdMarca != null) ? bd.Marcas.Where(p => p.IdMarca == oActivo.IdMarca).First() : null;
                if (omarca == null)
                {
                    oDatosAF.marca = "";
                }
                else
                {
                    oDatosAF.marca = omarca.Marca;
                }

                if (oActivo.IdProveedor != null)
                {
                    Proveedor oProveedor = bd.Proveedor.Where(p => p.IdProveedor == oActivo.IdProveedor).First();
                    oDatosAF.ProvDon = oProveedor.Nombre;
                    oDatosAF.IsProvDon = 1;
                }
                else
                {
                    Donantes oDonante = bd.Donantes.Where(p => p.IdDonante == oActivo.IdDonante).First();
                    oDatosAF.ProvDon = oDonante.Nombre;
                    oDatosAF.IsProvDon = 2;
                }
                if (oActivo.IdProveedor == null && oActivo.IdDonante == null)
                {
                    oDatosAF.ProvDon = "";
                }

                TarjetaDepreciacion oTarjeta = bd.TarjetaDepreciacion.Where(p => p.IdBien == oActivo.IdBien).Last();

                oDatosAF.idBien = (int)oActivo.IdBien;
                oDatosAF.fecha = oFOrmulario.FechaIngreso == null ? " " : ((DateTime)oFOrmulario.FechaIngreso).ToString("dd-MM-yyyy");
                oDatosAF.descripcion = oActivo.Desripcion;
                oDatosAF.valorAquisicion = oActivo.ValorAdquicicion.ToString();

                oDatosAF.modelo = oActivo.Modelo;
                //Datos del estado del activo
                if (oActivo.EstadoIngreso == "1")
                {
                    oDatosAF.estadoingreso = "Nuevo";
                }
                else if (oActivo.EstadoIngreso == "2")
                {
                    oDatosAF.estadoingreso = "Usado";
                }
                else
                {
                    oDatosAF.estadoingreso = "Usado mal estado";
                }
                oDatosAF.color = oActivo.Color;
                oDatosAF.clasificacion = oclasi.Clasificacion1;
                //Datos para el tipo de adquisición
                if (oActivo.TipoAdquicicion == 1)
                {
                    oDatosAF.tipoadquicicion = "Contado";
                }
                else if (oActivo.TipoAdquicicion == 2)
                {
                    oDatosAF.tipoadquicicion = "Crédito";
                }
                else
                {
                    oDatosAF.tipoadquicicion = "Donado";
                }
                //Datos del crédito
                if (oActivo.Prima != null)
                {
                    oDatosAF.prima = oActivo.Prima.ToString();
                }
                else
                {
                    oDatosAF.prima = "";
                }
                if (oActivo.PlazoPago != null)
                {
                    oDatosAF.plazo = oActivo.PlazoPago.ToString();
                }
                else
                {
                    oDatosAF.plazo = "";
                }
                if (oActivo.CuotaAsignanda != null)
                {
                    oDatosAF.cuota = oActivo.CuotaAsignanda.ToString();
                }
                else
                {
                    oDatosAF.cuota = "";
                }
                if (oActivo.Intereses != null)
                {
                    oDatosAF.interes = oActivo.Intereses.ToString();
                }
                else
                {
                    oDatosAF.interes = "";
                }
                oDatosAF.valorresidual = oActivo.ValorResidual.ToString();
                oDatosAF.Observaciones = oFOrmulario.Observaciones;
                oDatosAF.foto = oActivo.Foto;
                return oDatosAF;
            }

        }

        //Para modal de edificios e instalaciones
        [HttpGet]
        [Route("api/ActivoFijo/DatosGeneralesEdificios/{id}")]
        public DatosGeneralesEdificiosAF DatosGeneralesEdificios(int id)
        {

            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                DatosGeneralesEdificiosAF oDatosAF = new DatosGeneralesEdificiosAF();
                ActivoFijo oActivo = bd.ActivoFijo.Where(p => p.IdBien == id).First();
                FormularioIngreso oFOrmulario = bd.FormularioIngreso.Where(p => p.NoFormulario == oActivo.NoFormulario).First();
                Clasificacion oclasi = bd.Clasificacion.Where(p => p.IdClasificacion == oActivo.IdClasificacion).First();

                if (oActivo.IdProveedor != null)
                {
                    Proveedor oProveedor = bd.Proveedor.Where(p => p.IdProveedor == oActivo.IdProveedor).First();
                    oDatosAF.ProvDon = oProveedor.Nombre;
                    oDatosAF.IsProvDon = 1;
                }
                else
                {
                    Donantes oDonante = bd.Donantes.Where(p => p.IdDonante == oActivo.IdDonante).First();
                    oDatosAF.ProvDon = oDonante.Nombre;
                    oDatosAF.IsProvDon = 2;
                }
                if (oActivo.IdProveedor == null && oActivo.IdDonante == null)
                {
                    oDatosAF.ProvDon = "";
                }

                TarjetaDepreciacion oTarjeta = bd.TarjetaDepreciacion.Where(p => p.IdBien == oActivo.IdBien).Last();

                oDatosAF.idBien = (int)oActivo.IdBien;
                oDatosAF.descripcion = oActivo.Desripcion;
                oDatosAF.fecha = oFOrmulario.FechaIngreso == null ? " " : ((DateTime)oFOrmulario.FechaIngreso).ToString("dd-MM-yyyy");
                oDatosAF.codigo = oActivo.CorrelativoBien;
                oDatosAF.valorAquisicion = oActivo.ValorAdquicicion.ToString();
                oDatosAF.clasificacion = oclasi.Clasificacion1;
                oDatosAF.VidaUtil = oActivo.VidaUtil.ToString();
                oDatosAF.Observaciones = oFOrmulario.Observaciones;

                //Datos del estado del edificio
                if (oActivo.EstadoIngreso == "1")
                {
                    oDatosAF.estadoingreso = "Nuevo";
                }
                else if (oActivo.EstadoIngreso == "2")
                {
                    oDatosAF.estadoingreso = "Usado";
                }
                else
                {
                    oDatosAF.estadoingreso = "Usado mal estado";
                }

                //Datos para el tipo de adquisición
                if (oActivo.TipoAdquicicion == 1)
                {
                    oDatosAF.tipoadquicicion = "Contado";
                }
                else if (oActivo.TipoAdquicicion == 2)
                {
                    oDatosAF.tipoadquicicion = "Crédito";
                }
                else
                {
                    oDatosAF.tipoadquicicion = "Donado";
                }

                //Datos del crédito
                if (oActivo.Prima!=null)
                {
                    oDatosAF.prima = oActivo.Prima.ToString();
                } else
                {
                    oDatosAF.prima = "";
                }
                if(oActivo.PlazoPago!=null)
                {
                    oDatosAF.plazo = oActivo.PlazoPago.ToString();
                } else
                {
                    oDatosAF.plazo = "";
                }
                if(oActivo.CuotaAsignanda!=null)
                {
                    oDatosAF.cuota = oActivo.CuotaAsignanda.ToString();
                } else
                {
                    oDatosAF.cuota = "";
                }
                if(oActivo.Intereses!=null)
                {
                    oDatosAF.interes = oActivo.Intereses.ToString();
                } else
                {
                    oDatosAF.interes = "";
                }
                oDatosAF.valorresidual = oActivo.ValorResidual.ToString();
                oDatosAF.foto = oActivo.Foto;
                return oDatosAF;
            }

        }

        //Para modal de activos intangibles
        [HttpGet]
        [Route("api/ActivoFijo/DatosGeneralesIntangibles/{id}")]
        public DatosGeneralesIntangiblesAF DatosGeneralesIntangibles(int id)
        {

            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                DatosGeneralesIntangiblesAF oDatosAF = new DatosGeneralesIntangiblesAF();
                ActivoFijo oActivo = bd.ActivoFijo.Where(p => p.IdBien == id).First();
                FormularioIngreso oFOrmulario = bd.FormularioIngreso.Where(p => p.NoFormulario == oActivo.NoFormulario).First();
                Clasificacion oclasi = bd.Clasificacion.Where(p => p.IdClasificacion == oActivo.IdClasificacion).First();

                if (oActivo.IdProveedor != null)
                {
                    Proveedor oProveedor = bd.Proveedor.Where(p => p.IdProveedor == oActivo.IdProveedor).First();
                    oDatosAF.ProvDon = oProveedor.Nombre;
                    oDatosAF.IsProvDon = 1;
                }
                else
                {
                    Donantes oDonante = bd.Donantes.Where(p => p.IdDonante == oActivo.IdDonante).First();
                    oDatosAF.ProvDon = oDonante.Nombre;
                    oDatosAF.IsProvDon = 2;
                }
                if (oActivo.IdProveedor == null && oActivo.IdDonante == null)
                {
                    oDatosAF.ProvDon = "";
                }

                TarjetaDepreciacion oTarjeta = bd.TarjetaDepreciacion.Where(p => p.IdBien == oActivo.IdBien).Last();

                oDatosAF.idBien = (int)oActivo.IdBien;
                oDatosAF.descripcion = oActivo.Desripcion;
                oDatosAF.fecha = oFOrmulario.FechaIngreso == null ? " " : ((DateTime)oFOrmulario.FechaIngreso).ToString("dd-MM-yyyy");
                oDatosAF.codigo = oActivo.CorrelativoBien;
                oDatosAF.valorAquisicion = oActivo.ValorAdquicicion.ToString();
                oDatosAF.clasificacion = oclasi.Clasificacion1;
                oDatosAF.VidaUtil = oActivo.VidaUtil.ToString();
                oDatosAF.Observaciones = oFOrmulario.Observaciones;

                //Datos para el tipo de adquisición
                if (oActivo.TipoAdquicicion == 1)
                {
                    oDatosAF.tipoadquicicion = "Contado";
                }
                else if (oActivo.TipoAdquicicion == 2)
                {
                    oDatosAF.tipoadquicicion = "Crédito";
                }
                else
                {
                    oDatosAF.tipoadquicicion = "Donado";
                }

                //Datos del crédito
                if (oActivo.Prima != null)
                {
                    oDatosAF.prima = oActivo.Prima.ToString();
                }
                else
                {
                    oDatosAF.prima = "";
                }
                if (oActivo.PlazoPago != null)
                {
                    oDatosAF.plazo = oActivo.PlazoPago.ToString();
                }
                else
                {
                    oDatosAF.plazo = "";
                }
                if (oActivo.CuotaAsignanda != null)
                {
                    oDatosAF.cuota = oActivo.CuotaAsignanda.ToString();
                }
                else
                {
                    oDatosAF.cuota = "";
                }
                if (oActivo.Intereses != null)
                {
                    oDatosAF.interes = oActivo.Intereses.ToString();
                }
                else
                {
                    oDatosAF.interes = "";
                }
                oDatosAF.valorresidual = oActivo.ValorResidual.ToString();
                oDatosAF.foto = oActivo.Foto;
                return oDatosAF;
            }

        }


        [HttpGet]
        [Route("api/ActivoFIjo/listarActivosFiltro/{id}")]
        public IEnumerable<RegistroAF> listarActivosFiltro(int id)
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<RegistroAF> lista = (from activo in bd.ActivoFijo
                                                   join noFormulario in bd.FormularioIngreso
                                                   on activo.NoFormulario equals noFormulario.NoFormulario
                                                   join clasif in bd.Clasificacion
                                                   on activo.IdClasificacion equals clasif.IdClasificacion
                                                   join resposable in bd.Empleado
                                                   on activo.IdResponsable equals resposable.IdEmpleado
                                                   join area in bd.AreaDeNegocio
                                                   on resposable.IdAreaDeNegocio equals area.IdAreaNegocio
                                                   where (activo.EstadoActual == 1 || activo.EstadoActual == 2 || activo.EstadoActual == 3) && activo.EstaAsignado == 1 && area.IdAreaNegocio == id
                                                   orderby activo.CorrelativoBien
                                                   select new RegistroAF
                                                   {
                                                       IdBien = activo.IdBien,
                                                       Codigo = activo.CorrelativoBien,
                                                       fechacadena = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                                       Descripcion = activo.Desripcion,
                                                       Clasificacion = clasif.Clasificacion1,
                                                       AreaDeNegocio = area.Nombre,
                                                       Responsable = resposable.Nombres + " " + resposable.Apellidos
                                                   }).ToList();
                return lista;

            }
        }


    }
}