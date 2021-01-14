using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
using ASGARDAPI.Models;
using ASGARDAPI.Clases;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using BarcodeLib;
using System.Drawing.Imaging;
using Color = System.Drawing.Color;
using System.Drawing;



namespace ASGARDAPI.Controllers
{
    public class ReportesControlActivoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        //Inicio de reportes de control de activos

        //Reporte activos asignados

        [HttpGet]
        [Route("api/Reporte/activosAsignadosPdf")]
        public async Task<IActionResult> activosAsignadosPdf()
        {
            Document doc = new Document(PageSize.Letter);
            doc.SetMargins(40f, 40f, 40f, 40f);
            MemoryStream ms = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);

            //Instanciamos la clase para el paginado y la fecha de impresión
            var pe = new PageEventHelper();
            writer.PageEvent = pe;

            doc.AddAuthor("Asgard");
            doc.AddTitle("Reporte activos asignados");
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
            doc.Add(new Paragraph("REPORTE DE ACTIVOS ASIGNADOS", parrafo) { Alignment = Element.ALIGN_CENTER });

            //Espacio en blanco
            doc.Add(Chunk.Newline);

            //Agregamos una tabla
            var tbl = new PdfPTable(new float[] { 17f, 11f, 27f, 25f, 20f }) { WidthPercentage = 100f };
            var c1 = new PdfPCell(new Phrase("CÓDIGO", parrafo2));
            var c2 = new PdfPCell(new Phrase("FECHA", parrafo2));
            var c3 = new PdfPCell(new Phrase("DESCRIPCIÓN", parrafo2));
            var c4 = new PdfPCell(new Phrase("ÁREA DE NEGOCIO", parrafo2));
            var c5 = new PdfPCell(new Phrase("RESPONSABLE", parrafo2));
            //Agregamos a la tabla las celdas
            tbl.AddCell(c1);
            tbl.AddCell(c2);
            tbl.AddCell(c3);
            tbl.AddCell(c4);
            tbl.AddCell(c5);

            //Extraemos de la base y llenamos las celdas
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

