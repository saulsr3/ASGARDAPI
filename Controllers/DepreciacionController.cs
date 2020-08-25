﻿using System;
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
                                                            where (activo.EstadoActual == 1 || activo.EstadoActual == 2) &&( activo.EstaAsignado==0 || activo.EstaAsignado==1)
                                                            select new DepreciacionAF
                                                            {
                                                                idBien=activo.IdBien,
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
                                                                         orderby tarjeta.Fecha
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


    }
}