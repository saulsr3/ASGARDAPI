using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.StaticFiles;
using ASGARDAPI.Models;
using ASGARDAPI.Clases;
using System.Drawing;
using Microsoft.AspNetCore.Hosting;
using System.ComponentModel;

namespace ASGARDAPI.Controllers
{
    public class ReportesSeguridadController : Controller
    {
        private readonly IHostingEnvironment env;

        public ReportesSeguridadController(IHostingEnvironment _evn)
        {
            env = _evn;
        }

        public IActionResult Index()
        {
            return View();
        }

        // INICIO DE LISTADO DE USUARIO
        [HttpGet]
        [Route("api/ReportesSeguridad/validarlistarUsuarios")]
        public int listarUsuarios()
        {
            int respuesta = 0;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                List<UsuarioAF> lista = (from usuario in bd.Usuario
                                         join tipousuario in bd.TipoUsuario
                                         on usuario.IdTipoUsuario equals tipousuario.IdTipoUsuario
                                         join empleado in bd.Empleado
                                         on usuario.IdEmpleado equals empleado.IdEmpleado
                                         where usuario.Dhabilitado==1
                                         select new UsuarioAF

                                         {
                                             iidusuario= usuario.IdUsuario,
                                             iidTipousuario= tipousuario.IdTipoUsuario,
                                             nombreusuario= usuario.NombreUsuario,
                                             nombreEmpleado= empleado.Nombres +" "+ empleado.Apellidos,
                                             nombreTipoUsuario= tipousuario.Descripcion,

                                            }).ToList();
                if (lista.Count() > 0)
                {
                    respuesta = 1;
                }
                return respuesta;

            }
        }

        // FIN DE LISTADO DE USUARIO


        //INICIO DE REPORTE DE USUSARIO
        [HttpGet]
        [Route("api/ReportesSeguridad/usuariospdf")]
        public async Task<IActionResult> usuariospdf()
        {
            Document doc = new Document(PageSize.Letter);
            doc.SetMargins(40f, 40f, 40f, 40f);
            MemoryStream ms = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);

            //Instanciamos la clase para el paginado y la fecha de impresión
            var pe = new PageEventHelper();
            writer.PageEvent = pe;

            doc.AddAuthor("Asgard");
            doc.AddTitle("Reporte de usuarios");
            doc.Open();

            //Inicia cuerpo del reporte

            //Estilo y fuente personalizada
            BaseFont fuente = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo = new iTextSharp.text.Font(fuente, 12f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente2 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo2 = new iTextSharp.text.Font(fuente2, 11f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente3 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo3 = new iTextSharp.text.Font(fuente3, 15f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente4 = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo4 = new iTextSharp.text.Font(fuente4, 11f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));

            //Para las celdas
            BaseFont fuente5 = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);
            iTextSharp.text.Font parrafo5 = new iTextSharp.text.Font(fuente5, 10f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));


            //Encabezado
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                CooperativaAF oCooperativaAF = new CooperativaAF();
                Cooperativa oCooperativa = bd.Cooperativa.Where(p => p.Dhabilitado == 1).First();
                oCooperativaAF.idcooperativa = oCooperativa.IdCooperativa;
                oCooperativaAF.nombre = oCooperativa.Nombre;
                oCooperativaAF.descripcion = oCooperativa.Descripcion;