                foreach (var activoA in lista)
                {
                    c1.Phrase = new Phrase(activoA.Codigo, parrafo5);
                    c2.Phrase = new Phrase(activoA.fechacadena, parrafo5);
                    c3.Phrase = new Phrase(activoA.Descripcion, parrafo5);
                    c4.Phrase = new Phrase(activoA.AreaDeNegocio, parrafo5);
                    c5.Phrase = new Phrase(activoA.Responsable, parrafo5);
                    //Agregamos a la tabla
                    tbl.AddCell(c1);
                    tbl.AddCell(c2);
                    tbl.AddCell(c3);
                    tbl.AddCell(c4);
                    tbl.AddCell(c5);
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

        //Fin reporte activos asignados

        //Reporte activos no asignados

        [HttpGet]
        [Route("api/Reporte/activosNoAsignadosPdf")]
        public async Task<IActionResult> activosNoAsignadosPdf()
        {
            Document doc = new Document(PageSize.Letter);
            doc.SetMargins(40f, 40f, 40f, 40f);
            MemoryStream ms = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);

            //Instanciamos la clase para el paginado y la fecha de impresión
            var pe = new PageEventHelper();
            writer.PageEvent = pe;

            doc.AddAuthor("Asgard");
            doc.AddTitle("Reporte activos no asignados");
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
            doc.Add(new Paragraph("REPORTE DE ACTIVOS NO ASIGNADOS", parrafo) { Alignment = Element.ALIGN_CENTER });

            //Espacio en blanco
            doc.Add(Chunk.Newline);

            //Agregamos una tabla
            var tbl = new PdfPTable(new float[] { 20f, 11f, 29f, 40f}) { WidthPercentage = 100f };
            var c1 = new PdfPCell(new Phrase("NO. FORMULARIO", parrafo2));
            var c2 = new PdfPCell(new Phrase("FECHA", parrafo2));
            var c3 = new PdfPCell(new Phrase("CLASIFICACIÓN", parrafo2));
            var c4 = new PdfPCell(new Phrase("DESCRIPCIÓN", parrafo2));
            //Agregamos a la tabla las celdas 
            tbl.AddCell(c1);
            tbl.AddCell(c2);
            tbl.AddCell(c3);
            tbl.AddCell(c4);

            //Extraemos de la base y llenamos las celdas
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<NoAsignadosAF> lista = (from activo in bd.ActivoFijo
                                                    join noFormulario in bd.FormularioIngreso
                                                    on activo.NoFormulario equals noFormulario.NoFormulario
                                                    join clasif in bd.Clasificacion
                                                    on activo.IdClasificacion equals clasif.IdClasificacion
                                                    where (activo.EstadoActual == 1 || activo.EstadoActual == 2 || activo.EstadoActual == 3) && activo.EstaAsignado == 0
                                                    select new NoAsignadosAF
                                                    {
                                                        IdBien = activo.IdBien,
                                                        NoFormulario = noFormulario.NoFormulario,
                                                        fechacadena = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                                        Desripcion = activo.Desripcion,
                                                        Clasificacion = clasif.Clasificacion1

                                                    }).ToList();

                foreach (var activoNoA in lista)
                {
                    c1.Phrase = new Phrase(activoNoA.NoFormulario.ToString(), parrafo5);
                    c2.Phrase = new Phrase(activoNoA.fechacadena, parrafo5);
                    c3.Phrase = new Phrase(activoNoA.Clasificacion, parrafo5);
                    c4.Phrase = new Phrase(activoNoA.Desripcion, parrafo5);
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

        //Fin reporte activos no asignados

        //Reporte edificios e instalaciones

        [HttpGet]
        [Route("api/Reporte/edificiosInstalacionesPdf")]
        public async Task<IActionResult> edificiosInstalacionesPdf()
        {
            Document doc = new Document(PageSize.Letter);
            doc.SetMargins(40f, 40f, 40f, 40f);
            MemoryStream ms = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);

            //Instanciamos la clase para el paginado y la fecha de impresión
            var pe = new PageEventHelper();
            writer.PageEvent = pe;

            doc.AddAuthor("Asgard");
            doc.AddTitle("Reporte edficios e instalaciones");
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
            doc.Add(new Paragraph("REPORTE DE EDIFICIOS E INSTALACIONES", parrafo) { Alignment = Element.ALIGN_CENTER });

            //Espacio en blanco
            doc.Add(Chunk.Newline);

            //Agregamos una tabla
            var tbl = new PdfPTable(new float[] { 16f, 11f, 30f, 43f }) { WidthPercentage = 100f };
            var c1 = new PdfPCell(new Phrase("CÓDIGO", parrafo2));
            var c2 = new PdfPCell(new Phrase("FECHA", parrafo2));
            var c3 = new PdfPCell(new Phrase("CLASIFICACIÓN", parrafo2));
            var c4 = new PdfPCell(new Phrase("DESCRIPCIÓN", parrafo2));
            //Agregamos a la tabla las celdas 
            tbl.AddCell(c1);
            tbl.AddCell(c2);
            tbl.AddCell(c3);
            tbl.AddCell(c4);

            //Extraemos de la base y llenamos las celdas
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                List<RegistroAF> lista = (from activo in bd.ActivoFijo
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
                                              Clasificacion = clasif.Clasificacion1,
                                          }).ToList();

                foreach (var edificio in lista)
                {
                    c1.Phrase = new Phrase(edificio.Codigo, parrafo5);
                    c2.Phrase = new Phrase(edificio.fechacadena, parrafo5);
                    c3.Phrase = new Phrase(edificio.Clasificacion, parrafo5);
                    c4.Phrase = new Phrase(edificio.Descripcion, parrafo5);
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

        //Fin reporte edificios e instalaciones

        //Reporte activos intangibles

        [HttpGet]
        [Route("api/Reporte/activosIntangiblesPdf")]
        public async Task<IActionResult> activosIntangiblesPdf()
        {
            Document doc = new Document(PageSize.Letter);
            doc.SetMargins(40f, 40f, 40f, 40f);
            MemoryStream ms = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);

            //Instanciamos la clase para el paginado y la fecha de impresión
            var pe = new PageEventHelper();
            writer.PageEvent = pe;

            doc.AddAuthor("Asgard");
            doc.AddTitle("Reporte activos intangibles");
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
            doc.Add(new Paragraph("REPORTE DE ACTIVOS INTANGIBLES", parrafo) { Alignment = Element.ALIGN_CENTER });

            //Espacio en blanco
            doc.Add(Chunk.Newline);

            //Agregamos una tabla
            var tbl = new PdfPTable(new float[] { 18f, 11f, 29f, 42f }) { WidthPercentage = 100f };
            var c1 = new PdfPCell(new Phrase("CÓDIGO", parrafo2));
            var c2 = new PdfPCell(new Phrase("FECHA", parrafo2));
            var c3 = new PdfPCell(new Phrase("CLASIFICACIÓN", parrafo2));
            var c4 = new PdfPCell(new Phrase("DESCRIPCIÓN", parrafo2));
            //Agregamos a la tabla las celdas 
            tbl.AddCell(c1);
            tbl.AddCell(c2);
            tbl.AddCell(c3);
            tbl.AddCell(c4);

            //Extraemos de la base y llenamos las celdas
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                List<RegistroAF> lista = (from activo in bd.ActivoFijo
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
                                              Clasificacion = clasif.Clasificacion1,
                                          }).ToList();

                foreach (var intangible in lista)
                {
                    c1.Phrase = new Phrase(intangible.Codigo, parrafo5);
                    c2.Phrase = new Phrase(intangible.fechacadena, parrafo5);
                    c3.Phrase = new Phrase(intangible.Clasificacion, parrafo5);
                    c4.Phrase = new Phrase(intangible.Descripcion, parrafo5);
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

        //Fin reporte activos intangibles

        //Reporte de activos por jefe
        [HttpGet]
        [Route("api/Reporte/activosJefePdf/{idJefe}")]
        public async Task<IActionResult> activosJefePdf(int idJefe)
        {
            Document doc = new Document(PageSize.Letter);
            doc.SetMargins(40f, 40f, 40f, 40f);
            MemoryStream ms = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);

            //Instanciamos la clase para el paginado y la fecha de impresión
            var pe = new PageEventHelper();
            writer.PageEvent = pe;

            doc.AddAuthor("Asgard");
            doc.AddTitle("Reporte activos");
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
            doc.Add(new Paragraph("REPORTE DE ACTIVOS ASIGNADOS", parrafo) { Alignment = Element.ALIGN_CENTER });

            //Espacio en blanco
            doc.Add(Chunk.Newline);

            //Agregamos una tabla
            var tbl = new PdfPTable(new float[] { 17f, 11f, 27f, 25f, 20f }) { WidthPercentage = 100f };
            var c1 = new PdfPCell(new Phrase("CÓDIGO", parrafo2));
            var c2 = new PdfPCell(new Phrase("FECHA", parrafo2));
            var c3 = new PdfPCell(new Phrase("DESCRIPCIÓN", parrafo2));
            var c4 = new PdfPCell(new Phrase("ÁREA DE NEGOCIO", parrafo2));
            var c5 = new PdfPCell(new Phrase("RESPONSABLE", parrafo2));
            //Agregamos a la tabla las celdas
            tbl.AddCell(c1);
            tbl.AddCell(c2);
            tbl.AddCell(c3);
            tbl.AddCell(c4);
            tbl.AddCell(c5);

            //Extraemos de la base y llenamos las celdas
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                Empleado oempleado = bd.Empleado.Where(p => p.IdEmpleado == idJefe).FirstOrDefault();
                AreaDeNegocio oarea = bd.AreaDeNegocio.Where(p => p.IdAreaNegocio == oempleado.IdAreaDeNegocio).FirstOrDefault();
                List<RegistroAF> lista = (from activo in bd.ActivoFijo
                                          join noFormulario in bd.FormularioIngreso
                                          on activo.NoFormulario equals noFormulario.NoFormulario
                                          join clasif in bd.Clasificacion
                                          on activo.IdClasificacion equals clasif.IdClasificacion
                                          join resposable in bd.Empleado
                                          on activo.IdResponsable equals resposable.IdEmpleado
                                          join area in bd.AreaDeNegocio
                                          on resposable.IdAreaDeNegocio equals area.IdAreaNegocio
                                          where (activo.EstadoActual != 0) && activo.EstaAsignado == 1 && area.IdAreaNegocio == oarea.IdAreaNegocio

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

                foreach (var activoA in lista)
                {
                    c1.Phrase = new Phrase(activoA.Codigo, parrafo5);
                    c2.Phrase = new Phrase(activoA.fechacadena, parrafo5);
                    c3.Phrase = new Phrase(activoA.Descripcion, parrafo5);
                    c4.Phrase = new Phrase(activoA.AreaDeNegocio, parrafo5);
                    c5.Phrase = new Phrase(activoA.Responsable, parrafo5);
                    //Agregamos a la tabla
                    tbl.AddCell(c1);
                    tbl.AddCell(c2);
                    tbl.AddCell(c3);
                    tbl.AddCell(c4);
                    tbl.AddCell(c5);
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
        //Fin de reporte de activos por jefe

        //Reporte de código de barra general
        [HttpGet]
        [Route("api/Reporte/codigoBarraGeneralPdf")]
        public async Task<IActionResult> codigoBarraGeneralPdf()
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
               
               // doc.Add(new Phrase("\n"));
            }
            doc.Add(new Phrase("\n"));
            //Línea separadora
            Chunk linea = new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(1f, 100f, BaseColor.Black, Element.ALIGN_CENTER, 1f));
            doc.Add(linea);
            doc.Add(new Paragraph("CÓDIGO DE BARRA DE ACTIVOS", parrafo) { Alignment = Element.ALIGN_CENTER });

            //Espacio en blanco
            doc.Add(Chunk.Newline);

            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {

                PdfContentByte cb = writer.DirectContent;
                cb.Rectangle(doc.PageSize.Width - 90f, 830f, 50f, 50f);
                cb.Stroke();
                iTextSharp.text.pdf.Barcode128 bc = new Barcode128();
                bc.TextAlignment = Element.ALIGN_CENTER;

                //Llamo la lista para recorrer todos los activos
                List<RegistroAF> lista = (from activo in bd.ActivoFijo
                                          join noFormulario in bd.FormularioIngreso
                                          on activo.NoFormulario equals noFormulario.NoFormulario
                                          join clasif in bd.Clasificacion
                                          on activo.IdClasificacion equals clasif.IdClasificacion
                                          join marca in bd.Marcas
                                          on activo.IdMarca equals marca.IdMarca
                                          join resposable in bd.Empleado
                                          on activo.IdResponsable equals resposable.IdEmpleado
                                          join area in bd.AreaDeNegocio
                                          on resposable.IdAreaDeNegocio equals area.IdAreaNegocio
                                          where (activo.EstadoActual != 0) && activo.EstaAsignado == 1

                                          orderby activo.IdBien ascending
                                          select new RegistroAF
                                          {
                                              IdBien = activo.IdBien,
                                              //Sólo necesito el correlativo
                                              Codigo = activo.CorrelativoBien,
                                              Descripcion=activo.Desripcion,
                                              modelo=activo.Modelo,
                                              marca=marca.Marca
                       
                                          }).ToList();

                
               
                //Recorro la lista
                foreach (var activoA in lista)
                {
                  //  doc.Add(new Phrase(activoA.Descripcion + " - " + activoA.modelo, parrafo) { Element.ALIGN_CENTER });
                    bc.Code = activoA.Codigo;
                    bc.StartStopText = false;
                    bc.CodeType = iTextSharp.text.pdf.Barcode128.CODE128;
                    bc.Extended = true;
                    iTextSharp.text.Image PatImage1 = bc.CreateImageWithBarcode(cb, iTextSharp.text.BaseColor.Black, iTextSharp.text.BaseColor.Black);
                    PatImage1.ScaleToFit(250, 250);

                    PdfPTable p_detail1 = new PdfPTable(1);
                    p_detail1.WidthPercentage = 50;

                    PdfPCell barcideimage = new PdfPCell(PatImage1);
                    barcideimage.HorizontalAlignment = 3;
                    barcideimage.Border = 0;
                    PdfPCell desc = new PdfPCell(new Phrase(activoA.Descripcion + " - "+" "+ activoA.marca +" "+ activoA.modelo+" ", parrafo));
                    desc.HorizontalAlignment = 1;
                    desc.Border = 0;
                    p_detail1.AddCell(desc);
                    p_detail1.AddCell(barcideimage);
                    doc.Add(p_detail1);

                }

            }
          //  doc.Add(tbl);
            writer.Close();
            doc.Close();
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "application/pdf");
        }
        //Fin reporte de código de barra general


