using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASGARDAPI.Clases;
using ASGARDAPI.Models;
using Microsoft.AspNetCore.Mvc;


namespace ASGARDAPI.Controllers
{
    public class ConfiguracionController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

        //Listar
        [HttpGet]
        [Route("api/Cooperativa/listarCooperativa")]
        public IEnumerable<CooperativaAF> listarCooperativa()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<CooperativaAF> listarCooperativa = (from cooperativa in bd.Cooperativa
                                                                join operiodo in bd.Periodo
                                                                on cooperativa.IdCooperativa equals operiodo.IdCooperativa                                        
                                                   where cooperativa.Dhabilitado == 1
                                                   select new CooperativaAF
                                                   {
                                                       idcooperativa=cooperativa.IdCooperativa,
                                                       nombre=cooperativa.Nombre,
                                                       anio= (int)operiodo.Anio,
                                                       descripcion = cooperativa.Descripcion

                                                   }).ToList();
                return listarCooperativa;
            }
        }

        //Guardar
        [HttpPost]
        [Route("api/Cooperativa/guardarCooperativa")]
        public int guardarCooperativa([FromBody] CooperativaAF oCooperativaF, PeriodoAF oPeriodoAF)
        {
            int rpta = 0;

            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    Cooperativa oCooperativa = new Cooperativa();
                    oCooperativa.IdCooperativa = oCooperativaF.idcooperativa;
                    oCooperativa.Nombre = oCooperativaF.nombre;
                    oCooperativa.Logo = oCooperativaF.logo;
                    oCooperativa.Descripcion = oCooperativaF.descripcion;
                    oCooperativa.Dhabilitado = 1;
                    bd.Cooperativa.Add(oCooperativa);
                    bd.SaveChanges();

                    //Para guardar en la tabla periodo
                    Periodo oPeriodo = new Periodo();
                    Cooperativa oCooperativaPeriodo = bd.Cooperativa.Last();
                    oPeriodo.IdPeriodo = oPeriodoAF.idperiodo;

                    //Le mando el id de la cooperativa a la tabla periodo
                    oPeriodo.IdCooperativa = oCooperativaPeriodo.IdCooperativa;

                    oPeriodo.Anio = oCooperativaF.anio;
                    oPeriodo.Estado = 1;
                    bd.Periodo.Add(oPeriodo);
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

        //Recuperar
        [HttpGet]
        [Route("api/Cooperativa/recuperarCooperativa/{id}")]
        public CooperativaAF recuperarCooperativa(int id)
        {

            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                CooperativaAF oCooperativaAF = new CooperativaAF();
                Cooperativa oCooperativa = bd.Cooperativa.Where(p => p.IdCooperativa == id).First();
                oCooperativaAF.idcooperativa = oCooperativa.IdCooperativa;
                oCooperativaAF.nombre = oCooperativa.Nombre;
                oCooperativaAF.logo = oCooperativa.Logo;

                return oCooperativaAF;
            }

        }

        //Validar
        [HttpGet]
        [Route("api/Cooperativa/validarCooperativa/{idcooperativa}/{cooperativa}")]
        public int validarCooperativa(int idcooperativa, string cooperativa)
        {
            int rpta = 0;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    if (idcooperativa == 0)
                    {
                        rpta = bd.Cooperativa.Where(p => p.Nombre.ToLower() == cooperativa.ToLower() && p.Dhabilitado == 1).Count();
                    }
                    else
                    {
                        rpta = bd.Cooperativa.Where(p => p.Nombre.ToLower() == cooperativa.ToLower() && p.IdCooperativa != idcooperativa && p.Dhabilitado == 1).Count();
                    }
                }

            }
            catch (Exception ex)
            {
                return rpta = 0;
            }

            return rpta;
        }
        [HttpGet]
        [Route("api/Configuracion/DatosGenerales/{id}")]
        public DatosGenerlesAF DatosGenerales(int id)
        {

            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                DatosGenerlesAF oDatosF = new DatosGenerlesAF();
                ActivoFijo oActivo = bd.ActivoFijo.Where(p => p.IdBien == id).First();
                FormularioIngreso oFOrmulario = bd.FormularioIngreso.Where(p => p.NoFormulario == oActivo.NoFormulario).First();
                Empleado oEmpleado = bd.Empleado.Where(p => p.IdEmpleado == oActivo.IdResponsable).First();
                AreaDeNegocio oArea = bd.AreaDeNegocio.Where(p => p.IdAreaNegocio == oEmpleado.IdAreaDeNegocio).First();
                Sucursal oSucursal = bd.Sucursal.Where(p => p.IdSucursal == oArea.IdSucursal).First();
                if (oActivo.IdProveedor != null)
                {
                    Proveedor oProveedor = bd.Proveedor.Where(p => p.IdProveedor == oActivo.IdProveedor).First();
                    oDatosF.ProvDon = oProveedor.Nombre;
                    oDatosF.IsProvDon = 1;
                }
                else {
                    Donantes oDonante = bd.Donantes.Where(p => p.IdDonante == oActivo.IdDonante).First();
                    oDatosF.ProvDon = oDonante.Nombre;
                    oDatosF.IsProvDon = 2;
                }
                if (oActivo.IdProveedor==null && oActivo.IdDonante==null) {
                    oDatosF.ProvDon = "";
                }
                
                TarjetaDepreciacion oTarjeta= bd.TarjetaDepreciacion.Where(p => p.IdBien ==  oActivo.IdBien).Last();

                oDatosF.idBien = (int)oActivo.IdBien;
                oDatosF.descripcion = oActivo.Desripcion;
                oDatosF.fecha = oFOrmulario.FechaIngreso == null ? " " : ((DateTime)oFOrmulario.FechaIngreso).ToString("dd-MM-yyyy");
                oDatosF.codigo = oActivo.CorrelativoBien;
                oDatosF.valorAquisicion = oActivo.ValorAdquicicion.ToString();
                oDatosF.Respondable = oEmpleado.Nombres + " " + oEmpleado.Apellidos;
                oDatosF.Ubicacion = oArea.Nombre + " - " + oSucursal.Nombre;
                oDatosF.valorActual = oTarjeta.ValorActual.ToString();
                oDatosF.NoSerie = oActivo.NoSerie;
                oDatosF.VidaUtil = oActivo.VidaUtil.ToString();
                oDatosF.Observaciones = oFOrmulario.Observaciones;

                oDatosF.foto = oActivo.Foto;
                return oDatosF;
            }

        }
        [HttpGet]
        [Route("api/Configuracion/DatosGeneralesEdificiosIntangibles/{id}")]
        public DatosGenerlesAF DatosGeneralesEdificiosIntangibles(int id)
        {

            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                DatosGenerlesAF oDatosF = new DatosGenerlesAF();
                ActivoFijo oActivo = bd.ActivoFijo.Where(p => p.IdBien == id).First();
                Clasificacion oClasificacion = bd.Clasificacion.Where(p => p.IdClasificacion == oActivo.IdClasificacion).First();
                FormularioIngreso oFOrmulario = bd.FormularioIngreso.Where(p => p.NoFormulario == oActivo.NoFormulario).First();
                //Empleado oEmpleado = bd.Empleado.Where(p => p.IdEmpleado == oActivo.IdResponsable).First();
                //AreaDeNegocio oArea = bd.AreaDeNegocio.Where(p => p.IdAreaNegocio == oEmpleado.IdAreaDeNegocio).First();
                //Sucursal oSucursal = bd.Sucursal.Where(p => p.IdSucursal == oArea.IdSucursal).First();
                if (oActivo.IdProveedor != null)
                {
                    Proveedor oProveedor = bd.Proveedor.Where(p => p.IdProveedor == oActivo.IdProveedor).First();
                    oDatosF.ProvDon = oProveedor.Nombre;
                    oDatosF.IsProvDon = 1;
                }
                else
                {
                    Donantes oDonante = bd.Donantes.Where(p => p.IdDonante == oActivo.IdDonante).First();
                    oDatosF.ProvDon = oDonante.Nombre;
                    oDatosF.IsProvDon = 2;
                }
                if (oActivo.IdProveedor == null && oActivo.IdDonante == null)
                {
                    oDatosF.ProvDon = "";
                }

                TarjetaDepreciacion oTarjeta = bd.TarjetaDepreciacion.Where(p => p.IdBien == oActivo.IdBien).Last();

                oDatosF.idBien = (int)oActivo.IdBien;
                oDatosF.descripcion = oActivo.Desripcion;
                oDatosF.fecha = oFOrmulario.FechaIngreso == null ? " " : ((DateTime)oFOrmulario.FechaIngreso).ToString("dd-MM-yyyy");
                oDatosF.codigo = oActivo.CorrelativoBien;
                oDatosF.Clasificacion = oClasificacion.Clasificacion1;
                oDatosF.valorAquisicion = oActivo.ValorAdquicicion.ToString();
                //oDatosF.Respondable = oEmpleado.Nombres + " " + oEmpleado.Apellidos;
                //oDatosF.Ubicacion = oArea.Nombre + " - " + oSucursal.Nombre;
                oDatosF.valorActual = oTarjeta.ValorActual.ToString();
                //oDatosF.NoSerie = oActivo.NoSerie;
                oDatosF.VidaUtil = oActivo.VidaUtil.ToString();
                oDatosF.Observaciones = oFOrmulario.Observaciones;

                oDatosF.foto = oActivo.Foto;
                return oDatosF;
            }

        }

    }
}
