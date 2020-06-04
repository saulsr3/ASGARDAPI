using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ASGARDAPI.Models;
using ASGARDAPI.Clases;

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
                int oActivoC = bd.ActivoFijo.Where(p => p.IdClasificacion == oclasificacion.IdClasificacion).Count();
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
    }
}