                //Se agrega el encabezado
                var tbl1 = new PdfPTable(new float[] { 11f, 89f }) { WidthPercentage = 100f };
                tbl1.AddCell(new PdfPCell(new Phrase(" ", parrafo2)) { Border = 0, Rowspan = 2 });
                tbl1.AddCell(new PdfPCell(new Phrase(oCooperativa.Descripcion.ToUpper(), parrafo2)) { Border = 0, HorizontalAlignment = 1 });
                tbl1.AddCell(new PdfPCell(new Phrase(oCooperativa.Nombre.ToUpper(), parrafo3)) { Border = 0, HorizontalAlignment = 1 });
                doc.Add(tbl1);
                doc.Add(new Phrase("\n"));
            }
            doc.Add(new Phrase("\n"));
            //Línea separadora
            Chunk linea = new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(1f, 100f, BaseColor.Black, Element.ALIGN_CENTER, 1f));
            doc.Add(linea);
            doc.Add(new Paragraph("REPORTE DE USUARIOS", parrafo) { Alignment = Element.ALIGN_CENTER });

            //Espacio en blanco
            doc.Add(Chunk.Newline);

            //Agregamos una tabla
            var tbl = new PdfPTable(new float[] { 40f, 40f, 40f }) { WidthPercentage = 100f };
            var c1 = new PdfPCell(new Phrase("NOMBRE DE USUARIO", parrafo2));
            var c2 = new PdfPCell(new Phrase("NOMBRE DE EMPLEADO", parrafo2));
            var c3 = new PdfPCell(new Phrase("TIPO DE USUARIO", parrafo2));

            //Agregamos a la tabla las celdas
            tbl.AddCell(c1);
            tbl.AddCell(c2);
            tbl.AddCell(c3);

            //Extraemos de la base y llenamos las celdas
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                List<UsuarioAF> lista = (from usuario in bd.Usuario
                                         join tipousuario in bd.TipoUsuario
                                         on usuario.IdTipoUsuario equals tipousuario.IdTipoUsuario
                                         join empleado in bd.Empleado
                                         on usuario.IdEmpleado equals empleado.IdEmpleado
                                         where usuario.Dhabilitado == 1
                                         select new UsuarioAF

                                         {
                                             iidusuario = usuario.IdUsuario,
                                             iidTipousuario = tipousuario.IdTipoUsuario,
                                             nombreusuario = usuario.NombreUsuario,
                                             nombreEmpleado = empleado.Nombres + " " + empleado.Apellidos,
                                             nombreTipoUsuario = tipousuario.Descripcion,

                                         }).ToList();
                foreach (var usuario in lista)
                {
                    c1.Phrase = new Phrase(usuario.nombreusuario, parrafo5);
                    c2.Phrase = new Phrase(usuario.nombreEmpleado, parrafo5);
                    c3.Phrase = new Phrase(usuario.nombreTipoUsuario, parrafo5);



                    //Agregamos a la tabla
                    tbl.AddCell(c1);
                    tbl.AddCell(c2);
                    tbl.AddCell(c3);


                }

                //INICIO DE ADICIÓN DE LOGO
                CooperativaAF oCooperativaAF = new CooperativaAF();

                Cooperativa oCooperativa = bd.Cooperativa.Where(p => p.Dhabilitado == 1).First();
                oCooperativaAF.idcooperativa = oCooperativa.IdCooperativa;


                try
                {
                    iTextSharp.text.Image logo = null;
                    logo = iTextSharp.text.Image.GetInstance(oCooperativa.Logo.ToString());
                    logo.Alignment = iTextSharp.text.Image.ALIGN_LEFT;
                    logo.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    logo.BorderColor = iTextSharp.text.BaseColor.White;
                    logo.ScaleToFit(170f, 100f);

                    float ancho = logo.Width;
                    float alto = logo.Height;
                    float proporcion = alto / ancho;

                    logo.ScaleAbsoluteWidth(80);
                    logo.ScaleAbsoluteHeight(80 * proporcion);

                    logo.SetAbsolutePosition(40f, 695f);

                    doc.Add(logo);

                }
                catch (DocumentException dex)
                {
                    //log exception here
                }

                //FIN DE ADICIÓN DE LOGO



            }
            doc.Add(tbl);
            writer.Close();
            doc.Close();
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "application/pdf");
        }

        //FIN DE REPORTE DE USUARIO


        //LISTAR COMBO DE CLASIFICACIONES CON ID
  
        [HttpGet]
        [Route("api/ReportesSeguridad/comboClasificaciones")]
        public IEnumerable<ClasificacionAF> comboClasificaciones()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<ClasificacionAF> lista = (from clasificacion in bd.Clasificacion
                                                 where clasificacion.Dhabilitado == 1 //&& clasificacion.IdClasificacion == idclasificacion
                                                 //orderby empleado.Nombres
                                                 select new ClasificacionAF
                                                 {
                                                     idclasificacion= clasificacion.IdClasificacion,
                                                     clasificacion= clasificacion.Clasificacion1,

                                                 }).ToList();
                return lista;
            }
        }

        [HttpGet]
        [Route("api/ReportesSeguridad/validarcomboClasificaciones/{idclasificacion}")]
        public int validarcomboClasificaciones(int idclasificacion)
        {
            int respuesta = 0;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                
                Clasificacion oClasificacion = bd.Clasificacion.Where(p => p.IdClasificacion == idclasificacion).First();
                IEnumerable<RegistroAF> lista = (from activo in bd.ActivoFijo
                                                      join noFormulario in bd.FormularioIngreso
                                                      on activo.NoFormulario equals noFormulario.NoFormulario
                                                      join clasif in bd.Clasificacion
                                                      on activo.IdClasificacion equals clasif.IdClasificacion

                                                      where activo.EstaAsignado == 1 && clasif.IdClasificacion == idclasificacion && (activo.EstadoActual != 0)

                                                      orderby activo.CorrelativoBien
                                                      select new RegistroAF
                                                      {
                                                          IdBien = activo.IdBien,
                                                          Codigo = activo.CorrelativoBien,
                                                          fechacadena = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                                          Descripcion = activo.Desripcion,
                                                          Clasificacion = clasif.Clasificacion1,
                                                          valoradquicicion = activo.ValorAdquicicion.ToString(),
                                                      }).ToList();
                if (lista.Count() > 0)
                {
                    respuesta = 1;
                }
                return respuesta;
            }
        }

        //INICIO DE REPORTE DE ACTIVOS SEGÚN SU CLASIFICACIÓN
        [HttpGet]
        [Route("api/ReportesSeguridad/activosclasificacionpdf/{idclasificacion}")]
        public async Task<IActionResult> activosclasificacionpdf(int idclasificacion)
        {
            Document doc = new Document(PageSize.Letter);
            doc.SetMargins(40f, 40f, 40f, 40f);
            MemoryStream ms = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);

            //Instanciamos la clase para el paginado y la fecha de impresión
            var pe = new PageEventHelper();
            writer.PageEvent = pe;

            doc.AddAuthor("Asgard");
            doc.AddTitle("Reporte activos por clasficiación");
            doc.Open();

            //Inicia cuerpo del reporte

            //Estilo y fuente personalizada
            BaseFont fuente = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo = new iTextSharp.text.Font(fuente, 12f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente2 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo2 = new iTextSharp.text.Font(fuente2, 11f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente3 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo3 = new iTextSharp.text.Font(fuente3, 15f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente4 = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo4 = new iTextSharp.text.Font(fuente4, 11f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));

            //Para las celdas
            BaseFont fuente5 = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);
            iTextSharp.text.Font parrafo5 = new iTextSharp.text.Font(fuente5, 10f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));

            //Fuente para tarjeta
            BaseFont fuente6 = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo6 = new iTextSharp.text.Font(fuente2, 9f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente7 = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo7 = new iTextSharp.text.Font(fuente, 9f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));

            BaseFont fuente8 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo8 = new iTextSharp.text.Font(fuente2, 9f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));


            //Encabezado
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                CooperativaAF oCooperativaAF = new CooperativaAF();
                Cooperativa oCooperativa = bd.Cooperativa.Where(p => p.Dhabilitado == 1).First();
                oCooperativaAF.idcooperativa = oCooperativa.IdCooperativa;
                oCooperativaAF.nombre = oCooperativa.Nombre;
                oCooperativaAF.descripcion = oCooperativa.Descripcion;

                //Se agrega el encabezado
                var tbl1 = new PdfPTable(new float[] { 11f, 89f }) { WidthPercentage = 100f };
                tbl1.AddCell(new PdfPCell(new Phrase(" ", parrafo2)) { Border = 0, Rowspan = 2 });
                tbl1.AddCell(new PdfPCell(new Phrase(oCooperativa.Descripcion.ToUpper(), parrafo2)) { Border = 0, HorizontalAlignment = 1 });
                tbl1.AddCell(new PdfPCell(new Phrase(oCooperativa.Nombre.ToUpper(), parrafo3)) { Border = 0, HorizontalAlignment = 1 });
                doc.Add(tbl1);
                doc.Add(new Phrase("\n"));
            }
            doc.Add(new Phrase("\n"));
            //Línea separadora
            Chunk linea = new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(1f, 100f, BaseColor.Black, Element.ALIGN_CENTER, 1f));
            doc.Add(linea);
            doc.Add(new Paragraph("REPORTE ACTIVOS POR CLASIFICACIÓN ", parrafo) { Alignment = Element.ALIGN_CENTER });

            //Espacio en blanco
            doc.Add(Chunk.Newline);

            //Extraemos de la base y llenamos las celdas
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                ClasificacionAF odatos = new ClasificacionAF();
                //ActivoFijo oActivo = bd.ActivoFijo.Where(p => p.IdBien == idbien).First();
                Clasificacion oClasificacion = bd.Clasificacion.Where(p => p.IdClasificacion == idclasificacion).First();
                Categorias oCategoria = bd.Categorias.Where(p => p.IdCategoria == oClasificacion.IdCategoria).First();

                odatos.correlativo = oClasificacion.Correlativo;
                odatos.clasificacion = oClasificacion.Clasificacion1;
                odatos.categoria = oCategoria.Categoria;
                odatos.descripcion = oClasificacion.Descripcion;

                


                //Cuerpo de la tarjeta
                var tbl1 = new PdfPTable(new float[] { 3f, 7f, 3f, 7f }) { WidthPercentage = 100f };
                tbl1.AddCell(new PdfPCell(new Phrase("Correlativo: ", parrafo6)) { Border = 0, Rowspan = 2 });
                tbl1.AddCell(new PdfPCell(new Phrase(odatos.correlativo, parrafo7)) { Border = 0 });
                tbl1.AddCell(new PdfPCell(new Phrase("Clasificación: ", parrafo6)) { Border = 0, Rowspan = 2 });
                tbl1.AddCell(new PdfPCell(new Phrase(odatos.clasificacion, parrafo7)) { Border = 0 });


                var tbl2 = new PdfPTable(new float[] { 3f, 7f, 3f, 7f }) { WidthPercentage = 100f };
                tbl2.AddCell(new PdfPCell(new Phrase("Categoría: ", parrafo6)) { Border = 0, Rowspan = 2 });
                tbl2.AddCell(new PdfPCell(new Phrase(odatos.categoria, parrafo7)) { Border = 0 });
                tbl2.AddCell(new PdfPCell(new Phrase("Descripción: ", parrafo6)) { Border = 0, Rowspan = 2 });
                tbl2.AddCell(new PdfPCell(new Phrase(odatos.descripcion, parrafo7)) { Border = 0 });



                doc.Add(tbl1);
                doc.Add(Chunk.Newline);
                doc.Add(tbl2);
                doc.Add(Chunk.Newline);


                //Tabla de transacciones
                // doc.Add(new Paragraph("TABLA HISTORIAL DE TRASPASOS", parrafo2) { Alignment = Element.ALIGN_CENTER });


                //Agregamos una tabla
                //Agregamos una tabla
                var tbl = new PdfPTable(new float[] { 17f, 15f, 25f , 25f}) { WidthPercentage = 100f };
                var c1 = new PdfPCell(new Phrase("CÓDIGO", parrafo2));
                var c2 = new PdfPCell(new Phrase("FECHA INGRESO", parrafo2));
                var c3 = new PdfPCell(new Phrase("DESCRIPCIÓN", parrafo2));
                var c4 = new PdfPCell(new Phrase("VALOR DE ADQUICISIÓN", parrafo2));
                //var c5 = new PdfPCell(new Phrase("RESPONSABLE", parrafo2));
                //Agregamos a la tabla las celdas
                tbl.AddCell(c1);
                tbl.AddCell(c2);
                tbl.AddCell(c3);
                tbl.AddCell(c4);
                //tbl.AddCell(c5);


                List<RegistroAF> lista = (from activo in bd.ActivoFijo
                                          join noFormulario in bd.FormularioIngreso
                                          on activo.NoFormulario equals noFormulario.NoFormulario
                                          join clasif in bd.Clasificacion
                                          on activo.IdClasificacion equals clasif.IdClasificacion
                                         
                                          where activo.EstaAsignado == 1 && clasif.IdClasificacion ==  idclasificacion && (activo.EstadoActual != 0) 

                                          orderby activo.CorrelativoBien
                                          select new RegistroAF
                                          {
                                              IdBien = activo.IdBien,
                                              Codigo = activo.CorrelativoBien,
                                              fechacadena = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                              Descripcion = activo.Desripcion,
                                              Clasificacion = clasif.Clasificacion1,
                                              valoradquicicion= activo.ValorAdquicicion.ToString(),
                                          }).ToList();

                foreach (var activoasignado in lista)
                {

                    c1.Phrase = new Phrase(activoasignado.Codigo, parrafo5);
                    c2.Phrase = new Phrase(activoasignado.fechacadena, parrafo5);
                    c3.Phrase = new Phrase(activoasignado.Descripcion, parrafo5);
                    c4.Phrase = new Phrase("$"+ activoasignado.valoradquicicion, parrafo5);
                  //  c5.Phrase = new Phrase(activoasignado.Responsable, parrafo5);
                    //Agregamos a la tabla
                    tbl.AddCell(c1);
                    tbl.AddCell(c2);
                    tbl.AddCell(c3);
                    tbl.AddCell(c4);
                  //  tbl.AddCell(c5);


                }
                doc.Add(tbl);
                //INICIO DE ADICIÓN DE LOGO
                CooperativaAF oCooperativaAF = new CooperativaAF();

                Cooperativa oCooperativa = bd.Cooperativa.Where(p => p.Dhabilitado == 1).First();
                oCooperativaAF.idcooperativa = oCooperativa.IdCooperativa;


                try
                {
                    iTextSharp.text.Image logo = null;
                    logo = iTextSharp.text.Image.GetInstance(oCooperativa.Logo.ToString());
                    logo.Alignment = iTextSharp.text.Image.ALIGN_LEFT;
                    logo.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    logo.BorderColor = iTextSharp.text.BaseColor.White;
                    logo.ScaleToFit(170f, 100f);

                    float ancho = logo.Width;
                    float alto = logo.Height;
                    float proporcion = alto / ancho;

                    logo.ScaleAbsoluteWidth(80);
                    logo.ScaleAbsoluteHeight(80 * proporcion);

                    logo.SetAbsolutePosition(40f, 695f);

                    doc.Add(logo);

                }
                catch (DocumentException dex)
                {
                    //log exception here
                }

                //FIN DE ADICIÓN DE LOGO

            }
            writer.Close();
            doc.Close();
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "application/pdf");
        }

        //FIN DE REPORTES DE ACTIVOS SEGÚN SU CLASIFICACIÓN


        //LISTAR MARCAS 
        [HttpGet]
        [Route("api/ReportesSeguridad/comboMarcas")]
        public IEnumerable<MarcasAF> comboMarcas()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<MarcasAF> lista = (from marca in bd.Marcas
                                                      where marca.Dhabilitado == 1 
                                                      select new MarcasAF
                                                      {
                                                          IdMarca = marca.IdMarca,
                                                          Marca = marca.Marca,

                                                      }).ToList();
                return lista;
            }
        }
        //VALIDAR COMOBO MARCAS
        [HttpGet]
        [Route("api/ReportesSeguridad/validarcomboMarcas/{idmarca}")]
        public int validarcomboMarcas(int idmarca)
        {
            int respuesta = 0;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                Marcas oMarca = bd.Marcas.Where(p => p.IdMarca == idmarca).First();
                IEnumerable<RegistroAF> lista = (from activo in bd.ActivoFijo
                                                 join noFormulario in bd.FormularioIngreso
                                                 on activo.NoFormulario equals noFormulario.NoFormulario
                                                 join marca in bd.Marcas
                                                 on activo.IdMarca equals marca.IdMarca
                                                 where activo.EstaAsignado == 1 && marca.IdMarca == idmarca && (activo.EstadoActual != 0)
                                                 select new RegistroAF
                                               {
                                                   IdBien = activo.IdBien,
                                                   Codigo = activo.CorrelativoBien,
                                                   fechacadena = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                                   Descripcion = activo.Desripcion,
                                                   valoradquicicion = activo.ValorAdquicicion.ToString(),


                                               }).ToList();
                if (lista.Count() > 0)
                {
                    respuesta = 1;
                }
                return respuesta;
            }
        }

        //INICIO DE REPORTE DE ACTIVOS POR MARCA
        [HttpGet]
        [Route("api/ReportesSeguridad/activospormarcapdf/{idmarca}")]
        public async Task<IActionResult> activospormarcapdf(int idmarca)
        {
            Document doc = new Document(PageSize.Letter);
            doc.SetMargins(40f, 40f, 40f, 40f);
            MemoryStream ms = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);

            //Instanciamos la clase para el paginado y la fecha de impresión
            var pe = new PageEventHelper();
            writer.PageEvent = pe;

            doc.AddAuthor("Asgard");
            doc.AddTitle("Reporte activos por marca");
            doc.Open();

            //Inicia cuerpo del reporte

            //Estilo y fuente personalizada
            BaseFont fuente = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo = new iTextSharp.text.Font(fuente, 12f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente2 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo2 = new iTextSharp.text.Font(fuente2, 11f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente3 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo3 = new iTextSharp.text.Font(fuente3, 15f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente4 = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo4 = new iTextSharp.text.Font(fuente4, 11f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));

            //Para las celdas
            BaseFont fuente5 = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);
            iTextSharp.text.Font parrafo5 = new iTextSharp.text.Font(fuente5, 10f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));

            //Fuente para tarjeta
            BaseFont fuente6 = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo6 = new iTextSharp.text.Font(fuente2, 9f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente7 = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo7 = new iTextSharp.text.Font(fuente, 9f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));

            BaseFont fuente8 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo8 = new iTextSharp.text.Font(fuente2, 9f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));


            //Encabezado
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                CooperativaAF oCooperativaAF = new CooperativaAF();
                Cooperativa oCooperativa = bd.Cooperativa.Where(p => p.Dhabilitado == 1).First();
                oCooperativaAF.idcooperativa = oCooperativa.IdCooperativa;
                oCooperativaAF.nombre = oCooperativa.Nombre;
                oCooperativaAF.descripcion = oCooperativa.Descripcion;

                //Se agrega el encabezado
                var tbl1 = new PdfPTable(new float[] { 11f, 89f }) { WidthPercentage = 100f };
                tbl1.AddCell(new PdfPCell(new Phrase(" ", parrafo2)) { Border = 0, Rowspan = 2 });
                tbl1.AddCell(new PdfPCell(new Phrase(oCooperativa.Descripcion.ToUpper(), parrafo2)) { Border = 0, HorizontalAlignment = 1 });
                tbl1.AddCell(new PdfPCell(new Phrase(oCooperativa.Nombre.ToUpper(), parrafo3)) { Border = 0, HorizontalAlignment = 1 });
                doc.Add(tbl1);
                doc.Add(new Phrase("\n"));
            }
            doc.Add(new Phrase("\n"));
            //Línea separadora
            Chunk linea = new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(1f, 100f, BaseColor.Black, Element.ALIGN_CENTER, 1f));
            doc.Add(linea);
            doc.Add(new Paragraph("REPORTE DE ACTIVOS POR MARCA ", parrafo) { Alignment = Element.ALIGN_CENTER });

            //Espacio en blanco
            doc.Add(Chunk.Newline);

            //Extraemos de la base y llenamos las celdas
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                MarcasAF odatos = new MarcasAF();
                //ActivoFijo oActivo = bd.ActivoFijo.Where(p => p.IdBien == idbien).First();
                Marcas oMarca = bd.Marcas.Where(p => p.IdMarca == idmarca).First();
                //Categorias oCategoria = bd.Categorias.Where(p => p.IdCategoria == oClasificacion.IdCategoria).First();

                odatos.Marca = oMarca.Marca;
                odatos.Descripcion = oMarca.Descripcion;
                //Cuerpo de la tarjeta
                var tbl1 = new PdfPTable(new float[] { 3f, 7f, 3f, 7f }) { WidthPercentage = 80f };
                tbl1.AddCell(new PdfPCell(new Phrase("Marca: ", parrafo6)) { Border = 0, Rowspan = 2 });
                tbl1.AddCell(new PdfPCell(new Phrase(odatos.Marca, parrafo7)) { Border = 0 });
                tbl1.AddCell(new PdfPCell(new Phrase("Descripción: ", parrafo6)) { Border = 0, Rowspan = 2 });
                tbl1.AddCell(new PdfPCell(new Phrase(odatos.Descripcion, parrafo7)) { Border = 0 });
                doc.Add(tbl1);
                doc.Add(Chunk.Newline);

                //Agregamos una tabla
                var tbl = new PdfPTable(new float[] { 17f, 15f, 25f, 25f }) { WidthPercentage = 100f };
                var c1 = new PdfPCell(new Phrase("CÓDIGO", parrafo2));
                var c2 = new PdfPCell(new Phrase("FECHA INGRESO", parrafo2));
                var c3 = new PdfPCell(new Phrase("DESCRIPCIÓN", parrafo2));
                var c4 = new PdfPCell(new Phrase("VALOR DE ADQUICISIÓN", parrafo2));
                //var c5 = new PdfPCell(new Phrase("RESPONSABLE", parrafo2));
                //Agregamos a la tabla las celdas
                tbl.AddCell(c1);
                tbl.AddCell(c2);
                tbl.AddCell(c3);
                tbl.AddCell(c4);
                //tbl.AddCell(c5);


                List<RegistroAF> lista = (from activo in bd.ActivoFijo
                                          join noFormulario in bd.FormularioIngreso
                                          on activo.NoFormulario equals noFormulario.NoFormulario
                                          join marca in bd.Marcas
                                          on activo.IdMarca equals marca.IdMarca

                                          where activo.EstaAsignado == 1 && marca.IdMarca == idmarca && (activo.EstadoActual != 0)

                                          orderby activo.CorrelativoBien
                                          select new RegistroAF
                                          {
                                              IdBien = activo.IdBien,
                                              Codigo = activo.CorrelativoBien,
                                              fechacadena = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                              Descripcion = activo.Desripcion,
                                              valoradquicicion = activo.ValorAdquicicion.ToString(),
                                          }).ToList();

                foreach (var activoasignado in lista)
                {

                    c1.Phrase = new Phrase(activoasignado.Codigo, parrafo5);
                    c2.Phrase = new Phrase(activoasignado.fechacadena, parrafo5);
                    c3.Phrase = new Phrase(activoasignado.Descripcion, parrafo5);
                    c4.Phrase = new Phrase("$" + activoasignado.valoradquicicion, parrafo5);
                    //  c5.Phrase = new Phrase(activoasignado.Responsable, parrafo5);
                    //Agregamos a la tabla
                    tbl.AddCell(c1);
                    tbl.AddCell(c2);
                    tbl.AddCell(c3);
                    tbl.AddCell(c4);
                    //  tbl.AddCell(c5);


                }
                doc.Add(tbl);
                //INICIO DE ADICIÓN DE LOGO
                CooperativaAF oCooperativaAF = new CooperativaAF();

                Cooperativa oCooperativa = bd.Cooperativa.Where(p => p.Dhabilitado == 1).First();
                oCooperativaAF.idcooperativa = oCooperativa.IdCooperativa;


                try
                {
                    iTextSharp.text.Image logo = null;
                    logo = iTextSharp.text.Image.GetInstance(oCooperativa.Logo.ToString());
                    logo.Alignment = iTextSharp.text.Image.ALIGN_LEFT;
                    logo.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    logo.BorderColor = iTextSharp.text.BaseColor.White;
                    logo.ScaleToFit(170f, 100f);

                    float ancho = logo.Width;
                    float alto = logo.Height;
                    float proporcion = alto / ancho;

                    logo.ScaleAbsoluteWidth(80);
                    logo.ScaleAbsoluteHeight(80 * proporcion);

                    logo.SetAbsolutePosition(40f, 695f);

                    doc.Add(logo);

                }
                catch (DocumentException dex)
                {
                    //log exception here
                }

                //FIN DE ADICIÓN DE LOGO

            }
            writer.Close();
            doc.Close();
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "application/pdf");
        }

        // FIN DE REPORTE DE ACTIVOS POR MARCA

        //INICIO DE REPORTE DE PROVISIÓN ANUAL
        [HttpGet]
        [Route("api/ReportesSeguridad/provisionAnualPdf/{anio}")]
        public async Task<IActionResult> provisionAnualPdf(int anio)
        {
            Document doc = new Document(PageSize.Letter);
            doc.SetMargins(40f, 40f, 40f, 40f);
            MemoryStream ms = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);

            //Instanciamos la clase para el paginado y la fecha de impresión
            var pe = new PageEventHelper();
            writer.PageEvent = pe;

            doc.AddAuthor("Asgard");
            doc.AddTitle("Reporte de provisión de activos anual");
            doc.Open();

            //Inicia cuerpo del reporte

            //Estilo y fuente personalizada
            BaseFont fuente = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1252, true);
            iTextSharp.text.Font parrafo = new iTextSharp.text.Font(fuente, 12f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente2 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo2 = new iTextSharp.text.Font(fuente2, 10f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente3 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo3 = new iTextSharp.text.Font(fuente3, 15f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente4 = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo4 = new iTextSharp.text.Font(fuente4, 11f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));

            //Para las celdas
            BaseFont fuente5 = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);
            iTextSharp.text.Font parrafo5 = new iTextSharp.text.Font(fuente5, 9f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente6 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo6 = new iTextSharp.text.Font(fuente2, 8f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));

            //Encabezado
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                CooperativaAF oCooperativaAF = new CooperativaAF();
                Cooperativa oCooperativa = bd.Cooperativa.Where(p => p.Dhabilitado == 1).First();
                oCooperativaAF.idcooperativa = oCooperativa.IdCooperativa;
                oCooperativaAF.nombre = oCooperativa.Nombre;
                oCooperativaAF.descripcion = oCooperativa.Descripcion;

                //Se agrega el encabezado
                var tbl1 = new PdfPTable(new float[] { 11f, 89f }) { WidthPercentage = 100f };
                tbl1.AddCell(new PdfPCell(new Phrase(" ", parrafo2)) { Border = 0, Rowspan = 2 });
                tbl1.AddCell(new PdfPCell(new Phrase(oCooperativa.Descripcion.ToUpper(), parrafo2)) { Border = 0, HorizontalAlignment = 1 });
                tbl1.AddCell(new PdfPCell(new Phrase(oCooperativa.Nombre.ToUpper(), parrafo3)) { Border = 0, HorizontalAlignment = 1 });
                doc.Add(tbl1);
                doc.Add(new Phrase("\n"));

            }
            doc.Add(new Phrase("\n"));
          
            //Línea separadora
            Chunk linea = new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(1f, 100f, BaseColor.Black, Element.ALIGN_CENTER, 1f));
            doc.Add(linea);
            doc.Add(new Paragraph("REPORTE DE PROVISIÓN DE ACTIVOS ANUAL", parrafo) { Alignment = Element.ALIGN_CENTER });

            //Espacio en blanco
            doc.Add(Chunk.Newline);

            //Agregamos una tabla
            var tbl = new PdfPTable(new float[] { 8f, 9f, 13f, 10f, 9f, 10f, 10f }) { WidthPercentage = 100f };
            var c1 = new PdfPCell(new Phrase("FECHA", parrafo6));
            var c2 = new PdfPCell(new Phrase("CONCEPTO", parrafo6));
            var c3 = new PdfPCell(new Phrase("CÓDIGO", parrafo6));
            var c4 = new PdfPCell(new Phrase("VALOR DE ADQUISICIÓN", parrafo6));
            var c5 = new PdfPCell(new Phrase("VALOR DE MEJORA", parrafo6));
            var c6 = new PdfPCell(new Phrase("DEPRECIACIÓN ANUAL", parrafo6));
          //  var c7 = new PdfPCell(new Phrase("DEPRECIACIÓN ACUMULADA", parrafo6));
            var c8 = new PdfPCell(new Phrase("VALOR ACTUAL", parrafo6));
            //Agregamos a la tabla las celdas 
            tbl.AddCell(c1);
            tbl.AddCell(c2);
            tbl.AddCell(c3);
            tbl.AddCell(c4);
            tbl.AddCell(c5);
            tbl.AddCell(c6);
         //   tbl.AddCell(c7);
            tbl.AddCell(c8);

         //  doc.Add(new Phrase("\n"));

           



            //Extraemos de la base y llenamos las celdas
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                string fechaMin = "1-1-" + anio;
                string fechaMax = "31-12-" + anio;

                double valortotal =0;
                //  DateTime oDate = Convert.ToDateTime(fechaMax);
                DateTime uDate = DateTime.ParseExact(fechaMax, "dd-MM-yyyy", null);
                // oDate.ToString("dd-MM-yyyy");

                List<ActivoRevalorizadoAF> lista = (from activo in bd.ActivoFijo
                                                    join noFormulario in bd.FormularioIngreso
                                                    on activo.NoFormulario equals noFormulario.NoFormulario
                                                    join tarjeta in bd.TarjetaDepreciacion
                                                    on activo.IdBien equals tarjeta.IdBien
                                                    where (tarjeta.Fecha >= DateTime.Parse(fechaMin) && tarjeta.Fecha <= uDate)
                                                    && tarjeta.Concepto == "Depreciación"
                                                    orderby activo.IdBien
                                                    select new ActivoRevalorizadoAF
                                                    {
                                                        idBien = activo.IdBien,
                                                        //Código
                                                        codigo = activo.CorrelativoBien,
                                                        //Fecha
                                                        fecha = tarjeta.Fecha == null ? " " : ((DateTime)tarjeta.Fecha).ToString("dd-MM-yyyy"),
                                                        //Concepto
                                                        concepto = tarjeta.Concepto,
                                                        //Valor adquirido
                                                        valorAdquirido = activo.ValorAdquicicion.ToString(),
                                                        //Valor de mejora
                                                        montoTransaccion = Math.Round((double)tarjeta.Valor, 2),
                                                        //Depreciación anual
                                                        depreAnual = Math.Round((double)tarjeta.DepreciacionAnual, 2),
                                                        //Depreciación Acumulada
                                                       // depreAcum = Math.Round((double)tarjeta.DepreciacionAcumulada, 2),
                                                        

                                                        //     valorTransaccion = Math.Round((double)tarjeta.ValorTransaccion, 2),
                                                        //Valor actual
                                                        valorActual = Math.Round((double)tarjeta.ValorActual, 2)

                                                    }).ToList();

                foreach (var activos in lista)
                {
                    c1.Phrase = new Phrase(activos.fecha, parrafo5);
                    c2.Phrase = new Phrase(activos.concepto, parrafo5);
                    c3.Phrase = new Phrase(activos.codigo, parrafo5);
                    c4.Phrase = new Phrase("$" + activos.valorAdquirido, parrafo5);
                    c5.Phrase = new Phrase("$" + activos.montoTransaccion, parrafo5);
                    c6.Phrase = new Phrase("$" + activos.depreAnual, parrafo5);
                   // c7.Phrase = new Phrase("$" + activos.depreAcum, parrafo5);
                    c8.Phrase = new Phrase("$" + activos.valorActual, parrafo5);
                    //Agregamos a la tabla
                    tbl.AddCell(c1);
                    tbl.AddCell(c2);
                    tbl.AddCell(c3);
                    tbl.AddCell(c4);
                    tbl.AddCell(c5);
                    tbl.AddCell(c6);
                   // tbl.AddCell(c7);
                    tbl.AddCell(c8);
                 

                }
                

                //AGREGAMOS NUEVA TABLA
                //Espacio en blanco
                doc.Add(Chunk.Newline);

                //Agregamos una tabla
                var tbl11 = new PdfPTable(new float[] { 50f,50f}) { WidthPercentage = 100f };
                var c9 = new PdfPCell(new Phrase("TOTAL"));
               // var c10 = new PdfPCell(new Phrase("",parrafo6));
              
                //Agregamos a la tabla las celdas 
                tbl11.AddCell(c9);
               // tbl11.AddCell(c10);
       

                doc.Add(new Phrase("\n"));

                //PARA LA NUEVA TABLA


                
                ActivoRevalorizadoAF activototal = new ActivoRevalorizadoAF();

                    List<ActivoRevalorizadoAF> lista1 = (from activo in bd.ActivoFijo
                                                         join noFormulario in bd.FormularioIngreso
                                                         on activo.NoFormulario equals noFormulario.NoFormulario
                                                         join tarjeta in bd.TarjetaDepreciacion
                                                         on activo.IdBien equals tarjeta.IdBien
                                                         where (tarjeta.Fecha >= DateTime.Parse(fechaMin) && tarjeta.Fecha <= uDate)
                                                         && tarjeta.Concepto == "Depreciación"
                                                         orderby activo.IdBien
                                                         select new ActivoRevalorizadoAF
                                                         {
                                                             idBien = activo.IdBien,                                
                                         
                                                             montoTransaccion = Math.Round((double)tarjeta.Valor, 2),
                                                             //Depreciación anual
                                                             depreAnual = Math.Round((double)tarjeta.DepreciacionAnual, 2),
                                                             //Depreciación Acumulada
                                                             depreAcum = Math.Round((double)tarjeta.DepreciacionAcumulada, 2),

                                                                                                                
                                                               }).ToList();  

                foreach (var activos in lista1)
                {

                  //  c9.Phrase = new Phrase("$" + activos.depreAnual, parrafo5);
                   // c9.Phrase = new Phrase("$" + activos.depreAnual, parrafo5);

                    valortotal = valortotal + activos.depreAnual;

                    // tbl11.AddCell(activos.total.ToString());
                    // tbl11.AddCell(c9);
                }
                tbl11.AddCell("$" + valortotal.ToString());

                //INICIO DE ADICIÓN DE LOGO
                CooperativaAF oCooperativaAF = new CooperativaAF();

                    Cooperativa oCooperativa = bd.Cooperativa.Where(p => p.Dhabilitado == 1).First();
                    oCooperativaAF.idcooperativa = oCooperativa.IdCooperativa;


                    try
                    {
                        iTextSharp.text.Image logo = null;
                        logo = iTextSharp.text.Image.GetInstance(oCooperativa.Logo.ToString());
                        logo.Alignment = iTextSharp.text.Image.ALIGN_LEFT;
                        logo.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        logo.BorderColor = iTextSharp.text.BaseColor.White;
                        logo.ScaleToFit(170f, 100f);

                        float ancho = logo.Width;
                        float alto = logo.Height;
                        float proporcion = alto / ancho;

                        logo.ScaleAbsoluteWidth(80);
                        logo.ScaleAbsoluteHeight(80 * proporcion);

                        logo.SetAbsolutePosition(40f, 695f);

                        doc.Add(logo);

                    }
                    catch (DocumentException dex)
                    {
                        //log exception here
                    }

                    //FIN DE ADICIÓN DE LOGO

                
                doc.Add(tbl);
                doc.Add(tbl11);
                writer.Close();
                doc.Close();
                ms.Seek(0, SeekOrigin.Begin);
                return File(ms, "application/pdf");
            }
        }

        //FIN DE REPORTE DE PROVISIÓN ANUAL

        //INICIO DE REPORTE DE PROVISIÓN MENSUAL
        [HttpGet]
        [Route("api/ReportesSeguridad/provisionMensualPdf/{mes}/{anio}")]
        public async Task<IActionResult> provisionMensualPdf(int mes,int anio)
        {
            Document doc = new Document(PageSize.Letter);
            doc.SetMargins(40f, 40f, 40f, 40f);
            MemoryStream ms = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);

            //Instanciamos la clase para el paginado y la fecha de impresión
            var pe = new PageEventHelper();
            writer.PageEvent = pe;

            doc.AddAuthor("Asgard");
            doc.AddTitle("Reporte de provisión mensual de activos");
            doc.Open();

            //Inicia cuerpo del reporte

            //Estilo y fuente personalizada
            BaseFont fuente = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1252, true);
            iTextSharp.text.Font parrafo = new iTextSharp.text.Font(fuente, 12f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente2 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo2 = new iTextSharp.text.Font(fuente2, 10f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente3 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo3 = new iTextSharp.text.Font(fuente3, 15f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente4 = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo4 = new iTextSharp.text.Font(fuente4, 11f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));

            //Para las celdas
            BaseFont fuente5 = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);
            iTextSharp.text.Font parrafo5 = new iTextSharp.text.Font(fuente5, 9f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente6 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo6 = new iTextSharp.text.Font(fuente2, 8f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));

            //Encabezado
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                CooperativaAF oCooperativaAF = new CooperativaAF();
                Cooperativa oCooperativa = bd.Cooperativa.Where(p => p.Dhabilitado == 1).First();
                oCooperativaAF.idcooperativa = oCooperativa.IdCooperativa;
                oCooperativaAF.nombre = oCooperativa.Nombre;
                oCooperativaAF.descripcion = oCooperativa.Descripcion;

                //Se agrega el encabezado
                var tbl1 = new PdfPTable(new float[] { 11f, 89f }) { WidthPercentage = 100f };
                tbl1.AddCell(new PdfPCell(new Phrase(" ", parrafo2)) { Border = 0, Rowspan = 2 });
                tbl1.AddCell(new PdfPCell(new Phrase(oCooperativa.Descripcion.ToUpper(), parrafo2)) { Border = 0, HorizontalAlignment = 1 });
                tbl1.AddCell(new PdfPCell(new Phrase(oCooperativa.Nombre.ToUpper(), parrafo3)) { Border = 0, HorizontalAlignment = 1 });
                doc.Add(tbl1);
                doc.Add(new Phrase("\n"));

            }
            doc.Add(new Phrase("\n"));

            //Línea separadora
            Chunk linea = new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(1f, 100f, BaseColor.Black, Element.ALIGN_CENTER, 1f));
            doc.Add(linea);
            if (mes==01)
            {
                doc.Add(new Paragraph("REPORTE DE PROVISIÓN AL MES DE ENERO", parrafo) { Alignment = Element.ALIGN_CENTER });

            }
            if (mes == 02)
            {
                doc.Add(new Paragraph("REPORTE DE PROVISIÓN AL MES DE FEBRERO", parrafo) { Alignment = Element.ALIGN_CENTER });

            }
            if (mes == 03)
            {
                doc.Add(new Paragraph("REPORTE DE PROVISIÓN AL MES DE MARZO", parrafo) { Alignment = Element.ALIGN_CENTER });

            }
            if (mes == 04)
            {
                doc.Add(new Paragraph("REPORTE DE PROVISIÓN AL MES DE ABRIL", parrafo) { Alignment = Element.ALIGN_CENTER });

            }
            if (mes == 05)
            {
                doc.Add(new Paragraph("REPORTE DE PROVISIÓN AL MES DE MAYO", parrafo) { Alignment = Element.ALIGN_CENTER });

            }
            if (mes == 06)
            {
                doc.Add(new Paragraph("REPORTE DE PROVISIÓN AL MES DE JUNIO", parrafo) { Alignment = Element.ALIGN_CENTER });

            }
            if (mes == 07)
            {
                doc.Add(new Paragraph("REPORTE DE PROVISIÓN AL MES DE JULIO", parrafo) { Alignment = Element.ALIGN_CENTER });

            }
            if (mes == 08)
            {
                doc.Add(new Paragraph("REPORTE DE PROVISIÓN AL MES DE AGOSTO", parrafo) { Alignment = Element.ALIGN_CENTER });

            }
            if (mes == 09)
            {
                doc.Add(new Paragraph("REPORTE DE PROVISIÓN AL MES DE SEPTIEMBRE", parrafo) { Alignment = Element.ALIGN_CENTER });

            }
            if (mes == 10)
            {
                doc.Add(new Paragraph("REPORTE DE PROVISIÓN AL MES DE OCTUBRE", parrafo) { Alignment = Element.ALIGN_CENTER });

            }
            if (mes == 11)
            {
                doc.Add(new Paragraph("REPORTE DE PROVISIÓN AL MES DE NOVIEMBRE", parrafo) { Alignment = Element.ALIGN_CENTER });

            }
            if (mes == 12)
            {
                doc.Add(new Paragraph("REPORTE DE PROVISIÓN AL MES DE DICIEMBRE", parrafo) { Alignment = Element.ALIGN_CENTER });

            }




            //Espacio en blanco
            doc.Add(Chunk.Newline);

            //Agregamos una tabla
            var tbl = new PdfPTable(new float[] { 8f, 9f, 13f, 10f, 9f, 10f, 10f }) { WidthPercentage = 100f };
            var c1 = new PdfPCell(new Phrase("FECHA", parrafo6));
            var c2 = new PdfPCell(new Phrase("CONCEPTO", parrafo6));
            var c3 = new PdfPCell(new Phrase("CÓDIGO", parrafo6));
            var c4 = new PdfPCell(new Phrase("VALOR DE ADQUISICIÓN", parrafo6));
            var c5 = new PdfPCell(new Phrase("VALOR DE MEJORA", parrafo6));
            var c6 = new PdfPCell(new Phrase("DEPRECIACIÓN MENSUAL", parrafo6));
            //  var c7 = new PdfPCell(new Phrase("DEPRECIACIÓN ACUMULADA", parrafo6));
            var c8 = new PdfPCell(new Phrase("VALOR ACTUAL", parrafo6));
            //Agregamos a la tabla las celdas 
            tbl.AddCell(c1);
            tbl.AddCell(c2);
            tbl.AddCell(c3);
            tbl.AddCell(c4);
            tbl.AddCell(c5);
            tbl.AddCell(c6);
            //   tbl.AddCell(c7);
            tbl.AddCell(c8);

            //  doc.Add(new Phrase("\n"));





            //Extraemos de la base y llenamos las celdas
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
              // string fechaMin = "1-"+ "0"+ mes +"-"+  anio;
               //string fechaMax = "31-"+ "0" + mes + "-" + anio;
                
                string fechaMin = "1-1-" + anio;
                string fechaMax = "31-12-"+ anio;
               
                //string mes = "";

                double valortotal = 0;
                //  DateTime oDate = Convert.ToDateTime(fechaMax);
                DateTime uDate = DateTime.ParseExact(fechaMax, "dd-MM-yyyy", null);
                // oDate.ToString("dd-MM-yyyy");

                List<ActivoRevalorizadoAF> lista = (from activo in bd.ActivoFijo
                                                    join noFormulario in bd.FormularioIngreso
                                                    on activo.NoFormulario equals noFormulario.NoFormulario
                                                    join tarjeta in bd.TarjetaDepreciacion
                                                    on activo.IdBien equals tarjeta.IdBien
                                                    where (tarjeta.Fecha >= DateTime.Parse(fechaMin) && tarjeta.Fecha <= uDate)
                                                    && tarjeta.Concepto == "Depreciación"
                                                    orderby activo.IdBien
                                                    select new ActivoRevalorizadoAF
                                                    {
                                                        idBien = activo.IdBien,
                                                        //Código
                                                        codigo = activo.CorrelativoBien,
                                                        //Fecha
                                                        fecha = tarjeta.Fecha == null ? " " : ((DateTime)tarjeta.Fecha).ToString("dd-MM-yyyy"),
                                                        //Concepto
                                                        concepto = tarjeta.Concepto,
                                                        //Valor adquirido
                                                        valorAdquirido = activo.ValorAdquicicion.ToString(),
                                                        //Valor de mejora
                                                        montoTransaccion = Math.Round((double)tarjeta.Valor, 2),
                                                        //Depreciación anual
                                                        depreAnual = Math.Round((double)tarjeta.DepreciacionAnual, 2),
                                                        //Depreciación Acumulada
                                                        // depreAcum = Math.Round((double)tarjeta.DepreciacionAcumulada, 2),


                                                        //     valorTransaccion = Math.Round((double)tarjeta.ValorTransaccion, 2),
                                                        //Valor actual
                                                        valorActual = Math.Round((double)tarjeta.ValorActual, 2)

                                                    }).ToList();

                foreach (var activos in lista)
                {
                    c1.Phrase = new Phrase(activos.fecha, parrafo5);
                    c2.Phrase = new Phrase(activos.concepto, parrafo5);
                    c3.Phrase = new Phrase(activos.codigo, parrafo5);
                    c4.Phrase = new Phrase("$" + activos.valorAdquirido, parrafo5);
                    c5.Phrase = new Phrase("$" + activos.montoTransaccion, parrafo5);
                    // c7.Phrase = new Phrase("$" + activos.depreAcum, parrafo5);                     
                    // activos.depreAnual = activos.depreAnual / mes; 
                    activos.depreAnual = activos.depreAnual / 12;
                    activos.depreAnual = Math.Round((double)activos.depreAnual * mes, 2); //se divide entre mes para listar solo la cantidad que se depreciaría por mes.


                    c6.Phrase = new Phrase("$" + activos.depreAnual, parrafo5);
                    c8.Phrase = new Phrase("$" + activos.valorActual, parrafo5);
                    //Agregamos a la tabla
                    tbl.AddCell(c1);
                    tbl.AddCell(c2);
                    tbl.AddCell(c3);
                    tbl.AddCell(c4);
                    tbl.AddCell(c5);
                    tbl.AddCell(c6);
                    // tbl.AddCell(c7);
                    tbl.AddCell(c8);

                   // activos.depreAnual = valortotal + activos.depreAnual;

               


                }


                //AGREGAMOS NUEVA TABLA
                //Espacio en blanco
                doc.Add(Chunk.Newline);

                //Agregamos una tabla
                var tbl11 = new PdfPTable(new float[] { 50f, 50f }) { WidthPercentage = 100f };
                var c9 = new PdfPCell(new Phrase("TOTAL"));
                // var c10 = new PdfPCell(new Phrase("",parrafo6));

                //Agregamos a la tabla las celdas 
                tbl11.AddCell(c9);
                // tbl11.AddCell(c10);


                doc.Add(new Phrase("\n"));

                //PARA LA NUEVA TABLA



                ActivoRevalorizadoAF activototal = new ActivoRevalorizadoAF();
                string fechaMin1 = "1-" +  mes + "-" + anio;
                string fechaMax1 = "31-" + mes + "-" + anio;

               
                //  DateTime oDate = Convert.ToDateTime(fechaMax);
               // DateTime uDate1 = DateTime.ParseExact(fechaMax1, "dd-MM-yyyy", null);

                List<ActivoRevalorizadoAF> lista1 = (from activo in bd.ActivoFijo
                                                     join noFormulario in bd.FormularioIngreso
                                                     on activo.NoFormulario equals noFormulario.NoFormulario
                                                     join tarjeta in bd.TarjetaDepreciacion
                                                     on activo.IdBien equals tarjeta.IdBien
                                                     where (tarjeta.Fecha >= DateTime.Parse(fechaMin1) && tarjeta.Fecha <= uDate)
                                                     && tarjeta.Concepto == "Depreciación"
                                                     orderby activo.IdBien
                                                     select new ActivoRevalorizadoAF
                                                     {
                                                         idBien = activo.IdBien,

                                                         montoTransaccion = Math.Round((double)tarjeta.Valor, 2),
                                                         //Depreciación anual
                                                         depreAnual = Math.Round((double)tarjeta.DepreciacionAnual, 2),
                                                         //Depreciación Acumulada
                                                         depreAcum = Math.Round((double)tarjeta.DepreciacionAcumulada, 2),


                                                     }).ToList();

                foreach (var activos in lista1)
                {

                    //  c9.Phrase = new Phrase("$" + activos.depreAnual, parrafo5);
                    // c9.Phrase = new Phrase("$" + activos.depreAnual, parrafo5);
                    activos.depreAnual =activos.depreAnual/12;
                    activos.depreAnual = activos.depreAnual * mes;

                    //valortotal = valortotal + activos.depreAnual;
                    valortotal = Math.Round((double) valortotal + activos.depreAnual, 2);

                    //mes =  1;
                    // valortotal = valortotal / mes;

                    // tbl11.AddCell(activos.total.ToString());
                    // tbl11.AddCell(c9);
                }
                tbl11.AddCell("$" + valortotal.ToString());

                //INICIO DE ADICIÓN DE LOGO
                CooperativaAF oCooperativaAF = new CooperativaAF();

                Cooperativa oCooperativa = bd.Cooperativa.Where(p => p.Dhabilitado == 1).First();
                oCooperativaAF.idcooperativa = oCooperativa.IdCooperativa;


                try
                {
                    iTextSharp.text.Image logo = null;
                    logo = iTextSharp.text.Image.GetInstance(oCooperativa.Logo.ToString());
                    logo.Alignment = iTextSharp.text.Image.ALIGN_LEFT;
                    logo.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    logo.BorderColor = iTextSharp.text.BaseColor.White;
                    logo.ScaleToFit(170f, 100f);

                    float ancho = logo.Width;
                    float alto = logo.Height;
                    float proporcion = alto / ancho;

                    logo.ScaleAbsoluteWidth(80);
                    logo.ScaleAbsoluteHeight(80 * proporcion);

                    logo.SetAbsolutePosition(40f, 695f);

                    doc.Add(logo);

                }
                catch (DocumentException dex)
                {
                    //log exception here
                }

                //FIN DE ADICIÓN DE LOGO


                doc.Add(tbl);
                doc.Add(tbl11);
                writer.Close();
                doc.Close();
                ms.Seek(0, SeekOrigin.Begin);
                return File(ms, "application/pdf");
            }
        }

        //FIN DE REPORTE DE PROVISIÓN ANUAL




        //Reporte de bitácora
        [HttpGet]
        [Route("api/Reporte/bitacoraPdf")]
        public async Task<IActionResult> bitacoraPdf()
        {
            Document doc = new Document(PageSize.Letter);
            doc.SetMargins(40f, 40f, 40f, 40f);
            MemoryStream ms = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);

            //Instanciamos la clase para el paginado y la fecha de impresión
            var pe = new PageEventHelper();
            writer.PageEvent = pe;

            doc.AddAuthor("Asgard");
            doc.AddTitle("Reporte bitácora");
            doc.Open();

            //Inicia cuerpo del reporte

            //Estilo y fuente personalizada
            BaseFont fuente = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo = new iTextSharp.text.Font(fuente, 12f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente2 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo2 = new iTextSharp.text.Font(fuente2, 10f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente3 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo3 = new iTextSharp.text.Font(fuente3, 15f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente4 = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo4 = new iTextSharp.text.Font(fuente4, 11f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));

            //Para las celdas
            BaseFont fuente5 = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);
            iTextSharp.text.Font parrafo5 = new iTextSharp.text.Font(fuente5, 10f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));

            //Encabezado
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                CooperativaAF oCooperativaAF = new CooperativaAF();
                Cooperativa oCooperativa = bd.Cooperativa.Where(p => p.Dhabilitado == 1).First();
                oCooperativaAF.idcooperativa = oCooperativa.IdCooperativa;
                oCooperativaAF.nombre = oCooperativa.Nombre;
                oCooperativaAF.descripcion = oCooperativa.Descripcion;

                //Se agrega el encabezado
                var tbl1 = new PdfPTable(new float[] { 11f, 89f }) { WidthPercentage = 100f };
                tbl1.AddCell(new PdfPCell(new Phrase(" ", parrafo2)) { Border = 0, Rowspan = 2 });
                tbl1.AddCell(new PdfPCell(new Phrase(oCooperativa.Descripcion.ToUpper(), parrafo2)) { Border = 0, HorizontalAlignment = 1 });
                tbl1.AddCell(new PdfPCell(new Phrase(oCooperativa.Nombre.ToUpper(), parrafo3)) { Border = 0, HorizontalAlignment = 1 });
                doc.Add(tbl1);
                doc.Add(new Phrase("\n"));
            }
            doc.Add(new Phrase("\n"));
            //Línea separadora
            Chunk linea = new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(1f, 100f, BaseColor.Black, Element.ALIGN_CENTER, 1f));
            doc.Add(linea);
            doc.Add(new Paragraph("REPORTE DE BITÁCORA", parrafo) { Alignment = Element.ALIGN_CENTER });

            //Espacio en blanco
            doc.Add(Chunk.Newline);

            //Agregamos una tabla
            var tbl = new PdfPTable(new float[] { 26f, 23f, 22f, 38f }) { WidthPercentage = 100f };
            var c1 = new PdfPCell(new Phrase("NOMBRE EMPLEADO", parrafo2));
            var c2 = new PdfPCell(new Phrase("NOMBRE USUARIO", parrafo2));
            var c3 = new PdfPCell(new Phrase("FECHA", parrafo2));
            var c4 = new PdfPCell(new Phrase("DESCRIPCIÓN DEL PROCESO", parrafo2));
            //Agregamos a la tabla las celdas 
            tbl.AddCell(c1);
            tbl.AddCell(c2);
            tbl.AddCell(c3);
            tbl.AddCell(c4);

            //Extraemos de la base y llenamos las celdas
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<TablaBitacoraAF> listaBitacora = (from bitacora in bd.Bitacora
                                                                 join usuario in bd.Usuario
                                                                  on bitacora.IdUsuario equals usuario.IdUsuario
                                                                 join empleado in bd.Empleado
                                                                 on usuario.IdEmpleado equals empleado.IdEmpleado
                                                                 orderby bitacora.Fecha descending
                                                                 select new TablaBitacoraAF
                                                                 {
                                                                     idBitacora = bitacora.IdBitacora,
                                                                     nombreEmpleado = empleado.Nombres + " " + empleado.Apellidos,
                                                                     nombreUsuario = usuario.NombreUsuario,
                                                                     fecha = bitacora.Fecha == null ? " " : ((DateTime)bitacora.Fecha).ToString("dd-MM-yyyy : HH:mm:ss"),
                                                                     descripcion = bitacora.Descripcion,
                                                                 }).ToList();

                foreach (var bitacora in listaBitacora)
                {
                    c1.Phrase = new Phrase(bitacora.nombreEmpleado, parrafo5);
                    c2.Phrase = new Phrase(bitacora.nombreUsuario, parrafo5);
                    c3.Phrase = new Phrase(bitacora.fecha, parrafo5);
                    c4.Phrase = new Phrase(bitacora.descripcion, parrafo5);
                    //Agregamos a la tabla
                    tbl.AddCell(c1);
                    tbl.AddCell(c2);
                    tbl.AddCell(c3);
                    tbl.AddCell(c4);
                }

                //INICIO DE ADICIÓN DE LOGO
                CooperativaAF oCooperativaAF = new CooperativaAF();

                Cooperativa oCooperativa = bd.Cooperativa.Where(p => p.Dhabilitado == 1).First();
                oCooperativaAF.idcooperativa = oCooperativa.IdCooperativa;


                try
                {
                    iTextSharp.text.Image logo = null;
                    logo = iTextSharp.text.Image.GetInstance(oCooperativa.Logo.ToString());
                    logo.Alignment = iTextSharp.text.Image.ALIGN_LEFT;
                    logo.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    logo.BorderColor = iTextSharp.text.BaseColor.White;
                    logo.ScaleToFit(170f, 100f);

                    float ancho = logo.Width;
                    float alto = logo.Height;
                    float proporcion = alto / ancho;

                    logo.ScaleAbsoluteWidth(80);
                    logo.ScaleAbsoluteHeight(80 * proporcion);

                    logo.SetAbsolutePosition(40f, 695f);

                    doc.Add(logo);

                }
                catch (DocumentException dex)
                {
                    //log exception here
                }

                //FIN DE ADICIÓN DE LOGO

            }
            doc.Add(tbl);
            writer.Close();
            doc.Close();
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "application/pdf");
        }
        //Fin reporte de bitácora


        //Reporte de bitácora por año
        [HttpGet]
        [Route("api/Reporte/bitacoraAnioPdf/{anio}")]
        public async Task<IActionResult> bitacoraAnioPdf(int anio)
        {
            Document doc = new Document(PageSize.Letter);
            doc.SetMargins(40f, 40f, 40f, 40f);
            MemoryStream ms = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);

            //Instanciamos la clase para el paginado y la fecha de impresión
            var pe = new PageEventHelper();
            writer.PageEvent = pe;

            doc.AddAuthor("Asgard");
            doc.AddTitle("Reporte bitácora");
            doc.Open();

            //Inicia cuerpo del reporte

            //Estilo y fuente personalizada
            BaseFont fuente = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo = new iTextSharp.text.Font(fuente, 12f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente2 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo2 = new iTextSharp.text.Font(fuente2, 10f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente3 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo3 = new iTextSharp.text.Font(fuente3, 15f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente4 = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo4 = new iTextSharp.text.Font(fuente4, 11f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));

            //Para las celdas
            BaseFont fuente5 = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);
            iTextSharp.text.Font parrafo5 = new iTextSharp.text.Font(fuente5, 10f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));

            //Encabezado
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                CooperativaAF oCooperativaAF = new CooperativaAF();
                Cooperativa oCooperativa = bd.Cooperativa.Where(p => p.Dhabilitado == 1).First();
                oCooperativaAF.idcooperativa = oCooperativa.IdCooperativa;
                oCooperativaAF.nombre = oCooperativa.Nombre;
                oCooperativaAF.descripcion = oCooperativa.Descripcion;

                //Se agrega el encabezado
                var tbl1 = new PdfPTable(new float[] { 11f, 89f }) { WidthPercentage = 100f };
                tbl1.AddCell(new PdfPCell(new Phrase(" ", parrafo2)) { Border = 0, Rowspan = 2 });
                tbl1.AddCell(new PdfPCell(new Phrase(oCooperativa.Descripcion.ToUpper(), parrafo2)) { Border = 0, HorizontalAlignment = 1 });
                tbl1.AddCell(new PdfPCell(new Phrase(oCooperativa.Nombre.ToUpper(), parrafo3)) { Border = 0, HorizontalAlignment = 1 });
                doc.Add(tbl1);
                doc.Add(new Phrase("\n"));
            }
            doc.Add(new Phrase("\n"));
            //Línea separadora
            Chunk linea = new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(1f, 100f, BaseColor.Black, Element.ALIGN_CENTER, 1f));
            doc.Add(linea);
            doc.Add(new Paragraph("REPORTE DE BITÁCORA", parrafo) { Alignment = Element.ALIGN_CENTER });

            //Espacio en blanco
            doc.Add(Chunk.Newline);

            //Agregamos una tabla
            var tbl = new PdfPTable(new float[] { 26f, 23f, 22f, 38f }) { WidthPercentage = 100f };
            var c1 = new PdfPCell(new Phrase("NOMBRE EMPLEADO", parrafo2));
            var c2 = new PdfPCell(new Phrase("NOMBRE USUARIO", parrafo2));
            var c3 = new PdfPCell(new Phrase("FECHA", parrafo2));
            var c4 = new PdfPCell(new Phrase("DESCRIPCIÓN DEL PROCESO", parrafo2));
            //Agregamos a la tabla las celdas 
            tbl.AddCell(c1);
            tbl.AddCell(c2);
            tbl.AddCell(c3);
            tbl.AddCell(c4);

            //Extraemos de la base y llenamos las celdas
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                string fechaMin = "1-1-" + anio;
                string fechaMax = "31-12-" + anio;

                //  DateTime oDate = Convert.ToDateTime(fechaMax);
                DateTime uDate = DateTime.ParseExact(fechaMax, "dd-MM-yyyy", null);
                // oDate.ToString("dd-MM-yyyy");

                IEnumerable<TablaBitacoraAF> listaBitacora = (from bitacora in bd.Bitacora
                                                              join usuario in bd.Usuario
                                                               on bitacora.IdUsuario equals usuario.IdUsuario
                                                              join empleado in bd.Empleado
                                                              on usuario.IdEmpleado equals empleado.IdEmpleado
                                                              where (bitacora.Fecha >= DateTime.Parse(fechaMin) && bitacora.Fecha <= uDate)
                                                              orderby bitacora.Fecha descending
                                                              select new TablaBitacoraAF
                                                              {
                                                                  idBitacora = bitacora.IdBitacora,
                                                                  nombreEmpleado = empleado.Nombres + " " + empleado.Apellidos,
                                                                  nombreUsuario = usuario.NombreUsuario,
                                                                  fecha = bitacora.Fecha == null ? " " : ((DateTime)bitacora.Fecha).ToString("dd-MM-yyyy : HH:mm:ss"),
                                                                  descripcion = bitacora.Descripcion,
                                                              }).ToList();

                foreach (var bitacora in listaBitacora)
                {
                    c1.Phrase = new Phrase(bitacora.nombreEmpleado, parrafo5);
                    c2.Phrase = new Phrase(bitacora.nombreUsuario, parrafo5);
                    c3.Phrase = new Phrase(bitacora.fecha, parrafo5);
                    c4.Phrase = new Phrase(bitacora.descripcion, parrafo5);
                    //Agregamos a la tabla
                    tbl.AddCell(c1);
                    tbl.AddCell(c2);
                    tbl.AddCell(c3);
                    tbl.AddCell(c4);
                }

                //INICIO DE ADICIÓN DE LOGO
                CooperativaAF oCooperativaAF = new CooperativaAF();

                Cooperativa oCooperativa = bd.Cooperativa.Where(p => p.Dhabilitado == 1).First();
                oCooperativaAF.idcooperativa = oCooperativa.IdCooperativa;


                try
                {
                    iTextSharp.text.Image logo = null;
                    logo = iTextSharp.text.Image.GetInstance(oCooperativa.Logo.ToString());
                    logo.Alignment = iTextSharp.text.Image.ALIGN_LEFT;
                    logo.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    logo.BorderColor = iTextSharp.text.BaseColor.White;
                    logo.ScaleToFit(170f, 100f);

                    float ancho = logo.Width;
                    float alto = logo.Height;
                    float proporcion = alto / ancho;

                    logo.ScaleAbsoluteWidth(80);
                    logo.ScaleAbsoluteHeight(80 * proporcion);

                    logo.SetAbsolutePosition(40f, 695f);

                    doc.Add(logo);

                }
                catch (DocumentException dex)
                {
                    //log exception here
                }

                //FIN DE ADICIÓN DE LOGO

            }
            doc.Add(tbl);
            writer.Close();
            doc.Close();
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "application/pdf");
        }
        //Fin reporte de bitácira por año


        //INICIO DE REPORTE DE CÓDIGO DE BARRA POR ACTIVO

        [HttpGet]
        [Route("api/ReportesSeguridad/codigoBarraActivoPdf/{idactivo}")]
        public async Task<IActionResult> codigoBarraActivoPdf(int idactivo)
        {

            Document doc = new Document(PageSize.Letter);
            doc.SetMargins(40f, 40f, 40f, 40f);
            MemoryStream ms = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);

            //Instanciamos la clase para el paginado y la fecha de impresión
            var pe = new PageEventHelper();
            writer.PageEvent = pe;

            doc.AddAuthor("Asgard");
            doc.AddTitle("Código de Barra de activos");
            doc.Open();

            //Inicia cuerpo del reporte

            //Estilo y fuente personalizada
            BaseFont fuente = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo = new iTextSharp.text.Font(fuente, 12f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente2 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo2 = new iTextSharp.text.Font(fuente2, 10f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente3 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo3 = new iTextSharp.text.Font(fuente3, 15f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente4 = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo4 = new iTextSharp.text.Font(fuente4, 11f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));

            //Para las celdas
            BaseFont fuente5 = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);
            iTextSharp.text.Font parrafo5 = new iTextSharp.text.Font(fuente5, 10f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));


            //Encabezado
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                CooperativaAF oCooperativaAF = new CooperativaAF();
                Cooperativa oCooperativa = bd.Cooperativa.Where(p => p.Dhabilitado == 1).First();
                oCooperativaAF.idcooperativa = oCooperativa.IdCooperativa;
                oCooperativaAF.nombre = oCooperativa.Nombre;
                oCooperativaAF.descripcion = oCooperativa.Descripcion;

                //Se agrega el encabezado
                var tbl1 = new PdfPTable(new float[] { 11f, 89f }) { WidthPercentage = 100f };
                tbl1.AddCell(new PdfPCell(new Phrase(" ", parrafo2)) { Border = 0, Rowspan = 2 });
                tbl1.AddCell(new PdfPCell(new Phrase(oCooperativa.Descripcion.ToUpper(), parrafo2)) { Border = 0, HorizontalAlignment = 1 });
                tbl1.AddCell(new PdfPCell(new Phrase(oCooperativa.Nombre.ToUpper(), parrafo3)) { Border = 0, HorizontalAlignment = 1 });
                doc.Add(tbl1);
                doc.Add(new Phrase("\n"));
            }
            doc.Add(new Phrase("\n"));
            //Línea separadora
            Chunk linea = new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(1f, 100f, BaseColor.Black, Element.ALIGN_CENTER, 1f));
            doc.Add(linea);
            doc.Add(new Paragraph("CÓDIGO DE BARRAS DEL ACTIVO: ", parrafo) { Alignment = Element.ALIGN_CENTER });

            //Espacio en blanco
            doc.Add(Chunk.Newline);
            

            
            //Objeto cadena
            BarcodeLib.Barcode barcodeAPI = new BarcodeLib.Barcode();

            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {



                DatosCodigoBarraAF oDatos = new DatosCodigoBarraAF();
                //string nombre;
               
                ActivoFijo oActivo = bd.ActivoFijo.Where(p => p.IdBien == idactivo).First();
                Marcas oMarca = bd.Marcas.Where(p => p.IdMarca == oActivo.IdMarca).First();
               //  Marcas oMarca = bd.Marcas.Where(p => p.IdMarca == oActivo.IdMarca).First();
              
                doc.Add(Chunk.Newline);




                //string prodCode = "038000356216";
                PdfPCell descripcion = new PdfPCell(new Phrase(oActivo.Desripcion +" - "+oMarca.Marca+" "+ oActivo.Modelo,parrafo));
                descripcion.HorizontalAlignment = 1;
                descripcion.Border = 0;
               
                PdfContentByte cb = writer.DirectContent;
                cb.Rectangle(doc.PageSize.Width - 900f, 830f, 900f, 900f);
                cb.Stroke();
                iTextSharp.text.pdf.Barcode128 bc = new Barcode128();
                bc.TextAlignment = Element.ALIGN_CENTER;
                //bc.Code = "AC01-001-105-001";
                bc.Code = oActivo.CorrelativoBien;
               // bc.AltText = oActivo.Modelo;
                bc.StartStopText = false;
                bc.CodeType = iTextSharp.text.pdf.Barcode128.CODE128;
                bc.Extended = true;
                //bc.Font = null;
                PdfContentByte cba = writer.DirectContent;

                iTextSharp.text.Image PatImage1 = bc.CreateImageWithBarcode(cb, iTextSharp.text.BaseColor.Black, iTextSharp.text.BaseColor.Black);
                PatImage1.ScaleToFit(250, 250);

                PdfPTable p_detail1 = new PdfPTable(1);
                p_detail1.WidthPercentage = 45;

                PdfPCell barcideimage = new PdfPCell(PatImage1);
                //barcideimage.Colspan = 2;
                barcideimage.HorizontalAlignment = 3;
                barcideimage.Border = 0;
                 p_detail1.AddCell(descripcion);
                p_detail1.AddCell(barcideimage);

                doc.Add(p_detail1);


               }

            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                CooperativaAF oCooperativaAF = new CooperativaAF();
                Cooperativa oCooperativa = bd.Cooperativa.Where(p => p.Dhabilitado == 1).First();
                oCooperativaAF.idcooperativa = oCooperativa.IdCooperativa;
                try
                {
                    iTextSharp.text.Image logo = null;
                    //Acá le cambie el logo de la cooperativa para probar el código de barra para ahorra código
                    logo = iTextSharp.text.Image.GetInstance(oCooperativa.Logo.ToString());
                    logo.Alignment = iTextSharp.text.Image.ALIGN_LEFT;
                    logo.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    logo.BorderColor = iTextSharp.text.BaseColor.White;
                    logo.ScaleToFit(170f, 100f);

                    float ancho = logo.Width;
                    float alto = logo.Height;
                    float proporcion = alto / ancho;

                    logo.ScaleAbsoluteWidth(80);
                    logo.ScaleAbsoluteHeight(80 * proporcion);

                    logo.SetAbsolutePosition(40f, 695f);

                    doc.Add(logo);

                }
                catch (DocumentException dex)
                {
                    //log exception here
                }

                //FIN DE ADICIÓN DE LOGO

            }
           
            writer.Close();
            doc.Close();
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "application/pdf");
        }
        [HttpGet]
        [Route("api/ReportesSeguridad/codigoBarraGeneradoPdf/{codigo}/{nombre}/{marca}/{modelo}")]
        public async Task<IActionResult> codigoBarraGeneradoPdf(string codigo, string nombre, string marca, string modelo)
        {

            Document doc = new Document(PageSize.Letter);
            doc.SetMargins(40f, 40f, 40f, 40f);
            MemoryStream ms = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);

            //Instanciamos la clase para el paginado y la fecha de impresión
            var pe = new PageEventHelper();
            writer.PageEvent = pe;

            doc.AddAuthor("Asgard");
            doc.AddTitle("Código de Barra de activos");
            doc.Open();

            //Inicia cuerpo del reporte

            //Estilo y fuente personalizada

            BaseFont fuent = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1250, true);
            iTextSharp.text.Font parraf = new iTextSharp.text.Font(fuent, 12f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente2 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo2 = new iTextSharp.text.Font(fuente2, 10f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente3 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo3 = new iTextSharp.text.Font(fuente3, 15f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente4 = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo4 = new iTextSharp.text.Font(fuente4, 11f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));

            //Para las celdas
            BaseFont fuente5 = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);
            iTextSharp.text.Font parrafo5 = new iTextSharp.text.Font(fuente5, 10f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));


            //Encabezado
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                CooperativaAF oCooperativaAF = new CooperativaAF();
                Cooperativa oCooperativa = bd.Cooperativa.Where(p => p.Dhabilitado == 1).First();
                oCooperativaAF.idcooperativa = oCooperativa.IdCooperativa;
                oCooperativaAF.nombre = oCooperativa.Nombre;
                oCooperativaAF.descripcion = oCooperativa.Descripcion;

                //Se agrega el encabezado
                var tbl1 = new PdfPTable(new float[] { 11f, 89f }) { WidthPercentage = 100f };
                tbl1.AddCell(new PdfPCell(new Phrase(" ", parrafo2)) { Border = 0, Rowspan = 2 });
                tbl1.AddCell(new PdfPCell(new Phrase(oCooperativa.Descripcion.ToUpper(), parrafo2)) { Border = 0, HorizontalAlignment = 1 });
                tbl1.AddCell(new PdfPCell(new Phrase(oCooperativa.Nombre.ToUpper(), parrafo3)) { Border = 0, HorizontalAlignment = 1 });
                doc.Add(tbl1);
                doc.Add(new Phrase("\n"));
            }
            doc.Add(new Phrase("\n"));
            //Línea separadora
            Chunk linea = new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(1f, 100f, BaseColor.Black, Element.ALIGN_CENTER, 1f));
            doc.Add(linea);
            doc.Add(new Paragraph("CÓDIGO DE BARRAS DEL ACTIVO: ", parraf) { Alignment = Element.ALIGN_CENTER });

            //Espacio en blanco
            doc.Add(Chunk.Newline);



            //Objeto cadena
            BarcodeLib.Barcode barcodeAPI = new BarcodeLib.Barcode();

            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {



                DatosCodigoBarraAF oDatos = new DatosCodigoBarraAF();
                //string nombre;

                // ActivoFijo oActivo = bd.ActivoFijo.Where(p => p.IdBien == idactivo).First();
                // Marcas oMarca = bd.Marcas.Where(p => p.IdMarca == oActivo.IdMarca).First();
                // Marcas oMarca = bd.Marcas.Where(p => p.IdMarca == oActivo.IdMarca).First();

                doc.Add(Chunk.Newline);




                //string prodCode = "038000356216";
                PdfPCell descripcion = new PdfPCell(new Phrase(nombre + " - " + marca + " " + modelo,parraf));
                descripcion.HorizontalAlignment = 1;
                descripcion.Border = 0;

                PdfContentByte cb = writer.DirectContent;
                cb.Rectangle(doc.PageSize.Width - 900f, 830f, 900f, 900f);
                cb.Stroke();
                iTextSharp.text.pdf.Barcode128 bc = new Barcode128();
                bc.TextAlignment = Element.ALIGN_CENTER;
                //bc.Code = "AC01-001-105-001";
                bc.Code = codigo;
                //bc.AltText = modelo;
                bc.StartStopText = false;
                bc.CodeType = iTextSharp.text.pdf.Barcode128.CODE128;
                bc.Extended = true;
                //bc.Font = null;
                PdfContentByte cba = writer.DirectContent;

                iTextSharp.text.Image PatImage1 = bc.CreateImageWithBarcode(cb, iTextSharp.text.BaseColor.Black, iTextSharp.text.BaseColor.Black);
                PatImage1.ScaleToFit(250, 250);

                PdfPTable p_detail1 = new PdfPTable(1);
                p_detail1.WidthPercentage = 45;

                PdfPCell barcideimage = new PdfPCell(PatImage1);
                //barcideimage.Colspan = 2;
                barcideimage.HorizontalAlignment = 3;
                barcideimage.Border = 0;
                p_detail1.AddCell(descripcion);
                p_detail1.AddCell(barcideimage);


                doc.Add(p_detail1);


            }

            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                CooperativaAF oCooperativaAF = new CooperativaAF();
                Cooperativa oCooperativa = bd.Cooperativa.Where(p => p.Dhabilitado == 1).First();
                oCooperativaAF.idcooperativa = oCooperativa.IdCooperativa;
                try
                {
                    iTextSharp.text.Image logo = null;
                    //Acá le cambie el logo de la cooperativa para probar el código de barra para ahorra código
                    logo = iTextSharp.text.Image.GetInstance(oCooperativa.Logo.ToString());
                    logo.Alignment = iTextSharp.text.Image.ALIGN_LEFT;
                    logo.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    logo.BorderColor = iTextSharp.text.BaseColor.White;
                    logo.ScaleToFit(170f, 100f);

                    float ancho = logo.Width;
                    float alto = logo.Height;
                    float proporcion = alto / ancho;

                    logo.ScaleAbsoluteWidth(80);
                    logo.ScaleAbsoluteHeight(80 * proporcion);

                    logo.SetAbsolutePosition(40f, 695f);

                    doc.Add(logo);

                }
                catch (DocumentException dex)
                {
                    //log exception here
                }

                //FIN DE ADICIÓN DE LOGO

            }

            writer.Close();
            doc.Close();
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "application/pdf");
        }
        //FIN DE REPORTE DE CÓDIGO DE BARRA POR ACTIVO
    }
}