        //Reporte activos adquiridos por año
        [HttpGet]
        [Route("api/Reporte/activosPorAnioPdf/{anio}")]
        public async Task<IActionResult> activosPorAnioPdf(int anio)
        {
            Document doc = new Document(PageSize.Letter);
            doc.SetMargins(40f, 40f, 40f, 40f);
            MemoryStream ms = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);

            //Instanciamos la clase para el paginado y la fecha de impresión
            var pe = new PageEventHelper();
            writer.PageEvent = pe;

            doc.AddAuthor("Asgard");
            doc.AddTitle("Reporte activos por año");
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
            doc.Add(new Paragraph("REPORTE DE ACTIVOS ADQUIRIDOS POR AÑO", parrafo) { Alignment = Element.ALIGN_CENTER });

            //Espacio en blanco
            doc.Add(Chunk.Newline);

            //Agregamos una tabla
            var tbl = new PdfPTable(new float[] { 24f, 20f, 25f, 36f }) { WidthPercentage = 100f };
            var c1 = new PdfPCell(new Phrase("FECHA ADQUISICIÓN", parrafo2));
            var c2 = new PdfPCell(new Phrase("CÓDIGO", parrafo2));
            var c3 = new PdfPCell(new Phrase("VALOR DE ADQUISICIÓN", parrafo2));
            var c4 = new PdfPCell(new Phrase("DESCRIPCIÓN", parrafo2));
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

                DateTime uDate = DateTime.ParseExact(fechaMax, "dd-MM-yyyy", null);

                List<RegistroAF> lista = (from activo in bd.ActivoFijo
                                          join noFormulario in bd.FormularioIngreso
                                          on activo.NoFormulario equals noFormulario.NoFormulario
                                          where (activo.EstadoActual == 1 || activo.EstadoActual == 2 || activo.EstadoActual == 3)
                                        && (noFormulario.FechaIngreso >= DateTime.Parse(fechaMin) && noFormulario.FechaIngreso <= uDate)
                                          orderby activo.IdBien
                                          select new RegistroAF
                                          {
                                              IdBien = activo.IdBien,
                                              Codigo = activo.CorrelativoBien,
                                              fechacadena = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                              Descripcion = activo.Desripcion,
                                              valoradquicicion=activo.ValorAdquicicion.ToString()


                                          }).ToList();

