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
        [Route("api/ReportesSeguridad/listarUsuarios")]
        public List<UsuarioAF> listarUsuarios()
        {
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
                return lista;

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


                //Tabla de transacciones
                // doc.Add(new Paragraph("TABLA HISTORIAL DE TRASPASOS", parrafo2) { Alignment = Element.ALIGN_CENTER });


                //Agregamos una tabla
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
    }
}