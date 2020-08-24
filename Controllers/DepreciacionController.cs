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
                IEnumerable<DepreciacionAF> listaActivos = (from activo in bd.ActivoFijo
                                                            join empleado in bd.Empleado
                                                            on activo.IdResponsable equals empleado.IdEmpleado
                                                            join area in bd.AreaDeNegocio
                                                            on empleado.IdAreaDeNegocio equals area.IdAreaNegocio
                                                            join sucursal in bd.Sucursal
                                                            on area.IdSucursal equals sucursal.IdSucursal
                                                            where activo.EstadoActual == 1 || activo.EstadoActual == 2
                                                            select new DepreciacionAF
                                                            {
                                                                codigo = activo.CorrelativoBien,
                                                                descripcion = activo.Desripcion,
                                                                areanegocio = area.Nombre,
                                                                sucursal= sucursal.Nombre,
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
                IEnumerable<DepreciacionAF> listaActivos = (from activo in bd.ActivoFijo
                                                            join empleado in bd.Empleado
                                                            on activo.IdResponsable equals empleado.IdEmpleado
                                                            join area in bd.AreaDeNegocio
                                                            on empleado.IdAreaDeNegocio equals area.IdAreaNegocio
                                                            join sucursal in bd.Sucursal
                                                           on area.IdSucursal equals sucursal.IdSucursal
                                                            where (activo.EstadoActual == 1 || activo.EstadoActual == 2) && area.IdAreaNegocio==id
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
        [Route("api/Depreciacion/DatosDepreciacion/{idBien}")]
        public BienesDepreciacionAF DatosDepreciacion(int idBien)
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                BienesDepreciacionAF odatos = new BienesDepreciacionAF();
                ActivoFijo oactivo = bd.ActivoFijo.Where(p => p.IdBien == idBien).First();

                DateTime _date = DateTime.Now;
                var _dateString = _date.ToString("yyyy");
                odatos.fecha= _dateString;
                odatos.codigo = oactivo.CorrelativoBien;
                odatos.descipcion = oactivo.Desripcion;
                odatos.valorDepreciacion = 2555;
                odatos.mejoras = 00;
                
                return odatos;

            }
        }
        [HttpGet]
        [Route("api/Depreciacion/TarjetaDatos/{idBien}")]
        public BienesDepreciacionAF TarjetaDatos(int idBien)
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                BienesDepreciacionAF odatos = new BienesDepreciacionAF();
                ActivoFijo oactivo = bd.ActivoFijo.Where(p => p.IdBien == idBien).First();

                DateTime _date = DateTime.Now;
                var _dateString = _date.ToString("yyyy");
                odatos.fecha = _dateString;
                odatos.codigo = oactivo.CorrelativoBien;
                odatos.descipcion = oactivo.Desripcion;
                odatos.valorDepreciacion = 2555;
                odatos.mejoras = 00;

                return odatos;

            }
        }


    }
}