                foreach (var activos in lista)
                {
                    c1.Phrase = new Phrase(activos.fechacadena, parrafo5);
                    c2.Phrase = new Phrase(activos.Codigo, parrafo5);
                    c3.Phrase = new Phrase("$"+activos.valoradquicicion, parrafo5);
                    c4.Phrase = new Phrase(activos.Descripcion, parrafo5);
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
        //Fin activos adquiridos por año

        //Reporte de depreciación anual
        [HttpGet]
        [Route("api/Reporte/depreciacionAnualPdf/{anio}")]
        public async Task<IActionResult> depreciacionAnualPdf(int anio)
        {
            Document doc = new Document(PageSize.Letter);
            doc.SetMargins(40f, 40f, 40f, 40f);
            MemoryStream ms = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);

            //Instanciamos la clase para el paginado y la fecha de impresión
            var pe = new PageEventHelper();
            writer.PageEvent = pe;

            doc.AddAuthor("Asgard");
            doc.AddTitle("Reporte activos depreciados por año");
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
            doc.Add(new Paragraph("REPORTE DE ACTIVOS DEPRECIADOS POR AÑO", parrafo) { Alignment = Element.ALIGN_CENTER });

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
            var c7 = new PdfPCell(new Phrase("VALOR ACTUAL", parrafo6));
            //Agregamos a la tabla las celdas 
            tbl.AddCell(c1);
            tbl.AddCell(c2);
            tbl.AddCell(c3);
            tbl.AddCell(c4);
            tbl.AddCell(c5);
            tbl.AddCell(c6);
            tbl.AddCell(c7);

            //Extraemos de la base y llenamos las celdas
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                string fechaMin = "1-1-" + anio;
                string fechaMax = "31-12-" + anio;

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
                                                        depreAcum = Math.Round((double)tarjeta.DepreciacionAcumulada, 2),

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
                    c7.Phrase = new Phrase("$" + activos.valorActual, parrafo5);
                    //Agregamos a la tabla
                    tbl.AddCell(c1);
                    tbl.AddCell(c2);
                    tbl.AddCell(c3);
                    tbl.AddCell(c4);
                    tbl.AddCell(c5);
                    tbl.AddCell(c6);
                    tbl.AddCell(c7);
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
        //Fin reporte de depreciación anual 

        //Reporte activos revalorizados por año
        [HttpGet]
        [Route("api/Reporte/activosRevalorizadosAnioPdf/{anio}")]
        public async Task<IActionResult> activosRevalorizadosAnioPdf(int anio)
        {
            Document doc = new Document(PageSize.Letter);
            doc.SetMargins(40f, 40f, 40f, 40f);
            MemoryStream ms = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);

            //Instanciamos la clase para el paginado y la fecha de impresión
            var pe = new PageEventHelper();
            writer.PageEvent = pe;

            doc.AddAuthor("Asgard");
            doc.AddTitle("Reporte activos revalorizados por año");
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
            iTextSharp.text.Font parrafo5 = new iTextSharp.text.Font(fuente5, 10f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente6 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo6 = new iTextSharp.text.Font(fuente2, 8.5f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));

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
            doc.Add(new Paragraph("REPORTE DE ACTIVOS REVALORIZADOS POR AÑO", parrafo) { Alignment = Element.ALIGN_CENTER });

            //Espacio en blanco
            doc.Add(Chunk.Newline);

            //Agregamos una tabla
            var tbl = new PdfPTable(new float[] { 12f, 15f, 18f, 13f, 14f, 13f, 19 }) { WidthPercentage = 100f };
            var c1 = new PdfPCell(new Phrase("FECHA", parrafo6));
            var c2 = new PdfPCell(new Phrase("CONCEPTO", parrafo6));
            var c3 = new PdfPCell(new Phrase("CÓDIGO", parrafo6));
            var c4 = new PdfPCell(new Phrase("VALOR DE ADQUISICIÓN", parrafo6));
            var c5 = new PdfPCell(new Phrase("VALOR DE TRANSACCIÓN", parrafo6));
            var c6 = new PdfPCell(new Phrase("VALOR ACTUAL", parrafo6));
            var c7 = new PdfPCell(new Phrase("DESCRIPCIÓN", parrafo6));
            //Agregamos a la tabla las celdas 
            tbl.AddCell(c1);
            tbl.AddCell(c2);
            tbl.AddCell(c3);
            tbl.AddCell(c4);
            tbl.AddCell(c5);
            tbl.AddCell(c6);
            tbl.AddCell(c7);

            //Extraemos de la base y llenamos las celdas
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                string fechaMin = "1-1-" + anio;
                string fechaMax = "31-12-" + anio;

                //  DateTime oDate = Convert.ToDateTime(fechaMax);
                DateTime uDate = DateTime.ParseExact(fechaMax, "dd-MM-yyyy", null);
                // oDate.ToString("dd-MM-yyyy");

                List<ActivoRevalorizadoAF> lista = (from activo in bd.ActivoFijo
                                                    join noFormulario in bd.FormularioIngreso
                                                    on activo.NoFormulario equals noFormulario.NoFormulario
                                                    join tarjeta in bd.TarjetaDepreciacion
                                                    on activo.IdBien equals tarjeta.IdBien
                                                    where (tarjeta.Fecha >= DateTime.Parse(fechaMin) && tarjeta.Fecha <= uDate)
                                                    && tarjeta.Concepto == "Revalorización"
                                                    orderby activo.IdBien
                                                    select new ActivoRevalorizadoAF
                                                    {
                                                        idBien = activo.IdBien,
                                                        codigo = activo.CorrelativoBien,
                                                        fecha = tarjeta.Fecha == null ? " " : ((DateTime)tarjeta.Fecha).ToString("dd-MM-yyyy"),
                                                        concepto = tarjeta.Concepto,
                                                        valorTransaccion = Math.Round((double)tarjeta.ValorTransaccion, 2),
                                                        valorActual = Math.Round((double)tarjeta.ValorActual, 2),
                                                        valorAdquirido=activo.ValorAdquicicion.ToString(),
                                                        descripcion = activo.Desripcion

                                                    }).ToList();

                  foreach (var activos in lista)
                   {
                       c1.Phrase = new Phrase(activos.fecha, parrafo5); 
                       c2.Phrase = new Phrase(activos.concepto, parrafo5);
                       c3.Phrase = new Phrase(activos.codigo, parrafo5);
                       c4.Phrase = new Phrase("$" + activos.valorAdquirido, parrafo5);
                       c5.Phrase = new Phrase("$" + activos.valorTransaccion, parrafo5);
                       c6.Phrase = new Phrase("$" + activos.valorActual, parrafo5);
                       c7.Phrase = new Phrase(activos.descripcion, parrafo5);
                //Agregamos a la tabla
                       tbl.AddCell(c1);
                       tbl.AddCell(c2);
                       tbl.AddCell(c3);
                       tbl.AddCell(c4);
                       tbl.AddCell(c5);
                       tbl.AddCell(c6);
                       tbl.AddCell(c7);
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
        //Fin reporte activos revalorizados por año

        //Reporte tarjeta

        [HttpGet]
        [Route("api/Reporte/tarjetaPdf/{idbien}")]
        public async Task<IActionResult> tarjetaPdf(int idbien)
        {
            Document doc = new Document(PageSize.Letter);
            doc.SetMargins(40f, 40f, 40f, 40f);
            MemoryStream ms = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);

            //Instanciamos la clase para el paginado y la fecha de impresión
            var pe = new PageEventHelper();
            writer.PageEvent = pe;

            doc.AddAuthor("Asgard");
            doc.AddTitle("Reporte tarjeta");
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
            doc.Add(new Paragraph("REPORTE TARJETA DE DEPRECIACIÓN", parrafo) { Alignment = Element.ALIGN_CENTER });

            //Espacio en blanco
            doc.Add(Chunk.Newline);

            doc.Add(new Paragraph("CONTROL DE EXISTENCIAS DE MOBILIARIO, EQUIPO E INSTALACIONES", parrafo2) { Alignment = Element.ALIGN_CENTER });
            doc.Add(Chunk.Newline);

            //Extraemos de la base y llenamos las celdas
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                TarjetaDatosAF odatos = new TarjetaDatosAF();
                ActivoFijo oactivo = bd.ActivoFijo.Where(p => p.IdBien == idbien).First();
                FormularioIngreso oformulario = bd.FormularioIngreso.Where(p => p.NoFormulario == oactivo.NoFormulario).First();
                odatos.fechaAdquicicion = oformulario.FechaIngreso == null ? " " : ((DateTime)oformulario.FechaIngreso).ToString("dd-MM-yyyy");
                odatos.Observaciones = oformulario.Observaciones;
                odatos.codigo = oactivo.CorrelativoBien;
                odatos.descripcion = oactivo.Desripcion;
                odatos.valor = oactivo.ValorAdquicicion.ToString();
                odatos.prima = oactivo.Prima.ToString();
                odatos.plazo = oactivo.PlazoPago;
                odatos.cuota = oactivo.CuotaAsignanda.ToString();
                odatos.interes = oactivo.Intereses.ToString();
                odatos.color = oactivo.Color;
                odatos.modelo = oactivo.Modelo;
                odatos.noSerie = oactivo.NoSerie;
                int tasa = (int)(100 / oactivo.VidaUtil);
                odatos.tasaAnual = tasa.ToString();
                odatos.vidaUtil = oactivo.VidaUtil.ToString();
                odatos.valorResidual = oactivo.ValorResidual.ToString();

                //Cuerpo de la tarjeta
                var tbl1 = new PdfPTable(new float[] { 7f, 5f, 5f, 10f, 3f, 7f }) { WidthPercentage = 100f };
                tbl1.AddCell(new PdfPCell(new Phrase("Fecha de adquisición: ", parrafo6)) { Border = 0, Rowspan = 2 });
                tbl1.AddCell(new PdfPCell(new Phrase(odatos.fechaAdquicicion, parrafo7)) { Border = 0 });
                tbl1.AddCell(new PdfPCell(new Phrase("Denominación: ", parrafo6)) { Border = 0, Rowspan = 2 });
                tbl1.AddCell(new PdfPCell(new Phrase(oactivo.Desripcion, parrafo7)) { Border = 0 });
                tbl1.AddCell(new PdfPCell(new Phrase("Código: ",parrafo6)) { Border = 0, Rowspan = 2 });
                tbl1.AddCell(new PdfPCell(new Phrase(oactivo.CorrelativoBien, parrafo7)) { Border = 0 });
                //*****************************************************

                var tbl2 = new PdfPTable(new float[] { 2f, 7f, 2f, 8f, 2f, 5f }) { WidthPercentage = 100f };
                if (oactivo.IdMarca != null)
                {
                    Marcas oMarcas = bd.Marcas.Where(p => p.IdMarca == oactivo.IdMarca).First();
                    odatos.marca = oMarcas.Marca;
                    tbl2.AddCell(new PdfPCell(new Phrase("Marca: ", parrafo6)) { Border = 0, Rowspan = 2 } );
                    tbl2.AddCell(new PdfPCell(new Phrase(oMarcas.Marca, parrafo7)) { Border = 0 } );
                }
                else
                {
                    odatos.marca = " ";
                }
                    tbl2.AddCell(new PdfPCell(new Phrase("Color: ", parrafo6)) { Border = 0, Rowspan = 2 } );
                    tbl2.AddCell(new PdfPCell(new Phrase(oactivo.Color, parrafo7)) { Border = 0 });


                    tbl2.AddCell(new PdfPCell(new Phrase("Modelo: ", parrafo6)) { Border = 0, Rowspan = 2 });
                    tbl2.AddCell(new PdfPCell(new Phrase(oactivo.Modelo, parrafo7)) { Border = 0 });
                //*****************************************************
                var tbl3 = new PdfPTable(new float[] { 4f, 6f, 3f, 8f, 3f, 5f }) { WidthPercentage = 100f };
                tbl3.AddCell(new PdfPCell(new Phrase("No de serie: ", parrafo6)) { Border = 0,Rowspan = 2 });
                tbl3.AddCell(new PdfPCell(new Phrase(odatos.noSerie, parrafo7)) { Border = 0 });
                var tbl4 = new PdfPTable(new float[] { 4f, 11f, 6f, 12f, 8f, 4f }) { WidthPercentage = 100f };
                if (oactivo.IdProveedor != null)
                {
                    Proveedor oProveedor = bd.Proveedor.Where(p => p.IdProveedor == oactivo.IdProveedor).First();
                    string ProvDon;
                    odatos.proveedor = oProveedor.Nombre;
                    odatos.direccion = oProveedor.Direccion;
                    odatos.telefono = oProveedor.Telefono;
                    odatos.isProvDon = 1;
                    if (odatos.isProvDon == 1)
                    {
                        ProvDon = "Proveedor:";
                    }
                    else
                    {
                        ProvDon = "Donante:";
                    }
                    tbl3.AddCell(new PdfPCell(new Phrase(ProvDon, parrafo6)) { Border = 0, Rowspan = 2 });
                    tbl3.AddCell(new PdfPCell(new Phrase(oProveedor.Nombre, parrafo7)) { Border = 0 });
                    tbl3.AddCell(new PdfPCell(new Phrase("Dirección: ", parrafo6)) { Border = 0, Rowspan = 2 });
                    tbl3.AddCell(new PdfPCell(new Phrase(oProveedor.Direccion, parrafo7)) { Border = 0 });
                    tbl4.AddCell(new PdfPCell(new Phrase("Teléfono:", parrafo6)) { Border = 0 });
                    tbl4.AddCell(new PdfPCell(new Phrase(oProveedor.Telefono, parrafo7)) { Border = 0 });
                }
                else
                {
                    Donantes oDonante = bd.Donantes.Where(p => p.IdDonante == oactivo.IdDonante).First();
                    string ProvDon;
                    odatos.proveedor = oDonante.Nombre;
                    odatos.direccion = oDonante.Direccion;
                    odatos.telefono = oDonante.Telefono;
                    odatos.isProvDon = 2;
                    if (odatos.isProvDon == 1)
                    {
                        ProvDon = "Proveedor:";
                    }
                    else
                    {
                        ProvDon = "Donante:";
                    }
                    tbl3.AddCell(new PdfPCell(new Phrase(ProvDon, parrafo6)) { Border = 0, Rowspan = 2 });
                    tbl3.AddCell(new PdfPCell(new Phrase(oDonante.Nombre, parrafo7)) { Border = 0 });
                    tbl1.AddCell(new PdfPCell(new Phrase("Dirección: ", parrafo6)) { Border = 0, Rowspan = 2 });
                    tbl3.AddCell(new PdfPCell(new Phrase(oDonante.Direccion, parrafo7)) { Border = 0 });
                    tbl4.AddCell(new PdfPCell(new Phrase("Teléfono:", parrafo6)) { Border = 0 });
                    tbl4.AddCell(new PdfPCell(new Phrase(oDonante.Telefono, parrafo7)) { Border = 0 });
                }
                    tbl4.AddCell(new PdfPCell(new Phrase("Más interes del: ", parrafo6)) { Border = 0 } );
                    tbl4.AddCell(new PdfPCell(new Phrase(odatos.interes + "%", parrafo7)) { Border = 0 });
                    tbl4.AddCell(new PdfPCell(new Phrase("Vida util según referencia de fábrica: ", parrafo6)) { Border = 0 } );
                    tbl4.AddCell(new PdfPCell(new Phrase(odatos.vidaUtil + " años", parrafo7)) { Border = 0 });

                var tbl5 = new PdfPTable(new float[] { 9f, 3f, 5f, 10f, 5f, 5f }) { WidthPercentage = 100f };
                    tbl5.AddCell(new PdfPCell(new Phrase("Tasa anual de depreciación: ", parrafo6)) { Border = 0 , Rowspan = 2 });
                    tbl5.AddCell(new PdfPCell(new Phrase(odatos.tasaAnual + "%", parrafo7)) { Border = 0 });
                    tbl5.AddCell(new PdfPCell(new Phrase("Valor residual: ", parrafo6)) { Border = 0 , Rowspan = 2 });
                    tbl5.AddCell(new PdfPCell(new Phrase(odatos.valorResidual, parrafo7)) { Border = 0 });
                    tbl5.AddCell(new PdfPCell(new Phrase("Observaciones: ", parrafo6)) { Border = 0 , Rowspan = 2 });
                    tbl5.AddCell(new PdfPCell(new Phrase(odatos.Observaciones, parrafo7)) { Border = 0 });

                doc.Add(tbl1);
                doc.Add(tbl2);
                doc.Add(tbl3);
                doc.Add(tbl4);
                doc.Add(tbl5);

                //Tabla de transacciones
                doc.Add(new Paragraph("MOVIMIENTO DEL VALOR - DEPRECIACIONES - GASTOS DE CONSEVACIÓN", parrafo2) { Alignment = Element.ALIGN_CENTER });
                doc.Add(Chunk.Newline);

                //Para el cuadro de transacciones
                //Agregamos una tabla
                var tbl = new PdfPTable(new float[] { 11f, 12f, 10f, 26f, 15f, 15f, 11f }) { WidthPercentage = 100f };
                var c1 = new PdfPCell(new Phrase("FECHA", parrafo8));
                var c2 = new PdfPCell(new Phrase("CONCEPTO", parrafo8));
                var c3 = new PdfPCell(new Phrase("VALOR", parrafo8));
                var c4 = new PdfPCell(new Phrase("VALOR DE ADQUISICIÓN Y MEJORAS", parrafo8));
                var c5 = new PdfPCell(new Phrase("DEPRECIACIÓN ANUAL", parrafo8));
                var c6 = new PdfPCell(new Phrase("DEPRECIACIÓN ACUMULADA", parrafo8));
                var c7 = new PdfPCell(new Phrase("VALOR ACTUAL", parrafo8));
                //Agregamos a la tabla las celdas 
                tbl.AddCell(c1);
                tbl.AddCell(c2);
                tbl.AddCell(c3);
                tbl.AddCell(c4);
                tbl.AddCell(c5);
                tbl.AddCell(c6);
                tbl.AddCell(c7);

                IEnumerable<TarjetaTransaccionesAF> ListaTransacciones = (from tarjeta in bd.TarjetaDepreciacion
                                                                          where tarjeta.IdBien == idbien
                                                                          orderby tarjeta.IdTarjeta
                                                                          select new TarjetaTransaccionesAF
                                                                          {
                                                                              id = tarjeta.IdTarjeta,
                                                                              idBien = (int)tarjeta.IdBien,
                                                                              fecha = tarjeta.Fecha == null ? " " : ((DateTime)tarjeta.Fecha).ToString("dd-MM-yyyy"),
                                                                              concepto = tarjeta.Concepto,
                                                                              montoTransaccion = Math.Round((double)tarjeta.Valor, 2),
                                                                              depreciacionAnual = Math.Round((double)tarjeta.DepreciacionAnual, 2),
                                                                              depreciacionAcumulada = Math.Round((double)tarjeta.DepreciacionAcumulada, 2),
                                                                              valorActual = Math.Round((double)tarjeta.ValorActual, 2),
                                                                              valorTransaccion = Math.Round((double)tarjeta.ValorTransaccion, 2)
                                                                          }).ToList();

                foreach (var tarjeta in ListaTransacciones)
                {
                    c1.Phrase = new Phrase(tarjeta.fecha, parrafo5);
                    c2.Phrase = new Phrase(tarjeta.concepto, parrafo5);

                    c3.Phrase = new Phrase("$ " + tarjeta.valorTransaccion.ToString(), parrafo5);

                    c4.Phrase = new Phrase("$ " + tarjeta.montoTransaccion.ToString(), parrafo5);

                    c5.Phrase = new Phrase("$ " + tarjeta.depreciacionAnual.ToString(), parrafo5);

                    c6.Phrase = new Phrase("$ " + tarjeta.depreciacionAcumulada.ToString(), parrafo5);

                    c7.Phrase = new Phrase("$ " + tarjeta.valorActual.ToString(), parrafo5);
                    //Agregamos a la tabla
                    tbl.AddCell(c1);
                    tbl.AddCell(c2);
                    tbl.AddCell(c3);
                    tbl.AddCell(c4);
                    tbl.AddCell(c5);
                    tbl.AddCell(c6);
                    tbl.AddCell(c7);
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
        //Fin reporte tarjeta

        //Reporte cuadro de control activos asignados

        [HttpGet]
        [Route("api/Reporte/cuadroControlActivosPdf")]
        public async Task<IActionResult> cuadroControlActivosPdf()
        {
            Document doc = new Document(PageSize.Letter);
            doc.SetMargins(40f, 40f, 40f, 40f);
            MemoryStream ms = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);

            //Instanciamos la clase para el paginado y la fecha de impresión
            var pe = new PageEventHelper();
            writer.PageEvent = pe;

            doc.AddAuthor("Asgard");
            doc.AddTitle("Reporte cuadro de control");
            doc.Open();

            //Inicia cuerpo del reporte

            //Estilo y fuente personalizada
            BaseFont fuente = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo = new iTextSharp.text.Font(fuente, 12f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente2 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo2 = new iTextSharp.text.Font(fuente2, 7.5f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente3 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo3 = new iTextSharp.text.Font(fuente3, 15f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente4 = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo4 = new iTextSharp.text.Font(fuente4, 11f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));

            //Encabezado
            BaseFont fuenteE2 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafoE2 = new iTextSharp.text.Font(fuente2, 11f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));

            //Para las celdas
            BaseFont fuente5 = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);
            iTextSharp.text.Font parrafo5 = new iTextSharp.text.Font(fuente5, 8f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));

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
                tbl1.AddCell(new PdfPCell(new Phrase(oCooperativa.Descripcion.ToUpper(), parrafoE2)) { Border = 0, HorizontalAlignment = 1 });
                tbl1.AddCell(new PdfPCell(new Phrase(oCooperativa.Nombre.ToUpper(), parrafo3)) { Border = 0, HorizontalAlignment = 1 });
                doc.Add(tbl1);
                doc.Add(new Phrase("\n"));
            }
            doc.Add(new Phrase("\n"));
            //Línea separadora
            Chunk linea = new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(1f, 100f, BaseColor.Black, Element.ALIGN_CENTER, 1f));
            doc.Add(linea);
            doc.Add(new Paragraph("REPORTE DE CUADRO DE CONTROL DE BIENES DE LA PROPIEDAD PLANTA Y EQUIPO", parrafo) { Alignment = Element.ALIGN_CENTER });

