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
                oCooperativaAF.logo = oCooperativa.Logo;

                //Se agrega el encabezado
                doc.Add(new Paragraph(oCooperativa.Descripcion.ToUpper(), parrafo2) { Alignment = Element.ALIGN_CENTER });
                doc.Add(new Paragraph(oCooperativa.Nombre, parrafo3) { Alignment = Element.ALIGN_CENTER });
            }

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
                oCooperativaAF.logo = oCooperativa.Logo;

                //Se agrega el encabezado
                doc.Add(new Paragraph(oCooperativa.Descripcion.ToUpper(), parrafo2) { Alignment = Element.ALIGN_CENTER });
                doc.Add(new Paragraph(oCooperativa.Nombre, parrafo3) { Alignment = Element.ALIGN_CENTER });
            }

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
                oCooperativaAF.logo = oCooperativa.Logo;

                //Se agrega el encabezado
                doc.Add(new Paragraph(oCooperativa.Descripcion.ToUpper(), parrafo2) { Alignment = Element.ALIGN_CENTER });
                doc.Add(new Paragraph(oCooperativa.Nombre, parrafo3) { Alignment = Element.ALIGN_CENTER });
            }

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
                oCooperativaAF.logo = oCooperativa.Logo;

                //Se agrega el encabezado
                doc.Add(new Paragraph(oCooperativa.Descripcion.ToUpper(), parrafo2) { Alignment = Element.ALIGN_CENTER });
                doc.Add(new Paragraph(oCooperativa.Nombre, parrafo3) { Alignment = Element.ALIGN_CENTER });
            }

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
    }
}
