using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ASGARDAPI.Models;
using ASGARDAPI.Clases;
using BarcodeLib;

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
        public IEnumerable<ActivoFijoAF> listarActivosNoAsignados()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<ActivoFijoAF> lista = (from activo in bd.ActivoFijo
                                                   join noFormulario in bd.FormularioIngreso
                                                   on activo.NoFormulario equals noFormulario.NoFormulario
                                                   join clasif in bd.Clasificacion
                                                   on activo.IdClasificacion equals clasif.IdClasificacion
                                                   join marca in bd.Marcas
                                                   on activo.IdMarca equals marca.IdMarca
                                                   where activo.EstadoActual == 1 && activo.EstaAsignado == 0
                                                   orderby noFormulario.NoFormulario
                                                   select new ActivoFijoAF
                                                   {
                                                       IdBien = activo.IdBien,
                                                       NoFormulario = noFormulario.NoFormulario,
                                                       fechacadena = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                                       Desripcion = activo.Desripcion,
                                                       Clasificacion = clasif.Clasificacion1,
                                                       Marca = marca.Marca
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
        //Medotos para modulo de control mayra
        [HttpGet]
        [Route("api/ActivoFIjo/listarActivos")]
        public IEnumerable<ActivoFijoAF> listarActivos()
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
                                                   where activo.EstadoActual == 1 && activo.EstaAsignado == 1
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
        [Route("api/ActivoFijo/RecuperarBienes/{id}")]
        public ActivoFijoAF RecuperarBienes(int id)
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                ActivoFijoAF oActivoFijoAF = new ActivoFijoAF();
                ActivoFijo oActivoFijo = bd.ActivoFijo.Where(p => p.IdBien == id).First();
                oActivoFijoAF.IdBien = oActivoFijo.IdBien;
                oActivoFijoAF.Desripcion = oActivoFijo.Desripcion;
                oActivoFijoAF.NoFormulario = (int)oActivoFijo.NoFormulario;
                oActivoFijoAF.idclasificacion = (int)oActivoFijo.IdClasificacion;
                oActivoFijoAF.color = oActivoFijo.Color;
                oActivoFijoAF.modelo = oActivoFijo.Modelo;

                return oActivoFijoAF;
            }
        }

        [HttpGet]
        [Route("api/ActivoFijo/listarAreaCombo")]
        public IEnumerable<ComboAnidadoAF> listarAreaCombo()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<ComboAnidadoAF> listarAreas = (from area in bd.AreaDeNegocio
                                                           where area.Dhabilitado == 1
                                                           select new ComboAnidadoAF
                                                           {
                                                               id = area.IdAreaNegocio,
                                                               nombre = area.Nombre

                                                           }).ToList();


                return listarAreas;

            }
        }

        [HttpGet]
        [Route("api/ActivoFijo/listarSucursalCombo")]
        public IEnumerable<ComboAnidadoAF> listarSucursalCombo()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<ComboAnidadoAF> listarSucursal = (from sucursal in bd.Sucursal
                                                              where sucursal.Dhabilitado == 1
                                                              select new ComboAnidadoAF
                                                              {
                                                                  id = sucursal.IdSucursal,
                                                                  nombre = sucursal.Nombre

                                                              }).ToList();


                return listarSucursal;

            }
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
    }
}