            //Espacio en blanco
            doc.Add(Chunk.Newline);

            //Agregamos una tabla
            var tbl = new PdfPTable(new float[] { 10f, 15f, 11f, 11f, 12f, 12f, 8f, 10f, 13f }) { WidthPercentage = 100f };
            var c1 = new PdfPCell(new Phrase("CÓDIGO", parrafo2));
            var c2 = new PdfPCell(new Phrase("DESCRIPCIÓN", parrafo2));
            var c3 = new PdfPCell(new Phrase("FECHA DE ADQUISICIÓN", parrafo2));
            var c4 = new PdfPCell(new Phrase("VALOR DE ADQUISICIÓN", parrafo2));
            var c5 = new PdfPCell(new Phrase("DEPRECIACIÓN", parrafo2));
            var c6 = new PdfPCell(new Phrase("DEPRECIACIÓN ACUMULADA", parrafo2));
            var c7 = new PdfPCell(new Phrase("VALOR ACTUAL", parrafo2));
            var c8 = new PdfPCell(new Phrase("UBICACIÓN", parrafo2));
            var c9 = new PdfPCell(new Phrase("RESPONSABLE", parrafo2));

            //Agregamos a la tabla las celdas 
            tbl.AddCell(c1);
            tbl.AddCell(c2);
            tbl.AddCell(c3);
            tbl.AddCell(c4);
            tbl.AddCell(c5);
            tbl.AddCell(c6);
            tbl.AddCell(c7);
            tbl.AddCell(c8);
            tbl.AddCell(c9);

            //Extraemos de la base y llenamos las celdas
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<CuadroControlAF> listacuadro = (
                                                             from tarjeta in bd.TarjetaDepreciacion
                                                             group tarjeta by tarjeta.IdBien into bar
                                                             join cuadro in bd.ActivoFijo
                                                             on bar.FirstOrDefault().IdBien equals cuadro.IdBien
                                                             join noFormulario in bd.FormularioIngreso
                                                             on cuadro.NoFormulario equals noFormulario.NoFormulario
                                                             join clasif in bd.Clasificacion
                                                             on cuadro.IdClasificacion equals clasif.IdClasificacion
                                                             join resposable in bd.Empleado
                                                             on cuadro.IdResponsable equals resposable.IdEmpleado
                                                             join area in bd.AreaDeNegocio
                                                             on resposable.IdAreaDeNegocio equals area.IdAreaNegocio

                                                              //where cuadro.EstadoActual == 1 && cuadro.EstaAsignado == 1
                                                              select new CuadroControlAF()
                                                             {
                                                                 idbien = cuadro.IdBien,
                                                                 codigo = cuadro.CorrelativoBien,
                                                                 descripcion = cuadro.Desripcion,
                                                                 valoradquisicion = (double)cuadro.ValorAdquicicion,
                                                                 fechaadquisicion = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                                                 valoractual = Math.Round(((double)bar.OrderByDescending(x => x.IdTarjeta).First().ValorActual), 2),
                                                                 depreciacion = Math.Round(((double)bar.OrderByDescending(x => x.IdTarjeta).First().DepreciacionAnual), 2),
                                                                 depreciacionacumulada = Math.Round(((double)bar.Sum(x => x.DepreciacionAnual)), 2),
                                                                 ubicacion = area.Nombre,
                                                                 responsable = resposable.Nombres + " " + resposable.Apellidos

                                                             }).ToList();

                foreach (var cuadro in listacuadro)
                {
                    c1.Phrase = new Phrase(cuadro.codigo, parrafo5);
                    c2.Phrase = new Phrase(cuadro.descripcion, parrafo5);
                    c3.Phrase = new Phrase(cuadro.fechaadquisicion, parrafo5);
                    c4.Phrase = new Phrase("$" + cuadro.valoradquisicion.ToString(), parrafo5);
                    c5.Phrase = new Phrase("$" + cuadro.depreciacion.ToString(), parrafo5);
                    c6.Phrase = new Phrase("$" + cuadro.depreciacionacumulada.ToString(), parrafo5);
                    c7.Phrase = new Phrase("$" + cuadro.valoractual.ToString(), parrafo5);
                    c8.Phrase = new Phrase(cuadro.ubicacion, parrafo5);
                    c9.Phrase = new Phrase(cuadro.responsable, parrafo5);
                    //Agregamos a la tabla
                    tbl.AddCell(c1);
                    tbl.AddCell(c2);
                    tbl.AddCell(c3);
                    tbl.AddCell(c4);
                    tbl.AddCell(c5);
                    tbl.AddCell(c6);
                    tbl.AddCell(c7);
                    tbl.AddCell(c8);
                    tbl.AddCell(c9);
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

        //Fin reporte cuadro de control activos asignados

        //Reporte cuadro de control de Jefe
        [HttpGet]
        [Route("api/Reporte/cuadroControlJefePdf/{idJefe}")]
        public async Task<IActionResult> cuadroControlJefePdf(int idJefe)
        {
            Document doc = new Document(PageSize.Letter);
            doc.SetMargins(40f, 40f, 40f, 40f);
            MemoryStream ms = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);

            //Instanciamos la clase para el paginado y la fecha de impresión
            var pe = new PageEventHelper();
            writer.PageEvent = pe;

            doc.AddAuthor("Asgard");
            doc.AddTitle("Reporte cuadro de control");
            doc.Open();

            //Inicia cuerpo del reporte

            //Estilo y fuente personalizada
            BaseFont fuente = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo = new iTextSharp.text.Font(fuente, 12f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente2 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo2 = new iTextSharp.text.Font(fuente2, 7.5f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente3 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo3 = new iTextSharp.text.Font(fuente3, 15f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente4 = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo4 = new iTextSharp.text.Font(fuente4, 11f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));

            //Encabezado
            BaseFont fuenteE2 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafoE2 = new iTextSharp.text.Font(fuente2, 11f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));

            //Para las celdas
            BaseFont fuente5 = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);
            iTextSharp.text.Font parrafo5 = new iTextSharp.text.Font(fuente5, 8f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));

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
                tbl1.AddCell(new PdfPCell(new Phrase(oCooperativa.Descripcion.ToUpper(), parrafoE2)) { Border = 0, HorizontalAlignment = 1 });
                tbl1.AddCell(new PdfPCell(new Phrase(oCooperativa.Nombre.ToUpper(), parrafo3)) { Border = 0, HorizontalAlignment = 1 });
                doc.Add(tbl1);
                doc.Add(new Phrase("\n"));
            }
            doc.Add(new Phrase("\n"));
            //Línea separadora
            Chunk linea = new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(1f, 100f, BaseColor.Black, Element.ALIGN_CENTER, 1f));
            doc.Add(linea);
            doc.Add(new Paragraph("REPORTE DE CUADRO DE CONTROL DE BIENES DE LA PROPIEDAD PLANTA Y EQUIPO", parrafo) { Alignment = Element.ALIGN_CENTER });

            //Espacio en blanco
            doc.Add(Chunk.Newline);

            //Agregamos una tabla
            var tbl = new PdfPTable(new float[] { 10f, 15f, 11f, 11f, 12f, 12f, 8f, 10f, 13f }) { WidthPercentage = 100f };
            var c1 = new PdfPCell(new Phrase("CÓDIGO", parrafo2));
            var c2 = new PdfPCell(new Phrase("DESCRIPCIÓN", parrafo2));
            var c3 = new PdfPCell(new Phrase("FECHA DE ADQUISICIÓN", parrafo2));
            var c4 = new PdfPCell(new Phrase("VALOR DE ADQUISICIÓN", parrafo2));
            var c5 = new PdfPCell(new Phrase("DEPRECIACIÓN", parrafo2));
            var c6 = new PdfPCell(new Phrase("DEPRECIACIÓN ACUMULADA", parrafo2));
            var c7 = new PdfPCell(new Phrase("VALOR ACTUAL", parrafo2));
            var c8 = new PdfPCell(new Phrase("UBICACIÓN", parrafo2));
            var c9 = new PdfPCell(new Phrase("RESPONSABLE", parrafo2));

            //Agregamos a la tabla las celdas 
            tbl.AddCell(c1);
            tbl.AddCell(c2);
            tbl.AddCell(c3);
            tbl.AddCell(c4);
            tbl.AddCell(c5);
            tbl.AddCell(c6);
            tbl.AddCell(c7);
            tbl.AddCell(c8);
            tbl.AddCell(c9);

            //Extraemos de la base y llenamos las celdas
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                Empleado oempleado = bd.Empleado.Where(p => p.IdEmpleado == idJefe).FirstOrDefault();
                AreaDeNegocio oarea = bd.AreaDeNegocio.Where(p => p.IdAreaNegocio == oempleado.IdAreaDeNegocio).FirstOrDefault();
                IEnumerable<CuadroControlAF> listacuadro = (
                                                              from tarjeta in bd.TarjetaDepreciacion
                                                              group tarjeta by tarjeta.IdBien into bar
                                                              join cuadro in bd.ActivoFijo
                                                              on bar.FirstOrDefault().IdBien equals cuadro.IdBien
                                                              join noFormulario in bd.FormularioIngreso
                                                              on cuadro.NoFormulario equals noFormulario.NoFormulario
                                                              join clasif in bd.Clasificacion
                                                              on cuadro.IdClasificacion equals clasif.IdClasificacion
                                                              join resposable in bd.Empleado
                                                              on cuadro.IdResponsable equals resposable.IdEmpleado
                                                              join area in bd.AreaDeNegocio
                                                              on resposable.IdAreaDeNegocio equals area.IdAreaNegocio
                                                              where area.IdAreaNegocio == oarea.IdAreaNegocio
                                                              //where cuadro.EstadoActual == 1 && cuadro.EstaAsignado == 1
                                                              select new CuadroControlAF()
                                                              {
                                                                  idbien = cuadro.IdBien,
                                                                  codigo = cuadro.CorrelativoBien,
                                                                  descripcion = cuadro.Desripcion,
                                                                  valoradquisicion = (double)cuadro.ValorAdquicicion,
                                                                  fechaadquisicion = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                                                  valoractual = Math.Round(((double)bar.OrderByDescending(x => x.IdTarjeta).First().ValorActual), 2),
                                                                  depreciacion = Math.Round(((double)bar.OrderByDescending(x => x.IdTarjeta).First().DepreciacionAnual), 2),
                                                                  depreciacionacumulada = Math.Round(((double)bar.Sum(x => x.DepreciacionAnual)), 2),
                                                                  ubicacion = area.Nombre,
                                                                  responsable = resposable.Nombres + " " + resposable.Apellidos

                                                              }).ToList();

                foreach (var cuadro in listacuadro)
                {
                    c1.Phrase = new Phrase(cuadro.codigo, parrafo5);
                    c2.Phrase = new Phrase(cuadro.descripcion, parrafo5);
                    c3.Phrase = new Phrase(cuadro.fechaadquisicion, parrafo5);
                    c4.Phrase = new Phrase("$" + cuadro.valoradquisicion.ToString(), parrafo5);
                    c5.Phrase = new Phrase("$" + cuadro.depreciacion.ToString(), parrafo5);
                    c6.Phrase = new Phrase("$" + cuadro.depreciacionacumulada.ToString(), parrafo5);
                    c7.Phrase = new Phrase("$" + cuadro.valoractual.ToString(), parrafo5);
                    c8.Phrase = new Phrase(cuadro.ubicacion, parrafo5);
                    c9.Phrase = new Phrase(cuadro.responsable, parrafo5);
                    //Agregamos a la tabla
                    tbl.AddCell(c1);
                    tbl.AddCell(c2);
                    tbl.AddCell(c3);
                    tbl.AddCell(c4);
                    tbl.AddCell(c5);
                    tbl.AddCell(c6);
                    tbl.AddCell(c7);
                    tbl.AddCell(c8);
                    tbl.AddCell(c9);
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
        //Fin cuadro de control de Jefe

        //Reporte cuadro de control de edificios

        [HttpGet]
        [Route("api/Reporte/cuadroControlEdificiosPdf")]
        public async Task<IActionResult> cuadroControlEdificiosPdf()
        {
            Document doc = new Document(PageSize.Letter);
            doc.SetMargins(40f, 40f, 40f, 40f);
            MemoryStream ms = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);

            //Instanciamos la clase para el paginado y la fecha de impresión
            var pe = new PageEventHelper();
            writer.PageEvent = pe;

            doc.AddAuthor("Asgard");
            doc.AddTitle("Reporte cuadro de control");
            doc.Open();

            //Inicia cuerpo del reporte

            //Estilo y fuente personalizada
            BaseFont fuente = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo = new iTextSharp.text.Font(fuente, 12f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente2 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo2 = new iTextSharp.text.Font(fuente2, 8f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente3 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo3 = new iTextSharp.text.Font(fuente3, 15f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente4 = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo4 = new iTextSharp.text.Font(fuente4, 11f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));

            //Encabezado
            BaseFont fuenteE2 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafoE2 = new iTextSharp.text.Font(fuente2, 11f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));

            //Para las celdas
            BaseFont fuente5 = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);
            iTextSharp.text.Font parrafo5 = new iTextSharp.text.Font(fuente5, 8f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));

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
                tbl1.AddCell(new PdfPCell(new Phrase(oCooperativa.Descripcion.ToUpper(), parrafoE2)) { Border = 0, HorizontalAlignment = 1 });
                tbl1.AddCell(new PdfPCell(new Phrase(oCooperativa.Nombre.ToUpper(), parrafo3)) { Border = 0, HorizontalAlignment = 1 });
                doc.Add(tbl1);
                doc.Add(new Phrase("\n"));
            }
            doc.Add(new Phrase("\n"));
            //Línea separadora
            Chunk linea = new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(1f, 100f, BaseColor.Black, Element.ALIGN_CENTER, 1f));
            doc.Add(linea);
            doc.Add(new Paragraph("REPORTE DE CUADRO DE CONTROL DE BIENES DE LA PROPIEDAD PLANTA Y EQUIPO", parrafo) { Alignment = Element.ALIGN_CENTER });

            //Espacio en blanco
            doc.Add(Chunk.Newline);

            //Agregamos una tabla
            var tbl = new PdfPTable(new float[] { 12f, 15f, 11f, 11f, 12f, 12f, 8f }) { WidthPercentage = 100f };
            var c1 = new PdfPCell(new Phrase("CÓDIGO", parrafo2));
            var c2 = new PdfPCell(new Phrase("DESCRIPCIÓN", parrafo2));
            var c3 = new PdfPCell(new Phrase("FECHA DE ADQUISICIÓN", parrafo2));
            var c4 = new PdfPCell(new Phrase("VALOR DE ADQUISICIÓN", parrafo2));
            var c5 = new PdfPCell(new Phrase("DEPRECIACIÓN", parrafo2));
            var c6 = new PdfPCell(new Phrase("DEPRECIACIÓN ACUMULADA", parrafo2));
            var c7 = new PdfPCell(new Phrase("VALOR ACTUAL", parrafo2));

            //Agregamos a la tabla las celdas 
            tbl.AddCell(c1);
            tbl.AddCell(c2);
            tbl.AddCell(c3);
            tbl.AddCell(c4);
            tbl.AddCell(c5);
            tbl.AddCell(c6);
            tbl.AddCell(c7);

            //Extraemos de la base y llenamos las celdas
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<CuadroControlAF> listacuadro = (
                                                               from tarjeta in bd.TarjetaDepreciacion
                                                               group tarjeta by tarjeta.IdBien into bar
                                                               join cuadro in bd.ActivoFijo
                                                               on bar.FirstOrDefault().IdBien equals cuadro.IdBien
                                                               join noFormulario in bd.FormularioIngreso
                                                               on cuadro.NoFormulario equals noFormulario.NoFormulario
                                                               join clasif in bd.Clasificacion
                                                               on cuadro.IdClasificacion equals clasif.IdClasificacion
                                                               where (cuadro.EstadoActual == 1 || cuadro.EstadoActual == 2 || cuadro.EstadoActual == 3) && cuadro.TipoActivo == 1 && cuadro.EstaAsignado == 1
                                                               select new CuadroControlAF()
                                                               {
                                                                   idbien = cuadro.IdBien,
                                                                   codigo = cuadro.CorrelativoBien,
                                                                   descripcion = cuadro.Desripcion,
                                                                   valoradquisicion = (double)cuadro.ValorAdquicicion,
                                                                   fechaadquisicion = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                                                   valoractual = Math.Round(((double)bar.OrderByDescending(x => x.IdTarjeta).First().ValorActual), 2),
                                                                   depreciacion = Math.Round(((double)bar.OrderByDescending(x => x.IdTarjeta).First().DepreciacionAnual), 2),
                                                                   depreciacionacumulada = Math.Round(((double)bar.Sum(x => x.DepreciacionAnual)), 2),

                                                               }).ToList();

                foreach (var cuadro in listacuadro)
                {
                    c1.Phrase = new Phrase(cuadro.codigo, parrafo5);
                    c2.Phrase = new Phrase(cuadro.descripcion, parrafo5);
                    c3.Phrase = new Phrase(cuadro.fechaadquisicion, parrafo5);
                    c4.Phrase = new Phrase("$" + cuadro.valoradquisicion.ToString(), parrafo5);
                    c5.Phrase = new Phrase("$" + cuadro.depreciacion.ToString(), parrafo5);
                    c6.Phrase = new Phrase("$" + cuadro.depreciacionacumulada.ToString(), parrafo5);
                    c7.Phrase = new Phrase("$" + cuadro.valoractual.ToString(), parrafo5);
                    //Agregamos a la tabla
                    tbl.AddCell(c1);
                    tbl.AddCell(c2);
                    tbl.AddCell(c3);
                    tbl.AddCell(c4);
                    tbl.AddCell(c5);
                    tbl.AddCell(c6);
                    tbl.AddCell(c7);
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

        //Fin reporte cuadro de control de edificios

        //Reporte cuadro de control intangibles



        [HttpGet]
        [Route("api/Reporte/cuadroControlIntangiblesPdf")]
        public async Task<IActionResult> cuadroControlIntangiblesPdf()
        {
            Document doc = new Document(PageSize.Letter);
            doc.SetMargins(40f, 40f, 40f, 40f);
            MemoryStream ms = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);

            //Instanciamos la clase para el paginado y la fecha de impresión
            var pe = new PageEventHelper();
            writer.PageEvent = pe;

            doc.AddAuthor("Asgard");
            doc.AddTitle("Reporte cuadro de control");
            doc.Open();

            //Inicia cuerpo del reporte

            //Estilo y fuente personalizada
            BaseFont fuente = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo = new iTextSharp.text.Font(fuente, 12f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente2 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo2 = new iTextSharp.text.Font(fuente2, 8f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente3 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo3 = new iTextSharp.text.Font(fuente3, 15f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente4 = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo4 = new iTextSharp.text.Font(fuente4, 11f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));

            //Encabezado
            BaseFont fuenteE2 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafoE2 = new iTextSharp.text.Font(fuente2, 11f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));

            //Para las celdas
            BaseFont fuente5 = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);
            iTextSharp.text.Font parrafo5 = new iTextSharp.text.Font(fuente5, 8f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));

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
                tbl1.AddCell(new PdfPCell(new Phrase(oCooperativa.Descripcion.ToUpper(), parrafoE2)) { Border = 0, HorizontalAlignment = 1 });
                tbl1.AddCell(new PdfPCell(new Phrase(oCooperativa.Nombre.ToUpper(), parrafo3)) { Border = 0, HorizontalAlignment = 1 });
                doc.Add(tbl1);
                doc.Add(new Phrase("\n"));
            }
            doc.Add(new Phrase("\n"));
            //Línea separadora
            Chunk linea = new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(1f, 100f, BaseColor.Black, Element.ALIGN_CENTER, 1f));
            doc.Add(linea);
            doc.Add(new Paragraph("REPORTE DE CUADRO DE CONTROL DE BIENES DE LA PROPIEDAD PLANTA Y EQUIPO", parrafo) { Alignment = Element.ALIGN_CENTER });

            //Espacio en blanco
            doc.Add(Chunk.Newline);

            //Agregamos una tabla
            var tbl = new PdfPTable(new float[] { 12f, 15f, 11f, 11f, 12f, 12f, 8f }) { WidthPercentage = 100f };
            var c1 = new PdfPCell(new Phrase("CÓDIGO", parrafo2));
            var c2 = new PdfPCell(new Phrase("DESCRIPCIÓN", parrafo2));
            var c3 = new PdfPCell(new Phrase("FECHA DE ADQUISICIÓN", parrafo2));
            var c4 = new PdfPCell(new Phrase("VALOR DE ADQUISICIÓN", parrafo2));
            var c5 = new PdfPCell(new Phrase("DEPRECIACIÓN", parrafo2));
            var c6 = new PdfPCell(new Phrase("DEPRECIACIÓN ACUMULADA", parrafo2));
            var c7 = new PdfPCell(new Phrase("VALOR ACTUAL", parrafo2));

            //Agregamos a la tabla las celdas 
            tbl.AddCell(c1);
            tbl.AddCell(c2);
            tbl.AddCell(c3);
            tbl.AddCell(c4);
            tbl.AddCell(c5);
            tbl.AddCell(c6);
            tbl.AddCell(c7);

            //Extraemos de la base y llenamos las celdas
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<CuadroControlAF> listacuadro = (
                                                             from tarjeta in bd.TarjetaDepreciacion
                                                             group tarjeta by tarjeta.IdBien into bar
                                                             join cuadro in bd.ActivoFijo
                                                             on bar.FirstOrDefault().IdBien equals cuadro.IdBien
                                                             join noFormulario in bd.FormularioIngreso
                                                             on cuadro.NoFormulario equals noFormulario.NoFormulario
                                                             join clasif in bd.Clasificacion
                                                             on cuadro.IdClasificacion equals clasif.IdClasificacion
                                                             where (cuadro.EstadoActual == 1 || cuadro.EstadoActual == 2 || cuadro.EstadoActual == 3) && cuadro.TipoActivo == 3 && cuadro.EstaAsignado == 1
                                                             select new CuadroControlAF()
                                                             {
                                                                 idbien = cuadro.IdBien,
                                                                 codigo = cuadro.CorrelativoBien,
                                                                 descripcion = cuadro.Desripcion,
                                                                 valoradquisicion = (double)cuadro.ValorAdquicicion,
                                                                 fechaadquisicion = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                                                 valoractual = Math.Round(((double)bar.OrderByDescending(x => x.IdTarjeta).First().ValorActual), 2),
                                                                 depreciacion = Math.Round(((double)bar.OrderByDescending(x => x.IdTarjeta).First().DepreciacionAnual), 2),
                                                                 depreciacionacumulada = Math.Round(((double)bar.Sum(x => x.DepreciacionAnual)), 2),

                                                             }).ToList();

                foreach (var cuadro in listacuadro)
                {
                    c1.Phrase = new Phrase(cuadro.codigo, parrafo5);
                    c2.Phrase = new Phrase(cuadro.descripcion, parrafo5);
                    c3.Phrase = new Phrase(cuadro.fechaadquisicion, parrafo5);
                    c4.Phrase = new Phrase("$" + cuadro.valoradquisicion.ToString(), parrafo5);
                    c5.Phrase = new Phrase("$" + cuadro.depreciacion.ToString(), parrafo5);
                    c6.Phrase = new Phrase("$" + cuadro.depreciacionacumulada.ToString(), parrafo5);
                    c7.Phrase = new Phrase("$" + cuadro.valoractual.ToString(), parrafo5);
                    //Agregamos a la tabla
                    tbl.AddCell(c1);
                    tbl.AddCell(c2);
                    tbl.AddCell(c3);
                    tbl.AddCell(c4);
                    tbl.AddCell(c5);
                    tbl.AddCell(c6);
                    tbl.AddCell(c7);
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

        //Fin reporte cuadro de control intangibles
    }
}
